using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Author: Hwan Kim
///
/// This is a core class for Hack and Slash RPG style character action control.
/// This class is an base class so that it can be extended into many type of character controls.
/// In the example scene, this class is extended as PlayerControl and Enemy to perform different type of character action.
/// This class provides moving, attacking, damaging and dying actions.
/// Dying action will trigger the rag doll so that will do more realistic physics driven reaction of dying animation.
/// </summary>
[RequireComponent(typeof(CharacterEquipmentsV2))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class BaseCharacterController : MonoBehaviour
{
    public EquipmentController equipmentContoller;

    private CharacterEquipmentsV2 _currentEquipment;

    public CharacterEquipmentsV2 currentEquipment
    {
        get
        {
            return _currentEquipment;
        }
    }

    public RPGCharacter characterInfo = new RPGCharacter();
    public Transform myRootBone;
    public bool rootBoneInit = false;
    protected Animator thisAni;
    public Transform myTransform;
    public bool animatorInitialized = false;
    protected bool initialized = false;
    public bool targetLockOn = false;
    public int deadBodyLayer;
    private Rigidbody[] ragDollParts;
    private Collider[] ragDollCols;

    /// <summary>
    /// Modified by Hwan Kim.
    /// This will switch between Animator and Ragdoll.
    /// Author: Perttu Hämäläinen
    /// Source : http://perttuh.blogspot.fi/2013/10/unity-mecanim-and-ragdolls.html
    ///          https://docs.google.com/file/d/0B8lAtheEAvgmYUlqX1FjNm84cVU/edit?usp=sharing
    /// </summary>
    public bool ragdolled
    {
        set
        {
            if (value == true)
            {
                gameObject.layer = deadBodyLayer;
                //Transition from animated to ragdolled
                thisAni.enabled = false; //disable animation
                setKinematic(false); //allow the ragdoll RigidBodies to react to the environment
            }
            else
            {
                //Transition from ragdolled to animated through the blendToAnim state
                setKinematic(true); //disable gravity etc.
                thisAni.enabled = true; //enable animation
            }
        } //set
    }

    private Transform chestBornTran;
    private bool setChest = false;

    //A helper function to set the isKinematc property of all RigidBodies in the children of the
    //game object that this script is attached to
    protected void setKinematic(bool newValue)
    {
        //For each of the components in the array, treat the component as a Rigidbody and set its isKinematic property
        foreach (Rigidbody rd in ragDollParts)
        {
            if (!setChest && rd.name.Equals("chest"))
            {
                //get the chest born for dead effect(spawning bleeding effect, etc).
                chestBornTran = rd.transform;
                setChest = true;
            }
            rd.isKinematic = newValue;
        }
        foreach (Collider col in ragDollCols)
        {
            col.enabled = !newValue;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //Get an array of components that are of type Rigidbody
        ragDollParts = GetComponentsInChildren<Rigidbody>();
        ragDollCols = GetComponentsInChildren<Collider>();
        setKinematic(true);
        myTransform = transform;
        thisAni = GetComponent<Animator>();
        if (thisAni != null)
        {
            animatorInitialized = true;
            _currentEquipment = GetComponent<CharacterEquipmentsV2>();
            init();
        }
        else
        {
            Debug.LogError("Something weird happen, it should always have animator. Deleting this Game Object");
            Destroy(gameObject);
        }
    }

    protected virtual void init()
    {
    }

    public void setWeaponTypeAnimation(CharacterEquipmentsV2.WeaponType type)
    {
        restAnimationTypes();

        switch (type)
        {
            case CharacterEquipmentsV2.WeaponType.TwoHands:
                thisAni.SetBool(CharacterEquipmentsV2.WeaponType.TwoHands.ToString(), true);
                break;

            case CharacterEquipmentsV2.WeaponType.UnArmed:
                thisAni.SetBool(CharacterEquipmentsV2.WeaponType.UnArmed.ToString(), true);
                break;

            case CharacterEquipmentsV2.WeaponType.Shield:
                thisAni.SetBool(CharacterEquipmentsV2.WeaponType.OneHand.ToString(), true);
                break;

            case CharacterEquipmentsV2.WeaponType.DualWeild:
                thisAni.SetBool(CharacterEquipmentsV2.WeaponType.DualWeild.ToString(), true);
                break;

            case CharacterEquipmentsV2.WeaponType.OneHand:
                thisAni.SetBool(CharacterEquipmentsV2.WeaponType.OneHand.ToString(), true);
                break;
        }
    }

    private void restAnimationTypes()
    {
        thisAni.SetBool(CharacterEquipmentsV2.WeaponType.UnArmed.ToString(), false);
        thisAni.SetBool(CharacterEquipmentsV2.WeaponType.TwoHands.ToString(), false);
        thisAni.SetBool(CharacterEquipmentsV2.WeaponType.UnArmed.ToString(), false);
        thisAni.SetBool(CharacterEquipmentsV2.WeaponType.OneHand.ToString(), false);
        thisAni.SetBool(CharacterEquipmentsV2.WeaponType.DualWeild.ToString(), false);
        thisAni.SetBool(CharacterEquipmentsV2.WeaponType.OneHand.ToString(), false);
    }

    public EquipmentV2 getRandomEquippedGear()
    {
        return currentEquipment.getRandomEquippedEqipment();
    }

    protected void playDamagedEffect(bool doDamageAnimation)
    {
        if (doDamageAnimation)
            thisAni.SetTrigger("Damage");

        BattleEffectManager.instance.spwanBloodEffect(myTransform.position);
        BattleSoundsManager.instance.playSound(BattleSoundsManager.instance.damageSounds.GetRandomElement<AudioClip>(), 0.7f);
    }

    protected void playDeadEffect()
    {
        gameObject.layer = GameManager.instance.deadBodyLayer;
        name = "Dead_" + name;
        cc.enabled = false;
        rb.isKinematic = true;
        ragdolled = true;
        GameObject bloodPond = BattleEffectManager.instance.spwanBloodEffect(chestBornTran.position, 10);

        bloodPond.transform.parent = chestBornTran;
        bloodPond.transform.localPosition = Vector3.zero;
        BattleSoundsManager.instance.playSound(BattleSoundsManager.instance.deathSounds.GetRandomElement<AudioClip>(), 0.8f);
        if (GameManager.instance.turnOffRagDollDeadAnimationAfter)
            StartCoroutine("WaitAndOffRagdoll");
    }

    protected IEnumerator WaitAndOffRagdoll()
    {
        // suspend execution for passed seconds
        yield return new WaitForSeconds(GameManager.instance.intervalSecToTurnOffRagDoll);
        setKinematic(true);
    }

    protected void playAttackEffect()
    {
        thisAni.SetFloat("Movement_Speed", 0);
        BattleSoundsManager.instance.playSound(BattleSoundsManager.instance.swingSounds.GetRandomElement<AudioClip>(), 1);
    }

    protected void playAttackAnimation(bool on)
    {
        if (on)
        {
            thisAni.SetFloat("AttackSpeed", 1);
            thisAni.SetFloat("AttackAnimSpeed", characterInfo.attackSpeed);
            thisAni.SetFloat("Movement_Speed", 0);
        }
        else
        {
            thisAni.SetFloat("AttackSpeed", 0f);
        }
    }

    protected void playMoveAnimation(bool on, bool run)
    {
        float speed = 0;
        if (on)
        {
            //this speed will trigger running animation when it is 1.
            speed = run ? 1 : 0.2f;
            playAttackAnimation(false);
        }
        thisAni.SetFloat("Movement_Speed", speed);
    }

    protected CapsuleCollider cc;
    protected Rigidbody rb;

    public void setPhygicalBody(RigidbodyConstraints constraints)
    {
        cc = gameObject.AddComponent<CapsuleCollider>();
        cc.height = 2.5f;
        Vector3 center = new Vector3(0, 1.15f, 0);
        cc.center = center;
        cc.enabled = true;
        rb.constraints = constraints;
        rb.isKinematic = false;
    }

    /// <summary>
    /// This method will use the rigibody to move the character object, in order to get collision with other collider(ex,wall etc).
    /// If it is not required to consider any collision detecting while moving the character, please do use each transform.postion or chracter control etc..
    /// </summary>
    /// <param name="pos">desirable Vector3 position</param>
    protected void moveTo(Vector3 pos)
    {
        moveTo(pos, characterInfo.movementSpeed);
    }

    private Vector3 desiredVelocity;

    /// <summary>
    /// This method will use the rigibody to move the character object, in order to get collision with other collider(ex,wall etc).
    /// If it is not required to consider any collision detecting while moving the character, please do use each transform.postion or chracter control etc..
    /// </summary>
    /// <param name="pos">desirable Vector3 position</param>
    /// <param name="movementSpeed">desirable float movement speed</param>
    protected void moveTo(Vector3 pos, float movementSpeed)
    {
        rotateToTarget(pos);
        // will move toward by delta time.
        // which means no teleporting but moving smoothly.
        rb.velocity = myTransform.forward * movementSpeed;
    }

    protected void rotateToTarget(Vector3 pos)
    {
        //will rotate toward to the desitination position.
        Vector3 lookDirction = pos - myTransform.position;
        Quaternion lookRot = Quaternion.LookRotation(lookDirction);
        myTransform.rotation = Quaternion.Slerp(myTransform.rotation, lookRot, characterInfo.movementSpeed * 3 * Time.deltaTime);
    }

    public void initialCharacterEquipment()
    {
        characterInfo.setCurrentlyEquippedItems(currentEquipment);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 *
 * This script moves the GameObeject when you
 * click or click and hold the LeftMouseButton or touch
 *
 * Simply attach it to the gameObject you wanna move (player or not)
 *
 * Autor:       Vinicius Rezendrix - Brazil
 * Modifier:    Hwan Kim - New Zealand
 *
 * -Modification
 * 1.Added to trigger walk and idel animation
 * 2.Added to determine double click or touch event.
 *
 * Created  Date:   11/08/2012
 * Modified Date:   01/08/2015
 *
 */

public class PlayerControllerV2 : BaseCharacterController
{
    public delegate void OnTargetChange(RPGCharacter target);

    public delegate void OnKill(RPGCharacter target);

    public event OnTargetChange onTargetChanged;

    public event OnKill onKill;

    public Vector3 destinationPosition;		// The destination Point
    public float destinationDistance;			// The distance between myTransform and destinationPosition
    public float currentSpeed;
    public float minDistance = 0.5f;
    private float timer_for_double_click;
    private bool one_click = false;
    private bool timer_running;
    private GameObject targetSelectedEffect;
    private EnemyController enemy;

    //this is how long in seconds to allow for a double click
    private float delay = 0.25f;

    public bool run;
    public Transform target;
    private float attackableRange = 1.5f;
    public bool twoHands = false;
    private bool moveTargetInitialized = false;

    /**Loot item related*/
    public bool lootingItem = false;
    private bool onLootProcessing = false;
    private EquipmentV2 lootingEquipment;
    private DroppedItemView clickedView;

    protected override void init()
    {
        characterInfo.type = RPGCharacter.CharacterType.Player;
        equipmentContoller = GetComponent<EquipmentController>();
        currentEquipment.equipmentChanged += currentEquipment_equipmentChanged;
        // sets myTransform to this GameObject.transform
        destinationPosition = myTransform.position;			// prevents myTransform reset
        setWeaponTypeAnimation(currentEquipment.weaponType);
        initialized = true;
    }

    #region main update porcss

    private void FixedUpdate()
    {
        #region click process

        //Making sure we are clicking not GUI.

        // Moves the Player if the Left Mouse Button was clicked
        if (Input.GetMouseButtonDown(0) && GUIUtility.hotControl == 0 && !isItGUI())
        {
            lootingItem = false;
            clickProcess();
            setTargetFromClickPoint(run);
        }
        // Moves the player if the mouse button is hold down
        else if (Input.GetMouseButton(0) && GUIUtility.hotControl == 0 && !isItGUI())
        {
            setTargetFromClickPoint(run);
        }

        resetOneClicked();

        #endregion click process

        if (moveTargetInitialized || targetLockOn)
        {
            //set the distance from destination.
            getDestinationDistance();
        }
        // player reached the destination.
        if (destinationDistance < minDistance)
        {
            //means player was chasing the enemy or have been fighting.
            if (targetLockOn)
            {
                if (enemy.characterInfo.isAlive)
                {
                    playAttackAnimation(true);
                }
                else
                {
                    resetTarget();
                }
                rotateToTarget(target.transform.position);
            }
            else if (lootingItem && !onLootProcessing)
            {
                processLootItem();
            }
            else
                resetTarget();
        }
        //player is not reached destination
        else
        {
            // To Reset Speed to default
            currentSpeed = run ? characterInfo.movementSpeed * 3 : characterInfo.movementSpeed;
            //Play move animation
            playMoveAnimation(true, run);
            //move player character
            moveTo(destinationPosition, run ? characterInfo.movementSpeed * 2 : characterInfo.movementSpeed);
        }
    }

    #endregion main update porcss

    #region player input process

    private bool isItGUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void resetOneClicked()
    {
        //will reset on_click to false if the last click is happened 0.25 sec ago.
        if (one_click)
        {
            // if the time now is delay seconds more than when the first click started.
            if ((Time.time - timer_for_double_click) > delay)
            {
                //basically if thats true its been too long and we want to reset
                //so the next click is simply a single click and not a double click.
                one_click = false;
            }
        }
    }

    private void clickProcess()
    {
        if (!one_click) // first click no previous clicks
        {
            one_click = true;
            run = false;
            timer_for_double_click = Time.time; // save the current time
            // do one click things;
        }
        else
        {
            one_click = false; // found a double click, now reset
            run = true;
        }
    }

    private void setTargetFromClickPoint(bool run)
    {
        targetLockOn = false;
        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {
            if (hitInfo.transform.gameObject.tag.Equals("Enemy"))
            {
                enemy = hitInfo.transform.GetComponent<EnemyController>();
                if (enemy.characterInfo.isAlive)
                {
                    setTarget(enemy);
                }
            }
        }
        if (!targetLockOn)
        {
            BattleGUIControl.instance.hideInfo();
            resetTarget();
            setDestinationPosition(run);
        }
    }

    #endregion player input process

    #region target process

    /// <summary>
    /// This will reset the current target and remove the target effect
    /// </summary>
    private void resetTarget()
    {
        currentSpeed = 0;
        targetLockOn = false;
        moveTargetInitialized = false;

        clickedView = null;
        lootingItem = false;
        lootingEquipment = null;
        onLootProcessing = false;

        playMoveAnimation(false, false);
        playAttackAnimation(false);

        BattleEffectManager.instance.removeSpwanTargetEffectOnTarget();
    }

    private void getDestinationDistance()
    {
        if (targetLockOn)
        {
            minDistance = currentEquipment.getAttackableDistance();
            destinationPosition = target.position;
        }
        else if (lootingItem)
            minDistance = 3;
        else
            minDistance = 0.5f;
        destinationDistance = Vector3.Distance(destinationPosition, myTransform.position);
    }

    private void setDestinationPosition(bool run)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane playerPlane = new Plane(Vector3.up, myTransform.position);
        float hitdist = 0.0f;
        if (playerPlane.Raycast(ray, out hitdist))
        {
            moveTargetInitialized = true;
            destinationPosition = ray.GetPoint(hitdist);
        }
    }

    private void setTarget(EnemyController enemy)
    {
        if (this.enemy != enemy && targetSelectedEffect != null)
        {
            Destroy(targetSelectedEffect);
        }
        target = enemy.transform;
        this.enemy = enemy;
        targetLockOn = true;
        BattleEffectManager.instance.spwanTargetEffectOnTarget(enemy.transform);
        if (onTargetChanged != null)
            onTargetChanged(enemy.characterInfo);
    }

    #endregion target process

    #region battle related methods

    public void Attack()
    {
        if (targetLockOn)
        {
            playAttackEffect();
            if (destinationDistance <= currentEquipment.getAttackableDistance())
            {
                if (enemy.characterInfo.isAlive)
                {
                    enemy.Damage(this);
                }
                else
                {
                    playAttackAnimation(false);
                }
            }
            else
            {
                playMoveAnimation(true, false);
                playAttackAnimation(false);
            }

            if (!enemy.characterInfo.isAlive)
            {
                //At the moment let's just add exp from enemy's characterinfo to player's exp.
                //TODO please implement your game rule.
                characterInfo.currentExp += enemy.characterInfo.currentExp;

                if (onKill != null)
                    onKill(enemy.characterInfo);
                resetTarget();
                playMoveAnimation(false, false);
                playAttackAnimation(false);
            }
        }
    }

    /// <summary>
    /// This method has to be implemented for player getting damage from enemy.
    /// </summary>
    /// <param name="enemy"></param>
    public void Damage(EnemyController enemy)
    {
        int damage = enemy.characterInfo.attack(characterInfo) - characterInfo.defensePower;
        if (damage < 0)
            damage = 0;
        if (damage == 0)
            damage = 1;

        thisAni.SetFloat("Movement_Speed", 0);
        //30 % crit
        bool crit = UnityEngine.Random.Range(1, 100) < 30;
        damage = crit ? damage * 3 : damage;
        BattleEffectManager.instance.createDamageMessageView(damage, crit, myTransform);
        playDamagedEffect(UnityEngine.Random.Range(1, 100) < 20);

        if (GameManager.instance.playerCanDie)
        {
            characterInfo.currentHP -= damage;
        }
        if (characterInfo.isAlive)
        {
            // if player get attacked then auto set the attacker as the target
            if (!targetLockOn)
            {
                setTarget(enemy);
            }

            playDamagedEffect(crit);
            thisAni.SetFloat("AttackSpeed", characterInfo.attackSpeed);
        }
        else
        {
            playDeadEffect();
            Debug.Log("Game Over");
            //TODO do the proper game logic after this.
        }
        if (crit)
            BattleEffectManager.instance.cameraShaker.Shake();
    }

    #endregion battle related methods

    #region item loot process methods

    public void onDroppedItemClicked(DroppedItemView view)
    {
        clickProcess();
        resetOneClicked();
        if (clickedView != view && targetSelectedEffect != null)
        {
            Destroy(targetSelectedEffect);
        }
        clickedView = view;
        lootingEquipment = view.eq;
        target = lootingEquipment.transform;
        destinationPosition = target.position;
        BattleEffectManager.instance.spwanTargetEffectOnTarget(target);
        moveTargetInitialized = true;
        lootingItem = true;
    }

    private void processLootItem()
    {
        onLootProcessing = true;

        bool success = InventoryManager.instance.playerBagControl.insertItem(lootingEquipment);
        if (success)
        {
            Destroy(clickedView.gameObject);
            Destroy(lootingEquipment.gameObject);
        }
        else
        {
            //Random location offset.
            Vector3 location = target.position;
            location.y += 3;
            location.x += UnityEngine.Random.Range(-0.5f, 0.5f);
            location.z += UnityEngine.Random.Range(-0.5f, 0.5f);
            target.position = location;
        }
        resetTarget();
    }

    #endregion item loot process methods

    #region equipment change

    private void currentEquipment_equipmentChanged(object sender, EventArgs e)
    {
        EquipmentV2 changedEquipment = (EquipmentV2)sender;
        twoHands = changedEquipment.equipmentDetails.twoHands;
        if (changedEquipment.type.Equals(EquipmentV2.EquipmentType.Weapon))
        {
            setWeaponTypeAnimation(currentEquipment.weaponType);
        }
    }

    #endregion equipment change

    /// <summary>
    /// This is preventing error on null point exception, if the view got destroyed and player controller is set it as target.
    /// </summary>
    /// <param name="thisTran"></param>
    public void onViewDestroyed(Transform thisTran)
    {
        if (target == thisTran)
        {
            resetTarget();
        }
    }
}
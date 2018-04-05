using System.Collections;
using UnityEngine;

public class EnemyController : BaseCharacterController
{
    private Transform target;
    private PlayerControllerV2 pc;
    public bool targetInitialized = false;
    public float distance;
    private WanderController wanderorAiAgent;

    // Use this for initialization
    protected override void init()
    {
        gameObject.tag = "Enemy";
        characterInfo.type = RPGCharacter.CharacterType.Enemy;
        characterInfo.currentHP = 5;
        characterInfo.maxHP = 5;
        setWeaponTypeAnimation(currentEquipment.weaponType);
        targetInitialized = false;
        initialized = true;
    }

    public void addWonderor()
    {
        wanderorAiAgent = gameObject.AddComponent<WanderController>();
        wanderorAiAgent.init(cc);
    }

    public void moveFoward()
    {
        if (initialized)
        {
            distance = 1;
            moveTo(myTransform.position + myTransform.forward);
            thisAni.SetFloat("Movement_Speed", 0.5f);
        }
    }

    public void moveToTarget()
    {
        if (initialized)
        {
            if (distance > currentEquipment.getAttackableDistance())
            {
                moveTo(pc.myTransform.position);
                //TODO enemy also be able to run
                playMoveAnimation(true, false);
            }
            else
            {
                rotateToTarget(pc.myTransform.position);
                playAttackAnimation(true);
            }
        }
    }

    public void stop()
    {
        distance = 0;
        thisAni.SetFloat("Movement_Speed", 0);
    }

    public void rotateY(float degree)
    {
        if (initialized)
        {
            myTransform.RotateAround(transform.position, transform.up, Time.deltaTime * degree);
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (characterInfo.isAlive)
        {
            if (targetInitialized)
            {
                distance = Vector3.Distance(target.position, myTransform.position);
            }
        }
    }

    public void Attack()
    {
        playAttackEffect();
        if (distance <= currentEquipment.getAttackableDistance())
        {
            pc.Damage(this);
        }
        else
        {
            playAttackAnimation(false);
            thisAni.SetFloat("Movement_Speed", 1);
        }
    }

    public void Damage(PlayerControllerV2 pc)
    {
        int damage = pc.characterInfo.attack(characterInfo) - characterInfo.defensePower;
        if (damage < 0)
            damage = 0;
        if (damage == 0)
            damage = 1;

        thisAni.SetFloat("Movement_Speed", 0);
        //30 % crit
        bool crit = Random.Range(1, 100) < 30;
        damage = crit ? damage * 3 : damage;

        BattleEffectManager.instance.createDamageMessageView(damage, crit, myTransform);

        characterInfo.currentHP -= damage;
        if (characterInfo.isAlive)
        {
            target = pc.transform;
            this.pc = pc;
            targetInitialized = true;

            playDamagedEffect(crit);
            thisAni.SetFloat("AttackSpeed", characterInfo.attackSpeed);
        }
        else
        {
            playDeadEffect();
        }
        if (crit)
            BattleEffectManager.instance.cameraShaker.Shake();
    }
}
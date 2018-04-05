using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author: Hwan Kim.
/// Use this if you want to creating any RPG related stuff.
/// </summary>
[System.Serializable]
public class RPGCharacter
{
    //Id of this character.
    public int id;

    //Event handlers for the character value changes.
    public delegate void OnIntValueChange(int value);

    public event OnIntValueChange onHPChanged;

    public event OnIntValueChange onEXPChanged;

    public event OnIntValueChange onLevelUp;

    public delegate void OnDeath(RPGCharacter thisCharacter);

    public event OnDeath died;

    public enum CharacterType
    {
        Player, NPC, Enemy
    }

    public string characterName;
    public CharacterType type;
    public int strength;
    public int dex;
    public int wisdom;

    public int level = 1;
    public bool isAlive = true;
    public int maxHP;
    public int _currentHP;
    public int _currentExp;
    public float movementSpeed = 1.5f;
    public float attackSpeed = 1;

    public int maxExp
    {
        get
        {
            //Let say we need 10 exp per level. please change this log for your on game rule.
            //TODO please implement your game rule.
            return level * 10;
        }
    }

    public int currentExp
    {
        get
        {
            return _currentExp;
        }
        set
        {
            if (value >= maxExp)
            {
                if (onLevelUp != null)
                {
                    levelUpProcess();
                    onLevelUp(level);
                }
                _currentExp = 0;
            }
            else
                _currentExp = value;

            if (onEXPChanged != null)
                onEXPChanged(_currentExp);
        }
    }

    /// <summary>
    /// Please note this, put your level up logic for your game.
    /// For this example we are just adding max hp + 10 and add +1 for each attribute.
    /// </summary>
    private void levelUpProcess()
    {
        //TODO please implement your game rule.
        maxHP *= 2;
        currentHP = maxHP;
        strength++;
        dex++;
        wisdom++;
        level++;
        //each level attack speed 5% increase.
        attackSpeed += attackSpeed * 0.05f;
        //each level movement speed 5% increase.
        movementSpeed += movementSpeed * 0.05f;
    }

    /// <summary>
    /// Please note this, put your level up logic for your game.
    /// For this example we are just adding max hp + 10 and add +1 for each attribute.
    /// </summary>
    public void levelUpProcess(int level)
    {
        //TODO please implement your game rule.
        maxHP *= level * 2;
        currentHP = maxHP;
        strength *= level;
        dex *= level;
        wisdom *= level;
        this.level = level;
        //each level attack speed 5% increase.
        attackSpeed += (attackSpeed * 0.05f) * level;
        //each level movement speed 5% increase.
        movementSpeed += (movementSpeed * 0.05f) * level;
    }

    public int currentHP
    {
        get
        {
            return _currentHP;
        }
        set
        {
            bool hpChanged = _currentHP != value;
            if (value <= 0)
            {
                isAlive = false;
                _currentHP = 0;
                if (died != null)
                {
                    died(this);
                }
            }
            else
                _currentHP = value;

            if (hpChanged && onHPChanged != null)
            {
                onHPChanged(_currentHP);
            }
        }
    }

    /// <summary>
    /// This is the method you want to put the attack mechanism
    /// ex) STR + wepon dmg. etc..
    /// </summary>
    /// <param name="characterInfo"></param>
    /// <returns></returns>
    public int attack(RPGCharacter characterInfo)
    {
        return attackPower;
    }

    //TODO
    private int _defensePower;

    /// <summary>
    /// This is Armor defense + dex.
    /// </summary>
    public int defensePower
    {
        get
        {
            //Dex will increase the defense 1  per 1 DEX.
            return _defensePower + dex;
        }
    }

    private int _attackPower;

    /// <summary>
    /// This is Weapon damage + str.
    /// </summary>
    public int attackPower
    {
        get
        {
            //Str will increase the attack Power 1 per 1 Str.
            return _attackPower + strength;
        }
    }

    public void setCurrentlyEquippedItems(CharacterEquipmentsV2 currentEquipment)
    {
        foreach (EquipmentV2.EquipmentSide side in currentEquipment.equippedArmors.Keys)
        {
            foreach (EquipmentV2.EqupimentParts part in currentEquipment.equippedArmors[side].Keys)
            {
                _defensePower += currentEquipment.equippedArmors[side][part].equipmentDetails.defencePoint;
                movementSpeed -= movementSpeed * ((float)(currentEquipment.equippedArmors[side][part].equipmentDetails.wightType) / 100);
            }
        }

        foreach (EquipmentV2.EquipmentSide side in currentEquipment.equippedWeapons.Keys)
        {
            _attackPower += currentEquipment.equippedWeapons[side].equipmentDetails.attackPoint;
            attackSpeed -= attackSpeed * ((float)(currentEquipment.equippedWeapons[side].equipmentDetails.wightType) / 100);
        }
    }
}
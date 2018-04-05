using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEquipmentsV2 : MonoBehaviour
{
    public enum WeaponType
    {
        UnArmed, OneHand, TwoHands, DualWeild, Shield
    }

    private Transform myTran;

    public event EventHandler equipmentChanged;

    public EquipmentController equipmentContoller;

    public Dictionary<EquipmentV2.EquipmentSide, Dictionary<EquipmentV2.EqupimentParts, EquipmentV2>> equippedArmors =
        new Dictionary<EquipmentV2.EquipmentSide, Dictionary<EquipmentV2.EqupimentParts, EquipmentV2>>();

    public Dictionary<EquipmentV2.EquipmentSide, EquipmentV2> equippedWeapons = new Dictionary<EquipmentV2.EquipmentSide, EquipmentV2>();
    public EquipmentV2 raical;
    public List<string> allEquippmentIDs = new List<string>();
    public WeaponType weaponType = WeaponType.UnArmed;

    private void Awake()
    {
        myTran = transform;
    }

    public EquipmentV2 getRandomEquippedEqipment()
    {
        if (weaponType != WeaponType.UnArmed)
        {
            if (UnityEngine.Random.Range(0, 2) > 0)
            {
                return equippedWeapons.GetRandomValue();
            }
        }
        return equippedArmors.GetRandomValue().GetRandomValue();
    }

    public void equip(EquipmentV2 e)
    {
        switch (e.type)
        {
            case EquipmentV2.EquipmentType.Armor:
                allEquippmentIDs.Add(e.equipmentDetails.id);
                if (!equippedArmors.ContainsKey(e.side))
                {
                    equippedArmors.Add(e.side, new Dictionary<EquipmentV2.EqupimentParts, EquipmentV2>());
                }
                if (!equippedArmors[e.side].ContainsKey(e.part))
                {
                    equippedArmors[e.side].Add(e.part, e);
                }
                else
                {
                    equippedArmors[e.side][e.part] = e;
                }
                equipmentContoller.showEquipment(e);
                break;

            case EquipmentV2.EquipmentType.Weapon:
                allEquippmentIDs.Add(e.equipmentDetails.id);
                if (e.side == EquipmentV2.EquipmentSide.Left && equippedWeapons.ContainsKey(EquipmentV2.EquipmentSide.Right))
                {
                    if (equippedWeapons[EquipmentV2.EquipmentSide.Right].equipmentDetails.twoHands)
                    {
                        replaceWeaponProcess(EquipmentV2.EquipmentSide.Right);
                    }
                }
                else if (e.equipmentDetails.twoHands)
                {
                    if (equippedWeapons.ContainsKey(EquipmentV2.EquipmentSide.Left))
                    {
                        replaceWeaponProcess(EquipmentV2.EquipmentSide.Left);
                    }
                }

                if (!equippedWeapons.ContainsKey(e.side))
                {
                    equippedWeapons.Add(e.side, e);
                }
                else
                {
                    unequipped(equippedWeapons[e.side]);
                    equippedWeapons[e.side] = e;
                }

                equipmentContoller.showEquipment(e);
                setWeaponType();
                break;

            default:
                equipmentContoller.showEquipment(e);
                break;
        }
        if (equipmentChanged != null)
        {
            equipmentChanged(e, null);
        }
    }

    private void replaceWeaponProcess(EquipmentV2.EquipmentSide side)
    {
        EquipmentV2 unEquipingItem = equippedWeapons[side];
        if (!GameManager.instance.playerBagViewCon.insertItem(unEquipingItem))
        {
            ItemDropManager.instance.dropItem(unEquipingItem, GameManager.instance.getDroppableLocation(myTran));
        }
        unequipped(unEquipingItem);
    }

    public void setWeaponType()
    {
        if (equippedWeapons.ContainsKey(EquipmentV2.EquipmentSide.Left))
        {
            EquipmentV2 leftWeapon = equippedWeapons[EquipmentV2.EquipmentSide.Left];
            if (leftWeapon.equipmentDetails.dualweild)
                weaponType = WeaponType.DualWeild;
            else
                weaponType = WeaponType.Shield;
        }
        else if (equippedWeapons.ContainsKey(EquipmentV2.EquipmentSide.Right))
        {
            EquipmentV2 rightWeapon = equippedWeapons[EquipmentV2.EquipmentSide.Right];
            if (rightWeapon.equipmentDetails.twoHands)
                weaponType = WeaponType.TwoHands;
            else
                weaponType = WeaponType.OneHand;
        }
        else
        {
            weaponType = WeaponType.UnArmed;
        }
    }

    public float getAttackableDistance()
    {
        float distance = 0;
        switch (weaponType)
        {
            case WeaponType.OneHand:
                distance = 2f;
                break;

            case WeaponType.Shield:
                distance = 2f;
                break;

            case WeaponType.TwoHands:
                distance = 3.5f;
                break;

            case WeaponType.DualWeild:
                distance = 2f;
                break;

            case WeaponType.UnArmed:
                distance = 1.8f;
                break;
        }
        return distance;
    }

    public void unequipped(EquipmentV2 eq)
    {
        switch (eq.type)
        {
            case EquipmentV2.EquipmentType.Weapon:
                equippedWeapons.Remove(eq.side);
                setWeaponType();
                break;

            case EquipmentV2.EquipmentType.Armor:
                equippedArmors[eq.side].Remove(eq.part);
                if (equippedArmors[eq.side].Values.Count == 0)
                    equippedArmors.Remove(eq.side);
                break;
        }
        equipmentContoller.hideEquipment(eq);
        allEquippmentIDs.Remove(eq.equipmentDetails.id);
    }

    public EquipmentV2 getWeapon(EquipmentV2.EquipmentSide equipmentSide)
    {
        try
        {
            return equippedWeapons[equipmentSide];
        }
        catch
        {
            EquipmentV2 emptyOne = new EquipmentV2();
            emptyOne.type = EquipmentV2.EquipmentType.Weapon;
            emptyOne.side = equipmentSide;
            emptyOne.equipmentDetails.id = null;
            return emptyOne;
        }
    }
}
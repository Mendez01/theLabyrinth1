using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentView : BaseGameUIView
{
    public Dictionary<EquipmentV2.EquipmentSide, Dictionary<EquipmentV2.EqupimentParts, EquipmentSlot>> slots =
        new Dictionary<EquipmentV2.EquipmentSide, Dictionary<EquipmentV2.EqupimentParts, EquipmentSlot>>();

    protected override void init()
    {
        foreach (EquipmentSlot slot in GetComponentsInChildren<EquipmentSlot>())
        {
            if (!slots.ContainsKey(slot.side))
                slots.Add(slot.side, new Dictionary<EquipmentV2.EqupimentParts, EquipmentSlot>());
            if (!slots[slot.side].ContainsKey(slot.part))
            {
                slots[slot.side].Add(slot.part, slot);
            }
        }
        initialized = true;
    }

    /// <summary>
    /// Override method from the BaseGameUIView class.
    /// Will get runned from PlayerEquipmentController.
    /// </summary>
    /// <param name="anyObject"></param>
    public override void updateUI(UnityEngine.Object anyObject)
    {
        PlayerControllerV2 pc = (PlayerControllerV2)anyObject;

        foreach (EquipmentV2.EquipmentSide side in Extension.getValues<EquipmentV2.EquipmentSide>())
        {
            foreach (EquipmentV2.EqupimentParts part in Extension.getValues<EquipmentV2.EqupimentParts>())
            {
                EquipmentV2 foundEquipment = null;
                if (part == EquipmentV2.EqupimentParts.Hand)
                {
                    if (pc.currentEquipment.equippedWeapons.ContainsKey(side))
                        foundEquipment = pc.currentEquipment.equippedWeapons[side];
                }
                else
                {
                    if (pc.currentEquipment.equippedArmors.ContainsKey(side))
                        if (pc.currentEquipment.equippedArmors[side].ContainsKey(part))
                            foundEquipment = pc.currentEquipment.equippedArmors[side][part];
                }

                if (foundEquipment != null)
                {
                    slots[side][part].setEquipment(foundEquipment);
                }
                else if (slots.ContainsKey(side) && slots[side].ContainsKey(part))
                {
                    slots[side][part].removeEquipment();
                }
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBagView : BaseGameUIView
{
    // this is map for slot that is empty of not.
    private Dictionary<InventorySlot, bool> slots = new Dictionary<InventorySlot, bool>();

    public List<EquipmentV2> equipmentsInBag = new List<EquipmentV2>();

    protected override void init()
    {
        base.init();
        equipmentsInBag.Clear();
        slots.Clear();
        foreach (InventorySlot slot in GetComponentsInChildren<InventorySlot>())
        {
            slots.Add(slot, false);
        }
    }

    public bool hasSpace()
    {
        return equipmentsInBag.Count < slots.Count;
    }

    public void insertEquipment(EquipmentV2 eq)
    {
        InventorySlot emptySlot = null;
        foreach (InventorySlot slot in slots.Keys)
        {
            //this means the slot is empty
            if (!slots[slot])
            {
                emptySlot = slot;
                break;
            }
        }
        // it is not going to happen but in case
        if (emptySlot != null)
        {
            equipmentsInBag.Add(eq);
            emptySlot.setEquipment(eq);
            slots[emptySlot] = true;
        }
    }

    public void removeItem(EquipmentV2 eq)
    {
        InventorySlot foundSlot = null;
        foreach (InventorySlot slot in slots.Keys)
        {
            if (slots[slot] && slot.equipment.equipmentDetails.id.Equals(eq.equipmentDetails.id))
            {
                foundSlot = slot;
            }
        }
        if (foundSlot != null)
        {
            foundSlot.removeEquipment();
            slots[foundSlot] = false;

            equipmentsInBag.Remove(eq);
        }
    }
}
using System.Collections;
using UnityEngine;

public class ItemDetailsController : BaseGameUIController
{
    private EquipmentSlot currentEquippedSlot;
    private InventorySlot currentBagSlot;

    // Use this for initialization
    private void Start()
    {
        show(false);
    }

    public void onSlotButtonClicked(EquipmentSlot clickedSlot)
    {
        if (clickedSlot.equipment != null && !clickedSlot.Equals(currentEquippedSlot))
        {
            currentEquippedSlot = clickedSlot;
            show(true);
            mainUIView.updateUI(clickedSlot);
        }
        else
        {
            if (clickedSlot.Equals(currentEquippedSlot))
            {
                currentEquippedSlot = null;
            }
            show(false);
        }
    }

    public void onSlotButtonClicked(InventorySlot clickedSlot)
    {
        if (clickedSlot.equipment != null && !clickedSlot.Equals(currentBagSlot))
        {
            currentBagSlot = clickedSlot;
            show(true);
            mainUIView.updateUI(clickedSlot);
        }
        else
        {
            if (clickedSlot.Equals(currentBagSlot))
            {
                currentBagSlot = null;
            }
            show(false);
        }
    }

    public void onInventoryButtonClicked()
    {
        if (onShow)
        {
            show(false);
        }
    }

    public void UpdateByEquipmentWithComperance(EquipmentV2 eq, ItemDetailView.Comperance point, ItemDetailView.Comperance weight, bool isInventoryItem)
    {
        ((ItemDetailView)mainUIView).UpdateByEquipmentWithComperance(eq, point, weight, isInventoryItem);
    }
}
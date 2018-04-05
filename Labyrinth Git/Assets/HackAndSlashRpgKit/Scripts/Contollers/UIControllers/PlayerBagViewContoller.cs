using System.Collections;
using UnityEngine;

public class PlayerBagViewContoller : BaseGameUIController
{
    private PlayerBagView _playerBagView;

    private PlayerBagView playerBagView
    {
        get
        {
            if (_playerBagView == null)
            {
                _playerBagView = (PlayerBagView)mainUIView;
            }
            return _playerBagView;
        }
    }

    public PlayerEquipmentControl playerEquipmentCon;
    public ItemDetailsController currentlyEquippedItemCon, itemDetailsCon;
    private InventorySlot previouslyClickedSlot;
    public EquipmentV2 currentlyEquippedItem, inventoryItem;

    private void Awake()
    {
        show(false);
    }

    public void onInventoryButtonClicked()
    {
        show(!onShow);
    }

    public bool insertItem(EquipmentV2 eq)
    {
        eq = GameDataManager.Instance.equipmentManager.getEquipmentById(eq.equipmentDetails.id);
        if (playerBagView.hasSpace())
        {
            playerBagView.insertEquipment(eq);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void removeItem(EquipmentV2 eq)
    {
        playerBagView.removeItem(GameManager.instance.equipmentManager.getEquipmentById(eq.equipmentDetails.id));
    }

    public void bagSlotClicked(InventorySlot clickedSlot)
    {
        bool currentlyEquippedItemViewShow = false, itemDetailsShow = false;
        //If the inventory slot was already clicked then we will just closed this detail views.
        if (!clickedSlot.Equals(previouslyClickedSlot))
        {
            previouslyClickedSlot = clickedSlot;
            inventoryItem = clickedSlot.equipment;
            //Check this slot is empty or not.
            if (inventoryItem != null)
            {
                //1 check we have already same part item is equipped.
                currentlyEquippedItem =
                    playerEquipmentCon.getCurrentEquipmentWihtSideAndPart(inventoryItem.side, inventoryItem.part);

                if (currentlyEquippedItem != null)
                {
                    itemDetailsShow = true;
                    currentlyEquippedItemViewShow = true;
                    itemComapreProcess(inventoryItem, currentlyEquippedItem);
                }
                else
                {
                    itemDetailsShow = true;
                    itemDetailsCon.mainUIView.updateUI(clickedSlot);
                }
            }
        }
        else
        {
            previouslyClickedSlot = null;
        }
        currentlyEquippedItemCon.show(currentlyEquippedItemViewShow);
        itemDetailsCon.show(itemDetailsShow);
    }

    private void itemComapreProcess(EquipmentV2 inventoryItem, EquipmentV2 currentItem)
    {
        ItemDetailView.Comperance currentItemApOrDp, inventoryItemApOrDp, currentItemWeight, inventoryItemWeight;
        int currentItemPoint = currentItem.equipmentDetails.attackPoint + currentItem.equipmentDetails.defencePoint;
        int inventoryItemPoint = inventoryItem.equipmentDetails.attackPoint + inventoryItem.equipmentDetails.defencePoint;

        int currentItemWeightSpeed = (int)(currentItem.equipmentDetails.wightType);
        int inventoryItemWeightSpeed = (int)(inventoryItem.equipmentDetails.wightType);

        if (currentItemPoint > inventoryItemPoint)
        {
            currentItemApOrDp = ItemDetailView.Comperance.Better;
            inventoryItemApOrDp = ItemDetailView.Comperance.Worse;
        }
        else if (currentItemPoint == inventoryItemPoint)
        {
            currentItemApOrDp = ItemDetailView.Comperance.Equal;
            inventoryItemApOrDp = currentItemApOrDp;
        }
        else
        {
            currentItemApOrDp = ItemDetailView.Comperance.Worse;
            inventoryItemApOrDp = ItemDetailView.Comperance.Better;
        }

        //Wight is smaller is better as it will increase the Movement/Attack Speed.
        if (currentItemWeightSpeed > inventoryItemWeightSpeed)
        {
            currentItemWeight = ItemDetailView.Comperance.Worse;
            inventoryItemWeight = ItemDetailView.Comperance.Better;
        }
        else if (currentItemWeightSpeed == inventoryItemWeightSpeed)
        {
            currentItemWeight = ItemDetailView.Comperance.Equal;
            inventoryItemWeight = currentItemWeight;
        }
        else
        {
            currentItemWeight = ItemDetailView.Comperance.Better;
            inventoryItemWeight = ItemDetailView.Comperance.Worse;
        }

        itemDetailsCon.UpdateByEquipmentWithComperance(inventoryItem, inventoryItemApOrDp, inventoryItemWeight, true);
        currentlyEquippedItemCon.UpdateByEquipmentWithComperance(currentItem, currentItemApOrDp, currentItemWeight, false);
    }

    public void equipItemProcess()
    {
        removeItem(inventoryItem);
        inventoryItem = null;
        if (currentlyEquippedItem != null)
        {
            insertItem(currentlyEquippedItem);
            currentlyEquippedItem = null;
        }

        hideBothItemDetailViews();
    }

    public void hideBothItemDetailViews()
    {
        currentlyEquippedItemCon.show(false);
        itemDetailsCon.show(false);
    }
}
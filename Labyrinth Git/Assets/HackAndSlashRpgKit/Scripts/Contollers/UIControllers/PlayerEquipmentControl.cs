using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentControl : BaseGameUIController
{
    public ItemDetailsController itemDetailsCon, currentlyEquippedItemCon;
    private PlayerEquipmentView view;

    private void Start()
    {
        if (!initialized)
            init();
    }

    public override void init()
    {
        view = (PlayerEquipmentView)mainUIView;

        show(false);
    }

    public void addListenerToPlayerEquipmentChange(CharacterEquipmentsV2 playerEquipment)
    {
        playerEquipment.equipmentChanged += playerEquipment_equipmentChanged;
    }

    private void playerEquipment_equipmentChanged(object sender, System.EventArgs e)
    {
        show(true);
    }

    /// <summary>
    /// Overriding the show method to do the extra process we would like to do.
    /// </summary>
    /// <param name="on"></param>
    public override void show(bool on)
    {
        base.show(on);
        if (on)
        {
            mainUIView.updateUI(GameManager.instance.pc);
        }
    }

    public void inventoryButtonClicked()
    {
        show(!onShow);
    }

    public EquipmentV2 getCurrentEquipmentWihtSideAndPart(EquipmentV2.EquipmentSide side, EquipmentV2.EqupimentParts part)
    {
        EquipmentV2 equipemnt = null;
        try
        {
            equipemnt = view.slots[side][part].equipment;
        }
        catch
        {
            //If it reaches here mean it is empty.
        }
        return equipemnt;
    }

    public void removeEquipmentWihtSideAndPart(EquipmentV2.EquipmentSide side, EquipmentV2.EqupimentParts part)
    {
        view.slots[side][part].removeEquipment();
    }

    public void onSlotButtonClicked(EquipmentSlot clickedSlot)
    {
        itemDetailsCon.onSlotButtonClicked(clickedSlot);
        currentlyEquippedItemCon.show(false);
    }
}
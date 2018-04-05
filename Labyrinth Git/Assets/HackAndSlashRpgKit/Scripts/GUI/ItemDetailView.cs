using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailView : BaseGameUIView
{
    public enum ItemFrom
    {
        InvenotrySlot, EquipmentSlot
    }

    public enum Comperance
    {
        [Description("+")]
        Better,

        [Description("=")]
        Equal,

        [Description("-")]
        Worse
    }

    public Image itemIcon;
    private RectTransform myItemIconRecTran;
    public Text itemName, attackOrDefensePower, weight, special, description, buttonText;
    public Text attackOrDefensePowerPerform, weightPerform;
    public GameObject equipButton;
    public EquipmentV2 currentViewEqiupment;
    public string equipButtonText, unequipButtonText;
    public ItemFrom itemFrom;
    public Vector2 originIconSize;

    protected override void init()
    {
        if (itemIcon != null)
        {
            myItemIconRecTran = itemIcon.GetComponent<RectTransform>();
            originIconSize = myItemIconRecTran.sizeDelta;
            initialized = true;
        }
    }

    public override void updateUI(Object anyObject)
    {
        if (initialized)
        {
            currentViewEqiupment = null;
            if (anyObject.GetType() == typeof(EquipmentSlot))
            {
                EquipmentSlot slot = (EquipmentSlot)anyObject;
                currentViewEqiupment = slot.equipment;
                buttonText.text = unequipButtonText;
                itemFrom = ItemFrom.EquipmentSlot;
            }
            else if (anyObject.GetType() == typeof(InventorySlot))
            {
                InventorySlot slot = (InventorySlot)anyObject;
                currentViewEqiupment = slot.equipment;
                buttonText.text = equipButtonText;
                itemFrom = ItemFrom.InvenotrySlot;
            }
            if (currentViewEqiupment != null)
            {
                UpdateByEquipment(currentViewEqiupment);
                attackOrDefensePowerPerform.text = "";
                weightPerform.text = "";
            }
        }
        else
        {
            Debug.LogError("Please check with " + name + " that all public Texts and Image are set");
        }
    }

    public void UpdateByEquipment(EquipmentV2 eq)
    {
        currentViewEqiupment = eq;
        iconProcess(eq);
        itemName.text = eq.getFullName().realWordString();
        itemName.color = eq.getRarityColor();
        weight.text = eq.equipmentDetails.wightType.ToString() + " " + eq.type.ToString();
        //TODO if you want to put like + 20 block rate then add it here.
        special.text = "Nothing special";
        description.text = "Description :" + eq.equipmentDetails.description;
        if (eq.type == EquipmentV2.EquipmentType.Weapon)
        {
            attackOrDefensePower.text = "Offense " + eq.equipmentDetails.attackPoint;
            if (eq.equipmentDetails.defencePoint > 0)
                attackOrDefensePower.text += "(Df:" + eq.equipmentDetails.defencePoint + ")";
        }
        else
        {
            attackOrDefensePower.text = "Defense " + eq.equipmentDetails.defencePoint;
        }
    }

    public void UpdateByEquipmentWithComperance(EquipmentV2 eq, Comperance point, Comperance weight, bool isInventoryItem)
    {
        itemFrom = isInventoryItem ? ItemFrom.InvenotrySlot : ItemFrom.EquipmentSlot;
        UpdateByEquipment(eq);
        attackOrDefensePowerPerform.text = point.GetDesc();
        weightPerform.text = weight.GetDesc();
        if (buttonText != null)
            buttonText.text = equipButtonText;
        if (equipButton != null)
            equipButton.SetActive(isInventoryItem);
    }

    public void iconProcess(EquipmentV2 eq)
    {
        itemIcon.transform.localRotation = Quaternion.Euler(eq.iconRotationOffset);
        itemIcon.sprite = eq.icon;
        if (eq.side == EquipmentV2.EquipmentSide.Left)
            itemIcon.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        if (eq.iconOffsetSize != null && eq.iconOffsetSize != Vector2.zero)
        {
            myItemIconRecTran.sizeDelta = eq.iconOffsetSize;
        }
        else
        {
            myItemIconRecTran.sizeDelta = originIconSize;
        }
    }
}
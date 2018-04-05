using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DroppedItemView : BaseGameUIView
{
    public delegate void OnViewDestroy(Transform thisTran);

    public OnViewDestroy viewDestroyed;

    public Image itemIcon;
    private Vector2 originSizeOfIcon;
    private Vector2 ratioSizeChanged;
    private RectTransform iconRecTran;
    public Text itemName;
    public EquipmentV2 eq;
    public Vector3 inventoryScale = new Vector3(0.7f, 0.7f, 0.7f);

    private void Awake()
    {
        iconRecTran = (RectTransform)itemIcon.transform;
        originSizeOfIcon = iconRecTran.sizeDelta;
    }

    public override void updateUI(Object anyObject)
    {
        eq = (EquipmentV2)anyObject;
        setImageWithRatio(eq);
        itemName.text = eq.getFullName();
        itemName.color = eq.getRarityColor();
        name = "Dropped " + itemName.text;
    }

    private void setImageWithRatio(EquipmentV2 eq)
    {
        iconRecTran.localRotation = Quaternion.Euler(eq.iconRotationOffset);
        itemIcon.sprite = eq.icon;
        if (eq.side == EquipmentV2.EquipmentSide.Left)
            itemIcon.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        if (eq.iconOffsetSize != null && eq.iconOffsetSize != Vector2.zero)
        {
            iconRecTran.sizeDelta = eq.iconOffsetSize;
        }
        iconRecTran.localScale = inventoryScale;
    }

    public void destoryTheView()
    {
        if (viewDestroyed != null)
        {
            viewDestroyed(transform);
        }
        Destroy(this.gameObject);
    }
}
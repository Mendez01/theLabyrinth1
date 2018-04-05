using UnityEngine;
using UnityEngine.UI;

public class BaseItemIconView : MonoBehaviour
{
    public Image itemIcon;
    private RectTransform _myItemIconRecTran;

    private RectTransform myItemIconRecTran
    {
        get
        {
            if (_myItemIconRecTran == null)
            {
                _myItemIconRecTran = (RectTransform)itemIcon.transform;
            }
            return _myItemIconRecTran;
        }
    }

    private Vector2 originSizeDelta = new Vector2(40, 40);

    private Color zeroAlpha = new Color(255, 255, 255, 0);

    private Color origin = new Color(255, 255, 255, 255);
    public EquipmentV2 equipment;
    public bool initialized = false;
    public string itemIconTag = "ItemIcon";

    private void Start()
    {
        init();
    }

    protected void init()
    {
        Transform[] allChildren = gameObject.GetComponentsInChildren<Transform>();
        if (itemIcon == null)
        {
            foreach (Transform child in allChildren)
            {
                if (child.tag == itemIconTag)
                {
                    itemIcon = child.GetComponent<Image>();
                    break;
                }
            }
        }

        initialized = true;
    }

    public void setEquipment(EquipmentV2 eq)
    {
        if (eq != null && eq.isValidEquipment)
        {
            equipment = eq;
            iconProcess(equipment);
        }
    }

    public void iconProcess(EquipmentV2 eq)
    {
        myItemIconRecTran.localRotation = Quaternion.Euler(eq.iconRotationOffset);
        itemIcon.sprite = eq.icon;
        if (eq.side == EquipmentV2.EquipmentSide.Left)
            myItemIconRecTran.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        if (eq.iconOffsetSize != null && isValidSize(eq.iconOffsetSize))
        {
            myItemIconRecTran.sizeDelta = eq.iconOffsetSize;
        }
        else
        {
            myItemIconRecTran.sizeDelta = originSizeDelta;
        }
        itemIcon.color = origin;
    }

    public void removeEquipment()
    {
        equipment = null;
        itemIcon.sprite = null;
        itemIcon.color = zeroAlpha;
    }

    public bool isValidSize(Vector2 size)
    {
        return size.x > 0 && size.y > 0;
    }
}
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class EquipmentSlot : BaseItemIconView
{
    public EquipmentV2.EqupimentParts part;
    public EquipmentV2.EquipmentSide side;

    private char[] delimiterChars = { '-' };

    private RectTransform myRectTran;

    // Use this for initialization
    private void Awake()
    {
        if (!initialized)
        {
            string[] nameSperated = name.Split(delimiterChars);
            try
            {
                part = (EquipmentV2.EqupimentParts)Enum.Parse(typeof(EquipmentV2.EqupimentParts), nameSperated[1]);
                if (part == EquipmentV2.EqupimentParts.Head || part == EquipmentV2.EqupimentParts.Chest)
                    side = EquipmentV2.EquipmentSide.Middle;
                else
                {
                    if ("l".Equals(nameSperated[2]))
                        side = EquipmentV2.EquipmentSide.Left;
                    else if ("r".Equals(nameSperated[2]))
                        side = EquipmentV2.EquipmentSide.Right;
                }
                initialized = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.LogError("EquipmentSlot " + name + " name rule is not correct,  should be EquipmentSlot-part-side(l or r)");
            }
        }
    }
}
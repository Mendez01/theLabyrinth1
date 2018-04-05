using System;
using UnityEngine;

[ExecuteInEditMode]
public class EquipmentV2 : MonoBehaviour
{
    public enum Rarity
    {
        Normal, Superial, Magical, Rare, Legendary
    }

    public enum EqupimentParts
    {
        Chest, Shoulder, UpperArm, LowerArm, Hand, UpperLeg, LowerLeg, Feet, Head
    }

    public enum EquipmentType
    {
        Hair, FacialHair, FaceAccesary, Weapon, Armor, Body, Eyebrows, Face, Racial
    }

    public enum EquipmentSide
    {
        Left, Right, Middle
    }

    public Rarity rarity;
    public bool initialized = false;

    [Header("If this is set to true, then it will get used as place holder.")]
    public bool placeHolder = false;

    private SkinnedMeshRenderer _meshRen;

    [Header("Indicate of showing it contains Equipment.")]
    public bool isValidEquipment = false;

    public SkinnedMeshRenderer meshRen
    {
        get
        {
            if (_meshRen == null)
            {
                _meshRen = GetComponent<SkinnedMeshRenderer>();
            }
            return _meshRen;
        }
    }

    //For details of this item.
    public Item equipmentDetails;

    //if it is needed.
    public Vector3 iconRotationOffset;

    public Vector2 iconOffsetSize;
    public Sprite icon;

    public EquipmentType type;
    public EqupimentParts part;
    public EquipmentSide side;

    private void Awake()
    {
        if (!initialized)
            init();
    }

    private char[] delimiterChars = { '_' };

    /// <summary>
    /// This method will run and set up the part, side, type information of the equipment.
    /// there are naming rule to work with this method.
    /// name should be like this order and separator is "_"
    /// 1. part info(has to be one of EqupimentParts name string)
    /// 2. side info(has to be one of EqupimentSide name string)
    /// 3. type info(has to be one of EquipmentType name string)
    /// 4. name of the equipment and if the type is Armor then will set as set name.
    /// 5. (Optional) (t, 2, d can be entered.)
    /// t = body parts required, 2 means two hand weapon, d means dual wield.
    /// ex) LowerLeg_Right_Armor_Chainmail
    /// </summary>
    public void init()
    {
        try
        {
            if (!placeHolder)
            {
                equipmentDetails = new Item();
                equipmentDetails.id = gameObject.name;

                string[] nameSeperation = equipmentDetails.id.Split(delimiterChars);
                part = (EqupimentParts)Enum.Parse(typeof(EqupimentParts), nameSeperation[0]);
                if (part == EqupimentParts.Head || part == EqupimentParts.Chest)
                {
                    side = EquipmentSide.Middle;
                    type = (EquipmentType)Enum.Parse(typeof(EquipmentType), nameSeperation[1]);
                    if (type == EquipmentType.Armor)
                    {
                        equipmentDetails.nameOfSet = nameSeperation[2];
                        equipmentDetails.equipmentName = equipmentDetails.nameOfSet;
                        equipmentDetails.isSet = true;
                    }
                    else if (type == EquipmentType.Body)
                    {
                        equipmentDetails.nameOfSet = "Body";
                        equipmentDetails.equipmentName = equipmentDetails.nameOfSet;
                    }
                    else
                    {
                        equipmentDetails.equipmentName = nameSeperation[2].realWordString();
                        if (isAppearanceEquipment() && type != EquipmentType.FaceAccesary)
                        {
                            equipmentDetails.nameOfSet = equipmentDetails.equipmentName;
                        }
                    }
                }
                else
                {
                    side = (EquipmentSide)Enum.Parse(typeof(EquipmentSide), nameSeperation[1]);
                    type = (EquipmentType)Enum.Parse(typeof(EquipmentType), nameSeperation[2]);
                    if (type == EquipmentType.Body)
                    {
                        equipmentDetails.nameOfSet = "Body";
                        equipmentDetails.equipmentName = equipmentDetails.nameOfSet;
                    }
                    else if (type == EquipmentType.Armor)
                    {
                        equipmentDetails.isSet = true;
                        equipmentDetails.nameOfSet = nameSeperation[3];
                        equipmentDetails.equipmentName = equipmentDetails.nameOfSet;
                    }
                    else
                    {
                        equipmentDetails.equipmentName = nameSeperation[3].realWordString();
                    }
                }
                if (nameSeperation[nameSeperation.Length - 1].Equals("t"))
                    equipmentDetails.needBodyPart = true;
                else if (nameSeperation[nameSeperation.Length - 1].Equals("2"))
                    equipmentDetails.twoHands = true;
                else if (nameSeperation[nameSeperation.Length - 1].Equals("d"))
                    equipmentDetails.dualweild = true;
                isValidEquipment = true;
            }
            else
                isValidEquipment = false;
        }
        catch
        {
            Debug.Log("Error occurred during setting the enums up with " + gameObject.name);
        }

        initialized = true;
    }

    public string getPrefabFileName()
    {
        return "/" + name + ".prefab";
    }

    public string getFullName()
    {
        if (type == EquipmentType.Armor)
        {
            if (part != EqupimentParts.Feet)
                return
                      part + " " + equipmentDetails.equipmentName + " " + type;
            else
                return
                     equipmentDetails.equipmentName + " " + type + " Shoe ";
        }
        else
        {
            if (equipmentDetails.dualweild)
                return "Side Weapon " + equipmentDetails.equipmentName;
            else
                return equipmentDetails.equipmentName;
        }
    }

    /// <summary>
    /// This will mainly get used for the place holder Equipment.
    /// Send null Equipment will un-eqiup the current Equipment setting.
    /// </summary>
    /// <param name="e"></param>
    public void setEquipment(EquipmentV2 e)
    {
        if (e != null)
        {
            isValidEquipment = true;
            e.gameObject.SetActive(true);
            meshRen.sharedMesh = e.meshRen.sharedMesh;
            meshRen.sharedMaterial = e.meshRen.sharedMaterial;
            iconOffsetSize = e.iconOffsetSize;
            icon = e.icon;
            equipmentDetails = e.equipmentDetails;
            e.gameObject.SetActive(false);
        }
        else
        {
            isValidEquipment = false;
            meshRen.sharedMesh = null;
            icon = null;
            equipmentDetails = null;
        }
    }

    public bool isAppearanceEquipment()
    {
        return (type == EquipmentV2.EquipmentType.Hair || type == EquipmentV2.EquipmentType.FacialHair ||
                type == EquipmentV2.EquipmentType.Eyebrows || type == EquipmentV2.EquipmentType.FaceAccesary)
                && type != EquipmentV2.EquipmentType.Armor && type != EquipmentV2.EquipmentType.Racial
                && type != EquipmentV2.EquipmentType.Body;
    }

    public Color getRarityColor()
    {
        switch (rarity)
        {
            case Rarity.Normal:
                return Color.white;

            case Rarity.Superial:
                return Color.cyan;

            case Rarity.Magical:
                return Color.blue;

            case Rarity.Rare:
                return Color.yellow;

            case Rarity.Legendary:
                return Color.red;

            default:
                return Color.white;
        }
    }
}
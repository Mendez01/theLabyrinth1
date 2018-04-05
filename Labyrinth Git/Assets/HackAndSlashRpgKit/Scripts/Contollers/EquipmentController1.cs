using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is designed to change equipment in run time without causing calling Instantiate which is quite heavy process.
/// This class is extended to EquipmentManager for changing equipment functionality.
/// </summary>
public class EquipmentController : MonoBehaviour
{
    [Space(10)]
    [Header("Currently Equipped items")]
    //Currently equipped equipment, appearance also contains
    public List<string> equippedEquipmentIds = new List<string>();

    public string currentRace;

    public string placeHolderTag = "PlaceHolder";

    //for eye color
    public Eye characterEye;

    protected Transform myTran;

    //Body parts
    protected Dictionary<EquipmentV2.EquipmentSide, Dictionary<EquipmentV2.EqupimentParts, EquipmentV2>> bodies =
        new Dictionary<EquipmentV2.EquipmentSide, Dictionary<EquipmentV2.EqupimentParts, EquipmentV2>>();

    //appearance of the character dictionary ex) eyebrows, hair etc.
    public Dictionary<EquipmentV2.EquipmentType, EquipmentV2> appearance = new Dictionary<EquipmentV2.EquipmentType, EquipmentV2>();

    /// <summary>
    /// This is the Dictionary for keeping all place holder for the Body parts.
    /// </summary>
    public Dictionary<EquipmentV2.EquipmentSide, Dictionary<EquipmentV2.EqupimentParts, EquipmentV2>> bodyPlaceHolder =
        new Dictionary<EquipmentV2.EquipmentSide, Dictionary<EquipmentV2.EqupimentParts, EquipmentV2>>();

    /// <summary>
    /// This is the Dictionary for keeping all place holder for the armors.
    /// </summary>
    public Dictionary<EquipmentV2.EquipmentSide, Dictionary<EquipmentV2.EqupimentParts, EquipmentV2>> armorPlaceHolder =
        new Dictionary<EquipmentV2.EquipmentSide, Dictionary<EquipmentV2.EqupimentParts, EquipmentV2>>();

    /// <summary>
    /// This is the Dictionary for keeping all place holder for the weapons.
    /// </summary>
    public Dictionary<EquipmentV2.EquipmentSide, EquipmentV2> weaponPlaceHolder = new Dictionary<EquipmentV2.EquipmentSide, EquipmentV2>();

    /// <summary>
    /// This is the Dictionary for keeping all place holder for the appearance.
    /// </summary>
    public Dictionary<EquipmentV2.EquipmentType, EquipmentV2> appearancePlaceHolder = new Dictionary<EquipmentV2.EquipmentType, EquipmentV2>();

    protected void setAllEquipmentActive()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Will find all places holders, this will get used for the equipment manager clone
    /// as we already created them from original manager init.
    /// Will DELETE all Equipment (Except place holders and racial equipments) permanent.
    ///
    /// Important!! Please never call this method in EquipmentManager for itself.
    /// </summary>
    /// <param name="bodies"></param>
    /// <param name="appearance"></param>
    /// <param name="currentRace"></param>
    public void initContoller(Dictionary<EquipmentV2.EquipmentSide, Dictionary<EquipmentV2.EqupimentParts, EquipmentV2>> bodies,
        Dictionary<EquipmentV2.EquipmentType, EquipmentV2> appearance, string currentRace)
    {
        List<string> loadedPlaceHoder = new List<string>();
        myTran = transform;
        /*
        equippedEquipmentIds.Clear();
        armorPlaceHolder.Clear();
        weaponPlaceHolder.Clear();
        bodyPlaceHolder.Clear();
        appearancePlaceHolder.Clear();
        */
        setAllEquipmentActive();
        EquipmentV2[] eqs = myTran.GetComponentsInChildren<EquipmentV2>();
        //Now we are doing trimming for unnecessary equipments.
        foreach (EquipmentV2 eq in eqs)
        {
            if (eq.tag == placeHolderTag)
            {
                if (!loadedPlaceHoder.Contains(eq.gameObject.name))
                {
                    loadedPlaceHoder.Add(eq.gameObject.name);
                    initializingPlaceHolderByEquipment(eq);
                }
                //Delete duplications
                else
                {
                    destory(eq.transform.gameObject);
                }
            }
            //just in case keep them if we need to change race at run time.
            else if (eq.type == EquipmentV2.EquipmentType.Racial)
            {
                if (!eq.equipmentDetails.equipmentName.Equals(currentRace))
                    eq.gameObject.SetActive(false);
            }
            else
                destory(eq.gameObject);
        }
        this.bodies = bodies;
        this.appearance = appearance;
    }

    private void initializingPlaceHolderByEquipment(EquipmentV2 e)
    {
        e.init();
        e.gameObject.SetActive(true);

        if (e.type == EquipmentV2.EquipmentType.Armor)
        {
            //Lets Create Armor place holder, if there are not any.
            if (!armorPlaceHolder.ContainsKey(e.side))
            {
                armorPlaceHolder.Add(e.side, new Dictionary<EquipmentV2.EqupimentParts, EquipmentV2>());
            }
            if (!armorPlaceHolder[e.side].ContainsKey(e.part))
            {
                armorPlaceHolder[e.side].Add(e.part, e);
            }
        }
        //Weapon equipments.
        else if (e.type == EquipmentV2.EquipmentType.Weapon)
        {
            if (!weaponPlaceHolder.ContainsKey(e.side))
            {
                weaponPlaceHolder.Add(e.side, e);
            }
        }
        //Body parts.
        else if (e.type == EquipmentV2.EquipmentType.Body)
        {
            //Lets Create Armor place holder, if there are not any.
            if (!bodyPlaceHolder.ContainsKey(e.side))
            {
                bodyPlaceHolder.Add(e.side, new Dictionary<EquipmentV2.EqupimentParts, EquipmentV2>());
            }
            if (!bodyPlaceHolder[e.side].ContainsKey(e.part))
            {
                bodyPlaceHolder[e.side].Add(e.part, e);
            }
        }
        //Appearances
        else if (e.isAppearanceEquipment())
        {
            if (!appearancePlaceHolder.ContainsKey(e.type))
            {
                appearancePlaceHolder.Add(e.type, e);
            }
        }
        //Let's set nothing.
        e.setEquipment(null);
    }

    public virtual void showEquipment(EquipmentV2 e)
    {
        try
        {
            if (!equippedEquipmentIds.Contains(e.equipmentDetails.id) && !string.IsNullOrEmpty(e.equipmentDetails.id))
            {
                equippedEquipmentIds.Add(e.equipmentDetails.id);

                if (e.isAppearanceEquipment())
                {
                    if (!appearance.ContainsKey(e.type))
                    {
                        appearance.Add(e.type, e);
                    }
                    else
                    {
                        appearance[e.type] = e;
                    }
                    appearancePlaceHolder[e.type].setEquipment(e);
                }
                else
                {
                    switch (e.type)
                    {
                        case EquipmentV2.EquipmentType.Weapon:
                            weaponPlaceHolder[e.side].setEquipment(e);
                            break;

                        case EquipmentV2.EquipmentType.Armor:
                            armorPlaceHolder[e.side][e.part].setEquipment(e);

                            if (e.equipmentDetails.needBodyPart)
                            {
                                showEquipment(bodies[e.side][e.part]);
                            }
                            if (e.part == EquipmentV2.EqupimentParts.Head)
                            {
                                showHeadGearWithFacialFeatures(e);
                            }
                            break;

                        case EquipmentV2.EquipmentType.Body:
                            bodyPlaceHolder[e.side][e.part].setEquipment(e);

                            break;

                        case EquipmentV2.EquipmentType.Racial:
                            e.gameObject.SetActive(true);
                            break;
                    }
                }
            }
        }
        catch
        {
            //Not much thing to do just ignore.
        }
    }

    protected void showAllApperance()
    {
        if (appearance.ContainsKey(EquipmentV2.EquipmentType.Eyebrows))
            appearancePlaceHolder[EquipmentV2.EquipmentType.Eyebrows].setEquipment(appearance[EquipmentV2.EquipmentType.Eyebrows]);

        if (appearance.ContainsKey(EquipmentV2.EquipmentType.FacialHair))
            appearancePlaceHolder[EquipmentV2.EquipmentType.FacialHair].setEquipment(appearance[EquipmentV2.EquipmentType.FacialHair]);

        if (appearance.ContainsKey(EquipmentV2.EquipmentType.Hair))
            appearancePlaceHolder[EquipmentV2.EquipmentType.Hair].setEquipment(appearance[EquipmentV2.EquipmentType.Hair]);

        if (appearance.ContainsKey(EquipmentV2.EquipmentType.FaceAccesary))
            appearancePlaceHolder[EquipmentV2.EquipmentType.FaceAccesary].setEquipment(appearance[EquipmentV2.EquipmentType.FaceAccesary]);
    }

    protected void showHeadGearWithFacialFeatures(EquipmentV2 e)
    {
        if (appearance.ContainsKey(EquipmentV2.EquipmentType.FaceAccesary))
            appearancePlaceHolder[EquipmentV2.EquipmentType.FaceAccesary].setEquipment(appearance[EquipmentV2.EquipmentType.FaceAccesary]);

        if (e.equipmentDetails.HairVisible && appearance.ContainsKey(EquipmentV2.EquipmentType.Hair))
        {
            appearancePlaceHolder[EquipmentV2.EquipmentType.Hair].setEquipment(appearance[EquipmentV2.EquipmentType.Hair]);
        }
        else
        {
            appearancePlaceHolder[EquipmentV2.EquipmentType.Hair].setEquipment(null);
        }

        if (e.equipmentDetails.eyeBlowsVisible && appearance.ContainsKey(EquipmentV2.EquipmentType.Eyebrows))
        {
            appearancePlaceHolder[EquipmentV2.EquipmentType.Eyebrows].setEquipment(appearance[EquipmentV2.EquipmentType.Eyebrows]);
        }
        else
            appearancePlaceHolder[EquipmentV2.EquipmentType.Eyebrows].setEquipment(null);

        if (e.equipmentDetails.facialHairVisible && appearance.ContainsKey(EquipmentV2.EquipmentType.FacialHair))
        {
            appearancePlaceHolder[EquipmentV2.EquipmentType.FacialHair].setEquipment(appearance[EquipmentV2.EquipmentType.FacialHair]);
        }
        else
            appearancePlaceHolder[EquipmentV2.EquipmentType.FacialHair].setEquipment(null);
    }

    protected void destory(UnityEngine.Object anyObj)
    {
        if (Application.isPlaying)
        {
            Destroy(anyObj);
        }
#if UNITY_EDITOR
        else
        {
            DestroyImmediate(anyObj);
        }
#endif
    }

    public void hideEquipment(EquipmentV2 e)
    {
        if (equippedEquipmentIds.Contains(e.equipmentDetails.id) && !string.IsNullOrEmpty(e.equipmentDetails.id))
        {
            equippedEquipmentIds.Remove(e.equipmentDetails.id);
            if (e.isAppearanceEquipment())
            {
                appearancePlaceHolder[e.type].setEquipment(null);
            }
            else if (e.type == EquipmentV2.EquipmentType.Racial)
            {
                e.gameObject.SetActive(false);
            }
            else
            {
                switch (e.type)
                {
                    case EquipmentV2.EquipmentType.Weapon:
                        weaponPlaceHolder[e.side].setEquipment(null);
                        break;

                    case EquipmentV2.EquipmentType.Armor:
                        if (e.part == EquipmentV2.EqupimentParts.Head)
                        {
                            showAllApperance();
                        }
                        armorPlaceHolder[e.side][e.part].setEquipment(null);
                        break;

                    case EquipmentV2.EquipmentType.Body:
                        bodyPlaceHolder[e.side][e.part].setEquipment(null);
                        break;
                }
            }
        }
    }
}
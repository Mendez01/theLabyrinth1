using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor;

#endif

using UnityEngine;

/// <summary>
/// This is main core part of this asset.
/// This EquipmentManager, will deal with the equipment change and spawn new model with the equipment.
/// Also one of the main core functionality of this asset is that it will use bunch of armor set to creating random various item equipped enemy/npc
/// So that it will be reusable for many purpose.
///
/// Author  : Hwan Kim
/// Created : 16/03/2016
///
/// Version 2.0
/// </summary>

public class EquipmentManagerV2 : EquipmentController
{
    #region private variables

    private int currentLeftWeaponIndex = 0;
    private int currentRightWeaponIndex = 0;
    private string defaultRace = "Human";
    private string emptyFacialName = "Empty";
    private string previousHeadSetId;

    //set name dictionaries.
    private string currentSet = null;

    #endregion private variables

    #region public variables

    [Space(10)]
    [Header("Set false to see initialize Editor button")]
    public bool initialized = false;

    //Just for the character select scene.
    public Animator animator;

    public float initializationTime { get; set; }

    [Space(10)]
    [Header("List of set items in children")]
    public List<string> listOfSetArmors = new List<string>();

    [Space(10)]
    [Header("Values for random equipment character gen")]
    public bool randomFacialGeneration = false;

    public float randomLeftShoulderGearChancePercentage = 25;
    public float randomRightShoulderGearChancePercentage = 25;
    public float randomHeadGearChancePercentage = 30;
    public float randomSingleHandChancePercentage = 10;
    public float randomFacialFeatureChancePercentage = 45;
    public float randomArmorChancePercentage = 30;
    public float randomWeaponChancePercentage = 30;

    [Space(10)]
    [Header("!!Important!! path must be exist in the project under the Assets folder")]
    public string equipmentPrefabTargetPath = "Assets/HackAndSlashRpgKit/Resources/Prefabs/Equipments";

    [Space(10)]
    [Header("Target location for Spawn character")]
    public Vector3 spawnCharacterPos;

    #endregion public variables

    #region Dictionaries of equipment items.

    //entire dictionary of the loaded equipments, it can be easily found by equipment id.
    public Dictionary<string, EquipmentV2> allEquipments = new Dictionary<string, EquipmentV2>();

    //Armor
    private Dictionary<EquipmentV2.EquipmentSide, Dictionary<EquipmentV2.EqupimentParts, List<EquipmentV2>>> armorEquipments =
        new Dictionary<EquipmentV2.EquipmentSide, Dictionary<EquipmentV2.EqupimentParts, List<EquipmentV2>>>();

    //Weapons
    private Dictionary<EquipmentV2.EquipmentSide, List<EquipmentV2>> weaponEquipments =
   new Dictionary<EquipmentV2.EquipmentSide, List<EquipmentV2>>();

    //Racial features
    public Dictionary<string, List<EquipmentV2>> racialFeatures = new Dictionary<string, List<EquipmentV2>>();

    //Facial Equipments.
    protected Dictionary<EquipmentV2.EquipmentType, List<EquipmentV2>> facialEquipments = new Dictionary<EquipmentV2.EquipmentType, List<EquipmentV2>>();

    private Dictionary<EquipmentV2.EquipmentType, List<string>> listOfFacialSets =
        new Dictionary<EquipmentV2.EquipmentType, List<string>>();

    private Dictionary<EquipmentV2.EquipmentType, string> currentFacialSet =
        new Dictionary<EquipmentV2.EquipmentType, string>();

    #endregion Dictionaries of equipment items.

    #region loading all the equipment from equipment manager game object child.

    private void Awake()
    {
        if (!initialized)
        {
            init();
        }
    }

    private void Start()
    {
        if (allEquipments.Count == 0)
            init();
    }

    public void init()
    {
        myTran = transform;
        reset();
        characterEye = GetComponentInChildren<Eye>();

        animator = GetComponent<Animator>();
        initailizeEquipmentsInChildren();
        initialized = true;
        initializationTime = Time.realtimeSinceStartup;
        Debug.Log("[" + name + ".init] There are " + allEquipments.Count + " equipments in this manager " + name);
    }

    /// <summary>
    /// Main Equipment initializing process, will loop through the Equipments in child transform.
    /// </summary>
    private void initailizeEquipmentsInChildren()
    {
        foreach (EquipmentV2 e in GetComponentsInChildren<EquipmentV2>())
        {
            e.gameObject.SetActive(false);
            //For duplication
            if (!string.IsNullOrEmpty(e.equipmentDetails.id) && !allEquipments.ContainsKey(e.equipmentDetails.id))
            {
                allEquipments.Add(e.equipmentDetails.id, e);
                //Armor equipments.
                if (e.type == EquipmentV2.EquipmentType.Armor)
                {
                    ArmorPlaceHolderProcess(e);

                    initializeArmorEquipment(e);
                }
                //Weapon equipments.
                else if (e.type == EquipmentV2.EquipmentType.Weapon)
                {
                    weaponPlaceHolderProcess(e);

                    InitializeWeaponEquipment(e);
                }
                //Body parts.
                else if (e.type == EquipmentV2.EquipmentType.Body)
                {
                    bodyPlaceHolderProcess(e);
                    initializeBodyPart(e);
                    showEquipment(e);
                }
                //Appearances
                else if (e.isAppearanceEquipment())
                {
                    appearancePlcaeHolderProcess(e);

                    addFacialEqipments(e);
                    addFacialSet(e);
                }
                //Racial features
                else if (e.type == EquipmentV2.EquipmentType.Racial)
                {
                    addRacailEquipment(e);
                }

                //  if (e.type == Equipment.EquipmentType.Face)
                //you can generate method if there are more face type(face model)
                //With this asset there are only one face.
            }
        }
    }

    /// <summary>
    /// Will reset every thing with current EquipmentManager so that we can re-init again with clean start.
    /// </summary>
    private void reset()
    {
        setAllEquipmentActive();

        destroyPreviousPlaceHolders();
        allEquipments.Clear();
        armorEquipments.Clear();
        weaponEquipments.Clear();
        bodies.Clear();
        appearance.Clear();
        facialEquipments.Clear();
        equippedEquipmentIds.Clear();
        listOfSetArmors.Clear();
        racialFeatures.Clear();
        currentLeftWeaponIndex = 0;
        currentRightWeaponIndex = 0;
        currentSet = null;
        listOfFacialSets.Clear();
        currentFacialSet.Clear();
        currentRace = defaultRace;
    }

    private void destroyPreviousPlaceHolders()
    {
        foreach (GameObject placeHolder in GameObject.FindGameObjectsWithTag(placeHolderTag))
        {
            destory(placeHolder);
        }
    }

    #region initializing dictionaries of all equipment.

    private void initializeBodyPart(EquipmentV2 e)
    {
        if (!bodies.ContainsKey(e.side))
        {
            bodies.Add(e.side, new Dictionary<EquipmentV2.EqupimentParts, EquipmentV2>());
        }
        if (!bodies[e.side].ContainsKey(e.part))
        {
            bodies[e.side].Add(e.part, e);
        }
    }

    private void InitializeWeaponEquipment(EquipmentV2 e)
    {
        if (!weaponEquipments.ContainsKey(e.side))
        {
            weaponEquipments.Add(e.side, new List<EquipmentV2>());
        }
        weaponEquipments[e.side].Add(e);
    }

    private void initializeArmorEquipment(EquipmentV2 e)
    {
        if (!armorEquipments.ContainsKey(e.side))
        {
            armorEquipments.Add(e.side, new Dictionary<EquipmentV2.EqupimentParts, List<EquipmentV2>>());
        }
        if (!armorEquipments[e.side].ContainsKey(e.part))
        {
            armorEquipments[e.side].Add(e.part, new List<EquipmentV2>());
        }
        armorEquipments[e.side][e.part].Add(e);

        if (e.equipmentDetails.isSet && !listOfSetArmors.Contains(e.equipmentDetails.nameOfSet))
        {
            listOfSetArmors.Add(e.equipmentDetails.nameOfSet);
        }
    }

    #endregion initializing dictionaries of all equipment.

    #region initializing all place holder for changing/showing equipments

    private void bodyPlaceHolderProcess(EquipmentV2 e)
    {
        //Lets Create Body place holder, if there are not any.
        if (!bodyPlaceHolder.ContainsKey(e.side))
        {
            bodyPlaceHolder.Add(e.side, new Dictionary<EquipmentV2.EqupimentParts, EquipmentV2>());
        }
        if (!bodyPlaceHolder[e.side].ContainsKey(e.part))
        {
            EquipmentV2 initilizedBodyPlaceHodler = createPlaceHolderEquipment(e);
            initilizedBodyPlaceHodler.name = e.side + "_" + e.part + "_" + e.type + "_" + placeHolderTag;
            initilizedBodyPlaceHodler.setEquipment(null);
            bodyPlaceHolder[e.side].Add(e.part, initilizedBodyPlaceHodler);
        }
    }

    private void appearancePlcaeHolderProcess(EquipmentV2 e)
    {
        if (!appearancePlaceHolder.ContainsKey(e.type))
        {
            EquipmentV2 initilizedAppearancePlaceHodler = createPlaceHolderEquipment(e);
            initilizedAppearancePlaceHodler.setEquipment(null);
            initilizedAppearancePlaceHodler.name = "appearance_" + e.type + "_" + placeHolderTag;
            appearancePlaceHolder.Add(e.type, initilizedAppearancePlaceHodler);
        }
    }

    private void weaponPlaceHolderProcess(EquipmentV2 e)
    {
        if (!weaponPlaceHolder.ContainsKey(e.side))
        {
            EquipmentV2 initilizedWeaponPlaceHodler = createPlaceHolderEquipment(e);
            initilizedWeaponPlaceHodler.name = e.side + "_" + e.type + "_" + placeHolderTag;
            initilizedWeaponPlaceHodler.setEquipment(null);
            weaponPlaceHolder.Add(e.side, initilizedWeaponPlaceHodler);
        }
    }

    private void ArmorPlaceHolderProcess(EquipmentV2 e)
    {
        //Lets Create Armor place holder, if there are not any.
        if (!armorPlaceHolder.ContainsKey(e.side))
        {
            armorPlaceHolder.Add(e.side, new Dictionary<EquipmentV2.EqupimentParts, EquipmentV2>());
        }
        if (!armorPlaceHolder[e.side].ContainsKey(e.part))
        {
            EquipmentV2 initilizedArmorPlaceHodler = createPlaceHolderEquipment(e);
            initilizedArmorPlaceHodler.name = e.side + "_" + e.part + "_" + e.type + "_" + placeHolderTag;
            initilizedArmorPlaceHodler.setEquipment(null);
            armorPlaceHolder[e.side].Add(e.part, initilizedArmorPlaceHodler);
        }
    }

    private EquipmentV2 createPlaceHolderEquipment(EquipmentV2 e)
    {
        e.gameObject.SetActive(true);
        EquipmentV2 initilizedArmorPlaceHodler =
            Instantiate(e, e.transform.position, e.transform.rotation) as EquipmentV2;
        initilizedArmorPlaceHodler.transform.parent = e.transform.parent;
        initilizedArmorPlaceHodler.transform.localPosition = e.transform.localPosition;
        initilizedArmorPlaceHodler.transform.localRotation = e.transform.localRotation;
        initilizedArmorPlaceHodler.transform.localScale = e.transform.localScale;
        initilizedArmorPlaceHodler.placeHolder = true;
        initilizedArmorPlaceHodler.setEquipment(null);
        initilizedArmorPlaceHodler.tag = placeHolderTag;
        e.gameObject.SetActive(false);
        return initilizedArmorPlaceHodler;
    }

    #endregion initializing all place holder for changing/showing equipments

    private void addRacailEquipment(EquipmentV2 e)
    {
        if (!racialFeatures.ContainsKey(e.equipmentDetails.equipmentName))
        {
            racialFeatures.Add(e.equipmentDetails.equipmentName, new List<EquipmentV2>());
        }
        racialFeatures[e.equipmentDetails.equipmentName].Add(e);

        if (e.equipmentDetails.equipmentName.Equals(defaultRace))
        {
            showEquipment(e);
        }
    }

    private void addFacialEqipments(EquipmentV2 e)
    {
        if (!facialEquipments.ContainsKey(e.type))
        {
            facialEquipments.Add(e.type, new List<EquipmentV2>());
        }
        facialEquipments[e.type].Add(e);
    }

    private void addFacialSet(EquipmentV2 e)
    {
        if (!listOfFacialSets.ContainsKey(e.type))
        {
            listOfFacialSets.Add(e.type, new List<string>());
            //Add empty one.
            listOfFacialSets[e.type].Add(emptyFacialName);
            currentFacialSet[e.type] = emptyFacialName;
        }
        listOfFacialSets[e.type].AddNotContainOnly(e.equipmentDetails.equipmentName);
    }

    #endregion loading all the equipment from equipment manager game object child.

    #region Change Equipment to the next one from the list

    //change set of armor.
    public void changeSet(string newSet)
    {
        currentSet = newSet;
        disableCurrentArmorEquipment();
        disableCurrentBody();
        foreach (EquipmentV2.EqupimentParts part in Enum.GetValues(typeof(EquipmentV2.EqupimentParts)))
        {
            switch (part)
            {
                case EquipmentV2.EqupimentParts.Head:
                    EquipmentV2 head = getSetEquipmentFromDictionary(EquipmentV2.EquipmentSide.Middle, EquipmentV2.EqupimentParts.Head, armorEquipments, newSet);
                    //set can be without head gear.
                    if (head != null)
                        showEquipment(head);
                    else //show all appearance.
                    {
                        showAllApperance();
                    }
                    break;

                case EquipmentV2.EqupimentParts.Chest:

                    showEquipment(getSetEquipmentFromDictionary(EquipmentV2.EquipmentSide.Middle, EquipmentV2.EqupimentParts.Chest, armorEquipments, newSet));
                    break;

                case EquipmentV2.EqupimentParts.Shoulder:
                    showEquipment(getSetEquipmentFromDictionary(EquipmentV2.EquipmentSide.Left, EquipmentV2.EqupimentParts.Shoulder, armorEquipments, newSet));
                    showEquipment(getSetEquipmentFromDictionary(EquipmentV2.EquipmentSide.Right, EquipmentV2.EqupimentParts.Shoulder, armorEquipments, newSet));
                    break;

                case EquipmentV2.EqupimentParts.UpperArm:
                    showEquipment(getSetEquipmentFromDictionary(EquipmentV2.EquipmentSide.Left, EquipmentV2.EqupimentParts.UpperArm, armorEquipments, newSet));
                    showEquipment(getSetEquipmentFromDictionary(EquipmentV2.EquipmentSide.Right, EquipmentV2.EqupimentParts.UpperArm, armorEquipments, newSet));
                    break;

                case EquipmentV2.EqupimentParts.LowerArm:
                    showEquipment(getSetEquipmentFromDictionary(EquipmentV2.EquipmentSide.Left, EquipmentV2.EqupimentParts.LowerArm, armorEquipments, newSet));
                    showEquipment(getSetEquipmentFromDictionary(EquipmentV2.EquipmentSide.Right, EquipmentV2.EqupimentParts.LowerArm, armorEquipments, newSet));
                    break;

                case EquipmentV2.EqupimentParts.Hand:

                    EquipmentV2 handLeftEq = getSetEquipmentFromDictionary(EquipmentV2.EquipmentSide.Left, EquipmentV2.EqupimentParts.Hand, armorEquipments, newSet);
                    handLeftEq = handLeftEq == null ? bodies[EquipmentV2.EquipmentSide.Left][EquipmentV2.EqupimentParts.Hand] : handLeftEq;
                    showEquipment(handLeftEq);

                    EquipmentV2 handRightEq = getSetEquipmentFromDictionary(EquipmentV2.EquipmentSide.Right, EquipmentV2.EqupimentParts.Hand, armorEquipments, newSet);
                    handRightEq = handRightEq == null ? bodies[EquipmentV2.EquipmentSide.Right][EquipmentV2.EqupimentParts.Hand] : handRightEq;
                    showEquipment(handRightEq);
                    break;

                case EquipmentV2.EqupimentParts.UpperLeg:
                    showEquipment(getSetEquipmentFromDictionary(EquipmentV2.EquipmentSide.Left, EquipmentV2.EqupimentParts.UpperLeg, armorEquipments, newSet));
                    showEquipment(getSetEquipmentFromDictionary(EquipmentV2.EquipmentSide.Right, EquipmentV2.EqupimentParts.UpperLeg, armorEquipments, newSet));
                    break;

                case EquipmentV2.EqupimentParts.LowerLeg:
                    showEquipment(getSetEquipmentFromDictionary(EquipmentV2.EquipmentSide.Left, EquipmentV2.EqupimentParts.LowerLeg, armorEquipments, newSet));
                    showEquipment(getSetEquipmentFromDictionary(EquipmentV2.EquipmentSide.Right, EquipmentV2.EqupimentParts.LowerLeg, armorEquipments, newSet));
                    break;

                case EquipmentV2.EqupimentParts.Feet:
                    showEquipment(getSetEquipmentFromDictionary(EquipmentV2.EquipmentSide.Left, EquipmentV2.EqupimentParts.Feet, armorEquipments, newSet));
                    showEquipment(getSetEquipmentFromDictionary(EquipmentV2.EquipmentSide.Right, EquipmentV2.EqupimentParts.Feet, armorEquipments, newSet));
                    break;
            }
        }
    }

    //change face appearance
    public void changeToNextFacialSet(EquipmentV2.EquipmentType type)
    {
        //Check we have currently equipped appearance equipment.
        EquipmentV2 previousSet = findCurrentFacialEquipmentSetByType(type);
        //If we have then lets remove them from current equippedEquipmentIds.
        if (previousSet != null && equippedEquipmentIds.Contains(previousSet.equipmentDetails.id))
            equippedEquipmentIds.Remove(findCurrentFacialEquipmentSetByType(type).equipmentDetails.id);

        // Will get the next index or the fisrt index of the value from set name dictionary
        currentFacialSet[type] = listOfFacialSets[type].NextOf(currentFacialSet[type]);

        //If the set is not empty then let's set visible.
        if (!emptyFacialName.Equals(currentFacialSet[type]))
        {
            EquipmentV2 eq = findCurrentFacialEquipmentSetByType(type);
            showEquipment(eq);
        }
        else
        {
            appearancePlaceHolder[type].setEquipment(null);
        }
    }

    public void changerLeftHandSet()
    {
        disableCurrentWeaponSet(EquipmentV2.EquipmentSide.Left);
        EquipmentV2 rightHandwepon = findCurrentlyEquippedWeaponBySide(EquipmentV2.EquipmentSide.Right);
        if (rightHandwepon != null && rightHandwepon.equipmentDetails.twoHands)
            disableCurrentWeaponSet(EquipmentV2.EquipmentSide.Right);

        if (currentLeftWeaponIndex >= weaponEquipments[EquipmentV2.EquipmentSide.Left].Count)
            currentLeftWeaponIndex = 0;

        EquipmentV2 leftHandwepon = weaponEquipments[EquipmentV2.EquipmentSide.Left][currentLeftWeaponIndex++];
        if (leftHandwepon != null)
            showEquipment(leftHandwepon);

        setWeaponAnimationByWeapons();
    }

    public void changerRightHandSet()
    {
        disableCurrentWeaponSet(EquipmentV2.EquipmentSide.Right);
        if (currentRightWeaponIndex >= weaponEquipments[EquipmentV2.EquipmentSide.Right].Count)
            currentRightWeaponIndex = 0;

        EquipmentV2 rightHandwepon = weaponEquipments[EquipmentV2.EquipmentSide.Right][currentRightWeaponIndex++];
        if (rightHandwepon != null)
        {
            showEquipment(rightHandwepon);
            if (rightHandwepon.equipmentDetails.twoHands)
            {
                disableCurrentWeaponSet(EquipmentV2.EquipmentSide.Left);
            }
        }
        setWeaponAnimationByWeapons();
    }

    #endregion Change Equipment to the next one from the list

    #region Random character equipment creation.

    private EquipmentV2 getRandomArmorEquipment(EquipmentV2.EquipmentSide side, EquipmentV2.EqupimentParts part)
    {
        EquipmentV2 e = null;
        try
        {
            e = armorEquipments[side][part].GetRandomElement<EquipmentV2>();
        }
        catch
        {
            //Couldn't find armor with the side and part, so we will return body instead.
            e = bodies[side][part];
        }

        return e;
    }

    public void randomEquipment(bool isPlayer)
    {
        disableCurrentActiveEquipment();
        //10 % chance single handed
        bool singleHanded = false;
        bool onlyLeftHand = false, onlyRightHand = false;
        if (!isPlayer)
        {
            singleHanded = rollDiceToHundread() < randomSingleHandChancePercentage;

            if (singleHanded)
            {
                onlyLeftHand = rollDiceToHundread() < 50;
                onlyRightHand = !onlyLeftHand;
            }
        }

        foreach (EquipmentV2.EqupimentParts part in Enum.GetValues(typeof(EquipmentV2.EqupimentParts)))
        {
            if (part == EquipmentV2.EqupimentParts.Head)
            {
                //20 % chance head gear
                bool headGear = UnityEngine.Random.Range(1, 100) < randomHeadGearChancePercentage;

                if (randomFacialGeneration)
                {
                    randomAppearance();
                }

                if (headGear)
                {
                    EquipmentV2 randomeHeadGear =
                           armorEquipments[EquipmentV2.EquipmentSide.Middle][EquipmentV2.EqupimentParts.Head]
                           .GetRandomElement<EquipmentV2>();

                    showEquipment(randomeHeadGear);
                }
                else
                {
                    showAllApperance();
                }
            }
            else if (part == EquipmentV2.EqupimentParts.Chest)
            {
                RandomArmorProcess(EquipmentV2.EquipmentSide.Middle, part);
            }
            else if (part == EquipmentV2.EqupimentParts.Shoulder)
            {
                if (UnityEngine.Random.Range(1, 100) < randomLeftShoulderGearChancePercentage)
                    showEquipment(getRandomArmorEquipment(EquipmentV2.EquipmentSide.Left, part));
                if (UnityEngine.Random.Range(1, 100) < randomRightShoulderGearChancePercentage)
                    showEquipment(getRandomArmorEquipment(EquipmentV2.EquipmentSide.Right, part));
            }
            else if (part == EquipmentV2.EqupimentParts.UpperArm || part == EquipmentV2.EqupimentParts.LowerArm)
            {
                if (!onlyRightHand)
                {
                    RandomArmorProcess(EquipmentV2.EquipmentSide.Left, part);
                }
                if (!onlyLeftHand)
                {
                    RandomArmorProcess(EquipmentV2.EquipmentSide.Right, part);
                }
            }
            else if (part == EquipmentV2.EqupimentParts.Hand)
            {
                if (!onlyRightHand)
                    showEquipment(bodies[EquipmentV2.EquipmentSide.Left][part]);
                if (!onlyLeftHand)
                    showEquipment(bodies[EquipmentV2.EquipmentSide.Right][part]);
            }
            else
            {
                RandomArmorProcess(EquipmentV2.EquipmentSide.Left, part);
                RandomArmorProcess(EquipmentV2.EquipmentSide.Right, part);
            }
        }

        randomWeapons(onlyLeftHand, onlyRightHand);
    }

    private void RandomArmorProcess(EquipmentV2.EquipmentSide side, EquipmentV2.EqupimentParts part)
    {
        if (rollDiceToHundread() < randomArmorChancePercentage)

            showEquipment(getRandomArmorEquipment(side, part));
        else

            showEquipment(bodies[side][part]);
    }

    public void randomWeapons(bool onlyLeftHand, bool onlyRightHand)
    {
        EquipmentV2 rigtHandWeapon = null, leftHandWeapon = null;
        bool twoHandWeapon = false;
        if (!onlyLeftHand && rollDiceToHundread() <= randomWeaponChancePercentage)
        {
            rigtHandWeapon = weaponEquipments[EquipmentV2.EquipmentSide.Right].GetRandomElement<EquipmentV2>();
            if (rigtHandWeapon != null)
            {
                twoHandWeapon = rigtHandWeapon.equipmentDetails.twoHands;
                showEquipment(rigtHandWeapon);
            }
        }
        if (!onlyRightHand && !twoHandWeapon && rollDiceToHundread() <= randomWeaponChancePercentage)
        {
            leftHandWeapon = weaponEquipments[EquipmentV2.EquipmentSide.Left].GetRandomElement<EquipmentV2>();
            if (leftHandWeapon != null)
            {
                showEquipment(leftHandWeapon);
            }
        }
        setWeaponAnimationByWeapons();
    }

    private void ramdomRace()
    {
        try
        {
            currentRace = racialFeatures.GetRandomKey<string, List<EquipmentV2>>();
            foreach (EquipmentV2 e in racialFeatures[currentRace])
            {
                showEquipment(e);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void randomAppearance()
    {
        disableCurrentFacialStuff();

        if (rollDiceToHundread() < randomFacialFeatureChancePercentage)
            showEquipment(facialEquipments[EquipmentV2.EquipmentType.FaceAccesary].GetRandomElement<EquipmentV2>());
        if (rollDiceToHundread() < randomFacialFeatureChancePercentage)
            showEquipment(facialEquipments[EquipmentV2.EquipmentType.Hair].GetRandomElement<EquipmentV2>());
        if (rollDiceToHundread() < randomFacialFeatureChancePercentage)
            showEquipment(facialEquipments[EquipmentV2.EquipmentType.Eyebrows].GetRandomElement<EquipmentV2>());
        if (rollDiceToHundread() < randomFacialFeatureChancePercentage)
            showEquipment(facialEquipments[EquipmentV2.EquipmentType.FacialHair].GetRandomElement<EquipmentV2>());
        changeRandomRace();
        characterEye.randomEyeColorChange();
    }

    #endregion Random character equipment creation.

    #region character spawn methods

    /// <summary>
    /// Main character spawn method, this should be used for equipment change.
    /// Will Instantiate new GameObjects with equipments and will spawn at the parameter position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public GameObject spawnTrimmedCharacterWithCrrentEquipments(Vector3 position)
    {
        //Instantiate current Equipment master (which is the Character 3D model with all the equipments.)
        GameObject trimmedCharacter = Instantiate(gameObject, position, Quaternion.identity) as GameObject;
        //Set scale as one which is default.
        trimmedCharacter.transform.localScale = Vector3.one;

        //Destroy equipmentManager that we don't need with new character.
        destory(trimmedCharacter.GetComponent<EquipmentManagerV2>());
        //From equipment change with the spawned character will have to deal with the EquipmentController.
        EquipmentController equipmentContoller = trimmedCharacter.AddComponent<EquipmentController>();
        //This method will find all place holder and destroy all the other Equipments
        equipmentContoller.initContoller(bodies, appearance, currentRace);

        //This CharacterEquipments can be used as character equipment info, for your game will internally use EquipmentController.
        CharacterEquipmentsV2 newCharacterEquipment = trimmedCharacter.AddComponent<CharacterEquipmentsV2>();
        newCharacterEquipment.equipmentContoller = equipmentContoller;

        //Let's equip all the currently equipped item.
        foreach (string id in equippedEquipmentIds)
        {
            newCharacterEquipment.equip(allEquipments[id]);
        }

        //This will set the weaponType so that animation will get picked up right animation with right equipped weapon.
        newCharacterEquipment.setWeaponType();

        //Lets return this new equipped character.
        return trimmedCharacter;
    }

    #endregion character spawn methods

    #region get/find methods

    public List<EquipmentV2> getNextRaceFeatures()
    {
        currentRace = racialFeatures.NextKeyOf(currentRace);
        return racialFeatures[currentRace];
    }

    private void changeRandomRace()
    {
        currentRace = racialFeatures.GetRandomKey<string, List<EquipmentV2>>();
        changeRace(racialFeatures[currentRace]);
    }

    public void changeRace(List<EquipmentV2> racialFeaures)
    {
        disableCurrentRaceSet();
        foreach (EquipmentV2 e in racialFeaures)
        {
            showEquipment(e);
        }
    }

    private EquipmentV2 findCurrentFacialEquipmentSetByType(EquipmentV2.EquipmentType type)
    {
        return facialEquipments[type].Find(equipment => currentFacialSet[type].Equals(equipment.equipmentDetails.equipmentName));
    }

    public string getNextArmorSet()
    {
        return listOfSetArmors.NextOf(currentSet);
    }

    private EquipmentV2 getSetEquipmentFromDictionary(EquipmentV2.EquipmentSide side, EquipmentV2.EqupimentParts part,
        Dictionary<EquipmentV2.EquipmentSide, Dictionary<EquipmentV2.EqupimentParts, List<EquipmentV2>>> dic, string setName)
    {
        EquipmentV2 e = null;
        try
        {
            e = dic[side][part].Find(item => item.equipmentDetails.nameOfSet.Equals(setName));
            if (e == null)
                e = bodies[side][part];
        }
        catch
        {
            //ignore, it can be triggered by change set item with missing parts.
        }
        return e;
    }

    private EquipmentV2 findCurrentlyEquippedWeaponBySide(EquipmentV2.EquipmentSide side)
    {
        try
        {
            return allEquipments[weaponPlaceHolder[side].equipmentDetails.id];
        }
        catch
        {
            return null;
        }
    }

    public EquipmentV2 getEquipmentById(string id)
    {
        try
        {
            return allEquipments[id];
        }
        catch
        {
            return null;
        }
    }

    #endregion get/find methods

    #region un-equipping/invisible equipment methods

    public void hideHeadGear(bool on)
    {
        if (!on)
        {
            if (armorPlaceHolder[EquipmentV2.EquipmentSide.Middle][EquipmentV2.EqupimentParts.Head].isValidEquipment)
            {
                previousHeadSetId = armorPlaceHolder[EquipmentV2.EquipmentSide.Middle][EquipmentV2.EqupimentParts.Head].equipmentDetails.id;

                hideEquipment(armorPlaceHolder[EquipmentV2.EquipmentSide.Middle][EquipmentV2.EqupimentParts.Head]);
            }
        }
        else if (previousHeadSetId != null)
        {
            showEquipment(allEquipments[previousHeadSetId]);
        }
    }

    private void disableCurrentActiveEquipment()
    {
        foreach (string id in equippedEquipmentIds)
        {
            try
            {
                EquipmentV2 e = allEquipments[id];
                if (e.type == EquipmentV2.EquipmentType.Weapon)
                {
                    weaponPlaceHolder[e.side].setEquipment(null);
                }
                else if (e.type == EquipmentV2.EquipmentType.Armor)
                {
                    armorPlaceHolder[e.side][e.part].setEquipment(null);
                }
                else if (e.type == EquipmentV2.EquipmentType.Body)
                {
                    bodyPlaceHolder[e.side][e.part].setEquipment(null);
                }
                else if (e.isAppearanceEquipment())
                {
                    appearancePlaceHolder[e.type].setEquipment(null);
                }
                else
                {
                    e.gameObject.SetActive(false);
                }
            }
            catch
            {
                //If it reaches here then not much thing to do, so just ignore it.
            }
        }
        equippedEquipmentIds.Clear();
    }

    private void disableCurrentRaceSet()
    {
        foreach (string id in new List<string>(equippedEquipmentIds))
        {
            EquipmentV2 e = allEquipments[id];
            if (e.type == EquipmentV2.EquipmentType.Racial)
            {
                e.gameObject.SetActive(false);
                equippedEquipmentIds.Remove(id);
            }
        }
    }

    public void disableCurrentWeaponSet()
    {
        if (weaponPlaceHolder[EquipmentV2.EquipmentSide.Left].isValidEquipment)
        {
            equippedEquipmentIds.Remove(weaponPlaceHolder[EquipmentV2.EquipmentSide.Left].equipmentDetails.id);
            weaponPlaceHolder[EquipmentV2.EquipmentSide.Left].setEquipment(null);
        }
        if (weaponPlaceHolder[EquipmentV2.EquipmentSide.Right].isValidEquipment)
        {
            equippedEquipmentIds.Remove(weaponPlaceHolder[EquipmentV2.EquipmentSide.Right].equipmentDetails.id);
            weaponPlaceHolder[EquipmentV2.EquipmentSide.Right].setEquipment(null);
        }
    }

    public void disableCurrentWeaponSet(EquipmentV2.EquipmentSide side)
    {
        if (weaponPlaceHolder[side].isValidEquipment)
        {
            string weaponId = weaponPlaceHolder[side].equipmentDetails.id;
            equippedEquipmentIds.Remove(weaponId);
            weaponPlaceHolder[side].setEquipment(null);
        }
    }

    private void disableCurrentFacialStuff()
    {
        foreach (EquipmentV2 e in appearancePlaceHolder.Values)
        {
            if (e.isValidEquipment)
            {
                equippedEquipmentIds.Remove(e.equipmentDetails.id);
                appearancePlaceHolder[e.type].setEquipment(null);
            }
        }
    }

    private void disableCurrentArmorEquipment()
    {
        foreach (EquipmentV2.EquipmentSide side in armorPlaceHolder.Keys)
        {
            foreach (EquipmentV2.EqupimentParts part in armorPlaceHolder[side].Keys)
            {
                if (armorPlaceHolder[side][part].isValidEquipment)
                {
                    equippedEquipmentIds.Remove(armorPlaceHolder[side][part].equipmentDetails.id);
                    armorPlaceHolder[side][part].setEquipment(null);
                }
            }
        }
    }

    private void disableCurrentBody()
    {
        foreach (EquipmentV2.EquipmentSide side in bodyPlaceHolder.Keys)
        {
            foreach (EquipmentV2.EqupimentParts part in bodyPlaceHolder[side].Keys)
            {
                if (bodyPlaceHolder[side][part].isValidEquipment)
                {
                    equippedEquipmentIds.Remove(bodyPlaceHolder[side][part].equipmentDetails.id);
                    bodyPlaceHolder[side][part].setEquipment(null);
                }
            }
        }
    }

    #endregion un-equipping/invisible equipment methods

    #region animation related methods

    private void setWeaponAnimationByWeapons()
    {
        bool twoHandWeapon = false;
        CharacterEquipmentsV2.WeaponType weaponType = CharacterEquipmentsV2.WeaponType.UnArmed;

        if (weaponPlaceHolder[EquipmentV2.EquipmentSide.Right].isValidEquipment)
        {
            twoHandWeapon = weaponPlaceHolder[EquipmentV2.EquipmentSide.Right].equipmentDetails.twoHands;
            weaponType = twoHandWeapon ? CharacterEquipmentsV2.WeaponType.TwoHands : CharacterEquipmentsV2.WeaponType.OneHand;
        }
        if (weaponPlaceHolder[EquipmentV2.EquipmentSide.Left].isValidEquipment && !twoHandWeapon)
        {
            weaponType = weaponPlaceHolder[EquipmentV2.EquipmentSide.Left].equipmentDetails.dualweild
                ? CharacterEquipmentsV2.WeaponType.DualWeild : CharacterEquipmentsV2.WeaponType.OneHand;
        }
        setAnimationByWeaponType(weaponType);
    }

    private void setAnimationByWeaponType(CharacterEquipmentsV2.WeaponType weaponType)
    {
        resetAnimationType();
        animator.SetBool(weaponType.ToString(), true);
    }

    private void resetAnimationType()
    {
        animator.SetBool(CharacterEquipmentsV2.WeaponType.TwoHands.ToString(), false);
        animator.SetBool(CharacterEquipmentsV2.WeaponType.UnArmed.ToString(), false);
        animator.SetBool(CharacterEquipmentsV2.WeaponType.OneHand.ToString(), false);
        animator.SetBool(CharacterEquipmentsV2.WeaponType.DualWeild.ToString(), false);
    }

    #endregion animation related methods

    #region Scrapping all the equipment to prefabs

#if UNITY_EDITOR

    public void startSavingPrefabs()
    {
        Debug.Log("Staring startSavingPrefabs method");
        foreach (EquipmentV2 eq in GetComponentsInChildren<EquipmentV2>())
        {
            if (!existEquipmentPrefab(eq) && isValidType(eq))
            {
                createEquipmentToPrefab(eq);
            }
        }
        Debug.Log("Completed startSavingPrefabs method");
    }

    /// <summary>
    /// This will create Armor and Weapon equipment into Prefab, so that it can be used.
    /// </summary>
    /// <param name="eq"></param>
    private void createEquipmentToPrefab(EquipmentV2 eq)
    {
        string fileName = eq.getPrefabFileName();
        Debug.Log(eq);
        UnityEngine.Object emptyObj = PrefabUtility.CreateEmptyPrefab(equipmentPrefabTargetPath + fileName);
        //Clone it
        GameObject clonedItem = (GameObject)Instantiate(eq.gameObject, Vector3.zero, Quaternion.identity);
        Debug.Log(clonedItem);
        Debug.Log("createEquipmentToPrefab " + clonedItem.name);
        clonedItem.transform.parent = null;
        clonedItem.transform.localScale = Vector3.one;
        //All equipments are skinned mesh, so let's convert into MeshRenderer to modify transform.
        SkinnedMeshToMeshV2 newMesh = clonedItem.AddComponent<SkinnedMeshToMeshV2>();
        newMesh.process();
        DestroyImmediate(newMesh);
        clonedItem.name = fileName;
        PrefabUtility.ReplacePrefab(clonedItem, emptyObj, ReplacePrefabOptions.ConnectToPrefab);
        DestroyImmediate(clonedItem);
    }

    private bool isValidType(EquipmentV2 eq)
    {
        return (eq.type == EquipmentV2.EquipmentType.Armor || eq.type == EquipmentV2.EquipmentType.Weapon);
    }

    private bool existEquipmentPrefab(EquipmentV2 eq)
    {
        UnityEngine.Object equipmentPrefab;
        try
        {
            equipmentPrefab = Resources.Load("Prefabs/Equipments" + eq.getPrefabFileName());
        }
        catch
        {
            return false;
        }
        return equipmentPrefab != null;
    }

#endif

    #endregion Scrapping all the equipment to prefabs

    #region etc..

    public void equipEquipmentToCharacter(string id, BaseCharacterController character)
    {
        EquipmentV2 eq = allEquipments[id];
        character.equipmentContoller.showEquipment(eq);
    }

    private int rollDiceToHundread()
    {
        return UnityEngine.Random.Range(1, 101);
    }

    #endregion etc..
}
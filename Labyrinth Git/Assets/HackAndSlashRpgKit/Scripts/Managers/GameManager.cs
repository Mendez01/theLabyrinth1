using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the main method to managing battle game scene.
/// This will deal with initializing spawning players and enemies.
/// This will also deal with item drops as well.
///
/// Author  :   Hwan Kim
/// </summary>
public class GameManager : MonoBehaviour
{
    #region singleton

    static private GameManager _instance;

    static public GameManager instance
    {
        get
        {
            if (!initialized)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
                initialized = true;
            }
            return _instance;
        }
    }

    #endregion singleton

    #region variables

    public bool playerCanDie = false;
    public bool godMode = false;
    public EquipmentManagerV2 equipmentManager;
    public PlayerBagViewContoller playerBagViewCon;
    private PlayerEquipmentControl playerEquipmentViewCon;
    public GameObject player;
    public PlayerControllerV2 pc;

    public int initialEnemySpawnNumber = 10;
    public float maxEnemy = 20;

    [Header("Will only keep max allowed numbers of objects")]
    public float maxDeadBodyAllowed = 10;

    public float maxItemAllowed = 25;

    public string deadEnemyLayerName, enemyLayerName, playerLayerName;

    [Header("There are some weird physical issue with rag-doll.")]
    public bool turnOffRagDollDeadAnimationAfter = false;

    [Header("It is recommended to turn off and set interval less then 5 sec.")]
    public float intervalSecToTurnOffRagDoll = 5.5f;

    public int deadBodyLayer { get; set; }
    public int enemyLayer { get; set; }
    public int playerLayer { get; set; }

    private int _nextEnemyId = 0;

    public int nextEnemyId
    {
        get
        {
            return _nextEnemyId++;
        }
    }

    private static bool initialized = false;
    private ItemDropManager dropManager;
    private CameraControllerV2 camCon;
    private DroppedItemViewController droppedItemViewCon;
    private RigidbodyConstraints freezeRigidbody = RigidbodyConstraints.FreezeRotation;
    private Dictionary<int, BaseCharacterController> spwanedEnemies = new Dictionary<int, BaseCharacterController>();
    private Queue<GameObject> deadBodies = new Queue<GameObject>();
    private Queue<GameObject> droppedItems = new Queue<GameObject>();
    private Dictionary<GameObject, DroppedItemView> dropppedItemAndViewPair = new Dictionary<GameObject, DroppedItemView>();
    private int killCount = 0;
    private Transform deadOneParents;
    private Transform droppedItemParents;

    #endregion variables

    #region initialization

    /// <summary>
    /// This is set up the game before it gets started.
    /// </summary>
    private void Start()
    {
        init();
        killCount = 0;
        BattleGUIControl.instance.hideInfo();
    }

    private void init()
    {
        deadOneParents = new GameObject("DeadOneParent").transform;
        droppedItemParents = new GameObject("DroppedItemsParent").transform;
        droppedItemViewCon = GameObject.FindObjectOfType<DroppedItemViewController>();
        playerBagViewCon = GameObject.FindObjectOfType<PlayerBagViewContoller>();
        playerEquipmentViewCon = GameObject.FindObjectOfType<PlayerEquipmentControl>();
        playerLayer = LayerMask.NameToLayer(playerLayerName);
        enemyLayer = LayerMask.NameToLayer(enemyLayerName);
        deadBodyLayer = LayerMask.NameToLayer(deadEnemyLayerName);

        if (camCon == null)
        {
            camCon = GameObject.FindObjectOfType<CameraControllerV2>();
        }
        if (ItemDropManager.instance == null)
        {
            dropManager = gameObject.AddComponent<ItemDropManager>();
        }

        if (GameDataManager.Instance == null)
        {
            gameObject.AddComponent<GameDataManager>();
            equipmentManager = GameObject.FindObjectOfType<EquipmentManagerV2>();
            if (equipmentManager == null)
            {
                equipmentManager = (Instantiate(Resources.Load("Prefabs/EquipmentManagerV2"), new Vector3(0, -500, 0), Quaternion.identity) as GameObject).GetComponent<EquipmentManagerV2>();
                equipmentManager.initialized = false;
                equipmentManager.init();
            }
            equipmentManager.randomEquipment(true);
            player = equipmentManager.spawnTrimmedCharacterWithCrrentEquipments(Vector3.zero);
            player.gameObject.name = "Player";
        }
        else
        {
            equipmentManager = GameDataManager.Instance.equipmentManager;
            //get player.
            player = equipmentManager.spawnTrimmedCharacterWithCrrentEquipments(Vector3.zero);
            player.name = "Player";
            GameDataManager.Instance.player = player;
        }
        initPlayer(initializePlayerInfo());
        spawnRandomEnemies(initialEnemySpawnNumber);
    }

    #endregion initialization

    #region player related

    private void initPlayer(RPGCharacter playerInfo)
    {
        pc = player.AddComponent<PlayerControllerV2>();
        pc.setPhygicalBody(freezeRigidbody);
        pc.onTargetChanged += pc_onTargetChanged;
        pc.onKill += pc_onKill;
        pc.characterInfo = playerInfo;
        pc.initialCharacterEquipment();
        pc.gameObject.layer = playerLayer;
        pc.deadBodyLayer = deadBodyLayer;
        PlayerInfoManager.instance.setPlayer(pc.characterInfo);
        camCon.init(pc.myTransform);
        playerEquipmentViewCon.addListenerToPlayerEquipmentChange(pc.currentEquipment);
    }

    /// <summary>
    /// Please do implements your game rule for populating player RPG status information.
    /// This could get it from save file or simply set to initialized player attributes.
    /// </summary>
    /// <returns></returns>
    private RPGCharacter initializePlayerInfo()
    {
        RPGCharacter playerLevel1 = new RPGCharacter();
        playerLevel1.characterName = "The Hero";
        playerLevel1.maxHP = 20;
        playerLevel1.currentHP = playerLevel1.maxHP;
        playerLevel1.currentExp = 0;
        playerLevel1.attackSpeed = 1;
        playerLevel1.movementSpeed = 1.7f;
        playerLevel1.strength = 1;
        playerLevel1.dex = 1;
        playerLevel1.wisdom = 1;
        playerLevel1.level = 1;
        if (godMode)
        {
            playerLevel1.attackSpeed = 3;
            playerLevel1.movementSpeed = 3;
            playerLevel1.strength = 100;
        }
        return playerLevel1;
    }

    private void pc_onKill(RPGCharacter target)
    {
        PlayerInfoManager.instance.killCount.text = "" + (++killCount);
    }

    private void pc_onTargetChanged(RPGCharacter target)
    {
        BattleGUIControl.instance.setTarget(target);
    }

    #endregion player related

    #region enemy related

    private void spawnRandomEnemies(int numberOfEnemies)
    {
        for (int i = -numberOfEnemies; i < numberOfEnemies; i++)
        {
            Vector3 spwanLocation;
            if (i < 0)
            {
                spwanLocation = new Vector3(UnityEngine.Random.Range(i, 5 * i), 0, UnityEngine.Random.Range(i, 5 * i));
            }
            else
                spwanLocation = new Vector3(UnityEngine.Random.Range(1, 5), 0, UnityEngine.Random.Range(1, 5));
            spwanEnemy(spwanLocation, RandomName.randomFullName);
        }
    }

    public void spwanEnemy(Vector3 spwanLocation, String name)
    {
        equipmentManager.randomEquipment(false);
        GameObject spwanedEnemy = equipmentManager.spawnTrimmedCharacterWithCrrentEquipments(spwanLocation);
        spwanedEnemy.name = name;
        spwanedEnemy.layer = enemyLayer;
        EnemyController enemyControl = spwanedEnemy.AddComponent<EnemyController>();
        int level = UnityEngine.Random.Range(1, 10);
        enemyControl.characterInfo.id = nextEnemyId;
        //this is for the physic's collision matrix.
        enemyControl.deadBodyLayer = deadBodyLayer;
        enemyControl.characterInfo.currentExp = enemyControl.characterInfo.level;
        enemyControl.characterInfo.characterName = spwanedEnemy.name;
        enemyControl.characterInfo.attackSpeed = UnityEngine.Random.Range(0.5f, 1.5f);
        enemyControl.characterInfo.movementSpeed = UnityEngine.Random.Range(1.5f, 2);
        enemyControl.characterInfo.maxHP = UnityEngine.Random.Range(5, 10);
        enemyControl.characterInfo.levelUpProcess(level);
        enemyControl.setPhygicalBody(freezeRigidbody);
        enemyControl.addWonderor();
        enemyControl.initialCharacterEquipment();

        //Set OnDeath event for dropping item.
        enemyControl.characterInfo.died += onEnemyDeath;
        spwanedEnemies.Add(enemyControl.characterInfo.id, enemyControl);
    }

    /// <summary>
    /// Event handler on spawned enemy.
    /// Will check the drop roll and will drop the item from the chacter equipment.
    /// </summary>
    /// <param name="thisCharacter"></param>
    private void onEnemyDeath(RPGCharacter thisCharacter)
    {
        //This will keep the place clean. could be for the performance as well.
        deadBodyCleanProcess(thisCharacter);
        //Run the roll to check there are any loot.
        if (ItemDropManager.instance.anyDrop())
        {
            itemDropProcess(thisCharacter);
        }
        // And the enemy is dead so let's remove it from the spawned enemy dic.
        spwanedEnemies.Remove(thisCharacter.id);
        //Let check we have enough enemy in the field.
        if (spwanedEnemies.Count < maxEnemy)
        {
            spwanEnemy(new Vector3(UnityEngine.Random.Range(-5, 5), 0, UnityEngine.Random.Range(-5, 5)), RandomName.randomFullName);
        }
    }

    /// <summary>
    /// This is the method will get the random equipment from the enemy that wears.
    /// </summary>
    /// <param name="thisCharacter"></param>
    private void itemDropProcess(RPGCharacter thisCharacter)
    {
        /** You can add here more drops if you want. **/
        int numberOfDrops = ItemDropManager.instance.getNumberOfDropItem();

        for (int currentDroppedItemNum = 0; currentDroppedItemNum < numberOfDrops; currentDroppedItemNum++)
        {
            dropItem(spwanedEnemies[thisCharacter.id]);
        }
    }

    /// <summary>
    /// This will keep the place clean, will delete if there are more dead body then the max dead body allowed.
    /// </summary>
    /// <param name="thisCharacter"></param>
    private void deadBodyCleanProcess(RPGCharacter thisCharacter)
    {
        spwanedEnemies[thisCharacter.id].myTransform.parent = deadOneParents;
        deadBodies.Enqueue(spwanedEnemies[thisCharacter.id].gameObject);
        if (deadBodies.Count >= maxDeadBodyAllowed)
        {
            Destroy(deadBodies.Dequeue());
        }
    }

    /// <summary>
    /// This will get random item from the dead one and drop the loot to world space.
    /// </summary>
    /// <param name="characterCon"></param>
    public void dropItem(BaseCharacterController characterCon)
    {
        Vector3 location = getDroppableLocation(characterCon.myTransform);
        try
        {
            EquipmentV2 dropItem = characterCon.getRandomEquippedGear();
            characterCon.currentEquipment.unequipped(dropItem);

            dropItemProcess(location, dropItem);
        }
        catch
        {
            //Hm...KeyNotFoundException: The given key was not present in the dictionary.
        }
    }

    private Vector3 dropItemProcess(Vector3 location, EquipmentV2 dropItem)
    {
        GameObject spawnedItem = ItemDropManager.instance.dropItem(dropItem, location);
        if (droppedItems.Count >= maxItemAllowed)
        {
            GameObject doppedItem = droppedItems.Dequeue();
            dropppedItemAndViewPair[doppedItem].destoryTheView();
            dropppedItemAndViewPair.Remove(doppedItem);
            Destroy(doppedItem);
        }

        createDropItemView(spawnedItem);
        return location;
    }

    private void createDropItemView(GameObject spawnedItem)
    {
        DroppedItemView view = droppedItemViewCon.itemDropped(spawnedItem.GetComponent<EquipmentV2>());
        dropppedItemAndViewPair.Add(spawnedItem, view);
        spawnedItem.transform.parent = droppedItemParents;
        droppedItems.Enqueue(spawnedItem);
    }

    public Vector3 getDroppableLocation(Transform myTran)
    {
        Vector3 location = myTran.position;

        //Random location offset.
        location.y += 3;
        location.x += UnityEngine.Random.Range(-0.5f, 0.5f);
        location.z += UnityEngine.Random.Range(-0.5f, 0.5f);
        return location;
    }

    #endregion enemy related

    #region player equipment change

    private void InventoryItemEquipProcess()
    {
        if (playerBagViewCon.currentlyEquippedItem != null)
        {
            pc.currentEquipment.unequipped(playerBagViewCon.currentlyEquippedItem);
        }
        EquipmentV2 equipRequestedItem = playerBagViewCon.inventoryItem;
        pc.currentEquipment.equip(equipRequestedItem);
        playerBagViewCon.equipItemProcess();
    }

    #region Equipment Slot clicked

    public void onEquipOrUnEquipbuttonClicked(ItemDetailView itemDetailView)
    {
        switch (itemDetailView.itemFrom)
        {
            case ItemDetailView.ItemFrom.EquipmentSlot:
                //TODO check it is un-equiping or swapping.
                unequipEquipmentProcess(itemDetailView);
                break;

            case ItemDetailView.ItemFrom.InvenotrySlot:
                InventoryItemEquipProcess();
                break;
        }

        playerBagViewCon.hideBothItemDetailViews();
    }

    private void unequipEquipmentProcess(ItemDetailView itemDetailView)
    {
        //Get the un-equip requested equipment.
        EquipmentV2 unequipingEquipment = itemDetailView.currentViewEqiupment;
        unequipEqipment(unequipingEquipment);
        //let's try to put it back to the bag.
        if (!playerBagViewCon.insertItem(unequipingEquipment))
        {
            //no space so we are dropping it into the ground.
            ItemDropManager.instance.dropItem(unequipingEquipment, pc.myTransform.position);
            playerBagViewCon.hideBothItemDetailViews();
        }
    }

    private void unequipEqipment(EquipmentV2 equipment)
    {
        //Lets remove the equipment from equipment view.
        playerEquipmentViewCon.removeEquipmentWihtSideAndPart(equipment.side, equipment.part);
        //Un-equip from actual character.
        pc.currentEquipment.unequipped(equipment);
        //update the animation if it is weapon.
        if (equipment.type == EquipmentV2.EquipmentType.Weapon)
            pc.setWeaponTypeAnimation(pc.currentEquipment.weaponType);
    }

    public void onDropButtonClicked(ItemDetailView itemDetailView)
    {
        //TODO drop the item.
        EquipmentV2 droppingEquipment = itemDetailView.currentViewEqiupment;
        switch (itemDetailView.itemFrom)
        {
            case ItemDetailView.ItemFrom.InvenotrySlot:
                playerBagViewCon.removeItem(droppingEquipment);
                break;

            case ItemDetailView.ItemFrom.EquipmentSlot:
                unequipEqipment(droppingEquipment);
                break;
        }
        dropItemProcess(pc.myTransform.position, droppingEquipment);
        playerBagViewCon.hideBothItemDetailViews();
    }

    #endregion Equipment Slot clicked

    #endregion player equipment change
}
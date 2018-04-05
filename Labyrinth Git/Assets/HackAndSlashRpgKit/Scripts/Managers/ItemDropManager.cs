using System.Collections;
using UnityEngine;

public class ItemDropManager : MonoBehaviour
{
    //Event handlers for the character value changes.
    public delegate void OnItemDrop(EquipmentV2 droppedItem);

    public event OnItemDrop itemDropped;

    public string dropItemLayerName;
    public string droppedItemTag = "DroppedItem";
    public int minDropItemNum = 1;
    public int maxDropItemNum = 3;
    public int dropItemLayer;
    public float dropChancePercentage = 20;
    public float dropItemSizeOffset = 100;

    public Vector3 particleEffectSize = new Vector3(0.25f, 0.25f, 0.25f);

    private static bool initialized = false;
    static private ItemDropManager _instance;

    static public ItemDropManager instance
    {
        get
        {
            if (!initialized)
            {
                _instance = GameObject.FindObjectOfType<ItemDropManager>();

                initialized = true;
            }
            return _instance;
        }
    }

    private void Awake()
    {
        dropItemLayer = LayerMask.NameToLayer(dropItemLayerName);
    }

    public int getNumberOfDropItem()
    {
        return Random.Range(minDropItemNum, maxDropItemNum);
    }

    public GameObject dropItem(EquipmentV2 item, Vector3 location)
    {
        //Clone it
        EquipmentV2 droppedItem = Instantiate(item, location, UnityEngine.Random.rotation) as EquipmentV2;
        droppedItem.tag = droppedItemTag;
        droppedItem.name = item.name;
        droppedItem.init();
        droppedItem.transform.localScale = Vector3.one * dropItemSizeOffset;
        droppedItem.gameObject.layer = dropItemLayer;

        //All items are skinned mesh, so let's convert into MeshRenderer to modify transform.
        SkinnedMeshToMeshV2 newMesh = droppedItem.gameObject.AddComponent<SkinnedMeshToMeshV2>();
        newMesh.process();
        //Put physical body for dropping effect.
        MeshCollider meshCol = droppedItem.gameObject.AddComponent<MeshCollider>();
        meshCol.sharedMesh = newMesh.meshFilter.mesh;
        try
        {
            meshCol.material = GameDataManager.Instance.dropItemPhysicMaterial;
        }
        catch
        {
            meshCol.material = Resources.Load("DropItem") as PhysicMaterial;
        }
        meshCol.convex = true;
        droppedItem.gameObject.AddComponent<Rigidbody>();

        //Put drop item effect for player to check item easily.
        GameObject itemDropEffect = BattleEffectManager.instance.spwanItemDropEffect(droppedItem.transform);
        itemDropEffect.transform.parent = droppedItem.transform;
        itemDropEffect.transform.localScale = particleEffectSize;
        //you can add more stuff from here such as change particle color for different type of item(As Diablo 3 does.)
        itemDropEffect.GetComponent<ParticleSystem>().startColor = droppedItem.getRarityColor();
        droppedItem.gameObject.SetActive(true);
        if (itemDropped != null)
        {
            itemDropped(droppedItem);
        }
        return droppedItem.gameObject;
    }

    public bool anyDrop()
    {
        return Random.Range(0, 101) <= dropChancePercentage;
    }
}
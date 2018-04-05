using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroppedItemViewController : MonoBehaviour
{
    // Use this for initialization
    public RectTransform mainCanvasRectTran;

    public DroppedItemView itemDropViewPrefab;
    public Vector3 inventoryScale = new Vector3(0.35f, 0.35f, 0.35f);

    //This will prevent dropped ui draw on the top of other UIs.
    public Transform container;

    private void Start()
    {
    }

    /// <summary>
    /// This is event handler from Item Drop Manager, it will get triggered when Item manager drops the item.
    /// </summary>
    /// <param name="droppedItem"></param>
    public DroppedItemView itemDropped(EquipmentV2 droppedItem)
    {
        DroppedItemView droppedItemView = Instantiate(itemDropViewPrefab, Vector3.zero, Quaternion.identity) as DroppedItemView;
        droppedItemView.transform.parent = container;
        droppedItemView.transform.localScale = Vector3.one;
        droppedItemView.updateUI(droppedItem);
        droppedItemView.inventoryScale = inventoryScale;

        GuiWorldObjectFollowController guiWorldObjectFollowCon = droppedItemView.gameObject.AddComponent<GuiWorldObjectFollowController>();
        guiWorldObjectFollowCon.mainCanvasRecTran = mainCanvasRectTran;
        guiWorldObjectFollowCon.init(droppedItem.gameObject);
        droppedItemView.init(guiWorldObjectFollowCon);

        //Add button listener with PlayerControl that when the button clicked,
        //Player will go to the dropped item and will try to pick it up and insert to its bag.
        droppedItemView.GetComponent<Button>().onClick.AddListener(() => GameManager.instance.pc.onDroppedItemClicked(droppedItemView));
        droppedItemView.viewDestroyed += GameManager.instance.pc.onViewDestroyed;
        return droppedItemView;
    }
}
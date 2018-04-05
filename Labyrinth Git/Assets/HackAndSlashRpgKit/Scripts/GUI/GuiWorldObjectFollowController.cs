using System.Collections;
using UnityEngine;

public class GuiWorldObjectFollowController : BaseGameUIView
{
    //this is your object that you want to have the UI element hovering over
    public Transform target;

    private RectTransform myRectTran;
    private Camera main;

    //this is the ui element
    public RectTransform mainCanvasRecTran;

    private Vector2 canvasPos;
    private Vector2 screenPosition = Vector2.zero;

    public override void init(Object anyObject)
    {
        myRectTran = (RectTransform)transform;
        target = ((GameObject)anyObject).transform;
        main = Camera.main;
        initialized = true;
    }

    private void FixedUpdate()
    {
        if (initialized)
        {
            //Get the canvas location from real world location.
            canvasPos = main.WorldToViewportPoint(target.position);
            // as 0,0 is center position we need to find in canvas position.
            screenPosition.x = (canvasPos.x * mainCanvasRecTran.sizeDelta.x) - (mainCanvasRecTran.sizeDelta.x * 0.5f);
            screenPosition.y = (canvasPos.y * mainCanvasRecTran.sizeDelta.y) - (mainCanvasRecTran.sizeDelta.y * 0.5f);

            //This will put the icon name up on the item not hover over it.
            screenPosition.y += 25;
            //now set the position of the ui element
            myRectTran.localPosition = screenPosition;
        }
    }
}
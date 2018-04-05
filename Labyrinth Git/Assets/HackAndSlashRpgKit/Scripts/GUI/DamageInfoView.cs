using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class DamageInfoView : BaseGameUIView
{
    public Camera main;
    public Text damageInfoText;
    private Vector2 targetPos;

    private RectTransform myRectTran;
    public float floatUpSpeed = 3;

    //this is the ui element
    public RectTransform mainCanvasRecTran;

    /// <summary>
    /// This will set up the damage text.
    /// </summary>
    /// <param name="anyObject"></param>
    public override void init(object anyObject)
    {
        damageInfoText.text = "" + (int)anyObject;
    }

    /// <summary>
    /// Please set parameter to the damage taken character's.
    /// </summary>
    /// <param name="anyObject"></param>
    public override void init(UnityEngine.Object anyObject)
    {
        myRectTran = (RectTransform)transform;
        setUiAtTheDamagedCharacterPos(((Transform)anyObject));
        initialized = true;
    }

    public void FixedUpdate()
    {
        if (initialized)
        {
            targetPos = myRectTran.localPosition;
            targetPos.y += 10;
            myRectTran.localPosition = Vector2.Lerp(myRectTran.localPosition, targetPos, Time.deltaTime * floatUpSpeed);
        }
    }

    private void setUiAtTheDamagedCharacterPos(Transform target)
    {
        //Get the canvas location from real world location.
        Vector2 canvasPos = main.WorldToViewportPoint(target.position);
        Vector2 screenPosition = Vector2.zero;
        // as 0,0 is center position we need to find in canvas position.
        screenPosition.x = (canvasPos.x * mainCanvasRecTran.sizeDelta.x) - (mainCanvasRecTran.sizeDelta.x * 0.5f);
        screenPosition.y = (canvasPos.y * mainCanvasRecTran.sizeDelta.y) - (mainCanvasRecTran.sizeDelta.y * 0.5f);

        //This will put the icon name up on the item not hover over it.
        screenPosition.y += 50;
        //now set the position of the ui element
        myRectTran.localPosition = screenPosition;
    }
}
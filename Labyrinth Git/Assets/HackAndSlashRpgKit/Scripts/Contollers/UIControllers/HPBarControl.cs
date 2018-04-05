using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Author : Hwan Kim
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class HPBarControl : MonoBehaviour
{
    private Transform thisTran;
    private RectTransform canvas;
    private bool showHp;
    private bool show;
    private float maxHpXPos;
    private float minHpXPos;

    // Use this for initialization
    private void Awake()
    {
        canvas = GetComponent<RectTransform>();
        thisTran = transform;
        maxHpXPos = thisTran.transform.localPosition.x;
        minHpXPos -= canvas.sizeDelta.x;
    }

    public void setShow(bool on)
    {
        show = on;
    }

    private void FixedUpdate()
    {
        if (show)
        {
            thisTran.transform.localPosition = Vector3.Slerp(thisTran.transform.localPosition, targetPos, Time.deltaTime * 2);
        }
    }

    public Vector3 targetPos = Vector3.zero;

    public void setBar(float current, float max)
    {
        float hpPercentage = (current / max) * 100;
        targetPos.x = minHpXPos - ((minHpXPos * hpPercentage) / 100);
        if (targetPos.x != targetPos.x)
            targetPos.x = 0;
        thisTran.transform.localPosition = targetPos;
    }

    public void changeBar(float current, float max)
    {
        try
        {
            //Max hp(100%)  = x pos : 0                   (maxHpXPos)
            //Min hp(0%)    = x pos : -width from canvas. (minHpXPos)
            float hpPercentage = (current / max) * 100;
            targetPos.x = minHpXPos - ((minHpXPos * hpPercentage) / 100);
            //handling NaN exception.
            targetPos.x += 0.001f;
        }
        catch (Exception ex)
        {
            //ignore
        }
    }
}
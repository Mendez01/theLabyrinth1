using UnityEngine;
using UnityEngine.UI;

public class BaseGameUIView : MonoBehaviour
{
    protected Transform thisTran;
    protected GameObject thisGo;
    public bool initialized = false;

    private void Awake()
    {
        thisTran = transform;
        thisGo = gameObject;
        init();
    }

    public virtual void updateUI()
    {
        //Do override.
    }

    public virtual void updateUI(object any)
    {
        //Do override.
    }

    public virtual void updateUI(Object anyObject)
    {
        //Do override.
    }

    protected virtual void init()
    {
        //Do override.
    }

    public virtual void init(object anyObject)
    {
        //DO override.
    }

    public virtual void init(Object anyObject)
    {
        //DO override.
    }
}
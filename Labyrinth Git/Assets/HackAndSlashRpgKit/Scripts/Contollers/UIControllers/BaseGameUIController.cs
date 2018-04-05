using UnityEngine;

public class BaseGameUIController : MonoBehaviour
{
    private BaseGameUIView _mainUIView;

    public BaseGameUIView mainUIView
    {
        get
        {
            if (_mainUIView == null)
                _mainUIView = GetComponentInChildren<BaseGameUIView>();
            if (_mainUIView == null)
                _mainUIView = GetComponent<BaseGameUIView>();
            return _mainUIView;
        }
    }

    private GameObject _mainUIViewGo;

    protected GameObject mainUIViewGo
    {
        get
        {
            if (_mainUIViewGo == null)
            {
                _mainUIViewGo = mainUIView.gameObject;
            }
            return _mainUIViewGo;
        }
    }

    protected Transform thisTran;
    protected GameObject thisGo;
    protected bool onShow = false, initialized = false;

    /// <summary>
    /// This will initialize the transform and game object from the cache.
    /// </summary>
    private void Awake()
    {
        thisTran = transform;
        thisGo = gameObject;
    }

    /// <summary>
    /// This will use SetActive method of the mainUIView game object
    /// this also can be over-ridable
    /// </summary>
    /// <param name="on"></param>
    public virtual void show(bool on)
    {
        onShow = on;
        mainUIViewGo.SetActive(on);
        if (on)
        {
            mainUIView.updateUI();
        }
    }

    /// <summary>
    /// Please do override this method for initializing the script.
    /// </summary>
    public virtual void init()
    {
    }
}
using UnityEngine;
using System.Collections;

public class InventoryManager : MonoBehaviour
{
    static private InventoryManager _instance;

    static public InventoryManager instance
    {
        get
        {
            if (!initialized)
            {
                _instance = GameObject.FindObjectOfType<InventoryManager>();
            }
            return _instance;
        }
    }

    private static bool initialized = false;
    private PlayerBagViewContoller _playerBagControl;
    public PlayerBagViewContoller playerBagControl
    {
        get
        {
            if (!initialized)
            {
                _playerBagControl = GameObject.FindObjectOfType<PlayerBagViewContoller>();
                initialized = true;
            }
            return _playerBagControl;
        }
    }
}
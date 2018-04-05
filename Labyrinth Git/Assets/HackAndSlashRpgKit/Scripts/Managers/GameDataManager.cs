using System;
using System.Collections;

using UnityEngine;

/// <summary>
/// Author: Hwan Kim
///
/// This class is a singlton. Will contains player and equipment manager to swap the equipment.
///
/// </summary>
public class GameDataManager : MonoBehaviour
{
    private Camera _faceZoom;

    public Camera faceZoom
    {
        get
        {
            try
            {
                if (_faceZoom == null)
                    _faceZoom = GameObject.FindGameObjectWithTag("ZoomCamera").GetComponent<Camera>();
            }
            //in case we are trying to access the deleted camera reference.
            catch
            {
                _faceZoom = GameObject.FindGameObjectWithTag("ZoomCamera").GetComponent<Camera>();
            }
            return _faceZoom;
        }
    }

    public EquipmentManagerV2 _equipmentManager = null;

    public EquipmentManagerV2 equipmentManager
    {
        get
        {
            try
            {
                if (_equipmentManager == null)
                {
                    Debug.Log(name + ": Looking for EquipmentManager in the game scene.");
                    _equipmentManager = GameObject.FindObjectOfType<EquipmentManagerV2>();
                    Debug.Log(name + " : Found EquipmentManager in the game scene.(" + (_equipmentManager != null) + ")");
                }
            }
            //in case we are trying to access the deleted EquipmentManager reference.
            catch (Exception e)
            {
                Debug.LogError(e);
                _equipmentManager = GameObject.FindObjectOfType<EquipmentManagerV2>();
            }
            return _equipmentManager;
        }
    }

    public PhysicMaterial dropItemPhysicMaterial;
    public AudioClip buttonClickAudioClip;
    public static GameDataManager Instance { get; private set; }
    private static bool _instantiated = false;
    public float initializationTime;
    public int gameLoadedCounter = 0;

    private void Start()
    {
        if (_instantiated || Instance != null)
        {
            GameDataManager[] gameDbs = GameObject.FindObjectsOfType<GameDataManager>();
            if (gameDbs.Length > 1)
            {
                //we want to keep only one that is created first when the game is loaded.
                if (gameDbs[0].initializationTime < gameDbs[1].initializationTime)
                    DestroyImmediate(gameDbs[0].gameObject);
                else
                    DestroyImmediate(gameDbs[1].gameObject);

                //We also want to keep only one Equipment manger which is the one get loaded first.
                EquipmentManagerV2[] ems = GameObject.FindObjectsOfType<EquipmentManagerV2>();
                if (ems[0].initializationTime > ems[1].initializationTime)
                {
                    DestroyImmediate(ems[0].gameObject);
                    setEquipmentManagerIntoStands(ems[1]);
                }
                else
                {
                    DestroyImmediate(ems[1].gameObject);
                    setEquipmentManagerIntoStands(ems[0]);
                }
            }
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            init();
        }
    }

    private void setEquipmentManagerIntoStands(EquipmentManagerV2 eq)
    {
        eq.gameObject.transform.parent = GameObject.FindGameObjectWithTag("Stands").gameObject.transform;
    }

    private GameObject _player;

    public GameObject player
    {
        set
        {
            if (_player != null)
            {
                DestroyImmediate(_player);
            }
            _player = value;
            DontDestroyOnLoad(_player.gameObject);
        }
        get
        {
            return _player;
        }
    }

    private void init()
    {
        initializationTime = Time.realtimeSinceStartup;
        _instantiated = true;
    }
}
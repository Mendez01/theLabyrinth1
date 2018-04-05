using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

/// <summary>
/// Author: Hwan Kim.
///
/// This class is used for controlling button click event for the character selector view.
///
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class GUIController : MonoBehaviour
{
    private AudioSource audioSource;
    private GameObject standCharacter;
    public Camera main;
    public GameObject facialMenus, wepaonMenus;
    public ParticleSystem battleSceneEnterEffect;
    private bool weaponsMenuOn = false;
    private bool facialMenuOn = false;

    // Use this for initialization
    private void Awake()
    {
        toggleFacialMenu(false);
        toggleWeaponMenu(false);
    }

    private void toggleFacialMenu(bool on)
    {
        facialMenuOn = on;
        facialMenus.SetActive(on);
        GameDataManager.Instance.faceZoom.enabled = on;
        main.gameObject.SetActive(!on);
    }

    private void toggleWeaponMenu(bool on)
    {
        weaponsMenuOn = on;
        wepaonMenus.SetActive(on);
    }

    public void onRandomFaceButtonClicked()
    {
        GameDataManager.Instance.equipmentManager.randomAppearance();
    }

    public void onRacialButtonClicked()
    {
        GameDataManager.Instance.equipmentManager.changeRace(GameDataManager.Instance.equipmentManager.getNextRaceFeatures());
    }

    public void onEyeButtonClicked()
    {
        GameDataManager.Instance.equipmentManager.characterEye.randomEyeColorChange();
    }

    public void onToBattleButtonClicked()
    {
        toggleFacialMenu(false);
        battleSceneEnterEffect.Play();
        StartCoroutine(loadApplication(2));
    }

    public void onFaceFeatureButtonClicked()
    {
        GameDataManager.Instance.equipmentManager.changeToNextFacialSet(EquipmentV2.EquipmentType.FaceAccesary);
    }

    public void onRightWeaponButtonClicked()
    {
        GameDataManager.Instance.equipmentManager.changerRightHandSet();
    }

    public void onLeftWeaponButtonClicked()
    {
        GameDataManager.Instance.equipmentManager.changerLeftHandSet();
    }

    public void onEyeBrowButtonClicked()
    {
        GameDataManager.Instance.equipmentManager.changeToNextFacialSet(EquipmentV2.EquipmentType.Eyebrows);
    }

    public void onFacialHairButtonClciked()
    {
        GameDataManager.Instance.equipmentManager.changeToNextFacialSet(EquipmentV2.EquipmentType.FacialHair);
    }

    public void onHairButtonChange()
    {
        GameDataManager.Instance.equipmentManager.changeToNextFacialSet(EquipmentV2.EquipmentType.Hair);
    }

    public void onRandomWeaponButtonClicked()
    {
        GameDataManager.Instance.equipmentManager.disableCurrentWeaponSet();
        GameDataManager.Instance.equipmentManager.randomWeapons(false, false);
    }

    public void onToggleWeaponButtonClicked()
    {
        toggleFacialMenu(false);
        toggleWeaponMenu(!weaponsMenuOn);
    }

    public void onFaceAppearanceButtonClicked()
    {
        toggleWeaponMenu(false);
        GameDataManager.Instance.equipmentManager.hideHeadGear(facialMenuOn);
        toggleFacialMenu(!facialMenuOn);
    }

    public void onChangeSetArmorButtonClicked()
    {
        toggleFacialMenu(false);
        toggleWeaponMenu(false);
        GameDataManager.Instance.equipmentManager.changeSet(GameDataManager.Instance.equipmentManager.getNextArmorSet());
    }

    public void onRandomButtonClicked()
    {
        toggleFacialMenu(false);
        toggleWeaponMenu(false);
        GameDataManager.Instance.equipmentManager.randomEquipment(true);
    }

    private IEnumerator loadApplication(float waitSec)
    {
        yield return new WaitForSeconds(waitSec);
        GameDataManager.Instance.equipmentManager.transform.parent = GameDataManager.Instance.transform;
        GameDataManager.Instance.gameLoadedCounter++;
        checkAndSetUpTheBattleSceneV2();
        yield return new WaitForSeconds(waitSec);
        Application.LoadLevel("BattleSceneV2");
    }

    private void checkAndSetUpTheBattleSceneV2()
    {
#if UNITY_EDITOR
        bool isBattleExampleSetUp = false;

        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.path.Contains("BattleSceneV2.unity"))
            {
                isBattleExampleSetUp = true;

                break;
            }
        }
        if (!isBattleExampleSetUp)
        {
            var original = EditorBuildSettings.scenes;
            var newSettings = new EditorBuildSettingsScene[original.Length + 1];
            System.Array.Copy(original, newSettings, original.Length);
            var sceneToAdd = new EditorBuildSettingsScene("Assets/HackAndSlashRpgKit/Sample Scene/BattleSceneV2.unity", true);
            newSettings[newSettings.Length - 1] = sceneToAdd;
            EditorBuildSettings.scenes = newSettings;
            throw new Exception("BattleSceneV2 is not in your build setting," +
      "that is why it didn't load Battle Scene. However," +
      "in code we added the BattleSceneV2, so please simply run again.");
        }
#endif
    }
}
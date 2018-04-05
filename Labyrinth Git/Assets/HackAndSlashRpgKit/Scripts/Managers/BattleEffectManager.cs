using System.Collections;
using UnityEngine;

/// <summary>
/// Author: Hwan Kim
/// </summary>
public class BattleEffectManager : MonoBehaviour
{
    private static bool initialized = false;
    static private BattleEffectManager _instance;

    static public BattleEffectManager instance
    {
        get
        {
            if (!initialized)
            {
                _instance = GameObject.FindObjectOfType<BattleEffectManager>();

                initialized = true;
            }
            return _instance;
        }
    }

    public float damageDisapearInterval;
    public Transform targetCanvasTran;
    public DamageInfoView normalDamageInfo, criticalDamageInfo;
    public GameObject bloodEffect;
    public GameObject targetSelectedEffect;
    public CameraShaker cameraShaker;
    public GameObject itemDropEffect;

    private void Awake()
    {
        initialized = false;
        cameraShaker = GameObject.FindObjectOfType<CameraShaker>();
    }

    private void Start()
    {
    }

    public void createDamageMessageView(int damage, bool critical, Transform damageTakenCharacterTran)
    {
        DamageInfoView newDamageInfoView = Instantiate(critical ? criticalDamageInfo :
            normalDamageInfo, Vector3.zero, Quaternion.identity) as DamageInfoView;
        newDamageInfoView.transform.parent = targetCanvasTran;
        newDamageInfoView.transform.localScale = Vector3.one;
        newDamageInfoView.init(damage);
        newDamageInfoView.init(damageTakenCharacterTran);
        Destroy(newDamageInfoView.gameObject, damageDisapearInterval);
    }

    public GameObject spwanItemDropEffect(Transform item)
    {
        return Instantiate(itemDropEffect, item.position, Quaternion.identity) as GameObject;
    }

    private float lastTimeTargetGenerated;

    private float delay = 0.5f;

    public void spwanTargetSelectedEffect(Vector3 targetLocation, float destoryAfter, bool run)
    {
        if ((Time.time - lastTimeTargetGenerated) > delay)
        {
            lastTimeTargetGenerated = Time.time;
            GameObject spwanTargetSelectedEffect = Instantiate(targetSelectedEffect, targetLocation, Quaternion.identity) as GameObject;
            Destroy(spwanTargetSelectedEffect, destoryAfter);
        }
    }

    private GameObject spwanedTargetEffect;

    public void spwanTargetEffectOnTarget(Transform targetTran)
    {
        if (spwanedTargetEffect == null)
        {
            spwanedTargetEffect = Instantiate(targetSelectedEffect, targetTran.position, Quaternion.identity) as GameObject;
            spwanedTargetEffect.GetComponent<ParticleSystem>().startColor = Color.red;
        }
        spwanedTargetEffect.SetActive(true);
        spwanedTargetEffect.transform.parent = targetTran;
        spwanedTargetEffect.transform.localPosition = Vector3.zero;
    }

    public void removeSpwanTargetEffectOnTarget()
    {
        if (spwanedTargetEffect != null)
        {
            spwanedTargetEffect.SetActive(false);
        }
    }

    public void spwanBloodEffect(Vector3 targetLocation)
    {
        targetLocation.y += 2.5f;
        Destroy(Instantiate(bloodEffect, targetLocation, Random.rotation), 1);
    }

    public GameObject spwanBloodEffect(Vector3 targetLocation, float selfDestroySec)
    {
        GameObject spwanedBlood = Instantiate(bloodEffect, targetLocation, Random.rotation) as GameObject;
        Destroy(spwanedBlood, selfDestroySec);
        return spwanedBlood;
    }
}
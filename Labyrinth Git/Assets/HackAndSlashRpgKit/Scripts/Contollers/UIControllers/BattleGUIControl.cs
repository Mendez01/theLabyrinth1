using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Author: Hwan Kim
/// </summary>
public class BattleGUIControl : MonoBehaviour
{
    private static bool initialized = false;
    static private BattleGUIControl _instance;

    static public BattleGUIControl instance
    {
        get
        {
            if (!initialized)
            {
                _instance = GameObject.FindObjectOfType<BattleGUIControl>();
                initialized = true;
            }
            return _instance;
        }
    }

    private RPGCharacter targetEnemy;
    public GameObject enemyInfo;
    public HPBarControl targetHPBar;
    public Text targetHPText;
    public Text targetName;

    private void Awake()
    {
        initialized = false;
    }

    public void setTarget(RPGCharacter target)
    {
        resetTarget();
        targetEnemy = target;
        targetEnemy.onHPChanged += e_onHPChanged;
        enemyInfo.SetActive(true);
        targetName.text = "Lv: " + targetEnemy.level + " " + targetEnemy.characterName;
        targetHPText.text = targetEnemy.currentHP + " / " + targetEnemy.maxHP;
        targetHPBar.setShow(true);
        targetHPBar.setBar(targetEnemy.currentHP, targetEnemy.maxHP);
    }

    private void e_onHPChanged(int value)
    {
        targetHPBar.setShow(true);
        targetHPText.text = value + " / " + targetEnemy.maxHP;
        targetHPBar.changeBar(value, targetEnemy.maxHP);
        if (value <= 0)
        {
            hideAfter(1.2f);
        }
    }

    public void resetTarget()
    {
        if (targetEnemy != null)
            targetEnemy.onHPChanged -= e_onHPChanged;
        targetEnemy = null;
    }

    public void hideAfter(float waitTime)
    {
        StartCoroutine(WaitAndHide(waitTime));
    }

    private IEnumerator WaitAndHide(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        hideInfo();
    }

    public void hideInfo()
    {
        resetTarget();
        if (targetEnemy == null)
        {
            targetHPBar.setShow(false);
            enemyInfo.SetActive(false);
        }
    }
}
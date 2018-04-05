using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoManager : MonoBehaviour
{
    private static bool initialized = false;
    static private PlayerInfoManager _instance;

    static public PlayerInfoManager instance
    {
        get
        {
            if (!initialized)
            {
                _instance = GameObject.FindObjectOfType<PlayerInfoManager>();
                initialized = true;
            }
            return _instance;
        }
    }

    public Text killCount;
    public Text level;
    public Text nameText;
    public HPBarControl playerHPBar;
    public HPBarControl playerEXPBar;
    public Text playerHPText;
    private RPGCharacter player;

    private void Awake()
    {
        initialized = false;
    }

    public void setPlayer(RPGCharacter player)
    {
        this.player = player;
        player.onHPChanged += e_onHPChanged;
        player.onLevelUp += player_onLevelUp;
        player.onEXPChanged += player_onEXPChanged;
        nameText.text = player.characterName;
        level.text = "" + player.level;
        playerHPText.text = player.currentHP + " / " + player.maxHP;
        playerEXPBar.setShow(true);
        playerEXPBar.setBar(player.currentExp, player.maxExp);
        playerHPBar.setShow(true);
        playerHPBar.setBar(player.currentHP, player.maxHP);
    }

    private void player_onEXPChanged(int value)
    {
        playerEXPBar.changeBar(value, player.maxExp);
    }

    private void player_onLevelUp(int value)
    {
        level.text = "" + value;
    }

    private void e_onHPChanged(int value)
    {
        playerHPText.text = value + " / " + player.maxHP;
        playerHPBar.changeBar(value, player.maxHP);
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusView : BaseGameUIView
{
    public Text playerName, levelDetails, exprequired, str, dex, wis, speed, offense, defense;
    public HPBarControl expBarControl;
    private RPGCharacter player;

    public override void updateUI()
    {
        if (!initialized)
        {
            player = GameManager.instance.pc.characterInfo;
            player.onLevelUp += player_onLevelUp;
            player.onEXPChanged += player_onEXPChanged;
            initialized = true;
        }
        playerName.text = player.characterName;
        levelDetails.text = "Level " + player.level;
        exprequired.text = "Required EXP for next lvl :" + (player.maxExp - player.currentExp);
        str.text = "" + player.strength;
        dex.text = "" + player.dex;
        wis.text = "" + player.wisdom;
        speed.text = "Attack : " + string.Format("{0:N2}", player.attackSpeed) + ", Movement : " + string.Format("{0:N2}", player.movementSpeed);
        offense.text = "Offense " + player.attackPower;
        defense.text = "Defense " + player.defensePower;
        expBarControl.setBar(player.currentExp, player.maxExp);
    }

    private void player_onLevelUp(int value)
    {
        updateUI();
    }

    private void player_onEXPChanged(int value)
    {
        exprequired.text = "Required EXP for next lvl :" + (player.maxExp - player.currentExp);
        expBarControl.changeBar(player.currentExp, player.maxExp);
    }
}
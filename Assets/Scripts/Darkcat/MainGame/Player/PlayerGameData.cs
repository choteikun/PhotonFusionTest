using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家資料
/// </summary>

[System.Serializable]
public class PlayerGameData 
{
    public string playerName { get; private set; }
    public int playerID { get; private set; }
    public float hitPercentage { get; set; }
    public bool gearCharge  { get; set; }
    public int playerScoreGet { get; set; }
    public ItemEnum held_ItemEnum{ get; set; }
    public PlayerStatusEnum m_PlayerStatus;

    public PlayerGameData(string name,int playerid)
    {
        playerName = name;
        playerID = playerid;
        hitPercentage = 0;
        gearCharge = false;
        held_ItemEnum = ItemEnum.NoItem;
        m_PlayerStatus = PlayerStatusEnum.Playing;
    }
    public void Player_GetItem(ItemEnum itemGet)
    {
        if (held_ItemEnum == ItemEnum.NoItem)
        {
            held_ItemEnum = itemGet;
        }
    }
    public void Player_ChangeStatus(PlayerStatusEnum status)
    {
        m_PlayerStatus = status;
    }

    public void Player_Charge()
    {
        gearCharge = true;
    }
    public void Player_Be_Damage(float damage)
    {
        hitPercentage += damage;
    }
    public void PlayerScore()
    {
        playerScoreGet++;
    }
    public void PlayerRevive()
    {
        hitPercentage = 0;
        gearCharge = false;
        held_ItemEnum = ItemEnum.NoItem;
    }

}
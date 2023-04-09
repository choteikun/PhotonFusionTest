using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ª±®a¸ê®Æ
/// </summary>

[System.Serializable]
public class PlayerGameData 
{
    public string playerName { get; private set; }
    public int playerID { get; private set; }
    public float hitPercentage { get; set; }
    public bool gearCharge  { get; set; }
    public ItemEnum held_ItemEnum{ get; set; }
    public PlayerStatusEnum m_PlayerStatus { get; set; }

    public PlayerGameData(string name,int playerid)
    {
        playerName = name;
        playerID = playerid;
        hitPercentage = 0;
        gearCharge = false;
        held_ItemEnum = ItemEnum.NoItem;
        m_PlayerStatus = PlayerStatusEnum.Waiting;
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
}

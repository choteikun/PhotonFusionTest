using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/// <summary>
/// 玩家資料
/// </summary>

[System.Serializable]
public class PlayerGameData 
{
    /// <summary>
    /// 玩家姓名
    /// </summary>
    public string PlayerName { get; private set; }
    /// <summary>
    /// 玩家ID
    /// </summary>
    public int PlayerID { get; private set; }
    /// <summary>
    /// 玩家BK值
    /// </summary>
    [Networked]
    public float BreakPoint { get; set; } 
    /// <summary>
    /// 玩家是否有被充能
    /// </summary>
    public bool SuperSmashChargeOrNot { get; set; }
    /// <summary>
    /// 玩家拿到的分數
    /// </summary>
    public int PlayerScore { get; set; }
    /// <summary>
    /// 玩家身上的道具
    /// </summary>
    public ItemEnum Held_ItemEnum{ get; set; }
    /// <summary>
    /// 玩家狀態
    /// </summary>
    public PlayerStatusEnum m_PlayerStatus;

    public PlayerGameData(string name,int playerid)
    {
        PlayerName = name;
        PlayerID = playerid;
        BreakPoint = 0;
        SuperSmashChargeOrNot = false;
        Held_ItemEnum = ItemEnum.NoItem;
        m_PlayerStatus = PlayerStatusEnum.Playing;
    }
    public void Player_GetItem(ItemEnum itemGet)
    {
        if (Held_ItemEnum == ItemEnum.NoItem)
        {
            Held_ItemEnum = itemGet;
        }
    }
    public void Player_ChangeStatus(PlayerStatusEnum status)
    {
        m_PlayerStatus = status;
    }

    public void Player_Charge()
    {
        SuperSmashChargeOrNot = true;
    }
    public void Player_Be_Damage(float damage)
    {
        BreakPoint += damage;
    }
    public void PlayerAddScore()
    {
        PlayerScore++;
    }
    public void PlayerRevive()
    {
        BreakPoint = 0;
        SuperSmashChargeOrNot = false;
        Held_ItemEnum = ItemEnum.NoItem;
    }

}

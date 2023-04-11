using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家遊戲狀態
/// </summary>
/// 
[System.Serializable]
public enum PlayerStatusEnum 
{
    Waiting,
    Playing,
    BeAttack,
    Dead,
    Win,
}

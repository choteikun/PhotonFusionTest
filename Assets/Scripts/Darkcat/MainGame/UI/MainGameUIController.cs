using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameUIController : ToSingletonMonoBehavior<MainGameUIController>
{
    public int ThisGameMaxPlayer;

    [SerializeField] GameObject mainPlayerUI_;
    [SerializeField] GameObject[] UI_For_5_OrLessPlayer;
    [SerializeField] GameObject[] UI_ForManyPLayer;

    public void InitPlayerBKUI(int ThisPlayerID,List<Color>playersColor,List<string>playersName)
    {
        var totalPlayerCount = playersColor.Count;
        ThisGameMaxPlayer = totalPlayerCount;
        mainPlayerUI_.GetComponent<PlayerInformationBlockUpdater>().initThisBlock(ThisPlayerID, playersColor[ThisPlayerID],playersName[ThisPlayerID]);
        var colorPool = new List<Color>();
        var playerNamePool = new List<string>();
        var IDPool = new List<int>();
        for (int i = 0; i < playersColor.Count; i++)
        {
            if (i!=ThisPlayerID)
            {
                colorPool.Add(playersColor[i]);
                playerNamePool.Add(playersName[i]);
                IDPool.Add(i);
            }
        }
        if (totalPlayerCount-1<=5)
        {
            for (int i = 0; i < totalPlayerCount-1; i++)
            {
                UI_For_5_OrLessPlayer[i].SetActive(true);
                var playerUIBlock = UI_For_5_OrLessPlayer[i].GetComponent<PlayerInformationBlockUpdater>();
                playerUIBlock.initThisBlock(IDPool[i], colorPool[i], playerNamePool[i]);
            }
        }
        else
        {
            for (int i = 0; i < totalPlayerCount - 1; i++)
            {
                UI_ForManyPLayer[i].SetActive(true);
                var playerUIBlock = UI_ForManyPLayer[i].GetComponent<PlayerInformationBlockUpdater>();
                playerUIBlock.initThisBlock(IDPool[i], colorPool[i], playerNamePool[i]);
            }
        }
        
    }

    public void UpdatePlayerBKUI(int ThisPlayerID, List<int> playersBK)
    {
        var totalPlayerCount = ThisGameMaxPlayer;
        mainPlayerUI_.GetComponent<PlayerInformationBlockUpdater>().UpdateThisBlock(playersBK[ThisPlayerID]);//HasinputAuthority的BK設置
        var playerBKPool = new List<int>();
        for (int i = 0; i < playersBK.Count; i++)
        {
            if (i != ThisPlayerID)
            {
                playerBKPool.Add(playersBK[i]);//這個ID玩家以外的BK值加進這個pool裡面
            }
        }
        if (totalPlayerCount - 1 <= 5)
        {
            for (int i = 0; i < totalPlayerCount - 1; i++)
            {
                var playerUIBlock = UI_For_5_OrLessPlayer[i].GetComponent<PlayerInformationBlockUpdater>();
                playerUIBlock.UpdateThisBlock(playerBKPool[i]);
            }
        }
        else
        {
            for (int i = 0; i < totalPlayerCount - 1; i++)
            {
                var playerUIBlock = UI_ForManyPLayer[i].GetComponent<PlayerInformationBlockUpdater>();
                playerUIBlock.UpdateThisBlock(playerBKPool[i]);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameUIController : ToSingletonMonoBehavior<MainGameUIController>
{
    [SerializeField] GameObject mainPlayerUI_;
    [SerializeField] GameObject[] UI_For_5_OrLessPlayer;
    [SerializeField] GameObject[] UI_ForManyPLayer;

    public void InitPlayerBKUI(int MainPlayerID,List<Color>playersColor,List<string>playersName)
    {
        var totalPlayerCount = playersColor.Count; 
        mainPlayerUI_.GetComponent<PlayerInformationBlockUpdater>().initThisBlock(MainPlayerID, playersColor[MainPlayerID],playersName[MainPlayerID]);
        var colorPool = new List<Color>();
        var playerNamePool = new List<string>();
        var IDPool = new List<int>();
        for (int i = 0; i < playersColor.Count; i++)
        {
            if (i!=MainPlayerID)
            {
                colorPool.Add(playersColor[i]);
                playerNamePool.Add(playerNamePool[i]);
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
    public void UpdatePlayerBKUI(int MainPlayerID, List<int>playersBK)
    {
        var totalPlayerCount = playersBK.Count;
        mainPlayerUI_.GetComponent<PlayerInformationBlockUpdater>().UpdateThisBlock(playersBK[MainPlayerID]);
        var playerBKPool = new List<int>();
        for (int i = 0; i < playersBK.Count; i++)
        {
            if (i != MainPlayerID)
            {
                playersBK.Add(playersBK[i]);
            }
        }
        if (totalPlayerCount - 1 <= 5)
        {
            for (int i = 0; i < totalPlayerCount - 1; i++)
            {               
                var playerUIBlock = UI_For_5_OrLessPlayer[i].GetComponent<PlayerInformationBlockUpdater>();
                playerUIBlock.UpdateThisBlock(playersBK[i]);
            }
        }
        else
        {
            for (int i = 0; i < totalPlayerCount - 1; i++)
            {
                var playerUIBlock = UI_ForManyPLayer[i].GetComponent<PlayerInformationBlockUpdater>();
                playerUIBlock.UpdateThisBlock(playersBK[i]);
            }
        }
    }
}

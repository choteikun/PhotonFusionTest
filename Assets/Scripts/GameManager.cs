using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fusion;
using UnityEngine.Events;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private NetworkRunner runner = null;
    public NetworkRunner Runner
    {
        get
        {
            if (runner == null)
            {
                runner = gameObject.AddComponent<NetworkRunner>();

                //該 "NetworkRunner" 組件可能會收集來自其他組件或網絡的玩家輸入數據，以便在遊戲中使用這些數據
                runner.ProvideInput = true;
            }

            return runner;
        }
    }
    public string PlayerName = null;

    public Dictionary<PlayerRef, PlayerNetworkData> PlayerList = new Dictionary<PlayerRef, PlayerNetworkData>();

    public event Action OnPlayerListUpdated = null;

    public UnityEvent PlayerFinishCastingTeleporting = new UnityEvent();

    private void Awake()
    {
        //為了確保 Runner 對象存在並且被創建，以便其他方法可以使用 Runner 對象
        Runner.ProvideInput = true;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    private bool CheckAllPlayerIsReady()
    {
        //如果沒有連接到服務器回傳false
        if (!Runner.IsServer) return false;

        foreach (var playerData in PlayerList.Values)
        {
            //如果不是所有的玩家都按下ReadyButton回傳false
            if (!playerData.IsReady) return false;
        }

        //所有的玩家都按下ReadyButton(所有玩家的IsReady==true的情況下)，將IsReady改回false，並回傳true
        foreach (var playerData in PlayerList.Values)
        {
            playerData.IsReady = false;
        }

        return true;
    }

    public void UpdatePlayerList()
    {
        //加入房間時
        //如果不是空的，就更新gameManager.PlayerList(從InRoomPanel的UpdatePlayerList())
        OnPlayerListUpdated?.Invoke();

        if (CheckAllPlayerIsReady())
        {
            //將GamePlay場景設置為當前活動場景，以便其他組件或遊戲對象可以正確地訪問該場景中的內容
            Runner.SetActiveScene("GamePlay");
        }
    }

    public void SetPlayerNetworkData()
    {
        if (PlayerList.TryGetValue(runner.LocalPlayer, out PlayerNetworkData playerNetworkData))
        {
            playerNetworkData.SetPlayerName_RPC(PlayerName);
        }
    }
}


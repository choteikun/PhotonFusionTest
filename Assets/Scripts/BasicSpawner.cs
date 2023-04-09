using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;



public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    private GameManager gameManager = null;

    [SerializeField]
    private NetworkRunner networkRunner = null;

    [SerializeField]
    private NetworkPrefabRef playerPrefab;

    private Dictionary<PlayerRef, NetworkObject> playerList = new Dictionary<PlayerRef, NetworkObject>();//用PlayerRef當Key存放剛剛生成的可以操控的角色，是為了可以記錄所有玩家的名單


    void Start()
    {
        gameManager = GameManager.Instance;

        networkRunner = gameManager.Runner;

        networkRunner.AddCallbacks(this);

        SpawnAllPlayers();
    }

    private void SpawnAllPlayers()
    {
        //拿gameManager裡的PlayerList存的Key(PlayerRef)
        foreach (var player in gameManager.PlayerList.Keys)
        {
            Vector3 spawnPos = Vector3.up * 2;
            NetworkObject networkPlayerObject = networkRunner.Spawn(playerPrefab, spawnPos, Quaternion.identity, player);

            //設置關聯的網絡對象
            networkRunner.SetPlayerObject(player, networkPlayerObject);

            playerList.Add(player, networkPlayerObject);
        }
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)//傳入的runner會是場景中的Network Runner，PlayerRef則是代表實際進入的玩家
    {
        Vector3 spawnPos = Vector3.up * 2;
        NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, spawnPos, Quaternion.identity, player);//讓這個進入的玩家擁有這個生成的Prefab

        runner.SetPlayerObject(player, networkPlayerObject);

        playerList.Add(player, networkPlayerObject);//用list把玩家存起來                                                                         
        Debug.Log(player.PlayerId + "Join");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (playerList.TryGetValue(player, out NetworkObject networkObject))//檢查玩家是否還在list裡面
        {
            runner.Despawn(networkObject);
            playerList.Remove(player);
            Debug.Log(player.PlayerId + "Left");
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)//host 不會觸發
    {
        Debug.Log(runner.SessionInfo+"The INFO");
        Debug.LogWarning("MyPlayerIDis" + runner.GetPlayerUserId());
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();
        if (Input.GetKey(KeyCode.W))
        {
            data.Move += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            data.Move += Vector3.left;
        }
        if (Input.GetKey(KeyCode.S))
        {
            data.Move += Vector3.back;
        }
        if (Input.GetKey(KeyCode.D))
        {
            data.Move += Vector3.right;
        }

        data.Buttons.Set(InputButtons.JUMP, Input.GetKey(KeyCode.Space));
        data.Buttons.Set(InputButtons.Sprint, Input.GetKey(KeyCode.LeftShift));
        data.Buttons.Set(InputButtons.Attack, Input.GetKey(KeyCode.Mouse0));
        input.Set(data);//提供輸入
    }
   
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }   
}

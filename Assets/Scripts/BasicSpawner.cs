using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;
using Cinemachine;


public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    

    [SerializeField]
    private NetworkRunner networkRunner = null;

    [SerializeField]
    private NetworkPrefabRef playerPrefab;

    private Dictionary<PlayerRef, NetworkObject> playerList = new Dictionary<PlayerRef, NetworkObject>();//用PlayerRef當Key存放剛剛生成的可以操控的角色，是為了可以記錄所有玩家的名單


    void Start()
    {
        StartGame(GameMode.AutoHostOrClient);//第一位進入的玩家偵測有沒host，如果沒有的話自己成為host(GameMode是指要以什麼樣的身分進入遊戲)
    }

    async void StartGame(GameMode mode)
    {
        networkRunner.ProvideInput = true;//提供input

        await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "Fusion Room",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>() //管控跟scene有關的操作
        });
    }

    public void OnConnectedToServer(NetworkRunner runner)//host 不會觸發
    {
        Debug.Log(runner.SessionInfo+"The INFO");
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
        data.Buttons.Set(InputButtons.FIRE, Input.GetKey(KeyCode.Mouse0));
        input.Set(data);//提供輸入
    }
   
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)//傳入的runner會是場景中的Network Runner，PlayerRef則是代表實際進入的玩家
    {
        Vector3 spawnPos = Vector3.up * 2;
        NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, spawnPos, Quaternion.identity, player);//讓這個進入的玩家擁有這個生成的Prefab

        playerList.Add(player, networkPlayerObject);//用list把玩家存起來
        Debug.Log(player.PlayerId + "Join");
        Bind_Camera(networkPlayerObject.gameObject);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)//只有host會觸發
    {
        if (playerList.TryGetValue(player,out NetworkObject networkObject))//檢查玩家是否還在list裡面
        {
            runner.Despawn(networkObject);
            playerList.Remove(player);
            Debug.Log(player.PlayerId + "Left");
        }
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
    public void Bind_Camera(GameObject Player)
    {
        var CinemachineVirtualCamera = Camera.main.gameObject.transform.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
        CinemachineVirtualCamera.LookAt = Player.transform;
        CinemachineVirtualCamera.Follow = Player.transform;
    }
}

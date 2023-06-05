using System.Collections;
using UnityEngine;
using Fusion;

public class PlayerNetworkData : NetworkBehaviour
{
	private GameManager gameManager = null;

	[Networked] public Color PlayerColor { get; set; }
	
	[HideInInspector]
	[Networked] public int HostID { get; set; }
	[Networked] public int PlayerID { get; set; }
	[Networked(OnChanged = nameof(OnPlayerNameChanged))] public string PlayerName { get; set; }
	[Networked(OnChanged = nameof(OnIsReadyChanged))] public NetworkBool IsReady { get; set; }
	[Networked(OnChanged = nameof(OnGameOverChanged))] public NetworkBool OutOfTheBoat { get; set; }

    [Networked(OnChanged = nameof(OnPlayerBkChange))] public int PlayerBkPercent { get; set; }

    public void Start()
    {
		Debug.Log("PlayerNetworkData Start");
    }
    public override void Spawned()
	{
		Debug.Log("PlayerNetworkData Spawned");
		gameManager = GameManager.Instance;

		transform.SetParent(GameManager.Instance.transform);

		gameManager.PlayerList.Add(Object.InputAuthority, this);
        gameManager.UpdatePlayerList();

		if (Object.HasInputAuthority)
		{
			SetPlayerID_RPC(gameManager.HostID);//藉由確認HostID再依進入遊戲的玩家的順序逐一給Player ID
			SetPlayerName_RPC(gameManager.PlayerName);
			SetPlayerColor_RPC(gameManager.PlayerColor);
		}
	}

	#region - RPCs -

	[Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
	public void SetPlayerID_RPC(int hostID)
	{
        HostID = hostID;
        if (Object.InputAuthority.PlayerId == gameManager.HostID)//如果網路內置的PlayerId等於房間輸入人數id-1
		{
			PlayerID = 0;
		}
        else
        {
			PlayerID = Object.InputAuthority.PlayerId + 1;
		}
		
	}

	[Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
	public void SetPlayerName_RPC(string name)
	{
		PlayerName = name;
	}

	[Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
	public void SetReady_RPC(bool isReady)
	{
		IsReady = isReady;
	}

	[Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
	public void SetPlayerColor_RPC(Color color)
	{
		PlayerColor = color;
	}

	[Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
	public void SetPlayerOut_RPC(bool outOfTheBoat)
	{
		OutOfTheBoat = outOfTheBoat;
	}
	[Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
	public void SetPlayerBkPercent_RPC(int bkPercent)
	{
		PlayerBkPercent = bkPercent;
	}
	#endregion

	#region - OnChanged Events -
	private static void OnPlayerNameChanged(Changed<PlayerNetworkData> changed)
	{
		//PlayerName一變動就更新PlayerList
        GameManager.Instance.UpdatePlayerList();
    }

	private static void OnIsReadyChanged(Changed<PlayerNetworkData> changed)
	{
		//IsReady一變動就更新PlayerList
		GameManager.Instance.UpdatePlayerList();
    }

	private static void OnGameOverChanged(Changed<PlayerNetworkData> changed)
	{
        //有玩家出局就檢查玩家獲勝條件
        GameManager.Instance.UpdateWinnerWhoIs();
    }
	
	private static void OnPlayerBkChange(Changed<PlayerNetworkData> changed)
	{
		//PlayerNetworkData的BK值一變動就更新列表中所屬ID的BK值
		GameManager.Instance.UpdateAllPlayerBKData();
	}
	#endregion
}

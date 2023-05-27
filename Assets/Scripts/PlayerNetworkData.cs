using System.Collections;
using UnityEngine;
using Fusion;

public class PlayerNetworkData : NetworkBehaviour
{
	private GameManager gameManager = null;

	[Networked(OnChanged = nameof(OnPlayerNameChanged))] public string PlayerName { get; set; }
	[Networked(OnChanged = nameof(OnIsReadyChanged))] public NetworkBool IsReady { get; set; }
	[Networked(OnChanged = nameof(OnGameOverChanged))] public NetworkBool OutOfTheBoat { get; set; }

	//[Networked(OnChanged = nameof(OnPlayerBkChange))] public float ThisPlayerBkPoint { get; set; }
	[field:SerializeField]public string PlayerID { get; set; }
	public override void Spawned()
	{
		gameManager = GameManager.Instance;

		transform.SetParent(GameManager.Instance.transform);

		gameManager.PlayerList.Add(Object.InputAuthority, this);
        gameManager.UpdatePlayerList();

		if (Object.HasInputAuthority)
		{
			SetPlayerName_RPC(gameManager.PlayerName);
		}
	}

	#region - RPCs -

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
	public void SetPlayerOut_RPC(bool outOfTheBoat)
	{
		OutOfTheBoat = outOfTheBoat;
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
	
	private static void OnPlayerGameDataChange(Changed<PlayerNetGameData>changed)
    {
		
    }
	private static void OnPlayerBkChange(Changed<PlayerNetworkData> changed)
	{

	}
	#endregion
}

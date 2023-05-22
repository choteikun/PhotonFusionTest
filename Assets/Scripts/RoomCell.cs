using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomCell : MonoBehaviour
{
    public Button joinBtn = null;

    private string roomName = null;

    private LobbyManager lobbyManager = null;

    [SerializeField] private TMP_Text roomNameTxt = null;
    

    public void SetInfo(LobbyManager lobbyManager, string roomName)
    {
        this.lobbyManager = lobbyManager;
        roomNameTxt.text = roomName;
    }

    public async void OnJoinBtnClicked()
    {
        await lobbyManager.JoinRoom(roomName);
    }
}

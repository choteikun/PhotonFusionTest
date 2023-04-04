using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateRoomPanel : MonoBehaviour, IPanel
{
    [SerializeField] private LobbyManager lobbyManager = null;

    [SerializeField] private CanvasGroup canvasGroup = null;

    [SerializeField] private TMP_InputField roomNameInputField = null;
    [SerializeField] private TMP_InputField maxPlayerInputField = null;

    public void DisplayPanel(bool value)
    {
        canvasGroup.alpha = value ? 1 : 0;
        canvasGroup.interactable = value;
        canvasGroup.blocksRaycasts = value;
    }

    //返回大廳按鈕
    public void OnBackBtnClicked()
    {
        lobbyManager.SetPairState(PairState.Lobby);
    }

    //創建房間按鈕
    public async void OnCreateBtnClicked()
    {
        string roomName = roomNameInputField.text;
        int maxPlayer = int.Parse(maxPlayerInputField.text);

        await lobbyManager.CreateRoom(roomName, maxPlayer);
    }
}

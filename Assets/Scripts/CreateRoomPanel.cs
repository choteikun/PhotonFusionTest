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
        if(string.IsNullOrEmpty(roomNameInputField.text) || string.IsNullOrEmpty(maxPlayerInputField.text))
        {
            Debug.Log("請輸入你的房名以及房間人數");
            return;
        }

        string roomName = roomNameInputField.text;
        int maxPlayer = int.Parse(maxPlayerInputField.text);

        if (maxPlayer > 9)
        {
            Debug.Log("房間人數最多不超過9人");
            return;
        }
        Debug.Log(roomName);

        await lobbyManager.CreateRoom(roomName, maxPlayer);
    }
}

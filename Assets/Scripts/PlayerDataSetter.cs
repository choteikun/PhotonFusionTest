using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataSetter : MonoBehaviour
{
    private GameManager gameManager = null;
    private void Start()
    {
        gameManager = GameManager.Instance;
    }
    public void OnPlayerNameInputFieldChange(string value)
    {
        gameManager.PlayerName = value;

        gameManager.SetPlayerNetworkData();
    }
}

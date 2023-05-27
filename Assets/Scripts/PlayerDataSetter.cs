using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataSetter : MonoBehaviour
{
    private GameManager gameManager = null;

    [SerializeField] private MeshRenderer playerMeshRenderer = null;
    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void OnPlayerRSliderChanged(float value)
    {
        gameManager.PlayerColor.r = value / 255f;
        playerMeshRenderer.material.color = gameManager.PlayerColor;

        gameManager.SetPlayerNetworkData();
    }

    public void OnPlayerGSliderChanged(float value)
    {
        gameManager.PlayerColor.g = value / 255f;
        playerMeshRenderer.material.color = gameManager.PlayerColor;

        gameManager.SetPlayerNetworkData();
    }

    public void OnPlayerBSliderChanged(float value)
    {
        gameManager.PlayerColor.b = value / 255f;
        playerMeshRenderer.material.color = gameManager.PlayerColor;

        gameManager.SetPlayerNetworkData();
    }

    public void OnPlayerNameInputFieldChange(string value)
    {
        gameManager.PlayerName = value;

        gameManager.SetPlayerNetworkData();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SceneStarter : NetworkBehaviour
{
    private void Start()
    {
        var gameManager = GameManager.Instance;
        Debug.Log(gameManager.ThisLocalPlayerId);
        Debug.Log(gameManager.AllPlayersColor.Count);
        Debug.Log(gameManager.AllPlayersName.Count);
    }
}
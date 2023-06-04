using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneStarter : MonoBehaviour
{
    
    private void Start()
    {
        var gameManager = GameManager.Instance;
        MainGameUIController.Instance.InitPlayerBKUI(gameManager.ThisLocalPlayerId,gameManager.AllPlayersColor,gameManager.AllPlayersName);
    }
}

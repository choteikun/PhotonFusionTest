using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ExitGame : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void TitleScene()
    {
        SceneManager.MoveGameObjectToScene(GameManager.Instance.gameObject, SceneManager.GetActiveScene());
        SceneManager.MoveGameObjectToScene(MainGameUIController.Instance.gameObject, SceneManager.GetActiveScene());
        //SceneManager.LoadScene("Lobby");
        SceneManager.LoadScene("TitleScene");
    }
}

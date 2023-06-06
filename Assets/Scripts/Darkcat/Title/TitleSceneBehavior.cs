using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class TitleSceneBehavior : MonoBehaviour
{
    private bool CanEnterGame = false;
    [SerializeField] private Image WhiteBG;
    void Start()
    {
        Invoke("Timer", 12f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown&&CanEnterGame)
        {
            CanEnterGame = false;
            WhiteBG.DOFade(1, 0.5f).OnComplete(()=> { SceneManager.LoadScene(1);});
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            SceneManager.LoadScene(0);
        }
    }
    private void Timer()
    {
        CanEnterGame = true;
    }
    
}

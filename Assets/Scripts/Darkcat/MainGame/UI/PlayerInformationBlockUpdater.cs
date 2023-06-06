using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerInformationBlockUpdater : MonoBehaviour
{
    public int ThisPlayerID;
    [SerializeField] private TextMeshProUGUI thisPlayerID_;
    [SerializeField] private Image thisPlayerIcon_;
    [SerializeField] private Image thisPlayerBKBar_;
    [SerializeField] private TextMeshProUGUI thisPlayerBKPoint_;

    public void initThisBlock(int playerID,Color color,string playerName)
    {
        ThisPlayerID = playerID;
        thisPlayerID_.text = playerName;
        thisPlayerIcon_.color = color;
        thisPlayerBKPoint_.text = "0%";
    }
    public void UpdateThisBlock(int Bk)
    {
        thisPlayerBKPoint_.text = Bk.ToString() + "%";
        thisPlayerBKBar_.fillAmount = Bk * 0.01f;
    }
}

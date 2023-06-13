using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinnerNameText : MonoBehaviour
{
    [SerializeField]
    private TMP_Text tmp_text;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var playerNetworkData in GameManager.Instance.PlayerList.Values)
        {
            if (!playerNetworkData.OutOfTheBoat)
            {
                tmp_text.text = playerNetworkData.PlayerName.ToString();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

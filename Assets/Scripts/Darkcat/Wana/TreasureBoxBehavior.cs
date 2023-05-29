using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class TreasureBoxBehavior : NetworkBehaviour
{
    


    public void PlayerGetTreasure(PlayerGameData player)
    {
        player.Player_Charge();
        //this object play opened animation and destroy animation
        //this object play partical
        this.gameObject.SetActive(false);
        this.gameObject.transform.position = new Vector3(1000, 1000, 1000);
        //change player shader into golden
    }
    public void TriggerTreasureBox(NetworkObject player)
    {
        SoundEffectManager.Instance.PlayOneSE(SoundEffectManager.Instance.soundEffectData.TreasureBreakSoundEffect);
        if (player.CompareTag("Player"))
        {
            
            this.gameObject.SetActive(false);
            this.gameObject.transform.position = new Vector3(1000, 1000, 1000);
        }
        
    }
}

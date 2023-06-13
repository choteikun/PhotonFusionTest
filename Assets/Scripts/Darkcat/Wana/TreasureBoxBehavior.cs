using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class TreasureBoxBehavior : NetworkBehaviour
{
    [Networked]
    public bool ImUsefull { get; set; }
    public bool TreasureBoxAppear;
    [SerializeField]
    private MeshRenderer meshRenderer;

    public override void Spawned()
    {
        TreasureBoxAppear = false;
        ImUsefull = false;
    }
    public override void FixedUpdateNetwork()
    {
        if (!TreasureBoxAppear && ImUsefull)
        {
            SoundEffectManager.Instance.PlayOneSE(SoundEffectManager.Instance.soundEffectData.TreasureBoxAppearSoundEffect);
            StartCoroutine(TreasureBoxDissolveAmountTransition(0, 1));
            TreasureBoxAppear = true;
        }
    }
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
        
        if (player.CompareTag("Player") && ImUsefull)
        {
            SoundEffectManager.Instance.PlayOneSE(SoundEffectManager.Instance.soundEffectData.TreasureBreakSoundEffect);
            player.GetComponent<PlayerController>().SuperMode = true;

            //this.gameObject.SetActive(false);
            StartCoroutine(TreasureBoxDissolveAmountTransition(1, 3));
            Invoke("OutMap", 3f);
            TreasureBoxAppear = false;
            ImUsefull = false;
        }
        
    }
    public void OutMap()
    {
        this.gameObject.transform.position = new Vector3(1000, 1000, 1000);
    }
    IEnumerator TreasureBoxDissolveAmountTransition(float targetValue, float duration)//蠑螈傳送特效
    {
        float elapsedTime = 0f;
        float startValue = meshRenderer.material.GetFloat("_DissolveAmount"); //當前數值

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            meshRenderer.material.SetFloat("_DissolveAmount", Mathf.Lerp(startValue, targetValue, t));

            yield return null;
        }
        meshRenderer.material.SetFloat("_DissolveAmount", targetValue); //最終設定為目標數值 

    }
}

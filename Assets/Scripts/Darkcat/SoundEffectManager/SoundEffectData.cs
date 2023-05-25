using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SoundEffectData
{
    #region MainMenuSoundEffect
    public AudioSource ButtonClick { get; set; }
    public AudioSource IntoRoom { get; set; }
    public AudioSource StartGame { get; set; }
    #endregion
    #region OtherSoundEffect
    public AudioSource WinSoundEffect { get; set; }
    public AudioSource LoseSoundEffect { get; set; }
    #endregion
    #region PlayerSoundEffect
    public AudioClip WalkingSoundEffect { get; set; }
    public AudioClip AutoAttackSwing { get; set; }
    public AudioClip BonkSound { get; set; }
    public AudioClip IsCharging { get; set; }
    public AudioClip SmashSwing { get; set; }
    public AudioClip JumpSoundEffect { get; set; }
    public AudioClip IntoWaterSoundEffect { get; set; }
    #endregion
    #region WanaSoundEffect
    public AudioClip BoxBreakSoundEffect { get; set; }
    public AudioClip TunnelReady { get; set; }
    public AudioClip TunnelOut { get; set; }
    public AudioClip TreasureBoxAppearSoundEffect { get; set; }
    public AudioClip TreasureBreakSoundEffect { get; set; }
    public AudioClip BecomeTrailblazerSoundEffect { get; set; }
    #endregion

    public async void SoundEffectDataInit()
    {
        PropertyInfo[] propertyInfo = typeof(SoundEffectData).GetProperties();
        for (int i = 0; i < propertyInfo.Length; i++)
        {
            var targetAsset = await AddressableSearcher.GetAddressableAssetAsync<AudioClip>("Prefab/" + propertyInfo[i].Name);
            if (targetAsset != null)
            {
                propertyInfo[i].SetValue(this, targetAsset);
            }
        }
    }
}

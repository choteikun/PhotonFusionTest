using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManager : ToSingletonMonoBehavior<SoundEffectManager>
{
    public SoundEffectData soundEffectData = new SoundEffectData();
    [SerializeField] AudioSource SingleUseAudioSource;
    protected override void init()
    {
        soundEffectData.SoundEffectDataInit();
        Debug.Log("");
    }
    public void PlayOneSE(AudioClip clip)
    {
        SingleUseAudioSource.PlayOneShot(clip);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManager : ToSingletonMonoBehavior<SoundEffectManager>
{
    public SoundEffectData soundEffectData = new SoundEffectData();
    protected override void init()
    {
        soundEffectData.SoundEffectDataInit();
        Debug.Log("");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

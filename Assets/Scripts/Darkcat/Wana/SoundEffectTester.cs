using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SoundEffectTester : NetworkBehaviour
{
    [SerializeField] public AudioSource AudioSourceGlobal;

    [SerializeField] public AudioSource AudioSourcePrefab;

    [SerializeField] public  AudioClip TestClip;



    [Rpc(RpcSources.All, RpcTargets.All)]
    public void PlayAudioGlobalTest_RPC()
    {
        AudioSourceGlobal.clip = TestClip;
        AudioSourceGlobal.Play();
    }
}

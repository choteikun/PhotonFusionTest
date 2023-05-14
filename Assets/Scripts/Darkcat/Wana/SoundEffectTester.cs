using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SoundEffectTester : NetworkBehaviour
{

    [SerializeField] private AudioSource AudioSourceGlobal;

    [SerializeField] private AudioSource AudioSourcePrefab;
    
    [SerializeField] private AudioClip TestClip;



    public void PlayAudioTest()
    {
        AudioSourcePrefab.PlayOneShot(TestClip);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void PlayAudioGlobalTest_RPC()
    {
        AudioSourceGlobal.clip = TestClip;
        AudioSourceGlobal.Play();
    }
}

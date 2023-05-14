using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectTester : MonoBehaviour
{
    [SerializeField] private AudioSource AudioSourcePrefab;
    [SerializeField] private AudioSource AudioSourceGlobal;
    [SerializeField] private AudioClip TestClip;

    public void PlayAudioTest()
    {
        AudioSourcePrefab.PlayOneShot(TestClip);
    }
    public void PlayAudioGlobalTest()
    {
        AudioSourceGlobal.PlayOneShot(TestClip);
    }
}

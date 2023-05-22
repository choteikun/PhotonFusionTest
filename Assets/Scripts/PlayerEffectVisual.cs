using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerEffectVisual : MonoBehaviour
{
    [Header("Effect Transform")]
    [SerializeField] private Transform vfx_Effects;
    [SerializeField] private Transform pfx_Effects;
    public Transform VFX_Effects => vfx_Effects;
    public Transform PFX_Effects => pfx_Effects;


    [Header("Visual Effect Infos")]
    [SerializeField]
    private VisualEffectInfo[] visualEffectInfos;
    public VisualEffectInfo[] VisualEffectInfos => visualEffectInfos;

    [Header("Partice Effect Infos")]
    [SerializeField]
    private ParticleEffectInfo[] particleEffectInfos;
    public ParticleEffectInfo[] ParticleEffectInfos => particleEffectInfos;

    //Internal lists(存放你實例化的特效)
    [SerializeField]
    private List<VisualEffect> visualEffectsList = new List<VisualEffect>();
    [SerializeField]
    private List<ParticleSystem> particleEffectsList = new List<ParticleSystem>();
    public void InitializeVisualEffect()
    {
        //把scriptableObjs存放的特效一一實例出來
        for (int i = 0; i < visualEffectInfos.Length; i++)
        {
            if (visualEffectInfos[i] != null)
            {
                VisualEffect vfxInstan = Instantiate(visualEffectInfos[i].VisualEffect, vfx_Effects.GetChild(i).position, vfx_Effects.GetChild(i).rotation, vfx_Effects.GetChild(i));
                visualEffectsList.Add(vfxInstan);
            }
            else
            {
                visualEffectInfos[i] = null;
            }
        }
    }
    public void InitializeParticleEffect()
    {
        //把scriptableObjs存放的特效一一實例出來
        for (int i = 0; i < particleEffectInfos.Length; i++)
        {
            if (particleEffectInfos[i] != null)
            {
                ParticleSystem pfxInstan = Instantiate(particleEffectInfos[i].ParticleEffect, pfx_Effects.GetChild(i).position, pfx_Effects.GetChild(i).rotation, pfx_Effects.GetChild(i));
                particleEffectsList.Add(pfxInstan);
            }
            else
            {
                particleEffectsList[i] = null;
            }
        }
    }
    public void DrivingDustEffectPlay()
    {
        visualEffectsList[0].Play();
    }
    public void DrivingDustEffectStop()
    {
        visualEffectsList[0].Stop();
    }
    public void JumpingDustEffectPlay()
    {
        visualEffectsList[1].Play();
    }
    public void JumpingDustEffectStop()
    {
        visualEffectsList[1].Stop();
    }
    public void HitEffectPlay()
    {
        visualEffectsList[2].Play();
    }
    public void HitEffectStop()
    {
        visualEffectsList[2].Stop();
    }
}

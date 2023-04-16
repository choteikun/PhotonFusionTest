using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ParticleEffectInfo", menuName = "Assets/Scripts/ScriptableObjects/Particle Effect Info")]
public class ParticleEffectInfo : ScriptableObject
{
    [SerializeField] private ParticleSystem particleEffect;
    public ParticleSystem ParticleEffect => particleEffect;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "VisualEffectInfo", menuName = "Assets/Scripts/ScriptableObjects/Visual Effect Info")]
public class VisualEffectInfo : ScriptableObject
{
    [SerializeField] private VisualEffect visualEffect;
    public VisualEffect VisualEffect => visualEffect;
}

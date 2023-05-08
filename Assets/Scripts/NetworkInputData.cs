using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


public enum InputButtons
{
    JUMP,
    Sprint,
    Attack,
}

public struct NetworkInputData : INetworkInput
{
    public NetworkButtons Buttons;
    public Vector3 Move;
    public Vector3 Look;

    //public bool AnalogMovement;
    //public bool Jump;
    //public bool Fire;
    //public bool Sprint; 
}

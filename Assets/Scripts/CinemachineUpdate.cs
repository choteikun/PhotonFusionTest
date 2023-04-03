using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Fusion;

public class CinemachineUpdate : NetworkBehaviour
{
    Camera mainCam => Camera.main;
    CinemachineBrain brain;

    public override void Spawned()
    {
        brain = mainCam.GetComponent<CinemachineBrain>();
    }
    public override void FixedUpdateNetwork()
    {
        brain.ManualUpdate();
    }
}

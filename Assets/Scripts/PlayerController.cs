using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private NetworkCharacterControllerPrototype networkCharacterController = null;
    [SerializeField]
    private float moveSpeed = 15f;
    public override void FixedUpdateNetwork()//逐每個tick更新(一個tick相當1.666毫秒)
    {
        if(GetInput(out NetworkInputData data))
        {
            Vector3 moveVector = data.movementInput.normalized;
            networkCharacterController.Move(moveSpeed * moveVector * Runner.DeltaTime);//Runner不同於Delta是以每Tick去計算的而不是不是每秒
        }
    }
}

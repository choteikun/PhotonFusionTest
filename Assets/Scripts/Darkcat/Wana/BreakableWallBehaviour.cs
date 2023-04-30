using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BreakableWallBehaviour : NetworkBehaviour
{
    [field:SerializeField] [Networked] public int HealthPoint { get; set; }
    private NetworkObject thisObject_;
    public override void Spawned()
    {
        if (Object.HasStateAuthority)//�u�|�b���A���ݤW�B��
        {
            thisObject_ = this.GetComponent<NetworkObject>();
        }
    }
    public override void FixedUpdateNetwork()
    {
        if (HealthPoint <= 0)
        {
            DestroyThisBox();
        }
    }
    public void HurtThisWall()
    {
        HealthPoint--;
    }

    public void DestroyThisBox()
    {
        if (Object.HasStateAuthority)//�u�|�b���A���ݤW�B��
        {
            Runner.Despawn(thisObject_);
        }
    }
}

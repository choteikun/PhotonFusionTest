using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SelfRotation : NetworkBehaviour
{
    [SerializeField] private float rotateSpeed_;
    public override void FixedUpdateNetwork()
    {
        this.transform.Rotate(new Vector3(0, 1f, 0) * Runner.DeltaTime * rotateSpeed_);
    }
}

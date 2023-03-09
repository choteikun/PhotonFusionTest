using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Ball : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }//Tick計時器

    [SerializeField]
    private float bulletSpeed = 5f;

    public override void Spawned()
    {
        life = TickTimer.CreateFromSeconds(Runner, 5.0f);//(可以在任何有NetworkBehavior的地方下調用Runner)倒數5秒
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
        else
        {
            transform.position += bulletSpeed * transform.forward * Runner.DeltaTime;
        }        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();

            player.TakeDamage(10);

            Runner.Despawn(Object);
        }
    }
}
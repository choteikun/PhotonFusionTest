using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TeleportManager : NetworkBehaviour
{
    [SerializeField] private Teleporter[] teleporters_ = new Teleporter[4];
    
    
    private void playerStartTeleportation(NetworkObject playerInTeleporter_)
    {

    }
    
}

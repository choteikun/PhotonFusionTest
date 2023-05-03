using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Teleporter : NetworkBehaviour
{
    [SerializeField] private Vector3[] teleportFinalPositions_ = new Vector3[4];
    [SerializeField] private Vector3 tempStayPosition_;
    [SerializeField] private int thisTeleporterID_;
    [SerializeField] private bool canTeleport_ = true;
    [SerializeField] private float teleportCountDown_;
    [SerializeField] private NetworkObject playerInTeleporter_;

    public override void FixedUpdateNetwork()
    {
        if (canTeleport_ == false)
        {
            teleportCountDown_ += Runner.DeltaTime;
            if (teleportCountDown_>=3f)
            {
                canTeleport_ = true;
                teleportCountDown_ = 0f;
                //�ǰe���a
                teleportToFinalPosition();
                //�ק缾�a���A �ϥL��ާ@
            }
        }

    }


    public void TriggerTeleporter(NetworkObject player)
    {
        if (player.CompareTag("Player") && canTeleport_)
        {           
            canTeleport_ = false;
            playerInTeleporter_ = player;
            player.transform.position = tempStayPosition_;//�����a�Ǩ�q���޸�
            //�ק缾�a���A �ϥL����ާ@
        }
    }
    private void teleportToFinalPosition()
    {
        var finalPosition = randomAPosition();
        playerInTeleporter_.transform.position = finalPosition;
    }

    private Vector3 randomAPosition()
    {
        var randomNum = Random.Range(0, 4);
        while (randomNum == thisTeleporterID_)
        {
            randomNum = Random.Range(0, 4); 
        }
        return teleportFinalPositions_[randomNum];
    }
}

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
    [SerializeField] private bool startTeleporting = false;
    [SerializeField] private float teleportCountDown_;
    [SerializeField] private NetworkObject playerInTeleporter_;

    /// <summary>
    /// 呼叫這台傳送器，告知牠開始倒數傳送
    /// </summary>
    public void StartTeleportingCountDown()
    {
        Debug.Log("傳送囉!!");
        startTeleporting = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (startTeleporting == true)
        {
            teleportCountDown_ += Runner.DeltaTime;
            if (teleportCountDown_ >= 3f) 
            {
                canTeleport_ = true;
                startTeleporting = false;
                teleportCountDown_ = 0f;
                //傳送玩家
                teleportToFinalPosition();
                //修改玩家狀態 使他能操作
                playerInTeleporter_.GetComponent<PlayerController>().PlayerIsTeleporting = false;
            }
        }

    }


    public void TriggerTeleporter(NetworkObject player)
    {
        if (player.CompareTag("Player") && canTeleport_)
        {           
            canTeleport_ = false;
            playerInTeleporter_ = player;
            //修改玩家狀態 使他不能操作一秒
            //開始消失
            player.GetComponent<PlayerController>().PlayerIsTeleporting = true;
            //把自己的資料灌給player
            player.GetComponent<PlayerController>().PlayerGameData.InWhichTeleporter = this;
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

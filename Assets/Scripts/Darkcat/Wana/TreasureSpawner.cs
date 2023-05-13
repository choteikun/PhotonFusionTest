using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TreasureSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject treasureBox_;
    [SerializeField] private Vector3[] treasureBoxPlace_;

    [SerializeField] public bool spawnerSwitch = false;
    [SerializeField] private float countDownTime_;

    public override void FixedUpdateNetwork()
    {
        if (spawnerSwitch)
        {
            countDownTime_ += Runner.DeltaTime;
            if (countDownTime_>=3)
            {
                spawnerSwitch = false;
                countDownTime_ = 0;
                SpawnARandomTreasureBox();
            }
        }
    }
    public void SpawnARandomTreasureBox()
    {
        var treasureBoxRandomPlace = treasureBoxPlace_[getARandomPlace()];
        treasureBox_.gameObject.SetActive(true);
        treasureBox_.gameObject.transform.position = treasureBoxRandomPlace;
        //playTreasureBox Spawn animation and partical;        
    }

    private int getARandomPlace()
    {
        var num = Random.Range(0, treasureBoxPlace_.Length);
        return num;
    }
    private void spawnATreasureBoxPrefab(Vector3 position)
    {
        Runner.Spawn(treasureBox_, position, Quaternion.identity, Object.StateAuthority);      
    }
}

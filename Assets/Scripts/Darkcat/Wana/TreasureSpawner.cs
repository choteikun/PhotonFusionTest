using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UniRx;

public class TreasureSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject treasureBox_;
    [SerializeField] private Vector3[] treasureBoxPlace_;

    public bool SpawnerSwitch;

    public float CountDownTime_;//只要有Networked 命名都要大寫開頭規範 
    public float NextSpawnTime_;

    void Start()
    {
        SpawnerSwitch = true;//啟動寶箱生成器
    }
    public override void FixedUpdateNetwork()
    {
        if (SpawnerSwitch)
        {
            CountDownTime_ += Runner.DeltaTime;
            if (CountDownTime_ >= 5) 
            {
                SpawnerSwitch = false;
                CountDownTime_ = 0;
                
                SpawnARandomTreasureBox();
            }
        }
        else
        {
            if (!treasureBox_.GetComponent<TreasureBoxBehavior>().ImUsefull)
            {
                NextSpawnTime_ += Runner.DeltaTime;
                if (NextSpawnTime_ >= 60)
                {
                    SpawnerSwitch = true;
                    NextSpawnTime_ = 0;
                }
            }
            
        }

    }
    public void SpawnARandomTreasureBox()
    {
        var treasureBoxRandomPlace = treasureBoxPlace_[getARandomPlace()];
        //treasureBox_.gameObject.SetActive(true);
        treasureBox_.GetComponent<TreasureBoxBehavior>().ImUsefull = true;
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

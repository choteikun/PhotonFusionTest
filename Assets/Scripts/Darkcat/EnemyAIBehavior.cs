using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Fusion;

public class EnemyAIBehavior : NetworkBehaviour
{
    [SerializeField] public NetworkObject EnemyObject;
    [SerializeField] private float rotateSpeed_;
    [field: SerializeField] [Networked] private Quaternion rotation_ { get; set; }
    private UnityEvent enemyRotateEvent_ = new UnityEvent();
    private UnityEvent enemyStraightFowardEvent_ = new UnityEvent();
    private UnityEvent enemyDectectWallEvent_ = new UnityEvent();
    [SerializeField] private float StraightSpeed = 20f;
    //void Start()
    //{
    //    enemyStartRotateBehavior();
    //}

    public override void Spawned()
    {
        enemyStartRotateBehavior();
    }
    // Update is called once per frame
    //void Update()
    //{
    //    EnemyObject.transform.rotation = Quaternion.Slerp(EnemyObject.transform.rotation, rotation_, rotateSpeed_);
    //    enemyStraightFowardMovement();
    //    enemyStraightFowardEvent_.Invoke();
    //}
    public override void FixedUpdateNetwork()
    {
        EnemyObject.transform.rotation = Quaternion.Slerp(EnemyObject.transform.rotation, rotation_, rotateSpeed_);
        enemyStraightFowardMovement();
        enemyStraightFowardEvent_.Invoke();

    }

    private void generateMovement()
    {
        var randomAngle = Random.Range(0.0f, 360.0f);
        rotation_ = Quaternion.Euler(0, randomAngle, 0);
        Debug.Log("Turn");
    }
    private void enemyStartRotateBehavior()
    {
        InvokeRepeating("generateMovement", 1.5f, 5f);
        Debug.Log("IntoBehavior");
    }
    private void enemyStraightFowardMovement()
    {
        EnemyObject.transform.position += EnemyObject.transform.forward * StraightSpeed * Runner.DeltaTime;
    }    
}

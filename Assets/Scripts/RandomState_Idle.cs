using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomState_Idle : StateMachineBehaviour
{

    private float randomIdleTime = 0;

    public int numberOfStates = 3;

    [Tooltip("5秒以上看起來比較正常")]
    [Range(5, 10)]
    public float minNormTime;
    [Range(11, 15)]
    public float maxNormTime;

    private float randomIdleTimer;

    //protected float m_RandomNormTime;

    readonly int m_HashRandomIdle = Animator.StringToHash("RandomIdle");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //m_RandomNormTime來隨機決定過渡的時間
        randomIdleTimer = Random.Range(minNormTime, maxNormTime);//一個範圍內的隨機亂數計時器
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("randomIdleTime:" + randomIdleTime/60 + "     randomIdleTimer:" + randomIdleTimer);
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !animator.IsInTransition(0))//如果當前狀態是idle且不處於過渡條下
        {

            randomIdleTime++;
            if (randomIdleTime >= randomIdleTimer * 60)
            {
                animator.SetInteger(m_HashRandomIdle, Random.Range(0, numberOfStates));//設置隨機idle1,2,3,4等...
            }
            else
            {
                animator.SetInteger(m_HashRandomIdle, -1);//參數設為-1
            }
        }
        else
        {
            randomIdleTime = 0;
        }

        ////如果Idle狀態機超出隨機決定的歸一化時間並且尚未轉換，則設置隨機Idle
        //if (stateInfo.normalizedTime > m_RandomNormTime && !animator.IsInTransition(0))
        //{
        //    animator.SetInteger(m_HashRandomIdle, Random.Range(0, numberOfStates));
        //}
    }
}

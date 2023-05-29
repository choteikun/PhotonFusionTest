using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomState_Idle : StateMachineBehaviour
{

    private float randomMotionTime = 0;

    public int numberOfStates = 3;

    [Tooltip("5秒以上看起來比較正常")]
    [Range(5, 10)]
    public float minNormTime;
    [Range(11, 15)]
    public float maxNormTime;

    private float randomMotionTimer;

    readonly int m_HashRandomMotion = Animator.StringToHash("RandomMotion");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //m_RandomNormTime來隨機決定過渡的時間
        randomMotionTimer = Random.Range(minNormTime, maxNormTime);//一個範圍內的隨機亂數計時器
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("randomIdleTime:" + randomIdleTime/60 + "     randomIdleTimer:" + randomIdleTimer);
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !animator.IsInTransition(0))//如果當前狀態是idle且不處於過渡條下
        {

            randomMotionTime++;
            if (randomMotionTime >= randomMotionTimer * 60)
            {
                animator.SetInteger(m_HashRandomMotion, Random.Range(0, numberOfStates));//設置隨機idle1,2,3,4等...
            }
            else
            {
                animator.SetInteger(m_HashRandomMotion, -1);//參數設為-1
            }
        }
        else
        {
            randomMotionTime = 0;
        }
    }
}

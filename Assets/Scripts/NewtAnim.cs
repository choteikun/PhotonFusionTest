using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum NewtAnimState
{
    Idle,
    Move,
    BeAttack,
    Dead,
    Win,
}
public class NewtAnim : MonoBehaviour
{
    

    //當前狀態
    public NewtAnimState newtAnimState;

    //提前Hash進行優化
    readonly int h_AnimMoveSpeed = Animator.StringToHash("AnimMoveSpeed");
    readonly int h_AirborneSpeed = Animator.StringToHash("AirborneSpeed");

    readonly int h_Idle = Animator.StringToHash("Idle");
    readonly int h_Run = Animator.StringToHash("Run");
    readonly int h_Sprint = Animator.StringToHash("Sprint");
    readonly int h_Flap = Animator.StringToHash("Flap");
    readonly int h_Charging = Animator.StringToHash("Charging");
    readonly int h_ChargeFlap = Animator.StringToHash("ChargeFlap");
    readonly int h_BeAttack = Animator.StringToHash("BeAttack");
    readonly int h_Die = Animator.StringToHash("Die");

    readonly int h_TimeOutToIdle = Animator.StringToHash("TimeOutToIdle");

    readonly int h_InputDetected = Animator.StringToHash("InputDetected");
    readonly int h_Grounded = Animator.StringToHash("Grounded");


    [SerializeField, Tooltip("過渡到隨機Idle動畫所需要花的時間")]
    private float idleTimeOut;
    //Idle動畫計時器(跳轉至隨機動畫)
    private float _idleTimer;
    //Move狀態中回到idle的計時器(避免方向鍵切換時角色回到idle狀態)
    private float _moveToIdleTimer;

    //檢測按鈕
    private bool inputDetected;
    //角色水平速度
    private float player_horizontalVel;
    //角色垂直速度
    private float player_verticalVel;

    private NewtTest newtTest;
    private Animator anim;

    void Start()
    {
        newtTest = GetComponentInParent<NewtTest>();
        anim = GetComponent<Animator>();
        newtAnimState = NewtAnimState.Idle;
    }

    
    void Update()
    {
        player_horizontalVel = new Vector3(newtTest.Move.x, 0, newtTest.Move.z).magnitude * newtTest.PlayerSpeed;
        anim.SetFloat(h_AnimMoveSpeed, player_horizontalVel);

        if (newtTest.NewtIsGrounded())
        {
            player_verticalVel = -8;
        }
        else
        {
            player_verticalVel = newtTest.controller.velocity.y;
        }
        anim.SetFloat(h_AirborneSpeed, player_verticalVel);

        anim.SetBool(h_Grounded, newtTest.NewtIsGrounded());

        if (newtTest.Move == Vector3.zero && newtTest.NewtIsGrounded() && !newtTest.NewtFlapAnimPlay)//待機狀態
        {
            inputDetected = false;
            _moveToIdleTimer++;
            if (_moveToIdleTimer >= 60.0f)
            {
                newtAnimState = NewtAnimState.Idle;
            }
        }
        else
        {
            newtAnimState = NewtAnimState.Move;
            inputDetected = true;
        }
        if (newtTest.NewtFlapAnimPlay && (newtAnimState == NewtAnimState.Idle || newtAnimState == NewtAnimState.Move))//只有在Idle & Move 狀態下可以同時播放拍巴掌動畫
        {
            anim.SetBool(h_Flap, true);
            if ((anim.GetCurrentAnimatorStateInfo(1).shortNameHash == h_Flap && anim.GetCurrentAnimatorStateInfo(1).normalizedTime >= 1.0f))//如果在拍巴掌動畫結束時
            {
                anim.SetBool(h_Flap, false);
                newtTest.NewtFlapAnimPlay = false;
            }
        }
        if (newtTest.NewtChargeFlapAnimPlay && (newtAnimState == NewtAnimState.Idle || newtAnimState == NewtAnimState.Move))//只有在Idle & Move 狀態下可以同時播放蓄力動畫
        {
            if ((anim.GetCurrentAnimatorStateInfo(1).shortNameHash != h_Charging) && (anim.GetCurrentAnimatorStateInfo(1).shortNameHash != h_ChargeFlap)) //如果不在蓄力攻擊狀態機以及任何的過渡條下則開始進入蓄力動畫
            {
                anim.SetBool(h_Flap, false);
                anim.SetBool(h_Charging, true);
                
            }
            else if ((anim.GetCurrentAnimatorStateInfo(1).shortNameHash == h_Charging) && (!newtTest.NewtChargeAttackOrNot))//在蓄力狀態機下同時放開滑鼠左鍵釋放蓄力攻擊並
            {
                anim.SetBool(h_Charging, false);
                anim.SetBool(h_ChargeFlap, true);
                Debug.Log("h_ChargeFlap");

            }
            else if ((anim.GetCurrentAnimatorStateInfo(1).shortNameHash == h_ChargeFlap) && (anim.GetCurrentAnimatorStateInfo(1).normalizedTime >= 1.0f))
            {
                anim.SetBool(h_ChargeFlap, false);
                newtTest.NewtChargeFlapAnimPlay = false;
            }
        }
        switch (newtAnimState)
        {
            case NewtAnimState.Idle:
                anim.SetBool(h_Idle, true);
                anim.SetBool(h_Run, false);
                _moveToIdleTimer = 0;
                break;
            case NewtAnimState.Move:
                anim.SetBool(h_Run, true);
                anim.SetBool(h_Idle, false);
                break;
            case NewtAnimState.BeAttack:

                break;
            case NewtAnimState.Dead:

                break;
            case NewtAnimState.Win:

                break;
            default:
                break;
        }

        TimeoutToIdle();
    }
    void TimeoutToIdle()
    {
        if (newtAnimState == NewtAnimState.Idle)
        {
            _idleTimer += Time.deltaTime;

            if (_idleTimer >= idleTimeOut)
            {
                _idleTimer = 0f;
                anim.SetTrigger(h_TimeOutToIdle);
            }
        }
        else
        {
            _idleTimer = 0f;
            anim.ResetTrigger(h_TimeOutToIdle);
        }
        anim.SetBool(h_InputDetected, inputDetected);
    }
    public void OnFlapEnter()
    {
        
    }
    public void OnFlapExit()
    {
        
    }
}

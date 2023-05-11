using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public enum PlayerAnimState
{
    Idle,
    Move,
    BeAttack,
    Dead,
    Win,
}

[OrderAfter(typeof(PlayerController))]
public class PlayerNetworkMecanimAnimator : NetworkBehaviour
{
    [Networked]
    public NetworkButtons ButtonsPrevious { get; set; }//上一個按鈕的資料

    //當前狀態
    public PlayerAnimState playerAnimState;

    private PlayerController playerController;
    private NetworkMecanimAnimator networkAnimator;

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

    public override void FixedUpdateNetwork()
	{
		if (IsProxy == true)
			return;

		if (Runner.IsForward == false)
            return;
        //前向模式（Forward Prediction）是指客戶端在處理用戶輸入時，預測將來幾幀內的遊戲狀態，並在本地進行模擬，然後將輸入和狀態同步給服務器。
        //服務器會驗證客戶端的輸入，並進行狀態校正。這種方式可以減少網絡延遲對遊戲體驗的影響，但是需要客戶端做更多的處理，並且容易受到網絡抖動的影響。
        //回放模式（Replay）是指服務器將之前的遊戲狀態記錄下來，並將記錄發送給客戶端，客戶端根據記錄進行本地模擬，並且每隔一定時間向服務器發送用戶輸入。
        //這種方式可以保證客戶端和服務器之間的遊戲狀態一致性，但是會增加網絡延遲和帶寬使用，並且需要記錄和回放遊戲狀態，增加了服務器的負擔。

        player_horizontalVel = new Vector3(playerController.Network_CharacterControllerPrototype.Velocity.x, 0, playerController.Network_CharacterControllerPrototype.Velocity.z).magnitude;
        networkAnimator.Animator.SetFloat(h_AnimMoveSpeed, player_horizontalVel);

        if (playerController.Network_CharacterControllerPrototype.IsGrounded)
        {
            player_verticalVel = -8;
        }
        else
        {
            player_verticalVel = playerController.Network_CharacterControllerPrototype.Velocity.y;
        }
        networkAnimator.Animator.SetFloat(h_AirborneSpeed, player_verticalVel);

        networkAnimator.Animator.SetBool(h_Grounded, playerController.Network_CharacterControllerPrototype.IsGrounded);

        if (playerController.GetInput(out NetworkInputData data))
        {
            NetworkButtons buttons = data.Buttons;
            var pressed = buttons.GetPressed(ButtonsPrevious);//跟上一個按鈕去做比較
            var released = buttons.GetReleased(ButtonsPrevious);
            ButtonsPrevious = buttons;

            //在State Authority上，建議使用NetworkMecanimAnimator的SetTrigger()方法，以確保觸發器的正確同步。
            //而對於Input Authority，可以使用Animator的原生SetTrigger()方法，因為此時狀態同步不是關鍵問題。


            if (data.Move == Vector3.zero && playerController.Network_CharacterControllerPrototype.IsGrounded && !playerController.FlapAnimPlay)//待機狀態
            {
                inputDetected = false;
                _moveToIdleTimer++;
                if (_moveToIdleTimer >= 60.0f)
                {
                    playerAnimState = PlayerAnimState.Idle;
                }
            }
            else
            {
                playerAnimState = PlayerAnimState.Move;
                inputDetected = true;
            }
            if (playerController.FlapAnimPlay && (playerAnimState == PlayerAnimState.Idle || playerAnimState == PlayerAnimState.Move))//只有在Idle & Move 狀態下可以同時播放拍巴掌動畫
            {
                networkAnimator.Animator.SetBool(h_Flap, true);
                if ((networkAnimator.Animator.GetCurrentAnimatorStateInfo(1).shortNameHash == h_Flap && networkAnimator.Animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 1.0f))//如果在拍巴掌動畫結束時
                {
                    networkAnimator.Animator.SetBool(h_Flap, false);
                    playerController.FlapAnimPlay = false;
                }
            }
            if (playerController.ChargeFlapAnimPlay && (playerAnimState == PlayerAnimState.Idle || playerAnimState == PlayerAnimState.Move))//只有在Idle & Move 狀態下可以同時播放蓄力動畫
            {
                if ((networkAnimator.Animator.GetCurrentAnimatorStateInfo(1).shortNameHash != h_Charging) && (networkAnimator.Animator.GetCurrentAnimatorStateInfo(1).shortNameHash != h_ChargeFlap)) //如果不在蓄力攻擊狀態機以及任何的過渡條下則開始進入蓄力動畫
                {
                    playerController.collisionAvailable = false;//蓄力時關閉碰撞
                    networkAnimator.Animator.SetBool(h_Flap, false);
                    networkAnimator.Animator.SetBool(h_Charging, true);
                }
                else if ((networkAnimator.Animator.GetCurrentAnimatorStateInfo(1).shortNameHash == h_Charging) && (!playerController.ChargeAttackOrNot))//在蓄力狀態機下同時放開滑鼠左鍵釋放蓄力攻擊並
                {
                    networkAnimator.Animator.SetBool(h_Charging, false);
                    networkAnimator.Animator.SetBool(h_ChargeFlap, true);

                }
                else if ((networkAnimator.Animator.GetCurrentAnimatorStateInfo(1).shortNameHash == h_ChargeFlap) && (networkAnimator.Animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 1.0f))
                {
                    networkAnimator.Animator.SetBool(h_ChargeFlap, false);
                    playerController.ChargeFlapAnimPlay = false;
                }
            }

            
        }
        switch (playerAnimState)
        {
            case PlayerAnimState.Idle:
                networkAnimator.Animator.SetBool(h_Idle, true);
                networkAnimator.Animator.SetBool(h_Run, false);
                _moveToIdleTimer = 0;
                break;
            case PlayerAnimState.Move:   
                networkAnimator.Animator.SetBool(h_Run, true);
                networkAnimator.Animator.SetBool(h_Idle, false);
                break;
            case PlayerAnimState.BeAttack:

                break;
            case PlayerAnimState.Dead:

                break;
            case PlayerAnimState.Win:

                break;
            default:
                break;
        }

        TimeoutToIdle();
    }

    void TimeoutToIdle()
    {
        if (playerAnimState == PlayerAnimState.Idle)
        {
            _idleTimer += Time.deltaTime;

            if (_idleTimer >= idleTimeOut)
            {
                _idleTimer = 0f;
                networkAnimator.SetTrigger(h_TimeOutToIdle);
            }
        }
        else
        {
            _idleTimer = 0f;
            networkAnimator.Animator.ResetTrigger(h_TimeOutToIdle);
        }
        networkAnimator.Animator.SetBool(h_InputDetected, inputDetected);
    }
    public void OnFlapEnter()
    {
        playerController.collisionAvailable = true;
    }
    public void OnFlapExit()
    {
        playerController.collisionAvailable = false;
    }

    protected void Awake()
	{
		playerController = GetComponentInParent<PlayerController>();
        networkAnimator = GetComponent<NetworkMecanimAnimator>();
        playerAnimState = PlayerAnimState.Idle;
    }
}

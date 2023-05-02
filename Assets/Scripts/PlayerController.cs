using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using Cinemachine;

[RequireComponent(typeof(CharacterController))]
//[RequireComponent(typeof(PlayerInput))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private EnemyAIBehavior enemyPrefab;

    [SerializeField]
    private PlayerGameData playerGameData;

    public NetworkCharacterControllerPrototype Network_CharacterControllerPrototype = null;

    //[SerializeField]
    //private Ball ballPrefab;

    [SerializeField]
    private GameObject handCollider;//手掌Collider(用於偵測其他人的PlayerController腳本)

    [SerializeField]
    private Image CurBKBar = null;

    [SerializeField]
    private MeshRenderer meshRenderer = null;

    [SerializeField, Tooltip("衝刺速度")]
    private float drivingSpeed;

    [Tooltip("角色巴掌力度")]
    [Networked]
    public float PushForce { get; private set; }

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    

    [SerializeField, Tooltip("Mouse Cursor Settings")]
    private bool cursorLocked = true;
    public bool cursorInputForLook = true;

    //[SerializeField]
    //private int maxHp = 100;
    //[Networked(OnChanged =nameof(OnHpChanged))]//血量數值每次一有變化就刷新
    //public int CurHp { get; set; }

    [Networked]
    public NetworkButtons ButtonsPrevious { get; set; }//上一個按鈕的資料



    [Networked][Tooltip("角色BK係數(曲線X軸)")]
    public float CoefficientOfBreakDownPoint { get; private set; }

    [Tooltip("角色普攻BK值")][SerializeField]
    private int normalAttackBK;
    [Tooltip("角色蓄力BK的最大值")][SerializeField][Range(0,20)]
    private int chargeAttackMaxBK;
    [Tooltip("角色蓄力BK值")]
    [SerializeField]
    private int curChargeAttackBK;
    [Tooltip("角色蓄力條")]
    private int chargeAttackBar = 5;
    [SerializeField]
    private bool chargeAttackOrNot;

    // cinemachine
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;

    // player
    [Networked]
    public bool DrivingKeyStatus { get; private set; }//衝刺鍵狀態
    [Networked]
    public bool JumpEffectTrigger { get; private set; }//防止不斷播放跳躍特效，false為不可播放
    [Networked]
    public bool BeenHitOrNot { get; set; }//是否被擊中過


    [HideInInspector]
    public bool FlapAnimPlay { get; private set; }//播放攻擊動畫的狀態


    private float chargeAttackBarTimer;
    private float speed;

    private PlayerEffectVisual playerEffectVisual;
    private GameObject mainCamera;

    


    public override void Spawned()
    {
        FlapAnimPlay = false;
        BeenHitOrNot = false;
        DrivingKeyStatus = false;
        playerEffectVisual = GetComponent<PlayerEffectVisual>();
        playerEffectVisual.InitializeVisualEffect();//因為是所有客戶端都要看到的特效，所以放在外面
        playerEffectVisual.InitializeParticleEffect();

        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
        //cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

        speed = Network_CharacterControllerPrototype.MoveSpeed;
        if (Object.HasStateAuthority)//只會在伺服器端上運行
        {
            
            //CurHp = maxHp;//初始化血量
            chargeAttackOrNot = false;
            CoefficientOfBreakDownPoint = 0.0f;//初始化角色BK值
            chargeAttackBarTimer = 0.0f;
        }
        if (Object.HasInputAuthority)//在客戶端上運行
        {
            if (GameManager.Instance.PlayerList.TryGetValue(Object.InputAuthority, out var playerNetworkData))
            {
                //playerGameData = new PlayerGameData(playerNetworkData.PlayerName, Object.InputAuthority.PlayerId);
                playerGameData.SetNameAID(playerNetworkData.PlayerName, Object.InputAuthority.PlayerId);
                Debug.Log("PlayerName : " + playerGameData.PlayerName + "/PlayerId : " + playerGameData.PlayerID);
            }
            Debug.Log(this.gameObject.name);
            Bind_Camera(this.gameObject);
        }
    }

    
    public override void FixedUpdateNetwork()//逐每個tick更新(一個tick相當1.666毫秒)
    {
        //Debug.Log("speed : " + speed + "Acceleration : " + networkCharacterControllerPrototype.MoveSpeed + "SprintSpeed : " + sprintSpeed);
        ColorChangedByBreakDownPoint();
        Move();

        if (chargeAttackOrNot)
        {
            chargeAttackBarTimer++;
            chargeAttackBarTimer = (chargeAttackBarTimer >= (chargeAttackBar * 60)) ? (chargeAttackBar * 60) : chargeAttackBarTimer;
            curChargeAttackBK = (int)Mathf.Round((chargeAttackBarTimer / 60) * (chargeAttackMaxBK / chargeAttackBar));
            //Debug.Log("chargeAttackBarTimer : " + chargeAttackBarTimer);
        }
        else
        {
            chargeAttackBarTimer = 0;
        }
        if (FlapAnimPlay)//按下攻擊鍵後播放拍巴掌的動畫中
        {
            chargeAttackOrNot = true;
            //Debug.Log("Attack");
            PushCollision();
        }
        if (Network_CharacterControllerPrototype.IsGrounded)
        {
            BeenHitOrNot = false;//被擊中後落地時變成可以再被擊中的狀態
        }
        //if (CurHp <= 0)
        //{
        //    Respawn();
        //}

    }

    private void Move()
    {
        if(GetInput(out NetworkInputData data))
        {
            NetworkButtons buttons = data.Buttons;
            var pressed = buttons.GetPressed(ButtonsPrevious);//跟上一個按鈕去做比較
            var released = buttons.GetReleased(ButtonsPrevious);
            ButtonsPrevious = buttons;

            Network_CharacterControllerPrototype.MoveSpeed = pressed.IsSet(InputButtons.Sprint) ? drivingSpeed : released.IsSet(InputButtons.Sprint) ? speed : Network_CharacterControllerPrototype.MoveSpeed;

            if (pressed.IsSet(InputButtons.Sprint))
            {
                DrivingKeyStatus = true;
            }
            else if (released.IsSet(InputButtons.Sprint))
            {
                DrivingKeyStatus = false;
            }



            //if (data.Move == Vector3.zero) networkCharacterController.acceleration = 0.0f;

            //animationBlend = Mathf.Lerp(animationBlend, speed, Runner.DeltaTime * speedChangeRate);
            //if (animationBlend < 0.01f) animationBlend = 0f;

            // normalise input direction
            //Vector3 inputDirection = new Vector3(data.Move.x, 0.0f, data.Move.y).normalized;

            // note: Vector3's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            //if (data.Move != default)
            //{
            //    targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
            //                      mainCamera.transform.eulerAngles.y;
            //    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity,
            //        rotationSmoothTime);

            //    // rotate to face input direction relative to camera position
            //    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            //}
            //Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

            // move the player
            Vector3 moveVector = data.Move.normalized;
            Network_CharacterControllerPrototype.Move(moveVector * Runner.DeltaTime);

            if (pressed.IsSet(InputButtons.JUMP))
            {
                JumpEffectTrigger = true;
                Network_CharacterControllerPrototype.Jump();
                
                if (!Network_CharacterControllerPrototype.IsGrounded)
                {
                    JumpEffectTrigger = false;
                }
            }
            if (pressed.IsSet(InputButtons.Attack))
            {
                Debug.Log(data.Move);
                FlapAnimPlay = true;
                //Runner.Spawn(enemyPrefab, transform.position + new Vector3(1, -1, 1), Quaternion.identity, Object.StateAuthority);
                //Debug.Log("生產蜥蜴");
            }
            if (released.IsSet(InputButtons.Attack))
            {
                chargeAttackOrNot = false;
            }
        }
    }

    //private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    //{
    //    if (lfAngle < -360f) lfAngle += 360f;
    //    if (lfAngle > 360f) lfAngle -= 360f;
    //    return Mathf.Clamp(lfAngle, lfMin, lfMax);
    //}
    public override void Render()
    {
        if (DrivingKeyStatus && Network_CharacterControllerPrototype.IsGrounded && (Network_CharacterControllerPrototype.Velocity != Vector3.zero))//衝刺狀態下&&在地面時
        {
            playerEffectVisual.DrivingDustEffectPlay();//播放衝刺特效
        }
        else
        {
            playerEffectVisual.DrivingDustEffectStop();
        }
        if (JumpEffectTrigger)
        {
            playerEffectVisual.JumpingDustEffectPlay();//播放跳躍特效
            JumpEffectTrigger = false;
        }
        else
        {
            playerEffectVisual.JumpingDustEffectStop();
        }
    }
    private void Update()
    {
        if (Object.HasInputAuthority)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ChangeColor_RPC(Color.red);
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                ChangeColor_RPC(Color.green);
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                ChangeColor_RPC(Color.blue);
            }
        }
    }
    /*RPC可以遠端呼叫其他網路裝置的函數或方法
    RPC適用於同步那些狀態更新頻率比較低的資料，雖然他看上去非常簡單易用，但因為RPC並不是以Tick同步的，也不會保存狀態，這意味者RPC的同步不及時，且後加入的玩家會無法更新加入前的RPC，所以RPC通常不會是同步資料的最佳選擇。
    RPC使用時機：發送訊息、設定玩家資料、商城購買等等單一一次性的事件。*/
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void ChangeColor_RPC(Color newColor)
    {
        meshRenderer.material.color = newColor;
    }

    private void Respawn()//重生
    {
        Network_CharacterControllerPrototype.transform.position = Vector3.up * 2;
        //CurHp = maxHp;
    }

    private void PushCollision()//fusion官方不推薦使用unity的 OnTriggerEnter & OnTriggerCollision做網路上的物理碰撞，是因為Fusion網路狀態的更新率和Unity物理引擎的更新率不相同，而且無法做客戶端預測
    {
        var colliders = Physics.OverlapSphere(handCollider.transform.position + new Vector3(-0.0015f, 0, 0), radius: 0.002f);//畫一顆球，並檢測球裡的所有collider並回傳

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<PlayerController>(out PlayerController playerController) && !playerController.BeenHitOrNot)//判斷collider身上是否有PlayerController的腳本，並確認是否該對象被打擊過，如果沒有則
            {               
                // 計算推力方向

                var targetOriginPos = playerController.transform.position;
                targetOriginPos = new Vector3(targetOriginPos.x, 0, targetOriginPos.z);
                Vector3 pushDir = targetOriginPos - new Vector3(transform.position.x, 0, transform.position.z);

                Debug.Log(pushDir.magnitude);

                // 推動其他角色
                if (Object.HasStateAuthority)//只會在伺服器端上運行
                {
                    playerController.AddCoefficientOfBreakDownPoint(normalAttackBK);//代入普攻BK係數
                    playerController.AddCoefficientOfBreakDownPoint(curChargeAttackBK);//代入蓄力BK係數
                    playerController.Network_CharacterControllerPrototype.Jump();
                    playerController.Network_CharacterControllerPrototype.Move(Vector3.zero);
                    playerController.Network_CharacterControllerPrototype.Velocity += pushDir * (PushForce + playerController.playerGameData.BreakPoint);//推力計算
                    
                    //playerController.GetComponentInParent<CharacterController>().Move(pushDir.normalized * pushForce * Runner.DeltaTime);
                }
                playerController.BeenHitOrNot = true;
                Debug.Log(pushDir * (PushForce + playerController.playerGameData.BreakPoint));
                //playerController.GetComponentInParent<PlayerController>().TakeDamage(10);
                //Debug.Log("Push!!!!!!!");
                //Runner.Despawn(Object);
            }
            else
            {
                // 沒有找到組件
                // 做一些錯誤處理
            }
            //if (collider.TryGetComponent<BreakableWallBehaviour>(out BreakableWallBehaviour breakableWall))
            //{
            //    breakableWall.HurtThisWall();
            //}
        }
    }

    public void TakeDamage(int damage)
    {
        if (Object.HasStateAuthority)//只會在伺服器端上運行
        {
            //CurHp -= damage;
        }
    }
    public void AddCoefficientOfBreakDownPoint(float cob)
    {
        CoefficientOfBreakDownPoint += cob;
        playerGameData.BreakPoint = playerGameData.BreakDownPointCurve.Evaluate(CoefficientOfBreakDownPoint);//依據BK曲線計算BK值
    }
    //private static void OnHpChanged(Changed<PlayerController> changed)//changed代表變化後的值，可以透過changed來存取資料
    //{
    //    changed.Behaviour.CurHpBar.fillAmount = (float)changed.Behaviour.CurHp / changed.Behaviour.maxHp;
    //}

    public void ColorChangedByBreakDownPoint()
    {
        if (CoefficientOfBreakDownPoint >= 120.0f)//120是曲線x最陡的位置
        {
            CurBKBar.color = Color.red;
        }
        else
        {
            CurBKBar.color = Color.green;
        }
        CurBKBar.fillAmount = CoefficientOfBreakDownPoint / 200.0f;//200是曲線x的最末端
    }



    public void OnFlapAnimationStart()
    {
        
    }
    public void OnFlapAnimationEnd()
    {

    }


    #region - Player Mouse Setting -
    private void OnApplicationFocus(bool hasFocus)//當應用程式窗口的焦點狀態發生改變時，hasFocus為false；當應用程式窗口獲得焦點時，該函式的參數hasFocus為true
    {
        SetCursorState(cursorLocked);
    }
    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;//將滑鼠鎖定狀態設置為true，以將滑鼠固定在遊戲中心點
    }
    #endregion
    public void Bind_Camera(GameObject Player)
    {
        var CinemachineVirtualCamera = Camera.main.gameObject.transform.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
        CinemachineVirtualCamera.LookAt = Player.transform;
        CinemachineVirtualCamera.Follow = Player.transform;
    }
}

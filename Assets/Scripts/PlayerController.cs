using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using Cinemachine;
using TMPro;

[RequireComponent(typeof(CharacterController))]
//[RequireComponent(typeof(PlayerInput))]
public class PlayerController : NetworkBehaviour
{
    #region - Player Public Networked 變量 -
    [Networked]
    public NetworkButtons ButtonsPrevious { get; set; }//上一個按鈕的資料

    [Networked]
    [Tooltip("贏家")]
    public bool Winner { get; set; }
    [Networked]
    [Tooltip("輸家")]
    public bool Loser { get; set; }

    [Networked]
    [Tooltip("玩家正在使用傳送功能中")]
    public bool PlayerIsTeleporting { get; set; }
    [Networked]
    [Tooltip("玩家無敵的開關")]
    public bool PlayerImmuneDamage { get; set; }

    [Networked]
    [Tooltip("限制玩家移動的開關")]
    public bool PlayerMoveLimitOrNot { get; set; }

    [Networked]
    [Tooltip("是否被擊中過")]
    public bool BeenHitOrNot { get; set; }
    [Networked]
    [Tooltip("角色是否為蓄力狀態")]
    public bool ChargeAttackOrNot { get; set; }
    [Networked]
    [Tooltip("角色是否從船上出局")]
    public bool OutOfTheBoat { get; set; }

    [Networked]
    [Tooltip("角色巴掌力度")]
    public float PushForce { get; private set; }

    [Networked]
    [Tooltip("角色BK係數(曲線X軸)")]
    public float CoefficientOfBreakDownPoint { get; private set; }

    [Networked]
    [Tooltip("角色蓄力計時器")]
    public float ChargeAttackBarTimer { get; private set; }

    [Networked]
    [Tooltip("從哪裡受傷")]
    public Vector3 LocalHurt { get; set; }

    #endregion
    //------------------------------------------------------------------------------------------------------------------------
    #region - Player Public Networked HideInInspector 變量 -
    [Networked]
    [HideInInspector]
    [Tooltip("衝刺鍵狀態")]
    public bool DrivingKeyStatus { get; private set; }
    [Networked]
    [HideInInspector]
    [Tooltip("防止不斷播放跳躍特效，false為不可播放")]
    public bool JumpEffectTrigger { get; private set; }
    [Networked]
    [HideInInspector]
    [Tooltip("打擊特效，false為不可播放")]
    public bool HitEffectTrigger { get; private set; }
    #endregion
    //------------------------------------------------------------------------------------------------------------------------
    #region - Player Public 變量 -
    public PlayerGameData PlayerGameData;

    public NetworkCharacterControllerPrototype Network_CharacterControllerPrototype = null;

    public bool cursorInputForLook = true;
    //public AudioClip LandingAudioClip;
    //public AudioClip[] FootstepAudioClips;
    //[Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    #endregion
    //------------------------------------------------------------------------------------------------------------------------
    #region - Player Public HideInInspector 變量 -
    [HideInInspector]
    [Tooltip("播放拍巴掌動畫的狀態")]
    public bool FlapAnimPlay;
    [HideInInspector]
    [Tooltip("播放蓄力攻擊動畫的狀態")]
    public bool ChargeFlapAnimPlay;
    [HideInInspector]
    [Tooltip("碰撞啟用")]
    public bool collisionAvailable;
    [HideInInspector]
    [Tooltip("播放出局動畫")]
    public bool OutAnimPlay;
    #endregion
    //------------------------------------------------------------------------------------------------------------------------
    #region - Player Private SerializeField 變量 -
    [SerializeField, Tooltip("衝刺速度")]
    private float drivingSpeed;

    [SerializeField, Tooltip("擊飛力道")]
    private float airborneAmount;

    [Tooltip("角色蓄力BK傷害加成係數(百分比)的最大值")]
    [SerializeField]
    [Range(150, 250)]
    private int chargeAttackMaxBK;



    [SerializeField, Tooltip("Mouse Cursor Settings")]
    private bool cursorLocked = true;
    #endregion
    //------------------------------------------------------------------------------------------------------------------------
    #region - Player Private SerializeField Componment -
    [SerializeField]
    private EnemyAIBehavior enemyPrefab;

    [SerializeField]
    private AudioSource playerAudioSource;

    [SerializeField]
    private AudioClip[] playerAudioClips;

    [SerializeField]
    private GameObject bonkCollider;//手掌Collider(用於偵測其他人的PlayerController腳本)

    [SerializeField]
    private Image curBKBar = null;

    [SerializeField]
    private Image curChargeAttackBar = null;

    [SerializeField]
    private TMP_Text playerNameText = null;

    [SerializeField]
    private SkinnedMeshRenderer skinnedMeshRenderer = null;
    #endregion
    //------------------------------------------------------------------------------------------------------------------------




    //[SerializeField]
    //private int maxHp = 100;
    //[Networked(OnChanged =nameof(OnHpChanged))]//血量數值每次一有變化就刷新
    //public int CurHp { get; set; }

    // cinemachine
    //private float cinemachineTargetYaw;
    //private float cinemachineTargetPitch;


    #region - private 變量 & Componment -
    [Tooltip("角色普攻BK值")]
    private int normalAttackBK = 5;

    [Tooltip("角色蓄力BK值")]
    private int chargeAttackBK = 10;

    [Tooltip("角色當前攻擊BK值")]
    private float curAttackBK;

    [Tooltip("角色蓄力百分比")]
    private float chargeAttackPercent;

    [Tooltip("角色出局計時器")]
    private float playerOutTimer;

    [Tooltip("角色暫存速度")]
    private float speed;

    [Tooltip("角色限制移動的參數")]
    private int moveLimitParameter;//限制移動的參數
    private const int moveLimit_Y = 0;
    private const int moveLimit_N = 1;
    
    private PlayerEffectVisual playerEffectVisual;
    private GameObject mainCamera;
    #endregion





    public override void Spawned()
    {
        moveLimitParameter = moveLimit_N;
        FlapAnimPlay = false;
        BeenHitOrNot = false;
        DrivingKeyStatus = false;
        HitEffectTrigger = false;
        playerEffectVisual = GetComponent<PlayerEffectVisual>();
        playerAudioSource = GetComponent<AudioSource>();
        playerEffectVisual.InitializeVisualEffect();//因為是所有物件(包括IsProxy)都要顯示的特效，所以放在外面
        playerEffectVisual.InitializeParticleEffect();
        playerEffectVisual.HitEffectStop();

        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
        //cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

        speed = Network_CharacterControllerPrototype.MoveSpeed;
        if (Object.HasStateAuthority)//只會在伺服器端上運行
        {
            //CurHp = maxHp;//初始化血量
            Winner = false;
            Loser = false;
            PlayerIsTeleporting = false;
            OutOfTheBoat = false;
            ChargeAttackOrNot = false;
            CoefficientOfBreakDownPoint = 0.0f;//初始化角色BK值
            ChargeAttackBarTimer = 0.0f;

            playerOutTimer = 0.0f;
        }
        if (Object.HasInputAuthority)//在客戶端上運行
        {
            Debug.Log(this.gameObject.name);
            Bind_Camera(this.gameObject);
        }

        Invoke("setPlayerData_RPC", 0.2f);//因為要拿PlayerNetworkData的關係，有先後順序的問題，所以晚一點才設置角色的名稱

    }
    public override void FixedUpdateNetwork()//逐每個tick更新(一個tick相當1.666毫秒)
    {
        //Debug.Log("speed : " + speed + "Acceleration : " + networkCharacterControllerPrototype.MoveSpeed + "SprintSpeed : " + sprintSpeed);
        //Debug.Log(Network_CharacterControllerPrototype.Velocity);

        ColorChangedByBreakDownPoint();//動態顯示BK狀態的顏色
        ColorChangedByChargeAttackBar();//動態顯示蓄力條的顏色

        PlayerImmuneDamage = Winner || Loser || PlayerIsTeleporting;

        if (!OutOfTheBoat && DetectOutCollider())
        {
            playerOutTimer += Runner.DeltaTime;
            if (playerOutTimer >= 1.0f)//離開船上1秒後算出局
            {
                OutOfTheBoat = true;
                OutAnimPlay = true;
                playerOutTimer = 0.0f;
            }
        }

        if (Loser)
        {
            if (GameManager.Instance.PlayerList.TryGetValue(Object.Runner.LocalPlayer, out PlayerNetworkData playerNetworkData) && Object.HasInputAuthority)
            {
                playerNetworkData.SetPlayerOut_RPC(OutOfTheBoat);
            }
        }

        if (Winner || Loser || PlayerIsTeleporting)
            return;
        
        Move();

        if (BeenHitOrNot || OutOfTheBoat)
        {
            PlayerMoveLimitOrNot = true;
        }
        else
        {
            PlayerMoveLimitOrNot = false;
        }

        if ((collisionAvailable && FlapAnimPlay) || (collisionAvailable && ChargeFlapAnimPlay))
        {
            PushCollision();//碰撞啟動
        }

        

        //if (CurHp <= 0)
        //{
        //    Respawn();
        //}

    }

    private void Move()
    {
        if (GetInput(out NetworkInputData data))
        {
            NetworkButtons buttons = data.Buttons;
            var pressed = buttons.GetPressed(ButtonsPrevious);//跟上一個按鈕去做比較
            var released = buttons.GetReleased(ButtonsPrevious);
            ButtonsPrevious = buttons;

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
            moveLimitParameter = PlayerMoveLimitOrNot ? moveLimit_Y : moveLimit_N;
            Network_CharacterControllerPrototype.Move(moveVector * moveLimitParameter * Runner.DeltaTime);


            //if (pressed.IsSet(InputButtons.Attack))
            //{
            //    //Runner.Spawn(enemyPrefab, transform.position + new Vector3(1, -1, 1), Quaternion.identity, Object.StateAuthority);
            //    //Debug.Log("生產蜥蜴");
            //}
            #region - 跳躍按鍵處理 -
            if (pressed.IsSet(InputButtons.JUMP))
            {
                JumpEffectTrigger = true;
                Network_CharacterControllerPrototype.Jump();

                if (!Network_CharacterControllerPrototype.IsGrounded)
                {
                    JumpEffectTrigger = false;
                }
            }
            #endregion

            #region - 攻擊按鍵處理 -
            if (pressed.IsSet(InputButtons.Attack))//按下攻擊鍵後播放拍巴掌的動畫中
            {
                ChargeAttackOrNot = true;
            }
            if (released.IsSet(InputButtons.Attack))
            {
                ChargeAttackOrNot = false;
            }
            #endregion

            #region - 蓄力攻擊邏輯處理 -
            if (!ChargeAttackOrNot)
            {
                Network_CharacterControllerPrototype.MoveSpeed = pressed.IsSet(InputButtons.Sprint) ? drivingSpeed : released.IsSet(InputButtons.Sprint) ? speed : Network_CharacterControllerPrototype.MoveSpeed;
                if (pressed.IsSet(InputButtons.Sprint))
                {
                    DrivingKeyStatus = true;
                }
                else if (released.IsSet(InputButtons.Sprint))
                {
                    DrivingKeyStatus = false;
                }
            }
            if (ChargeAttackOrNot)//蓄力計時開始
            {
                ChargeAttackBarTimer += Runner.DeltaTime;

                PushForce = 50;

                if (ChargeAttackBarTimer > 0.5f)
                {
                    Network_CharacterControllerPrototype.MoveSpeed = speed;//回到走路速度
                    DrivingKeyStatus = false;//關閉加速特效

                    ChargeFlapAnimPlay = true;
                }
            }
            else if (!ChargeAttackOrNot && ChargeAttackBarTimer > 0.5f)//蓄力0.5秒以上
            {
                //Debug.Log("本次蓄力時間 : " + ChargeAttackBarTimer);
                //1~5秒有效蓄力時間
                ChargeAttackBarTimer = ChargeAttackBarTimer > 2.0f ? ChargeAttackBarTimer = 2 : ChargeAttackBarTimer;
                //Debug.Log("有效蓄力時間 : " + ChargeAttackBarTimer);
                //Debug.Log("蓄力時間百分比 : " + chargeAttackPercent);
                //0%~100%蓄力時間百分比
                chargeAttackPercent = (ChargeAttackBarTimer - 0.5f) / (2.0f - 0.5f) * 1.0f;
                float chargeAttackCoefficient = ((float)chargeAttackMaxBK / 100 - 1) * chargeAttackPercent + 1;//基礎100%數~蓄滿力chargeAttackMaxBK%
                curAttackBK = chargeAttackCoefficient * chargeAttackBK;
                PushForce = chargeAttackCoefficient * PushForce;
                //Debug.Log("蓄力傷害加成 : " + chargeAttackPercent);
                ChargeAttackBarTimer = 0;
            }
            else if (!ChargeAttackOrNot && ChargeAttackBarTimer > 0 && ChargeAttackBarTimer <= 0.5f) //蓄力小於0.5秒
            {
                curAttackBK = normalAttackBK;
                PushForce = 50;
                //Debug.Log("本次蓄力時間 : " + ChargeAttackBarTimer);
                ChargeAttackBarTimer = 0;
                FlapAnimPlay = true;
            }
            #endregion



            #region - 呼叫滑鼠按鍵處理 -
            if (pressed.IsSet(InputButtons.CursorUse))
            {
                cursorLocked = false;
            }
            if (released.IsSet(InputButtons.CursorUse))
            {
                cursorLocked = true;
            }
            #endregion
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
        if (Winner || Loser || PlayerIsTeleporting)
            return;

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
        if (HitEffectTrigger)
        {
            playerEffectVisual.HitEffectPlay();//播放打擊特效
            HitEffectTrigger = false;
        }
        else
        {
            if ((collisionAvailable && FlapAnimPlay) || (collisionAvailable && ChargeFlapAnimPlay))
            {
                playerEffectVisual.HitEffectStop();//再碰撞啟動時，事先重置打擊特效
            }
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
        skinnedMeshRenderer.material.color = newColor;
    }

    private void Respawn()//重生
    {
        Network_CharacterControllerPrototype.transform.position = Vector3.up * 2;
        //CurHp = maxHp;
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
        PlayerGameData.BreakPoint = PlayerGameData.BreakDownPointCurve.Evaluate(CoefficientOfBreakDownPoint);//依據BK曲線計算BK值
    }
    //private static void OnHpChanged(Changed<PlayerController> changed)//changed代表變化後的值，可以透過changed來存取資料
    //{
    //    changed.Behaviour.CurHpBar.fillAmount = (float)changed.Behaviour.CurHp / changed.Behaviour.maxHp;
    //}

    #region - 碰撞邏輯處理 -
    private void PushCollision()//fusion官方不推薦使用unity的 OnTriggerEnter & OnTriggerCollision做網路上的物理碰撞，是因為Fusion網路狀態的更新率和Unity物理引擎的更新率不相同，而且無法做客戶端預測
    {
        //var colliders = Physics.OverlapSphere(bonkCollider.transform.position + new Vector3(-0.001f, 0, 0), radius: 0.0035f);//畫一顆球，並檢測球裡的所有collider並回傳
        var colliders = Physics.OverlapBox(bonkCollider.transform.position + new Vector3(0f, -0.5f, 0f), new Vector3(transform.localScale.x / 3, transform.localScale.y / 1.5f, transform.localScale.z / 3f));//畫一個cube，並檢測cube裡的所有collider並回傳

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<PlayerController>(out PlayerController playerController) && !playerController.BeenHitOrNot && !playerController.PlayerImmuneDamage)//判斷collider身上是否有PlayerController的腳本，並確認是否該對象被打擊過，如果Player不是無敵狀態則
            {
                // 計算推力方向
                collisionAvailable = false;//碰撞一次馬上關閉碰撞

                var targetOriginPos = playerController.transform.position;
                targetOriginPos = new Vector3(targetOriginPos.x, 0, targetOriginPos.z);
                Vector3 pushDir = targetOriginPos - new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 airborneVec = new Vector3(0, airborneAmount, 0);

                Debug.Log(pushDir.magnitude);
                HitEffectTrigger = true;
                // 推動其他角色
                if (Object.HasStateAuthority)//只會在伺服器端上運行
                {
                    playerController.AddCoefficientOfBreakDownPoint(curAttackBK);//代入普攻BK係數
                    //playerController.AddCoefficientOfBreakDownPoint(curChargeAttackBK);//代入蓄力BK係數

                    //PlayASoundForEachPlayer(playerController);
                    playerController.soundEffectPlay_RPC();
                    //playerController.Network_CharacterControllerPrototype.Jump();
                    playerController.Network_CharacterControllerPrototype.Move(Vector3.zero);
                    playerController.Network_CharacterControllerPrototype.Velocity += airborneVec * (PushForce + playerController.PlayerGameData.BreakPoint);//垂直推力計算
                    playerController.Network_CharacterControllerPrototype.Velocity += pushDir * (PushForce + playerController.PlayerGameData.BreakPoint);//水平推力計算

                    playerController.LocalHurt = playerController.transform.InverseTransformDirection((playerController.transform.position - new Vector3(transform.position.x, 0, transform.position.z)));
                    playerController.BeenHitOrNot = true;
                    Debug.Log("X : " + playerController.LocalHurt.x + "Y : " + playerController.LocalHurt.y + "Z : " + playerController.LocalHurt.z);
                    //playerController.GetComponentInParent<CharacterController>().Move(pushDir.normalized * pushForce * Runner.DeltaTime);
                }
                

                Debug.Log(pushDir * (PushForce + playerController.PlayerGameData.BreakPoint));
                //playerController.GetComponentInParent<PlayerController>().TakeDamage(10);
                //Debug.Log("Push!!!!!!!");
                //Runner.Despawn(Object);
            }
            else
            {
                // 沒有找到組件
                // 做一些錯誤處理
            }
            if (collider.TryGetComponent<BreakableWallBehaviour>(out BreakableWallBehaviour breakableWall))//破壞牆
            {
                breakableWall.HurtThisWall();
            }
            if (collider.TryGetComponent<Teleporter>(out Teleporter teleporter))//傳送
            {
                teleporter.TriggerTeleporter(Object);//觸發傳送
                Network_CharacterControllerPrototype.Velocity = Vector3.zero;
                teleporter.Invoke("StartTeleportingCountDown", 2f);
                //teleporter.StartTeleportingCountDown();//傳送開始
            }
            if(collider.TryGetComponent<TreasureBoxBehavior>(out TreasureBoxBehavior treasureBox))
            {
                //開啟寶箱
                treasureBox.TreasureSound();
            }
        }
    }
    #endregion

    #region - 動態UI邏輯處理 -
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void setPlayerData_RPC()
    {
        if (GameManager.Instance.PlayerList.TryGetValue(Object.InputAuthority, out PlayerNetworkData playerNetworkData))
        {
            PlayerGameData.SetNameAID(playerNetworkData.PlayerName, Object.InputAuthority.PlayerId);
            playerNameText.text = playerNetworkData.PlayerName.ToString();
            Debug.Log("PlayerName : " + PlayerGameData.PlayerName + "/PlayerId : " + PlayerGameData.PlayerID);
        }
    }
    public void ColorChangedByBreakDownPoint()
    {
        if (CoefficientOfBreakDownPoint >= 120.0f)//120是曲線x最陡的位置
        {
            curBKBar.color = Color.red;
        }
        else
        {
            curBKBar.color = Color.green;
        }
        curBKBar.fillAmount = CoefficientOfBreakDownPoint / 200.0f;//200是曲線x的最末端
    }
    public void ColorChangedByChargeAttackBar()
    {
        if(ChargeAttackBarTimer > 0 && ChargeAttackBarTimer <= 0.5f)
        {
            curChargeAttackBar.color = Color.green;
        }
        else if(ChargeAttackBarTimer > 0.5f && ChargeAttackBarTimer < 2.0f)
        {
            curChargeAttackBar.color = Color.yellow;
        }
        else if(ChargeAttackBarTimer >= 2.0f)
        {
            curChargeAttackBar.color = Color.red;
        }

        if (ChargeAttackOrNot && ChargeAttackBarTimer > 0.5f)//蓄力條顯示
        {
            curChargeAttackBar.fillAmount = (ChargeAttackBarTimer - 0.5f) / (2.0f - 0.5f) * 1.0f / 1;
        }
        else
        {
            curChargeAttackBar.fillAmount = 0;
        }
    }
    #endregion

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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        //Gizmos.DrawWireSphere(bonkCollider.transform.position + new Vector3(-0.001f, 0, 0), radius: 0.0035f);
        Gizmos.DrawWireCube(bonkCollider.transform.position + new Vector3(0f, -0.5f, 0f), new Vector3(transform.localScale.x / 1.5f, transform.localScale.y / 0.75f, transform.localScale.z / 1.5f));
    }

    #region - 玩家出局判斷 -
    public bool DetectOutCollider()
    {
        Debug.DrawLine(transform.position, new Vector3(0, -1000, 0), Color.red);
        RaycastHit hit;
        Ray ray = new Ray(transform.position, -Vector3.up);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("OutCollider"))
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region - 聲音處理 -
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void soundEffectPlay_RPC()
    {
        playerAudioSource.clip = playerAudioClips[0];
        playerAudioSource.Play();
    }
    #endregion

}

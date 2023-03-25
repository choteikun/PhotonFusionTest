using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using Cinemachine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private NetworkCharacterControllerPrototype networkCharacterControllerPrototype = null;

    [SerializeField]
    private Ball ballPrefab;

    [SerializeField]
    private Image CurHpBar = null;

    [SerializeField]
    private MeshRenderer meshRenderer = null;

    [SerializeField, Tooltip("衝刺速度")]
    private float sprintSpeed;

    [SerializeField, Tooltip("衝刺狀態")]
    private bool sprintStatus;

    //[SerializeField, Tooltip("角色轉向移動方向的平滑速度")]
    //[Range(0.0f, 0.3f)]
    //private float rotationSmoothTime = 0.12f;

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    //跳躍落地後再次跳躍的時間。在著陸後這段時間過去之前，不能跳躍
    public float JumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    //離開地面時過渡到墜落動畫的時間。當下樓梯時，防止在每一步後播放跌倒動畫的時間
    public float FallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    //是判斷是否落地
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")]
    //落地偏移植(適用於粗糙的地面)
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    //設置落地的collider範圍。使其與 CharacterController 組件的碰撞器半徑相同。
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    //設置判斷為地面的圖層
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    //相機將跟隨的 Cinemachine 虛擬相機中設置的跟隨目標
    public GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    //設置相機向上旋轉多少
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    //設置相機向下旋轉多少
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    //用於微調相機角度
    public float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    //檢查 LockCameraPosition 以鎖定當前相機角度
    public bool LockCameraPosition = false;

    [SerializeField]
    private int maxHp = 100;

    [SerializeField, Tooltip("Mouse Cursor Settings")]
    private bool cursorLocked = true;
    public bool cursorInputForLook = true;

    [Networked(OnChanged =nameof(OnHpChanged))]//血量數值每次一有變化就刷新
    public int CurHp { get; set; }

    [Networked]
    public NetworkButtons ButtonsPrevious { get; set; }//上一個按鈕的資料

    // cinemachine
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;

    // player
    private float speed;
    private float animationBlend;

    // timeout deltatime
    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;

    private GameObject mainCamera;
    private Animator animator;

    private const float threshold = 0.01f;

   
    public override void Spawned()
    {
        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
        cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        if (Object.HasStateAuthority)//只會在伺服器端上運行
        {
            speed = networkCharacterControllerPrototype.MoveSpeed;
            CurHp = maxHp;//初始化血量           
            // reset our timeouts on start
            jumpTimeoutDelta = JumpTimeout;
            fallTimeoutDelta = FallTimeout;
        }
        if (Object.HasInputAuthority)
        {
            Debug.Log(this.gameObject.name);
            Bind_Camera(this.gameObject);
        }
    }

    
    public override void FixedUpdateNetwork()//逐每個tick更新(一個tick相當1.666毫秒)
    {
        //Debug.Log("speed : " + speed + "Acceleration : " + networkCharacterControllerPrototype.MoveSpeed + "SprintSpeed : " + sprintSpeed);
        Move();
        if (CurHp <= 0)
        {
            Respawn();
        }
    }

    private void Move()
    {
        if(GetInput(out NetworkInputData data))
        {
            NetworkButtons buttons = data.Buttons;
            var pressed = buttons.GetPressed(ButtonsPrevious);//跟上一個按鈕去做比較
            var released = buttons.GetReleased(ButtonsPrevious);
            ButtonsPrevious = buttons;

            networkCharacterControllerPrototype.MoveSpeed = pressed.IsSet(InputButtons.Sprint) ? sprintSpeed : released.IsSet(InputButtons.Sprint) ? speed : networkCharacterControllerPrototype.MoveSpeed;
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
            networkCharacterControllerPrototype.Move(moveVector * Runner.DeltaTime);
            if (pressed.IsSet(InputButtons.JUMP))
            {
                networkCharacterControllerPrototype.Jump();
            }
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void Update()
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
        networkCharacterControllerPrototype.transform.position = Vector3.up * 2;
        CurHp = maxHp;
    }
    public void TakeDamage(int damage)
    {
        if (Object.HasStateAuthority)//只會在伺服器端上運行
        {
            CurHp -= damage;//初始化血量
        }
    }
    private static void OnHpChanged(Changed<PlayerController> changed)//changed代表變化後的值，可以透過changed來存取資料
    {
        changed.Behaviour.CurHpBar.fillAmount = (float)changed.Behaviour.CurHp / changed.Behaviour.maxHp;
    }



    private void OnApplicationFocus(bool hasFocus)//當應用程式窗口的焦點狀態發生改變時，hasFocus為false；當應用程式窗口獲得焦點時，該函式的參數hasFocus為true
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;//將滑鼠鎖定狀態設置為true，以將滑鼠固定在遊戲中心點
    }
    public void Bind_Camera(GameObject Player)
    {
        var CinemachineVirtualCamera = Camera.main.gameObject.transform.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
        CinemachineVirtualCamera.LookAt = Player.transform;
        CinemachineVirtualCamera.Follow = Player.transform;
    }
}

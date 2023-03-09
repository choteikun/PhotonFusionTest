using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private NetworkCharacterControllerPrototype networkCharacterController = null;

    [SerializeField]
    private Ball ballPrefab;

    [SerializeField]
    private Image CurHpBar = null;

    [SerializeField]
    private float moveSpeed = 15f;

    [SerializeField]
    private int maxHp = 100;

    [Networked(OnChanged =nameof(OnHpChanged))]//血量數值每次一有變化就刷新
    public int CurHp { get; set; }

    [Networked]
    public NetworkButtons ButtonsPrevious { get; set; }//上一個按鈕的資料

    public override void Spawned()
    {
        if (Object.HasStateAuthority)//只會在伺服器端上運行
        {
            CurHp = maxHp;//初始化血量
        }
    }

    public override void FixedUpdateNetwork()//逐每個tick更新(一個tick相當1.666毫秒)
    {
        if(GetInput(out NetworkInputData data))
        {
            NetworkButtons buttons = data.buttons;
            var pressed = buttons.GetPressed(ButtonsPrevious);//跟上一個按鈕去做比較
            ButtonsPrevious = buttons;

            Vector3 moveVector = data.movementInput.normalized;
            networkCharacterController.Move(moveSpeed * moveVector * Runner.DeltaTime);//Runner不同於Delta是以每Tick去計算的而不是不是每秒

            if (pressed.IsSet(InputButtons.JUMP))
            {
                networkCharacterController.Jump();
            }

            if (pressed.IsSet(InputButtons.FIRE))
            {
                Runner.Spawn(
                    ballPrefab,
                    transform.position + transform.TransformDirection(Vector3.forward),
                    Quaternion.LookRotation(transform.TransformDirection(Vector3.forward)),
                    Object.InputAuthority
                    );
            }
        }
        if (CurHp <= 0)
        {
            Respawn();
        }
    }

    private void Respawn()//重生
    {
        networkCharacterController.transform.position = Vector3.up * 2;
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
}

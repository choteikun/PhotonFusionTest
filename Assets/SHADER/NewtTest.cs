using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewtTest : MonoBehaviour
{
    public CharacterController controller;

    

    [HideInInspector]
    public Vector3 Move;
    [HideInInspector]
    public Vector3 playerVelocity;
    [HideInInspector]
    public bool NewtFlapAnimPlay;
    [HideInInspector]
    public bool NewtChargeFlapAnimPlay;

    [HideInInspector]
    public bool NewtChargeAttackOrNot;
    [HideInInspector]
    public float NewtChargeAttackBarTimer;

    public float PlayerSpeed;
    [SerializeField]
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    private float newtChargeAttackPercent;

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
    }

    void Update()
    {
        if (NewtIsGrounded() && playerVelocity.y < 0)
        {
            playerVelocity.y = 0;
        }

        Move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(Move * Time.deltaTime * PlayerSpeed);

        if (Move != Vector3.zero)
        {
            gameObject.transform.forward = Move;
        }

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && NewtIsGrounded())
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);


        if (Input.GetKeyDown(KeyCode.Mouse0))//按下攻擊鍵後播放拍巴掌的動畫中
        {
            NewtChargeAttackOrNot = true; 
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            NewtChargeAttackOrNot = false;
        }

        if (NewtChargeAttackOrNot)//蓄力計時開始
        {
            NewtChargeAttackBarTimer += Time.deltaTime;

            if (NewtChargeAttackBarTimer > 0.5f)
            {
                NewtChargeFlapAnimPlay = true;
            }
        }
        else if (!NewtChargeAttackOrNot && NewtChargeAttackBarTimer > 0.5f)//蓄力0.5秒以上
        {
            //1~5秒有效蓄力時間
            NewtChargeAttackBarTimer = NewtChargeAttackBarTimer > 2.0f ? NewtChargeAttackBarTimer = 2 : NewtChargeAttackBarTimer;
            //0%~100%蓄力時間百分比
            newtChargeAttackPercent = (NewtChargeAttackBarTimer - 0.5f) / (2.0f - 0.5f) * 1.0f;
            NewtChargeAttackBarTimer = 0;
        }
        else if (!NewtChargeAttackOrNot && NewtChargeAttackBarTimer > 0 && NewtChargeAttackBarTimer <= 0.5f) //蓄力小於0.5秒
        {
            NewtChargeAttackBarTimer = 0;
            NewtFlapAnimPlay = true;
        }
    }
    public bool NewtIsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 0.1f);
    }
}

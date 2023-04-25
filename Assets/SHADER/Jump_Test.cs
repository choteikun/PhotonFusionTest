using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Jump_Test : MonoBehaviour
{
    private CharacterController cc;
    private Vector3 playerVelocity;
    private float playerSpeed = 2.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    private bool isGrounded = true; // 是否在地面上
    private Vector3 jumpPos;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
    }
    void Update()
    {
        
        // 檢查是否在地面上
        //isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.5f);
        CharacterCollision();
        if(isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0;
        }

        // 獲取水平和垂直軸向的輸入
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // 計算移動方向和距離
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput);
        cc.Move(moveDirection * Time.deltaTime * playerSpeed);
        if (moveDirection != Vector3.zero)
        {
            gameObject.transform.forward = moveDirection;
        }
        //moveDirection.Normalize();

        //// 計算移動量
        //Vector3 moveAmount = moveDirection * speed * Time.deltaTime;

        //// 如果正在移動，創建火花效果
        //if (moveAmount.magnitude > 0.000001f)
        //{
        //    if (!isMoving) // 只在移動開始時創建一次
        //    {
                
        //        isMoving = true;
        //    }
        //}
        //else // 如果沒有移動，停止火花效果
        //{
           
        //    isMoving = false;
        //}

        // 檢查是否按下跳躍鍵且在地面上
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            //jumpPos = transform.position;
            //// 添加向上的力量，讓角色跳躍
            //GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }
        playerVelocity.y += gravityValue * Time.deltaTime;
        cc.Move(playerVelocity * Time.deltaTime);
        //// 將移動量應用於角色位置
        //transform.position += moveAmount;
    }
    private void CharacterCollision()
    {
        var colliders = Physics.OverlapSphere(new Vector3(transform.position.x,transform.position.y-0.5f,transform.position.z), radius: 100f);//畫一顆球，並檢測球裡的所有collider並回傳

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Floor"))
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }
    }
}

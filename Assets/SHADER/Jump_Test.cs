using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Jump_Test : MonoBehaviour
{
    public float speed = 5f; // 移動速度
    public float jumpForce = 10f; // 跳躍力量

    private bool isMoving = false; // 是否正在移動
    private bool isGrounded = true; // 是否在地面上
    private Vector3 jumpPos;

    private void Start()
    {
     


    }
    void Update()
    {
        // 檢查是否在地面上
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.5f);

        // 獲取水平和垂直軸向的輸入
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // 計算移動方向和距離
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput);
        moveDirection.Normalize();

        // 計算移動量
        Vector3 moveAmount = moveDirection * speed * Time.deltaTime;

        // 如果正在移動，創建火花效果
        if (moveAmount.magnitude > 0.000001f)
        {
            if (!isMoving) // 只在移動開始時創建一次
            {
                
                isMoving = true;
            }
        }
        else // 如果沒有移動，停止火花效果
        {
           
            isMoving = false;
        }

        // 檢查是否按下跳躍鍵且在地面上
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpPos = transform.position;
            // 添加向上的力量，讓角色跳躍
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // 將移動量應用於角色位置
        transform.position += moveAmount;
    }
}

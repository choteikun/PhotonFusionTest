using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Jump_Test : MonoBehaviour
{
    public float speed = 5f; // ���ʳt��
    public float jumpForce = 10f; // ���D�O�q

    private bool isMoving = false; // �O�_���b����
    private bool isGrounded = true; // �O�_�b�a���W
    private Vector3 jumpPos;

    private void Start()
    {
     


    }
    void Update()
    {
        // �ˬd�O�_�b�a���W
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.5f);

        // ��������M�����b�V����J
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // �p�Ⲿ�ʤ�V�M�Z��
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput);
        moveDirection.Normalize();

        // �p�Ⲿ�ʶq
        Vector3 moveAmount = moveDirection * speed * Time.deltaTime;

        // �p�G���b���ʡA�Ыؤ���ĪG
        if (moveAmount.magnitude > 0.000001f)
        {
            if (!isMoving) // �u�b���ʶ}�l�ɳЫؤ@��
            {
                
                isMoving = true;
            }
        }
        else // �p�G�S�����ʡA�������ĪG
        {
           
            isMoving = false;
        }

        // �ˬd�O�_���U���D��B�b�a���W
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpPos = transform.position;
            // �K�[�V�W���O�q�A��������D
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // �N���ʶq���Ω󨤦��m
        transform.position += moveAmount;
    }
}

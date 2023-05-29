using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewtModelDisplayer : MonoBehaviour
{

    public float speed;

    private Animator anim;
    
    void Start()
    {
        anim = GetComponent<Animator>();

        anim.SetBool("Idle", true);
    }

    void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * speed);
    }
}

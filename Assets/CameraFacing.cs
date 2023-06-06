using UnityEngine;
using System.Collections;
using Cinemachine;

public class CameraFacing : MonoBehaviour
{   
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    public bool reverFace = false;
    private Transform mRoot;

    private void Awake()
    {
        if (!cinemachineVirtualCamera)
        {
            cinemachineVirtualCamera = Camera.main.gameObject.transform.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
        }
        mRoot = transform;
    }


    private void LateUpdate()
    {
        Vector3 targetPos = mRoot.position + cinemachineVirtualCamera.transform.rotation * (reverFace ? Vector3.back : Vector3.forward);
        Vector3 targetOrientation = cinemachineVirtualCamera.transform.rotation * Vector3.up;
        mRoot.LookAt(targetPos, targetOrientation);
        //transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.back, Camera.main.transform.rotation * Vector3.up);
    }
}

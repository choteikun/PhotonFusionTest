using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class Gear : MonoBehaviour
{
    VisualEffect visualEffect;
    VFXEventAttribute eventAttribute;

    static readonly ExposedProperty vfx_curveTrigger = "CurveTrigger";
    static readonly ExposedProperty vfs_hitEvent = "HIT";

    void Start()
    {
        visualEffect = GetComponent<VisualEffect>();
        // Caches an Event Attribute matching the
        // visualEffect.visualEffectAsset graph.
        eventAttribute = visualEffect.CreateVFXEventAttribute();
    }
    //private void Update()
    //{
    //    //Debug.Log(visualEffect.GetBool(vfx_curveTrigger));
    //}

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Debug.Log("Gear");

            // Sets some Attributes
            //eventAttribute.SetVector3(vfx_Pos, transform.position);
            visualEffect.SetBool(vfx_curveTrigger, false);

            // Sends the Event
            visualEffect.SendEvent(vfs_hitEvent, eventAttribute);
        }
    }

}

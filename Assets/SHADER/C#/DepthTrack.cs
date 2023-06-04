using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

// A behaviour that is attached to a playable

public class DepthTrack : PlayableBehaviour
{

    public float focusDistance;

    PostProcessVolume volume;

    float m_focusDistance = -1;
    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {
        
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
        
    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        
    }
    // Called each frame while the state is set to Play
    public override void ProcessFrame(Playable playable, FrameData info,object playerData)
    {
        volume = (PostProcessVolume)playerData;//此為可以拖放進來的自訂義物

    }
}

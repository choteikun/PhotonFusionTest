using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Playables;

[TrackColor(102f/255f,204f/255f,255f/255f)]//展示的顏色#66ccff
[TrackBindingType(typeof(PostProcessVolume))]
[TrackClipType(typeof(DepthOfField))]

public class DepthOfField : TrackAsset
{
   
}

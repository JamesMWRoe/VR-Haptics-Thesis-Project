using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PHFDataScriptableObject", menuName = "ScriptableObjects/PHFCurve")]
public class PHFCurveScriptableObject : ScriptableObject
{
    [SerializeField] public AnimationCurve frequencyCurve;
    [SerializeField] public AnimationCurve gainCurve;
    public float frameDuration = 20;
    public string Name;

    public List<PHFFrame> buildFrameData()
    {
        List<PHFFrame> phfFrames = new List<PHFFrame>();
        Keyframe lastKeyFrame = this.frequencyCurve.keys[frequencyCurve.length - 1];
        float endTime = lastKeyFrame.time;
        float frameDurationInSeconds = this.frameDuration/1000;

        int i = 1;

        for (float frameTime = 0; frameTime < endTime; frameTime += frameDurationInSeconds)
        {

            PHFFrame currentFrameToAdd = new PHFFrame(i, (int)(this.frequencyCurve.Evaluate(frameTime)), this.gainCurve.Evaluate(frameTime));
            i++;
            
            if (frameTime == 0)
            {  currentFrameToAdd.IsFirstFrame();  }

            phfFrames.Add(currentFrameToAdd);
        }

        return phfFrames;
    }

    public List<PHFFrame> buildEmptyFrameData()
    {
        List<PHFFrame> phfFrames = new List<PHFFrame>();
        PHFFrame emptyFrame = new PHFFrame(0, 0, 0.0f);
        phfFrames.Add(emptyFrame);

        return phfFrames;
    }
}

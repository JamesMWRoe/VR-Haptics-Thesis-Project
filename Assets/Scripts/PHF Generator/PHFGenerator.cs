using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public static class PHFGenerator
{
    public enum ControllerVibrateType { Left, Right, Both };

    public static void GenerateFile(PHFCurveScriptableObject phfFrameData, ControllerVibrateType controllerToVibrate, string filePath)
    {
        string fullFilepath = filePath + phfFrameData.Name + ".phf";

        string data = GeneratePHFFromPHFScriptableObject(phfFrameData, controllerToVibrate);

        Debug.Log(data);
        File.WriteAllText(fullFilepath, data);
    }

    static string GeneratePHFFromPHFScriptableObject(PHFCurveScriptableObject phfFrameData, ControllerVibrateType controllerToVibrate)
    {
        List<PHFFrame> phfPatternData_L;
        List<PHFFrame> phfPatternData_R;

        switch(controllerToVibrate)
        {
            case ControllerVibrateType.Left:
                phfPatternData_L = phfFrameData.buildFrameData();
                phfPatternData_R = phfFrameData.buildEmptyFrameData();
                break;
            case ControllerVibrateType.Right:
                phfPatternData_L = phfFrameData.buildEmptyFrameData();
                phfPatternData_R = phfFrameData.buildFrameData();
                break;
            default:
                phfPatternData_L = phfFrameData.buildFrameData();
                phfPatternData_R = phfFrameData.buildFrameData();
                break;
            
        }

        PHFBase phfBlueprint = new PHFBase(phfFrameData.Name, phfPatternData_L, phfPatternData_R);

        string phfAsJsonString = JsonConvert.SerializeObject(phfBlueprint);
        return phfAsJsonString;
    }
}

public class PHFBase
{
    public string Name;
    public string phfVersion = "V1.0";
    public int frameDuration = 20;
    public List<PHFFrame> patternData_L;
    public List<PHFFrame> patternData_R;

    public PHFBase(string name, List<PHFFrame> patternData_L, List<PHFFrame> patternData_R)
    {
        this.Name = name;
        this.patternData_L = patternData_L;
        this.patternData_R = patternData_R;
    }

}

public class PHFFrame
{
    public int frameseq;
    public int play = 2;
    public int frequency;
    public int loop = 1;
    public float gain;

    public PHFFrame(int frameseq, int frequency, float gain)
    {
        this.frameseq = frameseq;
        this.frequency = frequency;
        this.gain = gain;
    }

    public void IsFirstFrame()
    {
        this.play = 1;
    }

    public void IsLastFrame()
    {
        this.play = 0;
    }

}
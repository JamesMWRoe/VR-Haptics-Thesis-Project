using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Unity.XR.PXR;
using UnityEngine.UI;

public class HapticStream : MonoBehaviour
{

    public TextAsset PHFLeft;
    public TextAsset PHFRight;
    private PxrPhfParamsNum phfParamsNum = new PxrPhfParamsNum();
    private PxrPhfParams[] phfListleft;
    private PxrPhfParams[] phfListright;
    private PxrPhfFile phfFile;
    private bool start = false;
    private float timeLoop = 0;
    public int x = 0;
    VibrateInfo info = new VibrateInfo();

    private int id;

    public Text SpeedNum;

    public Text TextName;

    // Start is called before the first frame update
    void Start()
    {
        info.slot = 3;
        info.reversal = 0;
        info.amp = 1.5f;
        phfParamsNum.phfParams = new PxrPhfParams[50];
        ParseJson();
    }

    // Send a maximum of 25 frames of left and right controller vibration data at a certain interval, and the vibration data sent should appear in pairs, i.e., one left, one right, one left, and one right in the array
    // 
    void Update()
    {
        if (start)
        {
            timeLoop += Time.deltaTime;
            if (timeLoop >= 1.0f)
            {
                x += 1;
                if (x * 25 < phfListleft.Length)
                {
                    for (int i = (x - 1) * 25; i < x * 25; i++)
                    {
                        phfParamsNum.phfParams[(i % 25) * 2] = phfListleft[i];
                        phfParamsNum.phfParams[((i % 25) * 2) + 1] = phfListright[i];
                    }
                    PXR_Input.WriteHapticStream(id, ref phfParamsNum, 50);
                }
                else if ((phfListleft.Length - (x - 1) * 25) > 0)
                {
                    for (int i = (x - 1) * 25; i < phfListleft.Length; i++)
                    {
                        phfParamsNum.phfParams[(i % 25) * 2] = phfListleft[i];
                        phfParamsNum.phfParams[((i % 25) * 2) + 1] = phfListright[i];
                    }
                    PXR_Input.WriteHapticStream(id, ref phfParamsNum, (uint)(phfListleft.Length - (x - 1) * 25));
                }
                else
                {
                    start = false;
                }
            }
        }
    }
    
    // Parse the PHF data and store it in the corresponding struct. The SDK provides the basic ParseJson() method, or you can parse it yourself
    void ParseJson()
    {
        phfFile = PXR_Input.AnalysisHappticStreamPHF(PHFLeft);
        phfListleft = phfFile.patternData_L;
        phfListright = phfFile.patternData_R;
        Debug.Log("parsed phf. The length of left controller pattern data is: " + phfListleft.Length);
    }

    

    // Create a haptic stream and start the transmission of a specified stream
    public void StartStream() {
        PXR_Input.CreateHapticStream("V1.0", 20, ref info, 1.0f, ref id);
        PXR_Input.StartHappticStream(id);
        x = 0;
        start = true;
    }
    
    // Stop the transmission of a specified haptic stream
    public void StopStream() {
        PXR_Input.StopHappticStream(id);
    }
    
    // Remove a specified haptic stream
    public void RemoveStream() {
        PXR_Input.RemoveHappticStream(id);
    }

    // Set a transmission speed for a specified haptic stream
    public void setspeed() {
        PXR_Input.SetHapticStreamSpeed(id, float.Parse(SpeedNum.text));
    }
    
    // Get the transmission speed of a specified haptic stream
    public void getspeed() {
        float speed = 0;
        PXR_Input.GetHapticStreamSpeed(id, ref speed);
        TextName.text = speed.ToString();
    }
    
    // Get the No. of the frame that the controllers play with, which is for detecting the time delay between sound and vibration
    public void getCurrentFrameSequence() {
        UInt64 xxx = 0;
        PXR_Input.GetHapticStreamCurrentFrameSequence(id, ref xxx);
        TextName.text = xxx.ToString();
    }
}
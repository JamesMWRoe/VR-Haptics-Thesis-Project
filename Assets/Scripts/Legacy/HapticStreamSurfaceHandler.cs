using System;
using UnityEngine;
using Unity.XR.PXR;

public class OldHapticStreamSurfaceHandler : MonoBehaviour
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

    [SerializeField] PHFCurveScriptableObject phfCurve;
    HapticInteractable hapticInteractableInfo;

    [SerializeField] DebugLogger logger;

    private float timer;
    private float streamTimeIntervalInSeconds = 0.2f;
    private int streamFrameDurationInMilliSeconds = 20;
    private uint numberOfFramesToWrite;

    private int currentFrame;
    float currentPositionOnCurve;

    private PxrPhfParamsNum newHapticStreamFrames = new PxrPhfParamsNum();

    int startFrame;

    Rigidbody rigidBodyOfTheController;

    private int streamID;

    // Start is called before the first frame update
    void Start()
    {
        info.slot = 3;
        info.reversal = 0;
        info.amp = 1.5f;
        numberOfFramesToWrite = (uint)(1000*streamTimeIntervalInSeconds) / (uint)streamFrameDurationInMilliSeconds;
        Debug.Log("number of frames to write per time interval: " + numberOfFramesToWrite);
        logger.Log("number of frames to write per time interval: " + numberOfFramesToWrite);
        newHapticStreamFrames.phfParams = new PxrPhfParams[2 * numberOfFramesToWrite];
        phfParamsNum.phfParams = new PxrPhfParams[50];
        ParseJson();
    }

    // Send a maximum of 25 frames of left and right controller vibration data at a certain interval, and the vibration data sent should appear in pairs, i.e., one left, one right, one left, and one right in the array
    // 
    void Update()
    {
        if (start)
        {
            Debug.Log("haptic speed: " + hapticInteractableInfo.GetHapticSpeed(rigidBodyOfTheController.velocity));
            logger.Log("haptic speed: " + hapticInteractableInfo.GetHapticSpeed(rigidBodyOfTheController.velocity));

            HapticStreamUpdate();
            // if (rigidBodyOfTheController.velocity.z > 0.1f)
            // {
            //     x += 1;
            //     if ((x*25 + startFrame) < phfListleft.Length)
            //     {
            //         for (int i = (x - 1) * 25; i < x * 25; i++)
            //         {
            //             PxrPhfParams frameToWrite = phfListleft[i+startFrame];
            //             frameToWrite.frameseq = (ulong)i;

            //             phfParamsNum.phfParams[(i % 25) * 2] = frameToWrite;
            //             phfParamsNum.phfParams[((i % 25) * 2) + 1] = frameToWrite;
            //         }
            //         PXR_Input.WriteHapticStream(streamID, ref phfParamsNum, 50);
            //     }
            //     else if (((x - 1)*25 + startFrame) < phfListleft.Length)
            //     {
            //         for (int i = (x - 1) * 25; i+startFrame < phfListleft.Length; i++)
            //         {
            //             PxrPhfParams frameToWrite = phfListleft[i+startFrame];
            //             frameToWrite.frameseq = (ulong)i;
                        
            //             phfParamsNum.phfParams[(i % 25) * 2] = phfListleft[i+startFrame];
            //             phfParamsNum.phfParams[((i % 25) * 2) + 1] = phfListright[i+startFrame];
            //         }
            //         PXR_Input.WriteHapticStream(streamID, ref phfParamsNum, (uint)(phfListleft.Length - (x - 1) * 25));
            //     }
            //     else
            //     {
            //         start = false;
            //     }
            // }
        }
    }

    void HapticStreamUpdate()
    {
        timer -= Time.deltaTime;

        if (timer > 0)
        {   return;   }

        float distanceTraveledOnCurveThisInterval = hapticInteractableInfo.GetHapticSpeed(rigidBodyOfTheController.velocity)*streamTimeIntervalInSeconds;
        float distanceBetweenFramesOnCurveThisInterval = distanceTraveledOnCurveThisInterval / (float)numberOfFramesToWrite;

        PxrPhfParams[] patternData = new PxrPhfParams[numberOfFramesToWrite];

        for (int i = 0; i < numberOfFramesToWrite; i++)
        {
            patternData[i] = CreateHapticFrame(currentFrame, currentPositionOnCurve + (float)i*distanceBetweenFramesOnCurveThisInterval);

            Debug.Log("hapticFrame: " + patternData[i].frameseq);
            Debug.Log("frequency: " + patternData[i].frequency);
            Debug.Log("gain: " + patternData[i].gain);
            Debug.Log("play: " + patternData[i].play);
            Debug.Log("loop: " + patternData[i].loop);

            logger.Log("hapticFrame: " + patternData[i].frameseq);
            logger.Log("frequency: " + patternData[i].frequency);
            logger.Log("gain: " + patternData[i].gain);
            logger.Log("play: " + patternData[i].play);
            logger.Log("loop: " + patternData[i].loop);

            currentFrame++;
        }
        for (int i = 0; i < numberOfFramesToWrite; i++)
        {
            newHapticStreamFrames.phfParams[2*i] = patternData[i];
            newHapticStreamFrames.phfParams[2*i + 1] = patternData[i];
        }

        logger.Log("writing to haptic stream: " + streamID + " produced: " + PXR_Input.WriteHapticStream(streamID, ref newHapticStreamFrames, numberOfFramesToWrite));

        Debug.Log("Wrote to haptic stream. Current Frame is: " + currentFrame);
        Debug.Log("Assumed Position is : " + currentPositionOnCurve);

        logger.Log("Wrote to haptic stream. Current Frame is: " + currentFrame);
        logger.Log("Assumed Position is : " + currentPositionOnCurve);

        currentPositionOnCurve += numberOfFramesToWrite*distanceBetweenFramesOnCurveThisInterval;

        Debug.Log("Assumed Position in 0.2s: " + currentPositionOnCurve);
        logger.Log("Assumed Position in 0.2s: " + currentPositionOnCurve);

        timer = streamTimeIntervalInSeconds;

    }

    PxrPhfParams CreateHapticFrame(int frameNumber, float positionOnCurve)
    {
        PxrPhfParams hapticFrame = new PxrPhfParams();
        hapticFrame.frameseq = (ulong) frameNumber;
        hapticFrame.gain = phfCurve.gainCurve.Evaluate(positionOnCurve);
        hapticFrame.frequency = (ushort)phfCurve.frequencyCurve.Evaluate(positionOnCurve);
        if (frameNumber == 0)
        {   hapticFrame.play = 1;   }
        else
        {   hapticFrame.play = 2;   }
        
        hapticFrame.loop = 1;

        

        return hapticFrame;
    }

    public void StartHapticStream(HapticInteractable hapticInteractableInfo, Vector3 contactPoint, Rigidbody rb)
    {
        this.hapticInteractableInfo = hapticInteractableInfo;
        int lengthOfPHF = phfListleft.Length;

        float hapticContactValue = hapticInteractableInfo.GetHapticValue(contactPoint);
        currentPositionOnCurve = hapticContactValue;

        Debug.Log("Started at relative point: " + hapticContactValue);
        logger.Log("Started at relative point: " + hapticContactValue);

        PXR_Input.CreateHapticStream("V1.0", 20, ref info, 1.0f, ref streamID);
        PXR_Input.StartHappticStream(streamID);
        x = 0;
        rigidBodyOfTheController = rb;
        timer = 0;
        currentFrame = 0;

        firstFrameToWrite = 0;
        lastFrameToWrite = 25;

        start = true;
    }

    public void StopHapticStream()
    {
        PXR_Input.StopHappticStream(streamID);
        PXR_Input.RemoveHappticStream(streamID);
        rigidBodyOfTheController = null;
        start = false;
    }
    
    // Parse the PHF data and store it in the corresponding struct. The SDK provides the basic ParseJson() method, or you can parse it yourself
    void ParseJson()
    {
        phfFile = PXR_Input.AnalysisHappticStreamPHF(PHFLeft);
        phfListleft = phfFile.patternData_L;
        phfListright = phfFile.patternData_R;
        Debug.Log("parsed phf. The length of left controller pattern data is: " + phfListleft.Length);
    }
    
    // Remove a specified haptic stream
    public void RemoveStream() {
        PXR_Input.RemoveHappticStream(streamID);
    }
    
    // Get the No. of the frame that the controllers play with, which is for detecting the time delay between sound and vibration
    public void getCurrentFrameSequence() {
        UInt64 xxx = 0;
        PXR_Input.GetHapticStreamCurrentFrameSequence(streamID, ref xxx);
    }


    private PxrPhfParams[] phfLeftPatternData;

    private bool isStreamPlaying = false;

    int firstFrameToWrite;
    int lastFrameToWrite;

    private PxrPhfParamsNum framesToWriteToStream;

    void WriteToHapticStream()
    {
        if (!isStreamPlaying)
        {   return;   }

        Debug.Log("Writing to haptic stream");

        if (lastFrameToWrite < phfLeftPatternData.Length)
        {
            PxrPhfParams[] patternData = new PxrPhfParams[25];

            for (int i = 0; i < 25; i++)
            {
                if (firstFrameToWrite == 0)
                {   patternData[i].play = 1;   }
                else
                {   patternData[i].play = 2;   }

                patternData[i].frameseq = (ulong)i;
                patternData[i].frequency = (ushort) phfCurve.frequencyCurve.Evaluate(i/25);
                patternData[i].gain = phfCurve.gainCurve.Evaluate(((float)i)/((float)25));
                patternData[i].loop = 1;

                Debug.Log("frameseq: " + patternData[i].frameseq + " - frequency: " + patternData[i].frequency + " - gain: " + patternData[i].gain);
            }
            for (int i = firstFrameToWrite; i < lastFrameToWrite; i++)
            {
                framesToWriteToStream.phfParams[(i % 25) * 2] = patternData[i%25];
                framesToWriteToStream.phfParams[((i % 25) * 2) + 1] = patternData[i%25];
            }
            PXR_Input.WriteHapticStream(streamID, ref framesToWriteToStream, 50);
        }
        else if (firstFrameToWrite < phfLeftPatternData.Length)
        {
            PxrPhfParams[] patternData = new PxrPhfParams[phfLeftPatternData.Length - firstFrameToWrite];
            for (int i = 0; i < (phfLeftPatternData.Length - firstFrameToWrite)/2; i++)
            {
                
                
                patternData[i].play = 2;

                patternData[i].frameseq = (ulong)i;
                patternData[i].frequency = (ushort) phfCurve.frequencyCurve.Evaluate(i/25);
                patternData[i].gain = phfCurve.gainCurve.Evaluate(((float)i)/((float)25));
                patternData[i].loop = 1;

                Debug.Log("frameseq: " + patternData[i].frameseq + " - frequency: " + patternData[i].frequency + " - gain: " + patternData[i].gain);
            }

            for (int i = firstFrameToWrite; i < phfLeftPatternData.Length; i++)
            {
                framesToWriteToStream.phfParams[(i % 25) * 2] = patternData[i%25];
                framesToWriteToStream.phfParams[((i % 25) * 2) + 1] = patternData[i%25];
            }
            for (int i = 0; i < (lastFrameToWrite - phfLeftPatternData.Length); i++)
            {
                framesToWriteToStream.phfParams[((phfLeftPatternData.Length + i) % 25) * 2] = patternData[i%25];
                framesToWriteToStream.phfParams[(((phfLeftPatternData.Length + i) % 25) * 2) + 1] = patternData[i%25];
            }
            PXR_Input.WriteHapticStream(streamID, ref framesToWriteToStream, 50);

            firstFrameToWrite = lastFrameToWrite - phfLeftPatternData.Length;
            lastFrameToWrite = firstFrameToWrite + 25;
        }
        else
        {
            firstFrameToWrite = 0;
            lastFrameToWrite = 25;
            return;
        }
        firstFrameToWrite += 25;
        lastFrameToWrite += 25;
    }
}

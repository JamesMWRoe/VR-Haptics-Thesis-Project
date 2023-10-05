using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.InputSystem;

public class XRHapticStreamTest : MonoBehaviour
{

    [SerializeField] InputActionProperty leftTrigger;
    [SerializeField] InputActionProperty leftGrip;
    [SerializeField] InputActionProperty leftControllerPosition;

    [SerializeField]DebugLogger logger;

    float triggerValue;
    float gripValue;

    Vector3 controllerCurrentPositionVector;
    Vector3 controllerPreviousPositionVector;

    Vector3 controllerVelocity;

    private PxrPhfFile phfFileInfo;
    private PxrPhfParamsNum framesToWriteToStream;
    private PxrPhfParams[] phfLeftPatternData;
    private PxrPhfParams[] phfRightPatternData;

    private int streamID;
    private VibrateInfo vibrateInfo;
    private ulong currentFrame;

    private bool isStreamPlaying = false;
    private bool doesStreamExist = false;

    int firstFrameToWrite;
    int lastFrameToWrite;

    [SerializeField] TextAsset phfFile;
    [SerializeField] PHFCurveScriptableObject phfCurve;

    // Start is called before the first frame update
    void Awake()
    {
        vibrateInfo.slot = 1;
        vibrateInfo.reversal = 0;
        vibrateInfo.amp = 1;
        framesToWriteToStream.phfParams = new PxrPhfParams[50];
        //ParsePHF(phfFile);
        //ParsePHFCurve(100, phfCurve);
    }

    // Update is called once per frame
    void Update()
    {
        gripValue = leftGrip.action.ReadValue<float>();
        triggerValue = leftTrigger.action.ReadValue<float>();

        controllerPreviousPositionVector = controllerCurrentPositionVector;
        controllerCurrentPositionVector = leftControllerPosition.action.ReadValue<Vector3>();

        controllerVelocity = (controllerCurrentPositionVector - controllerPreviousPositionVector) / Time.deltaTime;

        if(gripValue > 0 && !doesStreamExist)
        {
            CreateHapticStream();
        }
        
        if(gripValue == 0 && doesStreamExist)
        {
            RemoveHapticStream();
        }

        if(doesStreamExist && triggerValue > 0 && !isStreamPlaying)
        {
            StartHapticStream();
        }

        if(doesStreamExist && triggerValue == 0 && isStreamPlaying)
        {
            StopHapticStream();
        }

        if(isStreamPlaying)
        {
            SetHapticStreamSpeed(controllerVelocity.magnitude);
            Debug.Log("stream current speed is: " + controllerVelocity.magnitude);
        }

        WriteToHapticStream();

        
    }

    void ParsePHF(TextAsset phfFile)
    {
        phfFileInfo = PXR_Input.AnalysisHappticStreamPHF(phfFile);
        phfLeftPatternData = phfFileInfo.patternData_L;
        phfRightPatternData = phfFileInfo.patternData_R;
    }

    void ParsePHFCurve(int numberOfFrames, PHFCurveScriptableObject phfCurve)
    {
        PxrPhfParams[] patternData = new PxrPhfParams[numberOfFrames];

        for (int i = 0; i < numberOfFrames; i++)
        {
            if (i == 0)
            {   patternData[i].play = 1;   }
            else
            {   patternData[i].play = 2;   }

            patternData[i].frameseq = (ulong)i;
            patternData[i].frequency = (ushort) phfCurve.frequencyCurve.Evaluate(i/numberOfFrames);
            patternData[i].gain = phfCurve.gainCurve.Evaluate(((float)i)/((float)numberOfFrames));
            patternData[i].loop = 1;

        }
        phfLeftPatternData = patternData;
        phfRightPatternData = patternData;
    }

    void CreateHapticStream()
    {
        PXR_Input.CreateHapticStream("V1.0", 20, ref vibrateInfo, 1.0f, ref streamID);

        firstFrameToWrite = 0;
        lastFrameToWrite = 25;

        doesStreamExist = true;

        Debug.Log("Created haptic stream");
    }

    void RemoveHapticStream()
    {
        PXR_Input.RemoveHappticStream(streamID);
        doesStreamExist = false;

        Debug.Log("Removed haptic stream");
    }

    void StartHapticStream()
    {
        PXR_Input.StartHappticStream(streamID);
        isStreamPlaying = true;

        Debug.Log("Started haptic stream");
    }

    void StopHapticStream()
    {
        PXR_Input.StopHappticStream(streamID);
        isStreamPlaying = false;

        Debug.Log("Stopped haptic stream");
    }

    void SetHapticStreamSpeed(float newStreamSpeed)
    {
        PXR_Input.SetHapticStreamSpeed(streamID, newStreamSpeed);
    }

    

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
            logger.Log("Writing to haptic stream: " + streamID + "resulted in result: " + PXR_Input.WriteHapticStream(streamID, ref framesToWriteToStream, 50));
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
            logger.Log("Writing to haptic stream: " + streamID + "resulted in result: " + PXR_Input.WriteHapticStream(streamID, ref framesToWriteToStream, 50));

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

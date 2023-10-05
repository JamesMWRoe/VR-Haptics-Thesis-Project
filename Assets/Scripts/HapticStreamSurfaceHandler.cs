using UnityEngine;
using Unity.XR.PXR;

public class HapticStreamSurfaceHandler : MonoBehaviour
{

    private PxrPhfParamsNum phfCurrentPacketData = new PxrPhfParamsNum();
    private bool start = false;
    private float timeLoop = 0;

    enum HandType { Left, Right }
    [SerializeField] HandType handScriptAttachedTo;

    private VibrateInfo info = new VibrateInfo();

    private int streamID;

    [SerializeField] PHFCurveScriptableObject phfCurve;

    private Rigidbody interactorRigidBody;
    private HapticInteractable currentHapticInteractable;

    private int currentFrame;

    private int frameTimeInMilliSeconds = 5;
    private float packetTimeInSeconds = 0.125f;
    private int framesPerPacket;

    private float currentCurvePosition;
    private float currentCurveVelocity;

    void Awake()
    {
        info.amp = 1.5f;
        info.reversal = 0;
        SelectControllerToVibrate();
        

        int packetTimeInMilliSeconds = (int)(packetTimeInSeconds * 1000);
        framesPerPacket = packetTimeInMilliSeconds / frameTimeInMilliSeconds;

        phfCurrentPacketData.phfParams = new PxrPhfParams[2*framesPerPacket];
    }

    void SelectControllerToVibrate()
    {
        if(handScriptAttachedTo == HandType.Left)
        {
            info.slot = 1;
        }
        else
        {
            info.slot = 2;
        }
    }

    public void ChangeHapticCurve(PHFCurveScriptableObject newCurve)
    {
        this.phfCurve = newCurve;
    }

    public void StartHapticStream(Vector3 contactPoint, HapticInteractable hapticInteractable, Rigidbody interactorRigidBody)
    {
        PXR_Input.CreateHapticStream("V1.0", (uint)frameTimeInMilliSeconds, ref info, 1.0f, ref streamID);
        PXR_Input.StartHappticStream(streamID);

        this.interactorRigidBody = interactorRigidBody;

        currentCurvePosition = hapticInteractable.GetHapticValue(contactPoint);
        currentHapticInteractable = hapticInteractable;

        currentFrame = 0;
        start = true;
    }
    
    public void StopHapticStream() 
    {
        PXR_Input.StopHappticStream(streamID);
        PXR_Input.RemoveHappticStream(streamID);

        start = false;
    }

    void Update()
    {
        if (!start)
        {   return;   }

        timeLoop -= Time.deltaTime;
        if (timeLoop > 0)
        {   return;   }

        currentCurveVelocity = currentHapticInteractable.GetHapticSpeed(interactorRigidBody.velocity);
        float startingCurveValue = currentCurvePosition;
        float totalDistanceOfCurveTraversed = currentCurveVelocity * packetTimeInSeconds;
        float distanceOfCurveTraversedPerFrame = totalDistanceOfCurveTraversed / framesPerPacket;

        for (int i = 0; i < framesPerPacket; i++)
        {
            phfCurrentPacketData.phfParams[i * 2] = CreateHapticFrame(currentFrame, startingCurveValue + i*distanceOfCurveTraversedPerFrame);
            phfCurrentPacketData.phfParams[(i * 2) + 1] = CreateHapticFrame(currentFrame, startingCurveValue + i*distanceOfCurveTraversedPerFrame);

            currentFrame++;
        }
        
        PXR_Input.WriteHapticStream(streamID, ref phfCurrentPacketData, (uint)(2*framesPerPacket));

        currentCurvePosition += totalDistanceOfCurveTraversed;
        
        timeLoop = packetTimeInSeconds-0.01f;
    }

    PxrPhfParams CreateHapticFrame(int frameNumber, float positionOnCurve)
    {
        PxrPhfParams patternData;

        if (frameNumber == 0)
        {   patternData.play = 1;   }
        else
        {   patternData.play = 2;   }

        patternData.frameseq = (ulong)frameNumber;
        patternData.frequency = (ushort) phfCurve.frequencyCurve.Evaluate(positionOnCurve);
        patternData.gain = phfCurve.gainCurve.Evaluate(positionOnCurve);
        patternData.loop = 1;

        return patternData;
    }

    PxrPhfParams CreateEmptyHapticFrame(int frameNumber)
    {
        PxrPhfParams patternData;

        if (frameNumber == 0)
        {   patternData.play = 1;   }
        else
        {   patternData.play = 2;   }

        patternData.frameseq = (ulong)frameNumber;
        patternData.frequency = 0;
        patternData.gain = 0;
        patternData.loop = 1;

        return patternData;
    }
}

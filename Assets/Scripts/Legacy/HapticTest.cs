using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;
using UnityEngine.InputSystem;

public class HapticTest : MonoBehaviour
{

    [SerializeField] AudioClip hapticClip;
    int hapticID;

    bool currentlyPlayingClip;

    [SerializeField] InputActionProperty triggerAction;

    void Update()
    {
        if (triggerAction.action.ReadValue<float>() > 0)
        {
            PlayBufferedHaptic();
        }
    }

    public void PlayBufferedHaptic()
    {
        if (!currentlyPlayingClip)
        {
            Debug.Log("Playing Clip");
            PXR_Input.SendHapticBuffer(PXR_Input.VibrateType.BothController, hapticClip, PXR_Input.ChannelFlip.No, ref hapticID);
            StartCoroutine(BufferedHapticTimeoutCoroutine());
        }
    }

    //A coroutine that acts as a timer so multiple haptics can't be sent at the same time
    IEnumerator BufferedHapticTimeoutCoroutine()
    {
        float timer = hapticClip.length + 0.1f;
        currentlyPlayingClip = true;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        currentlyPlayingClip = false;
    }
}

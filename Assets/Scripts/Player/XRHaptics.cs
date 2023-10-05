using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;
using UnityEngine.InputSystem;
using LitJson;
using System;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class XRHaptics : MonoBehaviour
{
    
    public InputActionProperty leftAnalogueStickInput;
    Vector2 leftAnalogueStickValue;

    public InputActionProperty leftControllerRotation;
    public Vector3 leftControllerAngularVelocity;
    Quaternion leftControllerCurrentFrameRotation;
    Quaternion leftControllerPreviousFrameRotation;

    [SerializeField] SliderValue[] hapticValues;

    [SerializeField] List<AudioClip> hapticClips;
    HapticAudioMenu hapticClipMenu;
    public List<string> hapticClipNames;
    AudioClip selectedHapticClip;
    bool isMenuUpdated = false;

    [SerializeField] TextAsset phfFile;

    TMP_Dropdown HapticSelectDropdown;

    float hapticAmplitude;
    int hapticFrequency;

    bool isBufferedAudioOn = true;
    bool isBufferedHapticsOn = true;

    [SerializeField] AudioClip clip1;
    [SerializeField] AudioClip clip2;

    int hapticID1;
    int hapticID2;
    int phfHapticId;

    void Awake()
    {
        foreach (AudioClip hapticClip in hapticClips)
        {
            hapticClipNames.Add(hapticClip.name);
        }

        hapticClipMenu = new HapticAudioMenu(hapticClips);
        selectedHapticClip = hapticClipMenu.GetCurrentlySelectedClip();

    }

    void Update()
    {

        UpdateHapticValues();
        leftAnalogueStickValue = leftAnalogueStickInput.action.ReadValue<Vector2>();
        //Debug.Log("left analogue stick value: " + leftAnalogueStickValue);
        HapticMenuUpdate();

        leftControllerPreviousFrameRotation = leftControllerCurrentFrameRotation;
        leftControllerCurrentFrameRotation = leftControllerRotation.action.ReadValue<Quaternion>();
    }

    public void InvertBufferedAudioBool()
    {
        isBufferedAudioOn = !isBufferedAudioOn;
    }

    public void InvertBufferedHapticBool()
    {
        isBufferedHapticsOn = !isBufferedHapticsOn;
    }

    public void SetSelectedHapticClip(int indexOfClip)
    {
        hapticClipMenu.SelectHapticClipByIndex(indexOfClip);
        selectedHapticClip = hapticClipMenu.GetCurrentlySelectedClip();
    }

    public int GetSelectedHapticClipIndex()
    {
        return hapticClipMenu.GetIndex();
    }

    void UpdateHapticValues()
    {
        hapticAmplitude = hapticValues[0].heldValue;
        hapticFrequency = ((int)hapticValues[1].heldValue);
    }

    void HapticMenuUpdate()
    {
        if (MathF.Abs(leftAnalogueStickValue.x) < 0.5f)
        {  
            isMenuUpdated = false;
            return;
        }
        if (isMenuUpdated)
        {   return;   }
        if (leftAnalogueStickValue.x >= 0.5f)
        {
            Debug.Log("selectedHapticClip Next Clip");
            isMenuUpdated = true;
            hapticClipMenu.SelectNextClip();
            selectedHapticClip = hapticClipMenu.GetCurrentlySelectedClip();
        }
        if (leftAnalogueStickValue.x <= -0.5f)
        {
            isMenuUpdated = true;
            hapticClipMenu.SelectPreviousClip();
            selectedHapticClip = hapticClipMenu.GetCurrentlySelectedClip();
        }
    }

    public void PlayUnbufferedHaptic()
    {
        Debug.Log("unbuffered haptics played");
        Debug.Log("amplitude" + hapticAmplitude);
        Debug.Log("Frequency" + hapticFrequency);
        PXR_Input.SendHapticImpulse(PXR_Input.VibrateType.BothController, hapticAmplitude, 500, hapticFrequency);
    }

    public void PlayBufferedHaptic()
    {
        Debug.Log("Sound effect played: " + selectedHapticClip.name);
        if (isBufferedAudioOn)
        {
            AudioSource hapticClipSource = gameObject.AddComponent<AudioSource>();
            hapticClipSource.clip = selectedHapticClip;
            hapticClipSource.Play();
            StartCoroutine(DestroyAudioSourceOnClipCompleteCoroutine(hapticClipSource));
        }
        
        if (isBufferedHapticsOn)
        {
            int idOfHapticClip = CreateBufferedHapticFromAudioClip(selectedHapticClip);
            PlayBufferedHapticFromID(idOfHapticClip);
        }
    }

    public void SetHapticID1()
    {
        hapticID1 = CreateBufferedHapticFromAudioClip(clip1);
    }

    public void SetHapticID2()
    {
        hapticID2 = CreateBufferedHapticFromAudioClip(clip2);
    }

    int CreateBufferedHapticFromAudioClip(AudioClip clip)
    {
        int hapticID = 0;
        PXR_Input.SendHapticBuffer(PXR_Input.VibrateType.BothController, clip, PXR_Input.ChannelFlip.No, ref hapticID, PXR_Input.CacheType.CacheNoVibrate);
        return hapticID;
    }

    public void PlayBufferedHapticFromID(int id)
    {   PXR_Input.StartHapticBuffer(id);   }

    void StopBufferedHapticFromID(int id)
    {   PXR_Input.StopHapticBuffer(id);   }

    public void PlayPHFBufferedHaptic1()
    {
        PXR_Input.StartHapticBuffer(hapticID1);
        Debug.Log("haptic id for haptic 1: "+ hapticID1);
    }

    public void PlayPHFBufferedHaptic2()
    {
        PXR_Input.StartHapticBuffer(hapticID2);
        Debug.Log("haptic id for haptic 2: "+ hapticID2);
    }

    public void PlayPHFBufferedHapti()
    {
        Debug.Log("phfFile played");
        PXR_Input.SendHapticBuffer(PXR_Input.VibrateType.BothController, phfFile, PXR_Input.ChannelFlip.No, 1.0f, ref phfHapticId);
    }
    
    IEnumerator DestroyAudioSourceOnClipCompleteCoroutine(AudioSource audioSourceToBeDestroyed)
    {
        while (audioSourceToBeDestroyed.isPlaying)
        {
            yield return null;
        }

        Destroy(audioSourceToBeDestroyed);
    }


    void SetControllerAngularVelocity()
    {

    }
}

public class HapticAudioMenu
{
    List<AudioClip> menuList;
    int currentlySelectedClipIndex;

    public HapticAudioMenu(List<AudioClip> listOfMenuItems)
    {
        this.menuList = listOfMenuItems;
        this.currentlySelectedClipIndex = 0;
    }

    public AudioClip GetCurrentlySelectedClip()
    {   return this.menuList[currentlySelectedClipIndex];   }

    public int GetIndex()
    {   return this.currentlySelectedClipIndex;   }

    public void SelectNextClip()
    {
        currentlySelectedClipIndex++;
        if (currentlySelectedClipIndex >= menuList.Count)
        {   currentlySelectedClipIndex -= menuList.Count;   }
    }

    public void SelectPreviousClip()
    {
        currentlySelectedClipIndex--;
        if (currentlySelectedClipIndex < 0)
        {   currentlySelectedClipIndex = menuList.Count - currentlySelectedClipIndex;   }
    }

    public void SelectHapticClipByIndex(int i)
    {
        currentlySelectedClipIndex = i;
        if (currentlySelectedClipIndex < 0)
        {   currentlySelectedClipIndex = menuList.Count - currentlySelectedClipIndex;   }
        if (currentlySelectedClipIndex >= menuList.Count)
        {   currentlySelectedClipIndex -= menuList.Count;   }
    }
}
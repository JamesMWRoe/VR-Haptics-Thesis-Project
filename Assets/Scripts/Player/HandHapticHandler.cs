using UnityEngine;
using Unity.XR.PXR;

public class HandHapticHandler : MonoBehaviour
{

    enum HandType { Left, Right }

    [SerializeField] HandType handScriptAttachedTo;
    PXR_Input.VibrateType controllerToVibrate;

    bool isContacting = false;
    bool isSlideClipPlaying = false;
    bool isContactClipPlaying = false;
    bool isContactHapticsDisabled = false;

    bool isInteracting = false;

    float clipTimer = 0;
    
    [SerializeField] Rigidbody rb;
    float speed;

    AudioSource audioSource;

    HapticEffect interactionEffect;

    HapticEffect thudEffect;
    HapticEffect slideEffect;
    HapticEffect contactEffect;

    float hapticSlideDuration;
    float hapticContactDuration;

    int thudID;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        isContacting = false;
        isSlideClipPlaying = false;
        isContactClipPlaying = false;
        SelectControllerToVibrate();
    }

    void Update()
    {
        HandleContactBasedHaptics();
        HandleInteractionBasedHaptics();
    }

    public void StartHapticContact(HapticMaterialScriptableObject hapticMaterial, float impulseMagnitude)
    {
        if (isContacting)
        {   return;   }

        if (impulseMagnitude > 0.5f)
        {
            if (hapticMaterial.thudEffectByAudio != null)
            {
                thudEffect = new HapticEffect(hapticMaterial.thudEffectByAudio, controllerToVibrate, audioSource);
                thudEffect.PlayBufferedHaptic();
                audioSource.PlayOneShot(hapticMaterial.thudEffectByAudio);
            }
            else
            {
                thudEffect = new HapticEffect(hapticMaterial.thudEffectByPHF, controllerToVibrate);
                thudEffect.PlayBufferedHaptic();
            }
        }

        if (hapticMaterial.slideEffectByAudio != null)
        {
            slideEffect = new HapticEffect(hapticMaterial.slideEffectByAudio, controllerToVibrate, audioSource);
            hapticSlideDuration = hapticMaterial.slideEffectByAudio.length;
        }
        else
        {
            slideEffect = new HapticEffect(hapticMaterial.slideEffectByPHF, controllerToVibrate);
            hapticSlideDuration = hapticMaterial.slidePHFLengthInSeconds;
        }

        if (hapticMaterial.contactEffectByAudio != null)
        {
            contactEffect = new HapticEffect(hapticMaterial.contactEffectByAudio, controllerToVibrate, audioSource);
            hapticContactDuration = hapticMaterial.contactEffectByAudio.length;
        }
        else
        {
            contactEffect = new HapticEffect(hapticMaterial.contactEffectByPHF, controllerToVibrate);
            hapticContactDuration = hapticMaterial.contactPHFLengthInSeconds;
        }

        isContacting = true;
    }

    public void StopHapticContact()
    {
        isContacting = false;

        contactEffect.RemoveHapticEffect();
        slideEffect.RemoveHapticEffect();

        contactEffect = null;
        slideEffect = null;
        thudEffect = null;

        isSlideClipPlaying = false;
        isContactClipPlaying = false;
    }

    public void StartInteractionBasedHaptics(TextAsset hapticClip = null)
    {
        isInteracting = true;
        isContactHapticsDisabled = true;

        Debug.Log("interaction based haptics begun");

        if (hapticClip != null)
        {
            interactionEffect = new HapticEffect(hapticClip, controllerToVibrate);
            interactionEffect.PlayBufferedHaptic();
        }
    }

    public void StopInteractionBasedHaptics()
    {
        isInteracting = false;
        isContactHapticsDisabled = false;

        Debug.Log("interaction based haptics ended");

        interactionEffect.RemoveHapticEffect();
        interactionEffect = null;
    }

    void SelectControllerToVibrate()
    {
        if(handScriptAttachedTo == HandType.Left)
        {
            controllerToVibrate = PXR_Input.VibrateType.LeftController;
        }
        else
        {
            controllerToVibrate = PXR_Input.VibrateType.RightController;
        }
    }

    void HandleContactBasedHaptics()
    {
        if (isContactHapticsDisabled)
        {   return;   }

        if (!isContacting)
        {   return;   }

        speed = rb.velocity.x*rb.velocity.x + rb.velocity.z*rb.velocity.z;

        if (speed > 0.1f*0.1f)
        {
            HandleSlideEffect();
        }
        else
        {
            HandleContactEffect();
        }
    }

    void HandleInteractionBasedHaptics()
    {
        if (!isInteracting)
        {   return;   }

        Debug.Log("currently handling interaction haptics");
    }

    void HandleSlideEffect()
    {
        if(isContactClipPlaying)
        {   
            contactEffect.StopBufferedHaptic();
            isContactClipPlaying = false;
        }
            

        if (isSlideClipPlaying)
        {
            
            clipTimer -= Time.deltaTime;
            if(clipTimer < 0)
            {   isSlideClipPlaying = false;   }
            return;
        }

        slideEffect.PlayBufferedHaptic();
        isSlideClipPlaying = true;
        clipTimer = hapticSlideDuration;
    }

    void HandleContactEffect()
    {

        if(isSlideClipPlaying)
        {
            Debug.Log("Stopped playing Slide");
            slideEffect.StopBufferedHaptic();
            isSlideClipPlaying = false;
        }

        if (isContactClipPlaying)
        {
            clipTimer -= Time.deltaTime;
            if(clipTimer < 0)
            {   isContactClipPlaying = false;   }
            return;
        }

        contactEffect.PlayBufferedHaptic();
        isContactClipPlaying = true;
        Debug.Log("Started Contact Clip");
        clipTimer = hapticContactDuration;
    }

}

public class HapticEffect
{
    int hapticID = 1;
    TextAsset hapticPHF;
    PXR_Input.VibrateType controllerToVibrate;
    AudioSource audioSource;
    AudioClip audio;

    public HapticEffect(AudioClip hapticClip, PXR_Input.VibrateType controllerToVibrate, AudioSource audioSource)
    {
        PXR_Input.SendHapticBuffer(controllerToVibrate, hapticClip, PXR_Input.ChannelFlip.No, ref hapticID, PXR_Input.CacheType.CacheNoVibrate);
        this.audio = hapticClip;
        this.audioSource = audioSource;
    }

    public HapticEffect(TextAsset hapticPHF, PXR_Input.VibrateType controllerToVibrate)
    {
        this.hapticPHF = hapticPHF;
        this.controllerToVibrate = controllerToVibrate;
    }

    public void PlayBufferedHaptic()
    {
        PXR_Input.StopHapticBuffer(hapticID);
        if (hapticPHF == null)
        {   
            PXR_Input.StartHapticBuffer(hapticID);
        }
        else
        {   PXR_Input.SendHapticBuffer(controllerToVibrate, hapticPHF, PXR_Input.ChannelFlip.No, 1.0f, ref hapticID);   }
        
    }


    public void StopBufferedHaptic()
    {
        PXR_Input.StopHapticBuffer(hapticID);
        if (audio != null)
        {   audioSource.Stop();   }
    }

    public void RemoveHapticEffect()
    {
        if (hapticPHF == null)
        {   PXR_Input.StopHapticBuffer(hapticID, true);   }
        else
        {   PXR_Input.StopHapticBuffer(hapticID);   }
        Debug.Log("Contact Ended");

        if (audio != null)
        {   audioSource.Stop();   }
    }
}
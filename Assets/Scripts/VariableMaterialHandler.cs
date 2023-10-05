using UnityEngine;

public class VariableMaterialHandler : MonoBehaviour
{
    [SerializeField] HapticEffectMenu thudMenu;
    [SerializeField] HapticEffectMenu slideMenu;
    [SerializeField] HapticEffectMenu contactMenu;
    

    public HapticMaterialScriptableObject hapticMaterial;

    void Awake()
    {
        thudMenu.OnAudioEffectClicked.AddListener(SetThudEffect);
        slideMenu.OnAudioEffectClicked.AddListener(SetSlideEffect);
        contactMenu.OnAudioEffectClicked.AddListener(SetContactEffect);

        thudMenu.OnPHFEffectClicked.AddListener(SetThudEffect);
        slideMenu.OnPHFEffectClicked.AddListener(SetSlideEffect);
        contactMenu.OnPHFEffectClicked.AddListener(SetContactEffect);
    }

    public void SetThudEffect(AudioClip newEffect)
    {   
        this.hapticMaterial.thudEffectByAudio = newEffect;
        this.hapticMaterial.thudEffectByPHF = null;
    }

    public void SetSlideEffect(AudioClip newEffect)
    {   
        this.hapticMaterial.slideEffectByAudio = newEffect;
        this.hapticMaterial.slideEffectByPHF = null;
    }

    public void SetContactEffect(AudioClip newEffect)
    {   
        this.hapticMaterial.contactEffectByAudio = newEffect;
        this.hapticMaterial.contactEffectByPHF = null;
    }

    public void SetThudEffect(TextAsset newEffect)
    {   
        this.hapticMaterial.thudEffectByPHF = newEffect;
        this.hapticMaterial.thudEffectByAudio = null;
    }

    public void SetSlideEffect(TextAsset newEffect)
    {   
        this.hapticMaterial.slideEffectByPHF = newEffect;
        this.hapticMaterial.slideEffectByAudio = null;
    }

    public void SetContactEffect(TextAsset newEffect)
    {   
        this.hapticMaterial.contactEffectByPHF = newEffect;
        this.hapticMaterial.contactEffectByAudio = null;
    }

    public void SetThudEffectNull()
    {   
        this.hapticMaterial.thudEffectByPHF = null;
        this.hapticMaterial.thudEffectByAudio = null;
    }

    public void SetSlideEffectNull()
    {   
        this.hapticMaterial.slideEffectByPHF = null;
        this.hapticMaterial.slideEffectByAudio = null;
    }

    public void SetContactEffectNull()
    {   
        this.hapticMaterial.contactEffectByPHF = null;
        this.hapticMaterial.contactEffectByAudio = null;
    }
}

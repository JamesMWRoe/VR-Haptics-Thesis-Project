using UnityEngine;

[CreateAssetMenu(fileName = "HapticMaterialScriptableObject", menuName = "ScriptableObjects/HapticMaterial")]
public class HapticMaterialScriptableObject : ScriptableObject
{
    public AudioClip thudEffectByAudio;
    public AudioClip slideEffectByAudio;
    public AudioClip contactEffectByAudio;

    public TextAsset thudEffectByPHF;
    public TextAsset slideEffectByPHF;
    public TextAsset contactEffectByPHF;

    public float thudPHFLengthInSeconds;
    public float slidePHFLengthInSeconds;
    public float contactPHFLengthInSeconds;
    
}

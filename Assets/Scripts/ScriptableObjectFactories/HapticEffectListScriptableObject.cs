using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HapticEffectListScriptableObject", menuName = "ScriptableObjects/HapticEffectsList")]
public class HapticEffectListScriptableObject : ScriptableObject
{
    
    public List<AudioClip> hapticEffects;
    public List<TextAsset> phfHapticEffects;
}

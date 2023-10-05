using UnityEngine;
using TMPro;

public class DropdownLoader : MonoBehaviour
{

    TMP_Dropdown dropdown;
    [SerializeField] XRHaptics hapticScript;

    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.AddOptions(hapticScript.hapticClipNames);
    }

    void Update()
    {
        dropdown.value = hapticScript.GetSelectedHapticClipIndex();
    }
}

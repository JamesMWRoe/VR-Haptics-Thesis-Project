using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;


public class HapticStreamEffectMenu : MonoBehaviour
{
    [SerializeField] GameObject effectButton;
    
    public UnityEvent<PHFCurveScriptableObject> OnPHFCurveClicked;
    VerticalLayoutGroup effectListLayoutGroup;

    [SerializeField] PHFCurveScriptableObject[] phfCurveList;
    
    
    void Awake()
    {
        effectListLayoutGroup = this.GetComponentInChildren<VerticalLayoutGroup>();
        MenuSetup();
    }

    void MenuSetup()
    {
        for (int i=0; i<phfCurveList.Length; i++)
        {
            GameObject currentEffectButtonObject = Instantiate(effectButton, effectListLayoutGroup.transform);
            currentEffectButtonObject.GetComponentInChildren<TextMeshProUGUI>().text = phfCurveList[i].name;
            Button currentEffectButton = currentEffectButtonObject.GetComponent<Button>();
            PHFCurveScriptableObject curve = phfCurveList[i];
            currentEffectButton.onClick.AddListener(() => OnEffectClick(curve));
        }
    }

    void OnEffectClick(PHFCurveScriptableObject effect)
    {
        Debug.Log("effect clicked: " + effect.name);
        OnPHFCurveClicked.Invoke(effect);
    }    
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;


public class HapticEffectMenu : MonoBehaviour
{
    [SerializeField] GameObject effectButton;
    
    public UnityEvent<AudioClip> OnAudioEffectClicked;
    public UnityEvent<TextAsset> OnPHFEffectClicked;
    public UnityEvent OnNullEffectClicked;
    VerticalLayoutGroup effectListLayoutGroup;
    
    
    void Awake()
    {
        effectListLayoutGroup = this.GetComponentInChildren<VerticalLayoutGroup>();
        MenuSetup();
    }

    void MenuSetup()
    {
        AudioClip[] audioClipList = Resources.LoadAll<AudioClip>("HapticClips");
        TextAsset[] phfClipList = Resources.LoadAll<TextAsset>("HapticClips/PHFs");

        GameObject nullEffectButtonObject = Instantiate(effectButton, effectListLayoutGroup.transform);
        nullEffectButtonObject.GetComponentInChildren<TextMeshProUGUI>().text = "No Effect";
        Button nullEffectButton = nullEffectButtonObject.GetComponent<Button>();
        nullEffectButton.onClick.AddListener(() => OnNullEffectClick());

        for (int i=0; i<audioClipList.Length; i++)
        {
            GameObject currentEffectButtonObject = Instantiate(effectButton, effectListLayoutGroup.transform);
            currentEffectButtonObject.GetComponentInChildren<TextMeshProUGUI>().text = audioClipList[i].name;
            Button currentEffectButton = currentEffectButtonObject.GetComponent<Button>();
            AudioClip clip = audioClipList[i];
            currentEffectButton.onClick.AddListener(() => OnEffectClick(clip));
        }

        for (int i=0; i<phfClipList.Length; i++)
        {
            GameObject currentEffectButtonObject = Instantiate(effectButton, effectListLayoutGroup.transform);
            currentEffectButtonObject.GetComponentInChildren<TextMeshProUGUI>().text = phfClipList[i].name;
            Button currentEffectButton = currentEffectButtonObject.GetComponent<Button>();
            TextAsset clip = phfClipList[i];
            currentEffectButton.onClick.AddListener(() => OnEffectClick(clip));
        }   
    }

    void OnEffectClick(AudioClip effect)
    {
        Debug.Log("effect clicked: " + effect.name);
        OnAudioEffectClicked.Invoke(effect);
    }

    void OnEffectClick(TextAsset effect)
    {
        Debug.Log("effect clicked: " + effect.name);
        OnPHFEffectClicked.Invoke(effect);
    }

    void OnNullEffectClick()
    {
        OnNullEffectClicked.Invoke();
    }

    
}

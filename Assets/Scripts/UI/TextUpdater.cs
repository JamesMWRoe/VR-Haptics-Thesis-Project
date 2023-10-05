using UnityEngine;
using TMPro;

public class TextUpdater : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textGUIToBeUpdated;

    public float textValue { get; set; }

    void Update ()
    {
        Debug.Log("text value:" + textValue);
        textGUIToBeUpdated.text = textValue.ToString();
    }
}

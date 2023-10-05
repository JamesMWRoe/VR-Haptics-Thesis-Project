using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderValue : MonoBehaviour
{

    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI textGUIToBeUpdated;
    public float heldValue;

    void Update()
    {
        heldValue = slider.value;
        textGUIToBeUpdated.text = heldValue.ToString();
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugLogger : MonoBehaviour
{
    [SerializeField] GameObject logTextBox;
    VerticalLayoutGroup logLayoutGroup;

    // Start is called before the first frame update
    void Awake()
    {
        logLayoutGroup = this.GetComponentInChildren<VerticalLayoutGroup>();
    }

    public void Log(string log)
    {
        GameObject currentLogTextBoxObject = Instantiate(logTextBox, logLayoutGroup.transform);
        currentLogTextBoxObject.GetComponent<TextMeshProUGUI>().text = log;

    }
}

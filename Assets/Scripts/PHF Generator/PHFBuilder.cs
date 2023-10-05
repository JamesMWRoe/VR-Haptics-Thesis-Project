using System.Collections.Generic;
using UnityEngine;

public class PHFBuilder : MonoBehaviour
{
    public List<PHFCurveScriptableObject> phfDataList;
    void Awake()
    {
        foreach(PHFCurveScriptableObject phfData in phfDataList)
        {
            if (phfData != null)
            {   PHFGenerator.GenerateFile(phfData, PHFGenerator.ControllerVibrateType.Both, @"D:\Projects\Unity\VR Haptics Thesis Project\Assets\Scripts\PHF Generator\PHFs\");   }
        }
    }
}

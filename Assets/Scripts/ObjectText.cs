using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectText : MonoBehaviour
{
    public TMP_Text objectName;
    public TMP_Text objectInfo;
    public string myName = "Empty";
    [TextArea(1,10)] ///prva hodnota - min pocet riadkov, druha hodnota, max pocet riadkov textoveho elementu
    public string myInfo = "No info yet.";
    
    // Start is called before the first frame update
    void Start()
    {
        objectName.text = myName; //konverzia stringu na Text
        objectInfo.text = myInfo;
    }

}

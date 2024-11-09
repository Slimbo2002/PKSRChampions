using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SetName : MonoBehaviour
{
    public TMP_InputField inputField;
    public TextMeshProUGUI text;
    
    private void Start()
    {
        text.text = PlayerPrefs.GetString("Username");
    }
    public void SaveName()
    {
        PlayerPrefs.SetString("Username", inputField.text);
        

    }
}

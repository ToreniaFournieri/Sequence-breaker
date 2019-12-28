using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class TranslateText : MonoBehaviour
{
    //public Text targetText;

    public string targetString;


    private Text _targetText;
    private string _previousLanguage;

    private void Start()
    {
        _targetText = gameObject.GetComponent<Text>();
    }

    private void Update()
    {

        if (_previousLanguage != LocalizationManager.CurrentLanguage)
        {


            _targetText.text = LocalizationManager.GetTranslation(targetString);

            _previousLanguage = LocalizationManager.CurrentLanguage;
        }
    }


}

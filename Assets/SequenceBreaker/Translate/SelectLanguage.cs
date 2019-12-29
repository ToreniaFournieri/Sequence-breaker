using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class SelectLanguage : MonoBehaviour
{

    private Dropdown _dropdown;


    private void Start()
    {
        _dropdown = gameObject.GetComponent<Dropdown>();
    }

    //void OnClick()
    //{
    //}

    public void Selected()
    {
        Debug.Log(_dropdown.options[_dropdown.value].text);

        if (LocalizationManager.HasLanguage(_dropdown.options[_dropdown.value].text))
        {
            LocalizationManager.CurrentLanguage = _dropdown.options[_dropdown.value].text;
        }
    }

}

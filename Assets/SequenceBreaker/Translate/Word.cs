using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

public static class Word 
{

    public static string Get(string term)
    {
        string _term = null;
        _term= LocalizationManager.GetTranslation(term);

        if (_term != null)
        {
            _term = term;
        }

        return _term;

    }


}

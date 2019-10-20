using _00_Asset._06_I2_Localization.Localization.Scripts;
using _00_Asset._06_I2_Localization.Localization.Scripts.Manager;
using _00_Asset._06_I2_Localization.Localization.Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace _00_Asset._06_I2_Localization.Localization.Examples.Common.Scripts
{
    public class ExampleLocalizedString : MonoBehaviour
    {
        [FormerlySerializedAs("_MyLocalizedString")] public LocalizedString myLocalizedString;      // This string sets a Term in the inspector, but returns its translation

        [FormerlySerializedAs("_NormalString")] public string normalString;                    // This is regular string to see that the LocalizedString has a custom inspector, but this shows only a textField

        [FormerlySerializedAs("_StringWithTermPopup")] [TermsPopup]
        public string stringWithTermPopup;             // Example of making a normal string that show as a popup with all the terms in the inspector

        public void Start()
        {
            // LocalizedString are strings that can be set to a Term, and when getting its value, return the Term's translation

            // Basic Example of using LocalizedString in the Inspector
            // Just change the Term in the inspector, and use this to access the term translation
            Debug.Log(myLocalizedString);
            Debug.Log(LocalizationManager.GetTranslation(normalString));         // regular strings need to manually call GetTranslation()
            Debug.Log(LocalizationManager.GetTranslation(stringWithTermPopup));  // same here, given that this string just have a custom inspector



            // Example of setting the term in code to get its translation
            LocalizedString locString = "Term2";
            string translation = locString;   // returns the translation of Term2 to the current language
            Debug.Log(translation);  



            // Assigning a LocalizedString to another LocalizedString, copies the reference to its term
            LocalizedString locString1 = myLocalizedString;
            Debug.Log(locString1);




            // LocalizedString have settings to customize the result

            LocalizedString customString = "Term3";
            Debug.Log(customString);

            LocalizedString customNoRtl = "Term3";
            customNoRtl.mRtlIgnoreArabicFix = true;
            Debug.Log(customNoRtl);


            LocalizedString customString1 = "Term3";
            customString1.mRtlConvertNumbers = true;
            customString1.mRtlMaxLineLength = 20;
            Debug.Log(customString1);




            // Copying a LocalizedString also copies its settings
            LocalizedString customStringCopy = customString1;
            Debug.Log(customStringCopy);
        }
    }
}
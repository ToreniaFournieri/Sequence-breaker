using UnityEngine;

namespace _00_Asset._06_I2_Localization.Localization.Scripts.Manager
{
    public static partial class LocalizationManager
    {
        static string _mCurrentDeviceLanguage;

        public static string GetCurrentDeviceLanguage( bool force = false )
        {
            if (force || string.IsNullOrEmpty(_mCurrentDeviceLanguage))
                DetectDeviceLanguage();

            return _mCurrentDeviceLanguage;
        }

        static void DetectDeviceLanguage()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            try { 
                        AndroidJavaObject locale = new AndroidJavaClass("java/util/Locale").CallStatic<AndroidJavaObject>("getDefault");
                        mCurrentDeviceLanguage = locale.Call<string>("toString");
                        //https://stackoverflow.com/questions/4212320/get-the-current-language-in-device


                        if (!string.IsNullOrEmpty(mCurrentDeviceLanguage))
                        {
                            mCurrentDeviceLanguage = mCurrentDeviceLanguage.Replace('_', '-');
                            mCurrentDeviceLanguage = GoogleLanguages.GetLanguageName(mCurrentDeviceLanguage, true, true);
                            if (!string.IsNullOrEmpty(mCurrentDeviceLanguage))
                                return;
                        }
            }
            catch (System.Exception)
            { 
            }
            #endif

            _mCurrentDeviceLanguage = Application.systemLanguage.ToString();
            if (_mCurrentDeviceLanguage == "ChineseSimplified") _mCurrentDeviceLanguage = "Chinese (Simplified)";
            if (_mCurrentDeviceLanguage == "ChineseTraditional") _mCurrentDeviceLanguage = "Chinese (Traditional)";
        }
    }
}
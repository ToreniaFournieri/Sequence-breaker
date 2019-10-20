using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace I2.Loc
{
	public enum ETermType 
	{ 
		Text, Font, Texture, AudioClip, GameObject, Sprite, Material, Child, Mesh,
		#if NGUI
			UIAtlas, UIFont,
		#endif
		#if TK2D
			TK2dFont, TK2dCollection,
		#endif
		#if TextMeshPro
			TextMeshPFont,
		#endif
		#if SVG
			SVGAsset,
		#endif
		Object 
	}

	public enum TranslationFlag : byte
	{
		Normal = 1,
		AutoTranslated = 2,
	}


    [Serializable]
	public class TermData
	{
		[FormerlySerializedAs("Term")] public string 			term 			= string.Empty;
		[FormerlySerializedAs("TermType")] public ETermType		termType 		= ETermType.Text;
		
		#if !UNITY_EDITOR
		[NonSerialized]
		#endif
		[FormerlySerializedAs("Description")] public string 			description;
		
        [FormerlySerializedAs("Languages")] public string[]         languages = new string[0];
        [FormerlySerializedAs("Flags")] public byte[]			flags 			= new byte[0];  // flags for each translation

        [FormerlySerializedAs("Languages_Touch")] [SerializeField] private string[] languagesTouch = null;      // TO BE REMOVED IN A FUTURE RELEASE

        public string GetTranslation ( int idx, string specialization=null, bool editMode=false )
		{
            string text = languages[idx];
            if (text != null)
            {
                text = SpecializationManager.GetSpecializedText(text, specialization);
                if (!editMode)
                {
                    text = text.Replace("[i2nt]", "").Replace("[/i2nt]", "");
                }
            }
            return text;
		}

        public void SetTranslation( int idx, string translation, string specialization = null)
        {
            languages[idx] = SpecializationManager.SetSpecializedText( languages[idx], translation, specialization);
        }

        public void RemoveSpecialization(string specialization)
        {
            for (int i = 0; i < languages.Length; ++i)
                RemoveSpecialization(i, specialization);
        }


        public void RemoveSpecialization( int idx, string specialization )
        {
            var text = languages[idx];
            if (specialization == "Any" || !text.Contains("[i2s_" + specialization + "]"))
            {
                return;
            }

            var dict = SpecializationManager.GetSpecializations(text);
            dict.Remove(specialization);
            languages[idx] = SpecializationManager.SetSpecializedText(dict);
        }

        public bool IsAutoTranslated( int idx, bool isTouch )
		{
			return (flags[idx] & (byte)TranslationFlag.AutoTranslated) > 0;
		}

		public void Validate ()
		{
			int nLanguages = Mathf.Max(languages.Length, flags.Length);

			if (languages.Length != nLanguages) 		Array.Resize(ref languages, nLanguages);
			if (flags.Length!=nLanguages) 				Array.Resize(ref flags, nLanguages);

            if (languagesTouch != null)
            {
                for (int i = 0; i < Mathf.Min(languagesTouch.Length, nLanguages); ++i)
                {
                    if (string.IsNullOrEmpty(languages[i]) && !string.IsNullOrEmpty(languagesTouch[i]))
                    {
                        languages[i] = languagesTouch[i];
                        languagesTouch[i] = null;
                    }
                }
                languagesTouch = null;
            }
        }
        
		public bool IsTerm( string name, bool allowCategoryMistmatch)
		{
			if (!allowCategoryMistmatch)
				return name == term;

			return name == LanguageSourceData.GetKeyFromFullTerm (term);
		}

        public bool HasSpecializations()
        {
            for (int i = 0; i < languages.Length; ++i)
            {
                if (!string.IsNullOrEmpty(languages[i]) && languages[i].Contains("[i2s_"))
                    return true;
            }
            return false;
        }

        public List<string> GetAllSpecializations()
        {
            List<string> values = new List<string>();
            for (int i = 0; i < languages.Length; ++i)
                SpecializationManager.AppendSpecializations(languages[i], values);
            return values;
        }
    };

    public class TermsPopup : PropertyAttribute
    {
        public TermsPopup(string filter = "")
        {
            this.Filter = filter;
        }

        public string Filter { get; private set; }
    }
}
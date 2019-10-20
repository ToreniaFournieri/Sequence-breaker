using System;
using System.Collections.Generic;
using System.Linq;
using _00_Asset._06_I2_Localization.Localization.Scripts.Google;
using _00_Asset._06_I2_Localization.Localization.Scripts.LanguageSource;
using _00_Asset._06_I2_Localization.Localization.Scripts.Manager;
using _00_Asset._06_I2_Localization.Localization.Scripts.Targets;
using _00_Asset._06_I2_Localization.Localization.Scripts.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;


namespace _00_Asset._06_I2_Localization.Localization.Scripts
{
    [AddComponentMenu("I2/Localization/I2 Localize")]
    public partial class Localize : MonoBehaviour
    {
        #region Variables: Term
        public string Term
        {
            get { return mTerm; }
            set { SetTerm(value); }
        }
        public string SecondaryTerm
        {
            get { return mTermSecondary; }
            set { SetTerm(null, value); }
        }

        public string mTerm = string.Empty,           // if Target is a Label, this will be the text,  if sprite, this will be the spriteName, etc
                      mTermSecondary = string.Empty; // if Target is a Label, this will be the font Name,  if sprite, this will be the Atlas name, etc

        // This are the terms actually used (will be mTerm/mSecondaryTerm or will get them from the objects if those are missing. e.g. Labels' text and font name)
        // This are set when the component starts
        [NonSerialized] public string FinalTerm, FinalSecondaryTerm;

        public enum TermModification { DontModify, ToUpper, ToLower, ToUpperFirst, ToTitle/*, CustomRange*/}
        [FormerlySerializedAs("PrimaryTermModifier")] public TermModification primaryTermModifier = TermModification.DontModify;

        [FormerlySerializedAs("SecondaryTermModifier")] public TermModification secondaryTermModifier = TermModification.DontModify;

        [FormerlySerializedAs("TermPrefix")] public string termPrefix;
        [FormerlySerializedAs("TermSuffix")] public string termSuffix;

        [FormerlySerializedAs("LocalizeOnAwake")] public bool localizeOnAwake = true;

        string _lastLocalizedLanguage;   // Used to avoid Localizing everytime the object is Enabled

#if UNITY_EDITOR
        [FormerlySerializedAs("Source")] public ILanguageSource source;   // Source used while in the Editor to preview the Terms (can be of type LanguageSource or LanguageSourceAsset)
#endif

        #endregion

        #region Variables: Target

        [FormerlySerializedAs("IgnoreRTL")] public bool ignoreRtl = false;	// If false, no Right To Left processing will be done
		[FormerlySerializedAs("MaxCharactersInRTL")] public int  maxCharactersInRtl = 0;     // If the language is RTL, the translation will be split in lines not longer than this amount and the RTL fix will be applied per line
		[FormerlySerializedAs("IgnoreNumbersInRTL")] public bool ignoreNumbersInRtl = true; // If the language is RTL, the translation will not convert numbers (will preserve them like: e.g. 123)

		[FormerlySerializedAs("CorrectAlignmentForRTL")] public bool correctAlignmentForRtl = true;	// If true, when Right To Left language, alignment will be set to Right

        [FormerlySerializedAs("AddSpacesToJoinedLanguages")] public bool addSpacesToJoinedLanguages; // Some languages (e.g. Chinese, Japanese and Thai) don't add spaces to their words (all characters are placed toguether), making this variable true, will add spaces to all characters to allow wrapping long texts into multiple lines.
        [FormerlySerializedAs("AllowLocalizedParameters")] public bool allowLocalizedParameters=true;

        #endregion

        #region Variables: References

        [FormerlySerializedAs("TranslatedObjects")] public List<Object> translatedObjects = new List<Object>();  // For targets that reference objects (e.g. AudioSource, UITexture,etc) 
                                                                    // this keeps a reference to the possible options.
                                                                    // If the value is not the name of any of this objects then it will try to load the object from the Resources

        
        [NonSerialized] public Dictionary<string, Object> MAssetDictionary = new Dictionary<string, Object>(StringComparer.Ordinal); //This is used to overcome the issue with Unity not serializing Dictionaries

        #endregion

        #region Variable Translation Modifiers


        [FormerlySerializedAs("LocalizeEvent")] public UnityEvent localizeEvent = new UnityEvent();             // This allows scripts to modify the translations :  e.g. "Player {0} wins"  ->  "Player Red wins"	


        public static string MainTranslation, SecondaryTranslation;		// The callback should use and modify this variables
		public static string CallBackTerm, CallBackSecondaryTerm;		// during the callback, this will hold the FinalTerm and FinalSecondary  to know what terms are originating the translation
		public static Localize CurrentLocalizeComponent;				// while in the LocalizeCallBack, this points to the Localize calling the callback

		[FormerlySerializedAs("AlwaysForceLocalize")] public bool alwaysForceLocalize = false;			// Force localization when the object gets enabled (useful for callbacks and parameters that change the localization even through the language is the same as in the previous time it was localized)

        [FormerlySerializedAs("LocalizeCallBack")] [SerializeField] public EventCallback localizeCallBack = new EventCallback();    //LocalizeCallBack is deprecated. Please use LocalizeEvent instead.

        #endregion

        #region Variables: Editor Related
        [FormerlySerializedAs("mGUI_ShowReferences")] public bool mGuiShowReferences = false;
		[FormerlySerializedAs("mGUI_ShowTems")] public bool mGuiShowTems = true;
		[FormerlySerializedAs("mGUI_ShowCallback")] public bool mGuiShowCallback = false;
        #endregion

        #region Variables: Runtime (LocalizeTarget)

        public LocalizeTarget mLocalizeTarget;
        public string mLocalizeTargetName; // Used to resolve multiple targets in a prefab

        #endregion

        #region Localize

        void Awake()
		{
            #if UNITY_EDITOR
            if (UnityEditor.BuildPipeline.isBuildingPlayer)
                return;
            #endif

            UpdateAssetDictionary();
            FindTarget();

            if (localizeOnAwake)
                OnLocalize();
        }

        #if UNITY_EDITOR
        void OnValidate()
        {
            if (localizeCallBack.HasCallback())
            {
                try
                {
                    var methodInfo = UnityEvent.GetValidMethodInfo(localizeCallBack.target, localizeCallBack.methodName, new Type[0]);

                    if (methodInfo != null)
                    {
                        UnityAction methodDelegate = System.Delegate.CreateDelegate(typeof(UnityAction), localizeCallBack.target, methodInfo, false) as UnityAction;
                        if (methodDelegate != null)
                            UnityEditor.Events.UnityEventTools.AddPersistentListener(localizeEvent, methodDelegate);
                    }
                }
                catch(Exception)
                {}

                localizeCallBack.target = null;
                localizeCallBack.methodName = null;
            }
        }
        #endif

        void OnEnable()
		{
            OnLocalize ();
		}

        public bool HasCallback()
        {
            if (localizeCallBack.HasCallback())
                return true;
            return localizeEvent.GetPersistentEventCount() > 0;
        }

		public void OnLocalize( bool force = false )
		{
			if (!force && (!enabled || gameObject==null || !gameObject.activeInHierarchy))
				return;

			if (string.IsNullOrEmpty(LocalizationManager.CurrentLanguage))
				return;

			if (!alwaysForceLocalize && !force && !HasCallback() && _lastLocalizedLanguage==LocalizationManager.CurrentLanguage)
				return;
			_lastLocalizedLanguage = LocalizationManager.CurrentLanguage;

			// These are the terms actually used (will be mTerm/mSecondaryTerm or will get them from the objects if those are missing. e.g. Labels' text and font name)
			if (string.IsNullOrEmpty(FinalTerm) || string.IsNullOrEmpty(FinalSecondaryTerm))
				GetFinalTerms( out FinalTerm, out FinalSecondaryTerm );


			bool hasCallback = I2Utils.IsPlaying() && HasCallback();

			if (!hasCallback && string.IsNullOrEmpty (FinalTerm) && string.IsNullOrEmpty (FinalSecondaryTerm))
				return;

			CallBackTerm = FinalTerm;
			CallBackSecondaryTerm = FinalSecondaryTerm;
			MainTranslation = (string.IsNullOrEmpty(FinalTerm) || FinalTerm=="-") ? null : LocalizationManager.GetTranslation (FinalTerm, false);
			SecondaryTranslation = (string.IsNullOrEmpty(FinalSecondaryTerm) || FinalSecondaryTerm == "-") ? null : LocalizationManager.GetTranslation (FinalSecondaryTerm, false);

			if (!hasCallback && /*string.IsNullOrEmpty (MainTranslation)*/ string.IsNullOrEmpty(FinalTerm) && string.IsNullOrEmpty (SecondaryTranslation))
				return;

			CurrentLocalizeComponent = this;

			{
				localizeCallBack.Execute (this);  // This allows scripts to modify the translations :  e.g. "Player {0} wins"  ->  "Player Red wins"
                localizeEvent.Invoke();
                LocalizationManager.ApplyLocalizationParams (ref MainTranslation, gameObject, allowLocalizedParameters);
			}

			if (!FindTarget())
				return;
            bool applyRtl = LocalizationManager.IsRight2Left && !ignoreRtl;

            if (MainTranslation != null)
            {
                switch (primaryTermModifier)
                {
                    case TermModification.ToUpper:      MainTranslation = MainTranslation.ToUpper(); break;
                    case TermModification.ToLower:      MainTranslation = MainTranslation.ToLower(); break;
                    case TermModification.ToUpperFirst: MainTranslation = GoogleTranslation.UppercaseFirst(MainTranslation); break;
                    case TermModification.ToTitle:      MainTranslation = GoogleTranslation.TitleCase(MainTranslation); break;
                }
                if (!string.IsNullOrEmpty(termPrefix))
                    MainTranslation = applyRtl ? MainTranslation + termPrefix : termPrefix + MainTranslation;
                if (!string.IsNullOrEmpty(termSuffix))
                    MainTranslation = applyRtl ? termSuffix + MainTranslation : MainTranslation + termSuffix;

                if (addSpacesToJoinedLanguages && LocalizationManager.HasJoinedWords && !string.IsNullOrEmpty(MainTranslation))
                {
                    var sb = new System.Text.StringBuilder();
                    sb.Append(MainTranslation[0]);
                    for (int i = 1, imax = MainTranslation.Length; i < imax; ++i)
                    {
                        sb.Append(' ');
                        sb.Append(MainTranslation[i]);
                    }

                    MainTranslation = sb.ToString();
                }
                if (applyRtl && mLocalizeTarget.AllowMainTermToBeRtl() && !string.IsNullOrEmpty(MainTranslation))
                    MainTranslation = LocalizationManager.ApplyRtLfix(MainTranslation, maxCharactersInRtl, ignoreNumbersInRtl);

            }

            if (SecondaryTranslation != null)
            {
                switch (secondaryTermModifier)
                {
                    case TermModification.ToUpper:      SecondaryTranslation = SecondaryTranslation.ToUpper(); break;
                    case TermModification.ToLower:      SecondaryTranslation = SecondaryTranslation.ToLower(); break;
                    case TermModification.ToUpperFirst: SecondaryTranslation = GoogleTranslation.UppercaseFirst(SecondaryTranslation); break;
                    case TermModification.ToTitle:      SecondaryTranslation = GoogleTranslation.TitleCase(SecondaryTranslation); break;
                }
                if (applyRtl && mLocalizeTarget.AllowSecondTermToBeRtl() && !string.IsNullOrEmpty(SecondaryTranslation))
                        SecondaryTranslation = LocalizationManager.ApplyRtLfix(SecondaryTranslation);
            }

            if (LocalizationManager.HighlightLocalizedTargets)
            {
                MainTranslation = "LOC:" + FinalTerm;
            }

            mLocalizeTarget.DoLocalize( this, MainTranslation, SecondaryTranslation );

			CurrentLocalizeComponent = null;
		}

		#endregion

		#region Finding Target

		public bool FindTarget()
		{
            if (mLocalizeTarget != null && mLocalizeTarget.IsValid(this))
                return true;

            if (mLocalizeTarget!=null)
            {
                DestroyImmediate(mLocalizeTarget);
                mLocalizeTarget = null;
                mLocalizeTargetName = null;
            }

            if (!string.IsNullOrEmpty(mLocalizeTargetName))
            {
                foreach (var desc in LocalizationManager.MLocalizeTargets)
                {
                    if (mLocalizeTargetName == desc.GetTargetType().ToString())
                    {
                        if (desc.CanLocalize(this))
                            mLocalizeTarget = desc.CreateTarget(this);
                        if (mLocalizeTarget!=null)
                            return true;
                    }
                }
            }

            foreach (var desc in LocalizationManager.MLocalizeTargets)
            {
                if (!desc.CanLocalize(this))
                    continue;
                mLocalizeTarget = desc.CreateTarget(this);
                mLocalizeTargetName = desc.GetTargetType().ToString();
                if (mLocalizeTarget != null)
                    return true;
            }

			return false;
		}

		#endregion

		#region Finding Term
		
		// Returns the term that will actually be translated
		// its either the Term value in this class or the text of the label if there is no term
		public void GetFinalTerms( out string primaryTerm, out string secondaryTerm )
		{
			primaryTerm 	= string.Empty;
			secondaryTerm 	= string.Empty;

			if (!FindTarget())
				return;


			// if either the primary or secondary term is missing, get them. (e.g. from the label's text and font name)
            if (mLocalizeTarget != null)
            {
                mLocalizeTarget.GetFinalTerms(this, mTerm, mTermSecondary, out primaryTerm, out secondaryTerm);
                primaryTerm = I2Utils.GetValidTermName(primaryTerm, false);
            }

            // If there are values already set, go with those
            if (!string.IsNullOrEmpty(mTerm))
				primaryTerm = mTerm;

			if (!string.IsNullOrEmpty(mTermSecondary))
				secondaryTerm = mTermSecondary;

			if (primaryTerm != null)
				primaryTerm = primaryTerm.Trim();
			if (secondaryTerm != null)
				secondaryTerm = secondaryTerm.Trim();
		}

		public string GetMainTargetsText()
		{
			string primary = null, secondary = null;

			if (mLocalizeTarget!=null)
				mLocalizeTarget.GetFinalTerms( this, null, null, out primary, out secondary );

			return string.IsNullOrEmpty(primary) ? mTerm : primary;
		}
		
		public void SetFinalTerms( string main, string secondary, out string primaryTerm, out string secondaryTerm, bool removeNonAscii )
		{
			primaryTerm = removeNonAscii ? I2Utils.GetValidTermName(main) : main;
			secondaryTerm = secondary;
		}
		
		#endregion

		#region Misc

		public void SetTerm (string primary)
		{
			if (!string.IsNullOrEmpty(primary))
				FinalTerm = mTerm = primary;

			OnLocalize (true);
		}

		public void SetTerm(string primary, string secondary )
		{
			if (!string.IsNullOrEmpty(primary))
				FinalTerm = mTerm = primary;
			FinalSecondaryTerm = mTermSecondary = secondary;

			OnLocalize(true);
		}

		internal T GetSecondaryTranslatedObj<T>( ref string mainTranslation, ref string secondaryTranslation ) where T: Object
		{
			string newMain, newSecond;

			//--[ Allow main translation to override Secondary ]-------------------
			DeserializeTranslation(mainTranslation, out newMain, out newSecond);

			T obj = null;

			if (!string.IsNullOrEmpty(newSecond))
			{
				obj = GetObject<T>(newSecond);
				if (obj != null)
				{
					mainTranslation = newMain;
					secondaryTranslation = newSecond;
				}
			}

			if (obj == null)
				obj = GetObject<T>(secondaryTranslation);

			return obj;
		}

        public void UpdateAssetDictionary()
        {
            translatedObjects.RemoveAll(x => x == null);
            MAssetDictionary = translatedObjects.Distinct()
                                                .GroupBy(o => o.name)
                                                .ToDictionary(g => g.Key, g => g.First());
        }

        internal T GetObject<T>( string translation) where T: Object
		{
			if (string.IsNullOrEmpty (translation))
				return null;
			T obj = GetTranslatedObject<T>(translation);
			
			//if (obj==null)
			//{
				// Remove path and search by name
				//int Index = Translation.LastIndexOfAny("/\\".ToCharArray());
				//if (Index>=0)
				//{
				//	Translation = Translation.Substring(Index+1);
				//	obj = GetTranslatedObject<T>(Translation);
				//}
			//}
			return obj;
		}

		T GetTranslatedObject<T>( string translation) where T: Object
		{
			T obj = FindTranslatedObject<T>(translation);
			/*if (Obj == null) 
				return null;
			
			if ((Obj as T) != null) 
				return Obj as T;
			
			// If the found Obj is not of type T, then try finding a component inside
			if (Obj as Component != null)
				return (Obj as Component).GetComponent(typeof(T)) as T;
			
			if (Obj as GameObject != null)
				return (Obj as GameObject).GetComponent(typeof(T)) as T;
			*/
			return obj;
		}


		// translation format: "[secondary]value"   [secondary] is optional
		void DeserializeTranslation( string translation, out string value, out string secondary )
		{
			if (!string.IsNullOrEmpty(translation) && translation.Length>1 && translation[0]=='[')
			{
				int index = translation.IndexOf(']');
				if (index>0)
				{
					secondary = translation.Substring(1, index-1);
					value = translation.Substring(index+1);
					return;
				}
			}
			value = translation;
			secondary = string.Empty;
		}
		
		public T FindTranslatedObject<T>( string value) where T : Object
		{
			if (string.IsNullOrEmpty(value))
				return null;

            if (MAssetDictionary == null || MAssetDictionary.Count != translatedObjects.Count)
            {
                UpdateAssetDictionary();
            }
 
            foreach (var kvp in MAssetDictionary)
            { 
				if (kvp.Value is T && value.EndsWith(kvp.Key, StringComparison.OrdinalIgnoreCase))
				{
					// Check if the value is just the name or has a path
					if (string.Compare(value, kvp.Key, StringComparison.OrdinalIgnoreCase)==0)
						return (T) kvp.Value;

					// Check if the path matches
					//Resources.get TranslatedObjects[i].
				}
			}

			T obj = LocalizationManager.FindAsset(value) as T;
			if (obj)
				return obj;

			obj = ResourceManager.PInstance.GetAsset<T>(value);
			return obj;
		}

		public bool HasTranslatedObject( Object obj )
		{
			if (translatedObjects.Contains(obj)) 
				return true;
			return ResourceManager.PInstance.HasAsset(obj);

		}

		public void AddTranslatedObject( Object obj )
		{
            if (translatedObjects.Contains(obj))
                return;
			translatedObjects.Add(obj);
            UpdateAssetDictionary();
		}

		#endregion
	
		#region Utilities
		// This can be used to set the language when a button is clicked
		public void SetGlobalLanguage( string language )
		{
			LocalizationManager.CurrentLanguage = language;
		}

		#endregion
	}
}
using System.Collections.Generic;

namespace _00_Asset._06_I2_Localization.Localization.Scripts.LanguageSource
{
	public partial class LanguageSourceData
	{
		public static string EmptyCategory = "Default";
		public static char[] CategorySeparators = "/\\".ToCharArray();

		#region Keys
		
		public List<string> GetCategories( bool onlyMainCategory = false, List<string> categories = null )
		{
			if (categories==null)
				categories = new List<string>();
			
			foreach (TermData data in mTerms)
			{
				string sCategory = GetCategoryFromFullTerm( data.term, onlyMainCategory );
				if (!categories.Contains(sCategory))
					categories.Add(sCategory);
			}
			categories.Sort();
			return categories;
		}
		
		public static string GetKeyFromFullTerm( string fullTerm, bool onlyMainCategory = false )
		{
			int index = (onlyMainCategory ? fullTerm.IndexOfAny(CategorySeparators) : 
			             					fullTerm.LastIndexOfAny(CategorySeparators));

			return (index<0 ? fullTerm :fullTerm.Substring(index+1));
		}
		
		public static string GetCategoryFromFullTerm( string fullTerm, bool onlyMainCategory = false )
		{
			int index = (onlyMainCategory ? fullTerm.IndexOfAny(CategorySeparators) : 
			             					fullTerm.LastIndexOfAny(CategorySeparators));

			return (index<0 ? EmptyCategory : fullTerm.Substring(0, index));
		}
		
		public static void DeserializeFullTerm( string fullTerm, out string key, out string category, bool onlyMainCategory = false )
		{
			int index = (onlyMainCategory ? fullTerm.IndexOfAny(CategorySeparators) : 
			             					fullTerm.LastIndexOfAny(CategorySeparators));

			if (index<0) 
			{
				category = EmptyCategory;
				key = fullTerm;
			}
			else 
			{
				category = fullTerm.Substring(0, index);
				key = fullTerm.Substring(index+1);
			}
		}

		#endregion
		
		#region Misc
		
		#endregion

	}
}
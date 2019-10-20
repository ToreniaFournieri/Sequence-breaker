using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace _00_Asset._06_I2_Localization.Localization.Scripts
{
	public class LocalizationReader 
	{
		#region Dictionary Assets

		public static Dictionary<string,string> ReadTextAsset( TextAsset asset )
		{
			string text = Encoding.UTF8.GetString (asset.bytes, 0, asset.bytes.Length);
			text = text.Replace("\r\n", "\n");
			text = text.Replace("\r", "\n");
			System.IO.StringReader reader = new System.IO.StringReader(text);
			
			string s;
            Dictionary<string, string> dict = new Dictionary<string, string>(System.StringComparer.Ordinal);
			while ( (s=reader.ReadLine()) != null )
			{
				string key, value, category, termType, comment;
				if (!TextAsset_ReadLine(s, out key, out value, out category, out comment, out termType))
					continue;
				
				if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
					dict[key]=value;
			}
			return dict;
		}

		public static bool TextAsset_ReadLine( string line, out string key, out string value, out string category, out string comment, out string termType )
		{
			key		= string.Empty;
			category= string.Empty;
			comment = string.Empty;
			termType= string.Empty;
			value = string.Empty;

			//--[ Comment ]-----------------------
			int iComment = line.LastIndexOf("//");
			if (iComment>=0)
			{
				comment = line.Substring(iComment+2).Trim();
				comment = DecodeString(comment);
				line = line.Substring(0, iComment);
			}

			//--[ Key ]-----------------------------
			int iKeyEnd = line.IndexOf("=");
			if (iKeyEnd<0)
			{
				return false;
			}
			else
			{
				key = line.Substring(0, iKeyEnd).Trim();
				value = line.Substring(iKeyEnd+1).Trim();
				value = value.Replace ("\r\n", "\n").Replace ("\n", "\\n");
				value = DecodeString(value);
			}

			//--[ Type ]---------
			if (key.Length>2 && key[0]=='[')
			{
				int iTypeEnd = key.IndexOf(']');
				if (iTypeEnd>=0)
				{
					termType = key.Substring(1, iTypeEnd-1);
					key = key.Substring(iTypeEnd+1);
				}
			}
			
			ValidateFullTerm( ref key );

			return true;
		}

		#endregion

		#region CSV
		public static string ReadCsVfile( string path, Encoding encoding )
		{
			string text = string.Empty;
			#if (UNITY_WP8 || UNITY_METRO) && !UNITY_EDITOR
				byte[] buffer = UnityEngine.Windows.File.ReadAllBytes (Path);
				Text = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
			#else
				/*using (System.IO.StreamReader reader = System.IO.File.OpenText(Path))
				{
					Text = reader.ReadToEnd();
				}*/
				using (var reader = new System.IO.StreamReader(path, encoding ))
					text = reader.ReadToEnd();
			#endif

			text = text.Replace("\r\n", "\n");
			text = text.Replace("\r", "\n");

			return text;
		}

		public static List<string[]> ReadCsv( string text, char separator=',' )
		{
			int iStart = 0;
			List<string[]> csv = new List<string[]>();

			while (iStart < text.Length)
			{
				string[] list = ParseCsVline (text, ref iStart, separator);
				if (list==null) break;
				csv.Add(list);
			}
			return csv;
		}

		static string[] ParseCsVline( string line, ref int iStart, char separator )
		{
			List<string> list = new List<string>();
			
			//Line = "puig,\"placeres,\"\"cab\nr\nera\"\"algo\"\npuig";//\"Frank\npuig\nplaceres\",aaa,frank\nplaceres";

			int textLength = line.Length;
			int iWordStart = iStart;
			bool insideQuote = false;

			while (iStart < textLength)
			{
				char c = line[iStart];

				if (insideQuote)
				{
					if (c=='\"') //--[ Look for Quote End ]------------
					{
						if (iStart+1 >= textLength || line[iStart+1] != '\"')  //-- Single Quote:  Quotation Ends
						{
							insideQuote = false;
						}
						else
						if (iStart+2 < textLength && line[iStart+2]=='\"')  //-- Tripple Quotes: Quotation ends
						{
							insideQuote = false;
							iStart+=2;
						}
						else 
							iStart++;  // Skip Double Quotes
					}
				}

				else  //-----[ Separators ]----------------------

				if (c=='\n' || c==separator)
				{
					AddCsVtoken(ref list, ref line, iStart, ref iWordStart);
					if (c=='\n')  // Stop the row on line breaks
					{
						iStart++;
						break;
					}
				}

				else //--------[ Start Quote ]--------------------

				if (c=='\"')
					insideQuote = true;

				iStart++;
			}
			if (iStart>iWordStart)
				AddCsVtoken(ref list, ref line, iStart, ref iWordStart);

			return list.ToArray();
		}

		static void AddCsVtoken( ref List<string> list, ref string line, int iEnd, ref int iWordStart)
		{
			string text = line.Substring(iWordStart, iEnd-iWordStart);
			iWordStart = iEnd+1;

			text = text.Replace("\"\"", "\"" );
			if (text.Length>1 && text[0]=='\"' && text[text.Length-1]=='\"')
				text = text.Substring(1, text.Length-2 );

			list.Add( text);
		}

		

		#endregion

		#region I2CSV

		public static List<string[]> ReadI2Csv( string text )
		{
			string[] columnSeparator = new string[]{"[*]"};
			string[] rowSeparator = new string[]{"[ln]"};

			List<string[]> csv = new List<string[]>();
			foreach (var line in text.Split (rowSeparator, System.StringSplitOptions.None))
				csv.Add (line.Split (columnSeparator, System.StringSplitOptions.None));

			return csv;
		}

		#endregion

		#region Misc

		public static void ValidateFullTerm( ref string term )
		{
			term = term.Replace('\\', '/');
			int first = term.IndexOf('/');
			if (first<0)
				return;
			
			int second;
			while ( (second=term.LastIndexOf('/')) != first )
				term = term.Remove( second,1);
		}

		
		// this function encodes \r\n and \n into \\n
		public static string EncodeString( string str )
		{
			if (string.IsNullOrEmpty(str))
				return string.Empty;

			return str.Replace("\r\n", "<\\n>")
				.Replace("\r", "<\\n>")
					.Replace("\n", "<\\n>");
		}
		
		public static string DecodeString( string str )
		{
			if (string.IsNullOrEmpty(str))
				return string.Empty;
			
			return str.Replace("<\\n>", "\r\n");
		}

		#endregion
	}
}
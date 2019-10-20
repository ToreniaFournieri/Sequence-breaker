//#define USE_SharpZipLib
#if !UNITY_WEBPLAYER && !NETFX_CORE
#define USE_FileIO
#endif

//#define DISABLE_LINQ_EXT

/* * * * *
 * A simple JSON Parser / builder
 * ------------------------------
 * 
 * It mainly has been written as a simple JSON parser. It can build a JSON string
 * from the node-tree, or generate a node tree from any valid JSON string.
 * 
 * If you want to use compression when saving to file / stream / B64 you have to include
 * SharpZipLib ( http://www.icsharpcode.net/opensource/sharpziplib/ ) in your project and
 * define "USE_SharpZipLib" at the top of the file
 * 
 * Written by Bunny83 
 * 2012-06-09
 * 
 * Features / attributes:
 * - provides strongly typed node classes and lists / dictionaries
 * - provides easy access to class members / array items / data values
 * - the parser ignores data types. Each value is a string.
 * - only double quotes (") are used for quoting strings.
 * - values and names are not restricted to quoted strings. They simply add up and are trimmed.
 * - There are only 3 types: arrays(JSONArray), objects(JSONClass) and values(JSONData)
 * - provides "casting" properties to easily convert to / from those types:
 *   int / float / double / bool
 * - provides a common interface for each node so no explicit casting is required.
 * - the parser try to avoid errors, but if malformed JSON is parsed the result is undefined
 * 
 * 
 * 2012-12-17 Update:
 * - Added internal JSONLazyCreator class which simplifies the construction of a JSON tree
 *   Now you can simple reference any item that doesn't exist yet and it will return a JSONLazyCreator
 *   The class determines the required type by it's further use, creates the type and removes itself.
 * - Added binary serialization / deserialization.
 * - Added support for BZip2 zipped binary format. Requires the SharpZipLib ( http://www.icsharpcode.net/opensource/sharpziplib/ )
 *   The usage of the SharpZipLib library can be disabled by removing or commenting out the USE_SharpZipLib define at the top
 * - The serializer uses different types when it comes to store the values. Since my data values
 *   are all of type string, the serializer will "try" which format fits best. The order is: int, float, double, bool, string.
 *   It's not the most efficient way but for a moderate amount of data it should work on all platforms.
 * 
 * * * * */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace _00_Asset._06_I2_Localization.Localization.Scripts.Google
{
	public enum JsonBinaryTag
	{
		Array            = 1,
		Class            = 2,
		Value            = 3,
		IntValue        = 4,
		DoubleValue        = 5,
		BoolValue        = 6,
		FloatValue        = 7,
	}
	
	public class JsonNode
	{
		#region common interface
		public virtual void Add(string aKey, JsonNode aItem){ }
		public virtual JsonNode this[int aIndex]   { get { return null; } set { } }
		public virtual JsonNode this[string aKey]  { get { return null; } set { } }
		public virtual string Value                { get { return "";   } set { } }
		public virtual int Count                   { get { return 0;    } }
		
		public virtual void Add(JsonNode aItem)
		{
			Add("", aItem);
		}
		
		public virtual JsonNode Remove(string aKey) { return null; }
		public virtual JsonNode Remove(int aIndex) { return null; }
		public virtual JsonNode Remove(JsonNode aNode) { return aNode; }
		
		public virtual IEnumerable<JsonNode> Childs { get { yield break;} }
		public IEnumerable<JsonNode> DeepChilds
		{
			get
			{
				foreach (var c in Childs)
					foreach (var d in c.DeepChilds)
						yield return d;
			}
		}
		
		public override string ToString()
		{
			return "JSONNode";
		}
		public virtual string ToString(string aPrefix)
		{
			return "JSONNode";
		}
		
		#endregion common interface
		
		#region typecasting properties
		public virtual int AsInt
		{
			get
			{
				int v = 0;
				if (int.TryParse(Value,out v))
					return v;
				return 0;
			}
			set
			{
				Value = value.ToString();
			}
		}
		public virtual float AsFloat
		{
			get
			{
				float v = 0.0f;
				if (float.TryParse(Value,out v))
					return v;
				return 0.0f;
			}
			set
			{
				Value = value.ToString();
			}
		}
		public virtual double AsDouble
		{
			get
			{
				double v = 0.0;
				if (double.TryParse(Value,out v))
					return v;
				return 0.0;
			}
			set
			{
				Value = value.ToString();
			}
		}
		public virtual bool AsBool
		{
			get
			{
				bool v = false;
				if (bool.TryParse(Value,out v))
					return v;
				return !string.IsNullOrEmpty(Value);
			}
			set
			{
				Value = (value)?"true":"false";
			}
		}
		public virtual JsonArray AsArray
		{
			get
			{
				return this as JsonArray;
			}
		}
		public virtual JsonClass AsObject
		{
			get
			{
				return this as JsonClass;
			}
		}
		
		
		#endregion typecasting properties
		
		#region operators
		public static implicit operator JsonNode(string s)
		{
			return new JsonData(s);
		}
		public static implicit operator string(JsonNode d)
		{
			return (d == null)?null:d.Value;
		}
		public static bool operator ==(JsonNode a, object b)
		{
			if (b == null && a is JsonLazyCreator)
				return true;
			return ReferenceEquals(a,b);
		}
		
		public static bool operator !=(JsonNode a, object b)
		{
			return !(a == b);
		}
		public override bool Equals (object obj)
		{
			return ReferenceEquals(this, obj);
		}
		public override int GetHashCode ()
		{
			return base.GetHashCode();
		}
		
		
		#endregion operators
		
		internal static string Escape(string aText)
		{
			string result = "";
			foreach(char c in aText)
			{
				switch(c)
				{
				case '\\' : result += "\\\\"; break;
				case '\"' : result += "\\\""; break;
				case '\n' : result += "\\n" ; break;
				case '\r' : result += "\\r" ; break;
				case '\t' : result += "\\t" ; break;
				case '\b' : result += "\\b" ; break;
				case '\f' : result += "\\f" ; break;
				default   : result += c     ; break;
				}
			}
			return result;
		}
		
		public static JsonNode Parse(string aJson)
		{
			Stack<JsonNode> stack = new Stack<JsonNode>();
			JsonNode ctx = null;
			int i = 0;
			string token = "";
			string tokenName = "";
			bool quoteMode = false;
			while (i < aJson.Length)
			{
				switch (aJson[i])
				{
				case '{':
					if (quoteMode)
					{
						token += aJson[i];
						break;
					}
					stack.Push(new JsonClass());
					if (ctx != null)
					{
						tokenName = tokenName.Trim();
						if (ctx is JsonArray)
							ctx.Add(stack.Peek());
						else if (tokenName != "")
							ctx.Add(tokenName,stack.Peek());
					}
					tokenName = "";
					token = "";
					ctx = stack.Peek();
					break;
					
				case '[':
					if (quoteMode)
					{
						token += aJson[i];
						break;
					}
					
					stack.Push(new JsonArray());
					if (ctx != null)
					{
						tokenName = tokenName.Trim();
						if (ctx is JsonArray)
							ctx.Add(stack.Peek());
						else if (tokenName != "")
							ctx.Add(tokenName,stack.Peek());
					}
					tokenName = "";
					token = "";
					ctx = stack.Peek();
					break;
					
				case '}':
				case ']':
					if (quoteMode)
					{
						token += aJson[i];
						break;
					}
					if (stack.Count == 0)
						throw new Exception("JSON Parse: Too many closing brackets");
					
					stack.Pop();
					if (token != "")
					{
						tokenName = tokenName.Trim();
						if (ctx is JsonArray)
							ctx.Add(token);
						else if (tokenName != "")
							ctx.Add(tokenName,token);
					}
					tokenName = "";
					token = "";
					if (stack.Count>0)
						ctx = stack.Peek();
					break;
					
				case ':':
					if (quoteMode)
					{
						token += aJson[i];
						break;
					}
					tokenName = token;
					token = "";
					break;
					
				case '"':
					quoteMode ^= true;
					break;
					
				case ',':
					if (quoteMode)
					{
						token += aJson[i];
						break;
					}
					if (token != "")
					{
						if (ctx is JsonArray)
							ctx.Add(token);
						else if (tokenName != "")
							ctx.Add(tokenName, token);
					}
					tokenName = "";
					token = "";
					break;
					
				case '\r':
				case '\n':
					break;
					
				case ' ':
				case '\t':
					if (quoteMode)
						token += aJson[i];
					break;
					
				case '\\':
					++i;
					if (quoteMode)
					{
						char c = aJson[i];
						switch (c)
						{
						case 't' : token += '\t'; break;
						case 'r' : token += '\r'; break;
						case 'n' : token += '\n'; break;
						case 'b' : token += '\b'; break;
						case 'f' : token += '\f'; break;
						case 'u':
						{
							string s = aJson.Substring(i+1,4);
							token += (char)int.Parse(s, System.Globalization.NumberStyles.AllowHexSpecifier);
							i += 4;
							break;
						}
						default  : token += c; break;
						}
					}
					break;
					
				default:
					token += aJson[i];
					break;
				}
				++i;
			}
			if (quoteMode)
			{
				throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
			}
			return ctx;
		}
		
		public virtual void Serialize(System.IO.BinaryWriter aWriter) {}
		
		public void SaveToStream(System.IO.Stream aData)
		{
			var w = new System.IO.BinaryWriter(aData);
			Serialize(w);
		}
		
		#if USE_SharpZipLib
		public void SaveToCompressedStream(System.IO.Stream aData)
		{
			using (var gzipOut = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(aData))
			{
				gzipOut.IsStreamOwner = false;
				SaveToStream(gzipOut);
				gzipOut.Close();
			}
		}
		
		public void SaveToCompressedFile(string aFileName)
		{
			#if USE_FileIO
			System.IO.Directory.CreateDirectory((new System.IO.FileInfo(aFileName)).Directory.FullName);
			using(var F = System.IO.File.OpenWrite(aFileName))
			{
				SaveToCompressedStream(F);
			}
			#else
			throw new Exception("Can't use File IO stuff in webplayer");
			#endif
		}
		public string SaveToCompressedBase64()
		{
			using (var stream = new System.IO.MemoryStream())
			{
				SaveToCompressedStream(stream);
				stream.Position = 0;
				return System.Convert.ToBase64String(stream.ToArray());
			}
		}
		
		#else
		public void SaveToCompressedStream(System.IO.Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}
		public void SaveToCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}
		public string SaveToCompressedBase64()
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}
		#endif
		
		public void SaveToFile(string aFileName)
		{
			#if USE_FileIO
			System.IO.Directory.CreateDirectory((new System.IO.FileInfo(aFileName)).Directory.FullName);
			using(var f = System.IO.File.OpenWrite(aFileName))
			{
				SaveToStream(f);
			}
			#else
			throw new Exception("Can't use File IO stuff in webplayer");
			#endif
		}
		public string SaveToBase64()
		{
			using (var stream = new System.IO.MemoryStream())
			{
				SaveToStream(stream);
				stream.Position = 0;
				return Convert.ToBase64String(stream.ToArray());
			}
		}
		public static JsonNode Deserialize(System.IO.BinaryReader aReader)
		{
			JsonBinaryTag type = (JsonBinaryTag)aReader.ReadByte();
			switch(type)
			{
			case JsonBinaryTag.Array:
			{
				int count = aReader.ReadInt32();
				JsonArray tmp = new JsonArray();
				for(int i = 0; i < count; i++)
					tmp.Add(Deserialize(aReader));
				return tmp;
			}
			case JsonBinaryTag.Class:
			{
				int count = aReader.ReadInt32();                
				JsonClass tmp = new JsonClass();
				for(int i = 0; i < count; i++)
				{
					string key = aReader.ReadString();
					var val = Deserialize(aReader);
					tmp.Add(key, val);
				}
				return tmp;
			}
			case JsonBinaryTag.Value:
			{
				return new JsonData(aReader.ReadString());
			}
			case JsonBinaryTag.IntValue:
			{
				return new JsonData(aReader.ReadInt32());
			}
			case JsonBinaryTag.DoubleValue:
			{
				return new JsonData(aReader.ReadDouble());
			}
			case JsonBinaryTag.BoolValue:
			{
				return new JsonData(aReader.ReadBoolean());
			}
			case JsonBinaryTag.FloatValue:
			{
				return new JsonData(aReader.ReadSingle());
			}
				
			default:
			{
				throw new Exception("Error deserializing JSON. Unknown tag: " + type);
			}
			}
		}
		
		#if USE_SharpZipLib
		public static JSONNode LoadFromCompressedStream(System.IO.Stream aData)
		{
			var zin = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(aData);
			return LoadFromStream(zin);
		}
		public static JSONNode LoadFromCompressedFile(string aFileName)
		{
			#if USE_FileIO
			using(var F = System.IO.File.OpenRead(aFileName))
			{
				return LoadFromCompressedStream(F);
			}
			#else
			throw new Exception("Can't use File IO stuff in webplayer");
			#endif
		}
		public static JSONNode LoadFromCompressedBase64(string aBase64)
		{
			var tmp = System.Convert.FromBase64String(aBase64);
			var stream = new System.IO.MemoryStream(tmp);
			stream.Position = 0;
			return LoadFromCompressedStream(stream);
		}
		#else
		public static JsonNode LoadFromCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}
		public static JsonNode LoadFromCompressedStream(System.IO.Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}
		public static JsonNode LoadFromCompressedBase64(string aBase64)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}
		#endif
		
		public static JsonNode LoadFromStream(System.IO.Stream aData)
		{
			using(var r = new System.IO.BinaryReader(aData))
			{
				return Deserialize(r);
			}
		}
		public static JsonNode LoadFromFile(string aFileName)
		{
			#if USE_FileIO
			using(var f = System.IO.File.OpenRead(aFileName))
			{
				return LoadFromStream(f);
			}
			#else
			throw new Exception("Can't use File IO stuff in webplayer");
			#endif
		}
		public static JsonNode LoadFromBase64(string aBase64)
		{
			var tmp = Convert.FromBase64String(aBase64);
			var stream = new System.IO.MemoryStream(tmp);
			stream.Position = 0;
			return LoadFromStream(stream);
		}
	} // End of JSONNode
	
	public class JsonArray : JsonNode, IEnumerable
	{
		private List<JsonNode> _mList = new List<JsonNode>();
		public override JsonNode this[int aIndex]
		{
			get
			{
				if (aIndex<0 || aIndex >= _mList.Count)
					return new JsonLazyCreator(this);
				return _mList[aIndex];
			}
			set
			{
				if (aIndex<0 || aIndex >= _mList.Count)
					_mList.Add(value);
				else
					_mList[aIndex] = value;
			}
		}
		public override JsonNode this[string aKey]
		{
			get{ return new JsonLazyCreator(this);}
			set{ _mList.Add(value); }
		}
		public override int Count
		{
			get { return _mList.Count; }
		}
		public override void Add(string aKey, JsonNode aItem)
		{
			_mList.Add(aItem);
		}
		public override JsonNode Remove(int aIndex)
		{
			if (aIndex < 0 || aIndex >= _mList.Count)
				return null;
			JsonNode tmp = _mList[aIndex];
			_mList.RemoveAt(aIndex);
			return tmp;
		}
		public override JsonNode Remove(JsonNode aNode)
		{
			_mList.Remove(aNode);
			return aNode;
		}
		public override IEnumerable<JsonNode> Childs
		{
			get
			{
				foreach(JsonNode n in _mList)
					yield return n;
			}
		}
		public IEnumerator GetEnumerator()
		{
			foreach(JsonNode n in _mList)
				yield return n;
		}
		public override string ToString()
		{
			string result = "[ ";
			foreach (JsonNode n in _mList)
			{
				if (result.Length > 2)
					result += ", ";
				result += n.ToString();
			}
			result += " ]";
			return result;
		}
		public override string ToString(string aPrefix)
		{
			string result = "[ ";
			foreach (JsonNode n in _mList)
			{
				if (result.Length > 3)
					result += ", ";
				result += "\n" + aPrefix + "   ";                
				result += n.ToString(aPrefix+"   ");
			}
			result += "\n" + aPrefix + "]";
			return result;
		}
		public override void Serialize (System.IO.BinaryWriter aWriter)
		{
			aWriter.Write((byte)JsonBinaryTag.Array);
			aWriter.Write(_mList.Count);
			for(int i = 0; i < _mList.Count; i++)
			{
				_mList[i].Serialize(aWriter);
			}
		}
	} // End of JSONArray
	
	public class JsonClass : JsonNode, IEnumerable
	{
        private Dictionary<string, JsonNode> _mDict = new Dictionary<string, JsonNode>(StringComparer.Ordinal);
		public override JsonNode this[string aKey]
		{
			get
			{
				if (_mDict.ContainsKey(aKey))
					return _mDict[aKey];
				else
					return new JsonLazyCreator(this, aKey);
			}
			set
			{
				if (_mDict.ContainsKey(aKey))
					_mDict[aKey] = value;
				else
					_mDict.Add(aKey,value);
			}
		}
		public override JsonNode this[int aIndex]
		{
			get
			{
				if (aIndex < 0 || aIndex >= _mDict.Count)
					return null;

#if DISABLE_LINQ_EXT
				foreach (var kvp in m_Dict)
				{
					if (aIndex==0)
						return kvp.Value;
					aIndex--;
				}
				return null;
#else
				return _mDict.ElementAt(aIndex).Value;
#endif
			}
			set
			{
				if (aIndex < 0 || aIndex >= _mDict.Count)
					return;
#if DISABLE_LINQ_EXT
				string[] keys = new string[m_Dict.Keys.Count];
				m_Dict.Keys.CopyTo(keys,0);
				string key = keys[aIndex];
#else
				string key = _mDict.ElementAt(aIndex).Key;
#endif
				_mDict[key] = value;
			}
		}
		public override int Count
		{
			get { return _mDict.Count; }
		}
		
		
		public override void Add(string aKey, JsonNode aItem)
		{
			if (!string.IsNullOrEmpty(aKey))
			{
				if (_mDict.ContainsKey(aKey))
					_mDict[aKey] = aItem;
				else
					_mDict.Add(aKey, aItem);
			}
			else
				_mDict.Add(Guid.NewGuid().ToString(), aItem);
		}
		
		public override JsonNode Remove(string aKey)
		{
			if (!_mDict.ContainsKey(aKey))
				return null;
			JsonNode tmp = _mDict[aKey];
			_mDict.Remove(aKey);
			return tmp;        
		}
		public override JsonNode Remove(int aIndex)
		{
			if (aIndex < 0 || aIndex >= _mDict.Count)
				return null;

#if DISABLE_LINQ_EXT
			string[] keys = new string[m_Dict.Keys.Count];
			m_Dict.Keys.CopyTo(keys,0);
			string key = keys[aIndex];
			var value = m_Dict[key];
			m_Dict.Remove(key);
			return value;
#else
			var item = _mDict.ElementAt(aIndex);
			_mDict.Remove(item.Key);
			return item.Value;
#endif
		}
		public override JsonNode Remove(JsonNode aNode)
		{
			try
			{
#if DISABLE_LINQ_EXT
				foreach (var kvp in m_Dict)
					if (kvp.Value == aNode)
					{
						m_Dict.Remove(kvp.Key);
						break;
					}
				return aNode;
#else
				var item = _mDict.Where(k => k.Value == aNode).First();
				_mDict.Remove(item.Key);
				return aNode;
#endif
			}
			catch
			{
				return null;
			}
		}
		
		public override IEnumerable<JsonNode> Childs
		{
			get
			{
				foreach(KeyValuePair<string,JsonNode> n in _mDict)
					yield return n.Value;
			}
		}
		
		public IEnumerator GetEnumerator()
		{
			foreach(KeyValuePair<string, JsonNode> n in _mDict)
				yield return n;
		}
		public override string ToString()
		{
			string result = "{";
			foreach (KeyValuePair<string, JsonNode> n in _mDict)
			{
				if (result.Length > 2)
					result += ", ";
				result += "\"" + Escape(n.Key) + "\":" + n.Value.ToString();
			}
			result += "}";
			return result;
		}
		public override string ToString(string aPrefix)
		{
			string result = "{ ";
			foreach (KeyValuePair<string, JsonNode> n in _mDict)
			{
				if (result.Length > 3)
					result += ", ";
				result += "\n" + aPrefix + "   ";
				result += "\"" + Escape(n.Key) + "\" : " + n.Value.ToString(aPrefix+"   ");
			}
			result += "\n" + aPrefix + "}";
			return result;
		}
		public override void Serialize (System.IO.BinaryWriter aWriter)
		{
			aWriter.Write((byte)JsonBinaryTag.Class);
			aWriter.Write(_mDict.Count);
			foreach(string k in _mDict.Keys)
			{
				aWriter.Write(k);
				_mDict[k].Serialize(aWriter);
			}
		}
	} // End of JSONClass
	
	public class JsonData : JsonNode
	{
		private string _mData;
		public override string Value
		{
			get { return _mData; }
			set { _mData = value; }
		}
		public JsonData(string aData)
		{
			_mData = aData;
		}
		public JsonData(float aData)
		{
			AsFloat = aData;
		}
		public JsonData(double aData)
		{
			AsDouble = aData;
		}
		public JsonData(bool aData)
		{
			AsBool = aData;
		}
		public JsonData(int aData)
		{
			AsInt = aData;
		}
		
		public override string ToString()
		{
			return "\"" + Escape(_mData) + "\"";
		}
		public override string ToString(string aPrefix)
		{
			return "\"" + Escape(_mData) + "\"";
		}
		public override void Serialize (System.IO.BinaryWriter aWriter)
		{
			var tmp = new JsonData("");
			
			tmp.AsInt = AsInt;
			if (tmp._mData == _mData)
			{
				aWriter.Write((byte)JsonBinaryTag.IntValue);
				aWriter.Write(AsInt);
				return;
			}
			tmp.AsFloat = AsFloat;
			if (tmp._mData == _mData)
			{
				aWriter.Write((byte)JsonBinaryTag.FloatValue);
				aWriter.Write(AsFloat);
				return;
			}
			tmp.AsDouble = AsDouble;
			if (tmp._mData == _mData)
			{
				aWriter.Write((byte)JsonBinaryTag.DoubleValue);
				aWriter.Write(AsDouble);
				return;
			}
			
			tmp.AsBool = AsBool;
			if (tmp._mData == _mData)
			{
				aWriter.Write((byte)JsonBinaryTag.BoolValue);
				aWriter.Write(AsBool);
				return;
			}
			aWriter.Write((byte)JsonBinaryTag.Value);
			aWriter.Write(_mData);
		}
	} // End of JSONData
	
	internal class JsonLazyCreator : JsonNode
	{
		private JsonNode _mNode = null;
		private string _mKey = null;
		
		public JsonLazyCreator(JsonNode aNode)
		{
			_mNode = aNode;
			_mKey  = null;
		}
		public JsonLazyCreator(JsonNode aNode, string aKey)
		{
			_mNode = aNode;
			_mKey = aKey;
		}
		
		private void Set(JsonNode aVal)
		{
			if (_mKey == null)
			{
				_mNode.Add(aVal);
			}
			else
			{
				_mNode.Add(_mKey, aVal);
			}
			_mNode = null; // Be GC friendly.
		}
		
		public override JsonNode this[int aIndex]
		{
			get
			{
				return new JsonLazyCreator(this);
			}
			set
			{
				var tmp = new JsonArray();
				tmp.Add(value);
				Set(tmp);
			}
		}
		
		public override JsonNode this[string aKey]
		{
			get
			{
				return new JsonLazyCreator(this, aKey);
			}
			set
			{
				var tmp = new JsonClass();
				tmp.Add(aKey, value);
				Set(tmp);
			}
		}
		public override void Add (JsonNode aItem)
		{
			var tmp = new JsonArray();
			tmp.Add(aItem);
			Set(tmp);
		}
		public override void Add (string aKey, JsonNode aItem)
		{
			var tmp = new JsonClass();
			tmp.Add(aKey, aItem);
			Set(tmp);
		}
		public static bool operator ==(JsonLazyCreator a, object b)
		{
			if (b == null)
				return true;
			return ReferenceEquals(a,b);
		}
		
		public static bool operator !=(JsonLazyCreator a, object b)
		{
			return !(a == b);
		}
		public override bool Equals (object obj)
		{
			if (obj == null)
				return true;
			return ReferenceEquals(this, obj);
		}
		public override int GetHashCode ()
		{
			return base.GetHashCode();
		}
		
		public override string ToString()
		{
			return "";
		}
		public override string ToString(string aPrefix)
		{
			return "";
		}
		
		public override int AsInt
		{
			get
			{
				JsonData tmp = new JsonData(0);
				Set(tmp);
				return 0;
			}
			set
			{
				JsonData tmp = new JsonData(value);
				Set(tmp);
			}
		}
		public override float AsFloat
		{
			get
			{
				JsonData tmp = new JsonData(0.0f);
				Set(tmp);
				return 0.0f;
			}
			set
			{
				JsonData tmp = new JsonData(value);
				Set(tmp);
			}
		}
		public override double AsDouble
		{
			get
			{
				JsonData tmp = new JsonData(0.0);
				Set(tmp);
				return 0.0;
			}
			set
			{
				JsonData tmp = new JsonData(value);
				Set(tmp);
			}
		}
		public override bool AsBool
		{
			get
			{
				JsonData tmp = new JsonData(false);
				Set(tmp);
				return false;
			}
			set
			{
				JsonData tmp = new JsonData(value);
				Set(tmp);
			}
		}
		public override JsonArray AsArray
		{
			get
			{
				JsonArray tmp = new JsonArray();
				Set(tmp);
				return tmp;
			}
		}
		public override JsonClass AsObject
		{
			get
			{
				JsonClass tmp = new JsonClass();
				Set(tmp);
				return tmp;
			}
		}
	} // End of JSONLazyCreator
	
	public static class Json
	{
		public static JsonNode Parse(string aJson)
		{
			return JsonNode.Parse(aJson);
		}
	}
}
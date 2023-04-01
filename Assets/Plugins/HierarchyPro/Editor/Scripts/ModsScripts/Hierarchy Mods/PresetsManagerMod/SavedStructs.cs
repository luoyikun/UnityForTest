using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace EMX.HierarchyPlugin.Editor.PresetsManager
{




	//[Serializable]
	/*public class KeeperData
	{
		//[NonSerialized]
		//public Dictionary<long, string> comp_to_Type = new Dictionary<long, string>();
		[NonSerialized]
		public Dictionary<long, KeeperDataItem> field_records = new Dictionary<long, KeeperDataItem>();

		public KeeperData(string json)
		{
			long[] array = null;
			string[] array2 = null;
			KeeperDataItem_SerizeHelper keeperDataItem_SerizeHelper = null;
			try
			{
				array = (long[])info.GetValue("comp_to_Type keys", typeof(long[]));
				array2 = (string[])info.GetValue("comp_to_Type values", typeof(string[]));
				keeperDataItem_SerizeHelper = Serializer.DESERIALIZE_SINGLE<KeeperDataItem_SerizeHelper>((string)info.GetValue("field_records", typeof(string)));
			}
			catch
			{
			}
			comp_to_Type = new Dictionary<long, string>();
			for (int i = 0; i < array.Length; i++)
			{
				comp_to_Type.Add(array[i], array2[i]);
			}
			field_records = new Dictionary<long, KeeperDataItem>();
			for (int j = 0; j < keeperDataItem_SerizeHelper.keys.Length; j++)
			{
				if (keeperDataItem_SerizeHelper.values[j] != null)
				{
					field_records.Add(keeperDataItem_SerizeHelper.keys[j], keeperDataItem_SerizeHelper.values[j]);
				}
			}
		}






		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			long[] array = new long[comp_to_Type.Count];
			string[] array2 = new string[comp_to_Type.Count];
			int num = 0;
			foreach (KeyValuePair<long, string> item in comp_to_Type)
			{
				array[num] = item.Key;
				array2[num] = item.Value;
				num++;
			}
			KeeperDataItem_SerizeHelper ob = new KeeperDataItem_SerizeHelper(field_records);
			info.AddValue("comp_to_Type keys", array, typeof(long[]));
			info.AddValue("comp_to_Type values", array2, typeof(string[]));
			info.AddValue("field_records", Serializer.SERIALIZE_SINGLE(ob), typeof(string));
		}



	}
	*/

	//[Serializable]
	public class KeeperDataItem
	{

		[NonSerialized]
		public Dictionary<int, KeeperDataUnityJsonData> records = new Dictionary<int, KeeperDataUnityJsonData>();
		[NonSerialized]
		public string COMP_TYPE = "";


		public string Save()
		{
			int v = 1;
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(v.ToString());
			sb.AppendLine(COMP_TYPE);
			foreach (var record in records)
			{
				sb.AppendLine("array");
				sb.AppendLine(record.Key.ToString());
				sb.AppendLine(record.Value.Save(v));
			}
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(sb.ToString()));
		}
		public void Load(string b64)
		{
			records.Clear();
			if (b64 == null || b64 == "") return;
			var sr = new StringReader(Encoding.UTF8.GetString(Convert.FromBase64String(b64)));
			var v = int.Parse(sr.ReadLine()); //version
			COMP_TYPE = (sr.ReadLine());
			while (sr.ReadLine() != null)
			{
				var key = int.Parse(sr.ReadLine());
				var value = new KeeperDataUnityJsonData();
				value.Load(sr.ReadLine(), v);
				records.Add(key, value);

				//	Array.Resize(ref presets, presets.Length + 1);
				//	presets[presets.Length - 1] = new SetItem();
				//	presets[presets.Length - 1].Load(ref sr, v);
			}
		}


	}























	//[Serializable]
	public class KeeperDataUnityJsonData : IEquatable<KeeperDataUnityJsonData>
	{
		[SerializeField]
		public int index;
		[SerializeField]
		public string default_json;
		[SerializeField]
		public string[] fields_name = new string[0];
		/*[Obsolete]
		[SerializeField]
		public long[] fields_value;*/
		[SerializeField]
		public KeeperDataFieldValue[] fields_new_value = new KeeperDataFieldValue[0];





		public string Save(int v)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(index.ToString());
			sb.AppendLine(default_json);
			foreach (var item in fields_name)
			{
				sb.AppendLine("-");
				sb.AppendLine(item);
			}
			sb.AppendLine("EOA");
			//if (fields_new_value.Any(c => c == null)) fields_new_value = fields_new_value.Where(c => c != null).ToArray();
			if (fields_new_value.Length != fields_name.Length) Array.Resize(ref fields_new_value, fields_name.Length);
			//for (int i = fields_new_value.Length - 1; i >= 0; i--)
			//{
			//	if (fields_new_value[i] == null)
			//	{
			//		ArrayUtility.RemoveAt(ref fields_new_value, i);
			//		ArrayUtility.RemoveAt(ref fields_name, i);
			//	}
			//}

			foreach (var item in fields_new_value)
			{
				sb.AppendLine("-");
				if (item != null) sb.AppendLine(item.Save(v));
				else sb.AppendLine("");
			}
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(sb.ToString()));
		}
		public void Load(string b64, int v)
		{
			var sr = new StringReader(Encoding.UTF8.GetString(Convert.FromBase64String(b64)));
			index = int.Parse(sr.ReadLine());
			default_json = (sr.ReadLine());
			List<string> r1 = new List<string>();
			while (sr.ReadLine() != "EOA")
			{
				r1.Add(sr.ReadLine());
				if (r1.Last() == null) throw new Exception("ASD");
			}
			List<KeeperDataFieldValue> r2 = new List<KeeperDataFieldValue>();
			while (sr.ReadLine() != null)
			{
				var line = sr.ReadLine();
				if (line != null && line != "")
				{
					var r = new KeeperDataFieldValue();
					r.Load(line, v);
					r2.Add(r);
				}
				else
				{
					r2.Add(null);
				}

			}
			fields_name = r1.ToArray();
			fields_new_value = r2.ToArray();
		}









		public static bool operator ==(KeeperDataUnityJsonData a1, KeeperDataUnityJsonData a2)
		{
			return StaticEquals(a1, a2);
		}

		public static bool operator !=(KeeperDataUnityJsonData a1, KeeperDataUnityJsonData a2)
		{
			return !StaticEquals(a1, a2);
		}

		public static bool StaticEquals(KeeperDataUnityJsonData x, KeeperDataUnityJsonData y)
		{
			if ((object)x == null && (object)y == null)
			{
				return true;
			}
			if ((object)x != null && (object)y != null)
			{
				return x.Equals(y);
			}
			return false;
		}

		public bool Equals(KeeperDataUnityJsonData x, KeeperDataUnityJsonData y)
		{
			return StaticEquals(x, y);
		}

		public bool Equals(KeeperDataUnityJsonData other)
		{
			if (other == null)
			{
				return false;
			}
			if (fields_new_value == null && other.fields_new_value == null)
			{
				return default_json == other.default_json;
			}
			if (fields_new_value == null || other.fields_new_value == null)
			{
				return false;
			}
			if (fields_new_value.Length != other.fields_new_value.Length)
			{
				return false;
			}
			for (int i = 0; i < fields_new_value.Length; i++)
			{
				if (fields_new_value[i] != other.fields_new_value[i])
				{
					return false;
				}
			}
			return default_json == other.default_json;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as KeeperDataUnityJsonData);
		}

		bool IEquatable<KeeperDataUnityJsonData>.Equals(KeeperDataUnityJsonData other)
		{
			return Equals(other);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
















	public struct externalheapdata
	{
		public int ID_IN_EXTERNAL_HEAP;
		public string _globaliddata;
		public string globaliddata
		{
			get
			{
				if (_globaliddata == null || _globaliddata == "") return "";
				return (Encoding.UTF8.GetString(Convert.FromBase64String(_globaliddata)));
			}
			set
			{
				_globaliddata = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
			}
		}

		public string asset_guid;

	}

	//[Serializable]
	public class KeeperDataFieldValue : IEquatable<KeeperDataFieldValue>
	{
		[SerializeField]
		public externalheapdata EXTERNAL_HEAP = new externalheapdata() { ID_IN_EXTERNAL_HEAP = -1, globaliddata = "" };
		[SerializeField]
		public string GUID;



		public string Save(int v)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(EXTERNAL_HEAP.ID_IN_EXTERNAL_HEAP.ToString());
			sb.AppendLine(EXTERNAL_HEAP._globaliddata);
			sb.AppendLine(GUID);
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(sb.ToString()));
		}
		public void Load(string b64, int v)
		{
			var sr = new StringReader(Encoding.UTF8.GetString(Convert.FromBase64String(b64)));
			EXTERNAL_HEAP.ID_IN_EXTERNAL_HEAP = int.Parse(sr.ReadLine());
			EXTERNAL_HEAP._globaliddata = (sr.ReadLine());
			GUID = (sr.ReadLine());
		}




		public bool Equals(KeeperDataFieldValue other)
		{
			return this == other;
		}

		public static bool operator ==(KeeperDataFieldValue a, KeeperDataFieldValue b)
		{
			if ((object)a == null && (object)b == null)
			{
				return true;
			}
			if ((object)a == null || (object)b == null)
			{
				return false;
			}
			if (a.EXTERNAL_HEAP.ID_IN_EXTERNAL_HEAP == b.EXTERNAL_HEAP.ID_IN_EXTERNAL_HEAP)
			{
				return a.GUID == b.GUID;
			}
			return false;
		}

		public static bool operator !=(KeeperDataFieldValue a, KeeperDataFieldValue b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as KeeperDataFieldValue);
		}

		public override int GetHashCode()
		{
			if (EXTERNAL_HEAP.ID_IN_EXTERNAL_HEAP != -1)
			{
				return (int)(EXTERNAL_HEAP.ID_IN_EXTERNAL_HEAP % int.MaxValue);
			}
			return GUID.GetHashCode();
		}
	}


	//[Serializable]
	/*
	internal class KeeperDataItem_SerizeHelper
	{
		[SerializeField]
		internal long[] keys;

		[SerializeField]
		internal KeeperDataItem[] values;

		public KeeperDataItem_SerizeHelper()
		{
		}

		public KeeperDataItem_SerizeHelper(int count)
		{
			keys = new long[count];
			values = new KeeperDataItem[count];
		}

		public KeeperDataItem_SerizeHelper(Dictionary<long, KeeperDataItem> field_records)
			: this(field_records.Count)
		{
			Fill(field_records);
		}

		internal void Fill(Dictionary<long, KeeperDataItem> field_records)
		{
			int num = 0;
			foreach (KeyValuePair<long, KeeperDataItem> field_record in field_records)
			{
				keys[num] = field_record.Key;
				values[num] = field_record.Value;
				num++;
			}
		}
	}*/
}

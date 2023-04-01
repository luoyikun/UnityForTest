using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

namespace EMX.HierarchyPlugin.Editor
{


	[Serializable]
	public class ColorFilter : IEquatable<ColorFilter>
	{


		[SerializeField] int __ENABLE;
		[SerializeField] internal string NAME = "New Filter";
		[SerializeField] internal string NameFilter = "", ComponentFilter = "", TagFilter = "", LayerFilter = "";
		[SerializeField] internal Texture2D _icon;
		[SerializeField] internal bool hasColorText, hasColorBg, hasColorIcon/*, child*/;
		[SerializeField] internal Color colorBg = Color.white, colorText = Color.white, colorIcon = Color.white;
		[SerializeField] internal int[] __Aligment;
		[SerializeField] internal bool child;
		//	internal int[] _Aligment { get { return __Aligment ?? (__Aligment = new int[5]); } set { __Aligment = value; } }
		//internal bool child { get { return Aligment[4] == 1; } set { Aligment[4] = value ? 1 : 0; } } //set { el.SetByte(4, 0, 1,  value ? 1 : 0);} }
		internal int GetFilterByNameToCompLength { get { return 4; } }
		internal string GetFilterByNameToComp(int NameToComp)
		{
			switch (NameToComp)
			{
				case 0: return NameFilter;

				case 1: return ComponentFilter;

				case 2: return TagFilter;

				case 3: return LayerFilter;
			}

			return "";
		}
		internal bool IsNullOrEmptyGetFilterByNameToComp(int NameToComp)
		{
			switch (NameToComp)
			{
				case 0: return string.IsNullOrEmpty(NameFilter.Trim(trimChars));

				case 1: return string.IsNullOrEmpty(ComponentFilter.Trim(trimChars));

				case 2: return string.IsNullOrEmpty(TagFilter.Trim(trimChars));

				case 3: return string.IsNullOrEmpty(LayerFilter.Trim(trimChars));
			}

			return false;
		}


		internal void SaveToString(ref System.Text.StringBuilder result)
		{
			result.AppendLine(__ENABLE.ToString());
			result.AppendLine(NAME.ToString());
			result.AppendLine(NameFilter.ToString());
			result.AppendLine(ComponentFilter.ToString());
			result.AppendLine(TagFilter.ToString());
			result.AppendLine(!_icon ? "" : string.IsNullOrEmpty(AssetDatabase.GetAssetPath(_icon)) ? "" : AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_icon)));
			result.AppendLine(hasColorText.ToString()); result.AppendLine(hasColorBg.ToString()); result.AppendLine(hasColorIcon.ToString()); result.AppendLine("");
			result.AppendLine(ColorToString(ref colorBg)); result.AppendLine(ColorToString(ref colorText)); result.AppendLine(ColorToString(ref colorIcon));
			result.AppendLine(__Aligment != null ? ListToString(ref __Aligment) : "");
			result.AppendLine(LayerFilter.ToString());
		}
		internal static ColorFilter ReadFromString(ref System.IO.StringReader reader)
		{
			var result = new ColorFilter();
			//  using (var reader = new System.IO.StringReader(str))
			{
				result.__ENABLE = int.Parse(reader.ReadLine());
				result.NAME = (reader.ReadLine());
				result.NameFilter = (reader.ReadLine());
				result.ComponentFilter = (reader.ReadLine());
				result.TagFilter = (reader.ReadLine());
				var icon = reader.ReadLine();
				var path = string.IsNullOrEmpty(icon) ? "" : AssetDatabase.GUIDToAssetPath(icon);
				result._icon = string.IsNullOrEmpty(path) ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(path);
				result.hasColorText = bool.Parse(reader.ReadLine()); result.hasColorBg = bool.Parse(reader.ReadLine());
				result.hasColorIcon = bool.Parse(reader.ReadLine()); /*result.child =*/ reader.ReadLine();
				result.colorBg = ColorFromString(reader.ReadLine()); result.colorText = ColorFromString(reader.ReadLine()); 
				result.colorIcon = ColorFromString(reader.ReadLine());
				result.Aligment = Int32ListFromString(reader.ReadLine());

				try
				{
					result.LayerFilter = (reader.ReadLine());

				}

				catch
				{

				}
			}
			return result;
		}


		[NonSerialized] static Dictionary<int, int> __id = new Dictionary<int, int>();
		[NonSerialized] int? _id;
		[NonSerialized] internal static IntList __SingleList = new IntList();
		[NonSerialized] internal static TempColorClass __TempColorClass = new TempColorClass();
		[NonSerialized] static Dictionary<string, States[]> __GetAllStates = new Dictionary<string, States[]>();


		public bool Equals(ColorFilter other)
		{
			return this == other;
		}

		internal bool ENABLE
		{
			get { return __ENABLE == 0; }

			set { if (value) __ENABLE = 0; else __ENABLE = 1; }
		}
		int id
		{
			get
			{
				if (_id.HasValue) return _id.Value;

				var genV = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

				while (__id.ContainsKey(genV)) genV = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

				_id = genV;
				__id.Add(genV, genV);
				return _id.Value;
			}
		}
		public static bool operator ==(ColorFilter a, ColorFilter b)
		{
			if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) return true;

			if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;

			return a.id == b.id;
		}
		public static bool operator !=(ColorFilter a, ColorFilter b)
		{
			return !(a == b);
		}
		public override bool Equals(object obj)
		{
			return this == (obj as ColorFilter);
		}
		public override int GetHashCode()
		{
			return id;
		}

		[NonSerialized] public const string STARTWITH = "<";
		[NonSerialized] public const string ENDWITH = ">";
		[NonSerialized] public const string AND = "+";
		[NonSerialized] public const string OR = "`";
		[NonSerialized] public const string NOT = "!";
		[NonSerialized] public const string IGNORECASE = "*";
		[NonSerialized] public const string ACTIVE = "╬@╥";
		[NonSerialized] public const char SEPARATOR = '╥';
		static char[] trimChars =
			STARTWITH.ToCharArray().Concat(
				ENDWITH.ToCharArray().Concat(
					AND.ToCharArray().Concat(
						OR.ToCharArray().Concat(
							NOT.ToCharArray().Concat(
								IGNORECASE.ToCharArray().Concat(
									ACTIVE.ToCharArray().Concat(new[] { SEPARATOR, ' ', '\n' }
															   ))))))).ToArray();


		internal int[] Aligment
		{
			get
			{
				if (__Aligment == null) __Aligment = new int[0];
				if (__Aligment.Length < 5) Array.Resize(ref __Aligment, 5);//  _Aligment.AddRange(Enumerable.Repeat(0, 5));
				return __Aligment;
			}
			set
			{
				__Aligment = value;
			}
		}


		internal bool LABEL_SHADOW
		{
			get
			{
				__SingleList.list = Aligment;
				return __SingleList.GetByte(10, 0, 1) == 1;
			}

			set
			{
				__SingleList.list = Aligment;
				__SingleList.SetByte(10, 0, 1, value ? 1 : 0);
			}
		}

		internal int BG_HEIGHT
		{
			get
			{
				__SingleList.list = Aligment;
				return __SingleList.GetByte(9, 7, 2);
			}

			set
			{
				__SingleList.list = Aligment;
				__SingleList.SetByte(9, 7, 2, value);
			}
		}


		internal TempColorClass AS_TEMPCOLOR_ALIGN_ONLY
		{
			get
			{
				var asd = Aligment.Select(c => c.ToString()).ToArray();
				__TempColorClass.AssignFromList(ref asd);
				__TempColorClass.BGCOLOR = hasColorBg ? colorBg : Color.clear;
				__TempColorClass.LABELCOLOR = hasColorText ? colorText : Color.clear;
				__TempColorClass.child = child;
				// __TempColorClass.child = child;

				__TempColorClass.add_icon = _icon;
				__TempColorClass.add_hasiconcolor = hasColorIcon;

				if (__TempColorClass.add_hasiconcolor)
					__TempColorClass.add_iconcolor = colorIcon;
				else
					__TempColorClass.add_iconcolor = Color.clear;

				return __TempColorClass;
			}

			set
			{
				string asd = "";
				foreach (var item in value.el) asd += " " + item ?? "null";
				//Debug.Log(asd);
				if (__Aligment == null) __Aligment = new int[0];
				if (__Aligment.Length < value.el.Length) Array.Resize(ref __Aligment, value.el.Length);
				__Aligment[9] = value.el[9] == null || value.el[9] == "" ? 0 : int.Parse(value.el[9]);
				__Aligment[10] = value.el[10] == null || value.el[10] == "" ? 0 : int.Parse(value.el[10]);
				//__Aligment = value.el.Select(s => s == null || s == "" ? 0 : int.Parse(s)).ToArray();
				child = value.child;
				colorBg = value.BGCOLOR;
				hasColorBg = value.HAS_BG_COLOR;
				colorText = value.LABELCOLOR;
				hasColorText = value.HAS_LABEL_COLOR;
				// child = value.child;

				hasColorIcon = value.add_hasiconcolor;
				_icon = value.add_icon;
				colorIcon = value.add_iconcolor;
			}
		}

		public class States : ICloneable
		{
			public States() { }
			public States(bool AND,
						   bool STARTWITH,
						   bool ENDWITH,
						   bool OR,
						   bool NOT,
						   bool IGNORECASE,
						   string filter)
			{
				this.AND = AND;
				this.STARTWITH = STARTWITH;
				this.ENDWITH = ENDWITH;
				this.OR = OR;
				this.NOT = NOT;
				this.IGNORECASE = IGNORECASE;
				this.filter = filter;
			}

			public bool STARTWITH { get; private set; }
			public bool ENDWITH { get; private set; }
			bool __AND = true;
			public bool AND { get { return __AND; } private set { __AND = value; } }
			public bool OR { get; private set; }
			public bool NOT { get; private set; }
			public bool IGNORECASE { get; private set; }
			public string filter { get; private set; }

			public enum Compar { Contains, StartWith, EndWith, Equals }
			public Compar GetCompar
			{
				get
				{
					if (!STARTWITH && !ENDWITH) return Compar.Contains;

					if (STARTWITH && !ENDWITH) return Compar.StartWith;

					if (!STARTWITH && ENDWITH) return Compar.EndWith;

					return Compar.Equals;
				}
			}

			object ICloneable.Clone()
			{
				return MemberwiseClone();
			}
			public States Clone()     //var res// = new State();
			{
				return MemberwiseClone() as ColorFilter.States;
			}
			public States SWAP_STARTWITH(bool v) { var s = Clone(); s.STARTWITH = v; return s; }
			public States SWAP_ENDWITH(bool v) { var s = Clone(); s.ENDWITH = v; return s; }
			public States SWAP_AND(bool v) { var s = Clone(); s.AND = v; return s; }
			public States SWAP_OR(bool v) { var s = Clone(); s.OR = v; return s; }
			public States SWAP_NOT(bool v) { var s = Clone(); s.NOT = v; return s; }
			public States SWAP_IGNORECASE(bool v) { var s = Clone(); s.IGNORECASE = v; return s; }


			public string ConvertToString()
			{
				var result = filter;

				if (STARTWITH) result = ColorFilter.STARTWITH + SEPARATOR + result;

				if (ENDWITH) result = ColorFilter.ENDWITH + SEPARATOR + result;

				if (AND) result = ColorFilter.AND + SEPARATOR + result;

				if (OR) result = ColorFilter.OR + SEPARATOR + result;

				if (NOT) result = ColorFilter.NOT + SEPARATOR + result;

				if (IGNORECASE) result = ColorFilter.IGNORECASE + SEPARATOR + result;

				return ColorFilter.ACTIVE + result;
			}
			const string asdas =  "...";
			internal string GetComparationString()
			//  {   return !STARTWITH && !ENDWITH ? "=..A.." : STARTWITH && !ENDWITH ? "=A..." : ENDWITH && !STARTWITH ? "=...A" : " == " ;
			{
				return !STARTWITH && !ENDWITH ? "\" "+asdas+" A "+asdas+" \"" : STARTWITH && !ENDWITH ? "\" A "+asdas+" \"" : ENDWITH && !STARTWITH ? "\" "+asdas+" A \"" : "\" A \"";
				//return !STARTWITH && !ENDWITH ? "\"__ A __\"" : STARTWITH && !ENDWITH ? "\"A ___\"" : ENDWITH && !STARTWITH ? "\"___ A\"" : "\"A\"";
				//return !STARTWITH && !ENDWITH ? "__ A __" : STARTWITH && !ENDWITH ? "A ___" : ENDWITH && !STARTWITH ? "___ A" : "A";
			}

			internal string GetConditionString()
			//  {   return OR ? "||" : "&&";
			{
				return !AND ? "[OR]" : "[AND]";
			}


		}

		public States[] AllStatesForName { get { return (GetAllStates(NameFilter)); } }
		public States[] AllStatesForComps { get { return (GetAllStates(ComponentFilter)); } }
		public States[] AllStatesForLayerss { get { return (GetAllStates(LayerFilter)); } }
		public States[] AllStatesForTagss { get { return (GetAllStates(TagFilter)); } }
		public States[] GetAllStates(string text)
		{
			if (__GetAllStates.ContainsKey(text)) return __GetAllStates[text];

			var conditions = text.Split('╩').Select(c => c == null ? "" : c).ToArray();

			if (conditions.Length == 0) return new States[1] { new States() };

			var result = new States[conditions.Length];

			for (int i = 0; i < conditions.Length; i++)
			{
				var _case = conditions[i].Split(SEPARATOR);

				if (_case.Length == 0) _case = new string[1] { "" };

				var or = _case.Any(s => s == OR);

				var state = new States(
					AND: (_case.Any(s => s == AND) || !or),
					STARTWITH: _case.Any(s => s == STARTWITH),
					ENDWITH: _case.Any(s => s == ENDWITH),
					OR: or,
					NOT: _case.Any(s => s == NOT),
					IGNORECASE: _case.Any(s => s == IGNORECASE),
					filter: _case.Last()
				);

				result[i] = state;
			}

			return result;
		}



























		internal static string ColorToString(ref Color __c)
		{
			Color32 c = __c;
			return c.r + " " + c.g + " " + c.b + " " + c.a;
		}
		internal static string ColorToString(Color __c)
		{
			Color32 c = __c;
			return c.r + " " + c.g + " " + c.b + " " + c.a;
		}
		internal static Color ColorFromString(string s)
		{
			var a = s.Split(' ');
			return new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), byte.Parse(a[3]));
		}
		internal static string ListToString<T>(ref List<T> __c)
		{
			var result = new System.Text.StringBuilder();
			for (int i = 0; i < __c.Count; i++)
			{
				if (i != 0) result.Append(' ');
				result.Append(__c[i].ToString());
			}
			return result.ToString();
		}
		internal static string ListToString<T>(ref T[] __c)
		{
			var result = new System.Text.StringBuilder();
			for (int i = 0; i < __c.Length; i++)
			{
				if (i != 0) result.Append(' ');
				result.Append(__c[i].ToString());
			}
			return result.ToString();
		}
		internal static int[] Int32ListFromString(string s)
		{
			var a = s.Split(' ');
			var result = new int[a.Length];

			for (int i = 0; i < a.Length; i++)
			{
				if (string.IsNullOrEmpty(a[i])) continue;
				result[i] = int.Parse(a[i]);
				//result.Add(int.Parse(a[i]));
			}

			return result;
		}


	}





	[Serializable]
	public class IntList
	{
		public int[] list = new int[0];

		public IntList()
		{
		}

		public IntList(float[] source)
		{
			list = source.Select((float s) => (int)(s * 255f)).ToArray();
		}

		public int GetElement(int index, int? defaultValue = null)
		{
			if (index < list.Length)
			{
				return list[index];
			}
			return defaultValue ?? 0;
		}

		public int GetByte(int index, int offset, int length, int? defaultValue = null)
		{
			return (GetElement(index, defaultValue) >> offset) & ~(-1 << length);
		}

		public void SetByte(int index, int offset, int length, int Value, int? defaultValue = null)
		{
			int element = GetElement(index, defaultValue);
			element &= ~(~(-1 << length) << offset);
			Value &= ~(-1 << length);
			element |= Value << offset;
			if (index >= list.Length)
			{
				if (index >= list.Length) Array.Resize(ref list, index + 1);
				/*while (index >= list.Length)
				{
					list.Add(0);
				}*/
			}
			list[index] = element;
		}

		static StringBuilder sb = new StringBuilder();
		public override string ToString()
		{
			sb.Clear();
			for (int i = 0; i < list.Length; i++)
			{
				if (i != 0) sb.Append(' ');
				sb.Append(list[i]);
			}
			return sb.ToString();
		}

		public IntList Clone()
		{
			return new IntList
			{
				list = list.ToArray()
			};
		}
	}
}

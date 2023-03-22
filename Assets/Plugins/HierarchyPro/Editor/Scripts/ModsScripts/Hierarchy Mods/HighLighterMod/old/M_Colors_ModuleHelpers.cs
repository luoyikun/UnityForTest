using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor.Mods
{
	internal partial class HighlighterMod : DrawStackAdapter, IModSaver
	{





		internal Type t_GameObject = typeof(GameObject);
		internal Texture __NullContext;

		internal Texture NullContext
		{
			get
			{
				return __NullContext ?? (__NullContext = TODO_Tools.GetObjectBuildinIcon( t_GameObject).add_icon);
			}
		}


		// static class GLD {
		static internal void GL_DrawTexture(Rect rect, Texture tex, ScaleMode scale, bool alpha, float ascpect, Color color,
											float roderWidth, float borderRadius)
		{
			GL_DrawTexture(rect, (Texture2D)tex, scale, alpha, ascpect, color, roderWidth, borderRadius);
		}

		static internal void GL_DrawTexture(Rect rect, Texture2D tex, ScaleMode scale, bool alpha, float ascpect,
											Color color, float roderWidth, float borderRadius)
		{
			if (Event.current.type == EventType.Repaint)
			{
				if (glStackPos >= glStack.Count) glStack.Add(new GL_STACK());

				glStack[glStackPos].color = color;
				glStack[glStackPos].rect = rect;
				glStack[glStackPos].texture = tex;
				glStackPos++;
			}
		}

		internal class GL_STACK
		{
			internal Color color;
			internal Rect rect;
			internal Texture2D texture;
		}
		static int glStackPos = 0;
		static List<GL_STACK> glStack = new List<GL_STACK>(500);



		internal class ArrayPrefs
		{
			internal ArrayPrefs(string key)
			{
				this.key = key + " ";
			}

			string key;

			internal List<int> Value
			{
				get
				{
					var result = new List<int>();
					int i = 0;
					int value;

					while ((value = EditorPrefs.GetInt(key + i, -1)) != -1)
					{
						result.Add(value);
						i++;
					}

					return result;
				}

				set
				{
					int i = 0;

					while (EditorPrefs.GetInt(key + i, -1) != -1)
					{
						EditorPrefs.DeleteKey(key + i);
						i++;
					}

					i = 0;

					foreach (var item in value)
					{
						EditorPrefs.SetInt(key + i, item);
						i++;
					}
				}
			}
		}


		/*static int[] tempI = new int[10];
		// static Color32 tempC = new Color32();

		static Color32[] result = new Color32[1];
		static public Color32[] String4ToColor(string[] res)
		{

		   //byte[] result = new byte[res.Length];
		   bool error = false;
		   for (int i = 0 ; i < res.Length ; i++)
		   {   int parse;
			   if (!int.TryParse( res[i], out parse ))
			   {   error = true;
				   break;
			   }
			   tempI[i] = parse;
		   }

		   if (!error && res.Length >= 4)
		   {   result[0].r = (byte)tempI[0];
			   result[0].g = (byte)tempI[1];
			   result[0].b = (byte)tempI[2];
			   result[0].a = (byte)tempI[3];

			   if (res.Length >= 9)
			   {   if (result.Length != 2) Array.Resize( ref result, 2 );
				   result[1].r = (byte)tempI[5];
				   result[1].g = (byte)tempI[6];
				   result[1].b = (byte)tempI[7];
				   result[1].a = (byte)tempI[8];
			   }

			   return result;
			   //if (list.Count < res.Length) list.AddRange(Enumerable.Repeat(0, res.Length - list.Count));
			   // for (int i = 0; i < tempI.Length; i++)
			   //    list[i] = tempI[i];
		   }

		   return null;
		}*/







		//	static TempColorClass temp = new TempColorClass().empty;
		//
		//	static public TempColorClass String4ToColor(string[] res)
		//	{
		//		var list = String4ToList(res);
		//		var el = new SingleList();
		//		el.list = list;
		//		temp.AssignFromList(el);
		//		return temp;
		//	}
		//
		//	static public List<int> String4ToList(string[] res)
		//	{   //byte[] result = new byte[res.Length];
		//		bool error = false;
		//		List<int> result = new List<int>();
		//
		//		for (int i = 0; i < res.Length; i++)
		//		{
		//			int parse;
		//
		//			if (!int.TryParse(res[i], out parse))
		//			{
		//				error = true;
		//				break;
		//			}
		//
		//			result.Add(parse);
		//		}
		//
		//		if (!error)
		//		{
		//			return result;
		//			//if (list.Count < res.Length) list.AddRange(Enumerable.Repeat(0, res.Length - list.Count));
		//			// for (int i = 0; i < tempI.Length; i++)
		//			//    list[i] = tempI[i];
		//		}
		//
		//		return null;
		//	}
		//
		//	static public bool StringToBool(int index, string[] res)
		//	{
		//		if (res.Length <= index) return false;
		//
		//		return res[index] == "1";
		//	}









	}
}
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Collections.Concurrent;

namespace EMX.HierarchyPlugin.Editor
{

	partial class Tools
	{
		static Type serType2 = typeof(System.NonSerializedAttribute);
		static Type serType = typeof(SerializeField);
		internal static Type unityMonoBehaviour = typeof(UnityEngine.MonoBehaviour);
		internal static Type unityGameObjectType = typeof(UnityEngine.GameObject);
		internal static Type unityObjectType = typeof(UnityEngine.Object);
		private static BindingFlags flags = ~BindingFlags.Static;

		class FieldsLocker
		{
			internal ConcurrentDictionary<string, FieldAdapter> data = new ConcurrentDictionary<string, FieldAdapter>();
			internal object lockedObject = new object();
		}

		static ConcurrentDictionary<Type, FieldsLocker> _SCAN_FIELDS_CACHE = new ConcurrentDictionary<Type, FieldsLocker>();
		static ConcurrentDictionary<Type, FieldsLocker> _SCAN_FIELDS_CACHE_NOARRAYS = new ConcurrentDictionary<Type, FieldsLocker>();
		static ConcurrentDictionary<Type, Type> genericTypeCache = new ConcurrentDictionary<Type, Type>();
		static Type additionalSkipper = typeof(IEnumerable);
		static Type arrayType = typeof(Array);

		/* interface IName {
             string Name { get; }
             object GetValue( object o );
         }*/

		public class FieldAdapter
		{
			//UnityEngine.Events.UnityEvent
			bool isObject;
			bool isField;
			FieldInfo f;
			PropertyInfo p;
			// public Type ObjectType = null;
			public string Name;
			//public FieldAdapter[] childFields;
			bool isClass;
			public ConcurrentDictionary<string, FieldAdapter> childFields;
			bool isEnumerable;
			bool isList;
			//bool UnityEventMarker;
			static bool? assignFromArray;
			Dictionary<string, object> fastDic;

			public static FieldAdapter TryToCreate(FieldInfo f, bool includeArrays)
			{   /* isField = true;
			 this.f = f;
			 Name = f.Name;*/
				var res = _constructor(f.FieldType, includeArrays);

				if (ReferenceEquals(res, null)) return null;

				res.isField = true;
				res.f = f;
				res.Name = f.Name;
				return res;
			}
			public static FieldAdapter TryToCreate(PropertyInfo p, bool includeArrays)
			{   /* isField = false;
			 this.p = p;
			 Name = p.Name;*/
				var res = _constructor(p.PropertyType, includeArrays);

				if (ReferenceEquals(res, null)) return null;

				res.isField = false;
				res.p = p;
				res.Name = p.Name;
				return res;
			}

			static bool HasUnityEvent(ref Dictionary<string, FieldAdapter> arr)
			{
				return arr.Any(c => c.Value.f != null && Root.UnityEventArgsType == c.Value.f.FieldType);
			}
			static FieldAdapter _constructor(Type type, bool includeArrays /*, GET_FIELDS_TYPE searchType*/ )
			{

				if (type.IsPrimitive)
				{
					return null;
				}

				if (unityObjectType.IsAssignableFrom(type))
				{
					var res = new FieldAdapter();
					res.isObject = true; // OBJECT
					return res;
					//ObjectType = type;
				}

				else
				{

					assignFromArray = null;

					if (type.IsSerializable || (assignFromArray ?? (assignFromArray = arrayType.IsAssignableFrom(type)).Value))
					{
						if (!type.IsGenericType && !(assignFromArray ?? (assignFromArray = arrayType.IsAssignableFrom(type)).Value))   //CLASS
						{
							var childFields = GET_FIELDS(type);

							if (childFields.Count == 0) return null;

							//  ObjectType = type;
							var res = new FieldAdapter();
							res.isClass = true;
							res.childFields = childFields;
							/* if ( searchType == GET_FIELDS_TYPE.UnityEvent && !res.UnityEventMarker ) {
                                 res.UnityEventMarker = HasUnityEvent( ref childFields );
                                 if ( res.UnityEventMarker ) Debug.Log( "ASD" );
                             }*/
							return res;
						}

						else
						{
							if (!includeArrays)
							{
								return null;
							}

							else
								if (assignFromArray ?? (assignFromArray = arrayType.IsAssignableFrom(type)).Value)     //[]
							{
								if (type.GetArrayRank() == 1)
								{   // isEnumerable = true;
									var elType = type.GetElementType();

									if (unityObjectType.IsAssignableFrom(elType))
									{
										var res = new FieldAdapter();
										res.isObject = true; // OBJECT
										res.isEnumerable = true; // OBJECT
										return res;
									}

									else
										if (additionalSkipper.IsAssignableFrom(elType))
									{
										return null;
									}

									else
									{
										var childFields = GET_FIELDS(/* ObjectType =*/ elType);

										if (childFields.Count == 0) return null;

										var res = new FieldAdapter();
										res.childFields = childFields;
										res.isEnumerable = true; // OBJECT
										/* if ( searchType == GET_FIELDS_TYPE.UnityEvent && !res.UnityEventMarker ) {
                                             res.UnityEventMarker = HasUnityEvent( ref childFields );
                                             if ( res.UnityEventMarker ) Debug.Log( "ASD" );
                                         }*/
										return res;
									}

								}

								else
								{
									return null;
								}
							}

							else
							{
								if (!genericTypeCache.ContainsKey(type))
								{
									genericTypeCache.TryAdd(type, (typeof(List<>).MakeGenericType(type.GetGenericArguments().First(g => !g.IsGenericParameter))));
								}

								if (genericTypeCache[type].IsAssignableFrom(type))   //LIST
								{   // isEnumerable = true;
									var elType = type.GetGenericArguments().First(g => !g.IsGenericParameter);

									if (unityObjectType.IsAssignableFrom(elType))
									{
										var res = new FieldAdapter();
										res.isObject = true; // OBJECT
										res.isEnumerable = true; // OBJECT
										res.isList = true;
										return res;
									}

									else
										if (additionalSkipper.IsAssignableFrom(elType))
									{
										return null;
									}

									else
									{
										var childFields = GET_FIELDS( /*ObjectType = */elType);

										if (childFields.Count == 0) return null;

										var res = new FieldAdapter();
										res.childFields = childFields;
										res.isEnumerable = true;
										res.isList = true;
										// OBJECT
										/* if ( searchType == GET_FIELDS_TYPE.UnityEvent && !res.UnityEventMarker ) {
                                             res.UnityEventMarker = HasUnityEvent( ref childFields );
                                             if ( res.UnityEventMarker ) Debug.Log( "ASD" );
                                         }*/
										return res;
									}
								}

								else
								{
									return null;
								}
							}
						}
					}

					else
					{
						return null;
					}

				}

				/* if ()
                 else if ( p.PropertyType.I )*/
			}


			object res = null;

			public Dictionary<int, Dictionary<string, object>> GetAllValuesCache = null;
			int oldSelID = -1;
			internal void CheckID(int sel_id, int SEK_INC)
			{
				if (oldSelID == (sel_id ^ SEK_INC)) return;

				oldSelID = sel_id ^ SEK_INC;

				if (GetAllValuesCache != null) GetAllValuesCache.Clear();
			}

			static Dictionary<string, object> emptyObject = new Dictionary<string, object>();
			// static object[] singleObject = new object[1];
			public Dictionary<string, object> GetAllValues(object o, int deep, int searchType)
			{
				if (deep == 20)
				{
					return emptyObject;
				}

				deep++;
				// Debug.Log( o.GetType().FullName + " " + Name );
				res = GetValue(o);
				/*if ( isField ) res = f.GetValue( o );
                else res = p.GetValue( o , null );*/
				// if ( Name == "GAGAGA" ) Debug.Log( "ASD" );


				if (Tools.IsObjectNull(res) && (!isObject || (searchType & 4) == 0)) return emptyObject;

				if (isEnumerable)
				{
					if ((searchType & 2) != 0) return emptyObject;

					var dic = new Dictionary<string, object>();
					int i = 0;

					foreach (var item in (IEnumerable)res)
					{
						if (!isObject)
						{
							foreach (var a in childFields.Values.SelectMany(c => c.GetAllValues(item, deep, searchType)))
							{
								dic.Add(Name + '#' + i + "#/" + a.Key, a.Value);
							}
						}

						else
						{
							if ((searchType & 1) == 0)
								dic.Add(Name + '#' + i + '#', item as UnityEngine.Object);
						}

						i++;
					}

					if (isObject && (searchType & 1) != 0) dic = dic.Where(d => d.Key.EndsWith("/m_Target") &&
							 d.Key.Contains('#') && d.Key.Remove(d.Key.LastIndexOf('#', d.Key.LastIndexOf('#') - 1)).EndsWith("/m_Calls")).ToDictionary(d => d.Key, d => d.Value);

					return dic;
				}

				if (isObject)
				{
					if (deep == 1 && (searchType & 1) != 0)
					{
						return emptyObject;
					}

					if (fastDic == null)
					{
						fastDic = new Dictionary<string, object>();
						fastDic.Add(Name, null);
					}

					fastDic[Name] = res;
					return fastDic;
				}

				if (isClass)
				{
					var dic = childFields.Values.SelectMany(c => c.GetAllValues(res, deep, searchType))
						  .ToDictionary(v2 => Name + '/' + v2.Key, v2 => v2.Value);

					/* foreach ( var item in dic ) {
                         Debug.Log( item.Key );

                     }*/
					if ((searchType & 1) != 0) dic = dic.Where(d => d.Key.EndsWith("m_Target") &&
														d.Key.Contains('#') && d.Key.Remove(d.Key.LastIndexOf('#', d.Key.LastIndexOf('#') - 1)).EndsWith("/m_Calls")).ToDictionary(d => d.Key, d => d.Value);

					return dic;
				}

				throw new Exception("Unknown field type");
			}


			public struct ArrayKey
			{
				public object ArrayObject;
				// public string ArrayFieldName;
				public int ArrayIndex;
				public List<string> ChildFieldsNames;

				public FieldKey ToFieldKey()
				{
					var res = new FieldKey();
					res.ChildFieldsNames = ChildFieldsNames.ToList();
					res.Value = ArrayObject;
					return res;
				}
			}
			public struct FieldKey
			{
				public List<string> ChildFieldsNames;
				public object Value;
				public FieldKey Clone()
				{
					var resuslt = this;
					resuslt.ChildFieldsNames = ChildFieldsNames.ToList();
					return resuslt;
				}
				public FieldKey TrimFirst()
				{
					ChildFieldsNames.RemoveAt(0);
					return this;
				}
			}
			public void SetAllValues(object o, Dictionary<string, object> values)
			{
				var converted = values.Select(v => new FieldKey() { ChildFieldsNames = v.Key.Split('/').ToList(), Value = v.Value }).ToArray();
				SetAllValues(o, converted);
			}
			public object SetAllValues(object o, FieldKey[] converted)
			{

				/*if ( Name == "as44dasd" )
                    Debug.Log( converted.Length );*/

				if (converted.Length == 0) return o;

				/*foreach ( var item in converted ) {
                    Debug.Log( Name + " " + (item.ChildFieldsNames.Count == 0 ? "0" : item.ChildFieldsNames.Aggregate( ( a , b ) => a + "/" + b ) ));
                }*/

				if (!isEnumerable)
				{
					if (isObject)
					{
						if (converted.Length != 1) throw new Exception("SetAllValues isObject Length != 1");

						SetValue(o, converted[0].Value);
						return o;
					}

					if (isClass)
					{
						var resClass = GetValue(o);

						foreach (var cf in childFields.Values)
						{
							var newConv = converted.Where(c => c.ChildFieldsNames.Count > 1 &&
													  (c.ChildFieldsNames[1][c.ChildFieldsNames[1].Length - 1] == '#' && c.ChildFieldsNames[1].Remove(c.ChildFieldsNames[1].IndexOf('#')) == cf.Name ||
													   c.ChildFieldsNames[1] == cf.Name)
													 ).Select(c => c.Clone().TrimFirst()).ToArray();
							var _resClass = cf.SetAllValues(resClass, newConv);

							/* Debug.Log( "----" );
                             foreach ( var item in newConv ) {
                                 Debug.Log( cf.Name + " " + (item.ChildFieldsNames.Count == 0 ? "0" : item.ChildFieldsNames.Aggregate( ( a , b ) => a + "/" + b )) );
                             }*/
#if GETFIELDS_CHECKERRORS
						
						if ( _resClass.GetType() != resClass.GetType() ) throw new Exception( "Type missmatch " + _resClass.GetType().Name + " " + resClass.GetType().Name );
						
#endif
							resClass = _resClass;
						}

						/* var dic = childFields.Values.SelectMany( c => c.GetAllValues( res , deep , searchType ) )
                         .ToDictionary( v2 => Name + '/' + v2.Key , v2 => v2.Value );*/
						SetValue(o, resClass);
						return o;
					}

					throw new Exception("Unknown field type");
				}

				var resArray = GetValue(o);

				var newConverted = new ArrayKey[converted.Length];
				var dicArrayKeys = new Dictionary<int, List<ArrayKey>>();

				for (int i = 0; i < converted.Length; i++)
				{
					if (converted[i].ChildFieldsNames.Count == 0) continue;

					/* var c = converted[i];
                     if (
                             (c.ChildFieldsNames[0][c.ChildFieldsNames[0].Length - 1] == '#' && c.ChildFieldsNames[0].Remove( c.ChildFieldsNames[0].IndexOf( '#' ) ) == Name ||
                             c.ChildFieldsNames[0] == Name) ) {

                         if ( c.ChildFieldsNames.Count < 2 ) throw new Exception( "ChildFieldsNames.Count < 2" );*/
					var field = converted[i].ChildFieldsNames[0];
					int ind = -1;

					if (field[field.Length - 1] == '#')
					{
						var R1 = field.IndexOf('#');
						var indS = field.Substring(R1 + 1);
						ind = int.Parse(indS.Remove(indS.Length - 1));
						//field = field.Remove( R1 );
					}

					newConverted[i] = new ArrayKey();
					newConverted[i].ArrayObject = converted[i].Value;
					// newConverted[i].ArrayFieldName = field;
					newConverted[i].ArrayIndex = ind;
					newConverted[i].ChildFieldsNames = converted[i].ChildFieldsNames.ToList();
					// newConverted[i].ChildFieldsNames.RemoveAt( 0 );

					if (!dicArrayKeys.ContainsKey(ind)) dicArrayKeys.Add(ind, new List<ArrayKey>());

					dicArrayKeys[ind].Add(newConverted[i]);
					// }

				}

				// Debug.Log( converted[0].ChildFieldsNames[0] );

				if (isList)
				{
					var arr = resArray as IList;

					if (isObject)
					{
						for (int i = 0; i < arr.Count; i++)
						{
							arr[i] = null;
						}
					}

					foreach (var dicKey in dicArrayKeys)
					{
						var i = dicKey.Key;

						if (i >= arr.Count) continue;

						var resClass = arr[i];

						if (!isObject)
						{
							var sendArgs = dicKey.Value.Select(d => d.ToFieldKey()).ToArray();

							foreach (var cf in childFields.Values)
							{
								var _sendArgs = sendArgs.Where(c => c.ChildFieldsNames.Count > 1 &&
														   (c.ChildFieldsNames[1][c.ChildFieldsNames[1].Length - 1] == '#' && c.ChildFieldsNames[1].Remove(c.ChildFieldsNames[1].IndexOf('#')) == cf.Name ||
															c.ChildFieldsNames[1] == cf.Name)
														  ).Select(c => c.Clone().TrimFirst()).ToArray();
								var _resClass = cf.SetAllValues(resClass, _sendArgs);
#if GETFIELDS_CHECKERRORS
							
							if ( _resClass.GetType() != resClass.GetType() ) throw new Exception( "Type missmatch " + _resClass.GetType().Name + " " + resClass.GetType().Name );
							
#endif
								resClass = _resClass;
							}
						}

						else
						{
							var _resClass = dicKey.Value[0].ArrayObject as UnityEngine.Object;
#if GETFIELDS_CHECKERRORS
						//if ( _resClass.GetType() != resClass.GetType() ) throw new Exception( "Type missmatch " + _resClass.GetType().Name + " " + resClass.GetType().Name );
#endif
							resClass = _resClass;
							// dic.Add( Name + "/#" + i , item as UnityEngine.Object );
						}

						arr[i] = resClass;
					}

					SetValue(o, arr);
					return o;
				}

				else
				{
					var arr = resArray as Array;

					if (isObject)
					{
						for (int i = 0; i < arr.Length; i++)
						{
							arr.SetValue(null, i);
						}
					}

					foreach (var dicKey in dicArrayKeys)
					{
						var i = dicKey.Key;

						if (i >= arr.Length) continue;

						var resClass = arr.GetValue(i);

						if (!isObject)
						{
							var sendArgs = dicKey.Value.Select(d => d.ToFieldKey()).ToArray();

							foreach (var cf in childFields.Values)
							{
								var _sendArgs = sendArgs.Where(c => c.ChildFieldsNames.Count > 1 &&
														   (c.ChildFieldsNames[1][c.ChildFieldsNames[1].Length - 1] == '#' && c.ChildFieldsNames[1].Remove(c.ChildFieldsNames[1].IndexOf('#')) == cf.Name ||
															c.ChildFieldsNames[1] == cf.Name)
														  ).Select(c => c.Clone().TrimFirst()).ToArray();
								var _resClass = cf.SetAllValues(resClass, _sendArgs);

								/*  Debug.Log( "----" );
                                  foreach ( var item in _sendArgs ) {
                                      Debug.Log( cf.Name + " " + (item.ChildFieldsNames.Count == 0 ? "0" : item.ChildFieldsNames.Aggregate( ( a , b ) => a + "/" + b )) );
                                  }*/
#if GETFIELDS_CHECKERRORS
							
							if ( _resClass.GetType() != resClass.GetType() ) throw new Exception( "Type missmatch " + _resClass.GetType().Name + " " + resClass.GetType().Name );
							
#endif
								resClass = _resClass;
								/* Debug.Log( Name + " "  + _sendArgs.Length );
                                 Debug.Log( converted[0].ChildFieldsNames[0] );
                                 Debug.Log( converted[0].ChildFieldsNames[1] );*/

							}
						}

						else
						{
							var _resClass = dicKey.Value[0].ArrayObject as UnityEngine.Object;
#if GETFIELDS_CHECKERRORS
						// if ( _resClass.GetType() != resClass.GetType() ) throw new Exception( "Type missmatch " + _resClass.GetType().Name + " " + resClass.GetType().Name );
#endif
							resClass = _resClass;
							// dic.Add( Name + "/#" + i , item as UnityEngine.Object );
						}

						arr.SetValue(resClass, i);
					}

					SetValue(o, arr);
					return o;
				}

				/*var asda =  (IEnumerable)resArray;

                foreach ( var item in (IEnumerable)resArray ) {
                    if ( !isObject ) {
                        foreach ( var a in childFields.Values.SelectMany( c => c.GetAllValues( item , deep , searchType ) ) ) {
                            dic.Add( Name + "/#" + i + '#' + a.Key , a.Value );
                        }
                    } else {
                        dic.Add( Name + "/#" + i , item as UnityEngine.Object );
                    }
                    i++;
                }*/

				//throw new Exception( "Unknown field type" );

			}



			public object GetValue(object o)
			{   //  if ( !isObject ) return null;
				if (isField) res = f.GetValue(o);
				else res = p.GetValue(o, null);

				// if ( isObject )
				return res;
			}
			public void SetValue(object o, object value)
			{   //if ( !isObject ) return;
				if (isField) f.SetValue(o, value);
				else p.SetValue(o, value, null);
			}



		}

		internal static int GET_SELECTION_STATE(bool isselected, bool IsSelectablePlus,bool IsSelectableMinus)
		{
			var STATE = 0;
			var mayMinus = false;

			var selected = false;

			if (Event.current.control)
			{
				selected = isselected;
			}

			// var selected = IsSelectedHadrScan();


			if (IsSelectablePlus)
				if ((Event.current.control && !selected || Event.current.shift))
					STATE = 1;
				else
					mayMinus = true;

			if (IsSelectableMinus && mayMinus)
				if (Event.current.control && selected)
					STATE = 2;

			if (Event.current.alt) STATE = 3;

			return STATE;
		}

		internal static void ROUND_RECT(ref Rect rect)
		{
			rect.x = Mathf.FloorToInt(rect.x);
			rect.y = Mathf.FloorToInt(rect.y);
			rect.width = Mathf.FloorToInt(rect.width);
			rect.height = Mathf.FloorToInt(rect.height);
		}
	}
}

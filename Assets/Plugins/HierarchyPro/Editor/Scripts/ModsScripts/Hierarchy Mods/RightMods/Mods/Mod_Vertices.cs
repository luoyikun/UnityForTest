//#define BROADCAST

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor.Sprites;
using UnityEngine.Profiling;
using System.Globalization;

namespace EMX.HierarchyPlugin.Editor.Mods
{



	internal class Mod_Vertices : RightModBaseClass
	{


		static Dictionary<Scene, Mod_VerticesHelper> d = new Dictionary<Scene, Mod_VerticesHelper>();


		public Mod_Vertices( int restWidth, int sib, bool enable, PluginInstance adapter ) : base( restWidth, sib, enable, adapter )
		{
			Clear();
		}

		internal static Mod_VerticesHelper GetDescript()
		{
			if ( !d.ContainsKey( EditorSceneManager.GetActiveScene() ) ) d.Add( EditorSceneManager.GetActiveScene(), new Mod_VerticesHelper() );

			return d[ EditorSceneManager.GetActiveScene() ];
		}
		static GUIContent content = new GUIContent();

		internal override bool USE_CONTENT_SHRINKING()
		{
			return true;
		}

		internal override void Subscribe( EditorSubscriber sbs )
		{
			sbs.OnUpdate += CalcBroadCast;
			sbs.OnHierarchyChanged += upbcs;
			sbs.OnUndoAction += upbcs;
			sbs.OnAssetImport += upbcs;
			sbs.OnPlayModeStateChanged += upbcs;
			mem = 0;
			changed++;
			// base.Subscribe( sbs );
		}

#pragma warning disable
		static int mem = 0;
		static int changed = 0;
#pragma warning restore
		void upbcs()
		{
			mem = changed++;
		}

		internal override void ModuleCommonGenericMenu( GenericMenu menu, GameObject activeGo, object _c, string sub = "" )
		{

			menu.AddItem( new GUIContent( sub + "Combine Children Values" ), adapter.par_e.RIGHT_MOD_BROADCAST_ENABLED, () => {
				Clear();
				adapter.par_e.RIGHT_MOD_BROADCAST_ENABLED = !adapter.par_e.RIGHT_MOD_BROADCAST_ENABLED;
				// adapter.SavePrefs();
				headOverrideTexture = adapter.par_e.RIGHT_MOD_BROADCAST_ENABLED ? Colors.redTTexure : (Color?)null;

				ResetStack();
				//   adapter.RESET_DRAW_STACKS();
				adapter.RepaintWindow( 0 );
			} );

			menu.AddSeparator( sub );

			VerticesModuleTypeEnum currentType = adapter.par_e.RIGHT_MOD_VERTICES_SCAN_TYPE;

			foreach ( var type in (VerticesModuleTypeEnum[])Enum.GetValues( typeof( VerticesModuleTypeEnum ) ) )
			{
				var targetType = type;
				var c = new GUIContent();
				c.text = sub + nameRight( type );
				var enabled = type == currentType;
				menu.AddItem( c, type == currentType, () => {
					if ( enabled ) return;

					Clear();

					adapter.par_e.RIGHT_MOD_VERTICES_SCAN_TYPE = targetType;
					//  adapter.SavePrefs();
					ResetStack();
					//adapter.RESET_DRAW_STACKS();

					StartBroadcasting();
					adapter.RepaintWindow( 0 );
				} );
			}
		}

		internal static void Clear() // var s = EditorSceneManager.GetActiveScene();
		{
			var dsc = GetDescript();
			interator = 0;
			currentList = null;
			dsc.BroadcastingInitializeAllObjects = false;
			dsc.Eroor = false;
			dsc.WasFirst = false;
			dsc.Broadcasting = false;
			dsc.md.Clear();
			dsc.shaderTextures.Clear();
			dsc.TEXTUREobjects.Clear();
			dsc.OBJECTtexture.Clear();
			dsc.cacheValue.Clear();
			dsc.broadCastValue.Clear();
			dsc.updateTimer.Clear();
		}
		// static Dictionary<GameObject, long> singleValue = new Dictionary<GameObject, long>();


		void WriteMF( Mod_VerticesHelper dsc, GameObject o )
		{
			if ( adapter.par_e.RIGHT_MOD_VERTICES_SCAN_TYPE == VerticesModuleTypeEnum.ChildCount ) return;

			if ( !dsc.md.ContainsKey( o.GetInstanceID() ) ) CHECK_MESHFILTER( ref dsc, o );

			if ( !dsc.md.ContainsKey( o.GetInstanceID() ) )
			{ // Debug.LogError( o.name + " hasn't MeshFilter" );
			  //D mf = null;
				return;
			}

			else
			{
				mf = dsc.md[ o.GetInstanceID() ];
			}
		}

		bool CHECK_MESHFILTER( GameObject o ) //  var s = EditorSceneManager.GetActiveScene();
		{
			if ( adapter.par_e.RIGHT_MOD_VERTICES_SCAN_TYPE == VerticesModuleTypeEnum.ChildCount ) return false;

			var dsc = GetDescript();
			return CHECK_MESHFILTER( ref dsc, o );
		}

		bool CHECK_MESHFILTER( ref Mod_VerticesHelper dsc, GameObject o ) //  var s = EditorSceneManager.GetActiveScene();
		{
			if ( adapter.par_e.RIGHT_MOD_VERTICES_SCAN_TYPE == VerticesModuleTypeEnum.ChildCount ) return false;

			// var dsc = GetDescript();
			// if ( dsc.)
			if ( !dsc.md.ContainsKey( o.GetInstanceID() ) )
			{ /* if (md.Any(meshFilter => !meshFilter.Value))
				     foreach (var source in md.Keys.ToArray().Where(source => md[source] == null))
				         md.Remove(source);*/

				dsc.md.Add( o.GetInstanceID(), new Mod_VerticesHelper.MeshGetter() );
			}

			//  if (!Validate(o)) return width;

			mf = dsc.md[ o.GetInstanceID() ];

			if ( !mf.mesh )
			{
				mf.SetMesh = o;

				/*= o.GetComponent<MeshFilter>();
                if (!mf.f) mf.s*/
				if ( !mf.mesh )
				{
					dsc.md.Remove( o.GetInstanceID() );
					return false;
				}

				dsc.md[ o.GetInstanceID() ] = mf;
			}

			return mf.mesh;
		}

		bool CHECK_TEXTURE( GameObject o ) // var mr = o.GetComponent<MeshRenderer>();
		{
			var mr = o.GetComponent<Renderer>();
			var pr = o.GetComponent<ParticleSystemRenderer>();
			var sr = mr ? null : o.GetComponent<SkinnedMeshRenderer>();
			var sharedMat = mr ? mr.sharedMaterials : pr ? pr.sharedMaterials : sr ? sr.sharedMaterials : null;


			if ( sharedMat != null && sharedMat.Length != 0 )
			{
				if ( sharedMat.Length != 0 ) return true;
			}

			else
			{
				var I = o.GetComponent<Image>();
				var SR = o.GetComponent<SpriteRenderer>();
				var sprite = I ? I.sprite : SR ? SR.sprite : null;

				// MonoBehaviour.print(sprite + " " + (sprite != null));
				if ( sprite != null /* && i.sprite != null*/) return true;
			}

			/*     var i = o.GetComponent<Image>();
                 if (i != null && i.sprite != null) return true;*/
			return false;
		}

		/*  string[] contexthelper =
          {
                  "Optimizer - Triangles count",
                  "Optimizer - Vertices count",
                  "Optimizer - ChildCount",
                  "Optimizer - Texture Memory Factor (Experimental)"
              };*/

		string BuildFullName()
		{
			return BuildFullName( adapter.par_e.RIGHT_MOD_VERTICES_SCAN_TYPE );
		}

		string BuildFullName( VerticesModuleTypeEnum type )
		{
			return HeaderText + " - " + nameRight( type );
		}

		string nameRight( VerticesModuleTypeEnum type )
		{
			switch ( type )
			{
				case VerticesModuleTypeEnum.Triangles:
					return "Triangles Count";

				case VerticesModuleTypeEnum.Vertices:
					return "Vertices Count";

				case VerticesModuleTypeEnum.ChildCount:
					return "Child Count";

				case VerticesModuleTypeEnum.TextureMemory:
					return adapter.par_e.RIGHT_MOD_BROADCAST_ENABLED ? "Textures In Memory (TextureSize\\ObjectsCount)" : "Textures In Memory (TextureSize Only)";

				default:
					throw new ArgumentOutOfRangeException();
			}
		}



		static int frameSkip = 1;

		Mod_VerticesHelper.MemoryData calcValue( Mod_VerticesHelper des, GameObject o )
		{ /*  if (par_e.RIGHT_MOD_BROADCAST_ENABLED)
			  {*/
			if ( des.broadCastValue.ContainsKey( o.GetInstanceID() ) ) return des.broadCastValue[ o.GetInstanceID() ];

			tempData.Clear();
			/*    tempData.memory = 0;
                tempData.postfix = ' ';*/
			return tempData;

			/*   }
               else
               {
                   return __calcValue(o);
               }*/
		}

		List<string> tempProps = new List<string>();

		List<int> tempTri = new List<int>();
		Mod_VerticesHelper.MemoryData tempData;


		Mod_VerticesHelper.MemoryData __fakeCalc( Mod_VerticesHelper des, GameObject o )
		{ /*if (!WriteMF( des, o))
			     {
			         tempData.Clear();
			         return tempData;
			     }*/
			WriteMF( des, o );
			return __calcValue( o );
		}

		Mod_VerticesHelper.MemoryData __calcValue( GameObject o )
		{
			tempData.Clear();

			/* tempData.memory = 0;
             tempData.postfix = ' ';*/
			switch ( adapter.par_e.RIGHT_MOD_VERTICES_SCAN_TYPE )
			{
				case VerticesModuleTypeEnum.Triangles:
					var mm = mf.mesh;
					var sb = mm.subMeshCount;
					tempData.memory = 0;

					for ( int i = 0; i < sb; i++ )
					{
						// adapter.GetTriangle( mf.mesh, ref tempTri, i );
						mf.mesh.GetTriangles( tempTri, i );
						tempData.memory += tempTri.Count / 3;
					}

					tempData.instance = true;
					break;

				case VerticesModuleTypeEnum.Vertices:
					tempData.memory = mf.mesh.vertexCount;
					tempData.instance = true;
					break;

				case VerticesModuleTypeEnum.ChildCount:
					tempData.memory = o.transform.childCount;
					tempData.instance = true;
					break;

				case VerticesModuleTypeEnum.TextureMemory:
					// Texture targetTexture = null;
					bool wasTexture = false;

					/*   var mr = o.GetComponent<Renderer>();
                       if (!mr) mr = o.GetComponent<ParticleSystemRenderer>();*/

					var mr = o.GetComponent<Renderer>();
					var pr = mr ? null : o.GetComponent<ParticleSystemRenderer>();
					var sr = mr ? null : o.GetComponent<SkinnedMeshRenderer>();
					var sharedMat = mr ? mr.sharedMaterials : pr ? pr.sharedMaterials : sr ? sr.sharedMaterials : null;

					if ( sharedMat != null && sharedMat.Length != 0 )
					{ /* if (sharedMat.Length != 0) return true;
					
						}
						
						if (mr)
						{*/
						if ( sharedMat.Length != 0 )
						{
							for ( int j = 0; j < sharedMat.Length; j++ )
							{
								if ( sharedMat[ j ] != null )
								{
									var shader = sharedMat[j].shader;

									/* if (!dsc.shaderTextures.ContainsKey(shader))
                                     {
                                         tempProps.Clear();
                                         Shader s;
                                         //s.name
                                         for (int k = 0, len = ShaderUtil.GetPropertyCount(shader); k < len; k++)
                                             if (ShaderUtil.GetPropertyType(shader, k) == ShaderUtil.ShaderPropertyType.TexEnv && !ShaderUtil.IsShaderPropertyHidden(shader, k))
                                                 tempProps.Add(ShaderUtil.GetPropertyName(shader, k));
                                    
                                         dsc.shaderTextures.Add(shader, tempProps.ToArray());
                                     }
                                    
                                     var arr = dsc.shaderTextures[shader];
                                    */

									tempProps.Clear();

									for ( int k = 0, len = ShaderUtil.GetPropertyCount( shader ); k < len; k++ )
										if ( ShaderUtil.GetPropertyType( shader, k ) == ShaderUtil.ShaderPropertyType.TexEnv && !ShaderUtil.IsShaderPropertyHidden( shader, k ) )
											tempProps.Add( ShaderUtil.GetPropertyName( shader, k ) );

									var texts = new Texture[tempProps.Count];
									bool haveANyTexture = false;

									for ( int i = 0; i < texts.Length; i++ )
									{
										texts[ i ] = sharedMat[ j ].GetTexture( tempProps[ i ] );

										if ( !haveANyTexture && texts[ i ] ) haveANyTexture = true;
									}

									//   var mem =
									if ( haveANyTexture )
									{
										tempData.memory += TextureOperator( o, new Mod_VerticesHelper.TextureSplitter( texts ), j );
										bool wasFirst = false;

										for ( int i = 0; i < texts.Length; i++ )
										{
											if ( !texts[ i ] ) continue;

											if ( !wasFirst )
											{
												wasFirst = true;
												tempData.addparams = "";
											}

											else
											{
												tempData.addparams += '\n';
											}

											tempData.addparams += "Texture '" + texts[ i ].name + "' " + MemoryToDisapley( rawMemory[ i ] );
										}


										tempData.instance = true;
										wasTexture = true;
									}
								}
							}

							/*       Shader s;
                                   s.
                                   targetTexture = mr.sharedMaterial.get;*/
						}
					}

					else
					{
						var I = o.GetComponent<Image>();
						var SR = o.GetComponent<SpriteRenderer>();
						//  MonoBehaviour.print(SR);
						var sprite = I ? I.sprite : SR ? SR.sprite : null;

						//  MonoBehaviour.print(sprite);
						if ( sprite != null /* && i.sprite != null*/)
						{
							Packer.GetAtlasDataForSprite( sprite, out atlas, out atlasTexture );

							if ( atlasTexture != null )
							{
								tempData.postfix = 'A';
								// var mem =
								tempData.memory += TextureOperator( o, new Mod_VerticesHelper.TextureSplitter( atlasTexture ), 0 );
								tempData.addparams = "Atlas '" + atlas + "' " + MemoryToDisapley( rawMemory[ 0 ] );
							}

							else
							{
								tempData.memory += TextureOperator( o, new Mod_VerticesHelper.TextureSplitter( sprite.texture ), 0 );
								tempData.addparams += "Texture '" + sprite.texture.name + "' " + MemoryToDisapley( rawMemory[ 0 ] );
								tempData.addparams += "\nWarning '" + sprite.texture.name + "' Not included in the atlas";
								tempData.addparams += "\nYou must assign a Packing Tag or Repack your atlases";
								tempData.postfix = '!';
							}

							tempData.instance = true;
							wasTexture = true;
						}
					}

					if ( !wasTexture ) tempData.memory += TextureOperator( o, null, 0 );


					// blow off the pass at the first round
					// make an error to show if it doesn't work
					// make the backlight red and for textures
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			return tempData;
		}
#pragma warning disable
		string atlas;
		Texture2D atlasTexture;
#pragma warning restore


		long TextureOperator( GameObject o, Mod_VerticesHelper.TextureSplitter? targetTexture, int index ) //  var s = EditorSceneManager.GetActiveScene();
		{
			var dsc = GetDescript();


			var id = o.GetInstanceID();

			if ( dsc.OBJECTtexture.ContainsKey( id ) )
			{
				var get = dsc.OBJECTtexture[id];

				if ( targetTexture == null ) // MonoBehaviour.print("ASD");
				{
					foreach ( var t in get[ index ].textures )
					{
						if ( !t ) continue;

						if ( dsc.TEXTUREobjects.ContainsKey( t ) ) dsc.TEXTUREobjects[ t ].Remove( id );

						if ( dsc.TEXTUREobjects[ t ].Count == 0 ) dsc.TEXTUREobjects.Remove( t );
					}

					dsc.OBJECTtexture.Remove( id );
				}

				else
				{
					if ( get.ContainsKey( index ) && !get[ index ].Equals( targetTexture.Value ) ) //changeTexture
					{
						//var t = get[index];
						foreach ( var t in get[ index ].textures )
						{
							if ( !t ) continue;

							if ( dsc.TEXTUREobjects.ContainsKey( t ) ) dsc.TEXTUREobjects[ t ].Remove( id );

							if ( dsc.TEXTUREobjects[ t ].Count == 0 ) dsc.TEXTUREobjects.Remove( t );
						}

						get.Remove( index );

						if ( get.Count == 0 ) dsc.OBJECTtexture.Remove( id );
					}
				}
			}


			if ( targetTexture != null )
			{
				if ( !dsc.OBJECTtexture.ContainsKey( id ) || !dsc.OBJECTtexture[ id ].ContainsKey( index ) )
				{
					if ( dsc.OBJECTtexture.ContainsKey( id ) ) dsc.OBJECTtexture.Remove( id );

					foreach ( var texturEobject in dsc.TEXTUREobjects ) texturEobject.Value.Remove( id );

					// if (OBJECTtexture.Any(meshFilter => !meshFilter.Value))
					/*   foreach (var source in OBJECTtexture.Keys.ToArray().Where(source => OBJECTtexture[source] == null))
                           OBJECTtexture.Remove(source);*/

					if ( !dsc.OBJECTtexture.ContainsKey( id ) ) dsc.OBJECTtexture.Add( id, new Dictionary<int, Mod_VerticesHelper.TextureSplitter>() );

					dsc.OBJECTtexture[ id ].Add( index, targetTexture.Value );

					for ( int i = 0; i < targetTexture.Value.textures.Length; i++ )
					{
						if ( !targetTexture.Value.textures[ i ] ) continue;

						if ( !dsc.TEXTUREobjects.ContainsKey( targetTexture.Value.textures[ i ] ) ) dsc.TEXTUREobjects.Add( targetTexture.Value.textures[ i ], new Dictionary<int, bool>() );

						if ( !dsc.TEXTUREobjects[ targetTexture.Value.textures[ i ] ].ContainsKey( id ) ) dsc.TEXTUREobjects[ targetTexture.Value.textures[ i ] ].Add( id, false );
					}
				}

				//OBJECTtexture[id] = targetTexture;

				long sum = 0;

				if ( rawMemory.Length < targetTexture.Value.textures.Length ) Array.Resize( ref rawMemory, targetTexture.Value.textures.Length );

				for ( int i = 0; i < targetTexture.Value.textures.Length; i++ )
				{
					if ( !targetTexture.Value.textures[ i ] ) continue;

					// MonoBehaviour.print(dsc.TEXTUREobjects[targetTexture.Value.textures[i]].Count);
					//#if UNITY_5_5
					// rawMemory[i] = Profiler.GetRuntimeMemorySize(targetTexture.Value.textures[i]);
					// rawMemory[ i ] = adapter.GetMemorySize( targetTexture.Value.textures[ i ] );
					rawMemory[ i ] = Profiler.GetRuntimeMemorySizeLong( targetTexture.Value.textures[ i ] ) / 2;

					/* Texture.
                     ((Texture2D)targetTexture.Value.textures[i].i).is
                     if ()*/
					/*#else
                                            rawMemory[i] = Profiler.GetRuntimeMemorySizeLong(targetTexture.Value.textures[i]);
                    #endif*/
					if ( dsc.Broadcasting ) sum += rawMemory[ i ] / dsc.TEXTUREobjects[ targetTexture.Value.textures[ i ] ].Count;
					else sum += rawMemory[ i ];
				}

				return sum;

				/* if (!singleValue.ContainsKey(o)) singleValue.Add(o, 0);
                 singleValue[o] = v;*/
			}

			return 0;
		}

		long[] rawMemory = new long[0];

		bool Allow( GameObject o )
		{
			if ( adapter.par_e.RIGHT_MOD_VERTICES_SCAN_TYPE == VerticesModuleTypeEnum.Triangles ||
				adapter.par_e.RIGHT_MOD_VERTICES_SCAN_TYPE == VerticesModuleTypeEnum.Vertices
			)
				if ( !CHECK_MESHFILTER( o ) )
					return false;

			if ( adapter.par_e.RIGHT_MOD_VERTICES_SCAN_TYPE == VerticesModuleTypeEnum.TextureMemory )
				if ( !CHECK_TEXTURE( o ) )
					return false;

			return true;
		}

		private static Mod_VerticesHelper.MeshGetter mf;


		void broadCastAction( Mod_VerticesHelper dsc, GameObject get_o )
		{
			if ( !dsc.updateTimer.ContainsKey( get_o.GetInstanceID() ) ) dsc.updateTimer.Add( get_o.GetInstanceID(), 0 );

			if ( Math.Abs( EditorApplication.timeSinceStartup - dsc.updateTimer[ get_o.GetInstanceID() ] ) < 0.5 ) return;


			tempData.Clear();

			if ( !dsc.broadCastValue.ContainsKey( get_o.GetInstanceID() ) )
			{
				adapter.RepaintWindowInUpdate( adapter.pluginID );
				ResetStack( get_o.GetInstanceID() );
				// adapter.RESET_DRAW_STACKS();
				dsc.broadCastValue.Add( get_o.GetInstanceID(), tempData );
			}

			dsc.updateTimer[ get_o.GetInstanceID() ] = EditorApplication.timeSinceStartup;

			/*   tempData.memory = 0;
               tempData.postfix = ' ';*/
			if ( Allow( get_o ) ) tempData = __calcValue( get_o );

			for ( int i = 0; i < get_o.transform.childCount; i++ )
			{
				var c = get_o.transform.GetChild(i);

				if ( dsc.broadCastValue.ContainsKey( c.gameObject.GetInstanceID() ) )
				{
					var g = dsc.broadCastValue[c.gameObject.GetInstanceID()];
					tempData.memory += g.memory;

					if ( g.postfix == '!' ) tempData.postfix = '!';
				}

				// if (!updateTimer.ContainsKey(c.gameObject.GetInstanceID())) oldval = -1;
			}

			var oldval = dsc.broadCastValue[get_o.GetInstanceID()];
			dsc.broadCastValue[ get_o.GetInstanceID() ] = tempData;

			if ( oldval.memory != dsc.broadCastValue[ get_o.GetInstanceID() ].memory ) //           lastUpdate = 0;
			{
				dsc.updateTimer[ get_o.GetInstanceID() ] = 0;
				adapter.RepaintWindowInUpdate( adapter.pluginID );
				ResetStack( get_o.GetInstanceID() );
				//  adapter.RESET_DRAW_STACKS();
			}
		}

		void broadCastActionOnlyCaclulate( Mod_VerticesHelper dsc, GameObject get_o )
		{
			if ( !dsc.updateTimer.ContainsKey( get_o.GetInstanceID() ) ) dsc.updateTimer.Add( get_o.GetInstanceID(), 0 );

			// if (Math.Abs(EditorApplication.timeSinceStartup - dsc.updateTimer[get_o.GetInstanceID()]) < 0.5) return;


			//   if (!dsc.broadCastValue.ContainsKey(get_o.GetInstanceID())) dsc.broadCastValue.Add(get_o.GetInstanceID(), tempData);
			dsc.updateTimer[ get_o.GetInstanceID() ] = EditorApplication.timeSinceStartup;

			var oldval = !dsc.broadCastValue.ContainsKey(get_o.GetInstanceID()) ? 0 : dsc.broadCastValue[get_o.GetInstanceID()].memory;
			long newVal = 0;

			if ( Allow( get_o ) ) newVal = __calcValue( get_o ).memory;

			if ( newVal != oldval )
			{
				dsc.updateTimer[ get_o.GetInstanceID() ] = 0;
				adapter.RepaintWindowInUpdate( adapter.pluginID );
				ResetStack( get_o.GetInstanceID() );
				// adapter.RESET_DRAW_STACKS();
			}
		}


		//bool initFlag  = false;



		//  static bool broadcasting = false;
		static IEnumerator<HierarchyObject> currentList = null;
		static int currentIndex = 0;
		static int interator = 0;
		static Action<Mod_VerticesHelper, GameObject> brc;
		static List<Texture> removeList = new List<Texture>();

		static void StartBroadcasting()
		{
			if ( frameSkip != 0 )
			{
				frameSkip--;
				return;
			}

			var s = EditorSceneManager.GetActiveScene();

			if ( !s.isLoaded || !s.IsValid() ) return;

			var dsc = GetDescript();

			if ( dsc.Broadcasting ) return;

			mem = changed;
			dsc.Broadcasting = true;
			dsc.shaderTextures.Clear();

			foreach ( var texturEobject in dsc.TEXTUREobjects )
			{
				/*   if (texturEobject.Key == null)
                   {
                       continue;
                   }*/
				if ( dsc.TEXTUREobjects[ texturEobject.Key ].Any( o => !EditorUtility.InstanceIDToObject( o.Key ) ) )
				{
					foreach ( var source in dsc.TEXTUREobjects[ texturEobject.Key ].Keys.ToArray() )
					{
						if ( !EditorUtility.InstanceIDToObject( source ) )
						{
							dsc.TEXTUREobjects[ texturEobject.Key ].Remove( source );
							dsc.OBJECTtexture.Remove( source );
						}
					}

					if ( dsc.TEXTUREobjects[ texturEobject.Key ].Count == 0 )
					{
						removeList.Add( texturEobject.Key );
					}
				}
			}

			if ( removeList.Count != 0 ) // MonoBehaviour.print("ASD" + removeList.Count);
			{
				foreach ( var texture in removeList )
				{
					dsc.TEXTUREobjects.Remove( texture );
				}

				removeList.Clear();
			}


			currentList = Tools.AllSceneObjectsInterator( 0 ).GetEnumerator();
			//currentList.Reverse();

			interator = currentIndex = 0;
		}


		internal void CalcBroadCast() // var s = EditorSceneManager.GetActiveScene();
		{

			if ( currentList == null ) return;
			var dsc = GetDescript();

			//  MonoBehaviour.print("AS");
			if ( !dsc.Broadcasting ) return;

			interator = 0;

			//foreach (var current in currentList)
			while ( currentList.MoveNext() )
			//while (currentIndex < currentList.Count)
			{
				var current = currentList.Current;

				if ( ! /*currentList[currentIndex]*/current.go )
				{
					currentIndex++;
					continue;
				}

				if ( (current.go.hideFlags & HideFlags.HideInInspector) != 0 )
				{
					currentIndex++;
					continue;
				}


				try
				{
					brc( dsc, current.go );
				}

				catch
				{
					Clear();
					currentIndex = 0;
					dsc.Broadcasting = false;
					dsc.Eroor = true;
					dsc.WasFirst = false;
					return;
				}

				currentIndex++;
				interator++;
				if ( interator > Root.p[ 0 ].par_e.RIGHT_MOD_BROADCASTING_PREFOMANCE ) return;
			}


			currentList = null;
			//   MonoBehaviour.print(dsc.TEXTUREobjects.Count + " " + dsc.OBJECTtexture.Count);
			if ( interator <= Root.p[ 0 ].par_e.RIGHT_MOD_BROADCASTING_PREFOMANCE )
			{
				//Debug.Log( interator );

				dsc.Broadcasting = false;
				dsc.Eroor = false;

				if ( !dsc.WasFirst )
				{
					dsc.WasFirst = true;
					Root.p[ 0 ].RepaintWindowInUpdate( adapter.pluginID );
					ResetStack();

					if ( !Root.p[ 0 ].par_e.RIGHT_MOD_BROADCAST_ENABLED && adapter.par_e.RIGHT_MOD_VERTICES_SCAN_TYPE == VerticesModuleTypeEnum.TextureMemory )
					{
						//dsc.broadCastValue.Clear();
						/*foreach (var memoryData in dsc.broadCastValue)
                        {
                            var v = memoryData.Value;
                            v.addparams
                        }*/
					}
				}

				//else
				{
					if ( !dsc.BroadcastingInitializeAllObjects )
					{
						dsc.BroadcastingInitializeAllObjects = true;
					}
				}
			}

			else
			{
				Root.p[ 0 ].RepaintWindowInUpdate( adapter.pluginID );
				ResetStack();
				//  Root.p[0].RESET_DRAW_STACKS();
			}
		}


		//  Color labelColor = new Color(0.9f, 0.5f, 0.2f, 1);
		Color labelWarningColor = new Color(1f, 0.5f, 0.4f, 1);

		string MemoryToDisapley( long v )
		{
			if ( v > 1000000 ) return (Mathf.RoundToInt( v / 100000f ) / 10f).ToString( CultureInfo.InvariantCulture ) + "M";
			else if ( v > 1000 ) return (Mathf.RoundToInt( v / 100f ) / 10f).ToString( CultureInfo.InvariantCulture ) + "k";

			return v.ToString();
		}

		public override void Draw() //Profiler.GetRuntimeMemorySizeLong( !!!!!!!!!!!!!!!
		{
			//if (OPT_EV_BR(EVENT)) return 0;

			var o = adapter.o.go;

			if ( adapter.EVENT.type == EventType.Layout ) return;

			// base.ContextHelper = "Optimizer set to " + Hierarchy.par_e.VerticesModuleType + " count";
			//  base.ContextHelper = contexthelper[(int)Hierarchy.par_e.VerticesModuleType];
			//  base.ContextHelper = BuildFullName(Hierarchy.par_e.VerticesModuleType);

			/*   if ( Root.p[0].DISABLE_DESCRIPTION( _o ) )
               { 
                   GUI.Label( drawRect, Adapter.CacheDisableConten, !callFromExternal() ? adapter.STYLE_LABEL_8_right : adapter.STYLE_LABEL_8_WINDOWS_right );
                   return width;
               }*/

			/* if ( EVENT.type == EventType.Layout && initFlag )
             {   ResetStack();
                 initFlag = false;
                 Debug.Log( "ASD" );
             }*/

			if ( !START_DRAW( drawRect, adapter.o ) ) return;

			// var o = adapter.o.go;


			base.ContextHelper = HeaderText;

			if ( adapter.par_e.RIGHT_MOD_BROADCAST_ENABLED && Application.isPlaying )
			{
				adapter.par_e.RIGHT_MOD_BROADCAST_ENABLED = false;
			}

#if BROADCAST
			if (Application.isPlaying && par_e.VerticesModuleType == VerticesModuleTypeEnum.TextureMemory)
			{	par_e.VerticesModuleType = VerticesModuleTypeEnum.Triangles;
			}

#endif
			//  var s = EditorSceneManager.GetActiveScene();
			var dsc = GetDescript();
			bool needhide = true;

			if ( EVENT.type == EventType.Repaint )
			{
				// headOverrideTexture = adapter.par_e.RIGHT_MOD_BROADCAST_ENABLED /*|| par_e.VerticesModuleType == VerticesModuleTypeEnum.TextureMemory*/ ? Colors.redTTexure : (Color?)null;
				if ( adapter.par_e.RIGHT_MOD_BROADCAST_ENABLED ) headOverrideTexture = Colors.redTTexure;
				else headOverrideTexture = null;
				// try
				// {


				//   if (!singleValue.ContainsKey(o)) singleValue.Add(o, 0);
				if ( adapter.par_e.RIGHT_MOD_BROADCAST_ENABLED ) //  if (Math.Abs(EditorApplication.timeSinceStartup - lastUpdate) > 0.5)
				{ //   {
				  //      lastUpdate = EditorApplication.timeSinceStartup;
				  //  bool needRepaing = false;
				  // foreach (var get_o in Utilites.AllSceneObjects())
				  // foreach (var get_o in Utilites.AllSceneObjects())
					/*  {
                    
                      }*/
					//  }

					//  Utilites.BroadCastActionReverse(o, broadCastAction);

					if ( mem != changed )
					{
						brc = broadCastAction;
						StartBroadcasting();
					}

				}

				else
				{
#if BROADCAST
					if (par_e.VerticesModuleType == VerticesModuleTypeEnum.TextureMemory)
					{	brc = broadCastActionOnlyCaclulate;
						StartBroadcasting();
					}

#endif


					if ( !dsc.updateTimer.ContainsKey( adapter.o.id ) ) dsc.updateTimer.Add( adapter.o.id, 555 );

					if ( Math.Abs( EditorApplication.timeSinceStartup - dsc.updateTimer[ adapter.o.id ] ) > 0.5 )
					{
						if ( !dsc.broadCastValue.ContainsKey( adapter.o.id ) ) dsc.broadCastValue.Add( adapter.o.id, tempData );

						if ( Allow( o ) )
						{
							var old = dsc.broadCastValue[adapter.o.id].memory;
							dsc.broadCastValue[ adapter.o.id ] = __calcValue( o );

							if ( old != dsc.broadCastValue[ adapter.o.id ].memory )
							{
								adapter.RepaintWindowInUpdate( adapter.pluginID );

								//ResetStack( adapter.o.id );
								//  adapter.RESET_DRAW_STACKS();
								if ( adapter.AD.firstFrame < 1 ) ResetStack();
								//adapter.RESET_DRAW_STACKS(); ///////////////////////
								else ResetStack();

								//initFlag = true;
								//
							}
						}

						else
						{
							tempData.Clear();

							/*   tempData.memory = 0;
                               tempData.postfix = ' ';*/
							dsc.broadCastValue[ adapter.o.id ] = tempData;
						}

						dsc.updateTimer[ adapter.o.id ] = EditorApplication.timeSinceStartup;

						if ( !dsc.WasFirst
#if BROADCAST
						        && par_e.VerticesModuleType != VerticesModuleTypeEnum.TextureMemory
#endif
						)
						{
							dsc.WasFirst = true;
							// initFlag = true;
							dsc.BroadcastingInitializeAllObjects = true;
							adapter.RepaintWindowInUpdate( adapter.pluginID );

							// if ( adapter.firstFrame < 4 ) adapter.RESET_DRAW_STACKS(); ///////////////////////
							//else 
							ResetStack();
						}
					}
				}


				/*   brc = broadCastAction;
                   StartBroadcasting();*/
				if ( Allow( o ) || adapter.par_e.RIGHT_MOD_BROADCAST_ENABLED )
				{
					if ( (dsc.WasFirst
#if BROADCAST
					        || par_e.VerticesModuleType != VerticesModuleTypeEnum.TextureMemory
#endif
						) && dsc.broadCastValue.ContainsKey( adapter.o.id ) )
					{
						var v = dsc.broadCastValue[adapter.o.id];

						content.text = MemoryToDisapley( v.memory );


						content.tooltip = BuildFullName() + " " + content.text;

						if ( v.memory == 0 ) content.text = "-";
						else if ( v.postfix != ' ' )
						{
							content.text = string.Concat( "", v.postfix, "  ", content.text );

							if ( v.postfix == 'A' ) content.tooltip += " (Atlas)";
						}

						if ( v.addparams != null )
						{
							content.tooltip += "\n" + v.addparams;
						}

						needhide = !v.instance;
					}

					else
					{
						if ( dsc.Eroor )
						{
							content.tooltip = BuildFullName() + " error";
							content.text = "error";
						}

						else
						{
							content.tooltip = BuildFullName() + " ...";
							content.text = "...";
							needhide = false;
						}
					}
				}

				else
				{
					content.tooltip = BuildFullName() + " 0";
					content.text = "-";
				}


				// }
				/*     catch
                     {
                         content.tooltip = BuildFullName() + " error";
                         content.text = "error";
                     }*/


				if ( !dsc.cacheValue.ContainsKey( adapter.o.id ) ) dsc.cacheValue.Add( adapter.o.id, new GUIContent() );

				dsc.cacheValue[ adapter.o.id ].tooltip = content.tooltip;
				dsc.cacheValue[ adapter.o.id ].text = content.text;
			}

			if ( dsc.cacheValue.ContainsKey( adapter.o.id ) )
			{
				content.tooltip = dsc.cacheValue[ adapter.o.id ].tooltip;
				content.text = dsc.cacheValue[ adapter.o.id ].text;
			}

			else
			{
				content.tooltip = "Updating";
				content.text = "...";
				needhide = false;
			}


			/*  var oldl = Adapter.GET_SKIN().label.fontSize;
            var olda = Adapter.GET_SKIN().label.alignment;
            var oldss = Adapter.GET_SKIN().label.fontStyle;
            Adapter.GET_SKIN().label.alignment = TextAnchor.MiddleRight;
            Adapter.GET_SKIN().label.fontSize = adapter.FONT_8();*/
#if BROADCAST
			|| par_e.VerticesModuleType == VerticesModuleTypeEnum.TextureMemory
#endif


			/*  if ( adapter.par_e.RIGHT_MOD_BROADCAST_ENABLED )
              {   var asd = GUI.color;
                  GUI.color *= new Color32( 200, 90, 50, 15 );
                  GUI.DrawTexture( drawRect, Texture2D.whiteTexture );
                  GUI.color = asd;
              }
            
              var L = adapter.STYLE_LABEL_8_right;
            
            
              if ( content.text.EndsWith( "M" ) ) L.fontStyle = FontStyle.Bold;
              GUI.enabled = o.activeInHierarchy;
              var hasContent = true;
            
              if ( content.text[0] == '!' )
              {   var oldC = L.normal.textColor;
                  L.normal.textColor = Color.black;
                  r.Set( drawRect.x + 0.5f, drawRect.y, drawRect.width, drawRect.height );
                  GUI.Label( r, content, L );
                  L.normal.textColor = labelWarningColor;
                  GUI.Label( drawRect, content, L );
                  L.normal.textColor = oldC;
              }
              else
              {   var oldstate = GUI.enabled;
                  GUI.enabled &= !needhide;
                  var al = L.alignment;
                  if ( content.text == "-" )
                  {   L.alignment = __Align;
                      hasContent = false;
                  }
                  if ( EditorGUIUtility.isProSkin && GUI.enabled )
                  {   var oldC = L.normal.textColor;
                      L.normal.textColor = Color.black;
                      r.Set( drawRect.x + 1, drawRect.y, drawRect.width, drawRect.height );
                      GUI.Label( r, content, L );
                      L.normal.textColor = oldC;
                  }
                  GUI.Label( drawRect, content, L );
                  L.alignment = al;
                  GUI.enabled = oldstate;
              }
            
              GUI.enabled = true;
              L.fontStyle = FontStyle.Normal;
              */



			if ( STYLE_M_BLACKCOLOR == null )
			{
				STYLE_M_BLACKCOLOR = new GUIStyle[ 4 ];
				STYLE_M_WARMCOLOR = new GUIStyle[ 4 ];
				STYLE_M_NORMALCOLOR = new GUIStyle[ 4 ];

				for ( int i = 0; i < 2; i++ )
				{
					STYLE_M_BLACKCOLOR[ i ] = new GUIStyle( RightModsStyles.STYLE_LABEL_8_right );
					STYLE_M_BLACKCOLOR[ i ].normal.textColor = Color.black;
					STYLE_M_WARMCOLOR[ i ] = new GUIStyle( RightModsStyles.STYLE_LABEL_8_right );
					STYLE_M_WARMCOLOR[ i ].normal.textColor = labelWarningColor;
					STYLE_M_NORMALCOLOR[ i ] = new GUIStyle( RightModsStyles.STYLE_LABEL_8_right );
				}

				STYLE_M_BLACKCOLOR[ 1 ].fontStyle = FontStyle.Bold;
				STYLE_M_WARMCOLOR[ 1 ].fontStyle = FontStyle.Bold;
				STYLE_M_NORMALCOLOR[ 1 ].fontStyle = FontStyle.Bold;
			}

			var BOLD = content.text.EndsWith("M");
			var _SI = BOLD ? 1 : 0;
			//_SI = 1;
			//if ( content.text.EndsWith( "M" ) ) L.fontStyle = FontStyle.Bold;
			var hasContent = true;

			r = adapter.modsController.rightModsManager.DrawCursorRect( ref drawRect, STYLE_M_NORMALCOLOR[ _SI ], content );

			if ( adapter.par_e.RIGHT_MOD_BROADCAST_ENABLED )
			{
				Draw_GUITexture( adapter.par_e.RIGHTDOCK_SHRINK_BUTTONS_INT == 2 ? r : drawRect, bc );
			}


			if ( content.text[ 0 ] == '!' )
			{
				r2 = r;
				r2.x += 0.5f;
				Draw_Label( r2, content, STYLE_M_BLACKCOLOR[ _SI ], true );
				Draw_Label( r, content, STYLE_M_WARMCOLOR[ _SI ], true );
			}
			else
			{
				var enableOverride = !needhide;
				enableOverride = true;
				var USE_ALIGN__ = content.text == "-";

				if ( USE_ALIGN__ )
				{ //  L.alignment = __Align;
					hasContent = false;
				}

				if ( EditorGUIUtility.isProSkin && GUI.enabled )
				{
					r2 = r;
					r2.x += 1;
					Draw_Label( r2, content, STYLE_M_BLACKCOLOR[ _SI ], true, ADDITIONAL_ENABLE: enableOverride );
				}

				Draw_Label( r, content, STYLE_M_NORMALCOLOR[ _SI ], true, ADDITIONAL_ENABLE: enableOverride );
			}


			/* Adapter.GET_SKIN().label.fontSize = oldl;
             Adapter.GET_SKIN().label.fontStyle = oldss;
             Adapter.GET_SKIN().label.alignment = olda;*/

			/// drawRect.y -= 2;

			Draw_ModuleButton( r, content, BUTTON_ACTION_HASH, hasContent, dsc, drawPointer: true );


			END_DRAW( adapter.o, savedData.temp_i );
			if ( savedData.temp_i != -1 ) adapter.o.lastContentRectLayout[ savedData.temp_i ].SET( ref r );


		}

		// interna struct
		internal static GUIStyle[] STYLE_M_BLACKCOLOR, STYLE_M_WARMCOLOR, STYLE_M_NORMALCOLOR;
		Color32 bc =new Color32( 200, 90, 50, 15 );

		DrawStackMethodsWrapper __BUTTON_ACTION_HASH = null;

		DrawStackMethodsWrapper BUTTON_ACTION_HASH {
			get { return __BUTTON_ACTION_HASH ?? (__BUTTON_ACTION_HASH = new DrawStackMethodsWrapper( BUTTON_ACTION )); }
		}
		Rect r , r2;
		void BUTTON_ACTION( Rect worldOffset, Rect inputRect, DrawStackMethodsWrapperData data, HierarchyObject _o )
		{
			var o = _o.go;
			var dsc = (Mod_VerticesHelper) data.args;
#pragma warning disable
			var content = data.content;
#pragma warning restore

			if ( EVENT.button == adapter.MOUSE_BUTTON_0 )
			{
				var menu = new GenericMenu();

				ModuleCommonGenericMenu( menu, o, null );

				//adapter.PUSH_MENU_OPENING_ACTION( menu, null );

				menu.ShowAsContext();
				Tools.EventUse();
			}


			if ( EVENT.button == adapter.MOUSE_BUTTON_1 && content.text != "error" )
			{
				Tools.EventUse();

				/*  int[] contentCost = new int[0];
                  GameObject[] obs = new GameObject[0];*/
				// if (EditorSceneManager.GetActiveScene().rootCount != 0) CallHeaderFillter(out obs, out contentCost, md[adapter.o.id].sharedMesh.vertexCount);
				//   Debug.Log( content.text );
				if ( content.text == "-" )
				{
					var result = CallHeader();

					if ( result != null ) // FillterData.Init(EVENT.mousePosition, SearchHelper, Hierarchy.par_e.VerticesModuleType + " All", obs, contentCost, null, this);
					{
						var mp = new MousePos(EVENT.mousePosition, MousePos.Type.Search_356_0, !callFromExternal(), adapter);
						Windows.SearchWindow.Init( mp, SearchHelper, adapter.par_e.RIGHT_MOD_VERTICES_SCAN_TYPE + " All",
							result, this, adapter.window, _o );
					}

					/*  } else
                      {
                         // FillterData.Init(EVENT.mousePosition, SearchHelper, Hierarchy.par_e.VerticesModuleType + " All", obs, contentCost, null, this);
                    
                             FillterData.Init(EVENT.mousePosition, SearchHelper, Hierarchy.par_e.VerticesModuleType + " All",
                                  CallHeaderFiltered(calc.memory), this);
                      }*/
				}

				else
				{
					if ( !dsc.BroadcastingInitializeAllObjects ) // var pos = InputData.WidnwoRect(!callFromExternal(), EVENT.mousePosition, 128, 68, adapter );
					{
						var pos = new MousePos(EVENT.mousePosition, MousePos.Type.Input_128_68, !callFromExternal(), adapter);
						Windows.InputWindow.Init( pos, HeaderText, adapter.window, null, null, "Waiting for broadcast calculating..." );
					}

					else
					{
						var calc = calcValue(dsc, o);

						if ( !calc.instance ) // var pos = InputData.WidnwoRect(!callFromExternal(), EVENT.mousePosition, 128, 68, adapter );
						{ // InputData.Init(pos, "", null, null, "hasn't own value");
							var pos = new MousePos(EVENT.mousePosition, MousePos.Type.Input_128_68, !callFromExternal(), adapter);
							Windows.InputWindow.Init( pos, HeaderText, adapter.window, null, null, "Object hasn't any texture, please select any child object" );
						}

						else if ( Validate( o ) )
						{ /* if (EditorSceneManager.GetActiveScene().rootCount != 0) CallHeaderFillter(out obs, out contentCost, calc.memory);
							
								 FillterData.Init(EVENT.mousePosition, SearchHelper, Hierarchy.par_e.VerticesModuleType + " " + content.text, obs, contentCost, null, this);*/

							var mp = new MousePos(EVENT.mousePosition, MousePos.Type.Search_356_0, !callFromExternal(), adapter);
							Windows.SearchWindow.Init( mp, SearchHelper, adapter.par_e.RIGHT_MOD_VERTICES_SCAN_TYPE + " " + content.text,
								CallHeaderFiltered( calc.memory ), this, adapter.window, _o );
						}
					}
				}


				// EditorGUIUtility.ic
			}

			// Undo.RecordObject(o, "GameObject Lock");

			// EditorGUIUtility.ExitGUI();
		}

		private bool Validate( GameObject o )
		{
			return Allow( o );
			/* if (!md.ContainsKey(adapter.o.id)) return false;
             if (md[adapter.o.id] == null) return false;
             var mf = md[adapter.o.id];
             if (mf.sharedMesh == null) return false;
             return true;*/
		}


		/* FillterData.Init(EVENT.mousePosition, SearchHelper, LayerMask.LayerToName(o.layer),
                      Validate(o) ?
                      CallHeaderFiltered(LayerMask.LayerToName(o.layer)) :
                      CallHeader(),
                      this);*/
		/** CALL HEADER */
		Windows.SearchWindow.FillterData_Inputs m_CallHeader()
		{
			var dsc = GetDescript();

			var result = new Windows.SearchWindow.FillterData_Inputs(callFromExternal_objects)
			{
				Valudator = null,
				SelectCompareString = (o, i) => (adapter.par_e.RIGHT_MOD_BROADCAST_ENABLED ? calcValue(dsc, o.go) : __fakeCalc(dsc, o.go)).memory.ToString(),
				SelectCompareCostInt = (o, i) => (int) Math.Min((adapter.par_e.RIGHT_MOD_BROADCAST_ENABLED ? calcValue(dsc, o.go) : __fakeCalc(dsc, o.go)).memory, int.MaxValue)
			};
			return result;
		}

		internal Windows.SearchWindow.FillterData_Inputs CallHeaderFiltered( long fillter )
		{
			var result = CallHeader();

			Func<Mod_VerticesHelper, GameObject, Mod_VerticesHelper.MemoryData> calcAction = adapter.par_e.RIGHT_MOD_BROADCAST_ENABLED ? calcValue : (Func<Mod_VerticesHelper, GameObject, Mod_VerticesHelper.MemoryData>) __fakeCalc;

			var dsc = GetDescript();

			result.Valudator = o => {
				try
				{
					var restul = Validate(o.go);

					if ( restul )
					{
						var c = calcAction(dsc, o.go);

						if ( !c.instance ) return false;

						var calc = c.memory;
						restul = calc != 0 && calc == fillter;
					}

					return restul;
				}

				catch ( Exception ex )
				{
					Debug.LogError( ex.Message + "\n\n" + ex.StackTrace );
					return false;
				}
			};

			return result;
		}

		/** CALL HEADER */
		internal override Windows.SearchWindow.FillterData_Inputs CallHeader() //  var s = EditorSceneManager.GetActiveScene();
		{
			var dsc = GetDescript();

			if ( !dsc.BroadcastingInitializeAllObjects ) //var pos = InputData.WidnwoRect( !callFromExternal(), EVENT.mousePosition, 128, 68, adapter );
			{
				var pos = new MousePos(EVENT.mousePosition, MousePos.Type.Input_128_68, !callFromExternal(), adapter);
				Windows.InputWindow.Init( pos, HeaderText, adapter.window, null, null, "Waiting for broadcast calculating..." );
				return null;
			}


			var result = m_CallHeader();

			result.Valudator = o => {
				try
				{
					if ( !Validate( o.go ) ) return false;

					var c = adapter.par_e.RIGHT_MOD_BROADCAST_ENABLED ? calcValue(dsc, o.go) : __fakeCalc(dsc, o.go);

					if ( !c.instance ) return false;

					//  return calcAction( dsc, o.go ).memory != 0;
					return c.memory != 0;
				}

				catch /*(Exception ex)*/
				{ //Debug.LogError(ex.Message + "\n" + o.go.name + " " + o.go.GetInstanceID() + "\n" + ex.StackTrace);
					return false;
				}
			};

			return result;

			/*   result.SelectCompareString = (d, i) => calcAction(dsc, d).memory.ToString();
               result.SelectCompareCostInt = (d, i) => calcAction(dsc, d).memory;
                   .OrderBy(d => d.name)
                   .Select((d, i) => new { d.startIndex, cost = i })
                   .OrderBy(d => d.startIndex)
                   .Select(d => d.cost).ToArray();
               return true;*/
		}

		/*  internal void CallHeaderFillter(out GameObject[] obs, out int[] contentCost, long fillter)
          {
              //   obs = Utilites.AllSceneObjects().Where(d => Validate(d) && calcValue(d) == fillter).ToArray();
              //  var s = EditorSceneManager.GetActiveScene();
              Func<M_VerticesHelper, GameObject, M_VerticesHelper.MemoryData> calcAction = par_e.RIGHT_MOD_BROADCAST_ENABLED ? calcValue :
                  (Func<M_VerticesHelper, GameObject, M_VerticesHelper.MemoryData>)__fakeCalc;
        
              var dsc = GetDescript();
              obs = Utilities.AllSceneObjects().Where(o => {
                  try
                  {
                      var restul = Validate(o);
                      if (restul)
                      {
                          var c = calcAction(dsc, o);
                          if (!c.instance) return false;
                          var calc = c.memory;
                          restul = calc != 0 && calc == fillter;
                      }
                      return restul;
                  }
                  catch
                  {
                      return false;
                  }
              }).ToArray();
        
              contentCost = obs.Select((d, i) => new { name = calcAction(dsc, d).memory, startIndex = i }).OrderBy(d => d.name).Select((d, i) => new { d.startIndex, cost = i }).OrderBy(d => d.startIndex).Select(d => d.cost).ToArray();
          }*/
	}
}

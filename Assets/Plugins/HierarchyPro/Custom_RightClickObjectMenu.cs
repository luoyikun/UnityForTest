#if UNITY_EDITOR

using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Examples.HierarchyPlugin.Hierarchy_Examples
{
	public class RightClickOnObjectMenu_Examples
	{

		////////
		//////// Examples:
		////////
		////////   -	To create a hotkey you can use the following special characters: % (ctrl on Windows, cmd on macOS), # (shift), & (alt).
		////////   -	To create a special hotkey you can use "MySubItem/MyMenuItem %LEFT" "MySubItem/MyMenuItem %HOME" "MySubItem/MyMenuItem %ENDER".
		////////
		////////   !	Note that - add this INIT method in your custom menu script, including this method allows to avoid additional assembly scan on load.
		////////



		////////////////////////////////////JUST COPY THAT///MENU ITEM TEMPLATE///////////////////////////////////////////////////////////////////////////////

		class MenuTemplate : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{
			const string NAME = "My Optional Sub Item/My Menu Item %k";
			const int POSITION = 0;

			public override bool IsEnable( GameObject clickedObject ) { return true; }
			public override bool NeedExcludeFromMenu( GameObject clickedObject ) { return false; }


			public override string Name { get { return NAME; } } // replace with 'return ItemsPlacementFolder + NAME;' if you want use Hierarchy Pro subcategory
			public override int PositionInMenu { get { return POSITION; } }
			[InitializeOnLoadMethod] public static void INIT() { System.Activator.CreateInstance( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType ); } // Don't forget this method


			public override void OnClick( GameObject[] affectedObjectsArray )
			{
				EMX.Utility.SHOW_StringInput( "My Name:", "default input text", ( newTextResult ) => {
					// Do Something!
				} );
			}

			public override bool DoRecordUndoOnClick { get { return true; } } // - yey, you can override this property if you want to use simple undo creation
																			   //
																			   // - if you need more complex solution, you may add this part of code below your code
																			   // Undo.RecordObject( o, "Undo text" );
																			   // your changes
																			   // MyMenu_Utils.SetDirty( o );
																			   //
																			   // - or you can just skip creating of undo operations
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



		#region ITEM 100-101 - Group/UnGroup
		class MyMenu_GroupFirst : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] public static void INIT() { System.Activator.CreateInstance( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType ); }
			public override bool IsEnable( GameObject clickedObject ) { return true; }
			public override bool NeedExcludeFromMenu( GameObject clickedObject ) { return false; }
			public override int PositionInMenu { get { return 100; } }
			public override string Name { get { return ItemsPlacementFolder + "Group (Last center) %g"; } }


			public override void OnClick( GameObject[] affectedObjectsArray )
			{
				var onlytop = MyMenu_Utils.GetOnlyTopObjects(affectedObjectsArray).OrderBy(go => go.transform.GetSiblingIndex()).ToArray();

				if ( onlytop.Length == 0 ) return;

				var center_object = onlytop[onlytop.Length - 1];

				var groupParent = center_object.transform.parent;
				var groupSiblingIndex = center_object.transform.GetSiblingIndex();

				var reference = center_object;
				var NEW_NAME = reference.name + " Group";

				// Top Object Name Variant
				EMX.Utility.SHOW_StringInput( "Group Name:", NEW_NAME, ( name ) => {

					var groupRoot = new GameObject(name);
					groupRoot.transform.SetParent( groupParent, false );
					//groupRoot.transform.localScale = Vector3.one;
					groupRoot.transform.SetSiblingIndex( groupSiblingIndex );
					//********************************//
					//groupRoot.transform.position = center_object.transform.position;
					//groupRoot.transform.rotation = center_object.transform.rotation;
					//********************************//

					//MyMenu_Utils.AssignUniqueName( groupRoot ); // name
					if ( reference.GetComponent<RectTransform>() )     // canvas
					{
						var source = reference.GetComponent<RectTransform>();
						var dest = groupRoot.AddComponent<RectTransform>();
						dest.sizeDelta = source.sizeDelta;
						dest.pivot = source.pivot;
						dest.anchoredPosition3D = source.anchoredPosition3D;
						dest.localRotation = source.localRotation;
						dest.localScale = source.localScale;
						dest.anchorMin = source.anchorMin;
						dest.anchorMax = source.anchorMax;
						dest.offsetMin = source.offsetMin;
						dest.offsetMax = source.offsetMax;
						groupRoot.AddComponent<CanvasRenderer>();
					}

					/*  if ( groupRoot.GetComponentsInParent<Canvas>( true ).Length != 0 )     // canvas
					  {   var rect = groupRoot.AddComponent<RectTransform>();
						  rect.anchorMin = Vector2.zero;
						  rect.anchorMax = Vector2.one;
						  rect.offsetMin = Vector2.zero;
						  rect.offsetMax = Vector2.zero;
						  groupRoot.AddComponent<CanvasRenderer>();
					  }*/

					Undo.RegisterCreatedObjectUndo( groupRoot, groupRoot.name );

					foreach ( var gameObject in onlytop )
					{
						Undo.SetTransformParent( gameObject.transform, groupRoot.transform, groupRoot.name );
					}

					EMX.Utility.SetExpanded( groupRoot.GetInstanceID(), true );

					Selection.objects = onlytop.ToArray();
				} );

				//Selection.objects = new[] { groubObject };
			}

		}
		class MyMenu_GroupWorld : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] public static void INIT() { System.Activator.CreateInstance( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType ); }
			public override bool IsEnable( GameObject clickedObject ) { return true; }
			public override bool NeedExcludeFromMenu( GameObject clickedObject ) { return false; }
			public override int PositionInMenu { get { return 101; } }
			public override string Name { get { return ItemsPlacementFolder + "Group (World center) %#g"; } }


			public override void OnClick( GameObject[] affectedObjectsArray )
			{
				var onlytop = MyMenu_Utils.GetOnlyTopObjects(affectedObjectsArray).OrderBy(go => go.transform.GetSiblingIndex()).ToArray();

				if ( onlytop.Length == 0 ) return;

				var last_object = onlytop[onlytop.Length - 1];

				var groupParent = onlytop[0].transform.parent;
				var groupSiblingIndex = onlytop[0].transform.GetSiblingIndex();

				var reference = last_object;
				var NEW_NAME = reference.name + " Group";

				// Top Object Name Variant
				EMX.Utility.SHOW_StringInput( "Group Name:", NEW_NAME, ( name ) => {

					var groupRoot = new GameObject(name);
					groupRoot.transform.SetParent( groupParent, false );
					//groupRoot.transform.localScale = Vector3.one;
					groupRoot.transform.SetSiblingIndex( groupSiblingIndex );
					//********************************//
					//********************************//

					//  MyMenu_Utils.AssignUniqueName( groupRoot ); // name
					if ( reference.GetComponent<RectTransform>() )     // canvas
					{
						var source = reference.GetComponent<RectTransform>();
						var dest = groupRoot.AddComponent<RectTransform>();
						dest.sizeDelta = source.sizeDelta;
						dest.pivot = source.pivot;
						dest.anchoredPosition3D = source.anchoredPosition3D;
						dest.localRotation = source.localRotation;
						dest.localScale = source.localScale;
						dest.anchorMin = source.anchorMin;
						dest.anchorMax = source.anchorMax;
						dest.offsetMin = source.offsetMin;
						dest.offsetMax = source.offsetMax;
						groupRoot.AddComponent<CanvasRenderer>();
					}

					Undo.RegisterCreatedObjectUndo( groupRoot, groupRoot.name );

					foreach ( var gameObject in onlytop )
					{
						Undo.SetTransformParent( gameObject.transform, groupRoot.transform, groupRoot.name );
					}

					EMX.Utility.SetExpanded( groupRoot.GetInstanceID(), true );

					Selection.objects = onlytop.ToArray();
				} );
				//Selection.objects = new[] { groubObject };
			}

		}
		class MyMenu_GroupAverage : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] public static void INIT() { System.Activator.CreateInstance( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType ); }
			public override bool IsEnable( GameObject clickedObject ) { return true; }
			public override bool NeedExcludeFromMenu( GameObject clickedObject ) { return false; }
			public override int PositionInMenu { get { return 102; } }
			public override string Name { get { return ItemsPlacementFolder + "Group (Average center)"; } }


			public override void OnClick( GameObject[] affectedObjectsArray )
			{
				var onlytop = MyMenu_Utils.GetOnlyTopObjects(affectedObjectsArray).OrderBy(go => go.transform.GetSiblingIndex()).ToArray();

				if ( onlytop.Length == 0 ) return;

				var last_object = onlytop[onlytop.Length - 1];

				var groupParent = onlytop[0].transform.parent;
				var groupSiblingIndex = onlytop[0].transform.GetSiblingIndex();


				var NEW_NAME = last_object.name + " Group";

				// Top Object Name Variant
				EMX.Utility.SHOW_StringInput( "Group Name:", NEW_NAME, ( name ) => {

					var groupRoot = new GameObject(name);
					groupRoot.transform.SetParent( groupParent, false );
					groupRoot.transform.localScale = Vector3.one;
					groupRoot.transform.SetSiblingIndex( groupSiblingIndex );
					//********************************//
					Vector3 center = Vector3.zero;
					Vector3 rot = Vector3.zero;

					foreach ( var item in onlytop )
					{
						center += item.transform.position;
						rot += item.transform.eulerAngles;
					}

					center /= onlytop.Length;
					rot /= onlytop.Length;
					groupRoot.transform.position = center;
					groupRoot.transform.eulerAngles = rot;
					//********************************//

					//MyMenu_Utils.AssignUniqueName( groupRoot ); // name
					if ( groupRoot.GetComponentsInParent<Canvas>( true ).Length != 0 )     // canvas
					{
						var rect = groupRoot.AddComponent<RectTransform>();
						rect.anchorMin = Vector2.zero;
						rect.anchorMax = Vector2.one;
						rect.offsetMin = Vector2.zero;
						rect.offsetMax = Vector2.zero;
						groupRoot.AddComponent<CanvasRenderer>();
					}

					Undo.RegisterCreatedObjectUndo( groupRoot, groupRoot.name );

					foreach ( var gameObject in onlytop )
					{
						Undo.SetTransformParent( gameObject.transform, groupRoot.transform, groupRoot.name );
					}

					EMX.Utility.SetExpanded( groupRoot.GetInstanceID(), true );

					Selection.objects = onlytop.ToArray();
				} );
				//Selection.objects = new[] { groubObject };
			}

		}

		class MyMenu_UnGroup : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] public static void INIT() { System.Activator.CreateInstance( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType ); }
			public override bool IsEnable( GameObject clickedObject ) { return clickedObject.transform.childCount != 0; }
			public override bool NeedExcludeFromMenu( GameObject clickedObject ) { return false; }
			public override int PositionInMenu { get { return 103; } }
			public override string Name { get { return ItemsPlacementFolder + "Ungroup"; } }


			public override void OnClick( GameObject[] affectedObjectsArray )
			{
				var ungroupedObjects = new List<GameObject>();
				var onlytop = MyMenu_Utils.GetOnlyTopObjects(affectedObjectsArray);

				foreach ( var ungroupedRoot in onlytop )
				{
					var ungroupSiblinkIndex = ungroupedRoot.transform.GetSiblingIndex();
					var ungroupParent = ungroupedRoot.transform.parent;
					var undoName = ungroupedRoot.name;

					for ( int i = ungroupedRoot.transform.childCount - 1; i >= 0; i-- )
					{
						var o = ungroupedRoot.transform.GetChild(i);
						Undo.SetTransformParent( o.transform, ungroupParent, "Remove " + undoName );

						if ( !Application.isPlaying ) Undo.RegisterFullObjectHierarchyUndo( o, "Remove " + undoName );

						o.SetSiblingIndex( ungroupSiblinkIndex );

						if ( !Application.isPlaying ) EditorUtility.SetDirty( o );

						ungroupedObjects.Add( o.gameObject );
					}

					if ( !Application.isPlaying ) Undo.DestroyObjectImmediate( ungroupedRoot ); else UnityEngine.Object.Destroy( ungroupedRoot );
				}

				Selection.objects = ungroupedObjects.ToArray();
			}

		}

		#endregion





		#region ITEM 200-203 - Sibling

		class MyMenu_Sibling0 : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] public static void INIT() { System.Activator.CreateInstance( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType ); }
			public override bool IsEnable( GameObject clickedObject ) { return true; }
			public override bool NeedExcludeFromMenu( GameObject clickedObject ) { return false; }
			public override int PositionInMenu { get { return 200; } }
			public override string Name { get { return ItemsPlacementFolder + "Sibling/Set previous sibling index %'"; } }


			public override void OnClick( GameObject[] affectedObjectsArray )
			{
				var obs = affectedObjectsArray.Select(g => g.transform).ToArray();

				if ( obs.Length == 0 ) return;

				obs = obs.OrderBy( o => o.GetSiblingIndex() ).ToArray();
				List<Transform> moveBack = new List<Transform>();

				foreach ( var item in obs.Select( o => new { sib = o.GetSiblingIndex(), transform = o } ).ToArray() )
				{
					var sib = item.sib - 1;
					Undo.SetTransformParent( item.transform, item.transform.parent, "Set previous sibling index" );

					if ( sib < 0 ) moveBack.Add( item.transform );

					item.transform.SetSiblingIndex( sib );
				}

				foreach ( var transform in moveBack )
				{
					transform.SetAsFirstSibling();
				}
			}
		}
		class MyMenu_Sibling1 : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] public static void INIT() { System.Activator.CreateInstance( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType ); }
			public override bool IsEnable( GameObject clickedObject ) { return true; }
			public override bool NeedExcludeFromMenu( GameObject clickedObject ) { return false; }
			public override int PositionInMenu { get { return 201; } }
			public override string Name { get { return ItemsPlacementFolder + "Sibling/Set next sibling index %/"; } }


			public override void OnClick( GameObject[] affectedObjectsArray )
			{
				var obs = affectedObjectsArray.Select(g => g.transform).ToArray();

				if ( obs.Length == 0 ) return;

				obs = obs.OrderByDescending( o => o.GetSiblingIndex() ).ToArray();
				List<Transform> moveBack = new List<Transform>();

				foreach ( var item in obs.Select( o => new { sib = o.GetSiblingIndex(), transform = o } ).ToArray() )
				{
					var sib = item.sib + 1;
					Undo.SetTransformParent( item.transform, item.transform.parent, "Set next sibling index" );
					var nned = sib;
					item.transform.SetSiblingIndex( sib );

					if ( nned != item.transform.GetSiblingIndex() ) moveBack.Add( item.transform );
				}

				foreach ( var transform in moveBack )
				{
					transform.SetAsLastSibling();
				}
			}
		}
		class MyMenu_Sibling2 : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] public static void INIT() { System.Activator.CreateInstance( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType ); }
			public override bool IsEnable( GameObject clickedObject ) { return true; }
			public override bool NeedExcludeFromMenu( GameObject clickedObject ) { return false; }
			public override int PositionInMenu { get { return 202; } }
			public override string Name { get { return ItemsPlacementFolder + "Sibling/Set first sibling index %["; } }


			public override void OnClick( GameObject[] affectedObjectsArray )
			{
				var obs = affectedObjectsArray.Select(g => g.transform).ToArray();

				if ( obs.Length == 0 ) return;

				obs = obs.OrderByDescending( o => o.GetSiblingIndex() ).ToArray();

				foreach ( var item in obs )
				{
					Undo.SetTransformParent( item, item.parent, "Set the first sibling index" );
					item.SetAsFirstSibling();
				}
			}
		}
		class MyMenu_Sibling3 : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] public static void INIT() { System.Activator.CreateInstance( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType ); }
			public override bool IsEnable( GameObject clickedObject ) { return true; }
			public override bool NeedExcludeFromMenu( GameObject clickedObject ) { return false; }
			public override int PositionInMenu { get { return 203; } }
			public override string Name { get { return ItemsPlacementFolder + "Sibling/Set last sibling index %]"; } }


			public override void OnClick( GameObject[] affectedObjectsArray )
			{
				var obs = affectedObjectsArray.Select(g => g.transform).ToArray();

				if ( obs.Length == 0 ) return;

				obs = obs.OrderBy( o => o.GetSiblingIndex() ).ToArray();

				foreach ( var item in obs )
				{
					Undo.SetTransformParent( item, item.parent, "Set the last sibling index" );
					item.SetAsLastSibling();
				}
			}
		}



		class MyMenu_ParentClear : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] public static void INIT() { System.Activator.CreateInstance( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType ); }
			public override bool IsEnable( GameObject clickedObject ) { return true; }
			public override bool NeedExcludeFromMenu( GameObject clickedObject ) { return false; }
			public override int PositionInMenu { get { return 204; } }
			public override string Name { get { return ItemsPlacementFolder + "Move to parent &%["; } }


			public override void OnClick( GameObject[] affectedObjectsArray )
			{
				var obs = affectedObjectsArray.Select(g => g.transform).ToArray();

				if ( obs.Length == 0 ) return;

				obs = obs.OrderBy( o => o.GetSiblingIndex() ).ToArray();

				foreach ( var item in obs )
				{
					if ( !item.parent )
					{
						Undo.SetTransformParent( item, item.parent, "Move to parent" );
						item.SetAsLastSibling();
					}

					else
					{
						Undo.SetTransformParent( item, item.parent.parent, "Move to parent" );
						item.SetAsLastSibling();
					}
				}
			}
		}



		class MyMenu_Parenter_SetParent : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] public static void INIT() { System.Activator.CreateInstance( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType ); }
			internal static GameObject _CurrentParent;
			internal static GameObject CurrentParent {
				get {
					if ( !_CurrentParent )
					{
						var previousID = EditorPrefs.GetInt("EMX/Set as parent for moving", -1);
						_CurrentParent = EditorUtility.InstanceIDToObject( previousID ) as GameObject;
					}

					return _CurrentParent;
				}

				set {
					if ( _CurrentParent != value )
					{
						_CurrentParent = value;
						EditorPrefs.SetInt( "EMX/Set as parent for moving", value ? value.GetInstanceID() : -1 );
					}
				}
			}
			public override bool IsEnable( GameObject clickedObject ) { return MyMenu_Parenter_SetParent.CurrentParent != clickedObject; }
			public override bool NeedExcludeFromMenu( GameObject clickedObject ) { return false; }
			public override int PositionInMenu { get { return 210; } }
			public override string Name { get { return ItemsPlacementFolder + "Set as parent for moving %&P"; } }


			public override void OnClick( GameObject[] affectedObjectsArray )
			{
				CurrentParent = affectedObjectsArray[ 0 ];
			}
		}
		class MyMenu_Parenter_MoveToSettedParent : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] public static void INIT() { System.Activator.CreateInstance( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType ); }
			string text { get { return "Set new parent {" + (MyMenu_Parenter_SetParent.CurrentParent ? MyMenu_Parenter_SetParent.CurrentParent.name : "not assigned") + "}"; } }
			public override bool IsEnable( GameObject clickedObject ) { return MyMenu_Parenter_SetParent.CurrentParent && MyMenu_Parenter_SetParent.CurrentParent != clickedObject; }
			public override bool NeedExcludeFromMenu( GameObject clickedObject ) { return false; }
			public override int PositionInMenu { get { return 211; } }
			public override string Name { get { return ItemsPlacementFolder + text + " %&R"; } }


			public override void OnClick( GameObject[] affectedObjectsArray )
			{
				if ( !MyMenu_Parenter_SetParent.CurrentParent ) return;

				var obs = affectedObjectsArray.Select(g => g.transform).ToArray();

				if ( obs.Length == 0 ) return;

				obs = obs.OrderBy( o => o.GetSiblingIndex() ).ToArray();

				foreach ( var item in obs )
				{
					if ( MyMenu_Parenter_SetParent.CurrentParent.transform == item.transform ) continue;

					Undo.SetTransformParent( item, MyMenu_Parenter_SetParent.CurrentParent.transform, text );
					item.SetAsLastSibling();
				}
			}
		}
		#endregion





		#region ITEM 500 - DuplicateNextToObject

		class MyMenu_DuplicateNextToObject : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] public static void INIT() { System.Activator.CreateInstance( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType ); }
			public override bool IsEnable( GameObject clickedObject ) { return true; }
			public override bool NeedExcludeFromMenu( GameObject clickedObject ) { return false; }
			public override int PositionInMenu { get { return 500; } }
			public override string Name { get { return ItemsPlacementFolder + "Duplicate next to object %#d"; } }


			public override void OnClick( GameObject[] affectedObjectsArray )
			{

				var onlytop = MyMenu_Utils.GetOnlyTopObjects(affectedObjectsArray).OrderByDescending(o => o.transform.GetSiblingIndex());

				List<GameObject> clonedObjects = new List<GameObject>();

				foreach ( var gameObject in onlytop )
				{
					var oldSib = gameObject.transform.GetSiblingIndex();
					Selection.objects = new[] { gameObject };
					EMX.Utility.DuplicateSelection();
					var clonedObject = Selection.activeGameObject;
					MyMenu_Utils.AssignUniqueName( clonedObject );
					clonedObject.transform.SetSiblingIndex( oldSib + 1 );
					clonedObjects.Add( clonedObject );
				}

				Selection.objects = clonedObjects.ToArray();

			}
		}

		#endregion




		#region ITEM 750 - Rename
		class MyMenu_Rename : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] public static void INIT() { System.Activator.CreateInstance( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType ); }
			public override bool IsEnable( GameObject clickedObject ) { return true; }
			public override bool NeedExcludeFromMenu( GameObject clickedObject ) { return false; }
			public override int PositionInMenu { get { return 750; } }
			public override string Name { get { return ItemsPlacementFolder + "Multi renamer"; } }


			public override void OnClick( GameObject[] affectedObjectsArray )
			{
				EMX.Utility.SHOW_StringInput( "Find:", EditorPrefs.GetString( "EMX/MultiRenamer/Find", "" ), ( find ) => {
					if ( string.IsNullOrEmpty( find ) ) return;

					EditorPrefs.SetString( "EMX/MultiRenamer/Find", find );
					EMX.Utility.SHOW_StringInput( "Replace:", EditorPrefs.GetString( "EMX/MultiRenamer/Replace", "" ), ( replace ) => {
						EditorPrefs.SetString( "EMX/MultiRenamer/Replace", replace );

						foreach ( var item in affectedObjectsArray )
						{
							if ( !item ) continue;
							Undo.RecordObject( item, "Multi renamer" );
							item.name = item.name.Replace( find, replace );
							MyMenu_Utils.SetDirty( item );
						}
					} );
				} );
			}
		}
		#endregion






		#region ITEM 1000-1001 - ExpandSelecdedObject/CollapseSelecdedObject

		class MyMenu_ExpandSelecdedObject : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] public static void INIT() { System.Activator.CreateInstance( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType ); }
			public override bool IsEnable( GameObject clickedObject ) { return clickedObject.transform.childCount != 0; }
			public override bool NeedExcludeFromMenu( GameObject clickedObject ) { return false; }
			public override int PositionInMenu { get { return 1000; } }
			public override string Name { get { return ItemsPlacementFolder + "Expand selection"; } }


			public override void OnClick( GameObject[] affectedObjectsArray )
			{
				foreach ( var result in affectedObjectsArray.Select( o => o.GetInstanceID() ) )
					EMX.Utility.SetExpandedWithChildren( result, true );
			}

		}


		class MyMenu_CollapseSelecdedObject : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] public static void INIT() { System.Activator.CreateInstance( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType ); }
			public override bool IsEnable( GameObject clickedObject ) { return clickedObject.transform.childCount != 0; }
			public override bool NeedExcludeFromMenu( GameObject clickedObject ) { return false; }
			public override int PositionInMenu { get { return 1001; } }
			public override string Name { get { return ItemsPlacementFolder + "Collapse selection"; } }


			public override void OnClick( GameObject[] affectedObjectsArray )
			{
				foreach ( var result in MyMenu_Utils.GetOnlyTopObjects( affectedObjectsArray ).Select( o => o.GetInstanceID() ) )
					EMX.Utility.SetExpandedWithChildren( result, false );
			}

		}

		#endregion


		class MyMenu_RemoveMissingComponents : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] public static void INIT() { System.Activator.CreateInstance( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType ); }
			bool HasMissingScript( GameObject go ) { return go.GetComponents<Component>().Any( c => !c ); }
			public override bool IsEnable( GameObject clickedObject ) { return HasMissingScript( clickedObject ); }
			public override bool NeedExcludeFromMenu( GameObject clickedObject ) { return false; }
			public override int PositionInMenu { get { return 1010; } }
			public override string Name { get { return ItemsPlacementFolder + "Remove missing scripts"; } }


			public override void OnClick( GameObject[] affectedObjectsArray )
			{
				foreach ( var result in affectedObjectsArray )
				{
					if ( !HasMissingScript( result ) ) continue;

					//Undo.RecordObject( result, "Remove Missing Scripts" );
					Undo.RegisterFullObjectHierarchyUndo( result, "Remove missing scripts" );
#if UNITY_2019_1_OR_NEWER
					GameObjectUtility.RemoveMonoBehavioursWithMissingScript( result );
#else
			var components = result.GetComponents<Component>();
			var serializedObject = new SerializedObject(result);
			var prop = serializedObject.FindProperty("m_Component");
			int r = 0;
			
			for ( int j = 0 ; j < components.Length ; j++ )
			{	if ( components[j] == null )
				{	prop.DeleteArrayElementAtIndex( j - r );
					r++;
				}
			}
			
			serializedObject.ApplyModifiedProperties();
#endif
					if ( !Application.isPlaying ) EditorUtility.SetDirty( result );
				}

				foreach ( var item in affectedObjectsArray.Select( g => g.scene ).Distinct() )
				{
					if ( !item.IsValid() ) continue;

					UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty( item );
				}
			}

		}

		#region ITEM 2000-2001 - ReverseChildOrder/SelectOnlyTopObjects/SelectAllChildren

		class MyMenu_ReverseChildrenOrder : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] public static void INIT() { System.Activator.CreateInstance( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType ); }
			public override bool IsEnable( GameObject clickedObject ) { return clickedObject.transform.childCount > 0; }
			public override bool NeedExcludeFromMenu( GameObject clickedObject ) { return false; }
			public override int PositionInMenu { get { return 2000; } }
			public override string Name { get { return ItemsPlacementFolder + "Reverse order of child sibling indexes"; } }


			public override void OnClick( GameObject[] affectedObjectsArray )
			{
				foreach ( var gameObject in MyMenu_Utils.GetOnlyTopObjects( affectedObjectsArray ) )
				{
					var T = gameObject.transform;

					for ( int i = 0; i < gameObject.transform.childCount; i++ )
					{
						Undo.SetTransformParent( T.GetChild( i ), T.GetChild( i ).transform.parent, "Reverse order of child sibling indexes" );
						T.GetChild( i ).SetAsFirstSibling();
					}
				}
			}

		}

		class MyMenu_SelectOnlyTopObjects : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] public static void INIT() { System.Activator.CreateInstance( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType ); }
			public override bool IsEnable( GameObject clickedObject ) { return Selection.gameObjects.Length >= 2; }
			public override bool NeedExcludeFromMenu( GameObject clickedObject ) { return false; }
			public override int PositionInMenu { get { return 2001; } }
			public override string Name { get { return ItemsPlacementFolder + "Select only top objects"; } }


			public override void OnClick( GameObject[] affectedObjectsArray )
			{
				Selection.objects = MyMenu_Utils.GetOnlyTopObjects( affectedObjectsArray );
			}

		}


		class MyMenu_SelectAllChild : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] public static void INIT() { System.Activator.CreateInstance( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType ); }
			public override bool IsEnable( GameObject clickedObject ) { return clickedObject.transform.childCount != 0; }
			public override bool NeedExcludeFromMenu( GameObject clickedObject ) { return false; }
			public override int PositionInMenu { get { return 2002; } }
			public override string Name { get { return ItemsPlacementFolder + "Select all child"; } }


			public override void OnClick( GameObject[] affectedObjectsArray )
			{
				Selection.objects = affectedObjectsArray.SelectMany( s => s.GetComponentsInChildren<Transform>( true ) ).Select( s => s.gameObject ).ToArray();
			}

		}

		#endregion






		#region - Utils

		static class MyMenu_Utils
		{
			public static void AssignUniqueName( GameObject o )
			{

				var usedNames = new SortedDictionary<string, string>();
				var childList = o.transform.parent
								? new Transform[o.transform.parent.childCount].Select((t, i) => o.transform.parent.GetChild(i))
								: o.scene.GetRootGameObjects().Select(go => go.transform);

				foreach ( var child in childList.Where( child => child != o.transform ) )
				{
					if ( !usedNames.ContainsKey( child.name ) ) usedNames.Add( child.name, child.name );
				}// existing names

				if ( !usedNames.ContainsKey( o.name ) ) return;



				var number = 1;
				var name = o.name;

				var leftBracket = name.IndexOf('(');
				var rightBracket = name.IndexOf(')');

				if ( leftBracket != -1 && rightBracket != -1 && rightBracket - leftBracket > 1 )
				{
					int parseResult;

					if ( int.TryParse( name.Substring( leftBracket + 1, rightBracket - leftBracket - 1 ), out parseResult ) )
					{
						number = parseResult + 1;
						name = name.Remove( leftBracket );
					}
				}// previous value



				name = name.TrimEnd();

				while ( usedNames.ContainsKey( name + " (" + number + ")" ) ) ++number;

				o.name = name + " (" + number + ")"; //result

			}

			public static GameObject[] GetOnlyTopObjects( GameObject[] affectedObjectsArray )
			{
				var converted = affectedObjectsArray.Select(a => new { a, par = a.GetComponentsInParent<Transform>(true).Where(p => p != a.transform) });
				return
					converted.Where( c => c.par.Count( p => affectedObjectsArray.Contains( p.gameObject ) ) == 0 ).
					Select( g => g.a ).ToArray();
			}


			public static void SetDirty( GameObject o )     //  if (Application.isPlaying || EditorSceneManager.GetActiveScene().isDirty || IS_PROJECT()) return;
			{
				if ( !o ) return;
				if ( Application.isPlaying ) return;
				EditorUtility.SetDirty( o );
				var s = o.scene;
				if ( !s.IsValid() ) return;
				if ( s.isLoaded && !s.isDirty ) UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty( s );
			}
		}

		#endregion


	}
}//namespace

#endif

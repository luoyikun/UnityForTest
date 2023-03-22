using System;
using UnityEngine;
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor.Windows
{

	//[InitializeOnLoad]
	public class InputWindow : IWindow
	{
		internal override bool PIN {
			get { return m_PIN; }
			set { m_PIN = value; }
		}

		static float destroyV = -1;
		static bool Integer = false;

		internal static void InitTeger( MousePos rect, string title, Window adapter, Action<string> conform, Action<string> close = null, string textInputSet = "" )
		{
			Integer = true;
			titleWin = title;
			textInput = textInputSet;
			comformAction = conform;
			closeAction = close;

			IWindow.private_Init( rect, typeof( InputWindow ), adapter );
		}

		internal static void Init( MousePos rect, string title, Window adapter, Action<string> conform, Action<string> close = null, string textInputSet = "",
			float destroy = -1, bool? firsL = null ) //  Debug.Log("ASD");
		{

			if ( conform == null )
			{
				if ( Event.current == null && Root.p[ 0 ].DEFAUL_SKIN == null )
				{
					EditorUtility.DisplayDialog( title, textInputSet, "Ok" );
					return; //null
				}
			}

			Root.p[ 0 ].PUSH_UPDATE_ONESHOT( 0, () => {


				if ( rect.type != MousePos.Type.Input_128_68 && rect.type != MousePos.Type.Input_190_68 )
				{
					Debug.LogWarning( "Mismatch type" );
					rect.SetType( MousePos.Type.Input_190_68 );
				}


				Integer = false;
				titleWin = title;
				textInput = textInputSet;
				comformAction = conform;
				destroyV = destroy;
				closeAction = close;
				IWindow result = null;
				float H = rect.Height;
				rect.Width = Root.p[ 0 ].par_e.ADDITIONA_INPUT_WINDOWS_WIDTH;

				if ( adapter.Instance )
					if ( rect.Width > adapter.position.width )
					{
						rect.Width = 1;
						var d = adapter.position.width / rect.Width;
						rect.Width = d;
					}
				/*  if (useClamp)
                  {   rect.y -= EditorGUIUtility.singleLineHeight * 1.5f;
                      rect = WidnwoRect( WidnwoRectType.Full, new Vector2( rect.x, rect.y ), rect.width, rect.height, adapter, savePosition: new Vector2( rect.x, rect.y ));


                  }*/

				if ( conform == null )
				{


					//if (Root.p[0].DEFAUL_SKIN == null) Root.p[0].DEFAUL_SKIN = Root.p[0].GET_SKIN();

					var oldL = Root.p[0].GET_SKIN().label.fontSize;
					tfStyle.fontSize = Root.p[ 0 ].WINDOW_FONT_10();
					//float w1, w2;
					//tfStyle.CalcMinMaxWidth( new GUIContent( textInputSet + " " ), out w1, out w2 );
					var w2 = tfStyle.CalcSize( new GUIContent( textInputSet + "             " ) ).x;
					var targetW = w2 + 10;
					if ( targetW > rect.Width )
					{
						rect.Width = 1;
						var m = targetW / rect.Width;
						rect.Width = m;
						//rect.width = w1 + 10;
					}

					tfStyle.fontSize = oldL;
					var t = titleWin;
					//titleWin = null;
					result = IWindow.private_Init( rect, typeof( InputWindow ), adapter, t );
				}
				else
				{
					//result = IWindow.private_Init( rect, typeof( InputWindow ), adapter );
					result = IWindow.private_Init( rect, typeof( InputWindow ), adapter, "New Description" ) as InputWindow;
				}
				result.SET_NEW_HEIGHT( adapter, H );
				if ( firsL.HasValue ) result.firstLaunch = firsL.Value;

				//return result;
			} );
		}


		static string titleWin;
		static Action<string> comformAction;
		static Action<string> closeAction;
		static string textInput = "";


		protected internal override bool CloseThis()
		{
			var fl = this;
			if ( closeAction != null ) closeAction( textInput );


			if ( !base.CloseThis() )
			{
				if ( fl && fl != null )
				{
					try
					{
						base.Close();
					}
					catch { }
				}
				return true;
			}


			if ( __inputWindow.ContainsKey( typeof( SearchWindow ) ) && __inputWindow[ typeof( SearchWindow ) ] )
			{
				if ( !__inputWindow[ typeof( SearchWindow ) ].PIN ) __inputWindow[ typeof( SearchWindow ) ].Focus();
				//__inputWindow[typeof(FillterData)].CloseThis();
			}
			return true;
		}

		protected override void Update()
		{
			base.Update();
		}

		GUIStyle __inputStyle;

		GUIStyle inputStyle {
			get {
				if ( __inputStyle == null )
				{
					__inputStyle = new GUIStyle( Root.p[ 0 ].label );
				}

				return __inputStyle;
			}
		}

		static GUIStyle __tfStyle;

		static GUIStyle tfStyle {
			get {
				if ( __tfStyle == null )
				{
					__tfStyle = new GUIStyle( Root.p[ 0 ].GET_SKIN().textField );
				}

				return __tfStyle;
			}
		}

		GUIStyle __tButton;

		GUIStyle tButton {
			get {
				if ( __tButton == null )
				{
					__tButton = new GUIStyle( Root.p[ 0 ].button );
					__tButton.alignment = TextAnchor.MiddleCenter;
				}

				return __tButton;
			}
		}

		protected override void OnGUI()
		{
			if ( _inputWindow == null ) return;

			if ( adapter == null )
			{
				CloseThis();
				return;
			}


			base.OnGUI();


			// if (Event.current.type == EventType.keyDown) MonoBehaviour.print(Event.current.keyCode);
			if ( Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape )
			{
				Tools.EventUseFast();
				Root.p[ 0 ]._SKIP_PREFAB_ESCAPE[ adapter.pluginID ] = true;

				closeAction = null;
				CloseThis();
				return;
			}

			if ( Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter) )
			{
				Tools.EventUseFast();
				CloseThis();
				//  try
				{
					if ( comformAction != null ) comformAction( textInput );
				}
				//  catch ( Exception ex )
				//  {
				//      Debug.LogWarning( "Changing Value: " + ex.Message + " " + ex.StackTrace );
				//  }

				EditorGUIUtility.ExitGUI();
				return;
			}


			Root.p[ 0 ].ChangeGUI( false );

			//  var oldL = PluginInstance.GET_SKIN().label.fontSize;
			inputStyle.fontSize = Root.p[ 0 ].WINDOW_FONT_10();
			GUILayout.Label( titleWin + ":", inputStyle );

			if ( comformAction != null )
			{
				GUI.SetNextControlName( "MyTextField" );
				//var oldT = PluginInstance.GET_SKIN().textField.fontSize;
				tfStyle.fontSize = Root.p[ 0 ].WINDOW_FONT_10();
				if ( Integer )
				{
					int pars = 0;
					int.TryParse( textInput, out pars );
					textInput = EditorGUILayout.IntField( pars, tfStyle ).ToString();
				}
				else textInput = EditorGUILayout.TextField( textInput, tfStyle );
			}
			else // foreach (var t in textInput.Split('\n'))
			{ //  {
			  // EditorGUILayout.LabelField( textInput );
				EditorGUILayout.LabelField( textInput );
				// GUILayout.Label( titleWin , inputStyle );

				// }
			}


			//  var oldS = PluginInstance.GET_SKIN().button.fontSize;
			tButton.fontSize = Root.p[ 0 ].WINDOW_FONT_12();
			var res = GUILayout.Button("Ok", tButton);
			Root.p[ 0 ].RestoreGUI();
			if ( res )
			{
				//try

				//////catch ( Exception ex )
				//{
				//	Debug.LogWarning( "Changing Value: " + ex.Message + " " + ex.StackTrace );
				//}

				CloseThis();

				{
					if ( comformAction != null ) comformAction( textInput );
				}
			}
			// PluginInstance.GET_SKIN().button.fontSize = oldS;




			{ //  wasFocus = true;
				EditorGUI.FocusTextInControl( "MyTextField" );
				// GUI.FocusControl("MyTextField");
			}
			/*    matColor = EditorGUI.ColorField(new Rect(3, 3, position.width - 6, 15), "New Color:", matColor);
                if (GUI.Button(new Rect(3, 25, position.width - 6, 30), "Change"))
                    ChangeColors();*/

			if ( destroyV != -1 && Event.current.type == EventType.Repaint )
			{
				Repaint();
				destroyV -= Root.p[ 0 ].deltaTime;
				if ( destroyV < 0 )
				{
					Root.p[ 0 ].PUSH_UPDATE_ONESHOT( 0, () => CloseThis() );
					/*adapter.OneFrameActionOnUpdate = true;
                    adapter.OneFrameActionOnUpdateAC += ( ) => { CloseThis(); };*/
				}
			}
		}
	}

}
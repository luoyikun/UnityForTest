using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



namespace EMX.HierarchyPlugin.Editor.Mods
{

	class Mod_Audio : RightModBaseClass
    {
        /*  internal override bool enableOverride()
          {
              return !Adapter.LITE;
          }*/
        //  internal override string enableOverrideMessage() { return " (Pro Only)"; }

        public Mod_Audio( int restWidth, int sibbildPos, bool enable, PluginInstance adapter ) : base( restWidth, sibbildPos, enable, adapter )
        {

        }


        internal override void Subscribe( EditorSubscriber sbs )
        {
            // base.Subscribe( sbs );
            sbs.OnHierarchyChanged += Clear;
        }


        internal static AudioSource currentPlay;
        static GUIContent content = new GUIContent();
        Dictionary<int, AudioSource> audio_cache = new Dictionary<int, AudioSource>();

        void Clear()
        {
            audio_cache.Clear();
        }

        static Action playingAct;

        static void Updater()
        {
            if ( playingAct != null )
            {
                playingAct();
            }
        }



        internal override void ModuleCommonGenericMenu( GenericMenu menu, GameObject activeGo, object _c, string sub = "" )
        {
        }


        internal static void PlayAudio( AudioSource aud, Mod_Audio mod = null )
        {
            if ( aud.isPlaying )
            {
                aud.Stop();
                if ( playingAct != null )
                {
                    playingAct = null;
                    EditorApplication.update -= Updater;
                }
            }
            else if ( aud.clip != null )
            {
                if ( !aud.enabled )
                {
                    Debug.LogWarning( "Can not play a disabled audio source\nUnityEngine.AudioSource:Play()" );
                }
                else
                {
                    if ( currentPlay != null && currentPlay.isPlaying ) currentPlay.Stop();
                    currentPlay = null;
                    aud.Play();
                    currentPlay = aud;

                    var capturePlay = currentPlay;
                    EditorApplication.update -= Updater;
                    EditorApplication.update += Updater;
                    playingAct = () => {
                        if ( capturePlay == null || !currentPlay.isPlaying )
                        {
                            playingAct = null;
                            if ( mod != null )
                            {
                                mod.ResetStack();
                                mod.adapter.RepaintWindowInUpdate( 0 );
                            }

                            EditorApplication.update -= Updater;
                        }
                    };
                }
            }
        }

        public override void Draw()
        {
            if ( !START_DRAW( drawRect, adapter.o ) ) return;

            // var o = adapter.o.go;

            if ( !audio_cache.ContainsKey( adapter.o.id ) ) audio_cache.Add( adapter.o.id, adapter.o.go.GetComponent<AudioSource>() );
            // if (audio_cache[o.GetInstanceID()] == null) return width;
            var aud = audio_cache[adapter.o.id];
            if ( !aud )
            {
                END_DRAW( adapter.o , savedData.temp_i );
                return;
            }

            var oldW = drawRect.width;
            var oldH = drawRect.height;
            drawRect.width = drawRect.height = 12;
            drawRect.x += (oldW - drawRect.width) / 2;
            drawRect.y += (oldH - drawRect.width) / 2;
            // Adapter.DrawTexture( drawRect, adapter.GetIcon( aud.clip == null ? "AUDIOPLAYLOCK" : aud.isPlaying ? "AUDIOSTOP" : "AUDIOPLAY" ) );
            Draw_GUITexture( drawRect, adapter.GetNewIcon( NewIconTexture.RightMods, aud.clip == null ? "AUDIOPLAYLOCK" : aud.isPlaying ? "AUDIOSTOP" : "AUDIOPLAY" ), USE_GO: true );

            //if ( !o.activeInHierarchy ) Adapter.FadeRect( drawRect );
            /* var r = drawRect;
             r.width = 100;
             r.height = EditorGUIUtility.singleLineHeight;
             EditorGUI.HelpBox(r, "GameObject not active", MessageType.Warning);*/
            drawRect.y -= 2;
            content.tooltip = aud.isPlaying ? "Stop AudioClip" : "Play AudioClip";
            if ( aud.playOnAwake ) content.tooltip += "\n (PlayOnAwake Enable)";
            if ( aud.loop ) content.tooltip += "\n (Loop Enable)";


            /* if ( adapter.ModuleButton( drawRect, content, true ) )
             {
             }*/
            str.aud = aud;

            Draw_ModuleButton( drawRect, content, BUTTON_ACTION_HASH, true, str, drawPointer: true );

            if ( currentPlay != null && currentPlay.isPlaying )
            {
                adapter.RepaintWindowInUpdate( 0 );
            }


            END_DRAW( adapter.o, savedData.temp_i );

		}

		argsS str;

        struct argsS
        {
            internal AudioSource aud;
        }

        DrawStackMethodsWrapper __BUTTON_ACTION_HASH = null;

        DrawStackMethodsWrapper BUTTON_ACTION_HASH {
            get { return __BUTTON_ACTION_HASH ?? (__BUTTON_ACTION_HASH = new DrawStackMethodsWrapper( BUTTON_ACTION )); }
        }

        void BUTTON_ACTION( Rect worldOffset, Rect inputRect, DrawStackMethodsWrapperData data, HierarchyObject _o )
        {
            audio_cache[ _o.id ] = _o.go.GetComponent<AudioSource>();

            var str = (argsS) data.args;
            var aud = str.aud;
            if ( EVENT.button == adapter.MOUSE_BUTTON_0 && _o.go.activeInHierarchy )
            {
                PlayAudio( aud );
            }

            if ( EVENT.button == adapter.MOUSE_BUTTON_1 )
            {
                Tools.EventUse();
                var mp = new MousePos(EVENT.mousePosition, MousePos.Type.Search_356_0, !callFromExternal(), adapter);

                Windows.SearchWindow.Init( mp, SearchHelper, typeof( AudioSource ).Name,
                    Validate( _o ) ? CallHeaderFiltered( audio_cache[ _o.id ].clip ) : CallHeader(),
                    this, adapter.window, _o );


                /*  int[] contentCost = new int[0];
                  GameObject[] obs = new GameObject[0];
                  if (EditorSceneManager.GetActiveScene().rootCount != 0) CallHeader(out obs, out contentCost);
                
                
                  FillterData.Init(EVENT.mousePosition, SearchHelper, typeof(AudioSource).Name, obs, contentCost, null, this);*/
            }

            ResetStack();
        }

        bool Validate( HierarchyObject _o )
        {
            var o = _o.go;
            var res = o.GetComponent<AudioSource>();
            if ( res )
            {
                if ( !audio_cache.ContainsKey( adapter.o.id ) ) audio_cache.Add( adapter.o.id, o.GetComponent<AudioSource>() );
                else audio_cache[ adapter.o.id ] = res;
            }

            return res != null;
        }

        int ToContentCost( HierarchyObject o, int i )
        {
            var aud = o.go.GetComponent<AudioSource>();
            var cost = i;
            if ( aud.clip == null ) cost += 10000;
            if ( !o.go.activeInHierarchy ) cost += 1000000;
            return cost;
        }



        /* FillterData.Init(EVENT.mousePosition, SearchHelper, LayerMask.LayerToName(o.layer),
                     Validate(o) ?
                     CallHeaderFiltered(LayerMask.LayerToName(o.layer)) :
                     CallHeader(),
                     this);*/
        /** CALL HEADER */
        internal override Windows.SearchWindow.FillterData_Inputs CallHeader()
        {
            var result = new Windows.SearchWindow.FillterData_Inputs(callFromExternal_objects)
            {
                Valudator = Validate,
                SelectCompareString = (d, i) => audio_cache[d.id] && audio_cache[d.id].clip != null ? audio_cache[d.id].clip.name : "",
                SelectCompareCostInt = ToContentCost
            };
            return result;
        }

        internal Windows.SearchWindow.FillterData_Inputs CallHeaderFiltered( AudioClip filter )
        {
            var result = CallHeader();
            result.Valudator = s => Validate( s ) && audio_cache[ s.id ].clip == filter;
            return result;
        }
        /** CALL HEADER */



        /*   internal override bool CallHeader(out GameObject[] obs, out int[] contentCost)
           {
               obs = Utilities.AllSceneObjects().Where(Validate).ToArray();
               contentCost = obs.Select(ToContentCost).ToArray();
               return true;
           }*/
    }


    /*   internal static int INT_COMPARE( string str )
	   {
		   return String.Compare( str, "", StringComparison.Ordinal );
	   }*/
}


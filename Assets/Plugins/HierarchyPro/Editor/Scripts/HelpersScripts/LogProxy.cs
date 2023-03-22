using System;
using UnityEngine;
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor
{

	internal class LogProxy
    {
        // Adapter adapter;
        static void Push( string message, LogType type )
        {

            EditorApplication.CallbackFunction ac = null;
            ac = ( ) =>
            {
                message = "[" + Root.HierarchyPro + "] " + message;
                EditorApplication.update -= ac;
                switch ( type )
                {
                    case LogType.Error:
                        Debug.LogError( message );
                        break;
                    case LogType.Assert:
                        Debug.LogAssertion( message );
                        break;
                    case LogType.Warning:
                        Debug.LogWarning( message );
                        break;
                    case LogType.Log:
                        Debug.Log( message );
                        break;
                    //case LogType.Exception:
                    default:
                        throw new ArgumentOutOfRangeException( "type", type, null );
                }
            };
            EditorApplication.update += ac;
            // this.adapter = adapter;
            // #if !UNITY_EDITOR

            /*  Action ac = () =>
              {   // #endif

                  if ( messages.Count != 0 )
                  {
                      foreach ( var kp in messages.ToDictionary( k => k.Key, v => v.Value ) )
                      {
                          var message = "[" + Root.PN + "] " + kp.Key;
                          var type = kp.Value;
                          switch ( type )
                          {
                              case LogType.Error:
                                  Debug.LogError( message );
                                  break;
                              case LogType.Assert:
                                  Debug.LogAssertion( message );
                                  break;
                              case LogType.Warning:
                                  Debug.LogWarning( message );
                                  break;
                              case LogType.Log:
                                  Debug.Log( message );
                                  break;
                              //case LogType.Exception:
                              default:
                                  throw new ArgumentOutOfRangeException( "type", type, null );
                          }
                      }
                      messages.Clear();
                  }
                  EditorApplication.update -= ac;
                  //  #if !UNITY_EDITOR
              };
              EditorApplication.update += ac;*/
            //   endif
        }

        //  Dictionary<string, LogType> messages = new Dictionary<string, LogType>();

        internal static void LogError( string message )
        {
            Push( message, LogType.Error );
        }
        internal static void LogAssertion( string message )
        {
            Push( message, LogType.Assert );
        }
        internal static void LogWarning( string message )
        {
            Push( message, LogType.Warning );
        }
        internal static void Log( string message )
        {
            Push( message, LogType.Log );
        }
    }
}

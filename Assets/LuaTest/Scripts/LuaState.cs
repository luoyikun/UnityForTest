using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

namespace LuaInterface
{
	public class MonoPInvokeCallbackAttribute : System.Attribute
	{
		private Type type;
		public MonoPInvokeCallbackAttribute( Type t )
		{
			type = t;
		}
	}

	public class LuaState : IDisposable
	{
		private IntPtr _L;

		public IntPtr L { get { return _L; } }

		public LuaState()
		{
			_L = LuaDLL.luaL_newstate();
			LuaDLL.luaL_openlibs( _L );
			LuaDLL.lua_atpanic( _L, Panic );
			LuaDLL.lua_pushcfunction( _L, Print );
			LuaDLL.lua_setglobal( _L, "print" );

			InitLuaPath();
			Debug.Log( "InitLuaState" );
		}

		private void InitLuaPath()
		{
			InitPackagePath();
			if( !Directory.Exists( LuaFileUtils.luaDir ) )
			{
				string msg = string.Format( "luaDir path not exists: {0}, configer it in LuaConst.cs", LuaFileUtils.luaDir );
				throw new Exception( msg );
			}

			AddSearchPath( LuaFileUtils.luaDir );
		}

		public void InitPackagePath()
		{
			//把package压栈
			LuaDLL.lua_getglobal( _L, "package" );
			//把package.path压栈
			LuaDLL.lua_getfield( _L, -1, "path" );
			string current = LuaDLL.lua_tostring( _L, -1 );
			string[] paths = current.Split( ';' );

			for( int i = 0; i < paths.Length; i++ )
			{
				if( !string.IsNullOrEmpty( paths[i] ) )
				{
					string path = paths[i].Replace( '\\', '/' );
					LuaFileUtils.instance.AddSearchPath( path );
				}
			}

			//把''压栈
			LuaDLL.lua_pushstring( _L, "" );
			//把package.path值设为''，同时把''出栈
			LuaDLL.lua_setfield( _L, -3, "path" );
			//把package和package.path出栈
			LuaDLL.lua_pop( _L, 2 );
		}

		public void AddSearchPath( string fullPath )
		{
			if( !Path.IsPathRooted( fullPath ) )
			{
				throw new Exception( fullPath + " is not a full path" );
			}

			fullPath = ToPackagePath( fullPath );
			LuaFileUtils.instance.AddSearchPath( fullPath );
		}

		string ToPackagePath( string path )
		{
			var sb = new StringBuilder();
			sb.Append( path );
			sb.Replace( '\\', '/' );

			if( sb.Length > 0 && sb[sb.Length - 1] != '/' )
			{
				sb.Append( '/' );
			}

			sb.Append( "?.lua" );
			return sb.ToString();
		}

		public void Dispose()
		{
			if( IntPtr.Zero != _L )
			{
				LuaDLL.lua_close( _L );
			}
			_L = IntPtr.Zero;
		}

		public void DoFile( string fileName )
		{
			byte[] buffer = LuaFileUtils.instance.ReadFile( fileName );
			var fullPath = GetLuaChunkName( fileName );
			LuaLoadBuffer( buffer, fullPath );
		}

		public void DoString( string chunk, string chunkName = "LuaState.cs" )
		{
			byte[] buffer = Encoding.UTF8.GetBytes( chunk );
			LuaLoadBuffer( buffer, chunkName );
		}

		private string GetLuaChunkName( string name )
		{
			name = LuaFileUtils.instance.GetFileFullPath( name );
			return "@" + name;
		}

		private void LuaLoadBuffer( byte[] buffer, string chunkName )
		{
			//TODO: 错误处理
			if( LuaDLL.luaL_loadbufferx( _L, buffer, buffer.Length, chunkName, null ) == 0 )
			{
				LuaDLL.lua_pcall( _L, 0, -1, 0 );
			}
		}

		[MonoPInvokeCallbackAttribute( typeof( LuaCSFunction ) )]
		private static int Print( IntPtr L )
		{
			try
			{
				int n = LuaDLL.lua_gettop( L );
				var sb = new StringBuilder();

				//获得当前运行的函数的上一个调用层的信息，返回行数，把调用层的名称入栈
				int line = LuaDLL.jlua_where( L, 1 );
				string filename = LuaDLL.lua_tostring( L, -1 );

				LuaDLL.lua_settop( L, n );
				int offset = filename[0] == '@' ? 1 : 0;
				sb.Append( '[' ).Append( filename, offset, filename.Length - offset ).Append( ':' ).Append( line ).Append( "]:" );

				for( int i = 1; i <= n; i++ )
				{
					if( i > 1 ) sb.Append( "    " );

					if( LuaDLL.lua_isstring( L, i ) == 1 )
					{
						sb.Append( LuaDLL.lua_tostring( L, i ) );
					}
					else if( LuaDLL.lua_isnil( L, i ) )
					{
						sb.Append( "nil" );
					}
					else if( LuaDLL.lua_isboolean( L, i ) )
					{
						sb.Append( LuaDLL.jlua_toboolean( L, i ) ? "true" : "false" );
					}
					else
					{
						IntPtr p = LuaDLL.lua_topointer( L, i );

						if( p == IntPtr.Zero )
						{
							sb.Append( "nil" );
						}
						else
						{
							sb.Append( LuaDLL.luaL_typename( L, i ) ).Append( ":0x" ).Append( p.ToString( "X" ) );
						}
					}
				}
				Debug.Log( sb.ToString() );
				return 0;
			}
			catch( Exception e )
			{
				throw e;
			}
		}

		[MonoPInvokeCallbackAttribute( typeof( LuaCSFunction ) )]
		private static int Panic( IntPtr L )
		{
			string reason = string.Format( "PANIC: unprotected error in call to Lua API ({0})", LuaDLL.lua_tostring( L, -1 ) );

			throw new Exception( reason );
		}
	}

}

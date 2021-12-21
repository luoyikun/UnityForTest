using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LuaInterface
{
	public class LuaIndexes
	{
		public static int LUA_REGISTRYINDEX = -10000;
	}

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA_10_0
    //UnmanagedFunctionPointer表面这个委托不受C#管理
    //CallConvention(调用约定)：决定函数参数传送时入栈和出栈的顺序，由调用者还是被调用者把参数弹出栈，以及编译器用来识别函数名字的修饰约定
    //Cdecl表示由调用方把参数出栈
    [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
    public delegate int LuaCSFunction( IntPtr luaState );
#else
	public delegate int LuaCSFunction( IntPtr luaState );
#endif

	public class LuaDLL
	{
		const string LUADLL = "Lua";

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern int luaL_callmeta( IntPtr luaState, int stackPos, string name );

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern IntPtr luaL_newstate();
		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern void lua_close( IntPtr luaState );
		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern void luaL_openlibs( IntPtr luaState );
		/*
		 * basic stack manipulation
		 */
		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern int lua_gettop( IntPtr luaState );
		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern void lua_settop( IntPtr luaState, int top );

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern void lua_getglobal( IntPtr luaState, string name );

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern void lua_setglobal( IntPtr luaState, string name );

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern void lua_getfield( IntPtr luaState, int index, string name );

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern void lua_setfield( IntPtr luaState, int index, string name );

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern double lua_tonumberx( IntPtr luaState, int index, IntPtr x );
		public static double lua_tonumber( IntPtr luaState, int index )
		{
			return lua_tonumberx( luaState, index, IntPtr.Zero );
		}

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern int luaL_loadbufferx( IntPtr luaState, byte[] buff, int size, string name, string mode );

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern int luaL_loadfilex( IntPtr luaState, string filename, string mode );
		public static int luaL_loadfile( IntPtr luaState, string filename )
		{
			return luaL_loadfilex( luaState, filename, null );
		}

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern int luaL_ref( IntPtr luaState, int t );
		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern void luaL_unref( IntPtr luaState, int registryIndex, int reference );

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern int lua_rawgeti( IntPtr luaState, int idx, int n );

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern void lua_call( IntPtr luaState, int nArgs, int nResults );

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern int lua_pcallk( IntPtr luaState, int nArgs, int nResults, int errfunc, int ctx, IntPtr k );
		public static int lua_pcall( IntPtr luaState, int nArgs, int nResults, int errfunc )
		{
			return lua_pcallk( luaState, nArgs, nResults, errfunc, 0, IntPtr.Zero );
		}

		/// <summary>
		/// 弹出栈顶amount个数量的值
		/// </summary>
		/// <param name="luaState"></param>
		/// <param name="amount"></param>
		public static void lua_pop( IntPtr luaState, int amount )
		{
			LuaDLL.lua_settop( luaState, -( amount ) - 1 );
		}

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern IntPtr lua_atpanic( IntPtr L, LuaCSFunction panicf );
		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern IntPtr lua_pushstring( IntPtr L, string s );

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern void lua_pushcclosure( IntPtr L, LuaCSFunction f, int n );
		public static void lua_pushcfunction( IntPtr L, LuaCSFunction f )
		{
			lua_pushcclosure( L, f, 0 );
		}

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern void lua_pushvalue( IntPtr L, int index );

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern void lua_pushnumber( IntPtr luaState, double number );


		/*
		* access functions (stack -> C)
		*/
		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern LuaTypes lua_type( IntPtr L, int index );

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern IntPtr lua_typename( IntPtr luaState, int type );
		public static string lua_typenamestr( IntPtr luaState, LuaTypes type )
		{
			IntPtr p = lua_typename( luaState, (int)type );
			return Marshal.PtrToStringAnsi( p );
		}
		public static string luaL_typename( IntPtr luaState, int stackPos )
		{
			return LuaDLL.lua_typenamestr( luaState, LuaDLL.lua_type( luaState, stackPos ) );
		}

		public static bool lua_isnil( IntPtr L, int index )
		{
			return ( lua_type( L, index ) == LuaTypes.LUA_TNIL );
		}
		public static bool lua_isboolean( IntPtr L, int index )
		{
			return lua_type( L, index ) == LuaTypes.LUA_TBOOLEAN;
		}

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern int lua_isnumber( IntPtr luaState, int index );

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern int lua_isstring( IntPtr luaState, int index );

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern int lua_iscfunction( IntPtr luaState, int index );

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern int lua_toboolean( IntPtr luaState, int index );

		public static bool jlua_toboolean(IntPtr luaState,int index )
		{
			return lua_toboolean( luaState, index ) > 0;
		}

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern IntPtr lua_tolstring( IntPtr luaState, int index, out int len );

		public static string lua_tostring( IntPtr luaState, int index )
		{
			int len = 0;
			IntPtr str = lua_tolstring( luaState, index, out len );
			if( str != IntPtr.Zero )
			{
				string result = Marshal.PtrToStringAnsi( str, len );
				if( result == null )
				{
					byte[] buffer = new byte[len];
					Marshal.Copy( str, buffer, 0, len );
					return Encoding.UTF8.GetString( buffer );
				}
				return result;
			}
			return null;
		}

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern IntPtr lua_topointer( IntPtr L, int index );

		#region extension function
		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern int jlua_where( IntPtr luaState, int level );

		[DllImport( LUADLL, CallingConvention = CallingConvention.Cdecl )]
		public static extern int jlua_get_registry_index();
		#endregion
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;

//例子2: 学习lua文件的加载过程，error的处理
public class LearnLoadFile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		var L = LuaDLL.luaL_newstate();

		var path = Application.dataPath + "/Examples/02_LoadFile/02.lua";

		var errorPath = Application.dataPath + "/02.lua";

		//返回结果不是0代表加载失败
		var result = LuaDLL.luaL_loadfile( L, path );

		if( result != 0 )
		{
			Debug.LogError( LuaDLL.lua_tostring( L, -1 ) );
		}
		//LuaDLL.luaL_dofile(L, path);
		if( LuaDLL.lua_pcall( L, 0, 0, 0 ) != 0 )
		{
			Debug.LogError( LuaDLL.lua_tostring( L, -1 ) );
		}

		LuaDLL.lua_getglobal( L, "a" );
		//LuaDLL.lua_getglobal( L, "b" );
        LuaDLL.lua_pushnumber(L, 123);
       
        LuaDLL.lua_setglobal(L, "b");
        if ( LuaDLL.lua_isnumber( L, -2 ) == 1 )
		{
			Debug.Log( LuaDLL.lua_tonumber( L, -2 ) );
		}

		if( LuaDLL.lua_isnumber( L, -1 ) == 1 )
		{
			Debug.Log( LuaDLL.lua_tonumber( L, -1 ) );
		}

		if( LuaDLL.lua_isnumber( L, -1 ) == 1 )
		{
			Debug.Log( LuaDLL.lua_tonumber( L, -2 ) );
		}

		LuaDLL.lua_pop( L, 1 );
		if( LuaDLL.lua_isnumber( L, -1 ) == 1 )
		{
			Debug.Log( LuaDLL.lua_tonumber( L, -1 ) );
		}

		LuaDLL.lua_close( L );
	}

}

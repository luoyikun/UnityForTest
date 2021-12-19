using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearnRegistry : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LuaState lua = new LuaState();
        LuaIndexes.LUA_REGISTRYINDEX = LuaDLL.jlua_get_registry_index();

        lua.DoFile( "04" );
        LuaDLL.lua_getglobal( lua.L, "foo" );
        Debug.LogFormat( "StackSize:{0},top type is {1}", LuaDLL.lua_gettop( lua.L ), LuaDLL.lua_type( lua.L, -1 ) );
        //存放函数到注册表
        int key =  LuaDLL.luaL_ref( lua.L, LuaIndexes.LUA_REGISTRYINDEX );
        Debug.LogFormat( "StackSize:{0}", LuaDLL.lua_gettop( lua.L ) );

        LuaDLL.lua_rawgeti( lua.L, LuaIndexes.LUA_REGISTRYINDEX, key );
        Debug.LogFormat( "StackSize:{0},top type is {1}", LuaDLL.lua_gettop( lua.L ), LuaDLL.lua_type( lua.L, -1 ) );
        LuaDLL.lua_pcall( lua.L, 0, 0, 0 );
        Debug.LogFormat( "StackSize:{0}", LuaDLL.lua_gettop( lua.L ) );

        LuaDLL.luaL_unref( lua.L, LuaIndexes.LUA_REGISTRYINDEX, key );
        lua.Dispose();
        lua = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

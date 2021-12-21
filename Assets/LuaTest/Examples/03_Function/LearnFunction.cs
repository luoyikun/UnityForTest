using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LuaInterface;
using UnityEngine;

public class LearnFunction : MonoBehaviour
{
    //xlua/tolua的dll是c写的
    // Start is called before the first frame update
    void Start()
    {
        var L = LuaDLL.luaL_newstate();

        //这三个方法不能同时运行，必须注释掉2个跑另一个
        //CallLuaFunction(L);
        //LuaHelloWorld(L);
        HandleError( L );

        LuaDLL.lua_close( L );
    }

    private void HandleError( IntPtr L )
    {
        LuaDLL.lua_pushcclosure( L, ErrorHandle, 0 ); //把某个函数压入栈

        var errorFuncIndex = LuaDLL.lua_gettop( L );//获得栈顶位置,因为只有一个所以是1   ，从顶到底，越来越小。。。栈底永远是1，栈顶永远是-1
        var path = Application.dataPath + "/Examples/03_Function/03.lua";

        LuaDLL.luaL_loadfile( L, path );//加载文件，未执行
        Debug.Log(LuaDLL.lua_gettop(L));
        LuaDLL.lua_pcall( L, 0, 0, 0 );//执行文件

        LuaDLL.lua_getglobal( L, "addandsub" ); //原来lua有的函数，放入栈顶

        LuaDLL.lua_pushstring(L, "uguy");

        LuaDLL.lua_pushnumber( L, 10 );
        //LuaDLL.lua_pushstring( L, "uguy" );
        //LuaDLL.lua_pushnumber(L, 10);
        LuaDLL.lua_pcall( L, 2, 2, errorFuncIndex ); //这个执行函数，相当于取刚才压入的两个参数，到addandsub 中，再依次往栈顶返回

        Debug.Log( LuaDLL.lua_tonumber( L, -1 ) );
        Debug.Log( LuaDLL.lua_tonumber( L, -2 ) );
    }

    private void LuaHelloWorld( IntPtr L )
    {
        LuaDLL.lua_pushcclosure( L, HelloWorld, 0 );
        LuaDLL.lua_pcall( L, 0, 0, 0 );
    }

    private void CallLuaFunction( IntPtr L )
    {
        var path = Application.dataPath + "/Examples/03_Function/03.lua";

        LuaDLL.luaL_loadfile( L, path );
        LuaDLL.lua_pcall( L, 0, 0, 0 );

        LuaDLL.lua_getglobal( L, "addandsub" );

        LuaDLL.lua_pushnumber( L, 10 );
        LuaDLL.lua_pushnumber( L, 20 );

        if( LuaDLL.lua_pcall( L, 2, 2, 0 ) != 0 )
        {
            Debug.LogError( LuaDLL.lua_tostring( L, -1 ) );
        }

        Debug.Log( LuaDLL.lua_tonumber( L, -1 ) );
        Debug.Log( LuaDLL.lua_tonumber( L, -2 ) );
    }

    [MonoPInvokeCallbackAttribute( typeof( LuaCSFunction ) )]
    private static int HelloWorld( IntPtr L )
    {
        Debug.Log( "helloworld" );
        return 0;
    }

    [MonoPInvokeCallbackAttribute( typeof( LuaCSFunction ) )]
    private static int ErrorHandle( IntPtr L )
    {
        if( LuaDLL.lua_isstring( L, -1 ) == 1 )
        {
            Debug.LogError( LuaDLL.lua_tostring( L, -1 ) );
            return -1;
        }
        else
        {
            Debug.Log( "Not find error string" );
            return 0;
        }
    }
}

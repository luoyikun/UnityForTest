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
        LuaHelloWorld(L);
        //HandleError( L );


        //---------------------------------------------------lua调用c#函数
        //TestClass obj = new TestClass();

        //LuaState lua = new LuaState();
        //// 注册CLR对象方法到Lua，供Lua调用   typeof(TestClass).GetMethod("TestPrint")
        //lua.RegisterFunction("TestPrint", obj, obj.GetType().GetMethod("TestPrint"));

        //// 注册CLR静态方法到Lua，供Lua调用
        //lua.RegisterFunction("TestStaticPrint", null, typeof(TestClass).GetMethod("TestStaticPrint"));

        //lua.DoString("TestPrint(10)");
        //lua.DoString("TestStaticPrint()");


        LuaDLL.lua_close( L );
    }



    private void HandleError( IntPtr L )
    {
        LuaDLL.lua_pushcclosure( L, ErrorHandle, 0 ); //把某个函数压入栈

        var errorFuncIndex = LuaDLL.lua_gettop( L );//获得栈顶位置,因为只有一个所以是1   ，从顶到底，越来越小。。。栈底永远是1，栈顶永远是-1
        var path = Application.dataPath + "/LuaTest/Examples/03_Function/03.lua";

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
        var path = Application.dataPath + "/LuaTest/Examples/03_Function/03.lua";
        Debug.Log(path);
        LuaDLL.luaL_loadfile( L, path );//执行lua文件，也可以luaL_dostring(L, string);执行lua字符串代码
        LuaDLL.lua_pcall( L, 0, 0, 0 );

        LuaDLL.lua_getglobal( L, "addandsub");//获取全局变量的addandsub的值,并将其放入栈顶

        LuaDLL.lua_pushnumber( L, 10 );//栈顶压入10
        LuaDLL.lua_pushnumber( L, 20 );//栈顶压入20

        if( LuaDLL.lua_pcall( L, 2, 2, 0 ) != 0 ) //执行函数，输入2个值，返回2个值，不为0，有错误
        {
            Debug.LogError( LuaDLL.lua_tostring( L, -1 ) );
        }

        Debug.Log( LuaDLL.lua_tonumber( L, -1 ) );//栈-2，为-10
        Debug.Log( LuaDLL.lua_tonumber( L, -2 ) );//栈-1，为30，，因为返回值是先入栈
        
    }

    //MonoPInvokeCallbackAttribute 用来标记这个方法是由C或者C++来调用的
    [MonoPInvokeCallbackAttribute( typeof( LuaCSFunction ) )]
    private static int HelloWorld( IntPtr L)
    {
        Debug.Log( "helloworld:"  );
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

class TestClass
{
    private int value = 0;

    public void TestPrint(int num)
    {
        value = num;
        Console.WriteLine("CSharp" + value);
    }

    public static void TestStaticPrint()
    {
        Console.WriteLine("TestStaticPrint");
    }
}

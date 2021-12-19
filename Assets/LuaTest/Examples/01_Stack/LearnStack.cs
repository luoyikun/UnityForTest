using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;

//例子1：学习LuaState和Lua中的虚拟栈
//详见博客: https://blog.csdn.net/j756915370/article/details/105779176

public class LearnStack : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var num = 756915370;
        IntPtr L = LuaDLL.luaL_newstate();
        LuaDLL.lua_pushnumber( L, num );
        //等于1说明是True
        if( LuaDLL.lua_isnumber( L, 1 ) == 1 )
        {
            Debug.Log( LuaDLL.lua_tonumber( L, 1 ) );
        }
        LuaDLL.lua_close( L );
    }
}

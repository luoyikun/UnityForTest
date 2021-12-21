using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LuaInterface
{
	public enum LuaTypes
	{
		LUA_TNONE = -1,
		LUA_TNIL = 0,
		LUA_TNUMBER = 3,
		LUA_TSTRING = 4,
		LUA_TBOOLEAN = 1,
		LUA_TTABLE = 5,
		LUA_TFUNCTION = 6,
		LUA_TUSERDATA = 7,
		LUA_TTHREAD = 8,
		LUA_TLIGHTUSERDATA = 2
	}
	//class ObjectTranslator
	//{
	//}
}

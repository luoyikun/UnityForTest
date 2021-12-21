using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace LuaInterface
{
	public class LuaFileUtils
	{
		public static string luaDir = Application.dataPath + "/Lua";

		protected static LuaFileUtils _instance = null;

		public static LuaFileUtils instance
		{
			get
			{
				if( _instance == null )
				{
					_instance = new LuaFileUtils();
				}

				return _instance;
			}
		}

		protected List<string> searchPaths = new List<string>();

		//格式: 路径/?.lua
		public bool AddSearchPath( string path, bool front = false )
		{
			int index = searchPaths.IndexOf( path );

			if( index >= 0 )
			{
				return false;
			}

			if( front )
			{
				searchPaths.Insert( 0, path );
			}
			else
			{
				searchPaths.Add( path );
			}

			return true;
		}

		public virtual byte[] ReadFile( string fileName )
		{
			string path = GetFileFullPath( fileName );
			byte[] str = null;

			if( !string.IsNullOrEmpty( path ) && File.Exists( path ) )
			{
				str = File.ReadAllBytes( path );
			}

			return str;
		}

		public string GetFileFullPath( string fileName )
		{
			if( fileName == string.Empty )
			{
				return string.Empty;
			}

			if( Path.IsPathRooted( fileName ) )
			{
				if( !fileName.EndsWith( ".lua" ) )
				{
					fileName += ".lua";
				}

				return fileName;
			}

			if( fileName.EndsWith( ".lua" ) )
			{
				fileName = fileName.Substring( 0, fileName.Length - 4 );
			}

			string fullPath = null;

			for( int i = 0; i < searchPaths.Count; i++ )
			{
				fullPath = searchPaths[i].Replace( "?", fileName );

				if( File.Exists( fullPath ) )
				{
					return fullPath;
				}
			}

			return null;
		}
	}
}

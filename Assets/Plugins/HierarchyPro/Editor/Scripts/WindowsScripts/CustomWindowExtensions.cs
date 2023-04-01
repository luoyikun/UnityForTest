using System;

namespace EMX
{
	public partial class Utility
	{

		/// <summary>Opens window with input field.
		/// </summary>
		/// <param name="defaultValue">value that will be in the input field.</param>
		/// <param name="OnValueChanged">will be invoked when the value is changed.</param>
		public static void
			SHOW_IntInput(string windowName, int defaultValue, Action<int> OnValueChanged)
		{
			EMX.HierarchyPlugin.Editor.CustomHierarchyMod.m_OpenIntInput_W(windowName, defaultValue, OnValueChanged);
		}

		/// <summary>Opens window with input field.
		/// </summary>
		/// <param name="defaultValue">value that will be in the input field.</param>
		/// <param name="OnValueChanged">will be invoked when the value is changed.</param>
		public static void
			SHOW_IntInput(int defaultValue, Action<int> OnValueChanged)
		{
			EMX.HierarchyPlugin.Editor.CustomHierarchyMod.m_OpenIntInput(defaultValue, OnValueChanged);
		}

		/// <summary>Opens window with input field.
		/// </summary>
		/// <param name="defaultValue">value that will be in the input field.</param>
		/// <param name="OnValueChanged">will be invoked when the value is changed.</param>
		public static void
			SHOW_StringInput(string windowName, string defaultValue, Action<string> OnValueChanged)
		{
			EMX.HierarchyPlugin.Editor.CustomHierarchyMod.m_OpenStringInput_W(windowName, defaultValue, OnValueChanged);
		}

		/// <summary>Opens window with input field.
		/// </summary>
		/// <param name="defaultValue">value that will be in the input field.</param>
		/// <param name="OnValueChanged">will be invoked when the value is changed.</param>
		public static void
			SHOW_StringInput(string defaultValue, Action<string> OnValueChanged)
		{
			EMX.HierarchyPlugin.Editor.CustomHierarchyMod.m_OpenStringInput(defaultValue, OnValueChanged);
		}

		/// <summary>Opens the drop-down menu with the specified parameters.
		/// </summary>
		/// <param name="defaultIndex">designated index of item.</param>
		/// <param name="Items">names of menu items.</param>
		/// <param name="OnIndexChanged">will be invoked when the value is changed.</param>
		/// <param name="OnItemAdded">will be invoked when the value is added.
		/// if OnItemAdded == null then 'add new' menu item won't available.</param>
		public static void
			SHOW_DropDownMenu(int defaultIndex, string[] Items, Action<int> OnIndexChanged, Action<string> OnItemAdded = null)
		{
			EMX.HierarchyPlugin.Editor.CustomHierarchyMod.m_OpenDropDownMenu(defaultIndex, Items, OnIndexChanged, OnItemAdded);
		}
	}
}

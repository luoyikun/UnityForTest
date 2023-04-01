



// All attributes are automatically included in the build like all other attributes, even lines like a [ContextMenu("")], 
// If you do not want to include the [DRAW_IN_HIER] attribute in the final build, just to comment RUNTIME_ALSO line below in DRAW_IN_HIER.cs
#define RUNTIME_ALSO



#if RUNTIME_ALSO || UNITY_EDITOR
using System;
using UnityEngine;

/// <summary>
/// If you do not want to include the [DRAW_IN_HIER] attribute in the final build, just to comment RUNTIME_ALSO line above in DRAW_IN_HIER.cs
/// You also will have to add the '#if UNITY_EDITOR' for each your [DRAW_IN_HIER] attributes
/// So keep in mind, it is better if you also add '#if UNITY_EDITOR' for all attributes like [ContextMenu("")] or [DRAW_IN_HIER] and etc, because, probably, obfuscator will skip these attributes
/// </summary>

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
public class DRAW_IN_HIER : Attribute
{
	public float? width = null;
	public Color? color = null;
	
	public DRAW_IN_HIER()
	{
	}

	public DRAW_IN_HIER(float width)
	{
		this.width = width;
	}

	public DRAW_IN_HIER(float[] color)
	{
		if (color.Length != 4) return;
		this.color = new Color(color[0], color[1], color[2], color[3]);
	}

	public DRAW_IN_HIER(float width, float[] color)
	{
		this.width = width;
		if (color.Length != 4) return;
		this.color = new Color(color[0], color[1], color[2], color[3]);
	}
}
#endif

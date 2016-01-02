using UnityEngine;
using System.Collections;

public abstract class TrashSpawnable : TrashObject, IAlignable {

	#region IAlignable implementation

	public float GetWidth ()
	{
		return CachedSpriteRenderer.bounds.size.x;
	}

	public float GetHeight ()
	{
		return CachedSpriteRenderer.bounds.size.y;
	}

	public float GetDepth ()
	{
		return 0f;
	}

	public FreeScreenUtility.Alignment GetAlignment ()
	{
		return FreeScreenUtility.Alignment.Left;
	}

	#endregion




}

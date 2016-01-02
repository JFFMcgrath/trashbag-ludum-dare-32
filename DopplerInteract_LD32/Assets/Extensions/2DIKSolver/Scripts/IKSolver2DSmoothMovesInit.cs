/*
 *  SmoothMoves tend to destroy the bone hierachy when rebuilding animations.
 *  Therefore use this script on the root to create instances of IKSolver2D
 *  Components on start. - Jesper Taxboel
 * 
 * */
using UnityEngine;
using System.Collections;

public class IKSolver2DSmoothMovesInit : MonoBehaviour
{
	
	[System.Serializable]
	public class AIKInit
	{
		public string target;
		public string chain;
		public bool bendCCW;
		
		
		
	}
	
	public AIKInit[] ik_list;
	
	Transform findRecursively(Transform t, string n)
	{
		if(t.name == n)
		{
			return t;
		}
		foreach(Transform c in t)
		{
			Transform r = findRecursively(c, n);
			if(r != null)
			{
				return r;
			}
		}
		return null;
	}
	
	
	
	void Awake () 
	{
		for(int i=0; i<ik_list.Length;i++)
		{
			Transform target = findRecursively(transform, ik_list[i].target);
			Transform chain = findRecursively(transform, ik_list[i].chain);
			
			if(target != null && chain != null)
			{
				IKSolver2D aik = target.gameObject.AddComponent<IKSolver2D>();
				aik.EndTransform = chain;
				aik.Target = target;
				aik.BendCCW = ik_list[i].bendCCW;
			}
			else
			{
				if(target == null)
				{
					Debug.Log("IKSolver2D Smoothmoves Init: Error finding target: " + ik_list[i].target);
				}
				if(chain == null)
				{
					Debug.Log("IKSolver2D Smoothmoves Init: Error finding chain: " + ik_list[i].chain);
				}
			}
		}
	}
}

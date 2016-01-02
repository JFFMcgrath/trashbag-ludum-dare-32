using System;
using UnityEngine;
using System.Text;
using System.Collections.Generic;

public static class FreeExtensions
{
	
	public static string ToRealString(this Vector3 v){
		
		StringBuilder sb = new StringBuilder();
		
		sb.Append('[');
		
		sb.Append(v.x);
		sb.Append(',');
		sb.Append(v.y);
		sb.Append(',');
		sb.Append(v.z);
		
		sb.Append(']');
		
		return sb.ToString();
		
	}

	public static void AddArray<T>(this List<T> list, T[] range){

		if (range == null) {
			return;
		}

		for (int i = 0; i < range.Length; i++) {

			list.Add (range [i]);

		}

	}
	
	public static Vector3 SafeLerp(Vector3 a, Vector3 b, float n){
	
		Vector3 r = new Vector3();
		
		r.x = SafeLerp(a.x,b.x,n);
		r.y = SafeLerp(a.y,b.y,n);
		r.z = SafeLerp(a.z,b.z,n);
	
		return r;
	
	}
	
	public static float SafeLerp(float a, float b, float n){
	
		float ulx = b;
		float lrx = a;
		float lrng = ulx - lrx;
		float hlrng = n * lrng;
		float res = hlrng + lrx;
		
		return res;
		
	}
	
}



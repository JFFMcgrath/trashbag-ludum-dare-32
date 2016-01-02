using System;
using UnityEngine;

public class FreeCoordinate
{

	public const int NONE = -1;
	public static FreeCoordinate None{
		get{
			return new FreeCoordinate (NONE, NONE, true);
		}
	}

	public int x{
		get;
		private set;
	}

	public int y{
		get;
		private set;
	}

	bool unchangeable {
		get;
		set;
	}

	public FreeCoordinate(){

		this.x = NONE;
		this.y = NONE;

	}

	public FreeCoordinate(FreeCoordinate cloneCoordinate){

		if (cloneCoordinate == null) {
			Debug.Log ("Cloning null coordinate!");
		}

		this.x = cloneCoordinate.x;
		this.y = cloneCoordinate.y;
	
	}

	public FreeCoordinate(int x, int y) : this(x,y,false){

	}

	public FreeCoordinate(int x, int y, bool unchangeable){
		
		this.x = x;
		this.y = y;
		this.unchangeable = unchangeable;
		
	}
	
	public void Reset(int x, int y){
	
		if (unchangeable) {
			Debug.LogError ("Cannot change fixed coordinate.");
			return;
		}

		this.x = x;
		this.y = y;
	
	}

	public float GetRandomValue(){

		return UnityEngine.Random.Range (this.x, this.y + 1);

	}

	public float Lerp(float n){

		return Mathf.Lerp (this.x, this.y, n);

	}

	public FreeCoordinate PivotLerp(float n){

		float mid = Lerp (0.5f);

		float range = (float)GetRange () * 0.5f;

		float newRange = range * n;

		float new_x = mid - newRange;
		float new_y = mid + newRange;

		return new FreeCoordinate ((int)new_x, (int)new_y);

	}

	public int GetRange(){
	
		return Mathf.Abs(this.x - this.y);
	
	}
	
	public bool Contains(int v){
	
		if(this.y == -1 && this.y == -1){
			return true;
		}
	
		if(this.y == -1){
			
			return v >= this.x;
			
		}
		
		if(this.x == -1){
		
			return v <= this.y;
		
		}
	
		return v >= this.x && v <= this.y;
	
	}
	
	public override string ToString ()
	{
		return x + "," + y;
	}
					
	public static bool operator ==(FreeCoordinate a, FreeCoordinate b)
	{
		// If both are null, or both are same instance, return true.
		if (System.Object.ReferenceEquals(a, b))
		{
			return true;
		}
		
		// If one is null, but not both, return false.
		if (((object)a == null) || ((object)b == null))
		{
			return false;
		}
		
		// Return true if the fields match:
		return a.x == b.x && a.y == b.y;
	}
	
	public static bool operator !=(FreeCoordinate a, FreeCoordinate b)
	{
		return !(a == b);
	}
																				
	public override bool Equals (object obj)
	{
		
		FreeCoordinate coordinate = obj as FreeCoordinate;
		
		if(coordinate != null){
		
			return coordinate.x == x && coordinate.y == y;
		
		}
	
		return false;
	}
	
	public override int GetHashCode ()
	{
		return x ^ y;
	}

}


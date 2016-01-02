using UnityEngine;
using System.Collections;

#pragma warning disable 0168

public static class FreeScreenUtility {

	public enum Alignment{
		Top,
		Bottom,
		Left,
		Right,
		Front,
		Back
	}

	static Vector3 UPPER_LEFT = new Vector3(0f,Screen.height,10f);
	static Vector3 UPPER_RIGHT = new Vector3(Screen.width,Screen.height,10f);
	static Vector3 LOWER_RIGHT = new Vector3(Screen.width,0f,10f);
	static Vector3 LOWER_LEFT = new Vector3(0f,0f,10f);
	static Vector3 MID_POINT = new Vector3(Screen.width*0.5f,Screen.height*0.5f,10f);
	
	public static void SetOrthoCameraForTargetWidth(Camera camera, float targetWidth){
	
		camera.orthographicSize = targetWidth * Screen.height / Screen.width * 0.5f;
		camera.orthographic = true;
		camera.nearClipPlane = -100f;
	
	}

	public static Vector3 GetUpperLeft(Camera camera){
		
		return camera.ScreenToWorldPoint(UPPER_LEFT);
		
	}
	
	public static Vector3 GetUpperLeft(Camera camera, float depth){
		
		UPPER_LEFT.z = depth;
		
		return camera.ScreenToWorldPoint(UPPER_LEFT);
		
	}
	
	public static Vector3 GetUpperRight(Camera camera){
		
		return camera.ScreenToWorldPoint(UPPER_RIGHT);
		
	}
	
	public static Vector3 GetUpperRight(Camera camera, float depth){
		
		UPPER_RIGHT.z = depth;
		
		return camera.ScreenToWorldPoint(UPPER_RIGHT);
		
	}
	
	public static Vector3 GetLowerRight(Camera camera){
		
		return camera.ScreenToWorldPoint(LOWER_RIGHT);
		
	}
	
	public static Vector3 GetLowerRight(Camera camera, float depth){
		
		LOWER_RIGHT.z = depth;
		
		return camera.ScreenToWorldPoint(LOWER_RIGHT);
		
	}
	
	public static Vector3 GetLowerLeft(Camera camera){
		
		return camera.ScreenToWorldPoint(LOWER_LEFT);
		
	}
	
	public static Vector3 GetLowerLeft(Camera camera, float depth){
		
		LOWER_LEFT.z = depth;
		
		return camera.ScreenToWorldPoint(LOWER_LEFT);
		
	}
	
	public static Vector3 GetMidpoint(Camera camera){
		
		return camera.ScreenToWorldPoint(MID_POINT);
		
	}
	
	public static Vector3 GetMidpoint(Camera camera, float depth){
		
		MID_POINT.z = depth;
		
		return camera.ScreenToWorldPoint(MID_POINT);
		
	}
	
	public static float GetDepthForTargetWidth(Camera camera, float targetWidth){
		
		float targetHeight = targetWidth / camera.aspect;
	
		float distance = targetHeight * 0.5f / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
		
		return distance;
						
	}

	public static float GetDepthForTargetHeight (Camera camera, float worldHeight)
	{
		float targetHeight = worldHeight;

		float distance = targetHeight * 0.5f / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);

		return distance;
	}
		
	public static void GetWorldScreenBounds(Camera camera, float depth, 
											out Vector3 world_UpperLeft, out Vector3 world_UpperRight, 
	                                        out Vector3 world_LowerLeft, out Vector3 world_LowerRight){
	
		Vector3 upperLeft = GetUpperLeft(camera,depth);
		Vector3 lowerRight = GetLowerRight(camera,depth);
		Vector3 midPoint = GetMidpoint(camera,depth);
		Vector3 upperRight = GetUpperRight(camera,depth);
		Vector3 lowerLeft = GetLowerLeft(camera,depth);
		
		GetWorldScreenBounds(camera,midPoint,upperLeft,upperRight,lowerLeft,lowerRight, 
		out world_UpperLeft, out world_UpperRight, out world_LowerLeft, out world_LowerRight);
	
	}
	
	public static void GetWorldScreenBounds(Camera camera, Vector3 midPoint, 
		Vector3 upperLeft, Vector3 upperRight, Vector3 lowerLeft, Vector3 lowerRight,
		out Vector3 world_UpperLeft, out Vector3 world_UpperRight, 
		out Vector3 world_LowerLeft, out Vector3 world_LowerRight){
		
		//http://wiki.unity3d.com/index.php/3d_Math_functions
				
		Vector3 ulDirection = (upperLeft-camera.transform.position).normalized;
		Vector3 lrDirection = (lowerRight-camera.transform.position).normalized;
		
		Vector3 urDirection = (upperRight-camera.transform.position).normalized;
		Vector3 llDirection = (lowerLeft-camera.transform.position).normalized;
		
		Vector3 ulCameraPosition = camera.transform.position;
		Vector3 lrCameraPosition = camera.transform.position;
		Vector3 urCameraPosition = camera.transform.position;
		Vector3 llCameraPosition = camera.transform.position;
		
		if(camera.orthographic){
			
			ulDirection = camera.transform.forward * -1f;
			lrDirection = camera.transform.forward * -1f;
			
			urDirection = camera.transform.forward * -1f;
			llDirection = camera.transform.forward * -1f;
				
			ulCameraPosition = upperLeft + (camera.transform.forward * -1f * 10f);
			lrCameraPosition = lowerRight + (camera.transform.forward * -1f * 10f);
			
			urCameraPosition = upperRight + (camera.transform.forward * -1f * 10f);
			llCameraPosition = lowerLeft + (camera.transform.forward * -1f * 10f);
		}
		
		Math3D.LinePlaneIntersection(out world_UpperLeft,ulCameraPosition,ulDirection,Vector3.up,midPoint);
		Math3D.LinePlaneIntersection(out world_LowerRight,lrCameraPosition,lrDirection,Vector3.up,midPoint);
		Math3D.LinePlaneIntersection(out world_UpperRight,urCameraPosition,urDirection,Vector3.up,midPoint);
		Math3D.LinePlaneIntersection(out world_LowerLeft,llCameraPosition,llDirection,Vector3.up,midPoint);		
	}
	
	public static Vector3 MultiplyWithVector(this Vector3 vector, Vector3 multiplier)
	{
		
		Vector3 result = Vector3.zero;
		
		result.x = vector.x * multiplier.x;
		result.y = vector.y * multiplier.y;
		result.z = vector.z * multiplier.z;
		
		return result;
	
	}
	
	public static Vector3 GetAlignedOffset(IAlignable alignable, Vector3 alignToPoint, Alignment alignment){
	
		Vector3 size = new Vector3(alignable.GetWidth(),alignable.GetHeight(),alignable.GetDepth()) * 0.5f;
	
		Vector3 alignmentDirection = Vector3.zero;
	
		switch(alignment){
			
			case Alignment.Back:{
			
				alignmentDirection = Vector3.forward*-1f;
			
				break;
			}
			case Alignment.Bottom:{
			
				alignmentDirection = Vector3.up * -1f;
			
				break;
			}
			case Alignment.Front:{
			
				alignmentDirection = Vector3.forward;
			
				break;
			}
			case Alignment.Left:{
			
				alignmentDirection = Vector3.right * -1f;
			
				break;
			}
			case Alignment.Right:{
			
				alignmentDirection = Vector3.right;
			
				break;
			}
			case Alignment.Top:{
			
				alignmentDirection = Vector3.up;
			
				break;
			}
			
		}
				
		Vector3 alignmentOffset = alignmentDirection * -1f;
				
		alignmentOffset = alignmentOffset.MultiplyWithVector(size);
		
		return alignmentOffset;
	
	}
	
}

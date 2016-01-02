// 2D IK Solver by Kanako
//Implementation is inspired by method described at http://www.ryanjuckett.com/programming/animation/16-analytic-two-bone-ik-in-2d
using UnityEngine;
using System.Collections;


		
public class IKSolver2D : MonoBehaviour
{
	public enum Plane2D {XY,XZ,YZ}
	public Plane2D LocalPlane2D = Plane2D.XY;
	
	[HideInInspector]
	public Vector3 NormalTo2DPlane;
	
	//This component is placed on the target the end effector should hit	
	public Transform EndTransform;
	
	public Transform Target;
	
	//If target is within reach there are two solutions
	public bool BendCCW;
	
	//Move target to best match so that it is always reachable
	public bool ClampTargetPosition;
	
	//We want to see what's happening while animating
	public bool UpdateInEditMode;
	
	//Shorthand references to bones in chain
	[HideInInspector]
	public Transform Lower;
	[HideInInspector]
	public Transform Upper;
	
	protected Transform upperParent; //shorthand 
	
	//The direction the bone points
	protected Vector3 upperLeg;
	protected Vector3 lowerLeg;
	
	//The length of the bones
	protected float upperlength;
	protected float lowerlength;
	
	//The rotational bias the bone is offset (Difference from X direction)
	protected float upperbias;
	protected float lowerbias;
		
	//The maximum length of the chain (max the length of the two bones)
	protected float maxlen;
	
	void Start ()
	{
		//If you change the target or similar, call this 
		GetTransforms();
	}
	
	public void GetTransforms()
	{
		if(EndTransform == null)
		{
			EndTransform = transform;
		}
		if(LocalPlane2D == Plane2D.XY)
		{
			NormalTo2DPlane = new Vector3(0,0,1);
		}
		else if(LocalPlane2D == Plane2D.XZ)
		{
			NormalTo2DPlane = new Vector3(0,1,0);
		} 
		else if(LocalPlane2D == Plane2D.YZ)
		{
			NormalTo2DPlane = new Vector3(1,0,0);
		}
		
		//You can disable smooothmoves animations on bones by renaiming them
		//chain.parent.name += "_";
		//chain.parent.parent.name += "_";
		//transform.name += "_";
		
		if(EndTransform == null)
		{
			Debug.LogWarning("2D IK Solver: EndTransform must be defined before Start");
			return;
		}
		if(EndTransform.parent == null)
		{
			Debug.LogWarning("2D IK Solver: Requires two parents in hieararchy to do IK solving.");
			return;
		}
		
		Lower = EndTransform.parent;
		Upper = Lower.parent;	
		
		if(Upper != null)	
		{
			upperParent = Upper.parent;
		}
		
		if(Upper == null)
		{
			Debug.LogWarning("2D IK Solver: Requires two parents in hieararchy to do IK solving. Otherwise just use transform.LookAt");
			return;
		}
		
		upperLeg = Lower.localPosition;
		lowerLeg = EndTransform.localPosition;
		
		upperlength = upperLeg.magnitude;
		lowerlength = lowerLeg.magnitude;
		
		maxlen = (upperlength+lowerlength) - 0.0001f;
		
		upperbias = Mathf.Atan2(upperLeg.y, upperLeg.x);
		lowerbias = Mathf.Atan2(lowerLeg.y, lowerLeg.x);
		
		if(Target == null)
		{
			Debug.LogWarning("2D IK Solver: No target defined, not moving");
			return;
		}
	}
	
	void LateUpdate () 
	{
		
		if(Upper == null || Target == null ) //Only one bone. Just use look at
		{
			//We need two segments to do IK
			return;
		}
		//Find correction in upper's parent space
		Vector3 targetWP = Target.position;
	
		Vector3 upperWP = Upper.position;
		
		Vector3 toTargetW = targetWP - upperWP;
		
		//Clamp length
		if(toTargetW.magnitude > maxlen)
		{
			toTargetW = toTargetW.normalized * maxlen;
			if(ClampTargetPosition)
			{
				Target.position = upperWP + toTargetW;
			}
		}
		Vector3 toTargetUPS = toTargetW;
		if(upperParent != null)
		{			
			toTargetUPS = upperParent.InverseTransformDirection(toTargetW);
		}
		
		float x = convertToRightPlane(toTargetUPS).x;
		float y = convertToRightPlane(toTargetUPS).y;
		
		float lowerangle = Mathf.Acos(
			(
				x*x + y*y - upperLeg.sqrMagnitude - lowerLeg.sqrMagnitude
			)/(
				2.0f*lowerlength*upperlength
			)
		);
		
		if(BendCCW) //bend the other way
		{
			lowerangle *= -1;
		}
		
		float x_ = x*(upperlength + lowerlength*Mathf.Cos (lowerangle)) + y*(lowerlength*Mathf.Sin(lowerangle));
		float y_ = y*(upperlength + lowerlength*Mathf.Cos (lowerangle)) - x*(lowerlength*Mathf.Sin(lowerangle));	
		float upperangle = Mathf.Atan2(y_, x_);
		
		if(Upper != null)
		{
			//Apply rotation (subtract bias). IF not NaN (impossible solution)
			float upperRotZ = (upperangle-upperbias)*Mathf.Rad2Deg;
			if(!float.IsNaN(upperRotZ))
			{
				Upper.localRotation = Quaternion.Euler(convertToRightPlane(new Vector3(0,0,upperRotZ)));
			}
		}
		
		float lowerRotZ = (lowerangle-lowerbias + upperbias)*Mathf.Rad2Deg;
		if(!float.IsNaN(lowerRotZ))
		{
			Lower.localRotation = Quaternion.Euler(convertToRightPlane(new Vector3(0,0,lowerRotZ)));
		}
		
	}
	
	private Vector3 convertToRightPlane(Vector3 direction)
	{
		Vector3 newDirection = direction;
		Vector3 tmp = direction;
		if(LocalPlane2D == Plane2D.XZ)
		{
			newDirection.y = tmp.z;
		}
		if(LocalPlane2D == Plane2D.YZ)
		{
			newDirection.x = tmp.y;
			newDirection.y = tmp.z;
			
		}
		return newDirection;
	}
	public Vector3 GetWorldNormalToPlane()
	{
		return transform.TransformDirection(NormalTo2DPlane);
	}
	
	public bool ContainsGameObject (GameObject go)
	{
		//Any objects begin used by this IK solver (for drawing bones)
		return ((Lower != null && Lower.gameObject == go) 
			|| (Upper != null && Upper.gameObject == go)
			|| (EndTransform != null && EndTransform.gameObject == go)
			|| (EndTransform.parent != null && EndTransform.parent.gameObject == go)
			|| (EndTransform.parent != null && EndTransform.parent.parent != null && EndTransform.parent.parent.gameObject == go)
			);
	}
	
}

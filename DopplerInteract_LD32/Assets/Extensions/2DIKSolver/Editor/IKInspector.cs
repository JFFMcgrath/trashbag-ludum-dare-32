using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(IKSolver2D))]
public class IKInspector : Editor {
	
	private GUIStyle warningStyle;
	private GUIStyle infoStyle;
	
	public override void OnInspectorGUI () {
		//inspector for this fellow
		IKSolver2D IK = (IKSolver2D)target;
		
		//Define GUI styles for labesl
		warningStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
		warningStyle.normal.textColor = new Color(1f,0.2f,0.2f,1f);
		infoStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
		infoStyle.normal.textColor = new Color(0.25f,0.25f,0.25f,1f);
		
		
		EditorGUIUtility.LookLikeInspector();
		
		//description  / guide
		EditorGUILayout.LabelField("This will rotate the parents of the EndTransform in order to move it to the Target position",infoStyle);
		EditorGUILayout.Space();
			
		//List all the public fields, but add error messages and info
		IK.EndTransform = (Transform)EditorGUILayout.ObjectField("End Transform",IK.EndTransform,typeof(Transform),true);
		Transform endPoint = IK.EndTransform;
		
		if(endPoint == null)
		{
			EditorGUILayout.LabelField("If no End Transform defined we assume this GameObject is the End Transform",warningStyle);
		}
		//Default to this transform
		if(endPoint == null)
		{
			endPoint = IK.transform;
		}
		
		if(endPoint != null && (endPoint.parent == null || endPoint.parent.parent == null))
		{
			EditorGUILayout.LabelField("Your EndTransform must have at least two parents (2 levels deep in hierarchy)",warningStyle);
		}
		EditorGUILayout.Space();
		
		IK.Target = (Transform)EditorGUILayout.ObjectField("Target",IK.Target,typeof(Transform),true);
		if(IK.Target == null)
		{
			EditorGUILayout.LabelField("No target defined, you should define this here or from a script",warningStyle);
		}
		
		EditorGUILayout.Space();
		
		IK.LocalPlane2D = (IKSolver2D.Plane2D)EditorGUILayout.EnumPopup("Local 2D Plane",IK.LocalPlane2D);
		IK.BendCCW =EditorGUILayout.Toggle("Bend CCW",IK.BendCCW);
		IK.ClampTargetPosition = EditorGUILayout.Toggle("Clamp Target Position",IK.ClampTargetPosition);
		
		EditorGUILayout.Space();
	}
}

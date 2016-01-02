using UnityEngine;
using System.Collections;
using UnityEditor;

[InitializeOnLoad] //Get unity to initialize this class
public class IKEditor {
	
	public static bool DrawIcons = false; //Might be useful at some point
	public static Color BoneColor = new Color(0f,0f,1f,0.4f);
	// Use this for initialization
	static IKEditor () {
		EditorApplication.hierarchyWindowItemOnGUI += OnDrawItem; //to draw icon in hiearchy view
		SceneView.onSceneGUIDelegate += OnDrawSceneView;
	}
	
	static void OnDrawSceneView(SceneView current)
	{
		IKSolver2D IK = null;
		if(Selection.activeGameObject != null)
		{
			IK = Selection.activeGameObject.GetComponentInChildren<IKSolver2D>();
			if(IK == null)
			{
				IK = Selection.activeGameObject.GetComponent<IKSolver2D>();
			}
		}
		if(IK != null)
		{		
			float boneLenght = 0f;
			Transform chain = IK.EndTransform==null?IK.transform:IK.EndTransform;
			if(chain.parent != null)
			{
				DrawBone (chain.parent.position, chain.position, IK.GetWorldNormalToPlane());
			}
			if(chain.parent != null && chain.parent.parent != null)
			{
				boneLenght = DrawBone (chain.parent.parent.position, chain.parent.position, IK.GetWorldNormalToPlane());
			}
			
			if(IK.Target != null)
			{
				float radius = boneLenght/10f; //target radius is 10% of bone length
				Handles.color = Color.red;
				Handles.DrawSolidDisc(IK.Target.position,IK.GetWorldNormalToPlane(),radius);
			}
		}
		
	}

	static float DrawBone (Vector3 start, Vector3 end, Vector3 plane)
	{
		Vector3 dir = (start-end);
		float distToSidesPct = 10f;
		float distFromBasePct = 80f;
		Vector3 p0 = end;
		Vector3 p1 = end+dir/100f*distFromBasePct +Vector3.Cross(plane,dir.normalized)*dir.magnitude/distToSidesPct;
		Vector3 p2 = start;
		Vector3 p3 = end+dir/100f*distFromBasePct -Vector3.Cross(plane,dir.normalized)*dir.magnitude/distToSidesPct;
		
		GL.Begin(GL.QUADS);
		GL.Color(BoneColor);
		GL.Vertex(p0);
		GL.Vertex(p1);
		GL.Vertex(p2);
		GL.Vertex(p3);
		
		GL.End();
		
		//return length of bone
		return dir.magnitude;
	}
	
	private static void OnDrawItem (int instanceID, Rect rect)
    {
		if(!DrawIcons)
		{
			return;
		}
		
		bool isIK = false;
		var instance = EditorUtility.InstanceIDToObject(instanceID);
		if(instance is GameObject)
		{
			GameObject go = (GameObject)instance;
			var ik = go.GetComponentInChildren<IKSolver2D>();
			if(ik != null && (ik.ContainsGameObject(go) || ik.gameObject == go))
			{
				isIK = true;
			}
		}

		rect.x = rect.width+rect.xMin; //all the way to the right minus width
		rect.width = 10;
		rect.height = 10; 
 		
		if (isIK)
		{			
			GUI.Box (rect, "IK", GUIStyle.none); 
		}
		
    }
}

using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateAvatar : Editor {

	[MenuItem("Assets/Create/Avatar")]
	static void CreateNewAvatar(){

		UnityEngine.GameObject o = Selection.activeGameObject;

		System.Collections.Generic.Dictionary<string, string> boneName = new System.Collections.Generic.Dictionary<string, string>();
		boneName["Chest"] = "Neck_Root";
		boneName["Head"] = "Head_Root";
		boneName["Hips"] = "Hips";
		boneName["LeftFoot"] = "Back_Foot";
		boneName["LeftHand"] = "Back_Hand";
		boneName["LeftLowerArm"] = "Back_Wrist";
		boneName["LeftLowerLeg"] = "Back_Ankle";
		boneName["LeftShoulder"] = "Back_Shoulder_Root";
		boneName["LeftUpperArm"] = "Back_Elbow";
		boneName["LeftUpperLeg"] = "Back_Knee";
		boneName["RightFoot"] = "Front_Foot";
		boneName["RightHand"] = "Front_Hand";
		boneName["RightLowerArm"] = "Front_Wrist";
		boneName["RightLowerLeg"] = "Front_Ankle";
		boneName["RightShoulder"] = "Front_Shoulder_Root";
		boneName["RightUpperArm"] = "Front_Elbow";
		boneName["RightUpperLeg"] = "Front_Knee";
		boneName["Spine"] = "Center_Spine";

		string[] humanName = HumanTrait.BoneName;
		HumanBone[] humanBones = new HumanBone[boneName.Count];
		int j = 0;
		int i = 0;
		while (i < humanName.Length) {
			if (boneName.ContainsKey(humanName[i])) {
				HumanBone humanBone = new HumanBone();
				humanBone.humanName = humanName[i];
				humanBone.boneName = boneName[humanName[i]];
				humanBone.limit.useDefaultValues = true;
				humanBones[j++] = humanBone;
			}
			i++;
		}

		HumanDescription hd = new HumanDescription ();
		hd.human = humanBones;

		Transform[] t = o.GetComponentsInChildren<Transform> ();

		SkeletonBone[] bones = new SkeletonBone[t.Length];

		for(int b = 0; b < t.Length; b++){

			bones [b] = new SkeletonBone ();
			bones [b].name = t [b].name;
			bones [b].position = t [b].localPosition;
			bones [b].rotation = t [b].localRotation;
			bones [b].scale = t [b].localScale;

		}

		hd.skeleton = bones;

		Avatar a = AvatarBuilder.BuildHumanAvatar (o, hd);

		AssetDatabase.CreateAsset(a,"Assets/"+o.name+"_human.asset");

	}

}

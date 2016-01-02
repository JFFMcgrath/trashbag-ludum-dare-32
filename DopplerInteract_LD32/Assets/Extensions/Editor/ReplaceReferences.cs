using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;

public class ReplaceReferences : MonoBehaviour {

	[MenuItem("Utilities/PSD to PNG")]
	static void ReplaceTextures(){

		string[] all = AssetDatabase.FindAssets ("t:Prefab");

		List<SpriteRenderer> sr = new List<SpriteRenderer> ();
		List<Image> img = new List<Image> ();

		for (int i = 0; i < all.Length; i++) {

			string path = AssetDatabase.GUIDToAssetPath (all [i]);

			GameObject o = AssetDatabase.LoadMainAssetAtPath (path) as GameObject;

			string p = "Attempting: " + path + " ";

			if (o != null) {

				p += o.name + ": ";

				SpriteRenderer[] ch_s = o.GetComponentsInChildren<SpriteRenderer> (true);
				Image[] ch_i = o.GetComponentsInChildren<Image> (true);

				for (int j = 0; j < ch_s.Length; j++) {

					p += ", " + ch_s [j];

					sr.Add (ch_s [j]);

				}

				for (int j = 0; j < ch_i.Length; j++) {

					p += ", " + ch_i [j];

					img.Add (ch_i [j]);

				}

			} else {

				p += "No main asset";

			}

			Debug.Log (p);

		}

		Image[] img_g = GameObject.FindObjectsOfType<Image> ();
		SpriteRenderer[] sr_g = GameObject.FindObjectsOfType<SpriteRenderer> ();

		for (int j = 0; j < img_g.Length; j++) {

			img.Add (img_g [j]);

		}

		for (int j = 0; j < sr_g.Length; j++) {

			sr.Add (sr_g [j]);

		}

		SpriteRenderer[] spriteRenderers = Resources.FindObjectsOfTypeAll<SpriteRenderer> ();
		Image[] images = Resources.FindObjectsOfTypeAll<Image> ();

		for (int j = 0; j < spriteRenderers.Length; j++) {

			sr.Add (spriteRenderers [j]);

		}

		for (int j = 0; j < images.Length; j++) {

			img.Add (images [j]);

		}

		ReplaceSpriteReferences (sr.ToArray());
		ReplaceImageReferences (img.ToArray());

		AssetDatabase.SaveAssets ();

		//ReconnectSpritePrefabs (spriteRenderers);
		//ReconnectImagePrefabs (images);

	}

	static void ReconnectSpritePrefabs (SpriteRenderer[] sr)
	{

		for (int i = 0; i < sr.Length; i++) {

			if (PrefabUtility.GetPrefabType (sr [i]) == PrefabType.Prefab) {
				continue;
			}

			GameObject root = PrefabUtility.FindPrefabRoot (sr[i].gameObject);

			if (root != null) {

				PrefabUtility.ReconnectToLastPrefab (root);

			}

		}

	}

	static void ReconnectImagePrefabs (Image[] imgs)
	{

		for (int i = 0; i < imgs.Length; i++) {

			if (PrefabUtility.GetPrefabType (imgs [i]) != PrefabType.Prefab) {
				continue;
			}

			GameObject root = PrefabUtility.FindPrefabRoot (imgs[i].gameObject);

			if (root != null) {

				PrefabUtility.ReconnectToLastPrefab (root);

			}

		}

	}

	static void ReplaceSpriteReferences(SpriteRenderer[] spriteRenderers){

		for (int i = 0; i < spriteRenderers.Length; i++) {

			GameObject parent = PrefabUtility.GetPrefabParent (spriteRenderers [i].gameObject) as GameObject;
			UnityEngine.Object prefab = PrefabUtility.GetPrefabObject (parent);
			GameObject root = PrefabUtility.FindPrefabRoot (spriteRenderers [i].gameObject);

			Sprite s = spriteRenderers [i].sprite;

			string path = AssetDatabase.GetAssetPath (s);

			string newPath = path.Replace (".PSD", ".png");
			newPath = newPath.Replace (".psd", ".png");

			Sprite newSprite = AssetDatabase.LoadAssetAtPath<Sprite> (newPath);

			if (newSprite != null) {

				spriteRenderers [i].sprite = newSprite;

			} else {

				Debug.Log ("No replacement sprite found for: " + path + " @ " + newPath);
				continue;

			}


			string processor = "Processing: " + spriteRenderers [i].name;

			EditorUtility.SetDirty (spriteRenderers [i].gameObject);

			if (parent != null) {
				EditorUtility.SetDirty (parent);
				processor += " Parent: " + parent.name;
			}

			if (prefab != null) {
				EditorUtility.SetDirty (prefab);
				processor += " Prefab: " + prefab.name;
			}

			if (root != null) {
				EditorUtility.SetDirty (root);
				processor += " Root: " + root.name;
			}

			processor += " = " + path + " to " + newPath;

			Debug.Log (processor);

		}

	}

	static void ReplaceImageReferences(Image[] images){

		for (int i = 0; i < images.Length; i++) {

			GameObject parent = PrefabUtility.GetPrefabParent (images [i].gameObject) as GameObject;
			UnityEngine.Object prefab = PrefabUtility.GetPrefabObject (parent);
			GameObject root = PrefabUtility.FindPrefabRoot (images [i].gameObject);

			Image img = images [i];

			Sprite s = img.sprite;

			string path = AssetDatabase.GetAssetPath (s);

			string newPath = path.Replace (".PSD", ".png");
			newPath = path.Replace (".psd", ".png");

			Sprite newSprite = AssetDatabase.LoadAssetAtPath<Sprite> (newPath);

			if (newSprite != null) {

				img.sprite = newSprite;

			} else {

				Debug.Log ("No replacement image found for: " + path + " @ " + newPath);
				continue;

			}

			string processor = "Processing: " + images [i].name;

			EditorUtility.SetDirty (images [i].gameObject);

			if (parent != null) {
				EditorUtility.SetDirty (parent);
				processor += " Parent: " + parent.name;
			}

			if (prefab != null) {
				EditorUtility.SetDirty (prefab);
				processor += " Prefab: " + prefab.name;
			}

			if (root != null) {
				EditorUtility.SetDirty (root);
				processor += " Root: " + root.name;
			}

			processor += " = " + path + " to " + newPath;

			Debug.Log (processor);

		}

	}

	static GameObject GetParentObject(GameObject o,UnityEngine.Object parent){

		Transform[] parentTransforms = o.GetComponentsInParent<Transform> ();

		for (int i = 0; i < parentTransforms.Length; i++) {

			if (parentTransforms[i].name.Equals (parent.name) && (PrefabUtility.GetPrefabObject(parentTransforms[i]) == parent)) {

				return parentTransforms [i].gameObject;

			}

		}

		return o;

	}
}


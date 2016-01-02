using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class PersistentObject : MonoBehaviour
{

	void OnEnable(){

		GameObject.DontDestroyOnLoad (this.gameObject);

	}

}



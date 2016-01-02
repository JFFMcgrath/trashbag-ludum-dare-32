using UnityEngine;
using System.Collections;

public class FreeSingleton<T> : EfficientBehaviour where T : MonoBehaviour{

	public bool DestroyOnLoad;

	static T _Instance;

	public static T Instance{

		get{

			if(_Instance == null){

				_Instance = GameObject.FindObjectOfType(typeof(T)) as T;

			}

			return _Instance;

		}

	}

	void OnEnable(){

		HandleEnable();

		ClearReferences();

		//if(!DestroyOnLoad){
			Object.DontDestroyOnLoad(this);
		//}

	}

	public virtual void HandleEnable(){}

	void OnDisable(){

		HandleDisable();

		ClearReferences();

	}

	public virtual void HandleDisable(){}

	public static void ClearReferences(){

		_Instance = null;

	}

}

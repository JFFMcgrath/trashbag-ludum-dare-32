using System;
using UnityEngine;

public abstract class FreeManagerBase<T> : FreeSingleton<T> where T : MonoBehaviour
{

	public void Initialize(){
	
		InitializeManager();
	
	}
	
	public abstract void InitializeManager();
	public abstract bool IsInitializationCompleted();

}



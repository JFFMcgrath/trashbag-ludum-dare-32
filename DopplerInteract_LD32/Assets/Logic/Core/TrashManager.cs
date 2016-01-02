using UnityEngine;
using System.Collections;

public class TrashManager : FreeManagerBase<TrashManager> {

	#region implemented abstract members of FreeManagerBase

	public override void InitializeManager ()
	{

	}

	public override bool IsInitializationCompleted ()
	{
		return true;
	}

	#endregion

	public void QuitGame ()
	{
		Application.Quit ();
	}

	public void Restart ()
	{
		Application.LoadLevel (0);
	}


}

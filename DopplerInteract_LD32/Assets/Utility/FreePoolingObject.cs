using UnityEngine;
using System.Collections;

public abstract class FreePoolingObject : EfficientBehaviour {

	public abstract void HandleFreeInstantiate();
	public abstract void HandleFreeDestroy ();
	public abstract void HandleFreePreReturnToPool ();
	public abstract void HandleFreeReturnToPool ();
	public abstract void HandleFreePreFetchFromPool ();
	public abstract void HandleFreeFetchFromPool ();

	public virtual bool ShouldPoolObject(){

		return true;

	}

}

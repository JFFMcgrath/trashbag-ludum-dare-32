using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class FreeAssetPool<T> where T : FreePoolingObject{

	static Transform _poolBind;

	static Transform PoolBind{
		get{

			if (_poolBind == null) {

				GameObject o = new GameObject ("Free Asset Pool");
				o.AddComponent<PersistentObject> ();

				_poolBind = o.transform;

			}

			return _poolBind;

		}
	}

	static Dictionary<string,FreePoolEntry<T>> assetPool = new Dictionary<string, FreePoolEntry<T>>(100);
	static bool poolingEnabled = true;

	public static void Preload (T prefab_Prewarm, int prewarmCount)
	{

		if (prefab_Prewarm == null) {
			Debug.LogWarning ("Cannot prewarm a null prefab.");
			return;
		}

		if (!prefab_Prewarm.ShouldPoolObject ()) {
			return;
		}

		List<T> created = new List<T> ();

		for (int i = 0; i < prewarmCount; i++) {

			created.Add(Instantiate (prefab_Prewarm));

		}

		for (int i = 0; i < prewarmCount; i++) {

			Destroy (created [i]);

		}

	}

	static int INSTANTIATE_COUNT = 0;
	static int POOL_RETRIEVAL_COUNT = 0;

	public static T Instantiate(T prefab){

		if (poolingEnabled && prefab.ShouldPoolObject()) {

			FreePoolEntry<T> poolEntry = GetPoolEntryFor (prefab);

			if (poolEntry.IsPoolEmpty ()) {

				INSTANTIATE_COUNT++;

				return InstantiatePrefab (prefab);

			} else {

				POOL_RETRIEVAL_COUNT++;

				return FetchObjectFromPoolEntry (poolEntry);

			}

		} else {

			return InstantiatePrefab (prefab);

		}

	}

	static T FetchObjectFromPoolEntry(FreePoolEntry<T> poolEntry){

		T obj = poolEntry.GetObjectFromPool ();

		ActivateObject (obj);

		return obj;

	}

	static void ActivateObject(T instantiatedObject){

		instantiatedObject.HandleFreePreFetchFromPool ();

		instantiatedObject.CachedTransform.SetParent (null);
		instantiatedObject.CachedGameObject.SetActive (true);

		instantiatedObject.HandleFreeFetchFromPool ();

	}

	static T InstantiatePrefab(T prefab){

		T obj = GameObject.Instantiate(prefab) as T;

		obj.name = prefab.name;

		obj.HandleFreeInstantiate ();

		return obj;

	}

	public static void Destroy(T instantiatedObject){

		if (poolingEnabled && instantiatedObject.ShouldPoolObject()) {

			FreePoolEntry<T> poolEntry = GetPoolEntryFor (instantiatedObject);

			DeactivateObject (instantiatedObject);

			poolEntry.AddObjectToPool (instantiatedObject);

		} else {

			DestroyObject (instantiatedObject);

		}

	}

	static void DeactivateObject(T instantiatedObject){

		instantiatedObject.HandleFreePreReturnToPool ();

		instantiatedObject.CachedGameObject.SetActive (false);

		instantiatedObject.CachedTransform.SetParent (PoolBind);

		instantiatedObject.HandleFreeReturnToPool ();

	}

	static void DestroyObject(T instantiatedObject){

		instantiatedObject.HandleFreeDestroy ();

		GameObject.Destroy (instantiatedObject.gameObject);

	}

	static FreePoolEntry<T> GetPoolEntryFor(T prefab){

		if (!assetPool.ContainsKey (prefab.name)) {

			FreePoolEntry<T> entry = new FreePoolEntry<T> (prefab);

			assetPool.Add (prefab.name, entry);

		}

		return assetPool [prefab.name];

	}

	public static void ClearPools(){

		List<T> allObjects = new List<T> (assetPool.Count * 10);

		foreach (string name in assetPool.Keys) {

			allObjects.AddRange (assetPool [name].GetAllObjects ());

		}

		assetPool.Clear ();

		for (int i = 0; i < allObjects.Count; i++) {

			GameObject.Destroy (allObjects [i]);

		}

		allObjects.Clear ();

	}

	public static void PrintPoolState(){

		StringBuilder sb = new StringBuilder ();

		sb.Append ("Asset Pool. Entries: ");
		sb.Append (assetPool.Count);
		sb.Append ('\n');

		foreach (string name in assetPool.Keys) {

			sb.Append (assetPool [name].ToString());
			sb.Append ('\n');

		}

		Debug.Log (sb.ToString ());

	}

	public static void EnablePooling(){
		poolingEnabled = true;
	}

	public static void DisablePooling(){
		poolingEnabled = false;
	}

	public static void ResetPooling(){

		ClearPools ();
		GameObject.Destroy (PoolBind.gameObject);
		_poolBind = null;

	}

	public class FreePoolEntry<U> where U : FreePoolingObject
	{

		public string root_Prefab_ID {
			get;
			private set;
		}

		public U root_Prefab {
			get;
			private set;
		}

		List<U> activeObjects;

		public FreePoolEntry(U root_Prefab){

			this.root_Prefab = root_Prefab;

			root_Prefab_ID = this.root_Prefab.name;

			this.activeObjects = new List<U>();

		}

		public List<U> GetAllObjects(){

			return activeObjects;

		}

		public U GetObjectFromPool(){

			if (IsPoolEmpty ()) {
				return default(U);
			}

			int index = activeObjects.Count - 1;

			U obj = activeObjects [index];

			activeObjects.RemoveAt (index);

			return obj;

		}

		public bool IsPoolEmpty(){

			return activeObjects.Count == 0;

		}

		public void AddObjectToPool(U obj){

			activeObjects.Add (obj);

		}

		static FreePoolEntry<U> eComp = null;

		public override bool Equals (object obj)
		{

			eComp = obj as FreePoolEntry<U>;

			if (eComp == null) {
				return false;
			}

			return eComp.GetHashCode () == GetHashCode ();

		}

		public override int GetHashCode ()
		{

			return root_Prefab_ID.GetHashCode ();

		}

		static StringBuilder sb = new StringBuilder();

		public override string ToString ()
		{

			sb.Remove (0, sb.Length);

			sb.Append (root_Prefab.name);
			sb.Append (" ID: ");
			sb.Append (root_Prefab_ID);
			sb.Append (" ");
			sb.Append (activeObjects.Count);
			sb.Append (" objects.");
			sb.Append (" Instantiate count: ");
			sb.Append(INSTANTIATE_COUNT);
			sb.Append (" Retrieval count: ");
			sb.Append(POOL_RETRIEVAL_COUNT);

			return sb.ToString ();

		}

	}

}

/*using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace DopplerInteractive.TidyTileMapper.Utilities
{
		
	public class AssetPool
	{
		static bool USE_RESOURCES = false;
		
		public static void EnableResources(){
			USE_RESOURCES = true;
		}
		
		public static void DisableResources(){
			USE_RESOURCES = false;
		}
		
		static bool POOLING_ENABLED = false;
		static bool DESTROY_IMMEDIATE = true;
		
		public static bool DestroyImmediate(){
			return DESTROY_IMMEDIATE;
		}
		
		public static void EnableDestroyImmediate(){
			DESTROY_IMMEDIATE = true;
		}
		
		public static void DisableDestroyImmediate(){
			DESTROY_IMMEDIATE = false;
		}
		
		static GameObject assetPoolObject = null;
		
		static AssetPoolEntry compEntry = new AssetPoolEntry(null);
		
		public static void EnablePooling(){
			POOLING_ENABLED = true;
		}
		
		public static void DisablePooling(){
			POOLING_ENABLED = false;
		}
		
		public static bool IsPoolingEnabled(){
			return POOLING_ENABLED;
		}
		
		static bool expireAssets = false;
		static float assetLifeTime = 0.0f;
		
		public static void EnableAssetExpiry(float timeToLife){
			expireAssets = true;
			assetLifeTime = timeToLife;
		}
		
		public static void DisableAssetExpiry(){
			expireAssets = false;
		}

		public static void Preload (GameObject prefab_Prewarm, int prewarmCount)
		{

			int currentCount = GetCurrentPooledCount (prefab_Prewarm);

			if (currentCount != 0) {

				prewarmCount = prewarmCount - currentCount;

			}

			GameObject[] prewarmed = new GameObject[prewarmCount];

			for (int i = 0; i < prewarmCount; i++) {

				GameObject p = Instantiate (prefab_Prewarm.gameObject);

				prewarmed [i] = p;

			}

			for (int i = 0; i < prewarmed.Length; i++) {

				Destroy (prewarmed [i]);

			}

			prewarmed = null;

		}

		public static int GetCurrentPooledCount(GameObject prefab){

			string key = prefab.name;

			if(!pool.ContainsKey(key)){
				return 0;
			}

			return pool [key].Count;

		}

		static string[] poolKeys;

		public static void StripAllAssetsOlderThan(float time){

			poolKeys = null;

			poolKeys = new string[pool.Keys.Count];

			pool.Keys.CopyTo(poolKeys,0);

			string key;

			for(int j = 0; j < poolKeys.Length; j++){

				key = poolKeys[j];

				List<AssetPoolEntry> entries = pool[key];

				for(int i = 0; i < entries.Count; i++){

					if(entries[i].lifeTime >= time){

						if(DESTROY_IMMEDIATE){
							GameObject.DestroyImmediate(entries[i].gameObject);
						}
						else{

							FreeObject fo = entries[i].gameObject.GetComponent<FreeObject>();

							if(fo != null){
								fo.FinalizeDestroyFreeObject();
							}

							GameObject.Destroy(entries[i].gameObject);


						}

						entries[i] = null;

						entries.RemoveAt(i);

						i--;

					}

				}

			}


		}

		public static void UpdatePool(float deltaTime){
			
			if(!expireAssets){
				return;
			}

			poolKeys = null;

			poolKeys = new string[pool.Keys.Count];

			pool.Keys.CopyTo(poolKeys,0);

			string key;

			for(int j = 0; j < poolKeys.Length; j++){

				key = poolKeys[j];

				List<AssetPoolEntry> entries = pool[key];
				
				for(int i = 0; i < entries.Count; i++){
					
					entries[i].lifeTime += deltaTime;
					
					if(entries[i].lifeTime >= assetLifeTime){
						
						if(DESTROY_IMMEDIATE){
							GameObject.DestroyImmediate(entries[i].gameObject);
						}
						else{

							FreeObject fo = entries[i].gameObject.GetComponent<FreeObject>();

							if(fo != null){
								fo.FinalizeDestroyFreeObject();
							}
							
							fo = null;

							GameObject.Destroy(entries[i].gameObject);
							
							
						}
						
						entries[i] = null;
						
						entries.RemoveAt(i);
						
						i--;
						
					}
					
				}
				
			}
			
		}
		
		public class AssetPoolEntry{
			public GameObject gameObject;
			public float lifeTime;
			
			public AssetPoolEntry(GameObject gameObject){
				this.gameObject = gameObject;
				this.lifeTime = 0.0f;
			}
			
			public override bool Equals (object obj)
			{
				AssetPoolEntry o = obj as AssetPoolEntry;
				
				if(o.gameObject == gameObject){
					return true;
				}
				
				return false;
			}
			
			public override int GetHashCode ()
			{
				return base.GetHashCode ();
			}
			
		}
		
		public static Dictionary<string,List<AssetPoolEntry>> pool = new Dictionary<string,List<AssetPoolEntry>>();
		public static bool DEBUG_MODE = false;
		
		static StringBuilder sb = new StringBuilder();
						
		public static string GetDebugString ()
		{
			
			sb.Remove(0,sb.Length);
			
			foreach(string key in pool.Keys){
			
				sb.Append(key);
				sb.Append(": ");
				sb.Append(pool[key].Count);
				sb.Append('\n');
			
			}
			
			return sb.ToString ();
			
		}

		public static GameObject Instantiate(string prefabName){
			
			string key = prefabName;
			
			if(!pool.ContainsKey(key)){
				pool.Add(key,new List<AssetPoolEntry>());
			}
			
			List<AssetPoolEntry> objectList = pool[key];
			
			if(objectList.Count > 0)
			{
			
				GameObject o = objectList[0].gameObject;
				objectList.RemoveAt(0);
				
				o.SetActive(true);
				
				if(DEBUG_MODE){
					////Debug.Log("Instantiate: " + pool[key].Count + " assets in pool for object: " + key);
				}
				
				Transform t = o.transform;
				
				t.position = new Vector3(-1000.0f,-1000.0f,-1000.0f);
				
				t.parent = null;
				
				return o;
			
			}
			else{
				
				GameObject prefab = Resources.Load(prefabName,typeof(GameObject)) as GameObject;
				
				GameObject o = GameObject.Instantiate(prefab) as GameObject;
				o.name = key;
				
				if(DEBUG_MODE){
					////Debug.Log("Instantiating " + key + " from prefab.");
				}
				
				Transform t = o.transform;
				
				t.position = new Vector3(1000.0f,1000.0f,1000.0f);
				
				t.parent = null;
				
				return o;
				
			}
			
		}
		
		public static GameObject Instantiate(GameObject prefab){
			
			if(USE_RESOURCES){
				////Debug.Log("Instantiating from prefab when Resource usage is enabled. This is at your discretion.");
			}
			
			string key = prefab.name;
			
			if(!pool.ContainsKey(key)){
				pool.Add(key,new List<AssetPoolEntry>());
			}
			
			List<AssetPoolEntry> objectList = pool[key];
			
			if(objectList.Count > 0)
			{
			
				GameObject o = objectList[0].gameObject;
				objectList.RemoveAt(0);

				o.SetActive(true);
				
				if(DEBUG_MODE){
					////Debug.Log("Instantiate: " + pool[key].Count + " assets in pool for object: " + key);
				}
								
				o.transform.parent = null;
				
				FreeObject fo = o.GetComponentInChildren<FreeObject>();
				
				if(fo != null){
					fo.ResetFreeObject();
				}
				
				return o;
			
			}
			else{
				
				GameObject o = GameObject.Instantiate(prefab) as GameObject;
				o.name = key;
				
				if(DEBUG_MODE){
					////Debug.Log("Instantiating " + key + " from prefab.");
				}
				
				o.transform.parent = null;
				
				return o;
			
			}
		}
		
		public static void Destroy(GameObject gameObject){
			
			if(!IsPoolingEnabled()){
				
				if(DESTROY_IMMEDIATE){
					GameObject.DestroyImmediate(gameObject);
				}
				else{
					GameObject.Destroy(gameObject);
				}
				
				return;
			}
			
			if(assetPoolObject == null){
				assetPoolObject = new GameObject("Asset Pool");
				assetPoolObject.transform.position = new Vector3(9000.0f,9000.0f,9000.0f);
				assetPoolObject.AddComponent<PersistentObject> ();
			}
			
			string key = gameObject.name;
			
			if(!pool.ContainsKey(key)){
				pool.Add(key,new List<AssetPoolEntry>());
			}
			
			compEntry.gameObject = gameObject;
			
			if(pool[key].Contains(compEntry)){
				////Debug.LogWarning("Asset Pool already contains object: " + gameObject.name + " - " + gameObject.GetInstanceID());
			}
			
			AssetPoolEntry entry = new AssetPoolEntry(gameObject);
			
			pool[key].Add(entry);
			
			FreeObject fo = gameObject.GetComponent<FreeObject>();
			
			if(fo != null){
				
				fo.HandleRemoveFromWorld();
				
			}
			
			fo = null;
			
			gameObject.SetActive(false);
			
			gameObject.transform.parent = assetPoolObject.transform;
			gameObject.transform.localPosition = Vector3.zero;
			
			if(DEBUG_MODE){
				////Debug.Log("Destroy:" + pool[key].Count + " assets in pool for object: " + key);
			}
		}
		
		public static void ClearPools(){
			
			if(DEBUG_MODE){
				////Debug.Log("Clearing pools.");
			}
			
			foreach(string key in pool.Keys){
				
				List<AssetPoolEntry> objectList = pool[key]; 
				
				int length = objectList.Count;
				
				for(int i = 0; i < length; i++){
					if(DESTROY_IMMEDIATE){
						GameObject.DestroyImmediate(objectList[i].gameObject);
					}
					else{
						GameObject.Destroy(objectList[i].gameObject);
					}
				}
				
				objectList.Clear();
				
			}
			
			pool.Clear();
			
		}
	}
}*/


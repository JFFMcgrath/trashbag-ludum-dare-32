using UnityEngine;
using System.Collections;

public class EfficientBehaviour : MonoBehaviour {

	Renderer _cachedRenderer;
	public Renderer CachedRenderer{
		get{
		
			if(_cachedRenderer == null){

				SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
				
				if(smr != null){
					_cachedRenderer = smr;
					OnCacheRenderer (_cachedRenderer);
				}
				else{				
				
					MeshRenderer mr = GetComponent<MeshRenderer>();
					
					if(mr != null){
					
						_cachedRenderer = mr;
						OnCacheRenderer (_cachedRenderer);
					
					}
					else{
					
						smr = GetComponentInChildren<SkinnedMeshRenderer>();
						
						if(smr != null){
							_cachedRenderer = smr;
							OnCacheRenderer (_cachedRenderer);
						}
						else{
						
							mr = GetComponentInChildren<MeshRenderer>();
							
							if(mr != null){
								
								_cachedRenderer = mr;
								OnCacheRenderer (_cachedRenderer);

							}
						
						}
					
					}
				
				}
			
			}
			
			return _cachedRenderer;
		}
	}

	protected virtual void OnCacheRenderer(Renderer r){}

	RectTransform _cachedRectTransform;
	public RectTransform CachedRectTransform{
		get{
			if(_cachedRectTransform == null){
				_cachedRectTransform = CachedGameObject.GetComponentInChildren<RectTransform>();
			}
			return _cachedRectTransform;
		}
	}


	Collider _cachedCollider;
	public Collider CachedCollider{
		get{
			if(_cachedCollider == null){
				_cachedCollider = CachedGameObject.GetComponentInChildren<Collider>();
			}
			return _cachedCollider;
		}
	}

	Collider[] _cachedColliders;
	public Collider[] CachedColliders{
		get{
			if(_cachedColliders == null){
				_cachedColliders = CachedGameObject.GetComponentsInChildren<Collider>();
			}
			return _cachedColliders;
		}
	}
	
	Rigidbody[] _cachedRigidbodies;
	public Rigidbody[] CachedRigidbodies{
		get{
			if(_cachedRigidbodies == null){
				_cachedRigidbodies = CachedGameObject.GetComponentsInChildren<Rigidbody>();
			}
			return _cachedRigidbodies;
		}
	}

	GameObject _cachedGameObject;
	public GameObject CachedGameObject{
		get{
			if(_cachedGameObject == null){
				_cachedGameObject = this.gameObject;
			}
			return _cachedGameObject;
		}
	}

	Transform _cachedTransform;	
	public Transform CachedTransform{
		get{
			return GetCachedComponent<Transform>(this.gameObject, ref _cachedTransform);
		}
	}
	
	Rigidbody _cachedRigidbody;	
	public Rigidbody CachedRigidbody{
		get{
			return GetCachedComponent<Rigidbody>(this.gameObject, ref _cachedRigidbody);
		}
	}
	
	Animation _cachedLegacyAnimation;	
	public Animation CachedLegacyAnimation{
		get{
			return GetCachedComponentInChildren<Animation>(this.gameObject, ref _cachedLegacyAnimation);
		}
	}
	
	Animator _cachedAnimator;	
	public Animator CachedAnimator{
		get{
			return GetCachedComponent<Animator>(this.gameObject, ref _cachedAnimator);
		}
	}
	
	CharacterController _cachedCharacterController;	
	public CharacterController CachedCharacterController{
		get{
			return GetCachedComponent<CharacterController>(this.gameObject, ref _cachedCharacterController);
		}
	}
	
	ParticleSystem _cachedParticleSystem;
	public ParticleSystem CachedParticleSystem{
		get{
			return GetCachedComponent<ParticleSystem>(this.gameObject, ref _cachedParticleSystem);
		}
	}

	public void SetLayer(int layer){

		Transform[] t = GetComponentsInChildren<Transform> ();

		CachedGameObject.layer = layer;

		for (int i = 0; i < t.Length; i++) {

			t [i].gameObject.layer = layer;

		}


	}
	
	public static T GetCachedComponent<T>(GameObject parent, ref T cachedObject) where T : Component {
	
		if(cachedObject == null){
		
			cachedObject = parent.GetComponent<T>();
		
		}
		
		if(cachedObject == null){
		
		}
		
		return cachedObject;
	
	}

	public static T GetCachedComponentInChildren<T>(GameObject parent, ref T cachedObject) where T : Component {

		if(cachedObject == null){

			cachedObject = parent.GetComponentInChildren<T>();

		}

		if(cachedObject == null){

			Debug.LogWarning("No component of type: " + typeof(T).ToString() + " found on " + parent.name);

		}

		return cachedObject;

	}
	
}

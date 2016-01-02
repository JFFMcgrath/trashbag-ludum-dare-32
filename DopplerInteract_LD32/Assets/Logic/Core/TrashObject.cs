using UnityEngine;
using System.Collections;

public abstract class TrashObject : EfficientBehaviour {

	public TrashObjectType default_trashObjectType;

	SpriteRenderer _cachedSpriteRenderer;	
	public SpriteRenderer CachedSpriteRenderer{
		get{
			return GetCachedComponentInChildren<SpriteRenderer>(this.gameObject, ref _cachedSpriteRenderer);
		}
	}

	Rigidbody2D _cachedRigidbody2D;	
	public Rigidbody2D CachedRigidbody2D{
		get{
			return GetCachedComponent<Rigidbody2D>(this.gameObject, ref _cachedRigidbody2D);
		}
	}

	Rigidbody2D[] _cachedRigidbodies2D;
	public Rigidbody2D[] CachedRigidbodies2D{
		get{
			if(_cachedRigidbodies2D == null){
				_cachedRigidbodies2D = CachedGameObject.GetComponentsInChildren<Rigidbody2D>();
			}
			return _cachedRigidbodies2D;
		}
	}

	Collider2D _cachedCollider2D;	
	public Collider2D CachedCollider2D{
		get{
			return GetCachedComponentInChildren<Collider2D>(this.gameObject, ref _cachedCollider2D);
		}
	}

	Collider2D[] _cachedColliders2D;
	public Collider2D[] CachedColliders2D{
		get{
			if(_cachedColliders2D == null){
				_cachedColliders2D = CachedGameObject.GetComponentsInChildren<Collider2D>();
			}
			return _cachedColliders2D;
		}
	}

	TrashObjectType _trashObjectType;
	public TrashObjectType TrashObjectType{
		get{
			return _trashObjectType;
		}
		private set{
			//Potentially some layer logic here
			_trashObjectType = value;
		}
	}

	public bool HasAnimationParameter(string stateName){

		if (CachedAnimator == null) {
			return false;
		}

		AnimatorControllerParameter[] parameters = CachedAnimator.parameters;

		for (int i = 0; i < parameters.Length; i++) {

			if (parameters [i].name == stateName) {

				return true;
			}

		}

		return false;

	}

	void Start(){

		InitializeTrashObject (default_trashObjectType);

	}

	public void InitializeTrashObject(TrashObjectType trashObjectType){

		this.TrashObjectType = trashObjectType;

		OnInitializeTrashObject ();

	}

	protected abstract void OnInitializeTrashObject();

}

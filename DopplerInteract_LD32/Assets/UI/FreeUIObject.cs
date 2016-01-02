using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class FreeUIObject : FreeUIObjectBase {

	public bool hideByDefault;

	bool initialized = false;

	public delegate void UIObjectEvent ();

	protected UIObjectEvent _onShowCompleted;
	protected UIObjectEvent _onHideCompleted;

	void Start(){

		HandleInitialize ();

	}

	public void HandleInitialize (){

		if (initialized) {
			return;
		}

		initialized = true;

		Initialize ();

	}

	public void SubscribeToVisibilityEvents(UIObjectEvent showCompleted, UIObjectEvent hideCompleted){
		this._onShowCompleted = showCompleted;
		this._onHideCompleted = hideCompleted;
	}

	RectTransform _currentModule = null;

	public RectTransform BindModulePrefab(RectTransform prefab){

		RectTransform r = GameObject.Instantiate (prefab) as RectTransform;

		BindModule (r);

		return r;

	}

	public void BindModule(RectTransform r){

		_currentModule = r;

		r.SetParent (CachedRectTransform, false);
		r.SetSiblingIndex (0);

		r.anchoredPosition = Vector2.zero;

	}

	public void DestroyModule(){

		RectTransform r = UnBindModule ();

		if (r != null) {

			GameObject.Destroy (r.gameObject);

		}

	}

	public RectTransform UnBindModule(){

		if(_currentModule != null){

			_currentModule.SetParent (null);
			RectTransform r = _currentModule;
			_currentModule = null;
			return r;

		}

		return null;

	}

	protected void Initialize(){

		InitializeUIObject();
		OnInitialize();

	}

	protected abstract void InitializeUIObject();

	public virtual void OnInitialize(){
	}

	public void Hide(bool instant){

		HandleInitialize ();

		if (IsUIObjectVisible ()) {

			SetIsInteractive (false);

			HandleHide (instant);

			OnHide (instant);

		}

	}

	public abstract void HandleHide(bool instant);

	public virtual void OnHide(bool instant){
	}

	protected void HideComplete(string anim){

		if (_onHideCompleted != null) {
			_onHideCompleted ();
		}

		CachedGameObject.SetActive (false);

		OnHideComplete ();

	}

	protected virtual void OnHideComplete(){
	}

	public void Show(bool instant){

		HandleInitialize ();

		OnShow (instant);

		SetIsInteractive (false);

		HandleShow (instant);

	}

	public abstract void HandleShow (bool instant);

	protected virtual void OnShow(bool instant){
	}

	protected void ShowComplete(string anim){

		if (_onShowCompleted != null) {
			_onShowCompleted ();
		}

		SetIsInteractive (true);

		OnShowComplete ();

	}

	protected virtual void OnShowComplete(){
	}

	public float GetShowAmount(){

		float showAmount = GetCurrentShowAmount ();

		return showAmount;

	}

	public abstract float GetCurrentShowAmount();

}

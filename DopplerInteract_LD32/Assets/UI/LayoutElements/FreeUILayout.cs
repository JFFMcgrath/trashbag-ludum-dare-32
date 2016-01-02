using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public abstract class FreeUILayout : EfficientBehaviour {

	public bool alwaysShowInstantly;
	public bool alwaysHideInstantly;

	protected LayoutActionCompleted _layoutShowCompleted;
	protected LayoutActionCompleted _layoutHideCompleted;

	public delegate void LayoutActionCompleted();

	protected WaitForSeconds eof = new WaitForSeconds(0);
	LayoutActionCompleted _layoutHidden;

	FreeUIObject[] _childObjects;

	bool hasInitialized = false;

	protected FreeUIObject[] ChildObjects{
		get{

			if (_childObjects == null) {

				_childObjects = CachedGameObject.GetComponentsInChildren<FreeUIObject> ();

			}

			return _childObjects;

		}
	}

	public FreeUIObject[] ShowByDefaultObjects{

		get{

			List<FreeUIObject> childObjects = new List<FreeUIObject> ();
			childObjects.AddRange (ChildObjects);

			for (int i = 0; i < childObjects.Count; i++) {

				if (childObjects [i].hideByDefault) {
					childObjects.RemoveAt (i);
					i--;
				}

			}

			return childObjects.ToArray ();

		}

	}

	public FreeUIObject[] HideByDefaultObjects{

		get{

			List<FreeUIObject> childObjects = new List<FreeUIObject> ();
			childObjects.AddRange (ChildObjects);

			for (int i = 0; i < childObjects.Count; i++) {

				if (!childObjects [i].hideByDefault) {
					childObjects.RemoveAt (i);
					i--;
				}

			}

			return childObjects.ToArray ();

		}

	}

	void Awake(){

		Initialize ();

	}

	public void Initialize(){

		if (hasInitialized) {
			return;
		}

		hasInitialized = true;

		for (int i = 0; i < ChildObjects.Length; i++) {
			ChildObjects [i].HandleInitialize ();
		}

		OnInitialize ();

		HideLayout (true, null);

	}

	public virtual void OnInitialize(){
	}


	public void RefreshLayout(){

		OnRefreshLayout ();

	}

	public abstract void OnRefreshLayout();

	public bool IsLayoutVisible ()
	{
		return CachedGameObject.activeSelf;
	}

	public bool IsTransitioning ()
	{
		return waitForShowFunction != null || waitForHideFunction != null;
	}

	public void ShowLayout(bool instantly, LayoutActionCompleted onCompleted){

		Initialize ();

		if (alwaysShowInstantly) {
			instantly = true;
		}

		CachedGameObject.SetActive (true);

		_layoutShowCompleted = onCompleted;
		_layoutHideCompleted = null;

		OnShowLayout (instantly, onCompleted);

		RefreshLayout ();

	}

	protected void HandleDefaultVisibility(bool instantly){

		HideChildren (HideByDefaultObjects, true, _layoutHideCompleted);
		ShowChildren (ShowByDefaultObjects, instantly, _layoutShowCompleted);

	}

	public abstract void OnShowLayout (bool instantly, LayoutActionCompleted onCompleted);

	public void ShowChildren(FreeUIObject[] children, bool instantly, LayoutActionCompleted onCompleted){

		if (waitForShowFunction != null) {
			StopCoroutine (waitForShowFunction);
			waitForShowFunction = null;
		}

		if (instantly) {

			for (int i = 0; i < children.Length; i++) {

				children [i].Show (true);

			}

			if (onCompleted != null) {
				onCompleted ();
			}

		} else {

			waitForShowFunction = WaitOnChildren (children, false, onCompleted);

			StartCoroutine (waitForShowFunction);

		}
	}

	public void HideLayout(bool instantly, LayoutActionCompleted onCompleted){

		if (alwaysHideInstantly) {
			instantly = true;
		}

		_layoutHidden = onCompleted;

		_layoutShowCompleted = null;
		_layoutHideCompleted = onCompleted;

		OnHideLayout (instantly,LayoutHidden);

	}

	void LayoutHidden ()
	{

		if (_layoutHidden != null) {
			_layoutHidden ();
		}

		_layoutHidden = null;

		CachedGameObject.SetActive (false);

	}

	public abstract void OnHideLayout (bool instantly, LayoutActionCompleted onCompleted);

	IEnumerator waitForShowFunction = null;
	IEnumerator waitForHideFunction = null;

	public void HideChildren(FreeUIObject[] children, bool instantly, LayoutActionCompleted onCompleted){

		if (waitForHideFunction != null) {
			StopCoroutine (waitForHideFunction);
			waitForHideFunction = null;
		}

		if (instantly) {

			for (int i = 0; i < children.Length; i++) {

				children [i].Hide (true);

			}

			if (onCompleted != null) {
				onCompleted ();
			}

		} else {

			waitForHideFunction = WaitOnChildren (children, true, onCompleted);

			StartCoroutine (waitForHideFunction);

		}

	}

	IEnumerator WaitOnChildren(FreeUIObject[] children, bool waitOnHide, LayoutActionCompleted onCompleted){

		bool waiting = true;

		for (int i = 0; i < children.Length; i++) {

			if (!waitOnHide) {
				children [i].Show (false);
			} else {
				children [i].Hide (false);
			}

		}

		while (waiting) {

			waiting = false;

			for (int i = 0; i < children.Length; i++) {

				if (!waitOnHide) {
					if (!children [i].IsInteractive ()) {

						waiting = true;

					}
				}
				else {

					if(children[i].IsUIObjectVisible()){
						waiting = true;
					}

				}

			}

			yield return eof;

		}

		if (onCompleted != null) {

			onCompleted ();

		}

		if (waitOnHide) {

			waitForHideFunction	= null;	

		} else {

			waitForShowFunction = null;

		}

	}

}

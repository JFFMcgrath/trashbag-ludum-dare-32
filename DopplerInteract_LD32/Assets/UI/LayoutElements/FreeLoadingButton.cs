using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FreeLoadingButton : FreeLerpingUIObject {

	public Button button_Load;
	public Image image_None;
	public Image image_Loading;
	public Image image_Successful;
	public Image image_Failed;

	public string anim_Loading;
	IEnumerator function_Loading;

	public delegate bool IsLoadingCompleted (out bool wasSuccessful);
	public delegate bool CanBeginLoading();
	public delegate void BeginLoading ();

	IsLoadingCompleted _isLoadCompleted;
	BeginLoading _beginLoading;
	CanBeginLoading _canBeginLoading;

	public override void OnInitialize ()
	{

		button_Load.onClick.AddListener (HandleLoadClicked);

		base.OnInitialize ();

	}

	protected override void OnShow (bool instant)
	{

		base.OnShow (instant);

		Refresh ();

	}

	void HandleLoadClicked ()
	{
		StartLoading ();
	}

	public void AddLoadingFunctions(IsLoadingCompleted isLoadCompleted, 
		BeginLoading beginLoading, CanBeginLoading canBeginLoading){

		this._isLoadCompleted = isLoadCompleted;
		this._beginLoading = beginLoading;
		this._canBeginLoading = canBeginLoading;
	}

	public void Refresh(){

		if (_canBeginLoading != null) {

			SetIsInteractive (_canBeginLoading ());

		}

	}

	public void StartLoading(){

		if (function_Loading != null) {
			Debug.LogWarning ("Already loading: " + name + " - aborting request");
			return;
		}

		button_Load.interactable = false;

		StartLoadingAnimation (anim_Loading);

		function_Loading = HandleLoad ();

		image_Loading.enabled = true;
		image_Successful.enabled = false;
		image_Failed.enabled = false;
		image_None.enabled = false;

		StartCoroutine (function_Loading);

	}

	void StartLoadingAnimation(string anim_Loading){

		if (CachedLegacyAnimation != null) {
			CachedLegacyAnimation.Play (anim_Loading);
		} else {
			Debug.LogWarning ("Loading button: " + name + " has no attached animation.");
		}

	}

	public void StopLoading(){

		button_Load.interactable = false;

		StopLoadingAnimation (anim_Loading);

		image_Loading.enabled = false;
		image_Successful.enabled = false;
		image_Failed.enabled = false;
		image_None.enabled = true;

		if (_canBeginLoading != null) {

			SetIsInteractive (_canBeginLoading ());

		}

		if (function_Loading != null) {
			StopCoroutine (function_Loading);
		}

	}

	public void StopLoading(bool successful){

		button_Load.interactable = false;

		StopLoadingAnimation (anim_Loading);

		image_Loading.enabled = false;
		image_Successful.enabled = successful;
		image_Failed.enabled = !successful;
		image_None.enabled = false;

		if (_canBeginLoading != null) {

			SetIsInteractive (_canBeginLoading ());

		}

		if (function_Loading != null) {
			StopCoroutine (function_Loading);
		}

	}

	void StopLoadingAnimation(string anim_Loading){

		if (CachedLegacyAnimation != null) {
			CachedLegacyAnimation.Stop (anim_Loading);
		} else {
			Debug.LogWarning ("Loading button: " + name + " has no attached animation.");
		}

	}

	WaitForSeconds waitForSeconds = new WaitForSeconds(0);

	IEnumerator HandleLoad(){

		if (_beginLoading != null) {
			_beginLoading ();
		}

		bool loading = true;
		bool successful = false;

		while (loading) {

			loading = false;

			if (_isLoadCompleted != null) {
				loading = !_isLoadCompleted (out successful);
			} else {
				StopLoading ();
				break;
			}

			if (!loading) {
				StopLoading (successful);
				break;
			}

			yield return waitForSeconds;

		}

		function_Loading = null;

	}

}

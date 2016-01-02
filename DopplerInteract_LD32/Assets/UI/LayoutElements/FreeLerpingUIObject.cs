using UnityEngine;
using System.Collections;

public class FreeLerpingUIObject : FreeUIObject {

	public const float STANDARD_ANIMATION_TIME = 0.5f;

	protected Vector2 rootPosition;

	public enum AnimationStyle
	{
		Return_To_Start,
		Animate_Through
	}

	public AnimationStyle animationStyle;
	public bool useUIEffectCurve;

	protected Vector2 hiddenPosition{
		get{

			if (animationStyle == AnimationStyle.Return_To_Start) {
				return enterPosition;
			}

			if (exitThrough) {

				return exitPosition;

			} else {

				return enterPosition;

			}
		}
	}

	bool exitThrough = false;

	protected Vector2 enterPosition;
	protected Vector2 exitPosition;

	protected Vector2 rootScale;
	protected Vector2 hiddenScale;
	protected float currentAnimationTime = 0f;
	protected float currentAnimation_N = 0f;
	protected bool isShowing = false;
	protected bool lerpInProgress = false;

	public float lerp_animationDuration;
	public float AnimationDuration{
		get{

			if (lerp_animationDuration == 0f) {
				return STANDARD_ANIMATION_TIME;
			} else {
				return lerp_animationDuration;
			}

		}
	}

	#region implemented abstract members of FreeUIObject

	protected override void InitializeUIObject ()
	{
		InitializeAutomaticAnimation ();
	}

	protected override void OnHideComplete ()
	{
		currentAnimation_N = 0f;
	}

	protected override void OnShowComplete ()
	{
		currentAnimation_N = 1f;
	}

	IEnumerator WaitAndHide(){
		yield return new WaitForSeconds (1f);
		Hide (false);
	}

	public override void HandleHide (bool instant)
	{

		if (instant) {

			if (_lerpFunction != null) {
				StopCoroutine (_lerpFunction);
			}

			CachedGameObject.SetActive (false);
			SetAnimationTime (0f);

			HideComplete (null);

		} else {

			if (lerpInProgress && !isShowing) {
				return;
			}

			if (_lerpFunction != null) {
				StopCoroutine (_lerpFunction);
			}

			float time = 1f - GetShowAmount ();

			if (time == 1f) {

				HideComplete (null);

			} else {

				if (GetShowAmount() == 1f) {
					exitThrough = true;
				} else {
					exitThrough = false;
				}

				BeginHide (time);

			}

		}
	}

	public override void HandleShow (bool instant)
	{

		if (instant) {

			if (_lerpFunction != null) {
				StopCoroutine (_lerpFunction);
			}

			CachedGameObject.SetActive (true);

			SetAnimationTime (1f);

			ShowComplete (null);


		} else {

			if (lerpInProgress && isShowing) {
				return;
			}

			if (_lerpFunction != null) {
				StopCoroutine (_lerpFunction);
			}

			float n = GetShowAmount ();

			if (n == 1f) {

				ShowComplete (null);

			} else {

				if (GetShowAmount() == 0f) {
					exitThrough = false;
				} else {
					exitThrough = true;
				}

				CachedGameObject.SetActive (true);

				BeginShow (n);

			}

		}
	}

	IEnumerator _lerpFunction = null;

	void BeginHide(float time){

		isShowing = false;

		_lerpFunction = HandleLerp (time, false);

		StartCoroutine (_lerpFunction);

	}

	void BeginShow(float time){

		isShowing = true;

		_lerpFunction = HandleLerp (time, true);

		StartCoroutine (_lerpFunction);

	}

	WaitForSeconds eof = new WaitForSeconds(0);

	IEnumerator HandleLerp(float time, bool show){

		lerpInProgress = true;

		if (show) {
			isShowing = true;
		} else {
			isShowing = false;
		}

		currentAnimation_N = time;

		currentAnimationTime = currentAnimation_N * AnimationDuration;

		float t = Time.time;

		while (currentAnimation_N <= 1f) {

			currentAnimationTime += Time.unscaledDeltaTime;

			currentAnimation_N = currentAnimationTime / AnimationDuration;

			if (isShowing) {
				SetAnimationTime (currentAnimation_N);
			} else {
				SetAnimationTime (1f-currentAnimation_N);
			}

			yield return eof;

		}

		yield return eof;

		if (show) {
			ShowComplete (null);
		} else {
			HideComplete (null);
		}

		lerpInProgress = false;

		_lerpFunction = null;
	}

	public override float GetCurrentShowAmount ()
	{
		if (lerpInProgress) {
			if (isShowing) {

				return currentAnimation_N;

			} else {

				return 1f - currentAnimation_N;

			}
		}

		if (this.IsUIObjectVisible ()) {
			return currentAnimation_N;
		} else {
			return 0f;
		}

	}

	#endregion

	IEnumerator function_HideAfterSeconds = null;
	WaitForSeconds waitForHide;
	float hideStartTime = 0f;

	public float GetWaitToHideStartTime(){
		return hideStartTime;
	}

	public void HideAfterSeconds(float seconds){

		if (function_HideAfterSeconds != null) {

			StopCoroutine (function_HideAfterSeconds);
			function_HideAfterSeconds = null;

		}

		hideStartTime = Time.time;

		waitForHide = new WaitForSeconds (seconds);

		function_HideAfterSeconds = HandleHideAfterSeconds ();

		StartCoroutine (function_HideAfterSeconds);

	}

	IEnumerator HandleHideAfterSeconds(){

		yield return waitForHide;

		function_HideAfterSeconds = null;

		hideStartTime = 0f;

		Hide (false);

	}

	void SetAnimationTime(float t){

		t = Mathf.Clamp (t, 0f, 1f);

		if (ShouldScaleTween ()) {

			if (useUIEffectCurve) {
				t = FreeUIEffectManager.Instance.GetUIScaleLerp (t);
			}

			CachedRectTransform.localScale = Vector2.Lerp (hiddenScale, rootScale, t);

		} else {

			if (useUIEffectCurve) {
				t = FreeUIEffectManager.Instance.GetUIPositionLerp (t);
			}

			CachedRectTransform.anchoredPosition = Vector2.Lerp (hiddenPosition, rootPosition, t);

		}

	}

	public void InitializeAutomaticAnimation(){

		if (ShouldScaleTween ()) {

			rootScale = CachedRectTransform.localScale;
			hiddenScale = Vector2.zero;

		} else {

			rootPosition = CachedRectTransform.anchoredPosition;

			Vector2 pos = CalculateHiddenPosition();

			enterPosition = pos;

			if (animationStyle == AnimationStyle.Animate_Through) {
				exitPosition = CalculateHiddenPosition (true);
			} else {
				enterPosition = pos;
			}

		}

	}

	public Vector2 CalculateHiddenPosition(){
		return CalculateHiddenPosition (false);
	}

	public Vector2 CalculateHiddenPosition(bool invert){

		Vector2 offset = Vector2.zero;

		Vector2 pivot = CachedRectTransform.pivot;

		pivot -= new Vector2 (0.5f, 0.5f);

		if (Mathf.Abs (pivot.x) > Mathf.Abs (pivot.y)) {

			bool less_x = pivot.x < 0f;

			if (invert) {
				less_x = !less_x;
			}

			//Horizontal
			if (less_x) {
				//left
				return new Vector2 (0f - CachedRectTransform.rect.width, rootPosition.y);
			} else {
				//right
				return new Vector2 (CachedRectTransform.rect.width, rootPosition.y);
			}

		} else {

			bool less_y = pivot.y < 0f;

			if (invert) {
				less_y = !less_y;
			}

			//Vertical
			if (less_y) {
				//top
				return new Vector2 (rootPosition.x, 0f - CachedRectTransform.rect.height);
			} else {
				//bottom
				return new Vector2 (rootPosition.x, CachedRectTransform.rect.height);
			}

		}

	}

	public bool ShouldScaleTween(){

		return CachedRectTransform.pivot == new Vector2 (0.5f, 0.5f);

	}

	IEnumerator DanceTest(){

		while(true){

			if (ShouldScaleTween ()) {

				CachedRectTransform.localScale = rootScale;

				yield return new WaitForSeconds (1f);

				CachedRectTransform.localScale = hiddenScale;

				yield return new WaitForSeconds (1f);


			} else {

				CachedRectTransform.anchoredPosition = rootPosition;

				yield return new WaitForSeconds (1f);

				CachedRectTransform.anchoredPosition = hiddenPosition;

				yield return new WaitForSeconds (1f);

			}

		}

	}


}

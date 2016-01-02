using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwishEffect : EfficientBehaviour {

	LineRenderer _cachedLineRenderer;	
	public LineRenderer CachedLineRenderer{
		get{
			return GetCachedComponent<LineRenderer>(this.gameObject, ref _cachedLineRenderer);
		}
	}

	public float minimumPointDistance;
	public float maximumLineDistance;
	public float fadeOutTime;

	public float TargetLineDistance{
		get{
			if (customValues) {
				return maximumLineDistance;
			} else {
				return customLineLength;
			}
		}
	}

	public Color defaultStartColor;
	public Color defaultEndColor;

	bool customValues = false;
	float customLineLength;
	Color customStartColor;
	Color customEndColor;

	List<Vector3> linePoints = new List<Vector3>();

	IEnumerator _fadeOutFunction;
	IEnumerator _drawFunction;
	WaitForSeconds eof;
	WaitForSeconds pointAddWait;

	public void BeginSwishEffect(float lineLength, Color startColor, Color endColor){

		CachedLineRenderer.sortingLayerName = "Particles";

		SetCustomValues (lineLength, startColor, endColor);

		BeginDrawingSwishEffect ();

	}

	public void SetColor(Color start, Color end){

		CachedLineRenderer.SetColors (start, end);

	}

	public void BeginSwishEffect(){

		//RevertToDefaultValues ();
		CachedLineRenderer.sortingLayerName = "Particles";

		SetCustomValues (customLineLength, customStartColor, customEndColor);

		BeginDrawingSwishEffect ();

	}

	void RevertToDefaultValues(){

		customValues = false;

		CachedLineRenderer.SetColors (defaultStartColor, defaultEndColor);

	}

	void SetCustomValues(float lineLength, Color startColor, Color endColor){

		customValues = true;

		customLineLength = lineLength;
		customStartColor = startColor;
		customEndColor = endColor;

		//CachedLineRenderer.SetColors (customStartColor, customEndColor);

	}

	void BeginDrawingSwishEffect(){

		CachedLineRenderer.enabled = true;

		_drawFunction = DrawSwishEffect ();

		StartCoroutine (_drawFunction);

	}

	public void EndSwishEffect(){

		_fadeOutFunction = FadeOutSwishEffect ();

		StopAllCoroutines ();

		CachedTransform.SetParent (null);

		if (_drawFunction != null) {
			StopCoroutine (_drawFunction);
		}

		if (CachedGameObject.activeSelf) {
			StartCoroutine (_fadeOutFunction);
		}

	}

	IEnumerator DrawSwishEffect(){

		AddPoint (CachedTransform.position);
		AddPoint (CachedTransform.position);

		while (true) {

			yield return new WaitForEndOfFrame ();

			if (GetCurrentSegmentLength () >= minimumPointDistance) {
				AddPoint (CachedTransform.position);
			} else {
				CachedLineRenderer.SetPosition (linePoints.Count-1, CachedTransform.position);
			}

		}

	}

	void RefreshLinePoints ()
	{

		CachedLineRenderer.SetVertexCount (linePoints.Count);

		for (int i = 0; i < linePoints.Count; i++) {

			CachedLineRenderer.SetPosition (i, linePoints [i]);

		}

	}

	void AddPoint(Vector3 point){

		linePoints.Add (point);

		while (GetLineLength() > TargetLineDistance) {

			linePoints.RemoveAt (0);

		}

		RefreshLinePoints ();

	}

	float GetCurrentSegmentLength(){

		if (linePoints.Count == 0) {
			return 0f;
		}

		Vector3 position = CachedTransform.position;

		float distance = Vector3.Distance (position, linePoints [linePoints.Count - 1]);

		return distance;

	}

	float GetLineLength(){

		if (linePoints.Count == 0) {
			return 0f;
		}

		return Vector3.Distance (linePoints [0], linePoints [linePoints.Count - 1]);

	}

	void RemoveAllPoints ()
	{

		CachedLineRenderer.SetVertexCount (0);

	}

	bool isFading = false;

	IEnumerator FadeOutSwishEffect(){

		if (isFading) {
			yield break;
		}

		isFading = true;

		if (eof == null) {
			eof = new WaitForSeconds (0);
		}

		float currentTime = 0f;
		float n = 0f;

		while (currentTime <= fadeOutTime) {

			currentTime += Time.deltaTime;
			n = currentTime / fadeOutTime;

			SetTransparency (1f - n);

			yield return eof;

		}

		StopFadeOut ();

	}

	void StopFadeOut(){

		if (_fadeOutFunction != null) {

			StopCoroutine (_fadeOutFunction);

		}

		isFading = false;

		DisableSwishEffect ();

	}

	void DisableSwishEffect(){

		linePoints.Clear ();

		SetTransparency (1f);

		RemoveAllPoints ();

		CachedLineRenderer.enabled = false;

		GameObject.Destroy (this.gameObject);

	}

	void SetTransparency (float n)
	{

		Color c = CachedLineRenderer.material.color;
		c.a = n;
		CachedLineRenderer.material.color = c;

	}


}

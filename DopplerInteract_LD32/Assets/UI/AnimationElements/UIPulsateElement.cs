using UnityEngine;
using System.Collections;

public class UIPulsateElement : MonoBehaviour {

	IEnumerator pulsateFunction = null;
	public const int FOREVER = -1;
	Vector2 originalSizeDelta;
	bool initialized = false;
	bool continuePulsating = true;

	RectTransform _cachedRectTransform;
	public RectTransform CachedRectTransform{
		get{

			if(_cachedRectTransform == null){

				_cachedRectTransform = GetComponent<RectTransform> ();

			}

			return _cachedRectTransform;
		}
	}

	public void PulsateElement(float targetScaleFactor, float timeToScale, int timesToRepeat){

		if (!initialized) {
			originalSizeDelta = CachedRectTransform.sizeDelta;
			initialized = true;
		}

		if (pulsateFunction != null) {
			StopPulsating (true);
		}

		pulsateFunction = HandlePulsate(targetScaleFactor, timeToScale, timesToRepeat);

		StartCoroutine (pulsateFunction);

	}

	public void StopPulsating(bool immediately){

		if (pulsateFunction == null) {
			return;
		}

		if (immediately) {
			StopCoroutine (pulsateFunction);
			CachedRectTransform.sizeDelta = originalSizeDelta;
			pulsateFunction = null;
		} else {
			continuePulsating = false;
		}

	}

	IEnumerator HandlePulsate(float targetScaleFactor, float timeToScale, int timesToRepeat){

		WaitForSeconds waitForSeconds = new WaitForSeconds (0);

		int i = 0;
		continuePulsating = true;

		Vector2 targetSizeDelta = originalSizeDelta * targetScaleFactor;

		while (continuePulsating) {

			float currentTime = 0f;
			float n = 0f;

			while (currentTime <= timeToScale) {

				currentTime += Time.deltaTime;
				n = currentTime / timeToScale;

				n = FreeUIEffectManager.Instance.anim_pulsateCurve.HalfWrapValue (n);
				n = FreeUIEffectManager.Instance.anim_pulsateCurve.GetValueAt (n);

				CachedRectTransform.sizeDelta = Vector2.Lerp (originalSizeDelta, targetSizeDelta, n);

				yield return waitForSeconds;

			}

			i++;

			if (timesToRepeat != FOREVER && i >= timesToRepeat) {

				continuePulsating = false;

			}

		}

		StopPulsating (true);

	}

}

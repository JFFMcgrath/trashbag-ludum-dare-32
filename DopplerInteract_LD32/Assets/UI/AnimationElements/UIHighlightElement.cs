using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIHighlightElement : MonoBehaviour {

	IEnumerator highlightFunction = null;
	public const int FOREVER = -1;
	Color originalColor;
	bool initialized = false;
	bool continueHighlighting = true;

	Graphic _cachedGraphic;
	public Graphic CachedGraphic{
		get{

			if(_cachedGraphic == null){

				_cachedGraphic = GetComponent<Graphic> ();

			}

			return _cachedGraphic;
		}
	}

	public void HighlightElement(Color targetColor, float timeToHighlight, float timeUnhighlighted, int timesToRepeat){

		if (!initialized) {
			originalColor = CachedGraphic.color;
			initialized = true;
		}

		if (highlightFunction != null) {
			StopHighlighting (true);
		}

		highlightFunction = HandleHighlight(targetColor, timeToHighlight, timeUnhighlighted, timesToRepeat);

		StartCoroutine (highlightFunction);

	}

	public void StopHighlighting(bool immediately){

		if (highlightFunction == null) {
			return;
		}

		if (immediately) {
			StopCoroutine (highlightFunction);
			CachedGraphic.color = originalColor;
			highlightFunction = null;
		} else {
			continueHighlighting = false;
		}

	}

	IEnumerator HandleHighlight(Color targetColor, float timeToHighlight, float timeUnhighlighted, int timesToRepeat){

		WaitForSeconds highlightTime = new WaitForSeconds (timeToHighlight);
		WaitForSeconds unhighlightTime = new WaitForSeconds (timeUnhighlighted);

		int i = 0;
		continueHighlighting = true;

		while (continueHighlighting) {

			Color c = originalColor;

			if (i % 2 == 0) {
				c = targetColor;
			}

			CachedGraphic.color = c;

			if (i % 2 == 0) {
				yield return highlightTime;
			} else {
				yield return unhighlightTime;
			}

			if (timesToRepeat != FOREVER && i > timesToRepeat) {

				continueHighlighting = false;

			}

			i++;

		}

		StopHighlighting (true);

	}

}

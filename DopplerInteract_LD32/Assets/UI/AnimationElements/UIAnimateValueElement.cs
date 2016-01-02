using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIAnimateValueElement : MonoBehaviour {

	Text  _cachedText;
	public Text CachedText{
		get{

			if(_cachedText == null){

				_cachedText = GetComponent<Text> ();

			}

			return _cachedText;
		}
	}

	float fromValue;
	float toValue;
	int decimalPlaces;
	IEnumerator animationFunction;

	public const float MAXIMUM_ANIMATION_TIME = 3f;

	public void AnimateValue(int fromValue, int toValue, int increment, float timePerIncrement, float delay){

		if (animationFunction != null) {

			Debug.Log ("Replacing existing animation");

			FinalizeAnimationImmediately ();

		}

		this.fromValue = (float)fromValue;
		this.toValue = (float)toValue;
		this.decimalPlaces = 0;

		animationFunction = HandleValueAnimation (this.fromValue, this.toValue, increment, timePerIncrement,delay);
		StartCoroutine (animationFunction);

	}

	public void AnimateValue(float fromValue, float toValue, float increment, float timePerIncrement, int decimalPlaces, float delay){

		if (animationFunction != null) {

			Debug.Log ("Replacing existing animation");

			FinalizeAnimationImmediately ();

		}

		this.fromValue = fromValue;
		this.toValue = toValue;
		this.decimalPlaces = decimalPlaces;

		animationFunction = HandleValueAnimation (this.fromValue, this.toValue, increment, timePerIncrement,delay);
		StartCoroutine (animationFunction);

	}

	public void FinalizeAnimationImmediately(){

		if (animationFunction != null) {
			StopCoroutine (animationFunction);
		}

		SetTextTo (this.toValue);
		animationFunction = null;

	}

	IEnumerator HandleValueAnimation(float fromValue, float toValue, float increment, float timePerIncrement, float delay){

		SetTextTo (fromValue);

		if (delay != 0f) {
			yield return new WaitForSeconds (delay);
		}

		if (toValue < fromValue && increment < 0f) {

			increment *= -1f;

		}

		float range = Mathf.Abs(toValue - fromValue);

		int steps = (int)(range / increment);

		float totalTime = steps * timePerIncrement;

		if (totalTime > MAXIMUM_ANIMATION_TIME) {

			float multiplier = totalTime / MAXIMUM_ANIMATION_TIME;
			increment *= multiplier;

			steps = (int)(range / increment);

		}

		WaitForSeconds waitForSeconds = new WaitForSeconds (timePerIncrement);

		float d_time = Time.time;

		for (int i = 0; i < steps; i++) {

			float value = fromValue + (i * increment);

			SetTextTo (value);

			yield return waitForSeconds;


		}

		SetTextTo (toValue);

		animationFunction = null;

	}

	void SetTextTo(float value){

		CachedText.SetFloatDecimalPlaces (this.decimalPlaces);
		CachedText.text = FreeUIExtensions.FloatToString (value);

	}

}

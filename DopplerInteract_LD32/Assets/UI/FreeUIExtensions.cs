using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public static class FreeUIExtensions {

	public const string BOOLEAN_TRUE_KEY = "UI_BOOL_TRUE";
	public const string BOOLEAN_FALSE_KEY = "UI_BOOL_FALSE";
	static int float_DecimalPlaces = 0;

	public static void AddClickListener(this Button button, UnityEngine.Events.UnityAction clickAction){

		button.onClick.RemoveListener (clickAction);
		button.onClick.AddListener (clickAction);

	}

	public static void HighlightForever(this Graphic image, Color targetColor, float timeToFlash, float timeUnhighlighted){

		image.Highlight (targetColor, timeToFlash, timeUnhighlighted, UIHighlightElement.FOREVER);

	}

	public static void Highlight(this Graphic image, Color targetColor, float timeToFlash, float timeUnhighlighted, int repetitions){

		UIHighlightElement animElement = image.GetHighlightElement ();

		animElement.HighlightElement (targetColor, timeToFlash, timeUnhighlighted, repetitions);

	}

	public static void StopHighlighting(this Graphic image, bool immediately){

		UIHighlightElement animElement = image.GetHighlightElement ();

		animElement.StopHighlighting (immediately);

	}

	public static void AlignToWorldPosition(this RectTransform alignObject, Vector3 targetPosition){

		Vector3 screenPos = Camera.main.WorldToScreenPoint (targetPosition);

		alignObject.position = 
			Camera.main.ScreenToWorldPoint(new Vector3 (screenPos.x,screenPos.y, FreeUIEffectManager.Instance.UICanvas.planeDistance));

	}

	public static void AlignToWorldTransform(this RectTransform alignObject, Transform targetObject){

		Vector3 screenPos = Camera.main.WorldToScreenPoint (targetObject.position);

		alignObject.position = 
			Camera.main.ScreenToWorldPoint(new Vector3 (screenPos.x,screenPos.y, FreeUIEffectManager.Instance.UICanvas.planeDistance));

	}

	public static void AlignToWorldPosition(this RectTransform alignObject, Vector3 targetPosition, float planeDistance){

		Vector3 screenPos = Camera.main.WorldToScreenPoint (targetPosition);

		alignObject.position = 
			Camera.main.ScreenToWorldPoint(new Vector3 (screenPos.x,screenPos.y,planeDistance));

	}

	public static void PulsateForever(this RectTransform targetTransform, float targetScale, float scaleTime){

		targetTransform.Pulsate (targetScale, scaleTime, UIPulsateElement.FOREVER);

	}

	public static void Pulsate(this RectTransform targetTransform, float targetScale, float scaleTime, int repetitions){

		UIPulsateElement animElement = targetTransform.GetPulsateElement ();

		animElement.PulsateElement (targetScale, scaleTime, repetitions);

	}

	public static void StopPulsating(this RectTransform targetTransform, bool immediately){

		UIPulsateElement animElement = targetTransform.GetPulsateElement ();

		animElement.StopPulsating (immediately);

	}

	public static void AnimateBetweenValues(this Text textField, int fromValue, int toValue, 
		int increment, float timePerIncrement, float delay){

		UIAnimateValueElement animElement = textField.GetAnimationElement ();

		animElement.AnimateValue (fromValue, toValue, increment, timePerIncrement,delay);

	}

	public static void AnimateBetweenValues(this Text textField, float fromValue, float toValue, 
		float increment, float timePerIncrement, float delay){

		UIAnimateValueElement animElement = textField.GetAnimationElement ();

		animElement.AnimateValue (fromValue, toValue, increment, timePerIncrement,float_DecimalPlaces,delay);

	}

	public static void AnimateBetweenValues(this Text textField, float fromValue, float toValue, 
		float increment, float timePerIncrement, int decimalPlaces, float delay){

		UIAnimateValueElement animElement = textField.GetAnimationElement ();

		animElement.AnimateValue (fromValue, toValue, increment, timePerIncrement,decimalPlaces,delay);

	}

	public static void FinalizeAnimationImmediately(this Text textField){

		UIAnimateValueElement animElement = textField.GetAnimationElement ();

		animElement.FinalizeAnimationImmediately ();

	}

	public static UIHighlightElement GetHighlightElement(this Graphic image){

		UIHighlightElement animElement = image.GetComponent<UIHighlightElement>();

		if(animElement == null){

			animElement = image.gameObject.AddComponent<UIHighlightElement>();

		}

		return animElement;

	}

	public static UIPulsateElement GetPulsateElement(this RectTransform rectTransform){

		UIPulsateElement animElement = rectTransform.GetComponent<UIPulsateElement>();

		if(animElement == null){

			animElement = rectTransform.gameObject.AddComponent<UIPulsateElement>();

		}

		return animElement;

	}

	public static UIAnimateValueElement GetAnimationElement(this Text textField){

		UIAnimateValueElement animElement = textField.GetComponent<UIAnimateValueElement>();

		if(animElement == null){

			animElement = textField.gameObject.AddComponent<UIAnimateValueElement>();

		}

		return animElement;

	}

	public static void SetFloatDecimalPlaces(this Text textField, int decimalPlaces){

		float_DecimalPlaces = decimalPlaces;

	}

	public static string BooleanToString(bool b){

		if (b) {

			return BOOLEAN_TRUE_KEY;

		} else {

			return BOOLEAN_FALSE_KEY;

		}

	}

	public static string IntToString(int i){

		return i.ToString ();

	}

	public static string FloatToString(float f){

		string formatString = "n" + float_DecimalPlaces.ToString ();

		return f.ToString (formatString);

	}

}

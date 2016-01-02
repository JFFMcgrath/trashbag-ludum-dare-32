using System;
using UnityEngine;

[Serializable]
public class ClampedAnimationCurve
{

	public AnimationCurve animationCurve;
	public bool useCurve = true;
	Vector2 curveBounds;
	float maxTime;
	bool initialized = false;
	Keyframe[] keys;

	void Initialize(){

		curveBounds = new Vector2(float.MaxValue,float.MinValue);
		maxTime = float.MinValue;

		this.keys = animationCurve.keys;

		for(int i = 0; i < keys.Length; i++){

			if(keys[i].value < curveBounds.x){
				curveBounds.x = animationCurve.keys[i].value;
			}

			if(keys[i].value > curveBounds.y){
				curveBounds.y = animationCurve.keys[i].value;
			}

			if(keys[i].time > maxTime){
				maxTime = animationCurve.keys[i].time;
			}

		}

		if (keys.Length == 0) {
			useCurve = false;
		}


		initialized = true;
	}

	float GetNormalizedValue(float min, float max, float value){

		float range = max - min;

		if(range == 0){
			return value;
		}

		value -= min;

		return value / range;

	}

	public float GetValueAt(float n){

		if(!initialized){
			Initialize();
		}

		if(!useCurve){
			return n;
		}

		if(keys.Length == 0){
			return n;
		}

		if(keys.Length == 0){
			return n;
		}

		n = Mathf.Clamp(n,0f,1f);

		float nTime = n * maxTime;

		float value = animationCurve.Evaluate(nTime);

		return GetNormalizedValue(curveBounds.x,curveBounds.y,value);

	}

	public float HalfWrapValue(float n){

		float mod_n = n;

		if(mod_n > 0.5f){

			mod_n -= 0.5f;

			mod_n = mod_n / 0.5f;
			mod_n = 1.0f - mod_n;

		}
		else{
			mod_n = mod_n / 0.5f;
		}

		return mod_n;

	}

}



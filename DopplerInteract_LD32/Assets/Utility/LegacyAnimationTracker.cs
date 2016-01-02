using System;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Animation))]
public class LegacyAnimationTracker : EfficientBehaviour
{

	public const float BEGIN_TIME = 0f;
	public const float MID_TIME = 0.5f;
	public const float END_TIME = 1f;

	List<AnimationSubscription> animationSubscriptions = new List<AnimationSubscription>();
	
	public string currentPlayingClipName{
		get;
		private set;	
	}
	
	public float lastAnimationUpdateTime{
		get;
		private set;
	}

	public void Initialize(){
		
		CachedLegacyAnimation.playAutomatically = false;
				
	}

#region Event Updates
		
	void LateUpdate(){
		
		if(animationSubscriptions.Count == 0){
			return;
		}
		
		bool endEvent = false;
		
		if(!CachedLegacyAnimation.isPlaying){
		
			if(!string.IsNullOrEmpty(currentPlayingClipName)){
				endEvent = true;
			}
			else{
				return;
			}
		}
		
		if(!endEvent && !CachedLegacyAnimation.IsPlaying(currentPlayingClipName)){
			Debug.LogWarning("Clip name cache out of sync.");
			return;
		}
		
		float animationTime = GetNormalizedAnimationTime(currentPlayingClipName);
	
		TriggerEventsForTimeRange(lastAnimationUpdateTime,animationTime);

		lastAnimationUpdateTime = animationTime;

		if(endEvent){
			currentPlayingClipName = null;
			lastAnimationUpdateTime = 0f;
		}

	
	}
	
	void TriggerEventsForTimeRange(float start, float end){
			
		for(int i = 0; i < animationSubscriptions.Count; i++){
			
			if(animationSubscriptions[i].animationName != currentPlayingClipName){
				continue;
			}
			
			if(animationSubscriptions[i].FallsBetween(start,end)){

				animationSubscriptions[i].TriggerEvent();
			}
			
		}
			
	}
	
#endregion

#region Animation Accessor Functions

	public void PlayAnimation(string animationName){
		PlayAnimation(animationName,false);
	}

	public void PlayAnimation(string animationName, bool continueFromCurrentTime){
	
		if (IsPlayingAnimation(animationName)) {

		} else {

			currentPlayingClipName = animationName;
			CachedLegacyAnimation.clip = CachedLegacyAnimation.GetClip (animationName);
			CachedLegacyAnimation.Play (animationName);

		}
		
		if(!continueFromCurrentTime){

			CachedLegacyAnimation[animationName].normalizedTime = 0f;
			lastAnimationUpdateTime = 0f;

		}
	
	}
		
	public bool IsPlayingAnimation(string animation){
	
		if(CachedLegacyAnimation.IsPlaying(animation)){
		
			if(animation != currentPlayingClipName){
			
				Debug.LogWarning("Animation cached clip name is out of sync!");
				currentPlayingClipName = animation;
			
			}
			
			return true;
		
		}
		
		return false;
	
	}
	
	public void StopAnimation(string animationName){
	
		StopAnimation(animationName,false);
	
	}
	
	public void StopAnimation(string animationName, bool sampleStop){

		if(IsPlayingAnimation(animationName)){
									
			if(sampleStop){
				SampleAnimation(animationName,0f);
			}

			CachedLegacyAnimation.Stop(animationName);		
						
			currentPlayingClipName = null;
			lastAnimationUpdateTime = 0f;	
		}
				
	}

	public void StopAllAnimations ()
	{

		if (!string.IsNullOrEmpty (currentPlayingClipName)) {
			StopAnimation (currentPlayingClipName, true);
		}

		CachedLegacyAnimation.Stop ();
		currentPlayingClipName = null;
		lastAnimationUpdateTime = 0f;	
	}

	public void SampleAnimation(string animationName, float sampleTime){
	
		PlayAnimation (animationName);

		CachedLegacyAnimation[animationName].normalizedTime = sampleTime;
		CachedLegacyAnimation[animationName].enabled = true;
		CachedLegacyAnimation.Sample ();
		CachedLegacyAnimation[animationName].enabled = false;

		StopAnimation (animationName);

		if(!IsPlayingAnimation(animationName)){
				
			lastAnimationUpdateTime = sampleTime;
		
		}
	
	}
	
	public void SetAnimationTime(string animationName, float time){
	
		CachedLegacyAnimation[animationName].normalizedTime = time;

		if (CachedLegacyAnimation.IsPlaying (animationName)) {
			lastAnimationUpdateTime = GetNormalizedAnimationTime(animationName);
		}

			
	}
	
	public void SetNormalizedAnimationSpeed(string animationName, float speed){

		CachedLegacyAnimation[animationName].normalizedSpeed = speed;

		if (CachedLegacyAnimation.IsPlaying (animationName)) {
			lastAnimationUpdateTime = GetNormalizedAnimationTime(animationName);
		}

	}

	public void SetAnimationSpeed(string animationName, float speed){

		CachedLegacyAnimation[animationName].speed = speed;

		if (CachedLegacyAnimation.IsPlaying (animationName)) {
			lastAnimationUpdateTime = GetNormalizedAnimationTime(animationName);
		}

	}

	public float GetNormalizedAnimationTime(string animationName){

		float time = CachedLegacyAnimation [currentPlayingClipName].normalizedTime;
		float speed = CachedLegacyAnimation [currentPlayingClipName].normalizedSpeed;

		float m_time = time % 1f;

		return m_time / speed;

	}
	
#endregion
	
#region Event Subscription
	
	public delegate void OnAnimationCompleted(string animationName);
		
	public void SubscribeToAnimationEvent(string animationName, float animationTime, OnAnimationCompleted completionEvent){
		
		AnimationSubscription sub = new AnimationSubscription(
			animationName,
			animationTime,
			completionEvent
			);
		
		animationSubscriptions.Add(sub);
		
	}
	
	public void UnsubscribeFromAnimationEvent(string animationName, float animationTime, OnAnimationCompleted completionEvent){
		
		AnimationSubscription sub = null;
		
		for(int i = 0; i < animationSubscriptions.Count; i++){
			
			sub = animationSubscriptions[i];
			
			if(sub.animationName == animationName &&
			   sub.animationTime == animationTime &&
			   sub.animationCompletedEvent == completionEvent){
				
				animationSubscriptions.RemoveAt(i);
				i--;
				
			}
			
		}
		
	}
	
	
	class AnimationSubscription{
		
		public string animationName;
		public float animationTime;
		public OnAnimationCompleted animationCompletedEvent;
		
		public override bool Equals (object obj)
		{
			AnimationSubscription ac = obj as AnimationSubscription;
			
			if(ac != null){
				
				return (
				ac.animationName == animationName &&
				ac.animationTime == animationTime &&
				ac.animationCompletedEvent == animationCompletedEvent
				);
				
			}
			
			return false;
		}
		
		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}
		
		public AnimationSubscription(string animationName, float animationTime, OnAnimationCompleted animationCompletedEvent){
			
			this.animationName = animationName;
			this.animationTime = animationTime;
			this.animationCompletedEvent = animationCompletedEvent;
			
		}
		
		public bool FallsBetween(float previousTime, float currentTime){

			//string time = (previousTime + " to " + currentTime);

			//Wrapping						
			if(currentTime < previousTime){

				//Debug.Log ("1: " + currentTime + " < " + previousTime + "("+time+")");
				
				if(animationTime < currentTime){

					//Debug.Log ("2: " + animationTime + " < " + currentTime + "("+time+")");

					return true;
				}
				
				if(animationTime >= previousTime){

					//Debug.Log ("3: " + animationTime + " >= " + previousTime + "("+time+")");

					return true;
				}
				
			}
			
			if(animationTime >= previousTime && animationTime < currentTime){

				//Debug.Log ("4: " + animationTime + " >= " + previousTime + " && " +  animationTime + " < " + currentTime + "("+time+")");

				return true;
			}
			
			return false;
			
		}
		
		public void TriggerEvent(){
		
			if(this.animationCompletedEvent != null){
				this.animationCompletedEvent(animationName);
			}
		
		}
		
		public override string ToString ()
		{
			return animationName + " @" + animationTime;
		}
		
	}
	
#endregion
	
}



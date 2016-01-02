using System;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class AnimationTracker : MonoBehaviour
{

	#region Animation State Management

	public Dictionary<int,string> cleanNameDictionary = new Dictionary<int,string>();

	public string GetCleanStateName(int nameHash){
	
		if (cleanNameDictionary.ContainsKey (nameHash)) {
			return cleanNameDictionary[nameHash];		
		}

		Debug.LogWarning ("No name hash in dictionary for: " + nameHash);

		return "Null";

	}

	public void InitializeStates(Type stateEnumType){

		cleanNameDictionary.Clear();

		string[] names = Enum.GetNames(stateEnumType);

		for(int i = 0; i < names.Length; i++){

			names[i] = "Base Layer." + names[i];

			cleanNameDictionary.Add(Animator.StringToHash(names[i]),names[i]);
		}
	
	}

	public void SetOneShotState(string name, OnAnimationStateEvent onComplete){

		if(boundAnimator == null){
			return;
		}	

		if(boundAnimator.runtimeAnimatorController == null){
			return;
		}

		boundAnimator.SetTrigger (name);

		SubscribeToEvent(name,1f,onComplete);

	}

	public void SetState(string name, bool state, OnAnimationStateEvent onComplete){

		if(boundAnimator == null){
			return;
		}

		if(boundAnimator.runtimeAnimatorController == null){
			return;
		}

		boundAnimator.SetBool (name, state);
		
		SubscribeToEvent(name,1f,onComplete);
		
	}

	#endregion

	void Start(){

		AnimatorStateInfo info = boundAnimator.GetCurrentAnimatorStateInfo (0);
		
		int state = info.nameHash;

		currentAnimationState = state;

	}

	public virtual void OnStart(){}

	Animator _boundAnimator;

	public Animator boundAnimator{

		get{

			if(_boundAnimator == null){
				_boundAnimator = GetComponent<Animator>();
			}

			return _boundAnimator;
		
		}

	}

	public class AnimationStateSubscriber{
	
		public int animationState;
		public float animationTime;

		public bool canTrigger = true;

		AnimationTracker.OnAnimationStateEvent stateDelegate;

		public AnimationStateSubscriber(int animationState, float animationTime, 
		                                AnimationTracker.OnAnimationStateEvent stateDelegate){
		
			this.animationTime = animationTime;
			this.animationState = animationState;
			this.stateDelegate = stateDelegate;

		}

		public void Trigger(){
		
			if (canTrigger) {
				//Trigger
				canTrigger = false;

				if(stateDelegate != null){
					stateDelegate();
				}

			}

		}

	}

	public void SubscribeToEvent(string animationState, float animationTime, AnimationTracker.OnAnimationStateEvent stateDelegate){
	
		if(stateDelegate == null){
			return;
		}

		animationState = "Base Layer." + animationState;

		int state = Animator.StringToHash (animationState);

		AnimationStateSubscriber subscriber = new AnimationStateSubscriber (state, animationTime, stateDelegate);

		subscribers.Add (subscriber);

	}

	public List<AnimationStateSubscriber> subscribers = new List<AnimationStateSubscriber>();

	public delegate void OnAnimationStateEvent();
	public delegate void OnAnimationStateChange(string stateName);

	public OnAnimationStateChange stateBegun;
	public OnAnimationStateChange stateEnded;

	int currentAnimationState;

	void BeginState(int state){

		for (int i = 0; i < subscribers.Count; i++) {
			
			if(subscribers[i].animationState == state){
				
				if(subscribers[i].animationTime == 0f){
					subscribers[i].Trigger();

					//Controversial - just based on the way its structured now
					//Remove
					subscribers.RemoveAt(i);
					i--;
					continue;

				}
				
			}
			
		}

	}

	void EndState(int state){

		for (int i = 0; i < subscribers.Count; i++) {
		
			if(subscribers[i].animationState == state){

				if(subscribers[i].animationTime == 1f){
					subscribers[i].Trigger();

					//Controversial - just based on the way its structured now
					//Remove
					subscribers[i].canTrigger = true;
					subscribers.RemoveAt(i);
					i--;

					continue;

				}

				subscribers[i].canTrigger = true;

			}

		}

	}

	void ProcessStateProgress(int state, float time){

		for (int i = 0; i < subscribers.Count; i++) {
			
			if(subscribers[i].animationState == state){
				
				if(subscribers[i].animationTime < time){
					subscribers[i].Trigger();

					//Controversial - just based on the way its structured now
					//Remove
					subscribers.RemoveAt(i);
					i--;

					continue;

				}
				
			}
			
		}

	}

	public float GetCurrentStateDuration(){

		AnimatorStateInfo info = boundAnimator.GetCurrentAnimatorStateInfo (0);

		return info.length;

	}

	public string GetCurrentPlayingState(){

		AnimatorStateInfo info = boundAnimator.GetCurrentAnimatorStateInfo (0);

		int state = info.nameHash;

		return GetCleanStateName (state);

	}

	void Update(){
	
		AnimatorStateInfo info = boundAnimator.GetCurrentAnimatorStateInfo (0);

		int state = info.nameHash;

		ProcessStateProgress (currentAnimationState, info.normalizedTime);

		if (state != currentAnimationState) {
		
			EndState(currentAnimationState);

			currentAnimationState = state;

			BeginState(currentAnimationState);

		}

		//Track animations here
		OnUpdate ();

	}

	public virtual void OnUpdate(){}

}



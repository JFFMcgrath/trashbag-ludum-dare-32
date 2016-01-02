using UnityEngine;
using System.Collections;

public class TrashBehavior : StateMachineBehaviour {

	public delegate void HandleStateEnter(int stateHash);
	HandleStateEnter _onStateEnter;

	public delegate void HandleStateExit(int stateHash);
	HandleStateExit _onStateExit;

	public void SubscribeToStateEvents(HandleStateEnter onStateEnter, HandleStateExit onStateExit){
		_onStateEnter = onStateEnter;
		_onStateExit = onStateExit;
	}

	public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{

		if (_onStateExit != null) {
			_onStateExit (stateInfo.shortNameHash);
		}

	}

	public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{

		if (_onStateEnter != null) {
			_onStateEnter (stateInfo.shortNameHash);
		}

	}

}

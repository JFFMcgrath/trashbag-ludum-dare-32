using UnityEngine;
using System.Collections;

public class CharacterFoot : EfficientBehaviour {

	IEnumerator _moveFunction = null;

	public delegate void MoveCompleted();
	public delegate void MoveInterrupted();

	MoveCompleted _moveCompleted = null;
	MoveInterrupted _moveInterrupted = null;

	public bool IsMoving(){

		return _moveFunction != null;

	}

	public void MoveTo(Vector3[] positions, float timePerUnit, 
		MoveCompleted moveCompleted, MoveInterrupted moveInterrupted){

		if (_moveFunction != null) {

			if (_moveInterrupted != null) {
				_moveInterrupted ();
			}

			StopCoroutine (_moveFunction);
			_moveFunction = null;

		}

		_moveCompleted = moveCompleted;
		_moveInterrupted = moveInterrupted;

		_moveFunction = HandleMoveTo (positions,timePerUnit);

		StartCoroutine (_moveFunction);

	}

	WaitForSeconds _waitForSeconds = new WaitForSeconds(0);

	IEnumerator HandleMoveTo(Vector3[] positions, float timePerUnit){

		Vector3 fromPosition = CachedTransform.position;
		Vector3 toPosition;

		for (int i = 0; i < positions.Length; i++) {

			toPosition = positions [i];

			float timeToMove = Vector2.Distance (toPosition, fromPosition) * timePerUnit;

			float currentTime = 0f;
			float n = 0f;

			while (n < 1f) {

				currentTime += Time.deltaTime;
				n = currentTime / timeToMove;

				CachedTransform.position = Vector3.Lerp (fromPosition, toPosition, n);

				yield return _waitForSeconds; 

			}

			fromPosition = positions [i];

			yield return _waitForSeconds; 

		}

		_moveFunction = null;

		if (_moveCompleted != null) {
			_moveCompleted ();
		}

	}

}

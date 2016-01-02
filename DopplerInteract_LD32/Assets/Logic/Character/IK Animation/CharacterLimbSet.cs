using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterLimbSet : EfficientBehaviour {

	public CharacterFoot[] characterLimbs;

	List<CharacterFoot> _sortedCaracterLimbs;
	List<CharacterFoot> SortedCharacterLimbs{
		get{

			if (_sortedCaracterLimbs == null || _sortedCaracterLimbs.Count == 0) {

				_sortedCaracterLimbs = new List<CharacterFoot> (characterLimbs);

			}

			return _sortedCaracterLimbs;
		}
	}

	int currentLimbIndex = 0;

	CharacterFoot CurrentLimb{
		get{

			return characterLimbs [currentLimbIndex];

		}
	}

	public Vector2[] offsetChain;
	public float footTimePerUnit;
	CharacterFoot.MoveCompleted _moveCompleted;
	CharacterFoot.MoveInterrupted _moveInterrupted;

	public enum LimbSetDirection{
		None,
		Left,
		Right
	}

	LimbSetDirection currentDirection;

	#region Debug

	void Update(){

		if(Input.GetMouseButtonDown(0)){

			Vector2 worldPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			RaycastHit2D hit = Physics2D.Raycast (worldPosition, Vector2.up * -1f);

			if (hit.collider != null) {

				Vector2 point = hit.point;
				Vector2 normal = hit.normal;

				MoveCharacterFootTo (point, normal, CurrentLimb, null, null);

			}

		}

	}

	public float debug_LimbLength;

	void OnDrawGizmos(){

		if (!Application.isPlaying) {
			return;
		}

		Vector2 i1 = Vector2.zero, i2 = Vector2.zero;

		Vector2 fp1 = characterLimbs [0].CachedTransform.position;
		Vector2 fp2 = characterLimbs [1].CachedTransform.position;

		int points = FreeMathUtilities.FindCircleCircleIntersections (fp1, debug_LimbLength, fp2, debug_LimbLength, out i1, out i2);

		Gizmos.color = Color.yellow;

		Gizmos.DrawWireSphere (fp1, debug_LimbLength);
		Gizmos.DrawWireSphere (fp2, debug_LimbLength);

		Gizmos.color = Color.red;

		if (points >= 1) {

			Gizmos.DrawSphere (i1, 0.5f);

		} 

		if (points >= 2) {

			Gizmos.DrawSphere (i2, 0.5f);

		}

	}

	#endregion

	public Vector3 GetAverageLimbPosition(){

		Vector3 average = Vector3.zero;

		for (int i = 0; i < SortedCharacterLimbs.Count; i++) {

			average += SortedCharacterLimbs [i].CachedTransform.position;

		}

		return average / (float)SortedCharacterLimbs.Count;

	}

	public CharacterFoot GetForwardLimb(){

		return SortedCharacterLimbs [0];

	}

	public List<CharacterFoot> GetSortedLimbs(){

		return SortedCharacterLimbs;

	}

	public void SetDirection(LimbSetDirection direction){

		if (direction == currentDirection) {
			return;
		}

		currentDirection = direction;

		SortedCharacterLimbs.Sort (SortLimbsByDirection);

	}

	public void MoveCharacterFootTo(Vector2 position, Vector2 normal,
		CharacterFoot.MoveCompleted moveCompleted, CharacterFoot.MoveInterrupted moveInterrupted){

		MoveCharacterFootTo (position, normal, CurrentLimb, moveCompleted, moveInterrupted);

	}

	public void MoveCharacterFootTo(Vector2 position, Vector2 normal, CharacterFoot foot, 
		CharacterFoot.MoveCompleted moveCompleted, CharacterFoot.MoveInterrupted moveInterrupted){

		Vector3[] chain = new Vector3[offsetChain.Length+1];

		for (int i = 0; i < offsetChain.Length; i++) {

			chain [i] = position + MultiplyVector (normal, offsetChain[i]);

		}

		chain [chain.Length - 1] = position;

		_moveCompleted = moveCompleted;
		_moveInterrupted = moveInterrupted;

		foot.MoveTo (chain, footTimePerUnit, CharacterFootMoveCompleted, CharacterFootMoveInterrupted);

	}

	void CharacterFootMoveCompleted(){

		currentLimbIndex += 1;

		if (currentLimbIndex >= characterLimbs.Length) {
			currentLimbIndex = 0;
		}

		if (_moveCompleted != null) {
			_moveCompleted ();
		}

	}

	void CharacterFootMoveInterrupted(){

		if (_moveInterrupted != null) {
			_moveInterrupted ();
		}

	}

	int SortLimbsByDirection(CharacterFoot a, CharacterFoot b){

		float a_x = CachedTransform.position.x;
		float b_x = CachedTransform.position.x;

		if (a_x > b_x) {
			return 1;
		} else if (a_x < b_x) {
			return -1;
		} else {
			return 0;
		}

	}

	Vector2 MultiplyVector(Vector2 a, Vector2 b){

		return new Vector2(
			a.x * b.x,
			a.y * b.y
		);

	}

}

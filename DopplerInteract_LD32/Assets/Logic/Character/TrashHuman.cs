using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class TrashHuman : TrashCollidingObject {

	public static Vector2 DIRECTION_LEFT = new Vector2 (-1f, 0f);
	public static Vector2 DIRECTION_RIGHT = new Vector2 (1f, 0f);

	public enum HumanState{
		None,
		Idle,
		Walking,
		Running,
		Jumping,
		Dying,
		Dead
	}

	public enum MovementDirection{
		None,
		Left,
		Right
	}

	public bool usePhysicsMovement;
	public ForceMode2D movementForceMode;

	public float walkSpeed;
	public float runSpeed;

	public float walkForce;
	public float runForce;
	public float maxWalkSpeed;
	public float maxRunSpeed;
	public float decaySpeed;

	public float jumpForce;

	public float debug_VelocityMagnitude;

	public const float GROUND_CHECK_FREQUENCY = 0.1f;
	protected bool _lastIsGrounded = false;
	protected Vector2 _lastGroundNormal = Vector2.up;
	float lastGroundCheck = 0f;

	public LayerMask groundedLayer{
		get{
			return TrashWorld.Instance.GetGroundedLayerMask ();
		}
	}

	public void SetGrounded(bool grounded){
		lastGroundCheck = Time.time + 1f;
		_lastIsGrounded = grounded;
	}

	void OnDrawGizmos(){

		if (IsGrounded) {
			Gizmos.color = Color.green;
		} else {
			Gizmos.color = Color.red;
		}

		Vector2 p = (CachedCollider2D.bounds.min);

		Gizmos.DrawWireSphere (p,0.5f);

		Gizmos.DrawLine (p, p + _lastGroundNormal * -1f * 10f);

	}

	public bool IsGrounded{
		get{

			if (Time.time - lastGroundCheck > GROUND_CHECK_FREQUENCY) {

				//Vector2 p = (CachedCollider2D.bounds.min);

				Vector2 p = CachedTransform.position;

				//p + Vector2.up * -1f * 0.05f

				RaycastHit2D r = Physics2D.Raycast (p + Vector2.up * 0.2f, Vector2.up * -1f, 0.1f,~groundedLayer.value);

				if (r.collider != null) {

					_lastIsGrounded = true;
					_lastGroundNormal = r.normal;

					if(GetState() == HumanState.Jumping){

						EndJump ();

					}


				} else {

					_lastIsGrounded = false;
					_lastGroundNormal = Vector2.up;

				}

			}

			return _lastIsGrounded;

		}
	}

	public float MovementSpeed{
		get{

			HumanState state = humanState.GetState ();

			if (state == HumanState.Walking) {
				return walkSpeed;
			} else if (state == HumanState.Running) {
				return runSpeed;
			} else {
				return 0f;
			}

		}
	}

	public float MaxJumpMoveSpeed;

	public float MaxSpeed{
		get{

			HumanState state = humanState.GetState ();

			if (state == HumanState.Walking) {
				return walkSpeed;
			} else if (state == HumanState.Running) {
				return maxRunSpeed;
			} else {
				return 0f;
			}

		}
	}

	public float MovementForce{
		get{

			HumanState state = humanState.GetState ();

			if (state == HumanState.Walking) {
				return walkForce;
			} else if (state == HumanState.Running) {
				return runForce;
			} else {
				return 0f;
			}

		}
	}

	protected StateMachine<HumanState> humanState;
	protected MovementDirection currentMovementDirection;

	TrashBehavior[] anim_TrashBehaviors;

	static Dictionary<int,HumanState> anim_HashToState;
	static List<HumanState> list_States;

	protected override void OnInitializeTrashObject (){

		humanState = new StateMachine<HumanState> (BeginHumanState, EndHumanState);

		if (anim_HashToState == null) {

			InitializeNameHashes ();

		}

		anim_TrashBehaviors = CachedAnimator.GetBehaviours<TrashBehavior> ();

		if (anim_TrashBehaviors != null) {

			for (int i = 0; i < anim_TrashBehaviors.Length; i++) {

				anim_TrashBehaviors[i].SubscribeToStateEvents (OnTrashAnimStateBegins, OnTrashAnimStateEnds);

			}

		} else {
			Debug.LogError ("No trash behavior on : " + name);
		}

		OnInitialize ();

	}

	protected virtual void OnInitialize(){}

	bool jumpMoving = false;

	public void StartMovement(MovementDirection movementDirection, HumanState movementType){

		if (IsJumping ()) {

			if (movementDirection == currentMovementDirection) {
				return;
			}

			ChangeDirection (movementDirection);

			jumpMoving = true;

			return;

		}

		if (IsMoving ()) {

			CachedRigidbody2D.velocity = Vector2.zero;

			EndMovement ();
		}

		ChangeDirection (movementDirection);
		SetState (movementType);

	}

	public void EndJumpMovement(){

		jumpMoving = false;

	}

	public HumanState test_CurrentState;

	IEnumerator WaitForJump(){

		yield return new WaitForSeconds (3f);

		if (GetState () == HumanState.Jumping) {

			EndJump ();

		}

	}

	public void Jump(){

		if (GetState () == HumanState.Jumping) {
			return;
		}

		SetGrounded (false);

		CachedRigidbody2D.isKinematic = false;

		Vector2 jumpVector = Vector2.up * jumpForce;

		if (usePhysicsMovement) {

			Vector2 velocity = CachedRigidbody2D.velocity;

			velocity = Vector2.ClampMagnitude (velocity, MaxSpeed);

			if (IsMoving ()) {
				//jumpVector = (Vector2.up + GetDirectionVector ()).normalized;
				jumpVector = (Vector2.up + GetDirectionVector () * 0.5f);
			} else {
				jumpVector = Vector2.up;
			}

			jumpVector *= jumpForce;

		} else {

			Vector2 movementDirection = GetDirectionVector ();
			float movementSpeed = MovementSpeed;

			jumpVector += movementDirection * movementSpeed;

		}


		EndMovement ();

		humanState.SetState (HumanState.Jumping);

		CachedRigidbody2D.AddForce (jumpVector, ForceMode2D.Impulse);

		StartCoroutine (WaitForJump ());


	}

	public void EndJump(){

		CachedRigidbody2D.velocity = Vector2.zero;

		SetState (HumanState.Idle);

	}

	public MovementDirection GetMovementDirection(){

		return currentMovementDirection;

	}

	protected void ChangeDirection (MovementDirection movementDirection)
	{

		currentMovementDirection = movementDirection;

		Vector3 localScale = CachedTransform.localScale;

		if (currentMovementDirection == MovementDirection.Left) {
			localScale.x = -1f;
		} else {
			localScale.x = 1f;
		}

		CachedTransform.localScale = localScale;

		RefreshPickupItem ();

		OnChangeDirection (movementDirection);
	}

	public bool IsFacingDirection(Vector2 dir){

		if (GetDirectionVector ().x < 0f && dir.x < 0f) {
			return false;
		}

		if (GetDirectionVector().x > 0f && dir.x > 0f) {
			return false;
		}

		return true;

	}

	public Vector2 ModifyUpVectorByDirection(Vector2 up){

		if (CachedTransform.localScale.x == -1f) {

			up.x *= -1f;

		}

		return up;

	}

	protected virtual void OnChangeDirection(MovementDirection movementDirection)
	{

	}

	void FixedUpdate(){

		if (IsMoving ()) {

			UpdateMovement ();

		} else if ((!IsJumping()) && GetState() != HumanState.Dying){

			if (IsGrounded) {
				UpdateDecay ();
			}

		}

		HandleFixedUpdate ();

	}

	public virtual void HandleFixedUpdate(){}

	void Update(){

		debug_VelocityMagnitude = CachedRigidbody2D.velocity.magnitude;

		HandleUpdate ();

	}

	public virtual void HandleUpdate(){}

	static Transform _trashTransform;
	Transform TrashTransform{
		get{

			if (_trashTransform == null) {

				GameObject o = new GameObject ("Trash transform");

				_trashTransform = o.transform;

			}

			return _trashTransform;

		}
	}

	void UpdateMovement(){

		if (usePhysicsMovement) {

			/*TrashTransform.up = _lastGroundNormal;
			TrashTransform.position = CachedTransform.position;

			Vector2 direction;

			if (currentMovementDirection == MovementDirection.Right) {
				direction = TrashTransform.right;
			} else {
				direction = TrashTransform.right * -1f;
			}

			CachedRigidbody2D.AddForce (direction * MovementForce, movementForceMode);*/

			CachedRigidbody2D.AddForce (GetDirectionVector () * MovementForce, movementForceMode);

			//CachedRigidbody2D.AddForceAtPosition (GetDirectionVector () * MovementForce, CachedTransform.position + Vector3.up * 2f, movementForceMode);

			CachedRigidbody2D.velocity = Vector2.ClampMagnitude (CachedRigidbody2D.velocity, MaxSpeed);


		} else {

			Vector3 newPosition = CachedRigidbody2D.position + (GetDirectionVector () * MovementSpeed * Time.fixedDeltaTime);

			CachedRigidbody2D.MovePosition (newPosition);

		}

	}

	void UpdateDecay ()
	{

		if (!usePhysicsMovement) {
			return;
		}

		if (CachedRigidbody2D.velocity == Vector2.zero) {
			return;
		}

		Vector2 velocity = CachedRigidbody2D.velocity;

		velocity.x = Mathf.Lerp (velocity.x, 0f, Time.fixedDeltaTime * decaySpeed);

		CachedRigidbody2D.velocity = velocity;

	}

	public bool CanJump(){

		if (GetState () == HumanState.Jumping || IsDead()) {

			return false;

		}

		return HandleCanJump ();

	}

	protected abstract bool HandleCanJump ();

	public bool CanMove(){

		if (IsJumping () || IsDead ()) {
			return false;
		}

		return HandleCanMove ();

	}

	protected abstract bool HandleCanMove ();

	public bool IsDead(){

		HumanState humanState = this.humanState.GetState ();

		if (humanState == HumanState.Dying || humanState == HumanState.Dead) {
			return true;
		}

		return false;

	}

	public bool IsMovingInDirection(MovementDirection direction, HumanState movementType){

		return IsMoving () && currentMovementDirection == direction && humanState.GetState() == movementType;

	}

	public bool IsMoving ()
	{

		HumanState humanState = this.humanState.GetState ();

		if (humanState == HumanState.Running || humanState == HumanState.Walking) {
			return true;
		}

		return false;

	}

	public bool IsJumping ()
	{

		HumanState humanState = this.humanState.GetState ();

		return humanState == HumanState.Jumping;

	}

	public Vector2 GetCurrentVelocity(){

		return CachedRigidbody2D.velocity;

	}

	protected Vector2 GetDirectionVectorFor(MovementDirection direction){


		if (direction == MovementDirection.Left) {

			return DIRECTION_LEFT;

		} else if (direction == MovementDirection.Right) {

			return DIRECTION_RIGHT;

		}

		return Vector2.zero;

	}

	public Vector2 GetDirectionVector ()
	{

		return GetDirectionVectorFor (currentMovementDirection);

	}

	public void EndMovement(){

		SetState (HumanState.Idle);

	}

	#region State Management

	public void SetState(HumanState state){

		humanState.SetState (state);

	}

	public HumanState GetState(){

		return humanState.GetState ();

	}

	protected void SetAnimationState(HumanState state, bool enabled, bool clearStates){

		SetAnimationState (state.ToString (), enabled, clearStates);

	}

	protected void SetAnimationTrigger(string trigger){

		CachedAnimator.SetTrigger (trigger);

	}

	protected void SetAnimationState(string state, bool enabled, bool clearStates){

		if(!HasAnimationParameter(state)){
			return;
		}

		if (clearStates) {
			ClearAnimationStates ();
		}

		CachedAnimator.SetBool (state, enabled);

	}

	protected void ClearAnimationStates(){

		for(int i = 0; i < list_States.Count; i++){

			if(!HasAnimationParameter(list_States[i].ToString())){
				continue;
			}

			CachedAnimator.SetBool (list_States[i].ToString (), false);

		}

	}

	void OnTrashAnimStateBegins (int stateHash)
	{

		HumanState state = GetStateForHash (stateHash);

		if (state != HumanState.None) {

			HandleAnimStateBegins (state);

		}

		HandleTrashAnimStateBegins (stateHash);

	}

	protected abstract void HandleTrashAnimStateBegins (int stateHash);

	protected abstract void HandleAnimStateBegins (HumanState state);

	void OnTrashAnimStateEnds (int stateHash)
	{

		HumanState state = GetStateForHash (stateHash);

		if (state != HumanState.None) {

			HandleAnimStateEnds (state);

		}

		HandleTrashAnimStateEnds (stateHash);

	}

	protected abstract void HandleTrashAnimStateEnds (int stateHash);

	protected abstract void HandleAnimStateEnds (HumanState state);

	public HumanState GetStateForHash(int hash){

		if (anim_HashToState.ContainsKey (hash)) {

			return anim_HashToState [hash];

		}

		return HumanState.None;

	}

	void InitializeNameHashes ()
	{

		anim_HashToState = new Dictionary<int, HumanState> ();

		list_States = new List<HumanState> ();

		string[] names = Enum.GetNames (typeof(HumanState));

		for (int i = 0; i < names.Length; i++) {

			int hash = Animator.StringToHash (names [i]);

			anim_HashToState.Add (hash, (HumanState)i);

			list_States.Add ((HumanState)i);

		}

		OnInitializeNameHashes ();

	}

	protected virtual void OnInitializeNameHashes(){}

	void BeginHumanState (HumanState state)
	{

		test_CurrentState = state;

		SetAnimationState(state, true, true);

		switch (state) {
			case HumanState.None:
			{
				break;
			}
			case HumanState.Idle:
			{
				break;
			}
			case HumanState.Walking:
			{
				break;
			}
			case HumanState.Running:
			{
				break;
			}
			case HumanState.Dying:
			{
				break;
			}
			case HumanState.Dead:
			{
				break;
			}
			case HumanState.Jumping:
			{
				break;
			}
		}

		OnBeginHumanState (state);

	}

	protected virtual void OnBeginHumanState (HumanState state){
	}

	void EndHumanState (HumanState state)
	{
		switch (state) {
			case HumanState.None:
			{
				break;
			}
			case HumanState.Idle:
			{
				break;
			}
			case HumanState.Walking:
			{
				break;
			}
			case HumanState.Running:
			{
				break;
			}
			case HumanState.Dying:
			{
				break;
			}
			case HumanState.Dead:
			{
				break;
			}
			case HumanState.Jumping:
			{
				break;
			}
		}

		OnEndHumanState (state);
	}

	protected virtual void OnEndHumanState (HumanState state){
	}

	void UpdateHumanState(HumanState state){

		switch (state) {
			case HumanState.None:
			{
				break;
			}
			case HumanState.Idle:
			{
				break;
			}
			case HumanState.Walking:
			{
				break;
			}
			case HumanState.Running:
			{
				break;
			}
			case HumanState.Dying:
			{
				break;
			}
			case HumanState.Dead:
			{
				break;
			}
			case HumanState.Jumping:
			{
				break;
			}
		}

	}

	#endregion

	#region Item Pickup

	public Transform bind_ItemBind;
	protected TrashItem item_CurrentItem;

	protected void RefreshPickupItem(){

		if (HasItem ()) {
			PickupItem (item_CurrentItem);
		}

	}

	public void PickupItem(TrashItem item){

		if (HasItem ()) {
			DropItem ();
		}

		item_CurrentItem = item;

		item.CachedTransform.SetParent (bind_ItemBind,false);
		item.CachedTransform.localPosition = Vector2.zero;
		item.CachedTransform.localRotation = Quaternion.identity;

		item.HandlePickup ();

	}

	public void ThrowItem(){

		if (HasItem ()) {

			item_CurrentItem.HandleThrow();
			item_CurrentItem = null;

		}

	}

	public void ThrowItem(Vector2 velocity){

		if (HasItem ()) {

			item_CurrentItem.HandleThrow(velocity);
			item_CurrentItem = null;

		}

	}

	public void ThrowItem(Vector2 direction, float force, ForceMode2D forceMode){

		if (HasItem ()) {

			item_CurrentItem.HandleThrow(direction,force,forceMode);
			item_CurrentItem = null;

		}

	}

	public void DropItem(){

		if (HasItem ()) {

			item_CurrentItem.HandleDrop ();
			item_CurrentItem = null;

		}

	}

	public bool HasItem(){

		return item_CurrentItem != null;

	}

	#endregion

	#region implemented abstract members of TrashCollidingObject
	public override void HandleCollisionEnter (Collision2D collision)
	{

		for (int i = 0; i < collision.contacts.Length; i++) {

			if (collision.contacts [i].point.y <= CachedTransform.position.y + 1f) {
				SetGrounded (true);
			}

		}

		ProcessCollisionEnter (collision);

	}

	protected abstract void ProcessCollisionEnter (Collision2D collision);

	public override void HandleCollisionEnterWithTrashObject (TrashObject trashObject)
	{

		if (trashObject.CachedTransform.position.y <= CachedTransform.position.y + 1f) {

			if (GetState () == HumanState.Jumping) {

				EndJump ();

			}

		}

		ProcessCollisionEnterWithTrashObject (trashObject);
	}

	protected abstract void ProcessCollisionEnterWithTrashObject (TrashObject trashObject);

	public override void HandleCollisionExit2D (Collision2D collision)
	{

		ProcessCollisionExit2D (collision);

	}

	protected abstract void ProcessCollisionExit2D (Collision2D collision);

	public override void HandleCollisionExitWithTrashObject (TrashObject trashObject)
	{

		ProcessCollisionExitWithTrashObject (trashObject);

	}

	protected abstract void ProcessCollisionExitWithTrashObject (TrashObject trashObject);

	public override void HandleTriggerEnter2D (Collider2D collider)
	{

		ProcessTriggerEnter2D (collider);

	}

	protected abstract void ProcessTriggerEnter2D (Collider2D collider);

	public override void HandleTriggerEnterWithTrashObject (TrashObject trashObject)
	{

		ProcessTriggerEnterWithTrashObject (trashObject);

	}

	protected abstract void ProcessTriggerEnterWithTrashObject (TrashObject trashObject);

	public override void HandleTriggerExit2D (Collider2D collider)
	{

		ProcessTriggerExit2D (collider);

	}

	protected abstract void ProcessTriggerExit2D (Collider2D collider);

	public override void HandleTriggerExitWithTrashObject (TrashObject trashObject)
	{

		ProcessTriggerExitWithTrashObject (trashObject);

	}

	protected abstract void ProcessTriggerExitWithTrashObject (TrashObject trashObject);

	#endregion

}

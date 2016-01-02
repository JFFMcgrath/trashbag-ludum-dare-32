using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TrashPlayer : TrashHuman {

	public TrashHuman.MovementDirection GetCameraDirection ()
	{

		if (GetState () == HumanState.Idle) {

			//return MovementDirection.None;

		}

		if (currentMovementDirection == MovementDirection.None) {
			return startDirection;
		}

		return currentMovementDirection;

	}

	TrashCamera trashCamera;

	public void SetTrashCamera (TrashCamera trashCamera)
	{
		this.trashCamera = trashCamera;

	}

	Vector2 startPosition;
	MovementDirection startDirection;

	public void SetStartPosition (Vector2 groundLevel, TrashHuman.MovementDirection facingDirection)
	{
		startPosition = groundLevel;
		startDirection = facingDirection;
	}


	#region TrashPlayer State

	public enum TrashPlayerState{
		None,
		Powering_Up,
		Hitting
	}

	public const float ZOOM_IN_VELOCITY = 3f;
	public const float WAIT_TO_ZOOM_TIME = .5f;

	public const string PARAM_TYRE_IRON_ANGLE = "Tyre_Iron_Angle";

	protected StateMachine<TrashPlayerState> playerState;

	static Dictionary<int,TrashPlayerState> anim_HashToPlayerState;
	static List<TrashPlayerState> list_PlayerStates;

	#endregion

	#region Item Management

	TrashItem pickup_Item = null;

	public void SetPickupItem (TrashItem trashItem)
	{

		pickup_Item = trashItem;
	}

	void RemovePickupItem (TrashItem trashItem)
	{
		if (trashItem == pickup_Item) {
			pickup_Item = null;
		}
	}

	public bool CanPickupItem(){

		return pickup_Item != null;

	}

	public void PickupItem(){

		if (CanPickupItem ()) {

			PickupItem (pickup_Item);

		}

	}

	#endregion

	#region Throwing

	TrashTyreIron _tyreIron;
	public TrashTyreIron tyreIron{
		get{

			if (_tyreIron == null) {

				_tyreIron = GetComponentInChildren<TrashTyreIron> ();

			}

			return _tyreIron;

		}
	}

	public Transform bind_TyreIronArm;
	public Transform bind_TrashArm;

	public float throw_Force;
	public ForceMode2D throw_ForceMode;

	public float serveAdjustmentAmount;

	public bool CanThrowCurrentItem(){

		return HasItem ();

	}

	public void ServeItem(){

		if (HasItem ()) {

			SetAnimationTrigger ("Throw");

			this.item_CurrentItem.Adjust (GetDirectionVector () * serveAdjustmentAmount);

			ThrowItem (Vector2.up, throw_Force, throw_ForceMode);

		}

	}

	#endregion

	#region Powering Up

	public const float PERFECT_HIT_RATIO = 0.9f;

	public float max_PowerUpTime;

	public float anim_PowerUpTime;

	public float max_SwingPower;
	public float perfectHitPowerMultiplier;
	public Transform bind_TyreIronLookAt;
	public Transform bind_TyreIronArmLookAt;
	public float timeToSwing;
	public float timeToRelax;
	Vector2 current_PowerUpTarget;
	float current_PowerUpTime;
	float current_Power_N;
	float startTime;
	bool isSwingRelaxing;

	Vector3 initial_tyreIronRotation;
	Vector3 initial_tyreIronArmRotation;

	Vector2 swing_Direction;
	float swing_Power;
	Vector2 swing_Force;
	Vector3 current_TyreIronArmLookAtRotation;
	Vector3 current_TyreIronLookAtRotation;

	Quaternion targetSwingRotation;
	Quaternion targetTyreIronRotation;

	void SetSwingRotation ()
	{
		bind_TyreIronArmLookAt.localRotation = targetSwingRotation;
		bind_TyreIronLookAt.localRotation = targetTyreIronRotation;
	}

	IEnumerator HandleSwingTyreIron(float heightDiff){

		float rotationDirection = -1f;

		isSwingRelaxing = false;

		if (heightDiff < 0f) {
			//rotationDirection = -1f;
		}

		WaitForSeconds wait = new WaitForSeconds (0);
		WaitForFixedUpdate eof = new WaitForFixedUpdate ();

		Vector3 rotation = current_TyreIronArmLookAtRotation;
		Vector3 tyreRotation = current_TyreIronLookAtRotation;

		tyreIron.BeginSwing ();

		targetTyreIronRotation = Quaternion.Euler (tyreRotation);

		float current_z_rotation = rotation.z;

		float target_z_rotation = current_z_rotation + (360f * rotationDirection);

		float currentSwingTime = 0f;

		while (currentSwingTime <= timeToSwing) {

			currentSwingTime += Time.fixedDeltaTime;

			float n = currentSwingTime / timeToSwing;

			if (n >= 0.8f) {
				isSwingRelaxing = true;
				tyreIron.EndSwing ();
					
			}

			float z = Mathf.Lerp (current_z_rotation, target_z_rotation, n);

			rotation.z = z;

			targetSwingRotation = Quaternion.Euler (rotation);

			yield return eof;

		}

		isSwingRelaxing = true;

		float currentRelaxTime = 0f;

		float z_tyre = bind_TyreIronLookAt.localRotation.eulerAngles.z;

		while (currentRelaxTime <= timeToRelax) {

			currentRelaxTime += Time.deltaTime;

			float n = currentRelaxTime / timeToRelax;

			tyreIron.UpdatePower (1f-(n*current_Power_N));

			float z = Mathf.LerpAngle (current_z_rotation, initial_tyreIronArmRotation.z, n);
			float zt = Mathf.LerpAngle (z_tyre, initial_tyreIronRotation.z, n);

			rotation.z = z;
			tyreRotation.z = zt;

			targetSwingRotation = Quaternion.Euler (rotation);
			targetTyreIronRotation = Quaternion.Euler (tyreRotation);

			yield return wait;

		}

		isSwingRelaxing = false;

		trashCamera.SetCameraTargetZoom (0f, this);

		playerState.SetState (TrashPlayerState.None);

		tyreIron.RevertColor ();

	}

	Vector2 initial_TyreIronUp;
	Vector2 initial_TyreIronArmUp;

	public void BeginPowerUp(){

		current_PowerUpTime = 0f;
		current_Power_N = 0f;
		startTime = Time.time;

		initial_TyreIronArmUp = bind_TyreIronArmLookAt.up;
		initial_TyreIronUp = bind_TyreIronLookAt.up;

		playerState.SetState (TrashPlayerState.Powering_Up);

	}

	void SetPowerUpTarget(Vector2 screenPosition){

		Vector2 playerScreenPosition = Camera.main.WorldToScreenPoint (bind_TyreIronLookAt.position);

		Vector2 direction = screenPosition - playerScreenPosition;

		current_PowerUpTarget = (Vector2)bind_TyreIronLookAt.position + direction * 3f;

	}

	void UpdatePowerUp(){

		Vector2 inputPos = Input.mousePosition;

		SetPowerUpTarget (inputPos);

		Vector2 up = (Vector2)bind_TyreIronLookAt.position - (Vector2)current_PowerUpTarget;

		if (IsFacingDirection (up)) {

		} else {
	
			//return;

		}

		up = ModifyUpVectorByDirection (up);

		up = up.normalized;

		current_PowerUpTime += Time.deltaTime;
		current_Power_N = current_PowerUpTime / max_PowerUpTime;

		current_Power_N = Mathf.Clamp (current_Power_N, 0f, 1f);

		float anim_N = current_Power_N / anim_PowerUpTime;

		anim_N = Mathf.Clamp (anim_N, 0f, 1f);

		trashCamera.SetCameraTargetZoom (current_Power_N,this);

		bind_TyreIronArmLookAt.up = Vector2.Lerp(initial_TyreIronArmUp, up * -1f,anim_N);
		bind_TyreIronLookAt.up = Vector2.Lerp(initial_TyreIronUp, up,anim_N);

		current_TyreIronArmLookAtRotation = bind_TyreIronArmLookAt.localRotation.eulerAngles;
		current_TyreIronLookAtRotation = bind_TyreIronLookAt.localRotation.eulerAngles;

		tyreIron.UpdatePower (current_Power_N);

		TrashUIManager.Instance.SetPowerMeter (current_Power_N);

	}

	void CancelPowerUp(){

		tyreIron.RevertColor ();
		playerState.SetState (TrashPlayerState.None);

		trashCamera.SetCameraTargetZoom (0f,this);

		TrashUIManager.Instance.HidePowerMeter ();

	}

	public float enemyLateralHitPower;
	public float enemyVerticalHitPower;

	void EndPowerUp(){

		TrashUIManager.Instance.HidePowerMeter ();

		if (playerState.GetState () != TrashPlayerState.Powering_Up) {
			return;
		}

		if (current_Power_N < (anim_PowerUpTime / max_PowerUpTime)) {

			CancelPowerUp ();

			return;

		}

		TrashEnemy[] e = CanSwingTyreIron ();

		if (e == null) {

			SwingTyreIron ();

		} else {

			HandleCannotSwingTyreIron ();

			for (int i = 0; i < e.Length; i++) {

				e[i].HandleHitByPlayer (GetDirectionVector () * enemyLateralHitPower + Vector2.up * enemyVerticalHitPower);

			}

			SwingTyreIron ();

		}

	}

	void SwingTyreIron(){

		swing_Direction = (current_PowerUpTarget - (Vector2)bind_TyreIronLookAt.position);

		float heightDiff = current_PowerUpTarget.y - bind_TyreIronLookAt.position.y;

		CachedAnimator.SetFloat (PARAM_TYRE_IRON_ANGLE, heightDiff);

		playerState.SetState (TrashPlayerState.Hitting);

		swing_Direction = ((Vector2)current_PowerUpTarget - (Vector2)bind_TyreIronLookAt.position).normalized;
		swing_Power = max_SwingPower * current_Power_N;

		swing_Force = swing_Direction * swing_Power;

		StartCoroutine (HandleSwingTyreIron (heightDiff));

	}

	public const float PERFECT_HIT_VALUE = 0.8f;

	public Color color_HitEffect_ItemHit;

	void HandleTyreIronHitEnemy (TrashEnemy enemy)
	{

		if (playerState.GetState () == TrashPlayerState.Hitting && !isSwingRelaxing) {

			Vector2 hitDirection = ((Vector2)enemy.CachedTransform.position - (Vector2)bind_TyreIronLookAt.position).normalized;

			float dot = Vector2.Dot (hitDirection, swing_Direction);

			float hitValue = 0f;

			dot = Mathf.Clamp (dot, 0f, PERFECT_HIT_RATIO);

			dot = dot / PERFECT_HIT_RATIO;

			hitValue = Mathf.Min (dot, current_Power_N);

			if (hitValue > PERFECT_HIT_VALUE) {

				hitValue = 1f;

				swing_Force = swing_Direction * (swing_Power * perfectHitPowerMultiplier);

			} else {

				swing_Force = hitDirection * (swing_Power);

			}


			enemy.HandleHit (hitValue,swing_Force);


		}

	}

	void HandleTyreIronHitItem (TrashItem item)
	{

		if (HasItem () && item_CurrentItem == item) {

			return;

		}

		if (playerState.GetState () == TrashPlayerState.Hitting && !isSwingRelaxing) {

			Vector2 hitDirection = ((Vector2)item.CachedTransform.position - (Vector2)bind_TyreIronLookAt.position).normalized;

			float dot = Vector2.Dot (hitDirection, swing_Direction);

			float hitValue = 0f;

			dot = Mathf.Clamp (dot, 0f, PERFECT_HIT_RATIO);

			dot = dot / PERFECT_HIT_RATIO;

			hitValue = Mathf.Min (dot, current_Power_N);

			if (hitValue > PERFECT_HIT_VALUE) {

				hitValue = 1f;

				swing_Force = swing_Direction * (swing_Power * perfectHitPowerMultiplier);

			} else {

				swing_Force = hitDirection * (swing_Power);

			}


			item.HandleHit (hitValue, color_HitEffect_ItemHit,swing_Force);

		}

	}

	public Transform announcementBind;

	void HandleCannotSwingTyreIron(){

		TrashAnnouncement a = TrashUIManager.Instance.MakeAnnouncement (announcementBind, AnnouncementType.Cannot_Hit, 0f);
		a.AutoDestroy ();

	}

	public float enemyCheckDistance = 2f;

	public LayerMask enemyLayer;

	Vector2 f,t;

	public TrashEnemy[] CanSwingTyreIron(){

		//Check in front of you

		Vector2 vDirection = GetDirectionVector ();

		Vector2 root = (Vector2)CachedTransform.position + Vector2.up;

		f = root - vDirection * enemyCheckDistance;;
		t = root + vDirection * enemyCheckDistance;

		RaycastHit2D[] rayCast = Physics2D.CircleCastAll (bind_TyreIronArmLookAt.position, enemyCheckDistance, Vector2.zero);

		//RaycastHit2D[] rayCast = Physics2D.RaycastAll (root, vDirection,enemyCheckDistance * 2f,enemyLayer.value);

		List<TrashEnemy> allEnemies = new List<TrashEnemy> ();

		for(int i = 0; i < rayCast.Length; i++){

			TrashObject o = GetTrashObjectFromCollider (rayCast[i].collider);

			if (o is TrashEnemy) {

				allEnemies.Add(o as TrashEnemy);

			}

		}

		if (allEnemies.Count == 0) {
			return null;
		}

		return allEnemies.ToArray ();

	}

	public bool CanBeginPowerUp(){

		return !IsDead () && playerState.GetState() == TrashPlayerState.None;

	}

	#endregion

	#region Trash Human functions

	public MovementDirection initialDirection;

	protected override void OnInitialize ()
	{
		playerState = new StateMachine<TrashPlayerState> (BeginPlayerState, EndPlayerState);

		currentMovementDirection = initialDirection;

		ChangeDirection (startDirection);

		CachedTransform.position = startPosition;

		if (trashCamera == null) {

			trashCamera = GameObject.FindObjectOfType<TrashCamera> ();

		}

		trashCamera.SnapToPlayerPosition ();

		initial_tyreIronRotation = bind_TyreIronLookAt.localRotation.eulerAngles;
		initial_tyreIronArmRotation = bind_TyreIronArmLookAt.localRotation.eulerAngles;

		this.tyreIron.SubscribeToTyreIronCollision (HandleTyreIronHitItem, HandleTyreIronHitEnemy);

	}

	protected override void OnChangeDirection (MovementDirection movementDirection)
	{

		if (playerState.GetState () == TrashPlayerState.Powering_Up) {

			//CancelPowerUp ();

		}

	}

	void BeginPlayerState (TrashPlayerState state)
	{

		switch (state) {
			case TrashPlayerState.None:
			{
				break;
			}
			case TrashPlayerState.Powering_Up:
			{
				SetAnimationState (state.ToString (), true, false);
				break;
			}
			case TrashPlayerState.Hitting:
			{
				SetAnimationTrigger (state.ToString());
				break;
			}
		}
	}

	void EndPlayerState (TrashPlayerState state)
	{
		switch (state) {
			case TrashPlayerState.None:
			{
				break;
			}
			case TrashPlayerState.Powering_Up:
			{
				SetAnimationState (state.ToString (), false, false);
				break;
			}
			case TrashPlayerState.Hitting:
			{
				break;
			}
		}
	}

	protected override bool HandleCanJump ()
	{
		return true;
	}
	protected override bool HandleCanMove ()
	{
		return true;
	}

	protected override void HandleAnimStateBegins (HumanState state)
	{
	}

	protected override void HandleAnimStateEnds (HumanState state)
	{
	}

	protected override void HandleTrashAnimStateBegins (int stateHash)
	{

		TrashPlayerState state = GetPlayerStateForHash (stateHash);

		if (state != TrashPlayerState.None) {

		}

	}

	protected override void HandleTrashAnimStateEnds (int stateHash)
	{

		TrashPlayerState state = GetPlayerStateForHash (stateHash);

		if (state != TrashPlayerState.None) {

			if (state == TrashPlayerState.Hitting) {

				//playerState.SetState (TrashPlayerState.None);

			}

		}

	}

	public TrashPlayerState GetPlayerStateForHash(int hash){

		if (anim_HashToPlayerState.ContainsKey (hash)) {

			return anim_HashToPlayerState [hash];

		}

		return TrashPlayerState.None;

	}

	protected override void OnInitializeNameHashes ()
	{

		anim_HashToPlayerState = new Dictionary<int, TrashPlayerState> ();

		list_PlayerStates = new List<TrashPlayerState> ();

		string[] names = Enum.GetNames (typeof(TrashPlayerState));

		for (int i = 0; i < names.Length; i++) {

			int hash = Animator.StringToHash (names [i]);

			anim_HashToPlayerState.Add (hash, (TrashPlayerState)i);

			list_PlayerStates.Add ((TrashPlayerState)i);

		}


	}

	#endregion
	#region implemented abstract members of TrashHuman

	protected override void ProcessCollisionEnter (Collision2D collision)
	{

	}
	protected override void ProcessCollisionEnterWithTrashObject (TrashObject trashObject)
	{
		
	}
	protected override void ProcessCollisionExit2D (Collision2D collision)
	{
		
	}
	protected override void ProcessCollisionExitWithTrashObject (TrashObject trashObject)
	{
		
	}
	protected override void ProcessTriggerEnter2D (Collider2D collider)
	{
		
	}

	protected override void ProcessTriggerEnterWithTrashObject (TrashObject trashObject)
	{
		if (trashObject is TrashItem) {

			TrashItem item = trashObject as TrashItem;

			if (!item.isBoundToHuman) {

				SetPickupItem (item);

			}
		}
	}
	protected override void ProcessTriggerExit2D (Collider2D collider)
	{
	}

	protected override void ProcessTriggerExitWithTrashObject (TrashObject trashObject)
	{
		if (trashObject is TrashItem) {
			RemovePickupItem (trashObject as TrashItem);
		}
	}
	#endregion


	#region Trash Input Functions

	void UpdatePlayerStates(){

		switch (playerState.GetState()) {
			case TrashPlayerState.None:
			{
				break;
			}
			case TrashPlayerState.Powering_Up:
			{

				break;
			}
			case TrashPlayerState.Hitting:
			{
				break;
			}
		}

	}

	public override void HandleFixedUpdate ()
	{
		switch (playerState.GetState()) {
		case TrashPlayerState.None:
			{
				break;
			}
		case TrashPlayerState.Powering_Up:
			{

				break;
			}
		case TrashPlayerState.Hitting:
			{

				SetSwingRotation ();

				break;
			}
		}
	}

	void LateUpdate(){



		switch (playerState.GetState()) {
		case TrashPlayerState.None:
			{
				break;
			}
		case TrashPlayerState.Powering_Up:
			{

				UpdatePowerUp ();

				break;
			}
		case TrashPlayerState.Hitting:
			{

				SetSwingRotation ();

				break;
			}
		}

	}

	void UpdatePlayerInput(){

		if (Input.GetMouseButtonDown (0)) {

			BeginPowerUp ();

		}

		if (Input.GetMouseButtonUp (0)) {

			EndPowerUp ();

		}

		if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.A)) {

			if (!IsMovingInDirection (MovementDirection.Left, HumanState.Walking)) {

				StartMovement (MovementDirection.Left, HumanState.Walking);

			}

		} 

		if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.D)) {

			if (!IsMovingInDirection (MovementDirection.Right, HumanState.Walking)) {

				StartMovement (MovementDirection.Right, HumanState.Walking);

			}

		}

		if (Input.GetKeyUp (KeyCode.LeftArrow) || Input.GetKeyUp (KeyCode.A)) {

			if (IsMovingInDirection (MovementDirection.Left, HumanState.Walking)) {

				EndMovement ();

			} else if (IsJumping ()) {

				EndJumpMovement ();

			}

		}


		if (Input.GetKeyUp (KeyCode.RightArrow) || Input.GetKeyUp (KeyCode.D)) {

			if (IsMovingInDirection (MovementDirection.Right, HumanState.Walking)) {

				EndMovement ();

			}
			else if (IsJumping ()) {

				EndJumpMovement ();

			}

		}

		if (Input.GetKeyDown (KeyCode.Space)) {

			Jump ();

		}

		if (Input.GetMouseButtonDown (1)) {

			if (!HasItem ()) {

				if (CanPickupItem ()) {

					PickupItem ();

				}

			} else {

				ServeItem ();

			}

		}

	}

	public override void HandleUpdate ()
	{


		if (!TrashUIManager.Instance.hasShownTutorial) {

			UpdateTutorial ();

		}

		UpdatePlayerStates ();
		UpdatePlayerInput ();


	}

	float currentTutorialTime;
	int currentTutorialIndex;

	void UpdateTutorial(){

		float tutorialInterval = 8.5f;

		currentTutorialTime += Time.deltaTime;

		if (currentTutorialTime >= tutorialInterval) {

			currentTutorialTime = 0f;

			TrashUIManager.Instance.ShowTutorialText (currentTutorialIndex);

			currentTutorialIndex++;

			if (currentTutorialIndex >= TrashUIManager.Instance.tutorialLines.Length) {

				TrashUIManager.Instance.hasShownTutorial = true;
				PlayerPrefs.SetInt ("TUT", 1);

			}

		}

	}

	#endregion
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrashEnemy : TrashHuman {

	public float killVelocity;
	public float killVelocityMultiplier;
	public Vector2 killTorqueRange;

	public float deathAngularDrag;
	public float deathLinearDrag;

	public float killUpwardVelocity;

	public float checkDistance = 4f;

	public Collider2D[] livingColliders;

	public Rigidbody2D coreDeathRigidbody;

	Collider2D[] deathColliders;
	Rigidbody2D[] deathRigidbodies;
	Joint2D[] deathJoints;

	public Color[] tintColors;
	bool hasSetColor = false;
	Color _tintColor;

	public Color TintColor{
		get{
			if (!hasSetColor) {
				hasSetColor = true;

				int index = UnityEngine.Random.Range (0,tintColors.Length);

				_tintColor = tintColors [index];

			}
			return _tintColor;
		}
	}

	public const float chanceToSuicide = .2f;

	public float chanceToIdle = 0.5f;

	public void HandleHitByItem (TrashItem trashItem)
	{

		Vector2 velocity = trashItem.CachedRigidbody2D.velocity;

		if (velocity.magnitude >= killVelocity) {

			velocity *= killVelocityMultiplier;

			velocity += Vector2.up * killUpwardVelocity;

			SetState (HumanState.Dying);

			CachedRigidbody2D.AddTorque (UnityEngine.Random.Range(killTorqueRange.x,killTorqueRange.y),ForceMode2D.Impulse);
		}

		CachedRigidbody2D.AddForce (velocity, ForceMode2D.Impulse);

	}

	public void TemporarilyEnableRagdoll(){

		StopAllCoroutines ();

		CachedRigidbody2D.velocity = Vector2.zero;

		EnableDeathColliders ();

		StartCoroutine (WaitToFreeze ());

	}

	public void DisableDeathRigidbodies(){

		DisableJoints ();

		for (int i = 0; i < deathRigidbodies.Length; i++) {

			deathRigidbodies [i].isKinematic = true;

		}

	}

	public void EnableDeathRigidbodies(){

		EnableJoints ();

		for (int i = 0; i < deathRigidbodies.Length; i++) {

			deathRigidbodies [i].isKinematic = false;

		}

	}

	public void HandleHit (float hitValue, Vector2 swing_Force)
	{

		if (IsDead ()) {

			TemporarilyEnableRagdoll ();

			CachedRigidbody2D.AddForce (swing_Force, ForceMode2D.Impulse);

			StartCoroutine (WaitForZoomIn ());

		}
	}

	public void HandleHitByPlayer(Vector2 force){

		CachedRigidbody2D.AddForce (force, ForceMode2D.Impulse);
		SetGrounded (false);

	}

	IEnumerator WaitForZoomIn(){

		TrashCamera camera = GameObject.FindObjectOfType<TrashCamera> ();

		camera.LockZoom (this);

		WaitForSeconds wait = new WaitForSeconds (0);

		while (CachedRigidbody2D.velocity.magnitude > TrashPlayer.ZOOM_IN_VELOCITY) {
			yield return wait;
		}

		yield return new WaitForSeconds (TrashPlayer.WAIT_TO_ZOOM_TIME);

		camera.UnlockZoom (this);

		camera.SetCameraTargetZoom (0f,this);

	}

	protected override void OnInitializeTrashObject ()
	{

		deathJoints = GetComponentsInChildren<Joint2D> ();
		DisableJoints ();

		Collider2D[] colliders = GetComponentsInChildren<Collider2D> ();
		Rigidbody2D[] rigidBodies = GetComponentsInChildren<Rigidbody2D> ();

		List<Collider2D> deathColliders = new List<Collider2D>();

		for (int i = 0; i < colliders.Length; i++) {

			if (IsLivingCollider (colliders [i])) {
				continue;
			}

			deathColliders.Add (colliders [i]);

		}

		this.deathColliders = deathColliders.ToArray ();

		List<Rigidbody2D> deathRigidbodies = new List<Rigidbody2D>();

		for (int i = 0; i < rigidBodies.Length; i++) {

			if (rigidBodies[i] == CachedRigidbody2D) {
				continue;
			}

			deathRigidbodies.Add (rigidBodies [i]);

			rigidBodies [i].angularDrag = deathAngularDrag;
			rigidBodies [i].drag = deathLinearDrag;

		}

		this.deathRigidbodies = deathRigidbodies.ToArray ();

		SetColliders (livingColliders, null, true);
		SetColliders (this.deathColliders, this.deathRigidbodies, false);

		base.OnInitializeTrashObject ();

		MovementDirection dir;

		if (UnityEngine.Random.Range (0, 2) == 1) {
			dir = MovementDirection.Left;
		} else {
			dir = MovementDirection.Right;
		}

		ChangeDirection (dir);

		BeginIdle ();

		SpriteRenderer[] sr = gameObject.GetComponentsInChildren<SpriteRenderer> ();

		for (int i = 0; i < sr.Length; i++) {

			sr [i].color = TintColor;

		}

	}

	bool IsLivingCollider(Collider2D collider){

		for (int i = 0; i < livingColliders.Length; i++) {

			if (collider == livingColliders [i]) {
				return true;
			}

		}

		return false;

	}

	void DisableJoints(){

		for (int i = 0; i < deathJoints.Length; i++) {

			deathJoints [i].enabled = false;

		}

	}

	void EnableJoints(){

		for (int i = 0; i < deathJoints.Length; i++) {

			deathJoints [i].enabled = true;

		}

	}

	void EnableDeathColliders(){

		CachedAnimator.enabled = false;

		//CachedRigidbody2D.isKinematic = true;

		CachedRigidbody2D.AddForce (Vector2.up * 10f);

		SetColliders (livingColliders, null, false);
		SetColliders (deathColliders, deathRigidbodies, true);

		for (int i = 0; i < deathJoints.Length; i++) {

			deathJoints [i].enabled = true;
			deathJoints [i].gameObject.GetComponent<Rigidbody2D> ().isKinematic = true;

		}

	}

	void SetColliders(Collider2D[] colliders, Rigidbody2D[] rigidBodies, bool enabled){

		if (colliders != null) {

			for (int i = 0; i < colliders.Length; i++) {

				colliders [i].enabled = enabled;

			}

		}

		if (rigidBodies != null) {

			for (int i = 0; i < rigidBodies.Length; i++) {

				rigidBodies [i].isKinematic = !enabled;

			}

		}

	}

	public Transform announcementBind;
	public HitEffect deathEffect;
	public Color deathColor;

	protected override void OnBeginHumanState (HumanState state)
	{

		if (state == HumanState.Dying) {

			TrashWorld.Instance.DecrementWasters ();

			ThrowItem ();

			StopAllCoroutines ();

			CachedAnimator.enabled = false;
			CachedRigidbody2D.fixedAngle = false;

			HitEffect h = GameObject.Instantiate (deathEffect) as HitEffect;

			h.SetEffect(CachedTransform.position,0f,deathColor,AnnouncementType.Kill);

			//TrashUIManager.Instance.MakeAnnouncement (announcementBind, AnnouncementType.Kill, 0f);

			TemporarilyEnableRagdoll ();

		} else if (state == HumanState.Idle) {

			BeginIdle ();

		}

	}

	public const float TIME_TO_FREEZE = 5f;

	IEnumerator WaitToFreeze(){

		yield return new WaitForSeconds(TIME_TO_FREEZE);

		DisableDeathRigidbodies ();

	}

	public Vector2 idleTimeRange;

	void BeginIdle ()
	{

		if (GetState () != HumanState.Idle) {
			SetState (HumanState.Idle);
		}

		StartCoroutine (HandleIdle ());

	}

	IEnumerator HandleIdle(){

		yield return new WaitForSeconds(UnityEngine.Random.Range(idleTimeRange.x,idleTimeRange.y));

		MakeMovementDecision ();


	}

	bool canPickupItem = true;

	public override void HandleUpdate ()
	{

		if (!IsDead () && !HasItem ()) {

			if (TrashWorld.Instance.CanGenerateTrash () && canPickupItem) {

				TrashItem item = TrashWorld.Instance.GetNewTrashItem ();

				PickupItem (item);

				StartDropTimer ();

			}

		}

		if (GetState () == HumanState.Walking) {

			MakeMovementDecision ();

		}

	}

	public Vector2 trashDropRate;

	void StartDropTimer (){

		StartCoroutine(HandleDropTimer(UnityEngine.Random.Range(trashDropRate.x,trashDropRate.y)));

	}

	public const float PICKUP_ITEM_COOLDOWN_TIME = 5f;

	IEnumerator HandleDropTimer(float time){

		canPickupItem = false;

		yield return new WaitForSeconds (time);	

		TrashUIManager.Instance.MakeAnnouncement (announcementBind, AnnouncementType.Trash_Created, 0f);

		ThrowItem ();

		yield return new WaitForSeconds (PICKUP_ITEM_COOLDOWN_TIME);	

		canPickupItem = true;
					
	}

	void MakeMovementDecision(){


		if (!CanMoveInDirection (currentMovementDirection)) {

			float v = UnityEngine.Random.value;

			if (wouldFall) {
				if (v <= chanceToSuicide) {
					Jump ();
					return;
				}
			}

			if (v <= chanceToIdle) {

				BeginIdle ();
				return;

			}

			MovementDirection d = SwitchDirections ();

			if (!CanMoveInDirection (d)) {

				if (wouldFall) {
					if (v <= chanceToSuicide) {
						Jump ();
						return;
					}
				}

				BeginIdle ();

			} else {

				StartMovement (d, HumanState.Walking);

			}

		} else if(!IsMoving()){

			StartMovement (currentMovementDirection, HumanState.Walking);

		}

	}

	MovementDirection SwitchDirections(){


		MovementDirection d = currentMovementDirection;

		if (d == MovementDirection.Left) {

			d = MovementDirection.Right;	

		} else if (d == MovementDirection.Right) {

			d = MovementDirection.Left;

		} else {

			if (UnityEngine.Random.Range (0, 2) == 0) {
				d = MovementDirection.Left;
			} else {
				d = MovementDirection.Right;
			}

		}

		return d;

	}

	public LayerMask canMoveLayer;
	bool wouldFall = false;

	public bool CanMoveInDirection(MovementDirection direction){

		if (!IsGrounded) {
			return false;
		}

		wouldFall = false;

		Vector2 vDirection = GetDirectionVectorFor(direction);

		Vector2 root = (Vector2)CachedTransform.position + Vector2.up + vDirection * 0.5f;

		RaycastHit2D rayCast = Physics2D.Raycast (root, vDirection,checkDistance,canMoveLayer);

		if (rayCast.collider != null) {

			TrashObject o = GetTrashObjectFromCollider (rayCast.collider);

			if (o != null) {

				return false;

			}

		}


		Vector2 position = root + vDirection * 0.5f;

		rayCast = Physics2D.Raycast (position, Vector2.up*-1f,checkDistance*2f);

		if (rayCast.collider == null) {

			wouldFall = true;

			return false;

		}

		return true;

		//

	}

	#region implemented abstract members of TrashHuman

	protected override bool HandleCanJump ()
	{
		return false;
	}

	protected override bool HandleCanMove ()
	{
		return true;
	}

	protected override void HandleTrashAnimStateBegins (int stateHash)
	{

	}

	protected override void HandleAnimStateBegins (HumanState state)
	{

	}

	protected override void HandleTrashAnimStateEnds (int stateHash)
	{

	}

	protected override void HandleAnimStateEnds (HumanState state)
	{

	}

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

	}

	protected override void ProcessTriggerExit2D (Collider2D collider)
	{

	}

	protected override void ProcessTriggerExitWithTrashObject (TrashObject trashObject)
	{

	}

	#endregion

}

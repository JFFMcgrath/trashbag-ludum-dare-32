using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrashItem : TrashCollidingObject {

	public const int BOUNCES_UNTIL_DESTROYED = 20;
	public const int TYRE_IRON_HIT_DAMAGE = 5;

	public int currentBounceHealth;

	public void Adjust (Vector2 adjustment)
	{
		CachedTransform.position += (Vector3)(adjustment * ADJUSTMENT_AMOUNT);
	}

	public bool isBoundToHuman = false;
	Vector2 lastPosition_a;
	Vector2 lastPosition_b;

	WaitForSeconds eof = new WaitForSeconds(0);

	public const float RANDOM_TORQUE_AMOUNT = 10f;
	public const float ADJUSTMENT_AMOUNT = 0.25f;

	public float debug_Velocity;

	public SwishEffect prefab_swishEffect;
	SwishEffect current_swishEffect;

	public float swishStartWidth;
	public float swishEndWidth;

	public HitEffect[] hitEffects;
	public HitEffect destroyEffect;

	public Color destroyColor;

	float lastHit = 0f;

	bool destroyed = false;

	void DestroyTrashItem ()
	{

		TrashCamera cam = GameObject.FindObjectOfType<TrashCamera> ();

		cam.UnlockZoom (this);
		cam.SetCameraTargetZoom (0f, this);

		if (destroyed) {
			return;
		}

		destroyed = true;

		//TrashUIManager.Instance.MakeAnnouncement (CachedTransform, AnnouncementType.Item_Destroyed, 0f);

		CreateDestroyEffect ();

		TrashWorld.Instance.DestroyTrashItem ();


		GameObject.Destroy (this.gameObject);


	}

	bool DecreaseBounceHealth(int amount){

		currentBounceHealth += amount;

		if (currentBounceHealth >= BOUNCES_UNTIL_DESTROYED) {

			DestroyTrashItem ();

			return true;

		}

		return false;
	}

	public HitEffect GetHitEffectForScore(float n){

		int index = (int)(n * (float)hitEffects.Length);

		index = Mathf.Clamp (index, 0, hitEffects.Length - 1);

		HitEffect o = GameObject.Instantiate (hitEffects [index]) as HitEffect;

		return o;

	}

	public void CreateDestroyEffect(){

		HitEffect h = GameObject.Instantiate (destroyEffect) as HitEffect;

		h.SetEffect(CachedTransform.position,0f,destroyColor,AnnouncementType.Item_Destroyed);

	}

	public void CreateHitEffect(float n,Color color){

		HitEffect h = GetHitEffectForScore (n);

		h.SetEffect (CachedTransform.position,n,color,AnnouncementType.Hit);

	}

	public void HandleHit(float n, Color color, Vector2 force){

		Unbind ();

		if (Time.time - lastHit <= 1f) {

			return;

		}

		if (DecreaseBounceHealth (TYRE_IRON_HIT_DAMAGE)) {

			return;

		} else {

			if (Time.time - lastHumanHandling <= 5f) {

				CreateHitEffect (n, color);

				BeginTrail ();

				CachedRigidbody2D.AddForce (force, ForceMode2D.Impulse);

				StartCoroutine (WaitForZoomIn ());

			} else {

				DestroyTrashItem ();
				return;

			}

			lastHit = Time.time;

		}


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

	int bounceCount;

	public void BeginTrail(){

		CachedRigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

		bounceCount = 0;

		if (current_swishEffect != null) {
			StopAllCoroutines ();
			current_swishEffect.EndSwishEffect ();
			current_swishEffect = null;
		}

		current_swishEffect = GameObject.Instantiate (prefab_swishEffect) as SwishEffect;

		current_swishEffect.CachedTransform.SetParent (CachedTransform);
		current_swishEffect.CachedTransform.localPosition = Vector3.zero;

		current_swishEffect.CachedLineRenderer.SetWidth (swishStartWidth, swishEndWidth);

		current_swishEffect.BeginSwishEffect ();

		StartCoroutine (WaitToKillSwish());


	}

	public float minSwishVelocity;

	IEnumerator WaitToKillSwish(){

		while (CachedRigidbody2D.velocity.magnitude > minSwishVelocity) {
			yield return new WaitForSeconds (0);
		}

		EndTrail ();

	}

	public void EndTrail(){

		CachedRigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Discrete;

		if (current_swishEffect != null) {

			current_swishEffect.EndSwishEffect ();
			current_swishEffect = null;
		}

	}

	public void HandlePickup(){
		Bind ();
	}

	void Bind(){

		isBoundToHuman = true;

		CachedRigidbody2D.isKinematic = true;
		CachedCollider2D.enabled = false;

	}

	float lastHumanHandling = 0f;

	void Unbind(){

		isBoundToHuman = false;

		CachedRigidbody2D.isKinematic = false;
		CachedCollider2D.enabled = true;

		Vector3 position = CachedTransform.position;

		CachedTransform.SetParent (null,false);

		CachedTransform.position = position;

	}

	public void HandleDrop(){

		lastHumanHandling = Time.time;

		Unbind ();

		CachedRigidbody2D.velocity = GetItemVelocity ();

		AddRandomTorque ();

	}

	public void HandleThrow(){

		lastHumanHandling = Time.time;

		Unbind ();

		CachedRigidbody2D.velocity = GetItemVelocity();

		AddRandomTorque ();

	}

	public void HandleThrow(Vector2 velocity){

		lastHumanHandling = Time.time;

		Unbind ();

		CachedRigidbody2D.velocity = velocity;

		AddRandomTorque ();

	}

	public void HandleThrow(Vector2 direction, float force, ForceMode2D forceMode){

		lastHumanHandling = Time.time;

		Unbind ();

		CachedRigidbody2D.AddForce (direction * force, forceMode);

		AddRandomTorque ();

		//StartCoroutine (ProcessHandleThrow (direction,force,forceMode));

	}

	public void AddRandomTorque(){

		CachedRigidbody2D.AddTorque (UnityEngine.Random.Range (-RANDOM_TORQUE_AMOUNT, RANDOM_TORQUE_AMOUNT));

	}

	void FixedUpdate(){

		if (isBoundToHuman) {

			TrackLastPosition ();

		}

	}

	Vector2 GetItemVelocity(){

		return lastPosition_a - lastPosition_b;

	}

	void TrackLastPosition(){

		lastPosition_b = lastPosition_a;
		lastPosition_a = CachedTransform.position;

		debug_Velocity = GetItemVelocity ().magnitude;

	}

	#region implemented abstract members of TrashObject

	protected override void OnInitializeTrashObject (){

		CachedCollider2D.ToString ();

		lastPosition_a = CachedTransform.position;
		lastPosition_b = CachedTransform.position;

	}

	#endregion

	#region implemented abstract members of TrashCollidingObject

	public override void HandleCollisionEnter (Collision2D collision)
	{

		bounceCount++;

		AddRandomTorque ();

		if (current_swishEffect != null && bounceCount > 5) {
			bounceCount = 0;
			EndTrail ();
		}



	}

	public override void HandleCollisionEnterWithTrashObject (TrashObject trashObject)
	{

		if (trashObject is TrashEnemy) {

			TrashEnemy enemy = trashObject as TrashEnemy;

			enemy.HandleHitByItem (this);

			CachedRigidbody2D.velocity *= -0.5f;

		}

	}

	public override void HandleCollisionExit2D (Collision2D collision)
	{
		
	}

	public override void HandleCollisionExitWithTrashObject (TrashObject trashObject)
	{
		
	}

	public override void HandleTriggerEnter2D (Collider2D collider)
	{
		
	}

	public override void HandleTriggerEnterWithTrashObject (TrashObject trashObject)
	{

		if (!isBoundToHuman) {

			if (trashObject is TrashPlayer) {

				TrashPlayer player = trashObject as TrashPlayer;

				player.SetPickupItem (this);

			}

		}
	}

	public override void HandleTriggerExit2D (Collider2D collider)
	{
		
	}

	public override void HandleTriggerExitWithTrashObject (TrashObject trashObject)
	{
	}

	#endregion




}

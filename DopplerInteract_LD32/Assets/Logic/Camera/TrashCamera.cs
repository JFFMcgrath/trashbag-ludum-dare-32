using UnityEngine;
using System.Collections;

public class TrashCamera : EfficientBehaviour {

	int lockingID = NONE;
	const int NONE = -1;

	Transform lockZoomTarget;

	public Vector2 player_Anchor_Position;

	public bool IsLocked(){

		return lockingID != NONE;

	}

	public void LockZoom (TrashObject trashItem)
	{
		lockingID = trashItem.GetInstanceID ();
		lockZoomTarget = trashItem.CachedTransform;
	}

	public void UnlockZoom (TrashObject trashItem)
	{
		if (lockingID == trashItem.GetInstanceID ()) {

			lockingID = NONE;
			lockZoomTarget = null;

		}

		FocusOnPlayer ();

	}

	void FocusOnPlayer(){

		targetCameraFocus = CalculatePlayerFocus ();

	}

	void OnDrawGizmos(){

		Gizmos.color = Color.cyan;

		Gizmos.DrawWireSphere (targetCameraFocus, 0.5f);

	}

	float GetNCameraXPos (float x)
	{

		float half_height = trashCamera.orthographicSize;
		float half_width = half_height * Screen.width / Screen.height;

		float height = half_height * 2f;
		float width = half_width * 2f;

		float x_offset = x * width;

		x_offset -= half_width;

		return x + x_offset;

	}

	float GetNCameraYPos (float y)
	{

		float half_height = trashCamera.orthographicSize;
		float half_width = half_height * Screen.width / Screen.height;

		float height = half_height * 2f;
		float width = half_width * 2f;

		float y_offset = y * height;

		y_offset -= half_height;

		return y + y_offset;

	}

	Vector3 CalculatePlayerFocus(){

		TrashPlayer.MovementDirection dir = player.GetCameraDirection ();

		float x = player.CachedTransform.position.x;

		if (dir == TrashHuman.MovementDirection.Left) {

			x = player.CachedTransform.position.x - GetNCameraXPos (1f - player_Anchor_Position.x);

		} else if (dir == TrashHuman.MovementDirection.Right) {

			x = player.CachedTransform.position.x + GetNCameraXPos (1f - player_Anchor_Position.x);

		}

		float y = player.CachedTransform.position.y - GetNCameraYPos (player_Anchor_Position.y);

		return new Vector3 (x, y, player.CachedTransform.position.z);

	}

	TrashPlayer player;
	TrashWorld world;

	public float minZoom;
	public float maxZoom;

	public Camera trashCamera;

	public Vector3 targetCameraFocus;
	public Vector3 currentCameraFocus;

	public float targetZoom;
	public float debug_Current_N_Zoom;

	public float zoomSpeed;
	public float focusSpeed;

	bool initialized = false;

	void Start(){

		Initialize ();

	}

	void Initialize(){

		if (initialized) {
			return;
		}

		initialized = true;

		player = GameObject.FindObjectOfType<TrashPlayer> ();
		world = TrashWorld.Instance;

		currentCameraFocus = CalculatePlayerFocus ();

		SetCameraZoom (0);

		player.SetTrashCamera (this);

		SnapToPlayerPosition ();

	}

	public void SnapToPlayerPosition ()
	{

		Initialize ();

		Vector3 alignedPos = world.AlignCameraPositionWithinWorld (currentCameraFocus, trashCamera);

		alignedPos.z = CachedTransform.position.z;

		trashCamera.transform.position = alignedPos;
	}

	void LateUpdate(){


		UpdateZoom ();

		FocusOnPlayer ();

		UpdateFocus ();

		Vector3 alignedPos = world.AlignCameraPositionWithinWorld (currentCameraFocus, trashCamera);

		alignedPos.z = CachedTransform.position.z;

		trashCamera.transform.position = alignedPos;

		FocusOnPlayer ();


	}

	public const float TIME_TO_ZOOM = 1.5f;

	float velocity;

	public const float TIME_TO_FOCUS = 0.075f;

	public float TimeToFocus{
		get{

			float playerSpeed = player.CachedRigidbody2D.velocity.magnitude;

			if (playerSpeed > player.maxWalkSpeed) {

				float n = player.maxWalkSpeed / playerSpeed;

				return TIME_TO_FOCUS * n;

			}

			return TIME_TO_FOCUS;

		}
	}

	Vector3 focus_velocity;

	public const float MAXIMUM_FOCUS_DISTANCE_FROM_PLAYER_N = 0.33f;
	public const float MIN_CAMERA_STEP = 0.0f;

	public float maxFocusDist;

	void UpdateFocus ()
	{

		if (currentCameraFocus != targetCameraFocus) {

			float difference = Vector2.Distance ((Vector2)currentCameraFocus, (Vector2)targetCameraFocus);

			//currentCameraFocus = Vector3.MoveTowards (currentCameraFocus, targetCameraFocus, maxFocusDist * Time.deltaTime);

			currentCameraFocus = Vector3.SmoothDamp (currentCameraFocus, targetCameraFocus, ref focus_velocity, TimeToFocus * difference);//, focusSpeed, Time.deltaTime);

		}

	}

	Vector3 CalculateFocus (Vector3 playerPos, Vector3 otherPos)
	{

		/*Vector2 direction = otherPos - playerPos;

		float distance = Vector2.Distance (playerPos, otherPos);

		float half_height = trashCamera.orthographicSize;
		float half_width = half_height * Screen.width / Screen.height;

		float height = half_height * 2f;
		float width = half_width * 2f;

		float d_Height = height * MAXIMUM_FOCUS_DISTANCE_FROM_PLAYER_N;
		float d_Width = width * MAXIMUM_FOCUS_DISTANCE_FROM_PLAYER_N;

		if (Mathf.Abs (direction.x) > d_Width) {

			if (direction.x < 0f) {

				direction.x = -d_Width;

			} else {

				direction.x = d_Width;

			}

		}

		if (Mathf.Abs (direction.y) > d_Height) {

			if (direction.y < 0f) {

				direction.y = -d_Height;

			} else {

				direction.y = d_Height;

			}

		}

		return playerPos + (Vector3)direction;*/

		return playerPos;

	}

	void UpdateZoom(){

		float range = maxZoom - minZoom;

		float z = trashCamera.orthographicSize - minZoom;

		float n_z = z / range;

		debug_Current_N_Zoom = n_z;

		if (n_z != targetZoom) {

			float n = Mathf.SmoothDamp(n_z, targetZoom, ref velocity, TIME_TO_ZOOM,zoomSpeed,Time.deltaTime);

			SetCameraZoom (n);

		}


	}

	public void SetCameraTargetZoom(float n, TrashObject requestor){

		if (lockingID != NONE) {

			if (lockingID != requestor.GetInstanceID ()) {

				return;

			}

		}

		targetZoom = n;

	}

	public void SetCameraZoom(float n){

		float zoom = Mathf.Lerp (minZoom, maxZoom, n);

		trashCamera.orthographicSize = zoom;

	}

}

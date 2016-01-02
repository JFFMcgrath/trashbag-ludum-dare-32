using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrashWorld : FreeManagerBase<TrashWorld> {

	public LayerMask groundedLayerMask;

	public LayerMask GetGroundedLayerMask ()
	{
		return groundedLayerMask;
	}

	public Transform tleft,tright,tup,tdown;

	Vector3 lowerLeft, upperLeft,lowerRight,upperRight;

	Vector3 left;
	Vector3 right;
	Vector3 upper;
	Vector3 lower;
	float cache_Ground_Size;

	public Vector3 AlignCameraPositionWithinWorld (Vector3 cameraPos, Camera c)
	{

		float half_height = c.orthographicSize;
		float half_width = half_height * Screen.width / Screen.height;

		Vector3 widthMod = new Vector3 (half_width, 0f, 0f);
		Vector3 heightMod = new Vector3 (0f, half_height, 0f);
		Vector3 groundMod = new Vector3 (0f, cache_Ground_Size, 0f);

		left = cameraPos - widthMod;
		right = cameraPos + widthMod;
		upper = cameraPos + heightMod;
		lower = cameraPos - heightMod + groundMod;

		if (left.x < lowerLeft.x) {

			cameraPos.x += lowerLeft.x - left.x;

		}

		if (right.x > lowerRight.x) {

			cameraPos.x -= right.x - lowerRight.x;

		}

		if (upper.y > upperLeft.y) {

			cameraPos.y -= upper.y - upperLeft.y;

		}

		if (lower.y < lowerLeft.y) {

			cameraPos.y += lowerLeft.y - lower.y;

		}

		return cameraPos;

	}

	void Awake(){

		GenerateWorld ();

	}

	void Start(){

		lowerLeft = new Vector3 (tleft.position.x, tdown.position.y, 0f);
		upperLeft = new Vector3 (tleft.position.x, tup.position.y, 0f);
		lowerRight = new Vector3 (tright.position.x, tdown.position.y, 0f);
		upperRight = new Vector3 (tright.position.x, tup.position.y, 0f);

	}

	#region implemented abstract members of FreeManagerBase

	public override void InitializeManager ()
	{


	}

	public override bool IsInitializationCompleted ()
	{
		return true;
	}

	#endregion

	void OnDrawGizmos(){

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (left,0.5f);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere (right,0.5f);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (upper,0.5f);
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere (lower,0.5f);

		Gizmos.color = Color.red;
		Gizmos.DrawSphere (lowerLeft,0.5f);
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere (lowerRight,0.5f);
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere (upperLeft,0.5f);
		Gizmos.color = Color.magenta;
		Gizmos.DrawSphere (lowerRight,0.5f);

		for (int i = 0; i < wasterPos.Count; i++) {

			Gizmos.color = Color.gray;

			Gizmos.DrawSphere (wasterPos [i], 0.1f);

		}

		for (int i = 0; i < startupPos.Count; i++) {

			Gizmos.color = Color.red;

			Gizmos.DrawSphere (wasterPos [i], 0.1f);

		}

	}

	public int min_Wasters;
	public int max_Wasters;

	public Vector2 world_WidthRange;
	public Vector2 world_HeightRange;
	public Vector2 world_PaddingRange;

	public Vector2 background_HorizontalDistribution;
	public Vector2 background_VerticalDistribution;
	public float chanceToSpawnBackgroundElement;

	public Vector2 foreground_HorizontalDistribution;
	public float chanceToSpawnForegroundElement;

	public Vector2 startPosition;

	public int debug_WasterCount;

	public int min_TrashOnStartup;
	public int max_TrashOnStartup;

	public int maxTrashGenerated;
	float worldWidth;
	float worldHeight;

	public float trashGenerationRate;
	float lastTrashGeneration = 0f;

	int masterTrashCount;
	public int totalTrashGenerated;
	public int totalWastersGenerated;
	int masterWasterCount;

	public int GetRemainingWasters(){

		return masterWasterCount;

	}

	public void IncrementWasters(){
		masterWasterCount++;
		totalWastersGenerated++;
	}

	public void DecrementWasters(){
		masterWasterCount--;
	}

	public int GetMasterTrashCount(){

		return masterTrashCount;

	}

	public int GetMasterWasterCount(){

		return masterWasterCount;

	}

	public void IncrementTrashCount(){
		masterTrashCount++;
		totalTrashGenerated++;
	}

	public void DecrementTrashCount(){
		masterTrashCount--;
	}

	public bool CanGenerateTrash(){

		if (Time.time - lastTrashGeneration >= trashGenerationRate) {

			if (masterTrashCount < maxTrashGenerated) {

				return true;

			}

		}

		return false;

	}

	public void DestroyTrashItem(){

		DecrementTrashCount ();

		if (masterTrashCount == 0) {

			TrashUIManager.Instance.SetUIState (TrashUIManager.UIState.Gameover);

		}

	}

	void GenerateWorld(){

		float width = UnityEngine.Random.Range (world_WidthRange.x, world_WidthRange.y);
		float height = UnityEngine.Random.Range (world_HeightRange.x, world_HeightRange.y);

		worldWidth = width;
		worldHeight = height;

		Vector3 wVector = new Vector3 (0.5f * width, 0f,0f);
		Vector3 hVector = new Vector3 (0f, 0.5f * height,0f);

		tleft.position = CachedTransform.position - wVector;
		tleft.name = "Left";

		tright.position = CachedTransform.position + wVector;
		tright.name = "Right";

		tup.position =  CachedTransform.position + hVector;
		tup.name = "Up";

		tdown.position =  CachedTransform.position - hVector;
		tdown.name = "Down";

		int wasters = UnityEngine.Random.Range (min_Wasters, max_Wasters);

		int startupTrash = UnityEngine.Random.Range (min_TrashOnStartup, max_TrashOnStartup);

		debug_WasterCount = wasters;

		float offset = GetPadding ();

		Vector2 groundLevel = tdown.position;

		CreateGround ();
		CreateSky ();
		CreateBackgroundElements ();
		CreateWorldSegments ();
		CreateForegroundElements ();

		TrashHuman.MovementDirection facingDirection;

		if (UnityEngine.Random.Range (0, 2) == 1) {

			groundLevel.x = tright.position.x - offset;
			facingDirection = TrashHuman.MovementDirection.Left;

		} else {

			groundLevel.x = tleft.position.x + offset;
			facingDirection = TrashHuman.MovementDirection.Right;

		}

		TrashPlayer player = GameObject.FindObjectOfType<TrashPlayer> ();
		player.SetStartPosition (groundLevel, facingDirection);

		GenerateWasters (wasters);
		GenerateTrash (startupTrash);

	}

	void GenerateTrash (int startupTrash)
	{

		for (int i = 0; i < startupTrash; i++) {

			Vector2 pos = GetRandomGroundedPosition ();

			pos.y += worldHeight - 1f;

			startupPos.Add (pos);

			TrashItem item = GetNewTrashItem ();

			item.CachedTransform.position = pos + Vector2.up * 0.5f;
			item.AddRandomTorque ();

		}

	}

	Vector2 GetRandomGroundedPosition(){

		Vector2 ground = tdown.position;

		float x = Mathf.Lerp (tleft.position.x + GetPadding(), tright.position.x - GetPadding(), UnityEngine.Random.value);

		ground.x = x;

		return ground;

	}

	List<Vector3> wasterPos = new List<Vector3>();
	List<Vector3> startupPos = new List<Vector3>();

	void GenerateWasters (int wasters)
	{

		TrashSpawnPoint[] spawnPoints = GameObject.FindObjectsOfType<TrashSpawnPoint> ();

		List<TrashSpawnPoint> list_SpawnPoints = new List<TrashSpawnPoint> (spawnPoints);
		list_SpawnPoints.Sort (ShuffleSpawnPoints);

		for (int i = 0; i < wasters; i++) {

			Vector2 position;

			if (i >= list_SpawnPoints.Count) {

				position = GetRandomGroundedPosition ();

			} else {

				position = list_SpawnPoints [i].CachedTransform.position;

			}

			TrashEnemy e = GetNewEnemy ();

			e.CachedTransform.position = position;

			wasterPos.Add (position);

		}

	}

	int ShuffleSpawnPoints(TrashSpawnPoint a,TrashSpawnPoint b){

		return UnityEngine.Random.Range (-1, 2);

	}

	BoxCollider2D groundCollider;
	BoxCollider2D leftCollider;
	BoxCollider2D rightCollider;
	BoxCollider2D topCollider;

	void CreateGround(){

		float x = 0f;

		groundCollider = CachedGameObject.AddComponent<BoxCollider2D> ();

		topCollider = tup.gameObject.AddComponent<BoxCollider2D> ();

		leftCollider = tleft.gameObject.AddComponent<BoxCollider2D> ();
		rightCollider = tright.gameObject.AddComponent<BoxCollider2D> ();

		Vector2 size = new Vector2 (worldWidth, 5f);
		Vector2 offset = new Vector2 (0f, -(worldHeight * 0.5f + 2.5f));

		groundCollider.offset = offset;
		groundCollider.size = size;

		offset = new Vector2 (0f, (2.5f));

		topCollider.offset = offset;
		topCollider.size = size;

		size = new Vector2 (5f, worldHeight);
		offset = new Vector2 (-(2.5f), 0f);

		leftCollider.offset = offset;
		leftCollider.size = size;

		size = new Vector2 (5f, worldHeight);
		offset = new Vector2 ((2.5f), 0f);

		rightCollider.offset = offset;
		rightCollider.size = size;

		while (x <= worldWidth) {

			TrashGround ground = GetNewGround ();

			//ground.CachedTransform.parent = CachedTransform;

			float x_pos = GetXPositionFor (ground, x);

			x = GetNextXPositionFor (ground, x_pos);

			x_pos += tleft.position.x;

			float y = tdown.position.y - ground.GetHeight () * 0.5f;

			Vector2 pos = new Vector2 (x_pos, y);

			ground.CachedTransform.position = pos;

			cache_Ground_Size = ground.GetHeight () * 0.99f;

		}

	}

	void CreateSky(){

		float y = 0f;

		while(y <= worldHeight){

			float x = 0f;
			float y_height = 0f;

			while (x <= worldWidth) {

				GenericTrash sky = GetNewSky ();

				y_height = sky.GetHeight ();

				float x_pos = GetXPositionFor (sky, x);

				x = GetNextXPositionFor (sky, x_pos);

				x_pos += tleft.position.x;

				Vector2 pos = new Vector2 (x_pos, y + tdown.position.y);

				sky.CachedTransform.position = pos;

			}

			y += y_height;

		}

	}

	public float backgroundBeginHeight = 5f;

	void CreateBackgroundElements(){

		float y = backgroundBeginHeight;

		while(y <= worldHeight){

			float x = 0f;
			float y_height = 0f;

			while (x <= worldWidth) {

				if (UnityEngine.Random.value <= chanceToSpawnBackgroundElement) {

					float x_pos = x + UnityEngine.Random.Range (background_HorizontalDistribution.x, background_HorizontalDistribution.y);

					if (x_pos > worldWidth) {
						x = worldWidth + 1f;
						break;
					}

					GenericTrash background = GetNewBackgroundElement ();

					x = GetNextXPositionFor (background, x_pos);

					x_pos += tleft.position.x;

					Vector2 pos = new Vector2 (x_pos, y + tdown.position.y);

					background.CachedTransform.position = pos;

				} else {

					x += UnityEngine.Random.Range (background_HorizontalDistribution.x, background_HorizontalDistribution.y);

				}

			}

			y += UnityEngine.Random.Range(background_VerticalDistribution.x,background_VerticalDistribution.y);

		}
	
	}

	void CreateForegroundElements(){

		float x = 0f;

		while (x <= worldWidth) {

			if (UnityEngine.Random.value <= chanceToSpawnForegroundElement) {

				float x_pos = x + UnityEngine.Random.Range (foreground_HorizontalDistribution.x, foreground_HorizontalDistribution.y);

				if (x_pos > worldWidth) {
					x = worldWidth + 1f;
					break;
				}

				GenericTrash foreground = GetNewForegroundElement ();

				x = GetNextXPositionFor (foreground, x_pos);

				x_pos += tleft.position.x;

				Vector2 pos = new Vector2 (x_pos, tdown.position.y + foreground.GetHeight() * 0.5f);

				foreground.CachedTransform.position = pos;

			} else {

				x += UnityEngine.Random.Range (background_HorizontalDistribution.x, background_HorizontalDistribution.y);

			}

		}


	}

	void CreateWorldSegments(){

		float x = world_PaddingRange.y;

		float padding = world_PaddingRange.y;

		while (x <= worldWidth - padding) {

			GenericTrash worldSegment = GetNewWorldSegment ();

			//worldSegment.CachedTransform.parent = CachedTransform;

			float x_pos = GetXPositionFor (worldSegment, x);

			x = GetNextXPositionFor (worldSegment, x_pos) + GetPadding();

			x_pos += tleft.position.x;

			float y = tdown.position.y + worldSegment.GetHeight () * 0.5f;

			Vector2 pos = new Vector2 (x_pos, y);

			worldSegment.CachedTransform.position = pos;

		}

	}

	float GetXPositionFor(TrashSpawnable spawnable, float last_x){

		return last_x + spawnable.GetWidth () * 0.5f;

	}

	float GetNextXPositionFor(TrashSpawnable spawnable, float x){

		return x + spawnable.GetWidth () * 0.5f;

	}

	float GetPadding(){

		return UnityEngine.Random.Range (world_PaddingRange.x, world_PaddingRange.y);

	}

	public TrashItem[] trashItemEntries;

	public TrashItem GetNewTrashItem(){

		IncrementTrashCount ();

		TrashItem t = trashItemEntries[UnityEngine.Random.Range(0,trashItemEntries.Length)];

		return GameObject.Instantiate (t) as TrashItem;

	}

	public TrashEnemy[] enemyEntries;

	public TrashEnemy GetNewEnemy(){

		TrashEnemy t = enemyEntries[UnityEngine.Random.Range(0,enemyEntries.Length)];

		IncrementWasters ();

		return GameObject.Instantiate (t) as TrashEnemy;

	}

	public TrashGround[] groundEntries;

	TrashGround GetNewGround(){

		TrashGround t = groundEntries[UnityEngine.Random.Range(0,groundEntries.Length)];

		return GameObject.Instantiate (t) as TrashGround;

	}

	public GenericTrash[] skyEntries;

	GenericTrash GetNewSky ()
	{
		return GetRandomEntry (skyEntries);
	}

	public GenericTrash[] backgroundEntries;

	GenericTrash GetNewBackgroundElement(){

		return GetRandomEntry (backgroundEntries);

	}

	public GenericTrash[] foregroundEntries;

	GenericTrash GetNewForegroundElement(){

		return GetRandomEntry (foregroundEntries);

	}

	public GenericTrash[] worldSegmentEntries;

	GenericTrash GetNewWorldSegment(){

		return GetRandomEntry (worldSegmentEntries);

	}

	GenericTrash GetRandomEntry(GenericTrash[] trash){

		GenericTrash t = trash[UnityEngine.Random.Range(0,trash.Length)];

		GenericTrash t_o = GameObject.Instantiate (t) as GenericTrash;

		//t_o.CachedTransform.parent = CachedTransform;

		return t_o;

	}


}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TrashUIManager : FreeManagerBase<TrashUIManager> {

	public TrashAnnouncement prefab_Announcement;

	public Canvas uiCanvas;

	public enum UIState{
		None,
		Title,
		Gameplay,
		Pause,
		Gameover,
		Restarting
	}

	public GameOverLayout layout_gameOver;
	public GameplayLayout layout_gamePlay;
	public PauseLayout layout_pause;
	public TitleLayout layout_Title;

	FreeUILayout currentLayout;

	StateMachine<UIState> uiState;
	UIState cacheState;

	public string[] tagLines;
	public string[] pauseLines;
	public string[] ratings;
	public string[] tutorialLines;
	public string[] trashDestroyed;
	public string[] wastersWasted;

	public bool hasShownTutorial = false;

	public string GetTagline(){

		int index = UnityEngine.Random.Range (0, tagLines.Length);

		return tagLines[index];

	}

	public string GetTrashDestroyedLabel ()
	{
		int index = UnityEngine.Random.Range (0, trashDestroyed.Length);

		return trashDestroyed[index];
	}

	public string GetWastersDestroyedLabel ()
	{
		int index = UnityEngine.Random.Range (0, wastersWasted.Length);

		return wastersWasted[index];
	}

	public string GetPauseLine(){

		int index = UnityEngine.Random.Range (0, pauseLines.Length);

		return pauseLines[index];

	}

	public string GetRating(){

		int index = UnityEngine.Random.Range (0, ratings.Length);

		return ratings[index];

	}

	void Awake(){

		uiState = new StateMachine<UIState> (BeginUIState, EndUIState);

		//hasShownTutorial = PlayerPrefs.GetInt ("TUT", 0) == 1;

	}

	void Start(){

		Screen.SetResolution (900, 600, false);

		//
		layout_gameOver.Initialize ();
		layout_gamePlay.Initialize ();
		layout_pause.Initialize ();
		layout_Title.Initialize ();

		uiState.SetState (UIState.Title);

		StartCoroutine (HandleGameTimer());

	}

	public void ShowTutorialText (int currentTutorialIndex)
	{

		if (uiState.GetState() == UIState.Gameplay) {

			layout_gamePlay.ShowTutorialText (tutorialLines [currentTutorialIndex]);

		}

	}

	public void SetPowerMeter(float n){

		if (uiState.GetState() == UIState.Gameplay) {

			layout_gamePlay.SetPowerMeter (n);

		}

	}

	public void HidePowerMeter(){

		if (uiState.GetState() == UIState.Gameplay) {

			layout_gamePlay.HidePowerMeter ();

		}

	}

	public bool gameInProgress = false;
	public float gameTime = 0f;

	public void EndGame(){

		gameInProgress = false;

	}

	IEnumerator HandleGameTimer(){

		yield return new WaitForSeconds (1f);

		gameInProgress = true;

		WaitForSeconds eof = new WaitForSeconds (0);

		while (gameInProgress) {

			gameTime += Time.deltaTime;
			
			yield return eof;

		}

	}

	public string GetTimeAsString ()
	{
		TimeSpan t = TimeSpan.FromSeconds (gameTime);

		return  string.Format("{0:D2}:{1:D2}", 
			t.Minutes, 
			t.Seconds);
	}

	void OnHideCompleted ()
	{

		if (cacheState != UIState.None) {

			uiState.SetState (cacheState);

		}

		cacheState = UIState.None;

	}

	public void SetUIState(UIState state){

		if (uiState.GetState () == state) {
			return;
		}

		/*if (cacheState == state) {
			return;
		}

		if (currentLayout != null) {
			cacheState = state;
			currentLayout.HideLayout (true, OnHideCompleted);
		} else {
			uiState.SetState (state);
		}*/

		uiState.SetState (state);

	}

	FreeUILayout GetLayoutForState(UIState state){

		switch(state){
			case UIState.Gameover:
			{

				return layout_gameOver;

				break;
			}
			case UIState.Title:
			{

				return layout_Title;

				break;
			}
			case UIState.Gameplay:
			{

				return layout_gamePlay;

				break;
			}
			case UIState.Pause:
			{

				return layout_pause;

				break;
			}
		}

		return null;

	}

	void BeginUIState (UIState state)
	{

		if (state == UIState.Restarting) {

			TrashManager.Instance.Restart ();

			return;

		}

		FreeUILayout layout = GetLayoutForState (state);

		if (layout == null) {

			return;

		}

		currentLayout = layout;

		layout.ShowLayout (true, null);

	}

	void EndUIState (UIState state)
	{

		FreeUILayout layout = GetLayoutForState (state);

		if (layout == null) {

			return;

		}
		//
		layout.HideLayout (true,null);

	}

	[Serializable]
	public class AnnouncementList{

		public List<string> entries;

	}

	[Serializable]
	public class AnnouncementMapping{

		public AnnouncementType announcementType;
		public Color color;
		public List<AnnouncementList> announcements;

	}

	public List<AnnouncementMapping> announcementMappings;

	string GetAnnouncementFor (AnnouncementType announcementType, float n)
	{

		List<AnnouncementList> announcements = GetAnnouncementsFor (announcementType);

		if (announcements == null) {

			Debug.LogError ("No announcements found for: " + announcementType.ToString ());
			return "...";

		}

		int index = (int)(n * (float)announcements.Count);

		index = Mathf.Clamp (index, 0, announcements.Count-1);

		List<string> words = announcements [index].entries;

		int i = UnityEngine.Random.Range(0,words.Count);

		return words [i];

	}

	Color GetColorFor(AnnouncementType aType){

		for(int i = 0; i < announcementMappings.Count; i++){

			if (announcementMappings [i].announcementType == aType) {

				return announcementMappings [i].color;

			}

		}

		return Color.white;

	}

	List<AnnouncementList> GetAnnouncementsFor(AnnouncementType aType){

		for(int i = 0; i < announcementMappings.Count; i++){

			if (announcementMappings [i].announcementType == aType) {

				return announcementMappings [i].announcements;

			}

		}

		return null;

	}

	public TrashAnnouncement MakeAnnouncement(Transform bind, AnnouncementType announcementType, float n){

		TrashAnnouncement a = GameObject.Instantiate (prefab_Announcement) as TrashAnnouncement;

		a.CachedTransform.SetParent (uiCanvas.transform, false);

		a.SetAnnouncement(bind, GetColorFor(announcementType), GetAnnouncementFor (announcementType,n));

		if (announcementType != AnnouncementType.Hit) {

			a.AutoDestroy ();

		}

		return a;

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

}

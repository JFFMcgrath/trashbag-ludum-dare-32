using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameplayLayout : FreeUILayout {

	public Text text_Time;
	public Text text_TrashRemaining;
	public Text text_WastersRemaining;

	public FreeLerpingUIObject panel_TutorialText;
	public Text text_TutorialText;

	public FreeLerpingUIObject panel_Power;
	public Image image_PowerFill;

	public Button button_Pause;

	void Update(){

		text_Time.text = TrashUIManager.Instance.GetTimeAsString ();
		text_TrashRemaining.text = TrashWorld.Instance.GetMasterTrashCount ().ToString();
		text_WastersRemaining.text = TrashWorld.Instance.GetMasterWasterCount ().ToString();

	}

	public override void OnInitialize ()
	{

		base.OnInitialize ();

		InitializeButtonListeners ();

	}

	void InitializeButtonListeners ()
	{

		button_Pause.AddClickListener (HandlePauseClicked);

	}

	void HandlePauseClicked ()
	{
		TrashUIManager.Instance.SetUIState (TrashUIManager.UIState.Pause);
	}

	IEnumerator hideTut = null;

	public void SetPowerMeter(float n){

		panel_Power.Show (false);
		image_PowerFill.fillAmount = n;

	}

	public void HidePowerMeter(){

		panel_Power.Hide (false);

	}

	public void ShowTutorialText (string str)
	{
		panel_TutorialText.Show (false);
		text_TutorialText.enabled = true;
		text_TutorialText.text = str;

		if (hideTut != null) {
			StopCoroutine (hideTut);
			hideTut = null;
		}

		hideTut = HideTutorialText ();

		StartCoroutine (hideTut);

	}

	IEnumerator HideTutorialText ()
	{

		yield return new WaitForSeconds (TrashAnnouncement.STANDARD_DESTROY_TIME);
		panel_TutorialText.Hide (false);

	}

	#region implemented abstract members of FreeUILayout
	public override void OnRefreshLayout ()
	{
		HidePowerMeter ();
		panel_TutorialText.Hide (false);
	}
	public override void OnShowLayout (bool instantly, global::FreeUILayout.LayoutActionCompleted onCompleted)
	{
		HandleDefaultVisibility (instantly);
	}
	public override void OnHideLayout (bool instantly, global::FreeUILayout.LayoutActionCompleted onCompleted)
	{
		HideChildren (ChildObjects, instantly, onCompleted);
	}
	#endregion

}

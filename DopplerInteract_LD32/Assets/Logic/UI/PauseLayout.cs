using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseLayout : FreeUILayout {

	public FreeLerpingUIObject panel_Message;

	public Text text_PauseMessage;

	public FreeLerpingUIObject panel_Play;
	public Button button_Play;
	public Button button_Exit;

	public override void OnInitialize ()
	{
		base.OnInitialize ();

		InitializeButtonListeners ();

	}

	void InitializeButtonListeners ()
	{
		button_Play.AddClickListener (HandlePlayClicked);
		button_Exit.AddClickListener (HandleExitClicked);

	}

	void HandlePlayClicked ()
	{
		TrashUIManager.Instance.SetUIState (TrashUIManager.UIState.Gameplay);
	}

	void HandleExitClicked ()
	{
		TrashManager.Instance.QuitGame ();
	}

	#region implemented abstract members of FreeUILayout
	public override void OnRefreshLayout ()
	{
		text_PauseMessage.text = TrashUIManager.Instance.GetPauseLine ();
	}
	public override void OnShowLayout (bool instantly, global::FreeUILayout.LayoutActionCompleted onCompleted)
	{
		HandleDefaultVisibility (instantly);
		Time.timeScale = 0f;
	}
	public override void OnHideLayout (bool instantly, global::FreeUILayout.LayoutActionCompleted onCompleted)
	{
		HideChildren (ChildObjects, instantly, onCompleted);
		Time.timeScale = 1f;
	}
	#endregion

}

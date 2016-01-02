using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TitleLayout : FreeUILayout {

	public Text text_Tagline;

	public Button button_Play;

	public override void OnInitialize ()
	{
		base.OnInitialize ();

		InitializeButtonListeners ();

	}

	void InitializeButtonListeners ()
	{

		button_Play.AddClickListener (HandlePlayClicked);

	}

	void HandlePlayClicked ()
	{
		TrashUIManager.Instance.SetUIState (TrashUIManager.UIState.Gameplay);
	}

	#region implemented abstract members of FreeUILayout
	public override void OnRefreshLayout ()
	{
		text_Tagline.text = TrashUIManager.Instance.GetTagline ();
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

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOverLayout : FreeUILayout {

	public Text text_Rating;

	public Text text_TimeTaken;

	public Text text_WastersWasted;
	public Text text_TrashDestroyed;

	public Text text_LabelWastersWasted;
	public Text text_LabelTrashDestroyed;

	public Button button_Replay;
	public Button button_Exit;

	public override void OnInitialize ()
	{
		base.OnInitialize ();

		InitializeButtonListeners ();

	}

	void InitializeButtonListeners ()
	{


		button_Replay.AddClickListener (HandleReplayClicked);

	}

	void HandleReplayClicked ()
	{
		TrashUIManager.Instance.SetUIState (TrashUIManager.UIState.Restarting);
	}

	#region implemented abstract members of FreeUILayout
	public override void OnRefreshLayout ()
	{
		text_Rating.text = TrashUIManager.Instance.GetRating ();
		text_TrashDestroyed.text = TrashWorld.Instance.totalTrashGenerated.ToString();
		text_WastersWasted.text = TrashWorld.Instance.totalWastersGenerated.ToString();

		text_LabelWastersWasted.text = TrashUIManager.Instance.GetWastersDestroyedLabel ();
		text_LabelTrashDestroyed.text = TrashUIManager.Instance.GetTrashDestroyedLabel ();

	}
	public override void OnShowLayout (bool instantly, global::FreeUILayout.LayoutActionCompleted onCompleted)
	{
		HandleDefaultVisibility (instantly);
		TrashUIManager.Instance.EndGame ();

	}
	public override void OnHideLayout (bool instantly, global::FreeUILayout.LayoutActionCompleted onCompleted)
	{
		HideChildren (ChildObjects, instantly, onCompleted);
	}
	#endregion

}

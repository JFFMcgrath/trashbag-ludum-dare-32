using UnityEngine;
using System.Collections;

public class FreeAnimatedUIObject : FreeUIObject {

	public string anim_Hide;
	public string anim_Show;

	public bool shareAnimationTimes;

	LegacyAnimationTracker _animationTracker;
	public LegacyAnimationTracker animationTracker{
		get{

			if(_animationTracker == null){

				GameObject target = CachedLegacyAnimation.gameObject;

				_animationTracker = target.AddComponent<LegacyAnimationTracker>();

			}

			return _animationTracker;
		}
	}

	protected override void InitializeUIObject(){

		if (!string.IsNullOrEmpty (anim_Hide)) {

			animationTracker.SubscribeToAnimationEvent (anim_Hide, 1f, HideComplete);

		}

		if (!string.IsNullOrEmpty (anim_Show)) {

			animationTracker.SubscribeToAnimationEvent (anim_Show, 1f, ShowComplete);

		}

	}

	public override void HandleHide (bool instant)
	{
		if (instant) {

			CachedGameObject.SetActive (false);
			animationTracker.SampleAnimation (anim_Hide, 1f);

			HideComplete (anim_Hide);

		} else {

			float time = 1f - GetShowAmount ();

			if (!shareAnimationTimes) {
				time = 0f;
			}

			if (time == 1f) {

				HideComplete (anim_Hide);

			} else {

				animationTracker.PlayAnimation (anim_Hide);
				animationTracker.SetAnimationTime (anim_Hide, time);

			}

		}
	}

	public override void HandleShow (bool instant)
	{

		if (instant) {

			animationTracker.SampleAnimation (anim_Show, 1f);

			CachedGameObject.SetActive (true);

			ShowComplete (anim_Show);


		} else {

			float time = GetShowAmount ();

			if (!shareAnimationTimes) {
				time = 0f;
			}

			if (time == 1f) {

				ShowComplete (anim_Show);

			} else {

				CachedGameObject.SetActive (true);

				animationTracker.PlayAnimation (anim_Show);

				animationTracker.SetAnimationTime (anim_Show, time);

			}

		}
	}

	public override float GetCurrentShowAmount ()
	{

		if (animationTracker.IsPlayingAnimation (anim_Hide)) {

			return 1f - animationTracker.GetNormalizedAnimationTime (anim_Hide);

		} else if (animationTracker.IsPlayingAnimation (anim_Show)) {

			return animationTracker.GetNormalizedAnimationTime (anim_Show);

		}

		return 0f;
	}

}

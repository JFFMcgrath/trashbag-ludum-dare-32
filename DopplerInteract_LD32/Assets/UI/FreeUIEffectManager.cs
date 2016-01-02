using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FreeUIEffectManager : FreeManagerBase<FreeUIEffectManager> {

	public ClampedAnimationCurve anim_pulsateCurve;
	public ClampedAnimationCurve anim_LerpPositionCurve;
	public ClampedAnimationCurve anim_LerpScaleCurve;

	public float highlight_OnFlashTime;
	public float highlight_OffFlashTime;

	public float pulsate_TargetScale;
	public float pulsate_ScaleTime;
	public int pulsate_ScaleRepetitions;

	public Canvas UICanvas;

	#region implemented abstract members of FreeManagerBase

	public override void InitializeManager ()
	{
	}

	public override bool IsInitializationCompleted ()
	{
		return true;
	}

	#endregion

	public float GetUIPositionLerp (float t)
	{
		return anim_LerpPositionCurve.GetValueAt (t);
	}

	public float GetUIScaleLerp (float t)
	{
		return anim_LerpScaleCurve.GetValueAt (t);
	}


}

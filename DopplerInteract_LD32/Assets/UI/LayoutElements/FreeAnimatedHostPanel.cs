using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FreeAnimatedHostPanel : FreeLerpingUIObject {

	public Animation anim_Controller;
	public string anim_Display;

	public void AttachIcon(Image image){

		RectTransform rt = image.GetComponent<RectTransform> ();

		BindModule (rt);

		if (anim_Controller != null) {

			anim_Controller.Play (anim_Display);

		}

	}

	public void DetachIcon(){

		DestroyModule ();

		if (anim_Controller != null) {

			anim_Controller.Stop ();

		}

	}

}

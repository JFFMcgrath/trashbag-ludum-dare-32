using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FreeUIObjectBase : EfficientBehaviour {

	protected bool isInteractive;
	//

	/*RectTransform _cachedRectTransform;
	public RectTransform CachedRectTransform{
		get{

			if(_cachedRectTransform == null){

				_cachedRectTransform = GetComponent<RectTransform> ();

			}

			return _cachedRectTransform;
		}
	}*/

	Graphic _cachedGraphic;
	public Graphic CachedGraphic{
		get{

			if(_cachedGraphic == null){

				_cachedGraphic = GetComponent<Graphic> ();

			}

			return _cachedGraphic;
		}
	}

	public void SetIsInteractive(bool isInteractive){

		if (isInteractive == this.isInteractive) {
			return;
		}

		this.isInteractive = isInteractive;

		if (CachedCanvasGroup != null) {
			if (this.isInteractive) {
				CachedCanvasGroup.interactable = true;
			} else {
				CachedCanvasGroup.interactable = false;
			}
		}

	}

	CanvasGroup _canvasGroup;
	public CanvasGroup CachedCanvasGroup{
		get{

			if(_canvasGroup == null){

				_canvasGroup = GetComponent<CanvasGroup> ();

				if (_canvasGroup == null) {

				}

			}

			return _canvasGroup;
		}
	}

	public void AlignToWorldPosition(Vector3 worldPosition){
	
		CachedRectTransform.AlignToWorldPosition (worldPosition);

	}

	public void AlignToWorldObject(Transform worldTransform){

		CachedRectTransform.AlignToWorldTransform (worldTransform);

	}

	public bool IsInteractive(){

		return isInteractive && CanObjectReceiveInput();

	}

	public bool IsUIObjectVisible(){

		return CachedGameObject.activeSelf;

	}

	public virtual bool CanObjectReceiveInput(){

		return true;

	}

}

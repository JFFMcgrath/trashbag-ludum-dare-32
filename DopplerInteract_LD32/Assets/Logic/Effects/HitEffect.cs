using UnityEngine;
using System.Collections;

public class HitEffect : TrashObject {

	public float timeToShow;
	public float timeToDisappear;
	public ClampedAnimationCurve disappearCurve;
	public Transform UIBind;

	SpriteRenderer[] allSprites;

	TrashAnnouncement announcement;
	float start_Alpha = 1f;
	
	#region implemented abstract members of TrashObject

	protected override void OnInitializeTrashObject ()
	{

		if (CachedParticleSystem != null) {

			CachedParticleSystem.GetComponent<Renderer> ().sortingLayerName = "Particles";

		}

		InitializeHitEffect ();
		StartCoroutine (WaitToDisappear ());

	}

	bool initialized = false;

	void InitializeHitEffect(){

		if (initialized) {
			return;
		}

		initialized = true;

		allSprites = GetComponentsInChildren<SpriteRenderer> ();

		if (allSprites.Length > 0) {
			start_Alpha = allSprites [0].color.a;
		}

	}

	#endregion

	public void SetEffect(Vector3 position, float n, Color c, AnnouncementType announcementType){

		InitializeHitEffect ();

		start_Alpha = c.a;

		SetSpriteColor (c);

		CachedTransform.position = position;

		announcement = TrashUIManager.Instance.MakeAnnouncement (UIBind, announcementType, n);

	}

	IEnumerator WaitToDisappear(){

		yield return new WaitForSeconds (timeToShow);

		float currentTime = 0f;

		while (currentTime <= timeToDisappear) {

			currentTime += Time.deltaTime;

			float n = currentTime / timeToDisappear;

			float d_n = disappearCurve.GetValueAt (n);

			if (allSprites != null && allSprites.Length > 0) {

				for (int i = 0; i < allSprites.Length; i++) {

					Color c = allSprites [i].color;

					c.a = Mathf.Lerp (start_Alpha, 0f, d_n);

					allSprites [i].color = c;

				}

			}

			announcement.SetAlpha (d_n);

			yield return new WaitForSeconds (0);

		}

		announcement.DestroyAnnouncement ();

		GameObject.Destroy (this.gameObject);

	}

	void SetSpriteColor(Color c){

		for (int i = 0; i < allSprites.Length; i++) {

			allSprites[i].color = c;

		}

	}

}

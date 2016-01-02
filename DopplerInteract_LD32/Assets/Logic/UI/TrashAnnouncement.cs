using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrashAnnouncement : EfficientBehaviour {

	Text text;
	float startAlpha;

	Vector3 worldPosition;

	public const float STANDARD_DESTROY_TIME = 4f;
	public const float STANDARD_FADE_TIME = 2f;

	public void SetAnnouncement(Transform bind, Color c, string message){

		worldPosition = bind.position;

		text = GetComponent<Text> ();

		Vector2 offset = new Vector2 (UnityEngine.Random.Range (-1.5f, 1.5f), UnityEngine.Random.Range (-1.5f, 1.5f));

		worldPosition += (Vector3)offset;

		text.fontSize = text.fontSize + UnityEngine.Random.Range (-5, 5);

		CachedRectTransform.AlignToWorldPosition (worldPosition, TrashUIManager.Instance.uiCanvas.planeDistance);

		Vector3 rotation = bind.root.eulerAngles;

		rotation.z += UnityEngine.Random.Range (-20f, 20f);

		CachedTransform.rotation = Quaternion.Euler(rotation);

		text.color = c;

		startAlpha = text.color.a;

		text.text = message;

	}

	void LateUpdate(){

		CachedRectTransform.AlignToWorldPosition (worldPosition, TrashUIManager.Instance.uiCanvas.planeDistance);

	}

	public void SetAlpha(float n){

		Color c = text.color;
		c.a = Mathf.Lerp (startAlpha, 0f, n);
		text.color = c;

	}

	public void AutoDestroy ()
	{
		StartCoroutine (WaitToDestroy ());
	}

	IEnumerator WaitToDestroy(){

		yield return new WaitForSeconds (STANDARD_DESTROY_TIME);

		float n = 0f;
		float time = 0f;

		while (time < STANDARD_FADE_TIME) {

			time += Time.deltaTime;

			n = time / STANDARD_FADE_TIME;

			SetAlpha (n);

			yield return new WaitForSeconds (0);

		}

		DestroyAnnouncement ();

	}

	public void DestroyAnnouncement(){

		GameObject.Destroy (this.gameObject);

	}

}

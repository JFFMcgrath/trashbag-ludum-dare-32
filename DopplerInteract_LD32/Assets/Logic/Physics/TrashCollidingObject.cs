using UnityEngine;
using System.Collections;

public abstract class TrashCollidingObject : TrashObject {

	void OnCollisionEnter2D(Collision2D collision){

		HandleCollisionEnter (collision);

		TrashObject trashObject = GetTrashObjectFromCollider (collision.collider);

		if (trashObject != null) {

			HandleCollisionEnterWithTrashObject (trashObject);

		}

	}

	public abstract void HandleCollisionEnter (Collision2D collision);
	public abstract void HandleCollisionEnterWithTrashObject (TrashObject trashObject);

	void OnCollisionExit2D(Collision2D collision){

		HandleCollisionExit2D (collision);

		TrashObject trashObject = GetTrashObjectFromCollider (collision.collider);

		if (trashObject != null) {

			HandleCollisionExitWithTrashObject (trashObject);

		}
	}

	public abstract void HandleCollisionExit2D (Collision2D collision);
	public abstract void HandleCollisionExitWithTrashObject (TrashObject trashObject);

	void OnTriggerEnter2D(Collider2D collider){

		HandleTriggerEnter2D (collider);

		TrashObject trashObject = GetTrashObjectFromCollider (collider);

		if (trashObject != null) {

			HandleTriggerEnterWithTrashObject (trashObject);

		}

	}

	public abstract void HandleTriggerEnter2D (Collider2D collider);
	public abstract void HandleTriggerEnterWithTrashObject (TrashObject trashObject);

	void OnTriggerExit2D(Collider2D collider){

		if (CachedCollider2D.bounds.Contains (collider.transform.position)) {
			return;
		}

		HandleTriggerExit2D (collider);

		TrashObject trashObject = GetTrashObjectFromCollider (collider);

		if (trashObject != null) {

			HandleTriggerExitWithTrashObject (trashObject);

		}

	}

	public abstract void HandleTriggerExit2D (Collider2D collider);
	public abstract void HandleTriggerExitWithTrashObject (TrashObject trashObject);

	protected TrashObject GetTrashObjectFromCollider(Collider2D collider){

		Rigidbody2D rb = collider.attachedRigidbody;

		TrashObject trashObject = null;

		if (rb != null) {

			trashObject = rb.GetComponentInChildren<TrashObject> ();

		}

		if (trashObject == null) {

			trashObject = collider.gameObject.GetComponentInChildren<TrashObject> ();

		}

		return trashObject;

	}

}

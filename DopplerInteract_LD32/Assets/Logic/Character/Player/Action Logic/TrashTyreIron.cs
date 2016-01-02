using UnityEngine;
using System.Collections;

public class TrashTyreIron : TrashCollidingObject {

	public delegate void OnTyreIronHitItem (TrashItem item);
	OnTyreIronHitItem _onTyreIronHitItem;

	public delegate void OnTyreIronHitEnemy (TrashEnemy enemy);
	OnTyreIronHitEnemy _onTyreIronHitEnemy;

	public Transform swish_Bind;
	public SwishEffect prefab_swishEffect;
	SwishEffect currentEffect = null;

	Color baseColor;

	public Color powerUpEndColor;

	public Color base_powerUpSwishStartColor;
	public Color base_powerUpSwishEndColor;

	public Color max_powerUpSwishStartColor;
	public Color max_powerUpSwishEndColor;

	Color swishColorStart;
	Color swishColorEnd;

	public float swishStartWidth;
	public float swishEndWidth;

	public void BeginSwing(){

		if (currentEffect != null) {
			currentEffect.EndSwishEffect ();
		}

		SwishEffect effect = GameObject.Instantiate (prefab_swishEffect) as SwishEffect;

		effect.CachedLineRenderer.SetWidth (swishStartWidth, swishEndWidth);

		effect.CachedTransform.SetParent (swish_Bind);
		effect.CachedTransform.localPosition = Vector3.zero;

		effect.SetColor (swishColorStart, swishColorEnd);
		effect.BeginSwishEffect ();

		currentEffect = effect;

	}

	public void EndSwing(){

		if (currentEffect != null) {
			currentEffect.EndSwishEffect ();
			currentEffect = null;
		}

	}

	public void RevertColor(){


		CachedSpriteRenderer.color = baseColor;

	}

	public void UpdatePower (float f)
	{

		CachedSpriteRenderer.color = Color.Lerp (baseColor, powerUpEndColor, f);
		swishColorStart = Color.Lerp (base_powerUpSwishStartColor, max_powerUpSwishStartColor, f);
		swishColorEnd = Color.Lerp (base_powerUpSwishEndColor, max_powerUpSwishEndColor, f);

	}

	public void SubscribeToTyreIronCollision(OnTyreIronHitItem onTyreIronHitItem, OnTyreIronHitEnemy onTyreIronHitEnemy){

		_onTyreIronHitItem = onTyreIronHitItem;
		_onTyreIronHitEnemy = onTyreIronHitEnemy;

	}

	#region implemented abstract members of TrashObject

	protected override void OnInitializeTrashObject ()
	{
	
		baseColor = CachedSpriteRenderer.color;

	}

	#endregion

	#region implemented abstract members of TrashCollidingObject

	public override void HandleCollisionEnter (Collision2D collision)
	{
		
	}

	public override void HandleCollisionEnterWithTrashObject (TrashObject trashObject)
	{
	}

	public override void HandleCollisionExit2D (Collision2D collision)
	{
		
	}

	public override void HandleCollisionExitWithTrashObject (TrashObject trashObject)
	{
		
	}

	public override void HandleTriggerEnter2D (Collider2D collider)
	{
		
	}

	public override void HandleTriggerEnterWithTrashObject (TrashObject trashObject)
	{

		if (trashObject is TrashItem) {

			if (_onTyreIronHitItem != null) {
				_onTyreIronHitItem (trashObject as TrashItem);
			}

		} else if (trashObject is TrashEnemy) {

			TrashEnemy enemy = trashObject as TrashEnemy;

			if (_onTyreIronHitEnemy != null) {
				_onTyreIronHitEnemy (enemy);
			}

		}
	}

	public override void HandleTriggerExit2D (Collider2D collider)
	{
		
	}

	public override void HandleTriggerExitWithTrashObject (TrashObject trashObject)
	{
		
	}

	#endregion


	//Responsible for collisions!

}

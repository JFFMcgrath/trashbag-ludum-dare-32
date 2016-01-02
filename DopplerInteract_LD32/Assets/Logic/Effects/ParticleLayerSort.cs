using UnityEngine;
using System.Collections;

public class ParticleLayerSort : MonoBehaviour {

	// Use this for initialization
	void Awake () {
	
		Renderer[] r = GetComponentsInChildren<Renderer> ();

		for (int i = 0; i < r.Length; i++) {

			r [i].sortingLayerName = "Particles";

		}

	}

}

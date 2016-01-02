using UnityEngine;
using System.Collections;

public class RenderQueueCorrector : MonoBehaviour {

	public Material[] materials;

	void OnEnable () {
	
		for (int i = 0; i < materials.Length; i++) {

			materials [i].renderQueue = i;

		}

	}

}

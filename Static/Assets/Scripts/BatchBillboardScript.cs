using UnityEngine;
using System.Collections;

public class BatchBillboardScript : MonoBehaviour {

	public bool unlockXAxis;
	public bool unlockYAxis;
	public bool unlockZAxis;

	Vector3 cameraDirection;
	Quaternion tempRotation;
	Vector3 finalEuler;

	Transform[] billboardTransforms;

	int iStart = 0;
	int iSkip = 5;


	void Start() {
		// Find all game objects with billboard sprites, then disable the billboard scripts
		GameObject[] billboards = GameObject.FindGameObjectsWithTag("Billboard");
		billboardTransforms = new Transform[billboards.Length];
		for (int i = 0; i < billboards.Length; i++) {
			billboardTransforms [i] = billboards [i].transform;
		}
	}


	public void ReupdateBillboards() {
		// Find all game objects with billboard sprites, then disable the billboard scripts
		GameObject[] billboards = GameObject.FindGameObjectsWithTag("Billboard");
		billboardTransforms = new Transform[billboards.Length];
		for (int i = 0; i < billboards.Length; i++) {
			billboardTransforms [i] = billboards [i].transform;
		}
	}


	void Update() 
	{	
		iStart += 1;
		iStart %= iSkip;

		for (int i = iStart; i < billboardTransforms.Length; i += iSkip)
		{
			if (billboardTransforms [i] != null) {
				billboardTransforms [i].rotation = Camera.main.transform.rotation;
			}
		}
	}
}

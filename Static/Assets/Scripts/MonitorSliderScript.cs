using UnityEngine;
using System.Collections;

public class MonitorSliderScript : MonoBehaviour {

	public float minXPos;
	public float maxXPos;
	public float oscSpeed = 0.3f;
	
	void Update () {
		float newXPos = MyMath.Map (Mathf.Sin (Time.time*oscSpeed), -1f, 1f, minXPos, maxXPos);
		transform.localPosition = new Vector3 (newXPos, transform.localPosition.y, transform.localPosition.z);
	}
}

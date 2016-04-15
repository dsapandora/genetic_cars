using UnityEngine;
using System.Collections.Generic;

public class TimeScale : MonoBehaviour {
	
	static float lastTimeScaleBeforePause;
	static public void PP(){
		if (Time.timeScale != 0) {
			lastTimeScaleBeforePause = Time.timeScale;
		}
		Time.timeScale = Time.timeScale == 0 ? lastTimeScaleBeforePause : 0;
	}
	void Update(){
		if (Input.GetKeyDown (KeyCode.Space)) {
			PP ();
		} else if (Input.GetKeyDown (KeyCode.F9)) {
			Time.timeScale *= 2.0f;
			print ("Speed increased to " + Time.timeScale);
		} else if (Input.GetKeyDown (KeyCode.F7)) {
			Time.timeScale /= 2.0f;
			print ("Speed decreased to " + Time.timeScale);
		} else if (Input.GetKeyDown (KeyCode.F8)) {
			print ("eq");
			Time.timeScale = 1.0f;
		}
	}
}


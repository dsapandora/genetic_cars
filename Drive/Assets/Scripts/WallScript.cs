using UnityEngine;
using System.Collections;

public class WallScript : MonoBehaviour {


	void OnCollisionEnter(Collision other){
		GameObject otherObject = other.gameObject;
		if (otherObject.tag == "Car") {
			var carControl = otherObject.GetComponent<CarControls> ();
			carControl.getChromosome ().fitness -= 1;
			carControl.Death ();
		}
	}
}
using UnityEngine;
using System.Collections;

public class GeneRegionScript : MonoBehaviour {

	public int geneIndex;
	public float fitnessPoints;

	void Start(){
		gameObject.name = "GeneRegion " + geneIndex;
	}

	void OnTriggerEnter(Collider other){
		GameObject otherObject = other.gameObject;
		if (otherObject.tag == "Car") {
			var carControl = otherObject.GetComponent<CarControls> ();
			carControl.tryToActiveateGene (geneIndex, fitnessPoints);
		}
	}
}

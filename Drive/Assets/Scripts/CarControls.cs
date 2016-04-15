using UnityEngine;
using System.Collections;

public class CarControls : MonoBehaviour {

	private Rigidbody rb;

	public bool playerController = false;

	public float forwardMagnitude = 20;
	public float steeringMagnitude = 2;
	public float forcedBasedSteeringMagnitude = 40;

	private Chromosome chromosome;
	private bool alive;
	//private float timeFitness;
	public LevelLoader levelLoaderReference;

	private int currentActiveGene = -1;

	void Start () {
		rb = this.GetComponent<Rigidbody> ();
	}

	int lastActiveGene = -1;
	int geneHasNotChangedInThisManyPasses = 0;

	void FixedUpdate () {

		if (!playerController && !alive){ return; }

		float steering = 0;
		float vertical = 0;

		if (playerController) {
			steering = Input.GetAxis ("Horizontal");
			vertical = Input.GetAxis ("Vertical");
		} else 
		if (currentActiveGene > -1)
		{
			var geneIndex =	(currentActiveGene >= chromosome.speeds.Count ? chromosome.speeds.Count - 1 : currentActiveGene);
			steering = chromosome.steerings[geneIndex];
			vertical = chromosome.speeds[geneIndex];
		}

		var newForceVector = new Vector3(0, 0, -1 * vertical) * forwardMagnitude;
		rb.AddRelativeForce (newForceVector);
		transform.Rotate (0, steering * steeringMagnitude, 0);
	
		DieIfStale ();
	}

	private void DieIfStale(){
		if (lastActiveGene == currentActiveGene) {
			geneHasNotChangedInThisManyPasses++;
		} else {
//			print ("gene changed from " + lastActiveGene + " to " + currentActiveGene);
			lastActiveGene = currentActiveGene;
			geneHasNotChangedInThisManyPasses = 0;
		}
		if (geneHasNotChangedInThisManyPasses > 100) {
			Death ();
		}
	}

	public void tryToActiveateGene(int geneIndex, float fitnessPoints){
		if (playerController || chromosome.Equals(null) || !alive ) { return; }

		if (geneIndex > currentActiveGene && geneIndex - currentActiveGene < 10) {
			if (geneIndex > LevelLoader.geneTracking) {
				GameObject.Find ("GeneRegion " + geneIndex).GetComponent<Renderer> ().enabled = true;
				GameObject.Find ("GeneRegion " + geneIndex).GetComponent<Renderer> ().material.color = new Color (0.3f, 0.2f, 0.4f, 1f);
				GameObject.Find ("GeneRegion " + LevelLoader.geneTracking).GetComponent<Renderer> ().enabled = false;
				LevelLoader.geneTracking = geneIndex;
			}
//			print ("New Gene" + geneIndex);
			currentActiveGene = geneIndex;
			//fitness points!
			chromosome.fitness += fitnessPoints;
		} else {
//			print ("FAILED to change gene from " + currentActiveGene + " to " + geneIndex);
//			LevelLoader.PP ();
		}

	}

	public void setChromosome(Chromosome chr){
		alive = true;
		chr.fitness = 0.0f;
//		timeFitness = 0.0f;
		lastActiveGene = -1;
		currentActiveGene = -1;
		geneHasNotChangedInThisManyPasses = 0;
		this.chromosome = chr;
	}
	public Chromosome getChromosome(){
		return chromosome;
	}


	public void Death(){
		if (!alive || playerController) {
			return;
		}
			
		alive = false;
		if (levelLoaderReference != null) {

			var explosion = Instantiate (Resources.Load ("Explosion")) as GameObject;
			explosion.transform.Translate (this.gameObject.transform.position);

			levelLoaderReference.onCarDied (gameObject);
		}
	}
}

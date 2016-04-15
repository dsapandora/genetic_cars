using UnityEngine;
using System.Collections.Generic;

public class LevelLoader : MonoBehaviour {


	//Run variables
	public static int geneTracking = 0;
	public int ai = 1;
	public bool startFromLargestGen = false;
	public bool onlyRunChildren = false;
	public int gen = 276; //if set at -1, it will begin with a randomGeneration, set gen to 0 and go from there
	private int maxSimulatingAtATime = 30;

	void Start () {
		Time.timeScale = 15.0f;
		List<Vector2> path = getLevelMedium ();

		DrawGame (path);

		List<Chromosome> chromesomes;
		if (startFromLargestGen) {
			gen = GenInitializer.GetLatestGenFromFileSystem (ai);
		}

		if (gen == -1) {
			print ("start with random generation, from gen = 0");
			gen = 0;
			chromesomes = GenInitializer.CreateRandomGeneration (1000, numGenes);
		} else {
			print ("loading gen " + gen);
			chromesomes = GenInitializer.LoadGeneration (ai, gen);
		}
	
		BeginSimulation (chromesomes);
	}
		

	//set by DrawGame
	Segment[] levelSegments;
	int numGenes;


	List<Vector2> getLevelMedium(){
		
		List<Vector2> path2 = new List<Vector2>();
		path2.Add (new Vector2 (0, 0));
		path2.Add (new Vector2 (2, 0));
		path2.Add (new Vector2 (4, 2));
		path2.Add (new Vector2 (5, 5));
		path2.Add (new Vector2 (7, 7));
		path2.Add (new Vector2 (7, 9));
		path2.Add (new Vector2 (5, 10));
		path2.Add (new Vector2 (3, 8));
		path2.Add (new Vector2 (0, 6));
		path2.Add (new Vector2 (-2, 3));
		path2.Add (new Vector2 (-3, 1));
		path2.Add (new Vector2 (0, 0));
		List<Vector2> path = new List<Vector2>();
		path2.ForEach (delegate(Vector2 v) {
			path.Add(v * 10);
		});
		return path;
	}
		
	private List<Chromosome> populationNeedingSimulation;
	private List<Chromosome> populationBeingSimulated;
	private List<Chromosome> populationFinishedSimulation;
	private bool simulationActive = false;
	private float bestChromeFitness;
	private float worstNonChildChromosomeFitness;

	void BeginSimulation(List<Chromosome> chromosomes){

		bestChromeFitness = chromosomes [0].fitness;
		worstNonChildChromosomeFitness = chromosomes[0].fitness;

		chromosomes.ForEach (delegate(Chromosome chr) {
			if (chr.fitness > 0){
				if (chr.fitness <= worstNonChildChromosomeFitness)
				{
					worstNonChildChromosomeFitness = chr.fitness;
				} 
				if (chr.fitness >= bestChromeFitness)
				{
					bestChromeFitness = chr.fitness;
				}
			}
		});

		simulationActive = true;
		populationNeedingSimulation = new List<Chromosome> ();
		populationBeingSimulated = new List<Chromosome> ();
		populationFinishedSimulation = new List<Chromosome> ();

		chromosomes.ForEach (delegate(Chromosome chr) {
			if (onlyRunChildren) {
				if (chr.fitness == 0) {
					populationNeedingSimulation.Add (chr);
				}
				else 
					populationFinishedSimulation.Add(chr);
			} 
			else populationNeedingSimulation.Add(chr);
			
		});

		ContinueSimulating();
	}

	private void ContinueSimulating()
	{
		if (populationNeedingSimulation.Count == 0 && populationBeingSimulated.Count == 0) {
			simulationActive = false;
			var nextChromosomes = GenReporter.NextGenerationFrom (populationFinishedSimulation, ai, gen);
			gen++;
			BeginSimulation (nextChromosomes);
		}
		var numberCurrentlyBeingSimulated = populationBeingSimulated.Count;

		var numToTake = Mathf.Min (populationNeedingSimulation.Count, maxSimulatingAtATime - numberCurrentlyBeingSimulated);
		var startSeg = levelSegments [0];

		int i = 0; 
		while (i < numToTake) {
			Chromosome chr = populationNeedingSimulation [0];
			var newCar = createCar (chr);
			var driveScript = newCar.GetComponent<CarControls> ();
			driveScript.setChromosome (chr);
			newCar.transform.Translate (new Vector3 (startSeg.start.x, 0, startSeg.start.y));
			Vector2 lookat = startSeg.direction * -100 + startSeg.start;
			newCar.transform.LookAt (new Vector3 (lookat.x, 0, lookat.y));

			populationNeedingSimulation.RemoveAt (0);
			populationBeingSimulated.Add (chr);

			i++;
		}
	}
		

	void DrawGame(List<Vector2> path)
	{
		int length = path.Count -1;
		Segment[] segs = new Segment[length];
		for (int i = 0; i < length; i++) {
			segs [i] = new Segment (path [i], path [(i + 1) % path.Count]);
		}

		float halfWidth = 10;
		drawGeneRegions (segs, 4.0f, halfWidth*2);

		Segment[] upper = new Segment[segs.Length];
		Segment[] lower = new Segment[segs.Length];

		for (int i = 0; i < segs.Length; i++){
			Segment seg = segs [i];

			Vector2 UL = halfWidth * (new Vector2 (seg.direction.y * -1, seg.direction.x)) + seg.start;
			Vector2 UR = UL + (seg.direction * seg.distance);

//			drawPoint (UL, 0.7f);
//			drawPoint (UR, 0.7f);

			Vector2 LL = -halfWidth * (new Vector2 (seg.direction.y * -1, seg.direction.x)) + seg.start;
			Vector2 LR = LL + (seg.direction * seg.distance);

//			drawPoint (LL, 0.7f);
//			drawPoint (LR, 0.7f);

			upper [i] = new Segment (UL, UR);
			lower [i] = new Segment (LL, LR);
		}


		Vector2[] upperVectors = new Vector2[segs.Length + 1];
		Vector2[] lowerVectors = new Vector2[segs.Length + 1];

		upperVectors [0] = upper [0].start;
		lowerVectors [0] = lower [0].start;

		upperVectors [segs.Length] = upper [segs.Length-1].end;
		lowerVectors [segs.Length] = lower [segs.Length-1].end;

		for (int i = 0; i < upper.Length - 1; i++) {
			Segment up = upper [i];
			Segment nextUp = upper [i + 1];
			Vector2 upperIntercept = Segment.Intercept (up, nextUp);

			upperVectors [i + 1] = upperIntercept;

			Segment low = lower [i];
			Segment nextLow = lower [i + 1];
			Vector2 lowerIntercept = Segment.Intercept (low, nextLow);

			lowerVectors [i + 1] = lowerIntercept;
		}


//		for (int i = 0; i < upperVectors.Length; i++) {
//			drawPoint (upperVectors [i], 0.2f);
//		}
//		for (int i = 0; i < upperVectors.Length; i++) {
//			drawPoint (lowerVectors [i], 0.2f);
//		}


		placeWalls (upperVectors);
		placeWalls (lowerVectors);

		this.levelSegments = segs;
	}

	void drawGeneRegions(Segment[] segs, float segmentLength, float trackWidth)
	{	
		float totalTrackLength = 0.0f;
		for (int i = 0; i < segs.Length; i++) {
			totalTrackLength += segs [i].distance;
		}
		print ("Total track length is : " + totalTrackLength);

	
		float coveredTrackLength = 0.0f;
		int segmentIndex = 0;
		Vector2 step = new Vector2 (segs[segmentIndex].start.x, segs[segmentIndex].start.y);

		int numIterations = 0;
		while (coveredTrackLength < totalTrackLength && segmentIndex < segs.Length && numIterations < 20*1000) {

			float actualSegmentLength = segmentLength;

			Segment currentSegment = segs [segmentIndex];
			Vector2 nextStep = step + currentSegment.direction * segmentLength;

			float diffFromStart = Vector2.Distance (currentSegment.start, nextStep) - currentSegment.distance;

			bool iterateToNextAfterThisPass = false;
			if (diffFromStart > 0) {
				actualSegmentLength = (segmentLength - diffFromStart);
				nextStep = new Vector2 (currentSegment.end.x, currentSegment.end.y);
//				print ("ADJUSTED segment lenght from "+ segmentLength +" to " + actualSegmentLength);
				iterateToNextAfterThisPass = true;
			}

			GameObject gR = geneRegion ();

			Vector2 position = new Vector2 (nextStep.x, nextStep.y);
			position -= (currentSegment.direction * (actualSegmentLength/2));
			Vector3 worldPosition = new Vector3( position.x, -0.2f, position.y);

			GeneRegionScript grs = gR.GetComponent<GeneRegionScript> ();
			grs.geneIndex = numIterations;
			grs.fitnessPoints = segmentLength;
				
			gR.transform.Translate (worldPosition);
			gR.transform.localScale = new Vector3 (trackWidth, 1, actualSegmentLength - 0.1f);
			gR.transform.LookAt (new Vector3 (currentSegment.end.x, worldPosition.y, currentSegment.end.y));


			coveredTrackLength += actualSegmentLength;
			step = nextStep;
			numIterations++;

			if (iterateToNextAfterThisPass) {
				segmentIndex++;
				step = new Vector2 (currentSegment.end.x, currentSegment.end.y);
			}



		}
		numGenes = numIterations;
		print (" track encodes " + numGenes + " many genes");
	}

	void placeWalls(Vector2[] vectors) {
		for (int i = 0; i < vectors.Length-1; i++) {
			Segment seg = new Segment(vectors[i], vectors[i+1]);

			GameObject instance = createWall();
			instance.transform.localScale = new Vector3 (1, 5, seg.distance);
			Vector2 position = seg.start + (seg.distance / 2) * seg.direction;
			instance.transform.Translate (new Vector3 (position.x, 0, position.y));
			instance.transform.LookAt (new Vector3 (seg.end.x, 0.5f, seg.end.y));

		}
	}


	//debugging methods
	void drawPoint(Vector2 point){
		drawPoint (point, 1.0f);
	}
	void drawPoint(Vector2 point, float s){
		GameObject instance = createWall ();
		instance.transform.localScale = new Vector3(s, s, s);
		instance.transform.Translate (new Vector3 (point.x, 2.5f, point.y));
	}

	GameObject createWall()
	{
		return Instantiate (Resources.Load ("Wall")) as GameObject;
	}
	GameObject geneRegion()
	{
		return Instantiate (Resources.Load ("GeneRegion")) as GameObject;
	}

	private static Vector3 bestColor = new Vector3(0, 1, 0);
	private static Vector3 worstColor = new Vector3 (1, 0, 0);
	private static Vector3 unknownFitnessColor = new Vector3 (0, 0, 1);

	private static Vector3 colorDirection = (bestColor - worstColor).normalized;
	private static float colorMag = (worstColor - bestColor).magnitude;

	GameObject createCar(Chromosome currentChromosome)
	{
		var currentFitness = Mathf.Max (currentChromosome.fitness, 0);

		var maxFitness = bestChromeFitness;
		var minFitness = worstNonChildChromosomeFitness;
		var diffFitness = maxFitness - minFitness;
		float p;
		if (diffFitness > 0.0f) {
			p = ((currentFitness - minFitness) / diffFitness);
		} else {
			p = 0.0f;
		}

		p = Mathf.Min (p, 1.0f);
		var colorVect = (p * colorMag) * colorDirection + worstColor;
		if (currentFitness == 0)
		{
			colorVect = unknownFitnessColor;
		}
			
		var car = Instantiate(Resources.Load("CarGO")) as GameObject;
		car.transform.FindChild ("Cabin").GetComponent<Renderer> ().material.color = new Color (colorVect.x, colorVect.y, colorVect.z);

		car.GetComponent<CarControls>().levelLoaderReference = this;
		return car;
	}
	public void onCarDied(GameObject car){
		var chr = car.GetComponent<CarControls> ().getChromosome ();
		populationBeingSimulated.Remove (chr);
		populationFinishedSimulation.Add (chr);
		Destroy (car);
		ContinueSimulating ();
	}
}
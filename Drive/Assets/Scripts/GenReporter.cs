using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;

public class ChromComp : IComparer<Chromosome>
{
	public int Compare(Chromosome x, Chromosome y) {
		if (x.fitness < y.fitness) {
			return 1;
		} else if (x.fitness > y.fitness) {
			return -1;
		} else return 0;
	}
}


public class GenReporter : MonoBehaviour {
//	public static int gen = GenInitializer.gen;
//	public static int AI = ++GenInitializer.ai;
//	public List<Chromosome> chromosomes;

	public static bool SaveGeneration(List<Chromosome> chromosomes, int ai, int gen)
	{
		var aiPath = Application.dataPath + "/AI" + ai + "/";
		//CREAT GEN FILE IF IT DOES NOT ALREADY EXIST
		if (!System.IO.Directory.Exists(aiPath)) {
			System.IO.Directory.CreateDirectory (aiPath);
		}

		var outPath = aiPath + "Gen" + gen + ".json";
		if (!System.IO.File.Exists (outPath)) {
			var chromosomesStr = "{\"chromosomes\":[";
			for (var i = 0; i < chromosomes.Count; i++) {
				chromosomesStr += chromosomes [i].ToJSONStr () + (i == chromosomes.Count - 1 ? "" : ", ");
			}
			chromosomesStr += "]}";

			File.WriteAllText(outPath, chromosomesStr);
			print ("Wrote to file generation " + gen);
			return true;
		} else {
			print ("Generation not saved!  File already exists at " + outPath);
			return false;
		}

	}

	public static List<Chromosome> NextGenerationFrom(List<Chromosome> chromosomes, int ai, int gen) {
		chromosomes.Sort(new ChromComp ());
		print ("fitest of gen " + gen + " is " + chromosomes [0].fitness);
		SaveGeneration (chromosomes, ai, gen);

		//only take the best chromosomes
		chromosomes.RemoveRange(chromosomes.Count/2, chromosomes.Count/2);
		var babies = MakeBabyCars (chromosomes);
		MutateCars (babies);
		chromosomes.AddRange (babies);

		return chromosomes;
	}

	public static List<Chromosome> MakeBabyCars(List<Chromosome> chroms) {
		List<Chromosome> babies = new List<Chromosome> ();
		int rand;
		for (int i = 1; i < chroms.Count; i += 2) {
			rand = (int)Random.Range (0, chroms [i - 1].speeds.Count);
			babies.Add (createChild (chroms [i - 1], chroms [i], rand));
			babies.Add (createChild (chroms [i], chroms [i - 1], rand));
		}
		return babies;
	}

	public static void MutateCars(List<Chromosome> chroms){

		chroms.ForEach (delegate(Chromosome chr) {
			for (var i = 0; i < chr.speeds.Count; i++){	
				if ( Random.Range(1, 20) <= 1 )
				{
					var newSpeed = Random.Range(330, 1000)/1000.0f;
					var newSteering = Random.Range(-1000, 1000)/1000.0f;

					chr.speeds[i] = newSpeed;
					chr.steerings[i] = newSteering;
				}
			}
		});
	}
		
	public static Chromosome createChild (Chromosome c1, Chromosome c2, int rand){
		List<float> childSpeeds = c1.speeds.GetRange (0, rand);
		childSpeeds.AddRange(c2.speeds.GetRange(rand, c2.speeds.Count - rand));
		List<float> childSteerings = c1.steerings.GetRange (0, rand);
		childSteerings.AddRange(c2.steerings.GetRange (rand, c2.steerings.Count - rand));
		Chromosome child = new Chromosome (childSpeeds, childSteerings, 0);
		return child;
	}
}

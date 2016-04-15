using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.Collections;

public class Chromosome 
{
	public List<float> speeds;
	public List<float> steerings;
	public float fitness;

	public Chromosome(List<float> sp, List<float> st, float ft) {
		speeds = sp;
		steerings = st;
		fitness = ft;
	}
	public Chromosome(List<float> sp, List<float> st) {
		speeds = sp;
		steerings = st;
		fitness = 0;
	}
	public string ToJSONStr()
	{
		var str = "{\"speeds\":";
		var speedsStr = "[";
		for (var i = 0; i < speeds.Count; i++) {
			speedsStr += speeds[i] + (i == speeds.Count -1 ? "" : ", ");
		}
		speedsStr += "],";
		str += speedsStr;

		str += "\"steerings\":";
		var steeringsStr = "[";
		for (var i = 0; i < steerings.Count; i++){
			steeringsStr += steerings[i] + (i == steerings.Count - 1 ? "" : ", ");
		}
		steeringsStr += "],";
		str += steeringsStr;

		str += "\"fitness\":" + this.fitness;
		str += "}";
		return str;
	}

	public Chromosome(JSONNode chromJSON){
		speeds = new List<float>();
		steerings = new List<float>();

		for (int j = 0; j < chromJSON ["speeds"].Count; j++) {
			float speedHolder = chromJSON ["speeds"] [j].AsFloat;
			float steeringHolder = chromJSON ["steerings"] [j].AsFloat;

			speeds.Add (speedHolder);
			steerings.Add (steeringHolder);
		}
		fitness = chromJSON ["fitness"].AsFloat;
	}

	public Chromosome copy(){
		var steerings = new List<float>();
		var speeds = new List<float> ();

		this.steerings.ForEach (delegate(float s) {
			steerings.Add (s);
		});
		this.speeds.ForEach (delegate(float s) {
			speeds.Add (s);
		});

		return new Chromosome (speeds, steerings, this.fitness);
	}


		
}
public class GenInitializer : MonoBehaviour {
	

	public static string DirectoryPath(int ai){
		return Application.dataPath + "/AI" + ai;
	}
	public static List<Chromosome> LoadGeneration (int ai, int gen) {
		var chromosomes = new List<Chromosome> ();
		string json;

		using (StreamReader r = new StreamReader(DirectoryPath(ai) + "/Gen" + gen + ".json"))
		{
			json = r.ReadToEnd();
		}
		JSONNode jfile = JSON.Parse (json);

		for (int i = 0; i < jfile ["chromosomes"].Count; i++) {
			var chrom = new Chromosome( jfile["chromosomes"][i] );
			chromosomes.Add (chrom);
		}

		return chromosomes;
	}

	public static int GetLatestGenFromFileSystem(int ai){
		var gen = -1;
//		string[] fileArray = Directory.GetFiles
		string[] fileArray = Directory.GetFiles(DirectoryPath(ai), "*.json");
		print (fileArray);

		for (var i = 0; i < fileArray.Length; i++) {
			var filePath = fileArray [i];
			var file = filePath.Split ('/').Last();
			Match m = Regex.Match(file, @"\d+");
			int number = Convert.ToInt32(m.Value);
			if (number > gen) {
				gen = number;
			}
		}
		return gen;
	}

	public static List<Chromosome> CreateRandomGeneration(int population, int numGenesPerChromosome)
	{
		var chromosomes = new List<Chromosome> ();

		for (var i = 0; i < population; i++) {

			List<float> sp = new List<float> ();
			List<float> st = new List<float> ();

			for (var j = 0; j < numGenesPerChromosome; j++) {
				sp.Add (UnityEngine.Random.Range(0.0f, 1.0f));
				st.Add (UnityEngine.Random.Range(-1.0f, 1.0f));
			}

			chromosomes.Add (new Chromosome (sp, st));
		}

		return chromosomes;
	}

}

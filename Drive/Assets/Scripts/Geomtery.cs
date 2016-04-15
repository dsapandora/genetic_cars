using UnityEngine;
using System.Collections;

public class Segment
{
	public Vector2 start;
	public Vector2 end;
	public Vector2 direction;
	public float distance;

	public float A;
	public float B;
	public float C;

	public Segment(Vector2 start, Vector2 end) {
		this.start = start;
		this.end = end;
		this.distance = Vector2.Distance (end, start);
		this.direction = (end - start)/distance;

		A = end.y - start.y;
		B = start.x - end.x;
		C = A * start.x + B * start.y;

	}

	public static Vector2 Intercept(Segment first, Segment second){
		float det = first.A * second.B - second.A * first.B;
		if (det == 0) {
			//parallel ?
			return new Vector2(second.start.x, second.start.y);
		}
		else {
			float x = (second.B * first.C - first.B * second.C) / det;
			float y = (first.A * second.C - second.A * first.C) / det;
			return new Vector2 (x, y);
		}
	}
}
using System;
using UnityEngine;

public class FreeMathUtilities
{
	// Find the points where the two circles intersect.
	public static int FindCircleCircleIntersections(
		Vector2 pivot_1, float radius_1,
		Vector2 pivot_2, float radius_2,
		out Vector2 intersection_a, out Vector2 intersection_b)
		{
		// Find the distance between the centers.
		double dist = Vector2.Distance (pivot_1, pivot_2);

		// See how many solutions there are.
		if (dist > radius_1 + radius_2)
		{
			// No solutions, the circles are too far apart.
			intersection_a = new Vector2(float.NaN, float.NaN);
			intersection_b = new Vector2(float.NaN, float.NaN);
			return 0;
		}
		else if (dist < Math.Abs(radius_1 - radius_2))
		{
			// No solutions, one circle contains the other.
			intersection_a = new Vector2(float.NaN, float.NaN);
			intersection_b = new Vector2(float.NaN, float.NaN);
			return 0;
		}
		else if ((dist == 0) && (radius_1 == radius_2))
		{
			// No solutions, the circles coincide.
			intersection_a = new Vector2(float.NaN, float.NaN);
			intersection_b = new Vector2(float.NaN, float.NaN);
			return 0;
		}
		else
		{
			// Find a and h.
			double a = (radius_1 * radius_1 -
				radius_2 * radius_2 + dist * dist) / (2 * dist);
			double h = Math.Sqrt(radius_1 * radius_1 - a * a);

			// Find P2.
			double cx2 = pivot_1.x + a * (pivot_2.x - pivot_1.x) / dist;
			double cy2 = pivot_1.y + a * (pivot_2.y - pivot_1.y) / dist;

			// Get the points P3.
			intersection_a = new Vector2(
				(float)(cx2 + h * (pivot_2.y - pivot_1.y) / dist),
				(float)(cy2 - h * (pivot_2.x - pivot_1.x) / dist));
			intersection_b = new Vector2(
				(float)(cx2 - h * (pivot_2.y - pivot_1.y) / dist),
				(float)(cy2 + h * (pivot_2.x - pivot_1.x) / dist));

			// See if we have 1 or 2 solutions.
			if (dist == radius_1 + radius_2) {
				return 1;
			}

			return 2;
		}
	}
}



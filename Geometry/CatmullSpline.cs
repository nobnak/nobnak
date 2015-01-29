using UnityEngine;
using System.Collections;

namespace nobnak.Geometry {

	public static class CatmullSpline {

		public static Vector3 Position(float t, System.Func<int, Vector3> CP) {
			var i = Mathf.FloorToInt(t);
			t -= i;
			return Position(t, CP(i-1), CP(i), CP(i+1), CP(i+2));
		}
		public static Vector3 Velosity(float t, System.Func<int, Vector3> CP) {
			var i = Mathf.FloorToInt(t);
			t -= i;
			return Velosity(t, CP(i-1), CP(i), CP(i+1), CP(i+2));
		}

		public static Vector3 Position(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
			var tm1 = t - 1f;
			var tm2 = tm1 * tm1;
			var t2 = t * t;

			var m1 = 0.5f * (p2 - p0);
			var m2 = 0.5f * (p3 - p1);

			return (1f + 2f * t) * tm2 * p1 + t * tm2 * m1 + t2 * (3 - 2f * t) * p2 + t2 * tm1 * m2;
		}
		public static Vector3 Velosity(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
			var tm1 = (t - 1f);
			var t6tm1 = 6f * t * tm1;

			var m1 = 0.5f * (p2 - p0);
			var m2 = 0.5f * (p3 - p1);

			return t6tm1 * p1 + (3f * t - 1f) * tm1 * m1 - t6tm1 * p2 + t * (3f * t - 2f) * m2;
		}
	}
}
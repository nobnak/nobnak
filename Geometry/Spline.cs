using UnityEngine;
using System.Collections;

namespace nobnak.Geometry {

	public class Spline : MonoBehaviour {
		public const int GIZMO_SMOOTH_LEVEL = 10;
		public const float NORMALIZE_ANGLE = 1f / 360;

		public ControlPoint[] cps;	

		void OnDrawGizmos() {
			if (cps == null || cps.Length < 2)
				return;
			
			var dt = 1f / GIZMO_SMOOTH_LEVEL;
			var startPos = CatmullSplineUtil.Position(0f, GetCP);
			for (var i = 0; i < cps.Length; i++) {
				var t = (float)i;
				for (var j = 0; j < GIZMO_SMOOTH_LEVEL; j++) {
					var velocity = CatmullSplineUtil.Velosity(t, GetCP);
					Gizmos.color = Color.green;
					Gizmos.DrawLine(startPos, startPos + 0.5f * velocity.normalized);
					
					var endPos = CatmullSplineUtil.Position(t += dt, GetCP);
					Gizmos.color = Color.green;
					Gizmos.DrawLine(startPos, endPos);
					startPos = endPos;
				}
			}
			
			Gizmos.color = Color.white;
			for (var i = 0; i < cps.Length; i++)
				Gizmos.DrawCube(GetCP(i), 0.2f * Vector3.one);
		}
		
		public Vector3 Position(float t) { return CatmullSplineUtil.Position(t, GetCP); }
		public Vector3 Velosity(float t) { return CatmullSplineUtil.Velosity(t, GetCP); }
		public float Duration() {
			if (cps == null || cps.Length < 2)
				return 0f;
			return float.MaxValue;
		}
		public float Period() {
			return (cps == null || cps.Length < 2) ? 0f : cps.Length;
		}
		public Vector3 GetCP(int i) {
			while (i < 0)
				i += cps.Length;
			i = i % cps.Length;
			return cps[i].position;
		}
		
		[System.Serializable]
		public class ControlPoint {
			public Vector3 position;
		}
	}
}
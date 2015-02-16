﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using nobnak.Geometry;
using System.Text;
using nobnak.Config;

namespace nobnak.Geometry {

    [CustomEditor(typeof(Spline))]
    public class SplineEditor : Editor {
    	public const float CIRCLE_IN_RAD = 2f * Mathf.PI;
    	public const float SCALE_GUI = 0.05f;
    	public const float SCALE_PICK = 0.07f;
		public const float SCALE_POS = 0.5f;

		public const int GIZMO_SMOOTH_LEVEL = 10;
		public const float NORMALIZE_ANGLE = 1f / 360;

    	private int _iSelectedCP = -1;
    	private Tool _lastTool;

    	public override void OnInspectorGUI() {
    		DrawDefaultInspector();

            var curve = (Spline)target;
            var cps = (curve.cps == null) ? (curve.cps = new Spline.ControlPoint[0]) : curve.cps;
    		
    		EditorGUILayout.BeginHorizontal();
    		if (GUILayout.Button("Add")) {
				var size = 5f;
				var pos = (_iSelectedCP >= 0) ? curve.Position(_iSelectedCP + 0.5f)
					: (size * Random.onUnitSphere);
				var newCP = new Spline.ControlPoint(){ position = pos };
    			
    			if (0 <= _iSelectedCP && _iSelectedCP < cps.Length)
    				nobnak.Collection.Array.Insert(ref cps, newCP, _iSelectedCP + 1);
    			else
    				nobnak.Collection.Array.Insert(ref cps, newCP, cps.Length);
    			curve.cps = cps;
    			_iSelectedCP++;
                EditorUtility.SetDirty(curve);
    		}
    		if (GUILayout.Button("Remove")) {
    			if (0 <= _iSelectedCP && _iSelectedCP < cps.Length) {
    				nobnak.Collection.Array.Remove(ref cps, _iSelectedCP);
    			}
    			curve.cps = cps;
    			_iSelectedCP--;
    			EditorUtility.SetDirty(curve);
    		}
    		EditorGUILayout.EndHorizontal();
    	}
		void OnSceneGUI() {
            var curve = (Spline)target;
    		var rot = Quaternion.identity;

    		var cps = curve.cps;
    		if (cps != null && cps.Length > 0) {
    			for (var i = 0; i < cps.Length; i++) {
    				var cp = cps[i];
    				var cpPos = cp.position;
    				var size = HandleUtility.GetHandleSize(cpPos);
    				Handles.color = Color.white;
    				if (i == 0)
    					Handles.color = Color.green;
    				else if (i == (cps.Length - 1))
    					Handles.color = Color.red;
    				if (Handles.Button(cpPos, rot, size * SCALE_GUI, size * SCALE_PICK, Handles.DotCap)) {
    					_iSelectedCP = i;
    					Repaint();
    				}
    			}
    		}

			Spline.ControlPoint selectedCP = null;
    		if (0 <= _iSelectedCP && cps != null && _iSelectedCP < cps.Length)
    			selectedCP = cps[_iSelectedCP];
    		else
    			_iSelectedCP = -1;

    		if (selectedCP != null) {
    			EditorGUI.BeginChangeCheck();
    			var pos = Handles.DoPositionHandle(selectedCP.position, rot);
    			if (EditorGUI.EndChangeCheck()) {
    				selectedCP.position = pos;
    				EditorUtility.SetDirty(curve);
    			}
    		}
		}
		void OnEnable() {
			SceneView.onSceneGUIDelegate += DrawSceneGUI;
		}
		void OnDisable() {
			SceneView.onSceneGUIDelegate -= DrawSceneGUI;
		}

		void DrawSceneGUI(SceneView sceneView) { 
			DrawGizmos();
			OnSceneGUI(); 
		}
		void DrawGizmos() {
			var spline = (Spline)target;
			var cps = spline.cps;
			System.Func<int, Vector3> GetCP = spline.GetCP;

			if (cps == null || cps.Length < 2)
				return;
			
			var dt = 1f / GIZMO_SMOOTH_LEVEL;
			var startPos = CatmullSplineUtil.Position(0f, GetCP);
			for (var i = 0; i < cps.Length; i++) {
				var t = (float)i;
				for (var j = 0; j < GIZMO_SMOOTH_LEVEL; j++) {
					var velocity = CatmullSplineUtil.Velosity(t, spline.GetCP);
					Handles.color = Color.green;
					Handles.DrawLine(startPos, startPos + 0.5f * velocity.normalized);
					
					var endPos = CatmullSplineUtil.Position(t += dt, GetCP);
					Handles.color = Color.green;
					Handles.DrawLine(startPos, endPos);
					startPos = endPos;
				}
			}
		}
		[MenuItem("Assets/Create/Spline")]
		public static void CreateSpline() { ScriptableObjUtil.CreateAsset<Spline>(); }
    }
}
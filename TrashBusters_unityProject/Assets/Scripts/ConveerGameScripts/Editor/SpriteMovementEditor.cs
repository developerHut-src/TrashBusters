using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SpriteMovement))]
public class SpriteMovementEditor : Editor {
	SpriteMovement curTarget = null;
	Transform targetTransform;
	
	public override void OnInspectorGUI(){

		curTarget = target as SpriteMovement;
		if (curTarget && targetTransform == null)
			targetTransform = curTarget.transform;
		if (GUILayout.Button ("Add target point"))
			curTarget.AddTargetPoint(targetTransform.localPosition);
		GUILayout.Space (20);
		if (GUILayout.Button ("Reset target points"))
			curTarget.ResetTargetPoints ();
		DrawDefaultInspector ();
	}
}

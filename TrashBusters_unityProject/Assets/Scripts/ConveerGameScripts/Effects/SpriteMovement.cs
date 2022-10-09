using UnityEngine;
using System.Collections;

public class SpriteMovement : MonoBehaviour {
	Vector3 targetPos = Vector3.zero;
	[SerializeField]
	Vector3[] targetPoints = new Vector3[0];
	[SerializeField]
	float minSpeed = 2f,maxSpeed = 4f;
	float curTime,curSpeed,baseSpeed;
	Transform thisTransform;
	int curPointIndex = -1;
	[SerializeField]
	bool randomBetweenTargetPoints = false;
	Vector3 curCloudVelocity = Vector3.zero;

	// Use this for initialization
	void Start () {
		thisTransform = transform;
	}
	
	// Update is called once per frame
	void Update () {
		curTime = Time.time;
		if (thisTransform == null)
			return;
		if (targetPoints.Length < 1)
			return;
		if (Vector3.Distance (thisTransform.localPosition,targetPos)<0.1f || targetPos == Vector3.zero) {
			baseSpeed = Random.Range (minSpeed,maxSpeed);
			if(!randomBetweenTargetPoints){
				curPointIndex++;
				if(curPointIndex>=targetPoints.Length)
					curPointIndex = 0;
			}else curPointIndex = GetRandomTargetPoint();
			if(curPointIndex>-1)
				targetPos = targetPoints[curPointIndex];
		}
		if (targetPos != Vector3.zero) {
			curSpeed = baseSpeed * Mathf.Clamp(Vector3.Distance (thisTransform.localPosition, targetPos),0.3f,1f);
			thisTransform.localPosition = Vector3.MoveTowards(thisTransform.localPosition,targetPos,Time.deltaTime*curSpeed);
		}
	
	}


	int GetRandomTargetPoint(){
		int result = -1;
		for (int i =0; i<targetPoints.Length; i++) {
			if(curPointIndex != i){
				if(result<0)
					result = i;
				else if(Random.value>0.4f)
					result = i;
			}
		}
		return result;
	}

	public void AddTargetPoint(Vector3 newTargetPoint){
		if (targetPoints.Length < 1) {
			targetPoints = new Vector3[]{newTargetPoint};
			return;
		}
		ArrayList tempArray = new ArrayList ();
		for (int i=0; i<targetPoints.Length; i++) {
			tempArray.Add (targetPoints[i]);
		}
		tempArray.Add (newTargetPoint);
		if (tempArray.Count > 0)
			targetPoints = (Vector3[])tempArray.ToArray (typeof(Vector3));

	}


	public void ResetTargetPoints(){
		targetPoints = new Vector3[0];
	}
	
}

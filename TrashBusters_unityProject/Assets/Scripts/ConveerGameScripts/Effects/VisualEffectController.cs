using UnityEngine;
using System.Collections;

public class VisualEffectController : MonoBehaviour {
	public bool destructionCounterAtStart = true;
	public float duration = 1f;
	float effectEndTime = Mathf.Infinity;


	// Use this for initialization
	void OnEnable () {
		//Debug.Log ("OnEnable void executed");
		if(destructionCounterAtStart)
			effectEndTime = Time.time+duration;

	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time>effectEndTime){
			gameObject.SetActive (false);
			effectEndTime = Mathf.Infinity;
		}
	}

	public void StartDestructionCounter(){
		effectEndTime = Time.time+duration;
	}
}

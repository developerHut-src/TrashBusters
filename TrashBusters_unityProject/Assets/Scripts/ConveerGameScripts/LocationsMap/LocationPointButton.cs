using UnityEngine;
using System.Collections;

public class LocationPointButton : MonoBehaviour {
	public GameObject lightObject,locationNumberObject;
	public float loadingDelay = 1.5f,flashingInterval = 0.3f;
	float nextFlashTime = 0f,nextScreenLoadingTime = -1f;
	public float curTime;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(nextScreenLoadingTime<0)
			return;
		curTime = Time.time;
		if(curTime<nextScreenLoadingTime)
			LightFlashing ();
		else LoadNextScreen();
	}


	public void SelectLocationPoint(){
		nextScreenLoadingTime = Time.time+loadingDelay;
	}


	public void LightFlashing(){
		if(curTime>nextFlashTime){
			nextFlashTime = curTime+flashingInterval;
			if(lightObject.activeSelf){
				lightObject.SetActive(false);
				locationNumberObject.SetActive (false);
			}else{
				lightObject.SetActive(true);
				locationNumberObject.SetActive (true);
			}
		}
	}


	public void RestoreLight(){
		lightObject.SetActive(true);
		locationNumberObject.SetActive (true);
	}


	public void LoadNextScreen(){
		nextScreenLoadingTime = -1f;
		RestoreLight();
		if(GameInfo.mainMenuObject){
			GameInfo.mainMenuObject.SendMessage("ActivateMenu",5,SendMessageOptions.RequireReceiver);
		}else Debug.Log ("Can't load next screen: GameInfo.mainMenuObject is empty!");
	}
}

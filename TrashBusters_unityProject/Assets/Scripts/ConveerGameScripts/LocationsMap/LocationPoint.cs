using UnityEngine;
using System.Collections;

public class LocationPoint : MonoBehaviour {
	public int locationID = 0;
	public GameObject locPointObjectA,locPointObjectB,cloudsObject;


	void OnEnable(){
		UnlockLocationPoint (CanUnlockThisPoint());
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool CanUnlockThisPoint(){
		bool result = true;
		for(int i = 0;i<GameInfo.locationRatings[locationID].levelsRatings.Length;i++){
			if(GameInfo.locationRatings[locationID].levelsRatings[i].rating<1)
				result = false;
		}
		return result;
	}

	public void UnlockLocationPoint(bool unlock){
		if(locPointObjectA == null){
			Debug.Log ("Location point object A not found!");
		}else{
			locPointObjectA.SetActive(!unlock);
		}
		if(cloudsObject == null){
			Debug.Log ("Location point clouds object not found!");
		}else{
			cloudsObject.SetActive(!unlock);
		}
		if(locPointObjectB == null){
			Debug.Log ("Location point object B not found!");
		}else{
			locPointObjectB.SetActive(unlock);
		}
	}
}

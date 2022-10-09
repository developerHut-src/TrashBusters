using UnityEngine;
using System.Collections;

public class InfoEventsManager : MonoBehaviour {
	public GameObject learningDialogObject;
	int useLearningInfo = 0,sessionsPassed = 0;
	
	void OnEnable () {
		Conveer.OnLocationLevelLoaded+=OnLocationLevelLoaded;
		Conveer.OnItemOfTypeOnScreen+=OnItemOfTypeOnScreen;
		Conveer.OnObjectLost += OnItemLost;
	}
	

	// Update is called once per frame
	void Update () {
		CheckInfoEvents();
	}

	void OnLocationLevelLoaded(){
		ResetLearningStatus();
		Conveer.sendItemsOnScreenMessage = false;
		if(GameInfo.selectedLocationID ==0 && GameInfo.levelID ==0){
			if(GameInfo.infoMenu)
				GameInfo.infoMenu.ResetSessionsViews();
			useLearningInfo++;
			if(learningDialogObject){
				learningDialogObject.SetActive(true);
				GameInfo.SetGamePause(true);
			}
		}
		//Conveer.OnLocationLevelLoaded-=OnLocationLevelLoaded;
		//Debug.Log ("OnLocationLevelLoaded() has been called");
	}


	void OnItemOfTypeOnScreen(string itemType){
		if (useLearningInfo < 2)
			return;
		//Debug.Log ("Item of type '"+itemType+"' is on screen");
		if (itemType == "GoodBonuses" || itemType == "BadBonuses") {
			if (GameInfo.infoMenu.GetSessionViewTimes(5)<1) {
				GameInfo.infoMenu.ShowMessage (new int[]{5,6,7}, 0,false);
				sessionsPassed++;

			}
		} else if (itemType == "TypeD") {
			if (GameInfo.infoMenu.GetSessionViewTimes(3)<1) {
				GameInfo.infoMenu.ShowMessage (new int[]{3}, 0,false);
				sessionsPassed++;
			}
		}

	}


	void OnItemLost(){

	}


	void CheckInfoEvents(){
		if(useLearningInfo <2)
			return;
		if(GameInfo.infoMenu == null)
			return;
		if(Conveer.sendItemsOnScreenMessage == false)
			Conveer.sendItemsOnScreenMessage = true;

		if (GameInfo.infoMenu.GetSessionViewTimes (0) < 1) {
			GameInfo.infoMenu.ShowMessage (new int[]{0,1}, 0,false);
			sessionsPassed++;
			//infoEventActivated[0] = true;
		} else if (GameInfo.infoMenu.GetSessionViewTimes (2) < 1 && GameInfo.curRightPickedItems > 0) {
			GameInfo.infoMenu.ShowMessage (new int[]{2}, 0,false);
			sessionsPassed++;
			//infoEventActivated[1] = true;
		} else if (GameInfo.infoMenu.GetSessionViewTimes (4) < 1 && (GameInfo.curRightPickedItems > 5 || GameInfo.curLostObjectsOnLevel > 0)) {
			GameInfo.infoMenu.ShowMessage (new int[]{4}, 0,false);
			sessionsPassed++;
			//infoEventActivated[2] = true;
		} else if (sessionsPassed >= 5 && GameInfo.infoMenu.GetSessionViewTimes (8) < 1) {
			GameInfo.infoMenu.ShowMessage (new int[]{8}, 0,true);
			Conveer.sendItemsOnScreenMessage = false;
			ResetLearningStatus();
		}
	}
	


	public void ApproveLearning(int l){
		useLearningInfo+=l;
		useLearningInfo = Mathf.Clamp (useLearningInfo,0,2);
		GameInfo.SetGamePause(false);
	}


	public void ResetLearningStatus(){
		useLearningInfo = 0;
		sessionsPassed = 0;
	}
}

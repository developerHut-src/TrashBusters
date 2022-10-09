using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	public GameObject startMenuObject,settingsWindowObject,locationSelectionWindowObject,levelSelectionWindowObject,messageWindowObject,loadingScreenObject,ratingTableObject,menuBackgroundObject;
	public Transform levelsRoot,cellsRoot;
	public LevelsStore[] locationLevels = new LevelsStore[0];
	public int popupMenuID = -1;
	public float popupMenuTime = -1f;
	public LevelSelectionCell[] levelSelectionCells = new LevelSelectionCell[0];
	

	// Use this for initialization
	void Start () {
		ReadSelectionCells();
	}
	
	// Update is called once per frame
	void Update () {
	if(popupMenuID>-1 && popupMenuTime>=0){
			if(Time.time>popupMenuTime){
				popupMenuTime = -1f;
				ActivateMenu(0);
			}
		}
	}
	

	public void DisableAudio(bool disable){
		AudioListener.pause = disable;
		popupMenuTime = Time.time+3f;
	}


	public void SetTimeScale(float newTimeScale){
		Time.timeScale = newTimeScale;
	}


	public void SetVolume(float inc){
		float newVolumeValue = AudioListener.volume;
		newVolumeValue+=inc;
		newVolumeValue = Mathf.Clamp01(newVolumeValue);
		AudioListener.volume = newVolumeValue;
		popupMenuTime = Time.time+3f;
	}


	public void ActivateMenu(int menuID){
		if(menuID <0){
			if(startMenuObject)
				startMenuObject.SetActive(false);
            if (menuBackgroundObject)
                menuBackgroundObject.SetActive(false);
            if (settingsWindowObject)
				settingsWindowObject.SetActive(false);
			if(locationSelectionWindowObject)
				locationSelectionWindowObject.SetActive(false);
            if (levelSelectionWindowObject)
                levelSelectionWindowObject.SetActive(false);
            if (messageWindowObject)
				messageWindowObject.SetActive(false);
			if(loadingScreenObject)
				loadingScreenObject.SetActive(false);
			if(ratingTableObject)
				ratingTableObject.SetActive(false);
			if(GameInfo.gameMenuObject)
				GameInfo.gameMenuObject.SetActive(false);
			
		}else if(menuID == 0){
			UnloadLevel(-1);
			if(startMenuObject)
				startMenuObject.SetActive(true);
            if (menuBackgroundObject)
                menuBackgroundObject.SetActive(true);
            if (settingsWindowObject)
					settingsWindowObject.SetActive(false);
				if(locationSelectionWindowObject)
					locationSelectionWindowObject.SetActive(false);
            if (levelSelectionWindowObject)
                levelSelectionWindowObject.SetActive(false);
            if (messageWindowObject)
					messageWindowObject.SetActive(false);
				if(loadingScreenObject)
					loadingScreenObject.SetActive(false);
			if(ratingTableObject)
				ratingTableObject.SetActive(false);
			if(GameInfo.gameMenuObject)
				GameInfo.gameMenuObject.SetActive(false);
			
		}else if(menuID ==1){
			if(startMenuObject)
				startMenuObject.SetActive(false);
            if (menuBackgroundObject)
                menuBackgroundObject.SetActive(false);
            if (settingsWindowObject)
					settingsWindowObject.SetActive(false);
				if(locationSelectionWindowObject)
					locationSelectionWindowObject.SetActive(true);
            if (levelSelectionWindowObject)
                levelSelectionWindowObject.SetActive(false);
				if(messageWindowObject)
					messageWindowObject.SetActive(false);
				if(loadingScreenObject)
					loadingScreenObject.SetActive(false);
			if(ratingTableObject)
				ratingTableObject.SetActive(false);
			
		}else if(menuID ==2){
			if(startMenuObject)
				startMenuObject.SetActive(false);
            if (menuBackgroundObject)
                menuBackgroundObject.SetActive(false);
            if (settingsWindowObject)
				settingsWindowObject.SetActive(false);
			if(locationSelectionWindowObject)
				locationSelectionWindowObject.SetActive(false);
            if (levelSelectionWindowObject)
                levelSelectionWindowObject.SetActive(false);
            if (messageWindowObject)
				messageWindowObject.SetActive(false);
			if(loadingScreenObject)
				loadingScreenObject.SetActive(true);
			if(ratingTableObject)
				ratingTableObject.SetActive(false);
			
		}else if(menuID ==3){
			if(startMenuObject)
				startMenuObject.SetActive(false);
            if (settingsWindowObject)
				settingsWindowObject.SetActive(true);
			if(locationSelectionWindowObject)
				locationSelectionWindowObject.SetActive(false);
            if (levelSelectionWindowObject)
                levelSelectionWindowObject.SetActive(false);
            if (messageWindowObject)
				messageWindowObject.SetActive(false);
			if(loadingScreenObject)
				loadingScreenObject.SetActive(false);
			if(ratingTableObject)
				ratingTableObject.SetActive(false);
			popupMenuID = menuID;
			popupMenuTime = Time.time+3f;	
		}else if(menuID ==4){
			if(startMenuObject)
				startMenuObject.SetActive(false);
            if (menuBackgroundObject)
                menuBackgroundObject.SetActive(false);
            if (settingsWindowObject)
				settingsWindowObject.SetActive(false);
			if(locationSelectionWindowObject)
				locationSelectionWindowObject.SetActive(false);
            if (levelSelectionWindowObject)
                levelSelectionWindowObject.SetActive(false);
            if (messageWindowObject)
				messageWindowObject.SetActive(false);
			if(loadingScreenObject)
				loadingScreenObject.SetActive(false);
			if(ratingTableObject)
				ratingTableObject.SetActive(true);
			
		}else if (menuID == 5)
        {
            if (startMenuObject)
                startMenuObject.SetActive(false);
            if (menuBackgroundObject)
                menuBackgroundObject.SetActive(true);
            if (settingsWindowObject)
                settingsWindowObject.SetActive(false);
            if (locationSelectionWindowObject)
                locationSelectionWindowObject.SetActive(false);
            if (levelSelectionWindowObject)
                levelSelectionWindowObject.SetActive(true);
            SetSelectionCellsStatus();
            if (messageWindowObject)
                messageWindowObject.SetActive(false);
            if (loadingScreenObject)
                loadingScreenObject.SetActive(false);
            if (ratingTableObject)
                ratingTableObject.SetActive(false);

        }
    }


	public void LoadLevel(int locationID, int levelID){
		if(levelsRoot==null){
			Debug.Log ("Can't load any level: level's root transform is not assigned!");
			return;
		}
		if(locationLevels.Length<1){
			Debug.Log ("Can't load any level: array of referenced levels is empty!");
			return;
		}
		if(locationLevels.Length<=locationID){
			Debug.Log ("Can't load any level: locationLevels.Length<=locationID!");
			return;
		}
		if(locationLevels[locationID].levelsTransforms.Length<=levelID){
			Debug.Log ("Can't load any level: locationLevels[locationID].levelsTransforms.Length<=levelID!");
			return;
		}
		//GameInfo.ResetCurretRatingData();
		UnloadLevel(-1);
		ActivateMenu(2);
		Transform newLevel = Instantiate (locationLevels[locationID].levelsTransforms[levelID],Vector3.zero,Quaternion.identity) as Transform;
		newLevel.SetParent(levelsRoot);
		newLevel.localPosition = Vector3.zero;
		newLevel.name = locationLevels[locationID].levelsTransforms[levelID].name;
	}

		/*
	public void LoadLevel(int preInc, int postInc,bool autoReturnToMainMenu){
		if(levelsRoot==null){
			Debug.Log ("Can't load any level: level's root transform is not assigned!");
			return;
		}
		if(refLevels.Length<1){
			Debug.Log ("Can't load any level: array of referenced levels is empty!");
			return;
		}
		//GameInfo.ResetCurretRatingData();
		GameInfo.selectedLocationID +=preInc;
		Debug.Log ("Try to load level "+GameInfo.selectedLocationID);
		if((refLevels.Length-1)<GameInfo.selectedLocationID || GameInfo.selectedLocationID<0){
			Debug.Log ("Can't load any level: selectedLocationID<0 or selectedLocationID> refLevels.Length-1!");
			if(autoReturnToMainMenu)
				ActivateMenu(0);
			return;
		}
		UnloadLevel(-1);
		ActivateMenu(2);
		Transform newLevel = Instantiate (refLevels[GameInfo.selectedLocationID],Vector3.zero,Quaternion.identity) as Transform;
		newLevel.SetParent(levelsRoot);
		newLevel.localPosition = Vector3.zero;
		newLevel.name = refLevels[GameInfo.selectedLocationID].name;
		GameInfo.selectedLocationID+=postInc;
	}
*/

	public void SelectLocation(int locationID){
		GameInfo.selectedLocationID = locationID;
       // ActivateMenu(5);
	}

	public void SelectLocationLevel(int levelID){
		GameInfo.levelID = levelID;
	}


	public void LoadSelectedLevel(){
		LoadLevel (GameInfo.selectedLocationID,GameInfo.levelID);
	}


    public void LoadNextLevel()
    {
        if (locationLevels.Length < 1)
        {
            Debug.Log("Can't load next level: locationLevels.Length<1!");
            return;
        }
        if (GameInfo.selectedLocationID < 0 || GameInfo.selectedLocationID >= locationLevels.Length)
        {
            Debug.Log("Can't load next level: Invalid GameInfo.selectedLocationID !");
            return;
        }
        int nextLevelID = GameInfo.levelID+1;
        if (locationLevels[GameInfo.selectedLocationID].levelsTransforms.Length <= nextLevelID)
            ActivateMenu(0);
        else
        {
            GameInfo.levelID = nextLevelID;
			GameInfo.SetRatingForLevel(GameInfo.selectedLocationID,GameInfo.levelID,0);
            LoadLevel(GameInfo.selectedLocationID, GameInfo.levelID);
        }

    }


	public void UnloadLevel(int levelID){
		if(levelsRoot==null){
			Debug.Log ("Can't unload any level: level's root transform is not assigned!");
			return;
		}
		if(levelsRoot.childCount>0){
			Transform curLevel = null;
			if(levelID<0){
				for(int i=0;i<levelsRoot.childCount;i++){
					curLevel = levelsRoot.GetChild(i);
					if(curLevel){
						Destroy(curLevel.gameObject);
					}
				}
			}else{
				curLevel = levelsRoot.GetChild(levelID);
				if(curLevel){
					Destroy(curLevel.gameObject);
				}
			}
		}else Debug.Log ("All levels are already unloaded!");
	}
	

	public void QuitFromGame(){
		Application.Quit ();
	}


	[System.Serializable]
	public class LevelSelectionCell
	{
		public Transform ratingLineTransform;
		public Transform selectionButtonTransform;
		public Transform accessInfoTransform;

		public void SetAccess(bool open){
			if(open == false){
				if(ratingLineTransform)
					ratingLineTransform.gameObject.SetActive(false);
				if(selectionButtonTransform)
					selectionButtonTransform.gameObject.SetActive(false);
				if(accessInfoTransform)
					accessInfoTransform.gameObject.SetActive(true);
			}else{
				if(ratingLineTransform)
					ratingLineTransform.gameObject.SetActive(true);
				if(selectionButtonTransform)
					selectionButtonTransform.gameObject.SetActive(true);
				if(accessInfoTransform)
					accessInfoTransform.gameObject.SetActive(false);
			}
		}


		public void SetRating(int rating){
			if(ratingLineTransform == null){
				Debug.Log ("Can't set rating for cell: ratingLineTransform is not assigned!");
				return;
			}
			if(ratingLineTransform.childCount<1){
				Debug.Log ("Can't set rating for cell: no one star founded in rating line!");
				return;
			}
			Transform curChild = null;
			for(int i=0;i<ratingLineTransform.childCount;i++){
				curChild = ratingLineTransform.GetChild (i);
				if(i<rating){
					curChild.gameObject.SetActive(true);
					Debug.Log ("star: "+curChild.name+" activated!");
				}else{
					curChild.gameObject.SetActive(false);
					Debug.Log ("star: "+curChild.name+" deactivated!");
				}
			}

		}
	}


	public void ReadSelectionCells(){
		if(cellsRoot == null){
			Debug.Log ("Can't read selection cells: cells root not found!");
			return;
		}
		if(cellsRoot.childCount<1){
			Debug.Log ("Can't read selection cells: cells root child count <1 !");
			return;
		}
		levelSelectionCells = new LevelSelectionCell[cellsRoot.childCount];
		for(int i=0;i<levelSelectionCells.Length;i++){
			levelSelectionCells[i] = new LevelSelectionCell();
			levelSelectionCells[i].selectionButtonTransform = cellsRoot.GetChild (i).Find("SelectionButton");
			levelSelectionCells[i].ratingLineTransform = cellsRoot.GetChild (i).Find("RatingInfo");
			levelSelectionCells[i].accessInfoTransform = cellsRoot.GetChild (i).Find("AccessInfo");
		}
	}


	public void SetSelectionCellsStatus(){
		if(levelSelectionCells.Length<1){
			Debug.Log ("Can't set status for selection cells: levelSelectionCells.Length<1!");
			return;
		}
		if(GameInfo.locationRatings[GameInfo.selectedLocationID].levelsRatings.Length != levelSelectionCells.Length){
			Debug.Log ("Can't set status for selection cells: levelSelectionCells.Length !=GameInfo.levelsRatings.Length, their length must be the same !");
			return;
		}
		for(int i=0;i<levelSelectionCells.Length;i++){
			if(levelSelectionCells[i] == null){
				Debug.Log ("level selection cell "+i+" is empty!");
			}else{
				if(GameInfo.locationRatings[GameInfo.selectedLocationID].levelsRatings[i].rating>-1){
					levelSelectionCells[i].SetRating(GameInfo.locationRatings[GameInfo.selectedLocationID].levelsRatings[i].rating);
					levelSelectionCells[i].SetAccess(true);
				}else{
					levelSelectionCells[i].SetAccess(false);
				}
			}
		}
	}


	[System.Serializable]
	public class LevelsStore
	{
		public Transform[] levelsTransforms = new Transform[0];
	}
}

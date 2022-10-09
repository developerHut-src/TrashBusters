using UnityEngine;
using System.Collections;

public class GameInfo : MonoBehaviour {
	public static bool gameOver = false,levelComplete = false;
	public GameObject refGameMenuButtonObject,refGameMenuObject,refRatingTableObject,refLevelsRootObject,refMainMenuObject,
	refNextStageInfoObject;
	public StatusBar refStatusBar;
	public BonusesBar refBonusesBar;
	public InfoMenu refInfoMenu;
	public LocationLevelsRatings[] refLocationRatings = new LocationLevelsRatings[5];
	public static GameObject gameMenuButtonObject,mainMenuObject,gameMenuObject,ratingTableObject,levelsRootObject,nextStageInfoObject;
	public static LocationLevelsRatings[] locationRatings = new LocationLevelsRatings[5];
    public static int selectedLocationID = 0,subLevelID = 0,levelID = 0;
	public static int maxLostObjectsOnLevel = 0, curLostObjectsOnLevel = 0, curWrongPickedItems = 0,curRightPickedItems = 0;
	public static StatusBar statusBar;
	public static bool pause = false;
	public static BonusesBar bonusesBar;
	public static InfoMenu infoMenu;

	// Use this for initialization
	void Awake () {

		if(refGameMenuButtonObject)
			gameMenuButtonObject = refGameMenuButtonObject;
		if(refGameMenuObject)
			gameMenuObject = refGameMenuObject;
		if(refRatingTableObject)
			ratingTableObject = refRatingTableObject;
		if(refMainMenuObject)
			mainMenuObject = refMainMenuObject;
		if(refLevelsRootObject)
			levelsRootObject = refLevelsRootObject;
		if(refNextStageInfoObject)
			nextStageInfoObject = refNextStageInfoObject;
		if(refStatusBar)
			statusBar = refStatusBar;
		if(refBonusesBar)
			bonusesBar = refBonusesBar;
		if(refInfoMenu)
			infoMenu = refInfoMenu;
		locationRatings = refLocationRatings;
	}


	void OnEnable(){
		Serializer.LoadGameData(refLocationRatings);
		locationRatings = refLocationRatings;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public static void SetGamePause(bool newPause){
		if(newPause){
			Time.timeScale = 0f;
		}else Time.timeScale = 1f;
		pause = newPause;
	}


	public static void UpdateLocationsRatings(LocationLevelsRatings[] newLocationLevelsRatings){
		locationRatings = newLocationLevelsRatings;
	}

	
	public static void ResetCurretRatingData(){
		maxLostObjectsOnLevel = curLostObjectsOnLevel = curWrongPickedItems = 0;
		subLevelID = 0;
	}

	public static void ResetStatusBar(){
		if(statusBar){
			statusBar.ResetStatusBar();
			//statusBar.RenewHealthUnits();
		}

	}


	public static void ResetCurrentItemCounters(){
		curLostObjectsOnLevel = curWrongPickedItems = curRightPickedItems = 0;
	}


	public static void SetGameOver(bool newGameOver){
		if(ratingTableObject)
			ratingTableObject.SetActive(newGameOver);
		if(gameMenuButtonObject)
			gameMenuButtonObject.SetActive(!newGameOver);
		gameOver = newGameOver;
	}


	public static void SetRatingForLevel(int locationID,int levelID,int rating){
		if(locationRatings[locationID].levelsRatings[levelID] == null)
            locationRatings[locationID].levelsRatings[levelID] = new LevelRating();
		if(locationRatings[locationID].levelsRatings[levelID].rating<rating)
            locationRatings[locationID].levelsRatings[levelID].rating = rating;
		Serializer.SaveGameData(locationRatings);
	}


	public static int GetLevelRating(){
		int result = 0;
		if(curLostObjectsOnLevel>=maxLostObjectsOnLevel || statusBar.activeHealthUnits<1)
			return result;
		Debug.Log ("cur lost objects:"+curLostObjectsOnLevel+" cur wrong objects:"+curWrongPickedItems);
		if(curLostObjectsOnLevel == 0 && curWrongPickedItems == 0)
			result = 3;
		else if(curLostObjectsOnLevel>0 && curWrongPickedItems>0)
			result = 1;
		else if(curLostObjectsOnLevel>0 || curWrongPickedItems>0)
			result = 2;
		return result;
		
	}


	[System.Serializable]
	public class LevelRating
	{
		public int rating = -1;
	}


    [System.Serializable]
    public class LocationLevelsRatings
    {
        public LevelRating[] levelsRatings = new LevelRating[5];
    }

}

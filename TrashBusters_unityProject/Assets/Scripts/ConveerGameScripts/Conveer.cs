/*=================================== made by Vladimir SILENT Maevskiy============================
 * ===============================================================================================*/
using UnityEngine;
using System.Collections;

public class Conveer : MonoBehaviour {
	public float spawnAreaMinY = 0f,spawnAreaMaxY = 0f,minPlacementDistance,maxPlacementDistance;
	public float baseLineMovementSpeed,objectsAttractionSpeed = 30f,curLineMovementSpeed;
	public int conveerMaxSize = 10,movementDirection = 1,maxLostObjects = 2,lostObjects = 0,desiredQuantityOfPickedItems = 70;
	public int totalPickedRightObjects = 0,totalPickedWrongObjects = 0,totalPassedObjects = 0;
	public ButtonVisualEffects[] buttonsEffects = new ButtonVisualEffects[0];
	public ConveerButton[] buttons = new ConveerButton[0];
	public ConveerObjectsStore[] objectsStores = new ConveerObjectsStore[0];
	public Vector3 lastObjectPos = Vector3.zero,curObjectPos = Vector3.zero;
	VirtualConveerObject[] conveerArray = new VirtualConveerObject[0];
	public RectTransform respawnBorder,spawnPoint;
	public AudioClip[] bugSounds = new AudioClip[0];
	public Transform subLevel;
	public bool isInitialLevel = false;
	float lastObjectRadius = 0f;
	Vector3 tempV3;
	bool skipPlacementOfItemsOfCurrentType = false;
	Transform thisTransform;
	VirtualConveerObject lastSpawnedObject = new VirtualConveerObject(-1,-1);
	public float nextStageInfoObjectTime = -2f;
	public float resetLineMovementSpeedTime = -1f,canEatEverythingUntilTime = -1f,bonusChance = 0.5f;
	public string[] bonusesNames = new string[5];
	//range of good bonuses in the bonusesNames array : from 0 to goodBonusesRange - good bonuses,rest items - bad bonuses
	public int goodBonusesRange = 2;
	public ConveerLine conveerLine = new ConveerLine ();
	public delegate void LocationLevelLoaded();
	public delegate void ItemOfTypeOnScreen(string itemName);
	public delegate void ObjectLost();
	public static event ObjectLost OnObjectLost;
	public static event ItemOfTypeOnScreen OnItemOfTypeOnScreen;
	public static event LocationLevelLoaded OnLocationLevelLoaded;
	public static bool sendItemsOnScreenMessage = false;
	Rect screenRect;


	// Use this for initialization
	void OnEnable () {
		thisTransform = transform;
		InitializeStoresObjects();
		InitializeConveerArray();
		//if(isInitialLevel){
			GameInfo.ResetStatusBar();
			GameInfo.ResetCurretRatingData();
			GameInfo.ResetCurrentItemCounters();
			if(GameInfo.mainMenuObject)
				GameInfo.mainMenuObject.SendMessage("ActivateMenu",-1,SendMessageOptions.RequireReceiver);
			if(GameInfo.gameMenuObject)
				GameInfo.gameMenuObject.SetActive(true);
			GameInfo.SetGameOver(false);
			SentRatingDataToGameInfo(0);
		//}
		//if(GameInfo.statusBar)
			//GameInfo.statusBar.ResetStatusBar();
		curLineMovementSpeed = baseLineMovementSpeed;
		//initiate level loaded event
		if(OnLocationLevelLoaded!=null)
			OnLocationLevelLoaded();
		screenRect = new Rect(0,0,Screen.width,Screen.height);

	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(GameInfo.gameOver)
			return;
		/*
		if(nextStageInfoObjectTime>0){
			GameInfo.pause = true;
			ShowNextStageInfoObject();
		}else if(GameInfo.curLostObjectsOnLevel>=GameInfo.maxLostObjectsOnLevel || IsSubLevelComplete() || GameInfo.curWrongPickedItems>=GameInfo.maxWrongPickedItems){
			LoadNewSubLevelOrFinishCurrentLevel();
		}
		*/
		if(GameInfo.curLostObjectsOnLevel>=GameInfo.maxLostObjectsOnLevel  || GameInfo.statusBar.activeHealthUnits<1 || IsSubLevelComplete()){
			FinishCurrentLevel();
		}
		if(GameInfo.pause == false)
		MoveConveerObjects();
		AttractObjectsToButtons();
		if(resetLineMovementSpeedTime>0 && Time.time>resetLineMovementSpeedTime)
			ResetLineMovementSpeed();
		if(canEatEverythingUntilTime>0f && Time.time>canEatEverythingUntilTime)
			canEatEverythingUntilTime = -1f;
	}



	public void CatchConveerObject(int buttonID){
		if(GameInfo.gameOver)
			return;

		if(conveerArray.Length<1){
			Debug.Log ("Nothing to catch: conveer array length<1!");
			return;
		}
		bool itemCatched = false;
        Vector3 bugLookDir = buttons[buttonID].buttonRectTransform.up*-1;
        Vector3 captureDir;
        //if(Mathf.Abs (buttons[buttonID].buttonRectTransform.localPosition.x 
        // -objectsStores[conveerArray[i].typeID].storeObjects[conveerArray[i].objectID].rectTransform.localPosition.x)< buttons[buttonID].catchWidthMultiplier && itemCatched == false){
        for (int i=0;i<conveerArray.Length;i++){
            captureDir= objectsStores[conveerArray[i].typeID].storeObjects[conveerArray[i].objectID].rectTransform.localPosition - buttons[buttonID].buttonRectTransform.localPosition;
            if (Vector3.Angle(bugLookDir,captureDir)<buttons[buttonID].catchWidthMultiplier && itemCatched==false){
				objectsStores[conveerArray[i].typeID].storeObjects[conveerArray[i].objectID].attractingToButton = buttonID;
				BugAttractItemAnimation(buttonID);
				itemCatched = true;
			}
		}

	}


	public void PlayButtonEffect(int buttonID, int effectID){
		if(buttonsEffects.Length<=buttonID){
			Debug.Log ("Can't play effect "+effectID+" for button "+buttonID+": button effects length<= buttonID!");
			return;
		}
			if(buttonsEffects[buttonID].effects.Length>0){
				if(buttonsEffects[buttonID].effects[effectID].gameObject !=null){
					buttonsEffects[buttonID].effects[effectID].gameObject.SetActive(true);
				}
			}
		
	}


	public void BugSendBubble(int buttonID,int effectID){
		if(buttons[buttonID].animator){
			buttons[buttonID].animator.Play ("BugBubble",-1,0f);
		}
		PlayButtonEffect(buttonID,effectID);
	}


	public void ShowScoreEffect(int buttonID,int effectID){
		if(buttonsEffects[buttonID].effects[effectID].animator){
			buttonsEffects[buttonID].effects[effectID].animator.Play ("ScoreAnimation",-1,0f);
		}
		PlayButtonEffect(buttonID,effectID);
	}


	public void BugAttractItemAnimation(int buttonID){
		if(buttons[buttonID].animator){
			buttons[buttonID].animator.Play ("ItemAttracting",-1,0f);
		}
	}


	public void MoveConveerObjects(){
		if(objectsStores.Length<1){
			Debug.Log ("Can't move conveer objects: objects stores length<1!");
			return;
		}
		if(conveerArray.Length<1){
			Debug.Log ("Can't move conveer objects: conveer array length<1!");
			return;
		}

		for(int i=0;i<conveerArray.Length;i++){
			if(conveerArray[i].typeID<objectsStores.Length){
				if(objectsStores[conveerArray[i].typeID]!=null){
					if(objectsStores[conveerArray[i].typeID].storeObjects.Length>0 && conveerArray[i].objectID<objectsStores[conveerArray[i].typeID].storeObjects.Length){
						if(objectsStores[conveerArray[i].typeID].storeObjects[conveerArray[i].objectID].attractingToButton<0){
							curObjectPos = objectsStores[conveerArray[i].typeID].storeObjects[conveerArray[i].objectID].rectTransform.localPosition;
							//curObjectPos.x-=lineMovementSpeed*Time.deltaTime*-1;
							curObjectPos = Vector3.MoveTowards (curObjectPos,respawnBorder.localPosition,curLineMovementSpeed*Time.deltaTime);
							objectsStores[conveerArray[i].typeID].storeObjects[conveerArray[i].objectID].rectTransform.localPosition = curObjectPos;
							if(ItemIsOnScreen(i)){
								if(OnItemOfTypeOnScreen!=null && sendItemsOnScreenMessage)
									OnItemOfTypeOnScreen(objectsStores[conveerArray[i].typeID].storeObjects[conveerArray[i].objectID].rectTransform.parent.name);
							}
								//Debug.Log (objectsStores[conveerArray[i].typeID].storeObjects[conveerArray[i].objectID].rectTransform.name+" appears on screen");
							RespawnObjectIfRequires(i);
						}
					}else Debug.Log ("3");
				}else Debug.Log ("2");
			}else Debug.Log ("ID of the objects store of the item "+i+" of conveer array larger or equal to objects stores length!");
		}
		//Animate conveer line
		if (conveerLine!=null) {
			if(conveerLine.IsValidToAnimate())
				conveerLine.MoveConveerLine(curLineMovementSpeed,Time.deltaTime);
		}
	}


	public void AttractObjectsToButtons(){
		if(buttons.Length<1)
			return;
		if(conveerArray.Length<1)
			return;
		Vector3 newObjectPos = Vector3.zero,newObjectScale = Vector3.zero;
		int buttonID;
		for(int i =0;i<conveerArray.Length;i++){
			if(objectsStores[conveerArray[i].typeID].storeObjects[conveerArray[i].objectID].attractingToButton<0)
				continue;
			if(buttons[objectsStores[conveerArray[i].typeID].storeObjects[conveerArray[i].objectID].attractingToButton].attractionPivot == null)
				continue;
			buttonID = objectsStores[conveerArray[i].typeID].storeObjects[conveerArray[i].objectID].attractingToButton;
			newObjectPos = objectsStores[conveerArray[i].typeID].storeObjects[conveerArray[i].objectID].rectTransform.position;
			newObjectScale = objectsStores[conveerArray[i].typeID].storeObjects[conveerArray[i].objectID].rectTransform.localScale;
			objectsStores[conveerArray[i].typeID].storeObjects[conveerArray[i].objectID].rectTransform.position = Vector3.Lerp(newObjectPos,
			                                                                                                                   buttons[buttonID].attractionPivot.position,Time.deltaTime*objectsAttractionSpeed);
			objectsStores[conveerArray[i].typeID].storeObjects[conveerArray[i].objectID].rectTransform.localScale = Vector3.Lerp(newObjectScale,Vector3.zero,Time.deltaTime*objectsAttractionSpeed);
            float distanceToObject = Vector3.Distance(objectsStores[conveerArray[i].typeID].storeObjects[conveerArray[i].objectID].rectTransform.position, buttons[buttonID].attractionPivot.position);
            if (distanceToObject < 3f)
            {
                objectsStores[conveerArray[i].typeID].storeObjects[conveerArray[i].objectID].SetActivity(false);
                objectsStores[conveerArray[i].typeID].storeObjects[conveerArray[i].objectID].attractingToButton = -1;
                objectsStores[conveerArray[i].typeID].storeObjects[conveerArray[i].objectID].rectTransform.localScale = new Vector3(1, 1, 1);
				int currentItemType = ItemIsBonus (i);
				if(currentItemType>-1){
					//GetBonusID(objectsStores[conveerArray[i].typeID].storeObjects[conveerArray[i].objectID].rectTransform.name)
					ApplyBonus(GetRandomBonusID(currentItemType),buttonID); //+GetRandomBonusID(currentItemType)
					if(currentItemType == 0)
						PlayBugSound(buttonID, 4);
					else if(currentItemType == 1)
						PlayBugSound(buttonID, 5);
				}else if(canEatEverythingUntilTime>0f)
					ApproveItemForButton(buttonID);
				else{
					if (buttons[buttonID].buttonType == objectsStores[conveerArray[i].typeID].storeObjects[conveerArray[i].objectID].rectTransform.parent.name)
						ApproveItemForButton(buttonID);
                	else
						DeclineItemForButton(buttonID);
				 
				}
                //Debug.Log ("Object "+conveerArray[i].objectID+" of type "+conveerArray[i].typeID+" has been attracted!");
            }
            //else Debug.Log("Distance to current object:" + distanceToObject);
		}
	}


	//approve currently cathed item for desired button
	public void ApproveItemForButton(int buttonID){
		totalPickedRightObjects++;
		if (GameInfo.statusBar)
			GameInfo.statusBar.SetScoreText((totalPickedRightObjects * 20).ToString());
		buttons[buttonID].rightCatchesCounter++;
		buttons[buttonID].buttonText.text = buttons[buttonID].rightCatchesCounter.ToString();
		GameInfo.curRightPickedItems = buttons[buttonID].rightCatchesCounter;
		PlayBugSound(buttonID, 0);
		if (IsAliquotTo(5, buttons[buttonID].rightCatchesCounter))
		{
			BugSendBubble(buttonID, 0);
			PlayBugSound(buttonID, 2);
		}
		ShowScoreEffect(buttonID, 3);
	}


	int ItemIsBonus(int virtualObjectID){
		int result = -1;
		if(objectsStores[conveerArray[virtualObjectID].typeID].storeObjects[conveerArray[virtualObjectID].objectID].rectTransform.parent.name == "GoodBonuses")
			result = 0;
		 if(objectsStores[conveerArray[virtualObjectID].typeID].storeObjects[conveerArray[virtualObjectID].objectID].rectTransform.parent.name == "BadBonuses")
			result = 1;
		return result;
	}
	

	bool ItemIsOnScreen(int virtualObjectID){
		bool result = true;
		Vector3 [] itemCorners = new Vector3[4];
		objectsStores [conveerArray [virtualObjectID].typeID].storeObjects [conveerArray [virtualObjectID].objectID].rectTransform.GetWorldCorners (itemCorners);
		for (int i=0; i<itemCorners.Length; i++) {
			if (!screenRect.Contains (itemCorners[i])) {
				result = false;

			}
		}
		return result;
	}


	int GetRandomBonusID(int bonusType){
		int result = -1,min,max;
		if(bonusesNames.Length<1){
			return result;
		}
		goodBonusesRange = Mathf.Clamp (goodBonusesRange,0,bonusesNames.Length-1);
			min = 0;
			max = bonusesNames.Length-1;
		if(bonusType == 0){
			min = 0;
			max = goodBonusesRange;
		}else if(bonusType == 1){
			min = goodBonusesRange+1;
			max = bonusesNames.Length-1;
		}
		result = Random.Range (min,max);
		return result;
	}


	public void DeclineItemForButton(int buttonID){
		totalPickedWrongObjects++;
		GameInfo.curWrongPickedItems++;
		buttons[buttonID].wrongCatchesCounter++;
		PlayButtonEffect(buttonID, 1);
		PlayBugSound(buttonID, 1);
		if (GameInfo.statusBar)
			GameInfo.statusBar.DestroyHealthUnits(1);
		if (GameInfo.statusBar.activeHealthUnits<1)
		{
			PlayButtonEffect(buttonID, 2);
			PlayBugSound(buttonID, 3);
			SetButtonsActive(false, buttonID);
		}
	}


	public void ApplyBonus(int bonusID,int buttonID){
		if(bonusID == 0){
			canEatEverythingUntilTime = Time.time+15f;
			if(GameInfo.bonusesBar)
				GameInfo.bonusesBar.SetBonus(bonusesNames[bonusID],15f,buttons[buttonID].buttonRectTransform.position);
		}else if(bonusID == 1){
			SetConveerSpeed(baseLineMovementSpeed*0.5f,5f);
			if(GameInfo.bonusesBar)
				GameInfo.bonusesBar.SetBonus(bonusesNames[bonusID],5f,buttons[buttonID].buttonRectTransform.position);
		}else if(bonusID == 2){
			if (GameInfo.statusBar)
				GameInfo.statusBar.AddHealthUnits(1);
			if(GameInfo.bonusesBar)
				GameInfo.bonusesBar.SetBonus(bonusesNames[bonusID],-1f,buttons[buttonID].buttonRectTransform.position);
		}else if(bonusID == 3){
			SetConveerSpeed(baseLineMovementSpeed*2f,10f);
			if(GameInfo.bonusesBar)
				GameInfo.bonusesBar.SetBonus(bonusesNames[bonusID],10f,buttons[buttonID].buttonRectTransform.position);
		}else if(bonusID == 4){
			if (GameInfo.statusBar)
				GameInfo.statusBar.AddHealthUnits(-1);
			if(GameInfo.bonusesBar)
				GameInfo.bonusesBar.SetBonus(bonusesNames[bonusID],-1f,buttons[buttonID].buttonRectTransform.position);
		}
	}


	int GetBonusID(string bonusName){
		int result = -1;
		if(bonusesNames.Length<1)
			return result;
		for(int i=0;i<bonusesNames.Length;i++){
			if(bonusName == bonusesNames[i]){
				result = i;
			}
		}
		return result;
	}


	public void InitializeStoresObjects(){
		if(objectsStores.Length<1)
			return;
		for(int i=0;i<objectsStores.Length;i++){
			if(objectsStores[i]!=null){
				objectsStores[i].ReadConveerObjects(objectsStores[i].activeAtStart);
			}
		}
	}


	public void RespawnObjectIfRequires(int virtualObjectID){
		if(respawnBorder == null){
			Debug.Log ("Can't check position of object for respawn: respawnBorder RectTransform is not assigned!");
			return;
		}
		if(spawnPoint == null){
			Debug.Log ("Can't check position of object for respawn: spawnPoint RectTransform is not assigned!");
			return;
		}
		//float distanceToSpawnBorder = 0f, distanceToSpawnPoint = 0f;
		//distanceToSpawnBorder = respawnBorder.localPosition.x
		bool spawnBorderReached = false;

			//spawnBorderReached = objectsStores[conveerArray[virtualObjectID].typeID].storeObjects[conveerArray[virtualObjectID].objectID].rectTransform.localPosition.x>respawnBorder.localPosition.x;
		spawnBorderReached = Vector3.Distance(objectsStores[conveerArray[virtualObjectID].typeID].storeObjects[conveerArray[virtualObjectID].objectID].rectTransform.localPosition,respawnBorder.localPosition)<0.1f;

		if(spawnBorderReached || objectsStores[conveerArray[virtualObjectID].typeID].storeObjects[conveerArray[virtualObjectID].objectID].active == false){
			if(objectsStores[conveerArray[virtualObjectID].typeID].storeObjects[conveerArray[virtualObjectID].objectID].active && objectsStores[conveerArray[virtualObjectID].typeID].objectsShouldBeCatched == 1){
				lostObjects++;
				GameInfo.curLostObjectsOnLevel++;
				if(OnObjectLost!=null)
					OnObjectLost();
				if(GameInfo.infoMenu)
					GameInfo.infoMenu.ShowPopUpMessage(0,1);
				
			}else if(objectsStores[conveerArray[virtualObjectID].typeID].storeObjects[conveerArray[virtualObjectID].objectID].active && objectsStores[conveerArray[virtualObjectID].typeID].objectsShouldBeCatched == 0){
				if(GameInfo.infoMenu)
					GameInfo.infoMenu.ShowPopUpMessage(0,0);
				
			}
			objectsStores[conveerArray[virtualObjectID].typeID].storeObjects[conveerArray[virtualObjectID].objectID].SetActivity(false);
			conveerArray[virtualObjectID] = GetRandomInactiveObject();
			if(ItemIsBonus(virtualObjectID)>-1 && Random.value>bonusChance){
				//Debug.Log ("Spawning of bonus has been skipped!");
				return;
			}
			objectsStores[conveerArray[virtualObjectID].typeID].storeObjects[conveerArray[virtualObjectID].objectID].SetActivity(true);
			if(lastSpawnedObject.ObjectDataValid() == false)
				lastSpawnedObject = new VirtualConveerObject(conveerArray[conveerArray.Length-1].typeID,conveerArray[conveerArray.Length-1].objectID);
			lastObjectPos = objectsStores[lastSpawnedObject.typeID].storeObjects[lastSpawnedObject.objectID].rectTransform.localPosition;
			lastObjectPos+= (spawnPoint.localPosition-respawnBorder.localPosition).normalized*Random.Range(minPlacementDistance,maxPlacementDistance);
			float baseDist = Vector3.Distance (spawnPoint.localPosition,respawnBorder.localPosition);
			float curDist = Vector3.Distance(lastObjectPos,respawnBorder.localPosition);
			if(curDist<baseDist)
				lastObjectPos = spawnPoint.localPosition;
			objectsStores[conveerArray[virtualObjectID].typeID].storeObjects[conveerArray[virtualObjectID].objectID].rectTransform.localPosition = lastObjectPos;
			//objectsStores[conveerArray[virtualObjectID].typeID].storeObjects[conveerArray[virtualObjectID].objectID].rectTransform.localEulerAngles = new Vector3(0,0,Random.Range(0,360));
			lastSpawnedObject.typeID = conveerArray[virtualObjectID].typeID;
			lastSpawnedObject.objectID = conveerArray[virtualObjectID].objectID;
			totalPassedObjects++;
		}
	}


	public void InitializeConveerArray(){
		if(objectsStores.Length<1){
			Debug.Log ("Can't make conveer array: objects stores are empty!");
			return;
		}
		if(spawnPoint == null){
			Debug.Log ("Can't make conveer array: spawn point not assigned!");
			return;
		}
		if(respawnBorder == null){
			Debug.Log ("Can't make conveer array: respawn border not assigned!");
			return;
		}
		ArrayList tempArray = new ArrayList();
		VirtualConveerObject tempObject = new VirtualConveerObject(-1,-1);
		Vector3 newObjectPos = spawnPoint.localPosition;
		for(int i=0;i<conveerMaxSize;i++){
			tempObject = GetRandomInactiveObject();
			if(tempObject.ObjectDataValid()){
				tempArray.Add (tempObject);
				objectsStores[tempObject.typeID].storeObjects[tempObject.objectID].SetActivity(true);
				objectsStores[tempObject.typeID].storeObjects[tempObject.objectID].rectTransform.localPosition = newObjectPos;
				newObjectPos += (spawnPoint.localPosition-respawnBorder.localPosition).normalized*Random.Range(minPlacementDistance,maxPlacementDistance);
				//newObjectPos = new Vector3(newObjectPos.x+Random.Range(minPlacementDistance,maxPlacementDistance)*-1,Random.Range(spawnAreaMinY,spawnAreaMaxY),0);
				//Debug.Log ("'"+objectsStores[tempObject.typeID].storeObjects[tempObject.objectID].gameObject.name+"' has been added");
			}
		}
		if(tempArray.Count>0)
			conveerArray = (VirtualConveerObject[]) tempArray.ToArray(typeof(VirtualConveerObject));
		else Debug.Log ("Can't initialize conveer array : no one inactive object of objectsStores were founded!");
	}


	public VirtualConveerObject GetRandomInactiveObject(){
		VirtualConveerObject result = new VirtualConveerObject(-1,-1);
		if(objectsStores.Length<1)
			return result;
		ArrayList inactiveObjects = new ArrayList();
		for(int i=0;i<objectsStores.Length;i++){
			if(objectsStores[i] == null){
				Debug.Log ("Objects store "+i+" is empty!");
				continue;
			}
			if(objectsStores[i].storeObjects.Length>0){
				for(int j=0;j<objectsStores[i].storeObjects.Length;j++){
					if(objectsStores[i].storeObjects[j].active == false){
						inactiveObjects.Add (new VirtualConveerObject(i,j));
					}
				}
			}else Debug.Log ("Objects store "+i+" store objects list is empty!");
		}
		if(inactiveObjects.Count>0){
			VirtualConveerObject[] temp = (VirtualConveerObject[]) inactiveObjects.ToArray(typeof(VirtualConveerObject));
			int randomIndex =  Random.Range(0,temp.Length);
			randomIndex = Mathf.Clamp (randomIndex,0,temp.Length-1);
			result = temp[randomIndex];
		}
		return result;

	}


	public void ResetButtonsCounters(){
		if(buttons.Length<1){
			Debug.Log ("Can't reset button counters: buttons array length <1!");
			return;
		}
		for(int i=0;i<buttons.Length;i++){
			buttons[i].wrongCatchesCounter = 0;
			buttons[i].rightCatchesCounter = 0;
			if(buttons[i].buttonText)
				buttons[i].buttonText.text = "0";
		}
	}


	public bool IsAliquotTo(int aliqoutValue,int curValue){
		bool result = false;
		int temp = curValue/aliqoutValue;
		if(temp>=1){
			if((curValue - temp*aliqoutValue) == 0)
				result = true;
		}
		return result;
	}
	

	public void SetConveerSpeed(float newSpeed){
		curLineMovementSpeed = newSpeed;
	}


	public void SetConveerSpeed(float newSpeed,float durationTime){
		resetLineMovementSpeedTime = Time.time+durationTime;
		curLineMovementSpeed = newSpeed;
	}


	public void ResetLineMovementSpeed(){
		curLineMovementSpeed = baseLineMovementSpeed;
		resetLineMovementSpeedTime = -1f;
	}


	public void SetTimeForEatingAll(float duration){
		canEatEverythingUntilTime = Time.time+duration;
	}


	public void SetItemsDistance(float newMaxPlacementDistance){
		maxPlacementDistance = newMaxPlacementDistance;
	}


	public void SetMaxLostObjects(float newMaxLostObjects){
		maxLostObjects = (int)newMaxLostObjects;
	}
	


	public void RestartConveer(){
		lastObjectPos = Vector3.zero;
		curObjectPos = Vector3.zero;
		lastSpawnedObject = new VirtualConveerObject(-1,-1);
		lostObjects = 0;
		InitializeStoresObjects();
		InitializeConveerArray();
		ResetButtonsCounters();
		SetButtonsActive(true,-1);
		GameInfo.SetGameOver(false);
		GameInfo.ResetCurrentItemCounters();
		totalPickedRightObjects = 0;
		totalPickedWrongObjects =0;
		curLineMovementSpeed = baseLineMovementSpeed;
	}


	public void SetButtonsActive(bool active,int buttonID){
		if(buttons.Length<1){
			Debug.Log ("Can't set buttons activity: buttons.Length<1!");
			return ;
		}
		if(buttonID<0){
			for(int i=0;i<buttons.Length;i++){
				buttons[i].buttonRectTransform.gameObject.SetActive (active);
			}
		}else{
			buttons[buttonID].buttonRectTransform.gameObject.SetActive (active);
		}
	}


	public bool IsSubLevelComplete(){
		bool result = false;
		if(totalPickedRightObjects>=desiredQuantityOfPickedItems)
			result = true;
		return result;
	}

	

	public void PlayBugSound(int buttonID,int soundID){
		if(bugSounds.Length<1 && soundID>=bugSounds.Length){
			Debug.Log ("Can't play bug "+buttonID+" sound "+soundID+" : bugSounds.Length<1 or invalid soundID ("+soundID+" !"); 
			return;
		}
		if(buttons[buttonID].audio){
			buttons[buttonID].audio.PlayOneShot(bugSounds[soundID]);
		}
	}


	public void SentRatingDataToGameInfo(int dataID){
		if(dataID ==0){
			GameInfo.maxLostObjectsOnLevel+=maxLostObjects;
		}else if(dataID == 1){
			//GameInfo.curWrongPickedItems += totalPickedWrongObjects;
			//GameInfo.curLostObjectsOnLevel += lostObjects;
		}
	}


	/*
	public void LoadNewSubLevelOrFinishCurrentLevel(){
		Debug.Log ("GameInfo: picked wrong items = "+GameInfo.curWrongPickedItems+"; max picked wrong items = "+GameInfo.maxWrongPickedItems+
		           "; lost objects = "+GameInfo.curLostObjectsOnLevel+"; max lost objects = "+GameInfo.maxLostObjectsOnLevel);
		if( subLevel!=null && GameInfo.curLostObjectsOnLevel<GameInfo.maxLostObjectsOnLevel && GameInfo.curWrongPickedItems<GameInfo.maxWrongPickedItems){
			nextStageInfoObjectTime = Time.time+3f;
			if(GameInfo.nextStageInfoObject)
				GameInfo.nextStageInfoObject.SetActive(true);
		}else{
			FinishCurrentLevel();
		}
	}


	public void LoadNewSubLevel(){
		if(GameInfo.levelsRootObject == null){
			Debug.Log ("Can't load level: GameInfo.levelsRootObject = null !");
			return;
		}
		Debug.Log ("Sub level loading function executed");
		if(subLevel != null ){
			if(GameInfo.mainMenuObject)
				GameInfo.mainMenuObject.SendMessage("UnloadLevel",-1,SendMessageOptions.DontRequireReceiver);
			Transform instSubLevel = Instantiate (subLevel,Vector3.zero,Quaternion.identity) as Transform;
			instSubLevel.name = subLevel.name;
			instSubLevel.SetParent (GameInfo.levelsRootObject.transform);
			GameInfo.subLevelID++;
			GameInfo.pause = false;
		}
	}


	public void ShowNextStageInfoObject(){
	 if(Time.time>nextStageInfoObjectTime){
			if(GameInfo.nextStageInfoObject)
				GameInfo.nextStageInfoObject.SetActive(false);
			//nextStageInfoObjectTime = -2f;
			LoadNewSubLevel();
		}
	}
	*/

	
	public void FinishCurrentLevel(){
		GameInfo.pause = false;
		//SentRatingDataToGameInfo(1);
		int curRating =GameInfo.GetLevelRating(); 
		Debug.Log ("Cur level rating:"+curRating);
		GameInfo.SetRatingForLevel(GameInfo.selectedLocationID,GameInfo.levelID,GameInfo.GetLevelRating());
		GameInfo.SetGameOver(true);
		if(GameInfo.ratingTableObject)
			GameInfo.ratingTableObject.SendMessage("SetRating",curRating);
	}


	[System.Serializable]
	public class ConveerLinePart
	{
		public Transform lineRoot;
		public bool objectsActiveAtStart = true;
		public ConveerObjectsStore[] objectsStores = new ConveerObjectsStore[0];
	}


	[System.Serializable]
	public class ConveerObjectsStore
	{
		public bool activeAtStart = false;
		//object's "should be catched flag": - 1 - neutral, i.e may be cathced or just skipped;0- should't be catched;1 - should be catched.
		public int objectsShouldBeCatched = -1;
		public Transform storeRoot;
		public ConveerObject[] storeObjects = new ConveerObject[0];

		public void ReadConveerObjects(bool activeAtStart){
			if(storeRoot == null){
				Debug.Log ("Conveer objects root not found!");
				return;
			}
			
			if(storeRoot.childCount<1){
				Debug.Log ("Conveer objects root child count <1!");
				return;
			}
			storeObjects = new ConveerObject[storeRoot.childCount];
			
			for(int i=0;i<storeObjects.Length;i++){
				storeObjects[i] = new ConveerObject(storeRoot.GetChild (i).gameObject,storeRoot.GetChild (i).GetComponent<RectTransform>(),activeAtStart);
				storeObjects[i].gameObject.SetActive(activeAtStart);
			}
			
		}
	}


	[System.Serializable]
	public class ConveerObject
	{
		public GameObject gameObject;
		public RectTransform rectTransform;
		public bool active = true;
		public int attractingToButton = -1;

		public ConveerObject(GameObject newGameObject,RectTransform newRectTransform,bool newActive){
			gameObject = newGameObject;
			rectTransform = newRectTransform;
			active = newActive;
		}

		public void SetActivity(bool activity){
			if(gameObject == null){
				Debug.Log ("gameObject of this conveer object is empty!");
				return;
			}

			if(active == activity)
				return;
			gameObject.SetActive(activity);
			active = activity;
		}
	}



	public struct VirtualConveerObject
	{
		public int typeID ;
		public int objectID ;

		public VirtualConveerObject(int newTypeID,int newObjectID){
			typeID = newTypeID;
			objectID = newObjectID;
		}

		public bool ObjectDataValid(){
			bool result = true;
			if(typeID <0 || objectID<0)
				result = false;
			return result;
		}
	}


	[System.Serializable]
	public class ConveerButton
	{
		public string buttonType = "";
		public float catchWidthMultiplier = 1f;
		public RectTransform buttonRectTransform;
		public RectTransform attractionPivot;
		public UnityEngine.UI.Text buttonText;
		public int rightCatchesCounter = 0,wrongCatchesCounter;
		public Animator animator;
		public AudioSource audio;
	}


	[System.Serializable]
	public class VisualEffect
	{
		public GameObject gameObject;
		public RectTransform rectTransform;
		public Animator animator;
	}


	[System.Serializable]
	public class ButtonVisualEffects
	{
		public VisualEffect[] effects = new VisualEffect[0];
	}


	[System.Serializable]
	public class ConveerLine
	{
		[SerializeField]
		Transform[] conveerLineParts = new Transform[0];
		[SerializeField]
		Transform spawnPointTransform,resetPointTransform;

		public void MoveConveerLine(float speed,float deltaTime){
			if (conveerLineParts.Length < 1)
				return;
			if (spawnPointTransform == null || resetPointTransform == null)
				return;
			for (int i=0; i<conveerLineParts.Length; i++) {
				conveerLineParts[i].localPosition = Vector3.MoveTowards(conveerLineParts[i].localPosition,resetPointTransform.localPosition,speed*deltaTime);
				if(Vector3.Distance(conveerLineParts[i].localPosition,resetPointTransform.localPosition)<0.1f)
					conveerLineParts[i].localPosition = spawnPointTransform.localPosition;
			}
		}


		public bool IsValidToAnimate(){
			bool result = false;
			if (conveerLineParts.Length < 1)
				return result;
			if (spawnPointTransform == null || resetPointTransform == null)
				return result;
			result = true;
			return result;
		}

	}
}

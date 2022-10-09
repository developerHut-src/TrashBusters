using UnityEngine;
using System.Collections;

public class BonusesBar : MonoBehaviour {
	public float bonusMovementSpeed = 1f,bonusReachDistance = 1f;
	public BonusID[] bonusesIDs = new BonusID[0];
	public Transform slotsTransformsRoot, reserveBonusesTransformsRoot;
	public RectTransform outOfScreenRectTransform;
	public BonusSlot[] bonusesSlots = new BonusSlot[0];
	public BonusSlot[] reserveBonusesSlots = new BonusSlot[0];
	public Vector3 targetBonusScale = new Vector3(1,1,1),initialBonusScale = new Vector3(1.5f,1.5f,1.5f);

	// Use this for initialization
	void Start () {
		ReadSlotsTransformsAndBonuses();
		ReadReserveBonuses();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateBonusesPosition();
		UpdateBonusesStatus();
		UpdateReserveBonuses();
	}


	public void SetBonus(string bonusName,float activityTime){
		int bonusID = -1,bonusSlotID = -1;
		bonusID = GetBonusID (bonusName);
		bonusSlotID = GetSlotWithBonus(bonusName);
		if(bonusSlotID<0)
			bonusSlotID = GetFirstFreeSlotID();
		if(bonusSlotID>-1)
			bonusesSlots[bonusSlotID].bonusObject.ActivateBonus(bonusID,Time.time+activityTime);
		else Debug.Log ("Can't set bonus '"+bonusName+"' for bonuses bar: bonusSlotID<0!");
	}


	public void SetBonus(string bonusName,float activityTime,Vector3 initialPos){
		int bonusID = -1,bonusSlotID = -1;
		bonusID = GetBonusID (bonusName);
		bonusSlotID = GetSlotWithBonus(bonusName);
		if(bonusSlotID<0)
			bonusSlotID = GetFirstFreeSlotID();
		if(bonusSlotID>-1){
			bonusesSlots[bonusSlotID].bonusObject.bonusRectTransform.position = initialPos;
			bonusesSlots[bonusSlotID].bonusObject.bonusRectTransform.localScale = initialBonusScale;
			bonusesSlots[bonusSlotID].bonusObject.ActivateBonus(bonusID,Time.time+activityTime);
		}else Debug.Log ("Can't set bonus '"+bonusName+"' for bonuses bar: bonusSlotID<0!");
	}
	


	int GetBonusID(string bonusName){
		int result = -1;
		if(bonusesIDs.Length<1){
			return result;
		}
		for(int i =0;i<bonusesIDs.Length;i++){
			if(bonusesIDs[i]._bonusName == bonusName){
				result = bonusesIDs[i]._bonusID;
				break;
			}
		}
		return result;
	}


	void UpdateBonusesStatus(){
		if(bonusesSlots.Length<1)
			return;
		for(int i=0;i<bonusesSlots.Length;i++){
if(bonusesSlots[i].bonusObject.GetActiveBonusID()>=0 && Time.time>bonusesSlots[i].bonusObject.deactivationTime && bonusesSlots[i].bonusObject.deactivationTime>-1f){
				int freeReserveBonusID = GetFreeReserveBonusID();
				if(freeReserveBonusID>-1){
					reserveBonusesSlots[freeReserveBonusID].bonusObject.ActivateBonus(bonusesSlots[i].bonusObject.GetActiveBonusID());
					reserveBonusesSlots[freeReserveBonusID].bonusObject.bonusRectTransform.position = bonusesSlots[i].bonusObject.bonusRectTransform.position;
					reserveBonusesSlots[freeReserveBonusID].bonusObject.bonusRectTransform.localScale = bonusesSlots[i].bonusObject.bonusRectTransform.localScale;
				}
				bonusesSlots[i].bonusObject.DisableBonusObject();
				bonusesSlots[i].reserved = false;
			}

			if(bonusesSlots[i].bonusObject.GetActiveBonusID()<0){
				BonusObject tempBonusObject = bonusesSlots[i].bonusObject;

				for(int j=i+1;j<bonusesSlots.Length;j++){
					if(bonusesSlots[j].bonusObject.GetActiveBonusID()>=0){
						bonusesSlots[i].bonusObject = bonusesSlots[j].bonusObject;
						bonusesSlots[i].bonusObject.SetParent(bonusesSlots[i].slotTransform);
						bonusesSlots[j].bonusObject = tempBonusObject;
						bonusesSlots[j].bonusObject.SetParent(bonusesSlots[j].slotTransform);
						break;
					}
				}
			}
			
		}

	}


	void UpdateBonusesPosition(){
		if(bonusesSlots.Length<1)
			return;
		for(int i=0;i<bonusesSlots.Length;i++){
			if(bonusesSlots[i].bonusObject.ShouldBeMoved(bonusesSlots[i].slotTransform.position,bonusReachDistance) && bonusesSlots[i].bonusObject.GetActiveBonusID()>=0)
				bonusesSlots[i].bonusObject.MoveBonus(bonusesSlots[i].slotTransform.position,bonusMovementSpeed,Time.deltaTime);
			else 
				bonusesSlots[i].bonusObject.MoveBonus(bonusesSlots[i].slotTransform.position,1f,1f);

			if(bonusesSlots[i].bonusObject.ShouldBeScaled(targetBonusScale,bonusReachDistance) && bonusesSlots[i].bonusObject.GetActiveBonusID()>=0)
				bonusesSlots[i].bonusObject.ChangeScale(targetBonusScale,bonusMovementSpeed,Time.deltaTime);
			else 
				bonusesSlots[i].bonusObject.ChangeScale(targetBonusScale,1f,1f);
		}
	}


	void UpdateReserveBonuses(){
		if(reserveBonusesSlots.Length<1)
			return;
		for(int i=0;i<reserveBonusesSlots.Length;i++){
			if(reserveBonusesSlots[i].bonusObject.ShouldBeMoved(outOfScreenRectTransform.position,1f) && reserveBonusesSlots[i].bonusObject.GetActiveBonusID()>=0){
				float deltaTime = Time.deltaTime;
				reserveBonusesSlots[i].bonusObject.MoveBonus(outOfScreenRectTransform.position,bonusMovementSpeed,deltaTime);
				reserveBonusesSlots[i].bonusObject.ChangeScale(targetBonusScale,bonusMovementSpeed,deltaTime);
			}else{ 
				reserveBonusesSlots[i].bonusObject.MoveBonus(Vector3.zero,1f,1f);
				reserveBonusesSlots[i].bonusObject.DisableBonusObject();
			}

			if(reserveBonusesSlots[i].bonusObject.ShouldBeScaled(targetBonusScale,bonusReachDistance) && reserveBonusesSlots[i].bonusObject.GetActiveBonusID()>=0)
				reserveBonusesSlots[i].bonusObject.ChangeScale(targetBonusScale,bonusMovementSpeed,Time.deltaTime);
			else 
				reserveBonusesSlots[i].bonusObject.ChangeScale(targetBonusScale,1f,1f);
		}
	}

	int GetFreeReserveBonusID(){
		int result = -1;
		if(reserveBonusesSlots.Length<1)
			return result;
		UpdateReserveBonuses();
		for(int i=0;i<reserveBonusesSlots.Length;i++){
			if(reserveBonusesSlots[i].bonusObject.GetActiveBonusID()<0){
				result = i;
				break;
			}
		}
		
		return result;
	}


	int GetFirstFreeSlotID(){
		int result = -1;
		if(bonusesSlots.Length<1)
			return result;
		UpdateBonusesStatus();

		for(int i=0;i<bonusesSlots.Length;i++){
			if(bonusesSlots[i].bonusObject.GetActiveBonusID()<0){
				result = i;
				break;
			}
		}

		return result;
	}
	


	public int GetFirstFreeSlotID(bool reserveSlot,bool useReserved){
		int result = -1;
		if(bonusesSlots.Length<1)
			return result;
		UpdateBonusesStatus();
		
		for(int i=0;i<bonusesSlots.Length;i++){
			if(bonusesSlots[i].bonusObject.GetActiveBonusID()<0){
				result = i;
				if(!useReserved && bonusesSlots[i].reserved){
					result = -1;
					continue;
				}else 
					bonusesSlots[i].reserved = reserveSlot;
				break;
			}
		}
		
		return result;
	}


	int GetSlotWithBonus(string bonusName){
		int bonusID = -1,result = -1,bonusType = -1;
		bonusID = GetBonusID (bonusName);
		bonusType = GetBonusType(bonusID);
		
		for(int i=0;i<bonusesSlots.Length;i++){
			if(GetBonusType(bonusesSlots[i].bonusObject.GetActiveBonusID()) == bonusType){
				result = i;
				break;
			}
		}
		return result;
	}


	public Vector3 GetSlotPosition(int slotID,bool globalPosition){
		Vector3 result = Vector3.zero;
		if(slotID<0)
			return result;
		if(bonusesSlots.Length<1 || bonusesSlots.Length<=slotID)
			return result;
		if(globalPosition)
			result = bonusesSlots[slotID].slotTransform.position;
		else
			result = bonusesSlots[slotID].slotTransform.localPosition;
		return result;
	}


	//Bonuses types
	//0- eatAll, 1- speedUp/speedDown
	int GetBonusType(int bonusID){
		int result = -1;
		if(bonusID == 0)
			result = 0;
		else if(bonusID == 1 || bonusID == 2)
			result = 1;
		return result;
	}
	

	void ReadSlotsTransformsAndBonuses(){
		if(slotsTransformsRoot == null){
			Debug.Log ("Can't read slots transforms: slotsTransformsRoot is empty!");
			return;
		}
		if(slotsTransformsRoot.childCount<1){
			Debug.Log ("Can't read slots transforms: slotsTransformsRoot.childCount<1!");
			return;
		}
		bonusesSlots = new BonusSlot[slotsTransformsRoot.childCount];

		for(int i = 0;i<bonusesSlots.Length;i++){
			bonusesSlots[i] = new BonusSlot();
			bonusesSlots[i].slotTransform = slotsTransformsRoot.GetChild (i);
			if(bonusesSlots[i].slotTransform.childCount>0){
				bonusesSlots[i].bonusObject = new BonusObject(bonusesSlots[i].slotTransform.GetChild (0).GetComponent<RectTransform>());
				bonusesSlots[i].bonusObject.InitializeBonusesObjects();
				//bonusesSlots[i].bonusObject.deactivationTime = Time.time+Random.Range(4f,10f);
			}else Debug.Log ("Transform of slot "+i+" don't have transform of bonus object!");
		}
	}


	void ReadReserveBonuses(){
		if(reserveBonusesTransformsRoot == null){
			Debug.Log ("Can't read reserve bonuses transforms: reserveBonusesTransformsRoot is empty!");
			return;
		}
		if(reserveBonusesTransformsRoot.childCount<1){
			Debug.Log ("Can't read reserve bonuses transforms: reserveBonusesTransformsRoot.childCount<1!");
			return;
		}
		reserveBonusesSlots = new BonusSlot[reserveBonusesTransformsRoot.childCount];
		for(int i=0;i<reserveBonusesSlots.Length;i++){
			reserveBonusesSlots[i] = new BonusSlot();
			reserveBonusesSlots[i].bonusObject = new BonusObject(reserveBonusesTransformsRoot.GetChild (i).GetComponent<RectTransform>());
			reserveBonusesSlots[i].bonusObject.InitializeBonusesObjects();
		}
	}
	

	[System.Serializable]
	public class BonusSlot
	{
		public Transform slotTransform;
		public BonusObject bonusObject;
		public bool reserved = false;
	}


	[System.Serializable]
	public class BonusObject
	{
		public float deactivationTime = -1f;
		public RectTransform bonusRectTransform;
		public GameObject[] bonusesObjects = new GameObject[0];

		public BonusObject(RectTransform newBonusRectTransform){
			bonusRectTransform = newBonusRectTransform;
		}

		public void InitializeBonusesObjects(){
			if(bonusRectTransform == null){
				Debug.Log ("Can't read bonuses objects: bonusRectTransform is empty!");
				return;
			}
			if(bonusRectTransform.childCount<1){
				Debug.Log ("Can't read bonuses objects: bonusRectTransform.childCount<1!");
				return;
			}
			bonusesObjects = new GameObject[bonusRectTransform.childCount];
			for(int i =0; i<bonusesObjects.Length;i++){
				bonusesObjects[i] = bonusRectTransform.GetChild (i).gameObject;
			}
		}


		public void ActivateBonus(int bonusID){
			if(bonusesObjects.Length<1){
				Debug.Log ("Can't enable/disable bonus: bonusesObjects.Length<1!");
				return;
			}
			for(int i=0;i<bonusesObjects.Length;i++){
				if(bonusID == i){
					bonusesObjects[i].SetActive(true);
				}else bonusesObjects[i].SetActive(false);
			}
		}


		public void ActivateBonus(int bonusID,float newDeactivationTime){
			if(bonusesObjects.Length<1){
				Debug.Log ("Can't enable/disable bonus: bonusesObjects.Length<1!");
				return;
			}
			deactivationTime = newDeactivationTime;
			for(int i=0;i<bonusesObjects.Length;i++){
				if(bonusID == i){
					bonusesObjects[i].SetActive(true);
				}else bonusesObjects[i].SetActive(false);
			}
		}


		public void DisableBonusObject(){
			deactivationTime = -1f;
			ActivateBonus(-1);
		}


		public bool ShouldBeMoved(Vector3 targetPos,float screenDifference){
			bool result = false;
			Vector3 bonusPos = bonusRectTransform.position;
			if(Vector3.Distance(targetPos,bonusPos)>screenDifference){
				result = true;
			}
			return result;
		}


		public void MoveBonus(Vector3 newPos,float movementSpeed, float deltaTime){
			if(bonusRectTransform == null){
				Debug.Log ("Can't move bonus: bonusRectTransform is empty!");
				return;
			}
			bonusRectTransform.position = Vector3.Lerp(bonusRectTransform.position,newPos,movementSpeed*deltaTime);

		}


		public bool ShouldBeScaled(Vector3 targetScale,float screenDifference){
			bool result = false;
			Vector3 bonusLocalScale = bonusRectTransform.localScale;
			if(Vector3.Distance(targetScale,bonusLocalScale)>screenDifference){
				result = true;
			}
			return result;
		}


		public void ChangeScale(Vector3 targetScale, float speed, float deltaTime){
			if(bonusRectTransform == null){
				Debug.Log ("Can't move bonus: bonusRectTransform is empty!");
				return;
			}
			bonusRectTransform.localScale = Vector3.Lerp(bonusRectTransform.localScale,targetScale,speed*deltaTime);
		}


		public int GetActiveBonusID(){
			int result = -1;
			if(bonusesObjects.Length<1){
				Debug.Log ("No one bonus is active : bonusesObjects.Length<1!");
				return result;
			}
			for(int i=0;i<bonusesObjects.Length;i++){
				if(bonusesObjects[i].activeSelf)
					result = i;
			}
			return result;
		}


		public void SetParent(Transform newParent){
			if(newParent == null){
				Debug.Log ("Can't set bonus parent: newParent transform is empty!");
				return;
			}
			if(bonusRectTransform == null){
				Debug.Log ("Can't set bonus parent: bonusRectTransform is empty!");
				return;
			}
			//Debug.Log ("Processing bonus '"+bonusRectTransform+"'");
			//Debug.Log ("old local position:"+bonusRectTransform.localPosition);
			//Debug.Log ("old global position:"+bonusRectTransform.position);
			//Debug.Log ("Changing parent from '"+bonusRectTransform.parent.name+"'");
			bonusRectTransform.SetParent(newParent);
			//Debug.Log ("to'"+bonusRectTransform.parent.name+"'");
			//Debug.Log ("new local position:"+bonusRectTransform.localPosition);
			//Debug.Log ("new global position:"+bonusRectTransform.position);

		}
	}


	[System.Serializable]
	public class BonusID
	{
		public string _bonusName = "";
		public int _bonusID = 0;

		/*
		public string bonusName{
			get{
				return _bonusName;
			}
			set{
				_bonusName = value;
				if(string.IsNullOrEmpty(_bonusName))
					_bonusName = "";
			}
		}
		*/
	}
}

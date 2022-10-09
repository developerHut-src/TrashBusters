using UnityEngine;
using System.Collections;

public class ScreenBonus : MonoBehaviour {
	int slotID = -1;
	Vector3 defaultPosition = Vector3.zero;
	Vector3 targetPosition = Vector3.zero;
	public float speed = 3f,releaseDistance = 10f,bonusActivityTime = 10f;
	public string bonusName = "";
	public bool sentMessageToBonusBar = false;
	RectTransform thisRectTransform;

	// Use this for initialization
	void Start () {
	
	}

	void Awake(){
		thisRectTransform = transform.GetComponent<RectTransform>();
		defaultPosition = thisRectTransform.position;
	}


	// Update is called once per frame
	void Update () {
		if(targetPosition == Vector3.zero)
			return;
		TranslateItem();
		BonusRelease();
	}

	void OnEnable(){
		slotID = -1;
		if(GameInfo.bonusesBar){
			slotID = GameInfo.bonusesBar.GetFirstFreeSlotID(true,false);
			if(slotID>-1){
				targetPosition = GameInfo.bonusesBar.GetSlotPosition(slotID,true);
			}
		}
	}


	void TranslateItem(){
		if(thisRectTransform == null)
			return;
		thisRectTransform.position = Vector3.Lerp (thisRectTransform.position,targetPosition,Time.deltaTime*speed);
	}


	void BonusRelease(){
		if(Vector3.Distance (thisRectTransform.position,targetPosition)<releaseDistance){
			targetPosition = Vector3.zero;
			thisRectTransform.position = defaultPosition;
			gameObject.SetActive(false);
			if(sentMessageToBonusBar)
				GameInfo.bonusesBar.SetBonus(bonusName,bonusActivityTime);
		}
	}
}

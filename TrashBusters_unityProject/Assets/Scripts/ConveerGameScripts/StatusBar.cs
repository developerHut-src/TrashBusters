using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour {
	public Text scoreText;
	public RectTransform healthBarRectTransform;
	public HealthBarUnit[] healthbarUnits = new HealthBarUnit[0];
	public int activeHealthUnits = 0,defaultHealthUnits = 3,maxHealthUnits = 3;

	// Use this for initialization
	void Start () {
		ReadHealthBarUnits();
		SetHealthUnits(defaultHealthUnits);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void SetScoreText(string scoreString){
		if(scoreText == null){	
			Debug.Log ("Can't set score text: scoreText is not assigned!");
            return;
        }
		scoreText.text = scoreString;
	}


	void ReadHealthBarUnits(){
		if(healthBarRectTransform == null){
			return;
		}
		if(healthBarRectTransform.childCount<1){
			return;
		}
		healthbarUnits = new HealthBarUnit[healthBarRectTransform.childCount];
		Animator tempAnimator = null;
		for(int i=0;i<healthBarRectTransform.childCount;i++){
			healthbarUnits[i] = new HealthBarUnit();
			healthbarUnits[i].gameObject = healthBarRectTransform.GetChild(i).gameObject;
			if(healthbarUnits[i].gameObject!=null){
				tempAnimator = healthbarUnits[i].gameObject.GetComponent<Animator>();
				if(tempAnimator){
					healthbarUnits[i].animator = tempAnimator;
					healthbarUnits[i].animator.Play("HealthUnitIdle",-1,0f);
				}
			}
		}
	}

	public void DestroyHealthUnits(int count){
		int destructedCounter = 0;
		for(int i=(healthbarUnits.Length-1);i>-1;i--){
			if(destructedCounter>=count)
				break;
			if(healthbarUnits[i].animator && healthbarUnits[i].destructed == false){
				healthbarUnits[i].gameObject.SendMessage("StartDestructionCounter",SendMessageOptions.RequireReceiver);
				healthbarUnits[i].animator.Play("HealthUnitDestruction",-1,0f);
				healthbarUnits[i].destructed = true;
				activeHealthUnits--;
				destructedCounter++;
			}
		}
	}
	


	public void AddHealthUnits(int newUnitsCount){
		if(activeHealthUnits == maxHealthUnits && newUnitsCount>0)
			return;
		int unitsCount = Mathf.Clamp (newUnitsCount,activeHealthUnits*-1,maxHealthUnits - activeHealthUnits);
		activeHealthUnits = Mathf.Clamp (activeHealthUnits+unitsCount,0,healthbarUnits.Length);
		for(int i=0;i<healthbarUnits.Length;i++){
			if(i<activeHealthUnits){
				healthbarUnits[i].gameObject.SetActive(true);
				healthbarUnits[i].destructed = false;
				if(healthbarUnits[i].animator){
					healthbarUnits[i].animator.Play("HealthUnitIdle",-1,0f);
				}
			}else{
				healthbarUnits[i].gameObject.SetActive(false);
				healthbarUnits[i].destructed = true;
			}
		}
	}


	public void SetHealthUnits(int unitsCount){
		activeHealthUnits = Mathf.Clamp (unitsCount,0,healthbarUnits.Length);
		for(int i=0;i<healthbarUnits.Length;i++){
			if(i<activeHealthUnits){
				healthbarUnits[i].gameObject.SetActive(true);
				healthbarUnits[i].destructed = false;
				if(healthbarUnits[i].animator){
					healthbarUnits[i].animator.Play("HealthUnitIdle",-1,0f);
				}
			}else{
				healthbarUnits[i].gameObject.SetActive(false);
				healthbarUnits[i].destructed = true;
			}
		}
	}


	public void ResetStatusBar(){
		SetHealthUnits(defaultHealthUnits);
		SetScoreText("0");
	}

	[System.Serializable]
	public class HealthBarUnit
	{
		public GameObject gameObject;
		public Animator animator;
		public bool destructed = false;
	}
}

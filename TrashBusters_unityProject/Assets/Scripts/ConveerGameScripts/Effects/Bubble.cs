using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Bubble : MonoBehaviour {
	public float flyingSpeedX = 30f,flyingSpeedY = 30f,deactivationX = Mathf.Infinity,deactivationY = Mathf.Infinity;
	RectTransform thisRectTransform;
	//Image bubbleImage;
	Vector3 newPos = Vector3.zero;
	public Vector3 initialPos = Vector3.zero;
	//float imageAlphaValue = 0f;

	// Use this for initialization
	void Start () {
		thisRectTransform = GetComponent<RectTransform>();
		initialPos = thisRectTransform.localPosition;
	}

	void OnEnable(){
		if(thisRectTransform)
		thisRectTransform.localPosition = initialPos;
	}
	
	// Update is called once per frame
	void Update () {
		if(thisRectTransform == null)
			return;
		//if(bubbleImage){
			//bubbleImage.CrossFadeAlpha(1f,fadeOutSpeed,false);
		//}
		if(Mathf.Abs (thisRectTransform.localPosition.x)>deactivationX || Mathf.Abs (thisRectTransform.localPosition.y)>deactivationY){
			gameObject.SetActive(false);
		}else
		BubbleFly ();
	}

	void BubbleFly(){
		newPos = thisRectTransform.localPosition;
		newPos.x+=flyingSpeedX*Time.deltaTime;
		newPos.y+=flyingSpeedY*Time.deltaTime;
		thisRectTransform.localPosition = newPos;
	}
}

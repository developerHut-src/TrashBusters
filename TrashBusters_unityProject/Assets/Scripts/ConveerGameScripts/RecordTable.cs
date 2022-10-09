using UnityEngine;
using System.Collections;

public class RecordTable : MonoBehaviour {
	public Transform ratingLineObjectTransform;
    public GameObject playButtonObject;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetRating(int rating){
        if (playButtonObject)
        {
            if (rating < 1)
                playButtonObject.SetActive(false);
            else
                playButtonObject.SetActive(true);
        }
		if(ratingLineObjectTransform == null){
			Debug.Log ("rating line transform is not assigned!");
			return;
		}
		if(ratingLineObjectTransform.childCount<1)
			return;
		//deactivate all stars at first==========================================
		for(int i=0;i<ratingLineObjectTransform.childCount;i++){
			ratingLineObjectTransform.GetChild(i).gameObject.SetActive(false);
		}
		//=======================================================================
		rating = Mathf.Clamp (rating,0,ratingLineObjectTransform.childCount);
		for(int i=0;i<rating;i++){
			ratingLineObjectTransform.GetChild(i).gameObject.SetActive(true);
		}
	}

	public void ReplayLevel(){
		if(GameInfo.levelsRootObject == null){
			Debug.Log ("Can't send reply message to the current level object: GameInfo.levelsRootObject == null !");
			return;
		}
		Transform levelsRootTransform = GameInfo.levelsRootObject.transform;
		if(levelsRootTransform.childCount<1){
			Debug.Log ("Can't send reply message to the current level object: No one level found in GameInfo.levelsRootObject !");
			return;
		}
		levelsRootTransform.GetChild(0).SendMessage("RestartConveer",SendMessageOptions.DontRequireReceiver);
	}
	
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoMenu : MonoBehaviour {
	public float windowMovementSpeed = 3f,distanceToDestination = 3f;
	public Transform infoWindowRectTransform;
	public Transform leftPoint,rightPoint,centralPoint;
	public Text infoWindowTitleText,infoWindowMessageText;
	public InfoSession[] infoSessions = new InfoSession[0];
	public int[] sessionsSequence = new int[0];
	public int curSessionInSequenceID = 0;
	GameObject infoWindowObject;
	int showInfoWindow = 0;
	public int currentMessageID = 0,currentSessionID = 0;
	Vector3 infoWindowTargetPosition = Vector3.zero;
	bool showNextMessage = false;
	[SerializeField]
	PopupInfoMessage[] popupMessages = new PopupInfoMessage[0];

	// Use this for initialization
	void Start () {
		if(infoWindowRectTransform == null)
			Debug.Log ("Can't get GameObject of info window: RectTransform of info window is not found!");
		else infoWindowObject = infoWindowRectTransform.gameObject;
		//SendMessageToScreen(currentSessionID,currentMessageID);
	}


	// Update is called once per frame
	void Update () {
			UpdatePopupMessages ();
		if(showInfoWindow !=0)
			MoveInfoWindow();
	}


	void UpdatePopupMessages(){
		if (popupMessages.Length < 1)
			return;
		for (int i=0; i<popupMessages.Length; i++) {
			popupMessages[i].UpdateMessagePositionAndStatus(Time.time,Time.deltaTime);
		}
	}


	public void ShowRandomPopupMessage(){
		if (popupMessages.Length < 1)
			return;
		popupMessages [0].ShowMessage (Random.Range (0,1));
	}


	public void ShowPopUpMessage(int messageID,int messageType){
		if (popupMessages.Length < 1)
			return;
		if (messageID>=popupMessages.Length || messageID < 0) {
			Debug.Log ("Can't show popup message: messageID is invalid ("+messageID+")!");
			return;
		}
		popupMessages [messageID].ShowMessage (messageType);
	}


	void SendMessageToScreen(int sessionID,int messageID){
		if(infoSessions.Length<1){
			Debug.Log ("Can't send message to screen: info sessions array is empty!");
			return;
		}
		if(infoSessions[sessionID].infoMessages.Length<1){
			Debug.Log ("Can't send message to screen: messages array is empty!");
			return;
		}
		if(infoSessions[sessionID].infoMessages.Length<=messageID || messageID<0){
			Debug.Log ("Can't send message to screen: invalid messageID:"+messageID);
			return;
		}
		infoSessions [sessionID].viewTimes++;
		infoWindowTitleText.text = infoSessions[sessionID].infoMessages[messageID].title;
		infoWindowMessageText.text = infoSessions[sessionID].infoMessages[messageID].infoText;
		infoWindowObject.SetActive(true);
		showInfoWindow = 1;
		infoWindowRectTransform.position = rightPoint.position;
		GameInfo.SetGamePause(true);
	}


	void MoveInfoWindow(){
		if(infoWindowRectTransform == null){
			Debug.Log ("Can't show info window: RectTransform of info window is not found!");
			return;
		}
		if(showInfoWindow == 1)
			infoWindowTargetPosition = centralPoint.position;
		else if(showInfoWindow == -1)
			infoWindowTargetPosition = leftPoint.position;

		infoWindowRectTransform.position = Vector3.Lerp(infoWindowRectTransform.position,infoWindowTargetPosition,Time.unscaledDeltaTime*windowMovementSpeed);
		if(Vector3.Distance (infoWindowRectTransform.position,infoWindowTargetPosition)<distanceToDestination){
			if(showInfoWindow == -1){
				infoWindowObject.SetActive(false);
				GameInfo.SetGamePause(false);
			}
			//infoWindowRectTransform.position = infoWindowTargetPosition;
			if(showNextMessage){
				SendMessageToScreen(currentSessionID,currentMessageID);
				showNextMessage = false;
			}else showInfoWindow = 0;
		}
	}


	public void HideInfoWindow(){
		showInfoWindow = -1;
		showNextMessage = false;
		ResetSessionsState ();
	}


	public void ShowNextMessage(){
		//currentMessageID = Mathf.Clamp(++currentMessageID,0,infoMessages.Length-1);
		if((currentMessageID+1)>=infoSessions[currentSessionID].infoMessages.Length){
			if((currentSessionID+1)>=infoSessions.Length){
				currentSessionID = 0;
				currentMessageID = 0;
				HideInfoWindow();
			}else if(sessionsSequence.Length>0 && curSessionInSequenceID<sessionsSequence.Length-1){
				curSessionInSequenceID++;
				currentSessionID = sessionsSequence[curSessionInSequenceID];
				currentMessageID = 0;
				showInfoWindow = -1;
				showNextMessage = true;
			}else HideInfoWindow ();
		}else{
			currentMessageID = Mathf.Clamp(++currentMessageID,0,infoSessions[currentSessionID].infoMessages.Length-1);
			UpdateScreenMessage();
		}
	}


	void UpdateScreenMessage(){
		infoWindowTitleText.text = infoSessions[currentSessionID].infoMessages[currentMessageID].title;
		infoWindowMessageText.text = infoSessions[currentSessionID].infoMessages[currentMessageID].infoText;
	}


	void AddToSessionSequence(int[] newSessionsSequence){
		if (newSessionsSequence.Length < 1)
			return;
		if (sessionsSequence.Length < 1) {
			sessionsSequence = newSessionsSequence;
			return;
		} else {
			int[] tempSessionsSequence = new int[sessionsSequence.Length+newSessionsSequence.Length];
			int newIndex = 0;
			for(int i=0;i<tempSessionsSequence.Length;i++){
				if(i<sessionsSequence.Length){
					tempSessionsSequence[i] = sessionsSequence[i];
				}else{
					newIndex = newSessionsSequence.Length - (tempSessionsSequence.Length-i);
					tempSessionsSequence[i] = newSessionsSequence[newIndex];
				}
			}
			sessionsSequence = tempSessionsSequence;
		}
	}
	

	public void ShowMessage(int[] newSessionsSequence,int messageID,bool addToCurrentSequence){
		if (addToCurrentSequence == false) {
			currentSessionID = Mathf.Clamp (newSessionsSequence [0], 0, infoSessions.Length - 1);
			currentMessageID = Mathf.Clamp (messageID, 0, infoSessions [currentSessionID].infoMessages.Length - 1);
			sessionsSequence = newSessionsSequence;
			curSessionInSequenceID = 0;
			if (infoWindowObject.activeSelf) {
				showInfoWindow = -1;
				showNextMessage = true;
			} else
				SendMessageToScreen (currentSessionID, currentMessageID);
		} else
			AddToSessionSequence (newSessionsSequence);
		
	}


	public int GetSessionViewTimes(int sessionID){
		int result = 0;
		sessionID = Mathf.Clamp(sessionID,0,infoSessions.Length-1);
		result = infoSessions [sessionID].viewTimes;
		return result;
	}


	void ResetSessionsState(){
		curSessionInSequenceID = currentSessionID = currentMessageID = 0;
		sessionsSequence = new int[0];
	}


	public void ResetSessionsViews(){
		if (infoSessions.Length < 1)
			return;
		for (int i=0; i<infoSessions.Length; i++) {
			if(infoSessions[i]!=null)
				infoSessions[i].viewTimes=0;
		}
	}
	

	[System.Serializable]
	public class WindowInfoMessage
	{
		public string title = "_-_";
		public string infoText = "_-_";

		public WindowInfoMessage(string newTitle, string newInfoText){
			title = newTitle;
			infoText = newInfoText;
		}
	}


	[System.Serializable]
	public class InfoSession
	{
		public WindowInfoMessage[] infoMessages = new WindowInfoMessage[0];
		public int viewTimes = 0;
	}


	[System.Serializable]
	public class PopupInfoMessage
	{
		int moveDir = 0;
		[SerializeField]
		GameObject messageGameObject;
		Transform messageTransform;
		[SerializeField]
		Transform destinationTransform;
		[SerializeField]
		Transform originTransform;
		[SerializeField]
		AudioClip[] messageSounds = new AudioClip[0];
		[SerializeField]
		CallCounter[] subMessageCallCounter = new CallCounter[0];
		AudioSource messageAudioSource;
		[Range(0f,1000f)]public float movementSpeed = 3f;
		[Range(0f,1000f)]public float onScreenDelay = 1f;
		float delayEndTime = -1f;
		Vector3 targetPoint = Vector3.zero;
		int nextStepMessage = -1,curMessageType = -1;


		public void ShowMessage(int messageType){
			if (messageGameObject == null)
				return;
			if (messageTransform == null) {
				messageTransform = messageGameObject.GetComponent<Transform>();
			}
			if (messageTransform == null)
				return;
			if (destinationTransform == null || originTransform == null)
				return;
			if (subMessageCallCounter.Length > 0 && messageType < subMessageCallCounter.Length && messageType > -1) {
				if(subMessageCallCounter[messageType]!=null){
					if(subMessageCallCounter[messageType].callsCounter<subMessageCallCounter[messageType].callsToShow){
						subMessageCallCounter[messageType].callsCounter++;
						return;
					}else subMessageCallCounter[messageType].callsCounter = 0;
				}
			}

			if (delayEndTime > -1f) {
				delayEndTime = -1f;
				moveDir = 1;
				nextStepMessage = messageType;
				return;
			}
			if (moveDir != 0) {
				moveDir = 1;
				nextStepMessage = messageType;
				return;
			}
			messageGameObject.SetActive(true);
			messageTransform.position = originTransform.position;
			targetPoint = destinationTransform.position;
			moveDir = -1;
			ActivateChild (messageType);
			PlayMessageSound (messageType);
		}


		void PlayMessageSound(int soundID){
			if (messageAudioSource == null)
				messageAudioSource = messageTransform.GetComponent<AudioSource> ();
			if (messageAudioSource == null)
				return;
			if(messageSounds.Length>0 && soundID>-1 && soundID<messageSounds.Length)
				messageAudioSource.PlayOneShot (messageSounds[soundID]);
		}


		public void UpdateMessagePositionAndStatus(float curTime,float deltaTime){
			if (messageGameObject.activeSelf == false && nextStepMessage>-1) {
				delayEndTime = -1f;
				ShowMessage(nextStepMessage);
				nextStepMessage = -1;

			}
			if (delayEndTime > -1f && curTime > delayEndTime) {
				moveDir = 1;
				delayEndTime = -1f;
			}
			if (moveDir == 0)
				return;
			if (messageGameObject == null)
				return;
			if (messageTransform == null) {
				messageTransform = messageGameObject.GetComponent<Transform>();
			}
			if (messageTransform == null)
				return;
			if (destinationTransform == null || originTransform == null)
				return;


			messageTransform.position = Vector3.Lerp (messageTransform.position, targetPoint, movementSpeed * deltaTime);
			if (Vector3.Distance (messageTransform.position, targetPoint) < 0.3f) {
				if(moveDir == -1){
					moveDir = 0;
					delayEndTime = curTime+onScreenDelay;
				}else if(moveDir ==1){
					if(targetPoint !=originTransform.position)
						targetPoint = originTransform.position;
					else{ 
						moveDir = 0;
						messageGameObject.SetActive(false);}
				}
			}
		}


		void ActivateChild(int id){
			if (messageTransform == null)
				return;
			if (messageTransform.childCount < 1)
				return;
			GameObject curChild = null;
			for (int i =0; i<messageTransform.childCount; i++) {
				curChild = messageTransform.GetChild(i).gameObject;
				if(i == id){
					curChild.SetActive(true);
					curMessageType = id;
				}else curChild.SetActive(false);
			}
		}


		[System.Serializable]
		class CallCounter
		{
			public int callsToShow = 0;
			public int callsCounter =0;

		}


	}
}

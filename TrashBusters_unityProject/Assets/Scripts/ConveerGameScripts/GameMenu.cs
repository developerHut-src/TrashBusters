/*=================================== made by Vladimir SILENT Maevskiy============================
 * ===============================================================================================*/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour {
	public Text gameMenuButtonText;
	public GameObject gameMenuObject;
	public bool gameMenuActivated = false;

	// Use this for initialization
	void Start () {
		if(gameMenuObject){
			gameMenuObject.SetActive (gameMenuActivated);
			if(gameMenuButtonText){
				gameMenuButtonText.text = "Показать меню";
			}else Debug.Log (" Game menu BUTTON text not found!");
		}else Debug.Log ("Game menu object not found!");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GameMenuButtonPressed(){
		if(gameMenuObject == null){
			Debug.Log ("Game menu object not found!");
			return;
		} 

		if(gameMenuActivated){
			gameMenuActivated = false;
			Time.timeScale = 1f;
			if(gameMenuButtonText){
				gameMenuButtonText.text = "Показать меню";
			}
		}else{
			gameMenuActivated = true;
			Time.timeScale = 0f;
			if(gameMenuButtonText){
				gameMenuButtonText.text = "Скрыть меню";
			}
		}
		gameMenuObject.SetActive (gameMenuActivated);
	}

	public void ExitFromGame(){
		Application.Quit();
	}
}

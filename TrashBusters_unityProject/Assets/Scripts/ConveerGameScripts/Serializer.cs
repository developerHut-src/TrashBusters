using UnityEngine;
using System.Collections;

public class Serializer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public static void SaveGameData(GameInfo.LocationLevelsRatings[] newLocationLevelsRatings){
		if(newLocationLevelsRatings == null)
			return;
		if(newLocationLevelsRatings.Length<1)
			return;
		Debug.Log ("Try to save game data...");
		for(int i =0;i<newLocationLevelsRatings.Length;i++){
			if(newLocationLevelsRatings[i]!=null){
				if(newLocationLevelsRatings[i].levelsRatings.Length>0){
					for(int j=0;j<newLocationLevelsRatings[i].levelsRatings.Length;j++){
						if(newLocationLevelsRatings[i].levelsRatings[j]!=null){
							PlayerPrefs.SetInt(i.ToString ()+j.ToString(),newLocationLevelsRatings[i].levelsRatings[j].rating);

						}
					}
				}
			}
		}
	}


	public static void LoadGameData(GameInfo.LocationLevelsRatings[] newLocationLevelsRatings){
		if(newLocationLevelsRatings == null)
			return;
		if(newLocationLevelsRatings.Length<1)
			return;
		Debug.Log ("Try to load game data...");
		for(int i =0;i<newLocationLevelsRatings.Length;i++){
			if(newLocationLevelsRatings[i]!=null){
				if(newLocationLevelsRatings[i].levelsRatings.Length>0){
					for(int j=0;j<newLocationLevelsRatings[i].levelsRatings.Length;j++){
						if(newLocationLevelsRatings[i].levelsRatings[j]!=null){
							int defaultValue = -1;
							if(j == 0)
								defaultValue = 0;
							newLocationLevelsRatings[i].levelsRatings[j].rating = PlayerPrefs.GetInt(i.ToString ()+j.ToString(),defaultValue);
							
						}
					}
				}
			}
		}
	}


	public static void ForceSaveDataOnDisc(){
		PlayerPrefs.Save();
	}
	
}

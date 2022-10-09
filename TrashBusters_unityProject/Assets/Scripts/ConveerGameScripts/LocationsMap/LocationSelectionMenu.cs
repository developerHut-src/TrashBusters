using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LocationSelectionMenu : MonoBehaviour {
    public RectTransform mapRectTransform;
    public Vector3 leftMapScrollLimit = Vector3.zero, rightMapScrollLimit = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateMapPosition()
    {
        if (mapRectTransform == null)
        {
            Debug.Log("Can't set map position: mapRectTransform is not assigned!");
            return;
        }
        Vector3 newMapPos = mapRectTransform.localPosition;
        newMapPos.x = Mathf.Clamp(newMapPos.x, rightMapScrollLimit.x, leftMapScrollLimit.x);
        newMapPos.y = Mathf.Clamp(newMapPos.y, rightMapScrollLimit.y, leftMapScrollLimit.y);
        newMapPos.z = Mathf.Clamp(newMapPos.z, rightMapScrollLimit.z, leftMapScrollLimit.z);
        mapRectTransform.localPosition = newMapPos;
    }
}

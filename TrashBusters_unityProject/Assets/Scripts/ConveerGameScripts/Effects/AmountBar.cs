using UnityEngine;
using System.Collections;

public class AmountBar : MonoBehaviour {
	[SerializeField]
	ColorProperty[] colorProperties = new ColorProperty[2];
	[SerializeField]
	Vector2 barSizeDelta = Vector2.zero;
	RectTransform barLine;


	void OnEnable () {
		if (barLine == null && transform.childCount>1)
			barLine = transform.GetChild (1).GetComponent<RectTransform> ();
		if (barLine != null)
			barSizeDelta = barLine.sizeDelta;
	}


	// Update is called once per frame
	void Update () {
	
	}


	[System.Serializable]
	class ColorProperty
	{
		
		public Color barColor,outlineColor;
	}


	class AmountBarItem
	{
		public ColorProperty colorProperty;
		public float startTime = -1f,length = -1f;
	}
		
}

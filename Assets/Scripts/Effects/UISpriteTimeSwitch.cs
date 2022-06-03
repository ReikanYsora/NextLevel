using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpriteTimeSwitch : MonoBehaviour
{
	#region ATTRIBUTES
	public Sprite sprite1; 
	public Sprite sprite2;
	private Image image;
	public float switchTime;
	private float switchTimeTemp;
	private bool switchState;
	#endregion

	#region UNITY METHODS
	private void Awake()
	{
		image = GetComponent<Image>();
		switchState = false;
	}

	private void Update()
	{
		if (image)
		{
			switchTimeTemp += Time.unscaledDeltaTime;

			if (switchTimeTemp >= switchTime)
			{
				switchTimeTemp = 0.0f;

				if (!switchState)
				{
					image.sprite = sprite2;
					switchState = true;
				}
				else
				{
					image.sprite = sprite1;
					switchState = false;
				}
			}
		}
	}
	#endregion

}

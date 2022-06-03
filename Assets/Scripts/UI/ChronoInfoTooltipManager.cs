using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChronoInfoTooltipManager : MonoBehaviour
{
	#region ATTRIBUTES
	public Sprite iconMore;
	public Sprite iconLess;
	public Image iconDisplayed;
	public TextMeshProUGUI valueDisplayed;
	#endregion

	#region METHODS
	public void Initialized(TimeDirection timeDirection, float value)
	{
		switch (timeDirection)
		{
			case TimeDirection.More:
				valueDisplayed.text = string.Format("+{0} sec.", value);
				iconDisplayed.sprite = iconMore;
				break;
			case TimeDirection.Less:
				valueDisplayed.text = string.Format("-{0} sec.", value);
				iconDisplayed.sprite = iconLess;
				break;
		}
	}

	public void DestroyTooltip()
	{
		Destroy(gameObject);
	}
	#endregion
}

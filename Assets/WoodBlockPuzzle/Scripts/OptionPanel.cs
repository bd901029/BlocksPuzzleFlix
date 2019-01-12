using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OptionPanel : MonoBehaviour 
{
	/// <summary>
	/// Raises the settings button pressed event.
	/// </summary>
	public void OnSettingsButtonPressed()
	{
		if (InputManager.Instance.canInput ()) {
			AudioManager.Instance.PlayButtonClickSound ();
			StackManager.Instance.SpawnUIScreen ("Settings", true);
		}
	}

	/// <summary>
	/// Raises the messages button pressed event.
	/// </summary>
	public void OnMessagesButtonPressed()
	{
		if (InputManager.Instance.canInput ()) {
			AudioManager.Instance.PlayButtonClickSound ();
		}
	}

	/// <summary>
	/// Raises the rate button pressed event.
	/// </summary>
	public void OnRateButtonPressed()
	{
		if (InputManager.Instance.canInput ()) {
			AudioManager.Instance.PlayButtonClickSound ();

			if (GameController.Instance.isInternetAvailable ()) {
				Application.OpenURL (GameInfo.ReviewURL);			
			}
		}	
	}

	/// <summary>
	/// Raises the shop button pressed event.
	/// </summary>
	public void OnShopButtonPressed()
	{
		if (InputManager.Instance.canInput ()) {
			AudioManager.Instance.PlayButtonClickSound ();
			StackManager.Instance.SpawnUIScreen ("Shop", true);	
		}
	}

	/// <summary>
	/// Raises the select language button pressed event.
	/// </summary>
	public void OnSelectLanguageButtonPressed()
	{
		AudioManager.Instance.PlayButtonClickSound ();
		StackManager.Instance.SpawnUIScreen ("SelectLanguage", true);	
	}

	/// <summary>
	/// Raises the share button pressed event.
	/// </summary>
	public void OnShareButtonPressed()
	{
		if (InputManager.Instance.canInput ()) {
			AudioManager.Instance.PlayButtonClickSound ();
		}
	}

}

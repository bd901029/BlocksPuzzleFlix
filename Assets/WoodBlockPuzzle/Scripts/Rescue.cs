using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rescue : MonoBehaviour 
{
	[SerializeField]
	private Button btnWatchVideo;

	[SerializeField]
	private Text txtRescueReason;


	void OnEnable() 
	{
		//YOUR AD NETWORK CALLBACK REGISTER. REPLACE THIS CODE WITH YOUR AD NETWORKS CALLBACK REGISTER
		//OnYourAdNetwork.OnWatchVideoSuccess += OnWatchVideoSuccess;

		switch (GamePlayUI.Instance.currentGameOverReson) {
		case GameOverReason.OUT_OF_MOVES:
			txtRescueReason.SetLocalizedTextForTag ("txt-out-moves");
			break;
		case GameOverReason.BOMB_COUNTER_ZERO:
			txtRescueReason.SetLocalizedTextForTag ("txt-bomb-blast");
			break;
		case GameOverReason.TIME_OVER:
			txtRescueReason.SetLocalizedTextForTag ("txt-time-over");
			break;
		}


		if(btnWatchVideo != null)
		{
			//Init this with ad network's status of ad is available or not.
			bool isAdsAvailable = false;

			if(isAdsAvailable &&  GamePlay.Instance.isFreeRescueAvailable())
			{
				btnWatchVideo.interactable = true;
				btnWatchVideo.GetComponent<CanvasGroup>().alpha = 1F;
			} else {
				btnWatchVideo.interactable = false;
				btnWatchVideo.GetComponent<CanvasGroup>().alpha = 0.3F;
			}
		}	
	}

	void OnDisable() { 
		//YOUR AD NETWORK CALLBACK UNREGISTER. REPLACE THIS CODE WITH YOUR AD NETWORKS CALLBACK UNREGISTER
		//OnYourAdNetwork.OnWatchVideoSuccess +-= OnWatchVideoSuccess;
	}

	//THIS IS JUST A PLACE HOLDER CODE. YOU CAN REPLACE IT WITH YOUR OWN LOGIC.
	void OnWatchVideoSuccess (bool result)
	{
		if (result == true) {
			GamePlay.Instance.OnRescueDone (true);
		} 
	}

	public void OnCloseButtonPressed()
	{
		if (InputManager.Instance.canInput ()) {
			AudioManager.Instance.PlayButtonClickSound ();
			GamePlay.Instance.OnRescueDiscarded ();
		}
	}

	public void OnRescueUsingWatchVideo()
	{
		if (InputManager.Instance.canInput ()) 
		{
			//CALL YOUR AD NETWORK VIDEO AD HERE TO RESCUE USING WATCH VIDEO.
		}
	}

	public void OnRescueUsingCoins()
	{
		if (InputManager.Instance.canInput ()) {
			bool coinDeduced = CurrencyManager.Instance.deductBalance (200);

			if (coinDeduced) {
				GamePlay.Instance.OnRescueDone (false);
			} else {
				//StackManager.Instance.SpawnUIScreen ("Shop");
                GamePlay.Instance.OnRescueDiscarded();
			}
		}
	}
}

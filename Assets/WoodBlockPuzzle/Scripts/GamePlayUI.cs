using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GamePlayUI : Singleton<GamePlayUI> 
{
	[SerializeField] private GameObject alertWindow;
	//Text txtAlertText;

	public GameOverReason currentGameOverReson;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		//txtAlertText = alertWindow.transform.GetChild (0).GetComponentInChildren<Text> ();
	}

	public void OnPauseButtonPressed(){
		if (InputManager.Instance.canInput ()) {
			AudioManager.Instance.PlayButtonClickSound ();
			StackManager.Instance.SpawnUIScreen ("Paused");
		}
	}

    public void OnQuitButtonPressed()
    {
        if (InputManager.Instance.canInput())
        {
            AudioManager.Instance.PlayButtonClickSound();
            StackManager.Instance.CloseGameplay();
        }
    }

	public void ShowAlert()
	{
		alertWindow.SetActive (true);
		if (!IsInvoking ("CloseAlert")) {
			Invoke ("CloseAlert", 2F);
		}
	}

	/// <summary>
	/// Closes the alert.
	/// </summary>
	void CloseAlert()
	{
		alertWindow.SetActive (false);
	}

	/// <summary>
	/// Shows the rescue.
	/// </summary>
	/// <param name="reason">Reason.</param>
	public void ShowRescue(GameOverReason reason)
	{
		currentGameOverReson = reason;
		StartCoroutine (ShowRescueScreen(reason));
	}

	/// <summary>
	/// Shows the rescue screen.
	/// </summary>
	/// <returns>The rescue screen.</returns>
	/// <param name="reason">Reason.</param>
	IEnumerator ShowRescueScreen(GameOverReason reason)
	{		
		#region time mode
		if(GameController.gameMode == GameMode.TIMED || GameController.gameMode == GameMode.CHALLENGE){
			GamePlay.Instance.timeSlider.PauseTimer();
		}
		#endregion

		switch (reason) {
		case GameOverReason.OUT_OF_MOVES:
			//txtAlertText.SetLocalizedTextForTag ("txt-out-moves");
			break;
		case GameOverReason.BOMB_COUNTER_ZERO:
			//txtAlertText.SetLocalizedTextForTag ("txt-bomb-blast");
			break;
		case GameOverReason.TIME_OVER:
			//txtAlertText.SetLocalizedTextForTag ("txt-time-over");
			break;
		}

		yield return new WaitForSeconds (0.5F);
		alertWindow.SetActive (true);
		yield return new WaitForSeconds (1.5F);
		alertWindow.SetActive (false);
        GamePlay.Instance.OnGameOver();
		//StackManager.Instance.SpawnUIScreen ("Rescue");
	}
}

/// <summary>
/// Game over reason.
/// </summary>
public enum GameOverReason
{
	OUT_OF_MOVES = 0,
	BOMB_COUNTER_ZERO = 1,
	TIME_OVER
}
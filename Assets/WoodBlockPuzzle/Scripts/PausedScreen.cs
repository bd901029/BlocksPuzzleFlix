using UnityEngine;
using System.Collections;

public class PausedScreen : MonoBehaviour
{
	bool hasGameExit = false;

	/// <summary>
	/// Raises the enable event.
	/// </summary>
	void OnEnable()
	{
		#region time mode
		if (GamePlay.Instance != null && (GameController.gameMode == GameMode.TIMED || GameController.gameMode == GameMode.CHALLENGE)) {
			GamePlay.Instance.timeSlider.PauseTimer ();
		}
		#endregion
	}

	/// <summary>
	/// Raises the disable event.
	/// </summary>
	void OnDisable()
	{
		#region time mode
		if(!hasGameExit)
		{
			if (GamePlay.Instance != null && (GameController.gameMode == GameMode.TIMED || GameController.gameMode == GameMode.CHALLENGE)) {
				GamePlay.Instance.timeSlider.ResumeTimer ();
			}
		}
		#endregion
	}

	/// <summary>
	/// Raises the home button pressed event.
	/// </summary>
	public void OnHomeButtonPressed ()
	{
		if (InputManager.Instance.canInput ()) 
		{
			hasGameExit = true;
			AudioManager.Instance.PlayButtonClickSound ();
			StackManager.Instance.OnCloseButtonPressed ();
			StackManager.Instance.CloseGameplay ();
		}
	}

	/// <summary>
	/// Raises the reset button pressed event.
	/// </summary>
	public void OnResetButtonPressed ()
	{
		if (InputManager.Instance.canInput ()) {
			AudioManager.Instance.PlayButtonClickSound ();
			StackManager.Instance.RestartGamePlay ();
		}
	}

	/// <summary>
	/// Raises the close button pressed event.
	/// </summary>
	public void OnCloseButtonPressed ()
	{
		if (InputManager.Instance.canInput ()) {
			AudioManager.Instance.PlayButtonClickSound ();
			StackManager.Instance.OnCloseButtonPressed ();
		}
	}
}

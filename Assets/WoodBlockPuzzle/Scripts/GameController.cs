using UnityEngine;
using System.Collections;

public class GameController : Singleton<GameController> 
{
	public static GameMode gameMode = GameMode.CLASSIC;
	public Canvas UICanvas; 

	// Checks if interner is available or not.
	public bool isInternetAvailable()
	{
		if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork) {
			return true;
		}
		return false;
	}

	/// <summary>
	/// Quits the game.
	/// </summary>
	public void QuitGame()
	{
		Invoke ("QuitGameWithDelay", 0.5F);
	}

	/// <summary>
	/// Quits the game with delay.
	/// </summary>
	void QuitGameWithDelay ()
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit ();
		#endif
	}
}

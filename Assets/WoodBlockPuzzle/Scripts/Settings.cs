using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour 
{
	public void OnRateButtonPressed()
	{
		if (InputManager.Instance.canInput ()) {
			AudioManager.Instance.PlayButtonClickSound ();
            #if UNITY_EDITOR
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.ponygames.MagicBlockPuzzle");
#elif UNITY_ANDROID
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.ponygames.MagicBlockPuzzle");
#elif UNITY_IPHONE
        Application.OpenURL("https://itunes.apple.com/us/app/magic-block-puzzle-breaker/id1402365261?ls=1&mt=8");
#else
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.ponygames.MagicBlockPuzzle");
#endif
		}
	}

	public void OnCloseButtonPressed()
	{
		if (InputManager.Instance.canInput ()) {
			AudioManager.Instance.PlayButtonClickSound ();
			StackManager.Instance.OnCloseButtonPressed ();
		}
	}

	public void OnSelectLanguageButtonPressed(){
		if (InputManager.Instance.canInput ()) {

			GameObject currentWindow = StackManager.Instance.PeekWindow ();
			StackManager.Instance.PopWindow ();

			StackManager.Instance.SpawnUIScreen ("SelectLanguage");
			Destroy (currentWindow);

		}
	}
}

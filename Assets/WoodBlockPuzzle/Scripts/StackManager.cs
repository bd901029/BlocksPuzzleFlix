using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StackManager : Singleton<StackManager> 
{
	/// <summary>
	/// This stack manages all the screen. any screen on the screen is pused and removing screen will be popped.
	/// You cab always ask for the help if you're having trouble in changing flow.
	/// </summary>
	public Stack<GameObject> WindowStack = new Stack<GameObject> ();

	void Start()
	{
		Application.targetFrameRate = 60;
	}

	/// <summary>
	/// Spawns the prefab from resources.
	/// </summary>
	/// <returns>The prefab from resources.</returns>
	/// <param name="path">Path.</param>
	public GameObject SpawnPrefabFromResources (string path)
	{
		GameObject thisObject = (GameObject)Instantiate (Resources.Load (path));
		thisObject.name = thisObject.name.Replace ("(Clone)", "");
		return thisObject;
	}

	/// <summary>
	/// Spawns the user interface screen.
	/// </summary>
	/// <returns>The user interface screen.</returns>
	/// <param name="name">Name.</param>
	/// <param name="doAddToStack">If set to <c>true</c> do add to stack.</param>
	public GameObject SpawnUIScreen (string name, bool doAddToStack = true)
	{
		if (name == "GamePlay") {
			if(WindowStack.Count > 0) {
				//Destroy (WindowStack.Pop ());
			}
		}
		GameObject thisScreen = (GameObject)Instantiate (Resources.Load ("Prefabs/UIScreens/" + name.ToString ()));
		thisScreen.name = name;
		thisScreen.transform.SetParent (GameController.Instance.UICanvas.transform);
		thisScreen.transform.localPosition = Vector3.zero;
		thisScreen.transform.localScale = Vector3.one;
		thisScreen.GetComponent<RectTransform> ().sizeDelta = Vector3.zero;
		thisScreen.OnWindowLoad ();
		thisScreen.SetActive (true);

		if (doAddToStack) {
			WindowStack.Push (thisScreen);
		}
		return thisScreen;
	}

	/// <summary>
	/// Spawns the prefab.
	/// </summary>
	/// <returns>The prefab.</returns>
	/// <param name="name">Name.</param>
	/// <param name="doAddToStack">If set to <c>true</c> do add to stack.</param>
	public GameObject SpawnPrefab (string name, bool doAddToStack = false)
	{
		GameObject thisScreen = (GameObject)Instantiate (Resources.Load ("Prefabs/GamePlay/" + name.ToString ()));
		if (doAddToStack) {
			WindowStack.Push (thisScreen);
		}
		return thisScreen;
	}

	/// <summary>
	/// Pushes the window to stack when it is spawed.
	/// </summary>
	/// <param name="window">Window.</param>
	public void PushWindow (GameObject window)
	{
		if (!WindowStack.Contains (window)) {
			WindowStack.Push (window);
		}
	}

	/// <summary>
	/// Pops the window when it is removed.
	/// </summary>
	/// <returns>The window.</returns>
	public GameObject PopWindow ()
	{
		if (WindowStack.Count > 0) {
			return WindowStack.Pop ();
		}
		return null;
	}

	/// <summary>
	/// Peeks the last entered windows from the stack.
	/// </summary>
	/// <returns>The window.</returns>
	public GameObject PeekWindow ()
	{
		if (WindowStack.Count > 0) {
			return WindowStack.Peek ();
		}
		return null;
	}

	/// <summary>
	/// Raises the back button pressed event.
	/// </summary>
	public void OnBackButtonPressed ()
	{
		if (WindowStack != null && WindowStack.Count > 0) 
		{
			GameObject currentWindow = WindowStack.Peek ();

			///if back button pressed from main screen, it will ask for quit-confirm.
			if (currentWindow.name == "MainScreen") {
				SpawnUIScreen ("Quit-Confirm-Game", true);
				return;
			} 

			/// if back button pressed during gameplay, this will ask for confirmation to quit the play.
			else if (currentWindow.name == "GamePlay") {
				SpawnUIScreen ("Paused", true);
				return;
			} else if (currentWindow.name == "Rescue") {
				GamePlay.Instance.OnRescueDiscarded ();
				return;
			}

			///if Game Over screen is opened and back/close/home button is pressed, it will navigate to main screen.
			else if (currentWindow.name == "GameOver") 
			{
				if (currentWindow.OnWindowRemove () == false) 
				{
					Destroy (currentWindow);
				}
				WindowStack.Pop ();
				Destroy (WindowStack.Pop ());
				SpawnUIScreen("MainScreen",true);
				return;
			}

			/// if any other screen mentioned above is opened and back button is pressed, this will lead to close that screen only.
			else {
				if (currentWindow.OnWindowRemove () == false) {
					Destroy (currentWindow);
				}
			}
			WindowStack.Pop ();
		} 

		InputManager.Instance.DisableTouchForDelay ();
	}

	public void LoadGamePlayFromModeSelection()
	{
		Destroy (PopWindow ());		//	Removing Home Screen
		Destroy (PopWindow ());		// Removing Mode Selection Popup
		Invoke("LoadGamePlay",0F);
	}

	/// <summary>
	/// Restarts the game play.
	/// This is an adjustment made where only game
	/// </summary>
	public void RestartGamePlay()
	{
		GameObject currentWindow = WindowStack.Peek ();
		if (currentWindow != null) {
			if (currentWindow.name == "GameOver" || currentWindow.name == "Paused") {
				if (currentWindow.OnWindowRemove () == false) {
					Destroy (currentWindow);
				}
			}
			WindowStack.Pop ();
			Destroy (PeekWindow ());
			Invoke("LoadGamePlay",0.1F);
		}
	}

	void LoadGamePlay()
	{
		SpawnUIScreen ("GamePlay", true);
	}

	public void CloseGameplay()
	{
		GameObject currentWindow = WindowStack.Peek ();
		if (currentWindow != null) {
			if (currentWindow.name == "GamePlay") {
				if (currentWindow.OnWindowRemove () == false) {
					Destroy (currentWindow);
				}
				WindowStack.Pop ();
			}

			SpawnUIScreen ("MainScreen");
		}
	}

    public void DestroyGamePlay()
    {
        GameObject currentWindow = WindowStack.Peek();
        if (currentWindow != null)
        {
            if (currentWindow.name == "GamePlay")
            {
                if (currentWindow.OnWindowRemove() == false)
                {
                    Destroy(currentWindow);
                }
                WindowStack.Pop();
            }
        }
    }

	/// <summary>
	/// Raises the close button pressed event.
	/// </summary>
	public void OnCloseButtonPressed ()
	{
		OnBackButtonPressed ();
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update ()
	{
		///Detects the back button press event.
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (InputManager.Instance.canInput ()) {
				OnBackButtonPressed ();
			}
		}
	}

//	public void CloseRescue()
//	{
//		GameObject currentWindow = WindowStack.Peek ();
//		if (currentWindow != null) {
//			if (currentWindow.OnWindowRemove () == false) {
//				Destroy (currentWindow);
//			}
//			WindowStack.Pop ();
//		}
//	}
}

using UnityEngine;
using System.Collections;
using Facebook.Unity;

public class MainScreen : UIScreen 
{

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnScreenWillBeLoaded()
    {
    }

    public override void init()
	{
		base.init ();
	}

    private void Start()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(OnFacebookInitCallback, OnFacebookHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    public void OnPlayButtonPressed()
	{
		if (InputManager.Instance.canInput ()) {
			AudioManager.Instance.PlayButtonClickSound ();
			StackManager.Instance.SpawnUIScreen ("SelectMode");
		}
	}

    public void Rate()
    {
        AdsControl.Instance.RateMyGame();
    }

    /// Facebook
    public void OnFacebookBtnClicked()
    {
        string[] perms = { "public_profile", "email" };
        FB.LogInWithReadPermissions(perms, OnFacebookLoggedInCallback);
    }

    private void OnFacebookInitCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnFacebookHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    private void OnFacebookLoggedInCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            Debug.Log(aToken.UserId);
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }
        }
        else
        {
            Debug.Log("User canceled login");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseSuccess : MonoBehaviour {

	public void OnOkButtonPressed()
	{
		if (InputManager.Instance.canInput ()) {
			StackManager.Instance.OnCloseButtonPressed ();
		}
	}

	public void OnCloseButtonPressed()
	{
		if (InputManager.Instance.canInput ()) {
			AudioManager.Instance.PlayButtonClickSound ();
			StackManager.Instance.OnCloseButtonPressed ();
		}
	}
}

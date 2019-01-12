using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

/// <summary>
/// Popup animation.
/// </summary>
public class PopupAnimation : MonoBehaviour
{
	[SerializeField] private bool AnimateOnLoad = true;
	[SerializeField] private bool AnimateOnDestroy = true;
	[SerializeField] private bool DestroyOnRemove = true;
	[SerializeField] private bool UseBackgroundFadeEffect = true;
	[SerializeField] private Image BlackLayImage;
	[SerializeField] private GameObject WindowContent;
	private Animator PopupAnimator;
	private Animator BlackLayFadeAnimator;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		if (WindowContent != null) {
			PopupAnimator = WindowContent.GetComponent<Animator> ();

			if (BlackLayImage != null) {
				BlackLayFadeAnimator = BlackLayImage.GetComponent<Animator> ();
			}
			if (BlackLayImage == null || BlackLayFadeAnimator == null) {
				UseBackgroundFadeEffect = false;
			}
		}
		if (PopupAnimator == null || WindowContent == null) {
			AnimateOnLoad = false;
			AnimateOnDestroy = false;
		}
	}

	/// <summary>
	/// Raises the window added event.
	/// </summary>
	public void OnWindowAdded ()
	{
		Invoke ("ShowStartAnimation", 0F);
	}

	/// <summary>
	/// Shows the start animation.
	/// </summary>
	void ShowStartAnimation()
	{
		if (AnimateOnLoad && (WindowContent != null)) {
			PopupAnimator.SetTrigger ("Open");

			if (UseBackgroundFadeEffect) {
				BlackLayFadeAnimator.SetTrigger ("FadeIn");
			}
		}
	}

	/// <summary>
	/// Raises the window remove event.
	/// </summary>
	public void OnWindowRemove ()
	{
		if ((AnimateOnDestroy && (WindowContent != null))) {
			PopupAnimator.SetTrigger ("Close");
			if (UseBackgroundFadeEffect) {
				BlackLayFadeAnimator.SetTrigger ("FadeOut");
			}
		}
		if (DestroyOnRemove) {
			Invoke ("OnRemoveTransitionComplete", 0.2F);
		} else {
			gameObject.SetActive (false);
		}
	}
		
	/// <summary>
	/// Raises the remove transition complete event.
	/// </summary>
	void OnRemoveTransitionComplete ()
	{
		Destroy (gameObject);
	}
}

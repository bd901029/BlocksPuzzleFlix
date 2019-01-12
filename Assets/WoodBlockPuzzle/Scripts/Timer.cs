using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour 
{
	[SerializeField]
	private int MaxTimeCounter = 60;

	float timeRemaining = 60.0F;

	[SerializeField] private Image imageProgress;

	void Start()
	{
		if (!IsInvoking ("ElapseTimer")) {
			InvokeRepeating ("ElapseTimer", 0.1F, 0.1F);
		}
	}

	void ElapseTimer()
	{
		timeRemaining -= 0.1F;
		SetTimeSlider (timeRemaining);
	}

	void SetTimeSlider(float timeRemaining)
	{
		if (timeRemaining <= 0) {
			GamePlayUI.Instance.ShowRescue (GameOverReason.TIME_OVER);
		} else {
			float sliderValue = (timeRemaining / (float)MaxTimeCounter);
			imageProgress.fillAmount = sliderValue;
		}
	}

	public void AddSeconds(int secondsToAdd){
		timeRemaining += secondsToAdd;
		timeRemaining = Mathf.Clamp (timeRemaining, 0, MaxTimeCounter);
	}

	public void PauseTimer()
	{
		if (IsInvoking ("ElapseTimer")) {
			CancelInvoke ("ElapseTimer");
		}
	}

	public void ResumeTimer()
	{
		if (!IsInvoking ("ElapseTimer")) {
			InvokeRepeating ("ElapseTimer", 0.1F, 0.1F);
		}
	}

	public int GetRemainingTime()
	{
		return (int)timeRemaining;
	}

	public void SetTime(int timeSeconds)
	{
		timeRemaining = timeSeconds;
	}
}

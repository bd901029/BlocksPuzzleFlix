using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif

public class Shop : MonoBehaviour 
{
	public void OnCloseButtonPressed()
	{
		if (InputManager.Instance.canInput ()) {
			AudioManager.Instance.PlayButtonClickSound ();
			StackManager.Instance.OnCloseButtonPressed ();
		}
	}
	#if UNITY_PURCHASING
	public void OnPurchaseSuccessful(Product product)
	{
		StackManager.Instance.SpawnUIScreen ("PurchaseSuccess");
		PurchaseRewardsManager.Instance.ProcessRewardForProduct (product);
	}
	#endif

	#if UNITY_PURCHASING
	public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
	{
		StackManager.Instance.SpawnUIScreen ("PurchaseFail");
	}
	#endif
}

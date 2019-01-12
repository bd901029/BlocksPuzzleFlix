using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif

public class PurchaseRewardsManager : Singleton<PurchaseRewardsManager> 
{
	#if UNITY_PURCHASING
	public void ProcessRewardForProduct(Product product)
	{
		switch (product.definition.id) {
		case "com.blockmagic.coin1":
			CurrencyManager.Instance.AddCoinBalance (500);
			break;
		case "com.blockmagic.coin2":
			CurrencyManager.Instance.AddCoinBalance (1650);
			break;
		case "com.blockmagic.coin3":
			CurrencyManager.Instance.AddCoinBalance (3000);
			break;
		case "com.blockmagic.coin4":
			CurrencyManager.Instance.AddCoinBalance (6250);
			break;
		}
	}
	#endif
}

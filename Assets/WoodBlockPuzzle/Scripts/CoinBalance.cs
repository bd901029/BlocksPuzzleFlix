using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinBalance : MonoBehaviour 
{
	public Text txtCoinBalance;

	/// <summary>
	/// Raises the enable event.
	/// </summary>
	void OnEnable()
	{
		CurrencyManager.OnCoinBalanceUpdated += OnCoinBalanceUpdated;	

		int coinBalance = CurrencyManager.Instance.GetCoinBalance ();
		OnCoinBalanceUpdated (coinBalance);
	}

	/// <summary>
	/// Raises the disable event.
	/// </summary>
	void OnDisable() {
		CurrencyManager.OnCoinBalanceUpdated -= OnCoinBalanceUpdated;	
	}

	/// <summary>
	/// Raises the coin balance updated event.
	/// </summary>
	/// <param name="coinBalance">Coin balance.</param>
	void OnCoinBalanceUpdated (int coinBalance)
	{
		StartCoroutine(SetCoinBalance(coinBalance));
	}

	/// <summary>
	/// Sets the coin balance.
	/// </summary>
	/// <returns>The coin balance.</returns>
	/// <param name="coinBalance">Coin balance.</param>
	IEnumerator SetCoinBalance(int coinBalance)
	{
		int oldBalance = 0;
		int.TryParse (txtCoinBalance.text.Replace(",",""), out oldBalance);

		int IterationSize = (coinBalance - oldBalance) / 50;

		for (int index = 1; index < 50; index++) {
			oldBalance += IterationSize;
			txtCoinBalance.text =  string.Format("{0:#,#.}", oldBalance);
			yield return new WaitForEndOfFrame ();
		}
		txtCoinBalance.text =  string.Format("{0:#,#.}", coinBalance);
	}
}

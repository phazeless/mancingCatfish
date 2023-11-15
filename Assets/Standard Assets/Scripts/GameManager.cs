using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	private void Start()
	{
		
		GameObject gameObject = GameObject.Find("Information");
		GameManager._textField = gameObject.GetComponent<Text>();
		GameManager._textField.text = "Please Click Init to Start";
		gameObject = GameObject.Find("Dropdown");
		this._dropdown = gameObject.GetComponent<Dropdown>();
		this._dropdown.ClearOptions();
		this._dropdown.options.Add(new Dropdown.OptionData(this.Product1));
		this._dropdown.options.Add(new Dropdown.OptionData(this.Product2));
		this._dropdown.RefreshShownValue();
		this.InitUI();
	}

	private static void Show(string message, bool append = false)
	{
		GameManager._textField.text = ((!append) ? message : string.Format("{0}\n{1}", GameManager._textField.text, message));
	}

	private void InitUI()
	{
		this.GetButton("InitButton").onClick.AddListener(delegate()
		{
			GameManager._initialized = false;
			UnityEngine.Debug.Log("Init button is clicked.");
			GameManager.Show("Initializing", false);
			
		});
		this.GetButton("BuyButton").onClick.AddListener(delegate()
		{
			if (!GameManager._initialized)
			{
				GameManager.Show("Please Initialize first", false);
				return;
			}
			string text = this._dropdown.options[this._dropdown.value].text;
			UnityEngine.Debug.Log("Buy button is clicked.");
			GameManager.Show("Buying Product: " + text, false);
			GameManager.m_consumeOnPurchase = false;
			UnityEngine.Debug.Log(this._dropdown.options[this._dropdown.value].text + " will be bought");
			
		});
		this.GetButton("BuyConsumeButton").onClick.AddListener(delegate()
		{
			if (!GameManager._initialized)
			{
				GameManager.Show("Please Initialize first", false);
				return;
			}
			string text = this._dropdown.options[this._dropdown.value].text;
			GameManager.Show("Buying Product: " + text, false);
			UnityEngine.Debug.Log("Buy&Consume button is clicked.");
			GameManager.m_consumeOnPurchase = true;
			
		});
		List<string> productIds = new List<string>
		{
			this.Product1,
			this.Product2
		};
		this.GetButton("QueryButton").onClick.AddListener(delegate()
		{
			if (!GameManager._initialized)
			{
				GameManager.Show("Please Initialize first", false);
				return;
			}
			GameManager._consumeOnQuery = false;
			UnityEngine.Debug.Log("Query button is clicked.");
			GameManager.Show("Querying Inventory", false);
			
		});
		this.GetButton("QueryConsumeButton").onClick.AddListener(delegate()
		{
			if (!GameManager._initialized)
			{
				GameManager.Show("Please Initialize first", false);
				return;
			}
			GameManager._consumeOnQuery = true;
			GameManager.Show("Querying Inventory", false);
			UnityEngine.Debug.Log("QueryConsume button is clicked.");
			
		});
	}

	private Button GetButton(string buttonName)
	{
		GameObject gameObject = GameObject.Find(buttonName);
		if (gameObject != null)
		{
			return gameObject.GetComponent<Button>();
		}
		return null;
	}

	public string Product1;

	public string Product2;

	private static bool m_consumeOnPurchase;

	private static bool _consumeOnQuery;

	private Dropdown _dropdown;

	private List<Dropdown.OptionData> options;

	private static Text _textField;

	private static bool _initialized;

	

	
	
}

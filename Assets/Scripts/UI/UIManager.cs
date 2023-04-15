using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
	[SerializeField] private TextMeshProUGUI cashText;
	[SerializeField] private Slider heatSlider;
	[SerializeField] private Transform activeContractsParent;
	[SerializeField] private GameObject activeContractPrefab;
	private Dictionary<Contract, GameObject> activeContractDictionary = new ();

	[SerializeField] private bool gamePaused;
	
	public float Cash
	{
		set => cashText.text = $"{value:0.00}zł";
	}

	public float Heat
	{
		set => heatSlider.value = value;
	}

	public void AddActiveContract(Contract contract)
	{
		GameObject newContract = Instantiate(activeContractPrefab, transform.position, Quaternion.identity, activeContractsParent);
		activeContractDictionary.Add(contract, newContract);
		ActiveContractUI activeContractUI = newContract.GetComponent<ActiveContractUI>();
		activeContractUI.Description = contract.description;
		Debug.Log(activeContractDictionary.Count);
	}

	public void RemoveActiveContract(Contract contract)
	{
		Debug.Log(activeContractDictionary.Count);
		Destroy(activeContractDictionary[contract].gameObject);
		activeContractDictionary.Remove(contract);
	}

	public void RefreshActiveContract(Contract contract)
	{

	}

	public void TogglePause()
	{
		GameManager.Instance.GameSpeed = gamePaused ? 1f : 0f;
		gamePaused = !gamePaused;
	}
}

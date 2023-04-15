using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryForm : Window
{
	[SerializeField] private TMP_Dropdown contractDropdown;
	[SerializeField] private TMP_Dropdown smugglersDropdown;
	[SerializeField] private Transform smugglerPanel;
	[SerializeField] private TMP_Dropdown roadDropdown;
	[SerializeField] private TMP_Dropdown wrappingDropdown;
	[SerializeField] private TMP_Dropdown vehicleDropdown;
	[SerializeField] private TextMeshProUGUI deliveryCostsText;
	[SerializeField] private TextMeshProUGUI estimatedEarningsText;
	[SerializeField] private Button sendButton;

	private void Start()
	{
		roadDropdown.ClearOptions();
		List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
		options.Add(new TMP_Dropdown.OptionData{
			text = "Wybierz drogę"
		});
		foreach (Road road in Map.Instance.roads)
		{
			TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
			{
				text = road.roadName,
				image = road.sprite
			};
			options.Add(optionData);
		}
		roadDropdown.AddOptions(options);
	}

	private void OnEnable()
	{
		GenerateContracts();
		GenerateSmugglers();
		GenerateVehicles();
		GenerateWrappings();
		
		CalculateDelivery();
	}

	public void GenerateContracts()
	{
		contractDropdown.ClearOptions();
		List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
		options.Add(new TMP_Dropdown.OptionData{
			text = "Wybierz kontrakt"
		});
		foreach (Contract contract in GameManager.Instance.activeContracts)
		{
			TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
			{
				text = contract.description,
				image = GameData.Instance.organisationsData.organisationsDictionary[contract.organisation].logo
			};
			options.Add(optionData);
		}
		contractDropdown.AddOptions(options);
	}

	public void SetContract(TMP_Dropdown change)
	{
		Debug.Log($"{change.value} chosen");
		
		CalculateDelivery();
	}

	public void GenerateSmugglers()
	{
		smugglersDropdown.ClearOptions();
		List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
		options.Add(new TMP_Dropdown.OptionData{
			text = "Wybierz przemytnika"
		});
		foreach (Smuggler smuggler in GameManager.Instance.hiredSmugglers)
		{
			TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
			{
				text = smuggler.fullName,
				image = smuggler.portrait
			};
			options.Add(optionData);
		}

		smugglersDropdown.AddOptions(options);
	}

	public void SetSmuggler(TMP_Dropdown change)
	{
		Debug.Log($"{change.value} chosen");
		
		CalculateDelivery();
	}
	
	public void SetRoad(TMP_Dropdown change)
	{
		Debug.Log($"{change.value} chosen");
		
		CalculateDelivery();
	}

	public void GenerateVehicles()
	{
		vehicleDropdown.ClearOptions();
		List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
		options.Add(new TMP_Dropdown.OptionData{
			text = "Wybierz pojazd"
		});
		foreach (Vehicle vehicle in GameManager.Instance.vehicles)
		{
			TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
			{
				text = vehicle.vehicleName,
				image = vehicle.thumbnail
			};
			options.Add(optionData);
		}

		vehicleDropdown.AddOptions(options);
	}

	public void SetVehicle(TMP_Dropdown change)
	{
		Debug.Log($"{change.value} chosen");
		
		CalculateDelivery();
	}
	
	public void GenerateWrappings()
	{
		wrappingDropdown.ClearOptions();
		List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
		options.Add(new TMP_Dropdown.OptionData{
			text = "Wybierz sposób przewozu"
		});
		foreach (Wrapping wrapping in GameData.Instance.wrappingsData.wrappings)
		{
			TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
			{
				text = wrapping.wrappingName
			};
			options.Add(optionData);
		}

		wrappingDropdown.AddOptions(options);
	}
	
	public void SetWrapping(TMP_Dropdown change)
	{
		Debug.Log($"{change.value} chosen");
		
		CalculateDelivery();
	}
	
	void CalculateDelivery()
	{
		sendButton.interactable = false;
		float cost = 0f;
		float income = 0f;
		bool canSend = true;

		float contractPayment = 0;
		if (contractDropdown.value > 0)
			contractPayment = GameManager.Instance.activeContracts[contractDropdown.value - 1].payment;
		else
			canSend = false;

		float smugglerPayment = 0;
		if (smugglersDropdown.value > 0)
			smugglerPayment = GameManager.Instance.hiredSmugglers[smugglersDropdown.value - 1].paymentPercent * contractPayment;
		else
			canSend = false;

		float vehicleExploatationCost = 0;
		if (vehicleDropdown.value > 0)
			vehicleExploatationCost = GameManager.Instance.vehicles[vehicleDropdown.value - 1].exploatationCost;
		else
			canSend = false;

		float wrappingPrice = 0;
		if (wrappingDropdown.value > 0)
			wrappingPrice = GameData.Instance.wrappingsData.wrappings[wrappingDropdown.value - 1].price;
		else
			canSend = false;

		// TODO sprawdź czy w magazynie jest wystarczająco
		
		income += contractPayment;
		cost -= smugglerPayment;
		cost -= vehicleExploatationCost;
		cost -= wrappingPrice;

		deliveryCostsText.text = $"{cost:0.00}zł";
		estimatedEarningsText.text = $"+{income:0.00}zł";
		
		if (canSend)
			sendButton.interactable = true;
	}

	public void Send()
	{
		IllegalTransport illegalTransport = new IllegalTransport();

		illegalTransport.contract = GameManager.Instance.activeContracts[contractDropdown.value - 1];
		illegalTransport.smuggler = GameManager.Instance.hiredSmugglers[smugglersDropdown.value - 1];
		illegalTransport.vehicle = GameManager.Instance.vehicles[vehicleDropdown.value - 1];
		illegalTransport.wrapping = GameData.Instance.wrappingsData.wrappings[wrappingDropdown.value - 1];
		// TODO pobierz z magazynu
		// illegalTransport.goods

		Map.Instance.roads[roadDropdown.value - 1].SmuggleUsingThisRoad(illegalTransport.vehicle.prefab, illegalTransport);
		Hide();
	}
}

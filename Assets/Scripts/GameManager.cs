using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	[SerializeField] private float cash;
	public List<Contract> activeContracts = new();
	public List<Smuggler> hiredSmugglers = new();
	public List<Vehicle> vehicles = new();
	
	public float Cash
	{
		get => cash;
		set {
			cash = value;
			UIManager.Instance.Cash = cash;
		}
	}

	public float GameSpeed {
		set => Time.timeScale = value;
	}

	public int MaxContractsLimit { get; set; }
	public int MaxSmugglersLimit { get; set; }
	public int MaxVehiclesLimit { get; set; }

	protected override void Awake()
	{
		base.Awake();
		Cash = 2500;
		MaxContractsLimit = 3;
		MaxSmugglersLimit = 3;
		MaxVehiclesLimit = 3;
		StartCoroutine(ECrewRest());
		StartCoroutine(ERefreshContracts());
	}

	public void AddContract(Contract contract)
	{
		if (activeContracts.Count >= MaxContractsLimit)
		{
			Debug.LogWarning("Max contracts limit reached!");
			return;
		}
		
		activeContracts.Add(contract);
		UIManager.Instance.AddActiveContract(contract);
	}

	public void RemoveContract(Contract contract)
	{
		activeContracts.Remove(contract);
		UIManager.Instance.RemoveActiveContract(contract);
	}

	public void HireSmuggler(Smuggler smuggler)
	{
		if (hiredSmugglers.Count >= MaxSmugglersLimit)
		{
			Debug.LogWarning("Max smugglers limit reached!");
			return;
		}

		Cash -= smuggler.hirePrice;
		hiredSmugglers.Add(smuggler);
		UIManager.Instance.SetSmugglerText();
	}

	public void BuyVehicle(Vehicle vehicle)
	{
		if (vehicles.Count >= MaxVehiclesLimit)
		{
			Debug.LogWarning("Max vehicles limit reached!");
			return;
		}

		Cash -= vehicle.price;
		vehicles.Add(vehicle);
        UIManager.Instance.SetCarText();
        UIManager.Instance.SetPlaneText();
        UIManager.Instance.SetBoatText();
    }

	IEnumerator ECrewRest()
	{
		while (true)
		{
			foreach (Smuggler smuggler in hiredSmugglers)
			{
				smuggler.tiredness -= Random.Range(0.01f, 0.1f);
				if (smuggler.tiredness < 0)
				{
					smuggler.tiredness = 0;
				}
			}
			yield return new WaitForSeconds(10f);
		}
	}

	IEnumerator ERefreshContracts()
	{
		while (true)
		{
			foreach (Contract contract in activeContracts)
			{
				UIManager.Instance.RefreshActiveContract(contract);
			}
			yield return new WaitForSeconds(1f);
		}
	}
}
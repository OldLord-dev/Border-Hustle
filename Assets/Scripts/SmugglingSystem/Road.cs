using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Road : MonoBehaviour
{
	public string roadName;
	public Sprite sprite;
	public VehicleType vehicleType;
	[SerializeField] private List<Waypoint> waypoints;
	
	public void SmuggleUsingThisRoad(GameObject vehiclePrefab, IllegalTransport illegalTransport)
	{
		StartCoroutine(EDriving(vehiclePrefab, illegalTransport));
	}

	private IEnumerator EDriving(GameObject vehiclePrefab, IllegalTransport illegalTransport)
	{
		Debug.Log(roadName);
		Transform vehicle = Instantiate(vehiclePrefab, waypoints[0].transform.position, waypoints[0].transform.rotation, transform).transform;
		Debug.Log("start driving");
		int currentWaypoint = 1;

		while (currentWaypoint < waypoints.Count)
		{
			Vector3 targetPosition = waypoints[currentWaypoint].transform.position;
			Waypoint currentWaypointObject = waypoints[currentWaypoint].GetComponent<Waypoint>();

			while (Vector3.Distance(vehicle.transform.position, targetPosition) > 2f)
			{
				Vector3 direction = waypoints[currentWaypoint].transform.position - vehicle.transform.position;
				Quaternion targetRotation = Quaternion.LookRotation(direction);
				
				// FIXME dostosować szybkość skręcania
				vehicle.transform.rotation = Quaternion.Lerp(vehicle.transform.rotation, targetRotation, Time.deltaTime * 2f);
				vehicle.transform.position += vehicle.transform.forward * (illegalTransport.vehicle.speed * Time.deltaTime * currentWaypointObject.speedModifier);
				yield return null;
			}
			
			if (waypoints[currentWaypoint] is BorderCrossing)
			{
				Debug.Log("border crossing");
				BorderCrossing borderCrossing = (BorderCrossing)waypoints[currentWaypoint];
				yield return new WaitForSeconds(borderCrossing.checkInTime);

				// TODO
				float skillCheck = Random.Range(0f, 1f);
				if (skillCheck < borderCrossing.baseFailPercentage)
				{
					Destroy(gameObject);
					Debug.LogWarning("Smuggler was caught");
					yield break;
				}
			}
			
			currentWaypoint++;
		}

		yield return new WaitForSeconds(illegalTransport.vehicle.unloadTime);
		// TODO rozładuj ładunek
		foreach (KeyValuePair<string, int> loadPair in illegalTransport.vehicle.load)
		{
			illegalTransport.contract.goods[loadPair.Key].item1 += loadPair.Value;
			if (illegalTransport.contract.goods[loadPair.Key].item1 > illegalTransport.contract.goods[loadPair.Key].item2)
				illegalTransport.contract.goods[loadPair.Key].item1 = illegalTransport.contract.goods[loadPair.Key].item2;
		}
		
		List<Waypoint> reverseWaypoints = new List<Waypoint>(waypoints);
		reverseWaypoints.Reverse();
		
		Debug.Log(roadName);
		Debug.Log("driving back");
		currentWaypoint = 1;

		while (currentWaypoint < reverseWaypoints.Count)
		{
			Vector3 targetPosition = reverseWaypoints[currentWaypoint].transform.position;
			Waypoint currentWaypointObject = reverseWaypoints[currentWaypoint].GetComponent<Waypoint>();

			while (Vector3.Distance(vehicle.transform.position, targetPosition) > 2f)
			{
				Vector3 direction = reverseWaypoints[currentWaypoint].transform.position - vehicle.transform.position;
				Quaternion targetRotation = Quaternion.LookRotation(direction);
				
				// FIXME dostosować szybkość skręcania
				vehicle.transform.rotation = Quaternion.Lerp(vehicle.transform.rotation, targetRotation, Time.deltaTime * illegalTransport.vehicle.speed / 3f);
				vehicle.transform.position += vehicle.transform.forward * (illegalTransport.vehicle.speed * Time.deltaTime * currentWaypointObject.speedModifier);
				yield return null;
			}
			
			if (reverseWaypoints[currentWaypoint] is BorderCrossing)
			{
				Debug.Log("border crossing");
				BorderCrossing borderCrossing = (BorderCrossing)reverseWaypoints[currentWaypoint];
				yield return new WaitForSeconds(borderCrossing.checkInTime);

				// TODO
				float skillCheck = Random.Range(0f, 1f);
				if (skillCheck < borderCrossing.baseFailPercentage)
				{
					Destroy(gameObject);
					Debug.LogWarning("Smuggler was caught");
					yield break;
				}
			}
			
			currentWaypoint++;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonPart : MonoBehaviour
{
	#region ATTRIBUTES
	[Header("Global attributes")]
	[Space]
	public bool isStartPart;
	public bool isFocused;
	public bool isInitialized;
	public ActioNeededForOpeningDoors actionNeeded; 

	[Header("Start position")]
	[Space]
	public Transform startPosition;

	[Header("Entrances points positions")]
	[Space]
	public Transform topEntrance;
	public Transform leftEntrance;
	public Transform rightEntrance;
	public Transform bottomEntrance;

	[Header("Exit points management")]
	[Space]
	public Transform[] endPositionsPosibilities;
	private Transform endPosition;
	private List<DoorManagement> loadingZones;
	#endregion

	#region EVENT
	public delegate void DoorOpeningOrder();
    public event DoorOpeningOrder OnDoorOpeningOrder;
	public delegate void DoorClosingOrder();
    public event DoorClosingOrder OnDoorClosingOrder;
	#endregion

	#region PROPERTIES
	public Guid ID { set; get; }
	#endregion

	#region UNITY EVENTS
	private void Awake()
	{
		loadingZones = GetComponentsInChildren<DoorManagement>().ToList();		
	}

	private void Update() 
	{
		CheckForInitialization();
	}
	#endregion

	#region METHODS
	private void CheckForInitialization()
	{
		if ((isFocused)	&& (!isInitialized))
		{
			switch(actionNeeded)
			{
				default:
				case ActioNeededForOpeningDoors.None:
				GetComponentsInChildren<DoorManagement>().ToList().ForEach(x => x.InitializeDoor(true));
				break;
				case ActioNeededForOpeningDoors.EnableSwitch:
				GetComponentsInChildren<DoorManagement>().ToList().ForEach(x => x.InitializeDoor(false));
				break;
				case ActioNeededForOpeningDoors.KillAllEnemies:
				break;
			}
			
			isInitialized = true;
		}
	}

	public void SendGameAction(object sender)
	{
		switch (actionNeeded)
		{
			case ActioNeededForOpeningDoors.None:
			if (sender is DungeonPart)
			{
				UnlockDoors();
			}
			break;
			case ActioNeededForOpeningDoors.EnableSwitch:
			if (sender is SwitchManagement)
			{
				UnlockDoors();
			}
			break;
			default:
			break;
		}
	}

	private void LockDoors()
	{
		if (isFocused)
		{
			OnDoorClosingOrder?.Invoke();
		}
	}

	private void UnlockDoors()
	{
		if (isFocused)
		{
			OnDoorOpeningOrder?.Invoke();
		}
	}

	public void GenerateEnd()
	{
		int index = UnityEngine.Random.Range(0, endPositionsPosibilities.Length - 1);
		endPosition = endPositionsPosibilities[index];
	}
	public Vector3 GetEntranceLocation(StartPoint start)
	{
		switch (start)
		{
			default:
			case StartPoint.Top:
				return topEntrance.position;
			case StartPoint.Bottom:
				return bottomEntrance.position;
			case StartPoint.Left:
				return leftEntrance.position;
			case StartPoint.Right:
				return rightEntrance.position;
		}
	}
	#endregion
}

public enum ActioNeededForOpeningDoors
{
	None, EnableSwitch, KillAllEnemies
}

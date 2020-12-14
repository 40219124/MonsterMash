using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartPickerManager : AdditiveSceneManager
{
	protected override void Awake()
	{
		base.Awake();
	}
	
	public void Setup()
	{
		PartPickerUiController picker = FindObjectOfType<PartPickerUiController>();
		var parts = OverworldMemory.GetLootProfile();
		// For getting new parts
		if (parts != null && parts.Count > 0)
		{
			picker.Setup(OverworldMemory.GetCombatProfile(true), parts);
		}
		// for stat viewing
		else
		{
			picker.Setup(OverworldMemory.GetCombatProfile(true), null);
		}
	}

	// when leaving clear the loot
	public void LeavePicker()
	{
		OverworldMemory.ClearLoot();
		FlowManager.Instance.TransToOverworld(Settings.SceneBodyPartPicker);
	}

	// Update is called once per frame
	void Update()
	{

	}
}

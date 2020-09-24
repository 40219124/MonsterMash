using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartPickerManager : AdditiveSceneManager
{
    public void Setup()
    {
        PartPickerUiController picker = FindObjectOfType<PartPickerUiController>();
        MonsterProfile monPro = OverworldMemory.GetLootProfile();
        // For getting new parts
        if (monPro != null)
        {
            List<BodyPartData> parts = new List<BodyPartData>();


			if(monPro.Torso != null &&
				(monPro.Torso.MonsterType == EMonsterType.lobster ||
				monPro.Torso.MonsterType == EMonsterType.shrimp))
			{
				parts.Add(monPro.Torso);
			}
			else
			{
				var toPickFrom = new List<BodyPartData>();
				if(monPro.LeftArm != null)
				{
					toPickFrom.Add(monPro.LeftArm);
				}
				if(monPro.RightArm != null)
				{
					toPickFrom.Add(monPro.RightArm);
				}
				if(monPro.Legs != null)
				{
					toPickFrom.Add(monPro.Legs);
				}

				int bodyPart = Random.Range(0, toPickFrom.Count);
				parts.Add(toPickFrom[bodyPart]);
			}

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartPickerManager : AdditiveSceneManager
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    public void Setup()
    {
        PartPickerUiController picker = FindObjectOfType<PartPickerUiController>();
        MonsterProfile monPro = OverworldMemory.GetLootProfile();
        // For getting new parts
        if (monPro != null)
        {
            List<BodyPartData> parts = new List<BodyPartData>();

            int bodyPart = Random.Range(0, 3);
            switch (bodyPart)
            {
                case 0:
                    parts.Add(monPro.LeftArm);
                    break;
                case 1:
                    parts.Add(monPro.RightArm);
                    break;
                case 2:
                    parts.Add(monPro.Legs);
                    break;
                default:
                    break;
            }
            // ~~~ 25% chance of second limb

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
        FindObjectOfType<FlowManager>().TransToOverworld(Settings.SceneBodyPartPicker);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

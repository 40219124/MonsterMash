using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : OverworldAgent
{
    [SerializeField]
    MonsterGenerator MGen;
    private void Start()
    {
        // ~~~ do better later
        Profile = MGen.GetMonster(EMonsterType.Frankenstein);
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        HorizontalValue = Input.GetAxisRaw("Horizontal");
        VerticalValue = Input.GetAxisRaw("Vertical");

        DoUpdate();

    }

    protected override bool OnTransition()
    {
        LockedMovement = true;
        if (IsMoving())
        {
            return false;
        }
        OverworldMemory.RecordPosition(transform.position);
        OverworldMemory.RecordProfile(Profile);
        return true;
    }
}

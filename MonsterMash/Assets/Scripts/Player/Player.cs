using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : OverworldAgent
{
    [SerializeField]
    MonsterGenerator MGen;
    protected override void Start()
    {
        // ~~~ do better later
        if (OverworldMemory.GetCombatProfile(true) == null)
        {
            Profile = MGen.GetMonster(EMonsterType.Frankenstein);
            OverworldMemory.RecordProfile(Profile);
        }
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        HorizontalValue = Input.GetAxisRaw("Horizontal");
        VerticalValue = Input.GetAxisRaw("Vertical");

        DoUpdate();

		CurrentRoom.Instance.TryCollectItems(transform.position);

        if (CurrentRoom.Instance.TileContent[(int)MoveTarget.x, (int)MoveTarget.y].HasFlag(CurrentRoom.ETileContentType.Door))
        {
            // ~~~ Lifespan of 1 game jam
            Vector2 diff = ((Vector2)MoveTarget - new Vector2(Room.GameWidth / 2.0f, Room.GameHeight / 2.0f)).normalized * 1.5f;
            Vector2Int diffInt = new Vector2Int((int)diff.x, (int)diff.y);

            if (diffInt != Vector2Int.left &&
				diffInt != Vector2Int.up &&
				diffInt != Vector2Int.down &&
				diffInt != Vector2Int.right)
            {
                Debug.LogError("Impossible");
            }
			else
			{
				LockedMovement = true;

				if(!IsMoving())
				{
					ProceduralDungeon.Instance.MoveRoom(diffInt, MoveTarget);

					FindObjectOfType<FlowManager>().TransToOverworld(Settings.SceneOverworld);
				}
			}
        }
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

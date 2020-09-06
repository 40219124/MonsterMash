using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOA : OverworldAgent
{
    static protected Player Target;

    private float FreezeTime = 1.0f;
    private float RemainingFreeze = 0.0f;
    protected override void Start()
    {
        base.Start();
        if (Target == null)
        {
            Target = FindObjectOfType<Player>();
        }
        RemainingFreeze = FreezeTime;
    }


    // Update is called once per frame
    void Update()
    {
        if(RemainingFreeze >= 0.0f)
        {
            RemainingFreeze -= Time.deltaTime;
            return;
        }

        for (int i = 0; i <= (int)EFourDirections.left; ++i)
        {
            foreach (var p in WhiskerInfo[i])
            {
                if (p.Key.Equals("Player"))
                {
                    StartBattle(this);
                }
            }
        }

        Vector3 diff = Target.MoveTarget - transform.position;

        EAxisDirection dir = EAxisDirection.none;


        if (diff.sqrMagnitude != 1 && diff != Vector3.zero)
        {
            if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            {
                ProbeDirection(diff, EAxisDirection.horizontal, out dir);
            }
            else
            {
                ProbeDirection(diff, EAxisDirection.vertical, out dir);
            }


        }


        switch (dir)
        {
            case EAxisDirection.horizontal:
                HorizontalValue = (diff.x < 0.0f ? -1.0f : 1.0f);
                VerticalValue = 0;
                break;
            case EAxisDirection.vertical:
                HorizontalValue = 0;
                VerticalValue = (diff.y < 0.0f ? -1.0f : 1.0f);
                break;
            default:
                HorizontalValue = 0;
                VerticalValue = 0;
                break;
        }
        DoUpdate();

    }

    void ProbeDirection(Vector3 diff, EAxisDirection priority, out EAxisDirection outDir)
    {
        outDir = EAxisDirection.none;

        EFourDirections smallDirH = EFourDirections.none;
        if (diff.x < 0.0f)
        {
            smallDirH = EFourDirections.left;
        }
        else if (diff.x > 0.0f)
        {
            smallDirH = EFourDirections.right;
        }
        if (smallDirH != EFourDirections.none && !ValidDirection(smallDirH))
        {
            smallDirH = EFourDirections.none;
        }

        EFourDirections smallDirV = EFourDirections.none;
        if (diff.y < 0.0f)
        {
            smallDirV = EFourDirections.down;
        }
        else if (diff.y > 0.0f)
        {
            smallDirV = EFourDirections.up;
        }
        if (smallDirV != EFourDirections.none && !ValidDirection(smallDirV))
        {
            smallDirV = EFourDirections.none;
        }


        switch (priority)
        {
            case EAxisDirection.horizontal:
                if (smallDirH != EFourDirections.none)
                {
                    outDir = EAxisDirection.horizontal;
                }
                else if (smallDirV != EFourDirections.none)
                {
                    outDir = EAxisDirection.vertical;
                }
                break;
            case EAxisDirection.vertical:
                if (smallDirV != EFourDirections.none)
                {
                    outDir = EAxisDirection.vertical;
                }
                else if (smallDirH != EFourDirections.none)
                {
                    outDir = EAxisDirection.horizontal;
                }
                break;
            default:
                break;
        }

        if (outDir == EAxisDirection.none)
        {
            outDir = priority;
        }
    }

}

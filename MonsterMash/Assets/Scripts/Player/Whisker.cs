using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EFourDirections { none = -1, up, right, down, left };
public class Whisker : MonoBehaviour
{
    [SerializeField]
    EFourDirections Direction;
    Vector3 DirectionVector;

    int TriggerCount = 0;

    static Player Player;
    // Start is called before the first frame update
    void Start()
    {
        if (Player == null)
        {
            Player = GetComponentInParent<Player>();
        }

        switch (Direction)
        {
            case EFourDirections.up:
                DirectionVector = Vector3.up;
                break;
            case EFourDirections.right:
                DirectionVector = Vector3.right;
                break;
            case EFourDirections.down:
                DirectionVector = Vector3.down;
                break;
            case EFourDirections.left:
                DirectionVector = Vector3.left;
                break;
            default:
                DirectionVector = Vector3.up;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(++TriggerCount  == 1)
        {
            Player.DirectionPossible(Direction, false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(--TriggerCount == 0)
        {
            Player.DirectionPossible(Direction, true);
        }
    }
}

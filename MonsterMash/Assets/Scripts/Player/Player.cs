using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum EAxisDirection { none = -1, horizontal, vertical };
    private float _horizontalValue = 0.0f;
    public float HorizontalValue
    {
        get { return _horizontalValue; }
        private set
        {
            _horizontalValue = value;
            if (value != 0)
            {
                TravelDirection = EAxisDirection.horizontal;
            }
            else
            {
                TravelDirection = EAxisDirection.none;
            }
        }
    }
    float _verticalValue = 0.0f;
    public float VerticalValue
    {
        get { return _verticalValue; }
        private set
        {
            _verticalValue = value;
            if (value != 0)
            {
                TravelDirection = EAxisDirection.vertical;
            }
            else
            {
                TravelDirection = EAxisDirection.none;
            }
        }
    }

    EAxisDirection _travelDirection = EAxisDirection.none;
    public EAxisDirection TravelDirection
    {
        get { return _travelDirection; }
        private set
        {
            if (value == _travelDirection)
            {
                return;
            }

            if (value == EAxisDirection.none)
            {
                if (HorizontalValue != 0.0f)
                {
                    _travelDirection = EAxisDirection.horizontal;
                }
                else if (VerticalValue != 0.0f)
                {
                    _travelDirection = EAxisDirection.vertical;
                }
                else
                {
                    _travelDirection = EAxisDirection.none;
                }
            }
            else
            {
                _travelDirection = value;
            }
        }
    }

    const float MoveCD = 1.0f;
    float RemainingMoveCD = 0.0f;

    bool[] ValidDirections = { true, true, true, true };
    EFourDirections CurrentMoveDir = EFourDirections.none;

    public float AnimationTime = 0.5f;
    public int PixelsPerStep = 16;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (RemainingMoveCD > 0)
        {
            RemainingMoveCD -= Time.deltaTime;
        }

        HorizontalValue = Input.GetAxisRaw("Horizontal");
        VerticalValue = Input.GetAxisRaw("Vertical");

        if(MoveAllowed(out EFourDirections nextDir))
        {
            CurrentMoveDir = nextDir;
            StartCoroutine(AnimatePlayerMovement());
        }

        /*if (RemainingMoveCD <= 0.0f)
        {
            switch (TravelDirection)
            {
                case EAxisDirection.horizontal:
                    {
                        if (MoveAllowed(out EFourDirections nextDir))
                        {
                            transform.Translate(Vector3.right * HorizontalValue);
                            RemainingMoveCD = MoveCD;
                        }
                    }
                    break;
                case EAxisDirection.vertical:
                    {
                        if (MoveAllowed(out EFourDirections nextDir))
                        {
                            transform.Translate(Vector3.up * VerticalValue);
                            RemainingMoveCD = MoveCD;
                        }
                    }
                    break;
                default:
                    break;
            }
        }*/
    }

    private bool MoveAllowed(out EFourDirections dir)
    {
        bool outBool = false;
        dir = EFourDirections.none;

        if (CurrentMoveDir != EFourDirections.none)
        {
            return outBool;
        }

        if (TravelDirection == EAxisDirection.horizontal)
        {
            if (HorizontalValue < 0 && ValidDirections[(int)EFourDirections.left])
            {
                dir = EFourDirections.left;
                outBool = true;
            }
            else if (HorizontalValue > 0 && ValidDirections[(int)EFourDirections.right])
            {
                dir = EFourDirections.right;
                outBool = true;
            }
        }
        else if (TravelDirection == EAxisDirection.vertical)
        {
            if (VerticalValue < 0 && ValidDirections[(int)EFourDirections.down])
            {
                dir = EFourDirections.down;
                outBool = true;
            }
            else if (VerticalValue > 0 && ValidDirections[(int)EFourDirections.up])
            {
                dir = EFourDirections.up;
                outBool = true;
            }
        }

        return outBool;
    }

    public void DirectionPossible(EFourDirections direction, bool state)
    {
        ValidDirections[(int)direction] = state;
    }

    IEnumerator AnimatePlayerMovement()
    {
        float timeElapsed = 0.0f;

        Vector3 start = transform.position;
        Vector3 dir = DirFromEnum(CurrentMoveDir);


        while(timeElapsed < AnimationTime)
        {
            yield return null;
            timeElapsed += Time.deltaTime;

            if(timeElapsed > AnimationTime)
            {
                timeElapsed = AnimationTime;
            }

            int progress = (int)(PixelsPerStep * timeElapsed / AnimationTime);

            transform.position = start + (dir * ((float)progress / PixelsPerStep));

        }
        CurrentMoveDir = EFourDirections.none;
        yield return null;
    }

    Vector3 DirFromEnum(EFourDirections dir)
    {
        switch (dir)
        {
            case EFourDirections.up:
                return Vector3.up;
            case EFourDirections.right:
                return Vector3.right;
            case EFourDirections.down:
                return Vector3.down;
            case EFourDirections.left:
                return Vector3.left;
            default:
                return Vector3.zero;
        }
    }
}

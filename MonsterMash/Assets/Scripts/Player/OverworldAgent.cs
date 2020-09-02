using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldAgent : MonoBehaviour
{
    public enum EAxisDirection { none = -1, horizontal, vertical };
    protected float _horizontalValue = 0.0f;
    public float HorizontalValue
    {
        get { return _horizontalValue; }
        protected set
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
    protected float _verticalValue = 0.0f;
    public float VerticalValue
    {
        get { return _verticalValue; }
        protected set
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
        protected set
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

    protected const float MoveCD = 1.0f;

    protected bool[] ValidDirections = { true, true, true, true };
    protected EFourDirections CurrentMoveDir = EFourDirections.none;
    protected EFourDirections CurrentFailDir = EFourDirections.none;
    public Vector3 MoveTarget;
    protected bool LockedMovement = false;

    public float AnimationTime = 0.5f;
    public int PixelsPerStep = 16;

    protected Animator Anim;

    public MonsterProfile Profile;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Anim = GetComponentInChildren<Animator>();
        FindObjectOfType<OverworldManager>().OnTransition += OnTransition;
    }

    protected void DoUpdate()
    {
        if (!LockedMovement)
        {
            if (TravelDirection != EAxisDirection.none)
            {
                if (MoveAllowed(out EFourDirections nextDir))
                {
                    CurrentMoveDir = nextDir;
                    StartCoroutine(AnimateSuccessfulMovement());
                }
                else
                {
                    if (CurrentMoveDir == EFourDirections.none)
                    {
                        CurrentMoveDir = nextDir;
                        StartCoroutine(AnimateFailedMovement());
                    }
                }
            }
        }
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
            if (HorizontalValue < 0)
            {
                dir = EFourDirections.left;
                if (ValidDirections[(int)EFourDirections.left])
                {
                    outBool = true;
                }
            }
            else if (HorizontalValue > 0)
            {
                dir = EFourDirections.right;
                if (ValidDirections[(int)EFourDirections.right])
                {
                    outBool = true;
                }
            }
        }
        else if (TravelDirection == EAxisDirection.vertical)
        {
            if (VerticalValue < 0)
            {
                dir = EFourDirections.down;
                if (ValidDirections[(int)EFourDirections.down])
                {
                    outBool = true;
                }
            }
            else if (VerticalValue > 0)
            {
                dir = EFourDirections.up;
                if (ValidDirections[(int)EFourDirections.up])
                {
                    outBool = true;
                }
            }
        }

        return outBool;
    }


    IEnumerator AnimateFailedMovement()
    {
        if (CurrentMoveDir == CurrentFailDir)
        {
            CurrentMoveDir = EFourDirections.none;
            yield break;
        }
        CurrentFailDir = CurrentMoveDir;
        CurrentMoveDir = EFourDirections.none;

        Anim.SetInteger("Direction", (int)CurrentFailDir);
        Anim.SetTrigger("Animate");


        yield return new WaitForSeconds(AnimationTime);

        CurrentFailDir = EFourDirections.none;
    }
    IEnumerator AnimateSuccessfulMovement()
    {
        float timeElapsed = 0.0f;

        Vector3 start = transform.position;
        Vector3 dir = DirFromEnum(CurrentMoveDir);
        MoveTarget = start + dir;
        MoveTarget = new Vector3(Mathf.Floor(MoveTarget.x + 0.5f), Mathf.Floor(MoveTarget.y + 0.5f), MoveTarget.z);

        Anim.SetInteger("Direction", (int)CurrentMoveDir);
        Anim.SetTrigger("Animate");

        while (timeElapsed < AnimationTime)
        {
            yield return null;
            timeElapsed += Time.deltaTime;

            if (timeElapsed > AnimationTime)
            {
                timeElapsed = AnimationTime;
            }

            int progress = (int)(PixelsPerStep * timeElapsed / AnimationTime);

            transform.position = MoveTarget - (dir * (1 - ((float)progress / PixelsPerStep)));

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

    public void SetDirectionPossible(EFourDirections direction, bool state)
    {
        ValidDirections[(int)direction] = state;
    }

    public void StartBattle(OverworldAgent opponent)
    {
        StartCoroutine(StartBattleRoutine(opponent));
    }

    IEnumerator StartBattleRoutine(OverworldAgent opponent)
    {
        FindObjectOfType<OverworldManager>().DoTransitionToBattle();
        yield return null;
        // ~~~ give monster profiles
    }

    // return true if ready
    protected virtual bool OnTransition()
    {
        if (IsMoving())
        {
            return false;
        }
        OverworldMemory.RecordPosition(transform.position, transform.GetInstanceID());
        OverworldMemory.RecordProfile(Profile, transform.GetInstanceID());
        return true;
    }

    protected bool IsMoving()
    {
        if (CurrentMoveDir == EFourDirections.none)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}

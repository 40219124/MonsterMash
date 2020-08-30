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

    private Animator Anim;
    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponentInChildren<Animator>();
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

        if (TravelDirection != EAxisDirection.none)
        {
            if (MoveAllowed(out EFourDirections nextDir))
            {
                CurrentMoveDir = nextDir;
                StartCoroutine(AnimatePlayerMovement());
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

    public void DirectionPossible(EFourDirections direction, bool state)
    {
        ValidDirections[(int)direction] = state;
    }

    IEnumerator AnimateFailedMovement()
    {
        Anim.SetInteger("Direction", (int)CurrentMoveDir);
        Anim.SetTrigger("Animate");

        yield return new WaitForSeconds(AnimationTime);

        Anim.SetBool("Animate", false);
        CurrentMoveDir = EFourDirections.none;
        yield return null;
    }
    IEnumerator AnimatePlayerMovement()
    {
        float timeElapsed = 0.0f;

        Vector3 start = transform.position;
        Vector3 dir = DirFromEnum(CurrentMoveDir);

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

            transform.position = start + (dir * ((float)progress / PixelsPerStep));

        }
        Anim.SetBool("Animate", false);
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

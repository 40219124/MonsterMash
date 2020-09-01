using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EButtonState { none = -1, pressed, held, released };
public enum EInput { none = -1, dpadUp, dpadRight, dpadDown, dpadLeft, A, B, start, select };
public class SimpleInput : MonoBehaviour
{
    public struct ButtonInfo
    {
        EInput button;
        EButtonState state;
        bool active;

        public EButtonState State
        {
            get { return state; }
            private set { state = value; }
        }
        public bool Active
        {
            get { return active; }
            private set
            {
                active = value;
            }
        }

        public ButtonInfo(EInput b)
        {
            button = b;
            state = EButtonState.none;
            active = false;
        }

        public void SetActive(bool _active)
        {
            active = _active;

            SetState(_active);
        }

        private void SetState(bool _active)
        {
            if (_active)
            {
                if (state == EButtonState.held || state == EButtonState.pressed)
                {
                    state = EButtonState.held;
                }
                else
                {
                    state = EButtonState.pressed;
                }
            }
            else
            {
                if (state == EButtonState.held || state == EButtonState.pressed)
                {
                    state = EButtonState.released;
                }
                else
                {
                    state = EButtonState.none;
                }
            }
        }
    }

    static List<ButtonInfo> Buttons = new List<ButtonInfo>();

    List<string> AxisStrings = new List<string>() {
         "Vertical", "Horizontal" , "Vertical" ,"Horizontal" , "ButtonA", "ButtonB", "Submit", "Cancel" };
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < (int)EInput.select; ++i)
        {
            Buttons.Add(new ButtonInfo((EInput)i));
        }
    }

    // Update is called once per frame
    void Update()
    {
        Buttons[(int)EInput.dpadUp].SetActive(Input.GetAxisRaw(AxisStrings[(int)EInput.dpadUp]) > 0);
        Buttons[(int)EInput.dpadRight].SetActive(Input.GetAxisRaw(AxisStrings[(int)EInput.dpadRight]) > 0);
        Buttons[(int)EInput.dpadDown].SetActive(Input.GetAxisRaw(AxisStrings[(int)EInput.dpadDown]) < 0);
        Buttons[(int)EInput.dpadLeft].SetActive(Input.GetAxisRaw(AxisStrings[(int)EInput.dpadLeft]) < 0);
        for (int i = (int)EInput.A; i < (int)EInput.select; ++i)
        {
            Buttons[i].SetActive(Input.GetAxisRaw(AxisStrings[i]) != 0);
        }
    }

    public static EButtonState GetInputState(EInput input)
    {
        return Buttons[(int)input].State;
    }

    public static bool GetInputActive(EInput input)
    {
        return Buttons[(int)input].Active;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EButtonState { none = -1, Pressed, Held, Released };
public enum EInput { none = -1, dpadUp, dpadRight, dpadDown, dpadLeft, A, B, Start, Select };
public class SimpleInput : MonoBehaviour
{
    static SimpleInput instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
			for (int i = 0; i <= (int)EInput.Select; ++i)
			{
				Buttons.Add(new ButtonInfo((EInput)i));
			}
        }
        else
        {
            enabled = false;
        }
    }

    float DeadZone = 0.5f;

    public class ButtonInfo
    {
        EInput button;
        bool isDpad;

        public EButtonState State { get; private set;}
        public bool Active { get; private set; }

        public ButtonInfo(EInput b)
        {
            button = b;
            State = EButtonState.none;
            Active = false;
            isDpad = (button == EInput.dpadUp || button == EInput.dpadRight || button == EInput.dpadDown || button == EInput.dpadLeft);
        }

		public override string ToString()
		{
			return $"Active: {Active}, state: {State}";
		}

        public void SetActive(bool active)
        {
			if (ScreenTransitionManager.IsAnimating)
			{
				active = false;
			}

            Active = active;
            if (Active)
            {
                if (State == EButtonState.Held || State == EButtonState.Pressed)
                {
                    State = EButtonState.Held;
                }
                else
                {
                    State = EButtonState.Pressed;
                    if(isDpad)
                    {
                        recentDpadInput = button;
                    }
                }
            }
            else
            {
                if (State == EButtonState.Held || State == EButtonState.Pressed)
                {
                    State = EButtonState.Released;
                }
                else
                {
                    State = EButtonState.none;
                }
            }
        }
    }

    static List<ButtonInfo> Buttons = new List<ButtonInfo>();

    static EInput recentDpadInput;

    List<string> AxisStrings = new List<string>() {
         "Vertical", "Horizontal" , "Vertical" ,"Horizontal" , "ButtonA", "ButtonB", "Start", "Select" };

    // Update is called once per frame
    void Update()
    {
        Buttons[(int)EInput.dpadUp].SetActive(Input.GetAxisRaw(AxisStrings[(int)EInput.dpadUp]) > DeadZone);
        Buttons[(int)EInput.dpadRight].SetActive(Input.GetAxisRaw(AxisStrings[(int)EInput.dpadRight]) > DeadZone);
        Buttons[(int)EInput.dpadDown].SetActive(Input.GetAxisRaw(AxisStrings[(int)EInput.dpadDown]) < -DeadZone);
        Buttons[(int)EInput.dpadLeft].SetActive(Input.GetAxisRaw(AxisStrings[(int)EInput.dpadLeft]) < -DeadZone);
        for (int i = (int)EInput.A; i <= (int)EInput.Select; ++i)
        {
            Buttons[i].SetActive(Input.GetButton(AxisStrings[i]));
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

    public static EInput GetRecentDpad()
    {
        return recentDpadInput;
    }
}

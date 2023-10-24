using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public enum InputType
    {
        None, User
    }

    public float horizontalInput { get; private set; }
    public bool doJump { get; private set; }

    private InputType type;

    [SerializeField] private string controlsPrefix; // For example: "Player 1" for "Player 1 Horizontal" movement

    public bool canAct { get; private set; }
    private void Start()
    {
        canAct = true;

        if (controlsPrefix != "")
        {
            controlsPrefix += " ";
        }
    }
    public virtual void Update()
    {
        horizontalInput = 0;
        if (canAct)
        {
            if (type == InputType.User)
            {
                horizontalInput = Input.GetAxisRaw(controlsPrefix + "Horizontal");
                doJump = Input.GetButtonDown(controlsPrefix + "Jump");
            }
        }
    }
}

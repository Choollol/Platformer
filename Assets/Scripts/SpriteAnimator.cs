using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(InputController), typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour
{
    /*
     * Requires animator with animation clips in base layer
     * Animation naming: SpriteName_Action
     * Facing right is default direction, left is when sprite is flipped
     */
    public enum Action
    {
        Idle, Run, Jump
    }

    private Animator animator;
    private InputController inputController;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private string spriteName;

    private Action action;
    
    public virtual void Start()
    {
        animator = GetComponent<Animator>();
        inputController = GetComponent<InputController>();
    }

    void Update()
    {
        if (inputController.canAct)
        {
            DirectionUpdate();
            ActionUpdate();
        }

        animator.Play("Base Layer." + spriteName + "_" + action);
    }
    protected virtual void DirectionUpdate()
    {
        if (inputController.horizontalInput < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (inputController.horizontalInput > 0)
        {
            spriteRenderer.flipX = false;
        }
    }
    private void ActionUpdate()
    {
        if (inputController.doJump)
        {
            action = Action.Jump;
        }
        else if (inputController.horizontalInput != 0)
        {
            action = Action.Run;
        }
        else
        {
            action = Action.Idle;
        }
    }
    public void SetAction(Action newAction)
    {
        action = newAction;
    }
}

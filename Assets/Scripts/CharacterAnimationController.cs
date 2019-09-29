using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    public Animator anim;

    //0 = idle;
    //1 = runForward;
    //2 = walkLeft;
    //3 = walkRight;
    //4 = walkBackward;
    //5 = die;

    public void runAnim()
    {
        anim.SetInteger("status", 1);
    }

    public void idleAnim()
    {
        anim.SetInteger("status", 0);
    }

    public void walkBackwardAnim()
    {
        anim.SetInteger("status", 4);
    }

    public void walkLeftAnim()
    {
        anim.SetInteger("status", 3);
    }

    public void walkRightAnim()
    {
        anim.SetInteger("status", 2);
    }

    public void attackAnim()
    {
        anim.SetInteger("status", 0);
        anim.SetTrigger("attack");
    }

    public void getHitAnim()
    {
        anim.SetInteger("status", 0);
        anim.SetTrigger("getHit");
    }

    public void dieAnim()
    {
        anim.SetInteger("status", 0);
        anim.SetBool("die", true);
    }

    public void winAnim()
    {
        anim.SetInteger("status", 0);
        anim.SetBool("victory", true);
        anim.SetTrigger("win");
    }

    void Start()
    {
        anim = GetComponent<Animator>();
    }
}

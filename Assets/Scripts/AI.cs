using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AI : MonoBehaviour
{
    public GameObject deathParticle;
    public Slider hpBar;
    public Slider playerHPBar;
    public GameObject gameManager;
    private bool isDead = false;
    private Animator anim;
    public int maxHP;
    public int currentHP;
    public bool canMove = true;
    public bool canAttack = true;
    public int moveSpeed = 5;
    public CharacterController controller;
    float h = 0f;
    float v = 0f;
    bool isWin = false;
    public GameObject target;
    public GameObject[] skills;
    public GameObject spawn;
    #region Animation Controller
    //0 = idle;
    //1 = runForward;
    //2 = walkLeft;
    //3 = walkRight;
    //4 = walkBackward;
    //5 = die;

    void runAnim()
    {
        anim.SetInteger("status", 1);
    }

    void idleAnim()
    {
        anim.SetInteger("status", 0);
    }

    void walkBackwardAnim()
    {
        anim.SetInteger("status", 4);
    }

    void walkLeftAnim()
    {
        anim.SetInteger("status", 3);
    }

    void walkRightAnim()
    {
        anim.SetInteger("status", 2);
    }
    #endregion
    public void getHit(int damage)
    {
        if (!isDead && !isWin)
        {
            StopAllCoroutines(); //Disrupt attacks
            anim.SetTrigger("getHit");
            anim.SetInteger("status", 0);
            currentHP -= damage;
            Debug.Log("Current HP: " + currentHP);
            hpBar.value = currentHP;
            canMove = true;
            if (currentHP <= 0)
            {
                die();
                die();
            }
        }
    }
    public void die()
    {
        CancelInvoke();
        anim.SetInteger("status", 0);
        canMove = false;
        canAttack = false;
        isDead = true;
        anim.SetBool("die", true);
        gameManager.GetComponent<GameManager>().win();
        deathParticle.SetActive(true);
    }

    private void move()
    {
        if (!canMove || isDead) return;
        if (v > 0)
        {
            if (Mathf.Abs(v) > Mathf.Abs(h))
            {
                runAnim();
            }
            else
            {
                v /= 3;
                if (h > 0)
                {
                    walkRightAnim();
                }
                else
                {
                    walkLeftAnim();
                }
            }
        }
        else
        {
            v /= 3;
            if (Mathf.Abs(v) > Mathf.Abs(h))
            {
                walkBackwardAnim();
            }
            else
            {
                if (h > 0)
                {
                    walkRightAnim();
                }
                else
                {
                    walkLeftAnim();
                }
            }
        }
        if (0 == h && 0 == v)
        {
            idleAnim();
        }
        Vector3 movement = new Vector3(h / 3, 0.0f, v);
        movement = transform.TransformDirection(movement);
        controller.Move(movement * moveSpeed * Time.deltaTime);
    }
    void moveGenerate()
    {
        if (transform.position.x <= 0f)
            h = Random.Range(-1f, 0f);
        else h = Random.Range(0f, 1f);
        if (transform.position.z <= 0f)
            v = Random.Range(-1f, 0f);
        else v = Random.Range(0f, 1f);
    }
    public void attack()
    {
        if (canAttack)
        {
            transform.LookAt(target.transform);
            anim.SetInteger("status", 0);
            anim.SetTrigger("attack");
            canMove = false;
            StartCoroutine(cooldownATK(1));
            int skillSlot = Random.Range(0, 3);
            StartCoroutine(skillSpawn(skillSlot));
        }

    }

    IEnumerator cooldownATK(float attackCD)
    {
        yield return new WaitForSeconds(attackCD);
        canMove = true;
    }

    IEnumerator skillSpawn(int skillSlot)
    {
        yield return new WaitForSeconds(0.5f);
        Instantiate(skills[skillSlot], spawn.transform.position, spawn.transform.rotation);
    }
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        hpBar.maxValue = maxHP;
        hpBar.value = maxHP;
        controller = GetComponent<CharacterController>();
        InvokeRepeating("moveGenerate", 1, 3);
        InvokeRepeating("attack", 2, 4);
    }
    public void win()
    {
        this.enabled = false;
        CancelInvoke();
        anim.SetInteger("status", 0);
        anim.SetBool("victory", true);
        anim.SetTrigger("win");
    }
    // Update is called once per frame
    private void Update()
    {
        if (playerHPBar.value <= 0 && !isWin)
        {
            win();
        }
    }
    void FixedUpdate()
    {
        if (canMove) move();
    }
}

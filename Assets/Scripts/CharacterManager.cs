using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    #region Private

    private CharacterController controller;

    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float gravity = 20f;

    [SerializeField]
    float attackCD = 1f;

    [SerializeField]
    float maxHP = 500;

    [SerializeField]
    float flashCD = 10f;

    float nextAttack; //Time that next attack available

    float nextFlash;

    AnimatorStateInfo currentState;

    bool isDead;

    bool isWin;

    Camera playerCamera;

    #endregion
    #region Public Fields
    public float currentHP = 500;
    public GameObject[] Skill;

    public GameObject spawn; //Skill spawner

    public GameObject deathParticle; //Death particle

    public GameObject flashParticle; //Flash particle

    Vector3 flashPosition;

    public float rotateSpeed = 5;

    Vector3 offset; //Camera <> Player

    public static GameObject LocalPlayerInstance;

    public GameObject gameManager;

    public Slider hpBar;

    public Slider enemyHPBar;

    public FixedJoystick movementJoystick;

    public FixedJoystick rotateJoystick;

    public float flashDistance = 10f;

    public bool canAttack;
    public bool canMove;
    #endregion
    #region Character Controller

    private void move()
    {
        float h = movementJoystick.Horizontal;
        float v = movementJoystick.Vertical;
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
                    walkLeftAnim();
                }
                else
                {
                    walkRightAnim();
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
                    walkLeftAnim();
                }
                else
                {
                    walkRightAnim();
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

    public void skill(int skillSlot)
    {
        if (!isDead && !isWin)
        {
            if (nextAttack < Time.time)
            {
                attack(skillSlot);
            }
        }
    }

    public void flash()
    {
        if (Time.time > nextFlash)
        {
            flashPosition.y += 1f;
            Instantiate(flashParticle, flashPosition, transform.rotation);
            controller.Move(transform.forward * flashDistance);
            nextFlash = Time.time + flashCD;
        }
    }

    public void attack(int skillSlot)
    {
        anim.SetInteger("status", 0);
        anim.SetTrigger("attack");
        nextAttack = Time.time + attackCD;
        canMove = false;
        StartCoroutine(cooldownATK(attackCD));
        StartCoroutine(skillSpawn(skillSlot));
    }

    IEnumerator cooldownATK(float attackCD)
    {
        yield return new WaitForSeconds(attackCD);
        canMove = true;
    }

    IEnumerator skillSpawn(int skillSlot)
    {
        yield return new WaitForSeconds(0.5f);
        Instantiate(Skill[skillSlot], spawn.transform.position, spawn.transform.rotation);
    }

    public void getHit(int damage)
    {
        if (!isWin && !isDead)
        {
            StopAllCoroutines(); //Disrupt attacks
            anim.SetTrigger("getHit");
            anim.SetInteger("status", 0);
            currentHP -= damage;
            hpBar.value = currentHP;
            if (currentHP <= 0)
            {
                die();
            }
            canMove = true;
        }
    }

    public void die()
    {
        gameObject.GetComponent<CharacterGesture>().enabled = false;
        gameObject.GetComponent<CharacterGesture>().clear();
        gameManager.GetComponent<GameManager>().lose();
        anim.SetInteger("status", 0);
        isDead = true;
        anim.SetBool("die", true);
        deadCamera();
        Time.timeScale = 0.5f;
        Invoke("resetTimeScale", 2);
        deathParticle.SetActive(true);
        this.enabled = false;
    }

    public void win()
    {
        gameObject.GetComponent<CharacterGesture>().enabled = false;
        gameObject.GetComponent<CharacterGesture>().clear();
        anim.SetInteger("status", 0);
        anim.SetBool("victory", true);
        anim.SetTrigger("win");
        isWin = true;
        deadCamera();
        Time.timeScale = 0.5f;
        Invoke("resetTimeScale", 2);
        this.enabled = false;
    }

    void resetTimeScale()
    {
        Time.timeScale = 1f;
    }

    void deadCamera()
    {
        playerCamera.GetComponent<CameraMoveUponDeath>().enabled = true;
    }

    #endregion
    #region Animation Controller
    //0 = idle;
    //1 = runForward;
    //2 = walkLeft;
    //3 = walkRight;
    //4 = walkBackward;
    //5 = die;
    Animator anim;

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
    #region MonoBehaviour Callbacks

    void Awake()
    {

    }

    void Start()
    {
        canMove = true;
        canAttack = true;
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main;
        offset = transform.position - playerCamera.transform.position;
        hpBar.maxValue = maxHP;
        hpBar.value = currentHP;
    }

    void Update()
    {
        if (enemyHPBar.value <= 0)
        {
            if (!isWin)
            {
                win();
            }
        }
    }

    void FixedUpdate()
    {
        if (canMove && !isDead && !isWin)
        {
            move();
            controller.Move(Vector3.up * -1 * gravity);
        }
    }

    void LateUpdate()
    {
        if (!isDead && !isWin && canMove)
        {
            if (rotateJoystick.Horizontal != 0)
            {
                float horizontal = rotateJoystick.Horizontal * rotateSpeed;
                transform.Rotate(0, horizontal, 0);
            }

            float desiredAngle = transform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);
            playerCamera.transform.position = transform.position - (rotation * offset);

            playerCamera.transform.LookAt(transform);
            playerCamera.transform.Rotate(new Vector3(-22.442f, 0.0f, 0.0f));
        }
        flashPosition = transform.position;
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class BRCharacterManager : MonoBehaviourPunCallbacks
{
    #region Public Fields
    public static GameObject localPlayer;
    public Joystick movementJoystick;

    public float moveSpeed = 5;

    public GameObject spawn;
    public GameObject[] skills;
    public GameObject UI;
    public float nextAttack;
    public float attackCD = 2f;

    public bool canMove;
    public bool canAttack;
    public bool inDanger;
    public BRGameManager gameManager;
    public float maxHP = 500f;
    public float currentHP = 500f;
    private bool isDead;
    private bool isWin;
    public GameObject deathEffect;

    #endregion
    #region Private Fields

    CharacterController controller;
    CharacterAnimationController anim;
    Vector3 offset;
    Camera playerCamera;
    #endregion
    #region MonoBehaviour

    void Start()
    {
        if (photonView.IsMine)
        {
            localPlayer = gameObject;
        }
        controller = gameObject.GetComponent<CharacterController>();
        anim = gameObject.GetComponent<CharacterAnimationController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<BRGameManager>();
        canMove = true;
        canAttack = true;
        inDanger = false;
        isDead = false;
        isWin = false;
        playerCamera = Camera.main;
        offset = playerCamera.transform.position - transform.position;
        if (UI != null)
        {
            GameObject _UI = Instantiate(UI);
            _UI.SendMessage("setTarget", this);
        }
    }
    void Update()
    {
        if (inDanger)
        {
            currentHP -= 50 * Time.deltaTime;
        }
        if (Input.GetMouseButtonDown(1))
        {
            attack(1);
        }
        if (currentHP <= 0)
        {
            die();
        }
    }
    void FixedUpdate()
    {
        if (canMove) move();
    }
    void LateUpdate()
    {
        playerCamera.transform.position = transform.position + offset;
    }

    #endregion
    #region Functions

    void move()
    {
        float h = movementJoystick.Horizontal;
        float v = movementJoystick.Vertical;
        if (h * v != 0)
        {
            anim.runAnim();
        }
        else anim.idleAnim();
        Vector3 movement = new Vector3(h, -10.0f, v);
        //movement = transform.TransformDirection(movement);        
        transform.LookAt(new Vector3(h, 0.0f, v) + transform.position);
        controller.Move(movement * moveSpeed * Time.deltaTime);
    }
    public void attack(int skillSlot)
    {
        if (canAttack)
        {
            anim.attackAnim();
            nextAttack = Time.time + attackCD;
            canMove = false;
            StartCoroutine(cooldownATK(attackCD));
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
        //PhotonNetwork.Instantiate(skills[skillSlot].name, spawn.transform.position, spawn.transform.rotation);
        Instantiate(skills[skillSlot], spawn.transform.position, spawn.transform.rotation);
    }
    public void die()
    {
        this.enabled = false;
        canAttack = false;
        isDead = true;
        gameObject.GetComponent<OnlineCharacterGesture>().enabled = false;
        gameObject.GetComponent<OnlineCharacterGesture>().clear();
        anim.dieAnim();
        deathEffect.SetActive(true);
        Time.timeScale = 0.5f;
        Invoke("resetTimeScale", 2);
        if (photonView.IsMine)
        {
            gameManager.lose();
        }
        else
        {
            gameManager.win();
        }
    }
    public void win()
    {
        this.enabled = false;
        isWin = true;
        gameObject.GetComponent<OnlineCharacterGesture>().enabled = false;
        gameObject.GetComponent<OnlineCharacterGesture>().clear();
        anim.winAnim();
        Time.timeScale = 0.5f;
        Invoke("resetTimeScale", 2);
    }
    #endregion
}

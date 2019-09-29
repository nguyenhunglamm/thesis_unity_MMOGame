using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class OnlineCharacterManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHP);
        }
        else
        {
            this.currentHP = (int)stream.ReceiveNext();
        }
    }

    #endregion
    #region Public Fields

    public static GameObject localPlayer;
    public CharacterController controller;
    public CharacterAnimationController anim;

    public OnlineGameManager gameManager;

    public Slider ownHPBar;
    public Slider enemyHPBar;

    public Text ownName;
    public Text enemyName;

    public Joystick movementJoystick;
    public Joystick rotateJoystick;

    public GameObject spawn; //Skill spawn location
    public GameObject deathEffect;
    public GameObject[] skills;

    public float moveSpeed = 5;
    public float rotateSpeed = 5;
    public float attackCD;

    public Camera playerCamera;
    public GameObject lineCamera;

    public bool canMove = true;
    public bool canAttack = true;

    public int maxHP = 200;
    public int currentHP = 200;

    #endregion
    #region Private Fields

    Vector3 offset = new Vector3(0.0f, -2.5f, 4.6f);

    float nextAttack;

    bool isWin = false;
    bool isDead = false;

    #endregion
    #region MonoBehaviour

    void Start()
    {
        canMove = true;
        canAttack = true;
        attackCD = 2f;
        DontDestroyOnLoad(gameObject);
        anim = GetComponent<CharacterAnimationController>();
        controller = GetComponent<CharacterController>();
        ownName = GameObject.Find("Own Name").GetComponent<Text>();
        enemyName = GameObject.Find("Enemy Name").GetComponent<Text>();
        gameManager = GameObject.Find("Game Manager").GetComponent<OnlineGameManager>();
        if (photonView.IsMine)
        {
            localPlayer = gameObject;
            ownHPBar = GameObject.Find("LeftBar").GetComponent<Slider>();
            enemyHPBar = GameObject.Find("RightBar").GetComponent<Slider>();
            movementJoystick = GameObject.Find("Movement Joystick").GetComponent<FixedJoystick>();
            rotateJoystick = GameObject.Find("Rotate Joystick").GetComponent<FixedJoystick>();
            playerCamera = Camera.main;
            Debug.Log(offset);
            ownName.text = PhotonNetwork.NickName;
        }
        else
        {
            lineCamera.SetActive(false);
            GetComponent<OnlineCharacterGesture>().enabled = false;
            ownHPBar = GameObject.Find("RightBar").GetComponent<Slider>();
            enemyHPBar = GameObject.Find("LeftBar").GetComponent<Slider>();
            enemyName.text = gameObject.GetComponent<PhotonView>().Owner.NickName;
        }
        ownHPBar.maxValue = maxHP;
        ownHPBar.value = maxHP;
    }

    void Update()
    {
        if (enemyHPBar.value <= 0)
        {
            win();
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            move();
        }
    }

    private void LateUpdate()
    {
        if (photonView.IsMine)
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
    }
    #endregion
    #region Character

    void move()
    {
        float h = movementJoystick.Horizontal;
        float v = movementJoystick.Vertical;
        if (v > 0)
        {
            if (Mathf.Abs(v) > Mathf.Abs(h))
            {
                anim.runAnim();
            }
            else
            {
                v /= 3;
                if (h > 0)
                {
                    anim.walkLeftAnim();
                }
                else
                {
                    anim.walkRightAnim();
                }
            }
        }
        else
        {
            v /= 3;
            if (Mathf.Abs(v) > Mathf.Abs(h))
            {
                anim.walkBackwardAnim();
            }
            else
            {
                if (h > 0)
                {
                    anim.walkLeftAnim();
                }
                else
                {
                    anim.walkRightAnim();
                }
            }
        }
        if (0 == h && 0 == v)
        {
            anim.idleAnim();
        }
        Vector3 movement = new Vector3(h / 3, 0.0f, v);
        movement = transform.TransformDirection(movement);
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
        PhotonNetwork.Instantiate(skills[skillSlot].name, spawn.transform.position, spawn.transform.rotation);
    }

    public void getHit(int damage)
    {
        if (!isWin && !isDead)
        {
            StopAllCoroutines(); //Disrupt attacks
            anim.getHitAnim();
            currentHP -= damage;
            ownHPBar.value = currentHP;
            if (currentHP <= 0)
            {
                die();
            }
            canMove = true;
        }
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
        endGameCamera();
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
        endGameCamera();
        Time.timeScale = 0.5f;
        Invoke("resetTimeScale", 2);
    }

    void resetTimeScale()
    {
        Time.timeScale = 1f;
    }

    void endGameCamera()
    {
        if (photonView.IsMine)
        {
            playerCamera.GetComponent<CameraMoveUponDeath>().enabled = true;
        }
    }

    #endregion
}

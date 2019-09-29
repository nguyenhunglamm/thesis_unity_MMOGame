using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerUI : MonoBehaviour
{
    public BRCharacterManager target;

    public Text playerName;
    public Slider HP;
    private Vector3 screenOffset = new Vector3(0f, 30f, 0f);
    float characterControllerHeight = 0f;
    Transform targetTransform;
    Renderer targetRenderer;
    Vector3 targetPosition;
    public void setTarget(BRCharacterManager target)
    {
        this.target = target;
        targetTransform = this.target.GetComponent<Transform>();
        targetRenderer = this.target.GetComponent<Renderer>();
        characterControllerHeight = target.GetComponent<CharacterController>().height;
        HP.maxValue = target.maxHP;
    }
    void Awake()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (playerName != null)
        {
            playerName.text = target.photonView.Owner.NickName;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HP.value = target.currentHP;
        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }
    }
    void LateUpdate()
    {
        // Do not show the UI if we are not visible to the camera, thus avoid potential bugs with seeing the UI, but not the player itself.
        if (targetRenderer != null)
        {
            this.gameObject.SetActive(targetRenderer.isVisible);
        }


        // #Critical
        // Follow the Target GameObject on screen.
        if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y += characterControllerHeight;
            this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
        }
    }
}

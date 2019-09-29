using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    void OnTriggerExit(Collider other) {
        other.gameObject.GetComponent<BRCharacterManager>().inDanger = true;
    }
    void OnTriggerEnter(Collider other) {
        other.gameObject.GetComponent<BRCharacterManager>().inDanger = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("scale", 1, 0.01f);
    }

    void scale() {
        float x = transform.localScale.x * 0.9999f;
        float z = transform.localScale.z * 0.9999f;
        transform.localScale = new Vector3(x, transform.localScale.y, z);
        GetComponent<SphereCollider>().radius *= 0.99999f;
    }

    // Update is called once per frame
    void Update()
    {

    }
}

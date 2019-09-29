using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter5s : MonoBehaviour
{
    public int damage;
    void OnParticleCollision(GameObject other)
    {
        if (other.GetComponent<CharacterManager>() != null) other.GetComponent<CharacterManager>().getHit(damage);
        if (other.GetComponent<AI>() != null) other.GetComponent<AI>().getHit(damage);
        if (other.GetComponent<OnlineCharacterManager>() != null) other.GetComponent<OnlineCharacterManager>().getHit(damage);
    }
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

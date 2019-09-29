using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SkillSetting : MonoBehaviour {

	public float speed;
    public float lifeSpellTime;
	public float lifeHitTime;
	public float fireRate;
    public int damage;
	private float nextFire;
	public GameObject muzzlePrefab;
	public GameObject hitPrefab;
    public AudioClip shotFX;
	
	private Vector3 offset;

	void Start () {


		if (muzzlePrefab != null) {
			
			
			
			var muzzleVFX = Instantiate (muzzlePrefab, transform.position, Quaternion.identity);
			muzzleVFX.transform.forward = gameObject.transform.forward;
			var psMuzzle = muzzleVFX.GetComponent<ParticleSystem> ();
			if (psMuzzle != null)
				Destroy (muzzleVFX, psMuzzle.main.duration);
			else {
				var psChild = muzzleVFX.transform.GetChild (0).GetComponent<ParticleSystem> ();
				Destroy (muzzleVFX, psChild.main.duration);
				Destroy (gameObject, lifeSpellTime);
			
		}
		    if (shotFX != null && GetComponent<AudioSource>()) {
			GetComponent<AudioSource> ().PlayOneShot (shotFX);
		}
		
	}
	}

	
	void Update () {
		
		
		if (speed != 0) {
		
			transform.position += transform.forward * (speed * Time.deltaTime);
			
		}
	}

	void OnCollisionEnter (Collision co) {
        if (co.gameObject.layer == 11)
        {
            speed = 0;

            ContactPoint contact = co.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;
            Debug.Log(co.gameObject);
            if (hitPrefab != null)
            {
                var hitVFX = Instantiate(hitPrefab, pos, rot);
                var ps = hitVFX.GetComponent<ParticleSystem>();
                if (ps == null)
                {
                    var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                    Destroy(hitVFX, psChild.main.duration * lifeHitTime);
                }
                else
                    Destroy(hitVFX, ps.main.duration);
            }
            if (co.gameObject.GetComponent<CharacterManager>() != null) co.gameObject.GetComponent<CharacterManager>().getHit(damage);
            if (co.gameObject.GetComponent<AI>() != null) co.gameObject.GetComponent<AI>().getHit(damage);
            if (co.gameObject.GetComponent<OnlineCharacterManager>() != null) co.gameObject.GetComponent<OnlineCharacterManager>().getHit(damage);
            StartCoroutine(DestroyParticle(0f));
        }

	}

	
		public IEnumerator DestroyParticle (float waitTime) {

		if (transform.childCount > 0 && waitTime != 0) {
			List<Transform> tList = new List<Transform> ();

			foreach (Transform t in transform.GetChild(0).transform) {
				tList.Add (t);
			}		

			while (transform.GetChild(0).localScale.x > 0) {
				yield return new WaitForSeconds (0.01f);
				transform.GetChild(0).localScale -= new Vector3 (0.1f, 0.1f, 0.1f);
				for (int i = 0; i < tList.Count; i++) {
					tList[i].localScale -= new Vector3 (0.1f, 0.1f, 0.1f);
				}
			}
		}
		
		yield return new WaitForSeconds (waitTime);
		Destroy (gameObject);
	}
}
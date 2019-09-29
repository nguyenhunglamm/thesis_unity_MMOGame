using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillNext : MonoBehaviour {

	public Text effectName;
	public GameObject firePoint;
	public List<GameObject> VFXs = new List<GameObject> ();

	public RotateToMouse rotateToMouse;

	private int count = 0;
	private GameObject effectToSpawn;
	private float timeToFire = 0;

	void Start () {
		effectToSpawn = VFXs[0];
		if (effectName != null) effectName.text = effectToSpawn.name;
	}

	void Update () {
		if(Input.GetMouseButton (0) && Time.time >= timeToFire){
			timeToFire = Time.time + 1f / effectToSpawn.GetComponent<SkillSetting>().fireRate;
			SpawnVFX ();	
		}

		if (Input.GetKeyDown (KeyCode.X))
			Next ();
		if (Input.GetKeyDown (KeyCode.Z)) 
			Previous ();	
	}
	void SpawnVFX (){
		GameObject vfx;


		if (firePoint != null) {
			vfx = Instantiate (effectToSpawn, firePoint.transform.position, Quaternion.identity);
			if(rotateToMouse != null){
				vfx.transform.localRotation = rotateToMouse.GetRotation ();
			} 
			else Debug.Log ("No RotateToMouseScript found on firePoint.");
		}
		else
			vfx = Instantiate (effectToSpawn);

	}
	
		public void Next () {
		count++;

		if (count > VFXs.Count)
			count = 0;

		for(int i = 0; i < VFXs.Count; i++){
			if (count == i)	effectToSpawn = VFXs [i];
			if (effectName != null)	effectName.text = effectToSpawn.name;
		}
	}

	public void Previous () {
		count--;

		if (count < 0)
			count = VFXs.Count;

		for (int i = 0; i < VFXs.Count; i++) {
			if (count == i) effectToSpawn = VFXs [i];
			if (effectName != null)	effectName.text = effectToSpawn.name;
		}
	}
	
}

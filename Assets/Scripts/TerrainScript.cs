using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainScript : MonoBehaviour
{
    public TerrainLayer terrain;
    private bool increase = true;
    public float speed = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (0.3f < terrain.metallic) increase = false;
        if (0.01f > terrain.metallic) increase = true;
        if (increase) terrain.metallic += speed;
        else terrain.metallic -= speed;
    }

}

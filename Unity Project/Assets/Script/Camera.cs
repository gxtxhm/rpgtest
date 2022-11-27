using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject player;
    public float x=0.23f, y=7.19f, z=-7.54f;
    Vector3 vec;
    void Start()
    {
        vec = new Vector3(x, y, z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + vec;
    }
}

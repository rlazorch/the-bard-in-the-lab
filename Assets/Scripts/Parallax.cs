using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public float rate = 1.2f;
    public float xOffset = 0;
    void Update()
    {
        gameObject.transform.position = new Vector3(xOffset + Camera.main.transform.position.x / rate, gameObject.transform.position.y, gameObject.transform.position.z);
    }
}

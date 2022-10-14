using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndScript : MonoBehaviour
{
    GameObject model;
    public float delta;

    public void Start()
    {
        model = transform.GetChild(0).gameObject;
    }

    public void Update()
    {
        model.transform.Rotate(Vector3.up, Time.deltaTime * delta);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            LevelControl.instance.EndLevel();
        }
    }
}

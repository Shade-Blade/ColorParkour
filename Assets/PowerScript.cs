using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerScript : MonoBehaviour
{
    public PlayerController.Power power;
    public bool active;
    public GameObject effects;

    public void Update()
    {
        active = !(PlayerController.instance.power == power);
        if (active)
        {
            effects.SetActive(true);
        } else
        {
            effects.SetActive(false);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            PlayerController.instance.power = power;
            //Destroy(gameObject);
        }
    }
}

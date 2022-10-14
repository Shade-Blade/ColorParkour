using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusScript : MonoBehaviour
{
    bool collected;

    GameObject model;

    public void Start()
    {
        model = transform.GetChild(0).gameObject;
    }

    public void Update()
    {
        if (!collected)
        {
            model.SetActive(true);
        }
        else
        {
            model.SetActive(false);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            collected = true;
            Debug.Log("yes");
        }
    }

    public bool GetCollected()
    {
        return collected;
    }
}

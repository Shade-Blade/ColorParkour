using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelControl : MonoBehaviour
{
    public static LevelControl instance;

    //the level end object
    public GameObject levelEnd;

    public BonusScript[] bonuses;
    public bool[] bonusCollected;

    void Start()
    {
        instance = this;
        bonuses = FindObjectsOfType<BonusScript>();
    }

    public void EndLevel()
    {
        Debug.Log("end");
    }

    void UpdateBonuses()
    {
        for (int i = 0; i < bonuses.Length; i++)
        {
            bonusCollected[i] = bonuses[i].GetCollected();
        }
    }
}

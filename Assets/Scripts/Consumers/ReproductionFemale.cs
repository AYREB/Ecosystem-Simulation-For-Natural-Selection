using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReproductionFemale : MonoBehaviour
{
    public Consumer consumerScript;
    public bool isPregnant;
    public bool pregnancyTimerCoolingDown;
    public bool isFertile;
    public float energyLevelRequiredForPregnancy;
    public float pregnancyTimerMax;
    public float currentPregnancyTimer;

    // Start is called before the first frame update
    void Start()
    {
        isFertile = consumerScript.isFertile;
        energyLevelRequiredForPregnancy = consumerScript.energyLevelRequiredForPregnancy;
        pregnancyTimerMax = consumerScript.pregnancyCooldownTimerMax;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPregnancyTimer <= 0)
        {
            pregnancyTimerCoolingDown = false;
        }
        else
        {
            pregnancyTimerCoolingDown = true;
        }
    }

    //public bool ReproduceCheck()
    //{
    //    bool returnBackTrueForReproduce = false;
    //    if (isPregnant == false)
    //    {
    //        if (pregnancyTimerCoolingDown == false)
    //        {
    //            if (isFertile == true)
    //            {
    //                if (energyLevelRequiredForPregnancy <= consumerScript.energyLevel)
    //                {
    //                    returnBackTrueForReproduce = true;
    //                }
    //            }
    //        }
    //    }
    //    return returnBackTrueForReproduce;
    //}

    public void BeginPregnancy(float[] motherGenes, float[] fatherGenes)
    {
        if (isPregnant == false)
        {
            Debug.Log("WASSSUUUPPP");
            isPregnant = true;
        }
    }
}

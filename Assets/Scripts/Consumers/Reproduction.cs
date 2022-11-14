using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reproduction : MonoBehaviour
{
    public Consumer consumerScript;
    public bool isPregnant;
    public bool pregnancyTimerCoolingDown;
    public bool stillFertile;
    public float energyLevelRequiredForPregnancy;

    // Start is called before the first frame update
    void Start()
    {
               
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool ReproduceCheck()
    {
        bool returnBackTrueForReproduce = false;
        if (isPregnant == false)
        {
            if(pregnancyTimerCoolingDown == false)
            {
                if (stillFertile == true)
                {
                    if (energyLevelRequiredForPregnancy <= consumerScript.energyLevel)
                    {
                        returnBackTrueForReproduce = true;
                    }
                }
            }
        }
        return returnBackTrueForReproduce;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyConsumption : MonoBehaviour
{
    public Consumer consumerScript;
    public float energyCapacity;
    public float currentEnergy;
    public float energyDepletionRatePerSecond;
    public float[] genesOfAnimal;
    public bool countingDown;
    // Start is called before the first frame update
    void Start()
    {
        consumerScript = GetComponent<Consumer>();
        energyCapacity = consumerScript.energyCapacity;
        countingDown = false;
        genesOfAnimal = consumerScript.myGenesListToPassToChildren;
    }

    void CalculateEnergyDepletionPerSecond()
    {
        //0 = speed
        //1 = vision radius
        //2 = gestation duration
        //3 = max offspring
        //4 = anti reproductive urge
        //5 = fight or flight strength
    }

    // Update is called once per frame
    void Update()
    {
        DepleteEnergy();
    }

    public void DepleteEnergy()
    {
        if (countingDown == false)
        {
            StartCoroutine(DepleteTimer());
            countingDown = true;
        }
    }

    public IEnumerator DepleteTimer()
    {
        yield return new WaitForSeconds(energyDepletionRatePerSecond);
        countingDown = false;
    }
}

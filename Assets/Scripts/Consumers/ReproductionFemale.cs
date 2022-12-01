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
    public float pregnancyCooldownTimerMax;
    public float currentPregnancyCooldownTimer;
    public int maxNumberOfOffspring;

    [Header("Number Of Children")]
    public List<int> oneOfEachNumberOfChildren = new List<int>();
    public List<int> numberOfEachNumberOfChildren = new List<int>();

    public float[] childGenes;

    // Start is called before the first frame update
    void Start()
    {
        consumerScript = GetComponent<Consumer>();
        isFertile = consumerScript.isFertile;
        energyLevelRequiredForPregnancy = consumerScript.energyLevelRequiredForPregnancy;
        pregnancyCooldownTimerMax = consumerScript.pregnancyCooldownTimerMax;
        maxNumberOfOffspring = (int)consumerScript.maxOffspring;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPregnancyCooldownTimer <= 0)
        {
            pregnancyTimerCoolingDown = false;
        }
        else
        {
            pregnancyTimerCoolingDown = true;
        }
    }

    public void BeginPregnancy(float[] motherGenes, float[] fatherGenes, GameObject father)
    {
        StartCoroutine(DisableMovementToMate(father));
        Debug.Log("Pregnancy Began");
        isPregnant = true;
        PrepareChildrenForBirth();
    }

    public void PrepareChildrenForBirth()
    {
        int howManyChildrenWillIGiveBirthTo = NumberOfChildrenToMake();

    }

    public int NumberOfChildrenToMake()
    {
        oneOfEachNumberOfChildren = new List<int>();
        numberOfEachNumberOfChildren = new List<int>();

        for (int i = 0; i < maxNumberOfOffspring; i++)
        {
            oneOfEachNumberOfChildren.Add(i + 1);
        }

        for (int i = 0; i < oneOfEachNumberOfChildren.Count; i++)
        {
            for (int x = 0; x < oneOfEachNumberOfChildren[i]; x++)
            {
                numberOfEachNumberOfChildren.Add(oneOfEachNumberOfChildren[i]);
            }
        }

        return (numberOfEachNumberOfChildren[Random.Range(0, numberOfEachNumberOfChildren.Count)]);
    }

    public IEnumerator DisableMovementToMate(GameObject father)
    {
        yield return new WaitForSeconds(3);
        consumerScript.mateMovingTo = null;
        consumerScript.movingToMate = false;
        consumerScript.interested = false;
        consumerScript.objectInterestedIn = null;

        father.GetComponent<Consumer>().mateMovingTo = null;
        father.GetComponent<Consumer>().movingToMate = false;
        father.GetComponent<Consumer>().interested = false;
        father.GetComponent<Consumer>().objectInterestedIn = null;
    }
}

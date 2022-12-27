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
    public int maxNumberOfOffspring;
    public float mutationAmount;
    public float mutationChance;
    public GameObject objectToGiveBirthTo;
    public float gestationDuration;

    [Header("Number Of Children")]
    public List<int> oneOfEachNumberOfChildren = new List<int>();
    public List<int> numberOfEachNumberOfChildren = new List<int>();

    public float[] childGenes;

    // Start is called before the first frame update
    void Start()
    {
        consumerScript = GetComponent<Consumer>();
        gestationDuration = consumerScript.gestationDuration;
        mutationAmount = consumerScript.mutationAmount;
        mutationChance = consumerScript.mutationChance;
        objectToGiveBirthTo = consumerScript.objectToGiveBirthTo;
        isFertile = consumerScript.isFertile;
        energyLevelRequiredForPregnancy = consumerScript.energyLevelRequiredForPregnancy;
        pregnancyCooldownTimerMax = consumerScript.pregnancyCooldownTimerMax;
        maxNumberOfOffspring = (int)consumerScript.maxOffspring;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BeginPregnancy(float[] motherGenes, float[] fatherGenes, GameObject father)
    {
        StartCoroutine(DisableMovementToMate(father));
        Debug.Log("Pregnancy Began");
        isPregnant = true;
        StartCoroutine(TimeBeforeGivingBirth(motherGenes, fatherGenes, father));
    }

    public IEnumerator TimeBeforeGivingBirth(float[] motherGenes, float[] fatherGenes, GameObject father)
    {
        yield return new WaitForSeconds(gestationDuration);
        Birth(motherGenes, fatherGenes, father);
    }

    public void Birth(float[] motherGenes, float[] fatherGenes, GameObject father)
    {
        int howManyChildrenWillIGiveBirthTo = NumberOfChildrenToMake();

        for (int i = 0; i < howManyChildrenWillIGiveBirthTo; i++)
        {
            float[] genesToApply = GeneListMaker(motherGenes, fatherGenes);
            GameObject babyJustGivenBirthTo = Instantiate(objectToGiveBirthTo, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), Quaternion.identity);
            GeneAssigner(babyJustGivenBirthTo, genesToApply);
            babyJustGivenBirthTo.GetComponent<Consumer>().ApplierAtBirth();
        }
        isPregnant = false;
        CooldownAfterGivingBirth();
    }

    public IEnumerator CooldownAfterGivingBirth()
    {
        yield return new WaitForSeconds(pregnancyCooldownTimerMax);
        pregnancyTimerCoolingDown = false;
    }

    public void GeneAssigner(GameObject objectToAssignValuesTo, float[] genesToApply)
    {
        Consumer babyConsumerScript = objectToAssignValuesTo.GetComponent<Consumer>();
        babyConsumerScript.speed = genesToApply[0];
        babyConsumerScript.stamina = genesToApply[1];
        babyConsumerScript.weight = genesToApply[2];
        babyConsumerScript.lifespanYears = genesToApply[3];
        babyConsumerScript.height = genesToApply[4];
        babyConsumerScript.strength = genesToApply[5];
        babyConsumerScript.width_length = genesToApply[6];
        babyConsumerScript.visionRadius = genesToApply[7];
        babyConsumerScript.gestationDuration = genesToApply[8];
        babyConsumerScript.greed = genesToApply[9];
        babyConsumerScript.foodCapacity = genesToApply[10];
        babyConsumerScript.maxOffspring = genesToApply[11];
        babyConsumerScript.antiReproductiveUrge = genesToApply[12];
        babyConsumerScript.fightOrFlightStrength10Flight0Fight = genesToApply[13];
        babyConsumerScript.isMale = Random.Range(1, 3);
    }

    public float[] GeneListMaker(float[] motherGenes, float[] fatherGenes)
    {
        Debug.Log("1");
        float[] returnGenesList = {0,0,0,0,0,0,0,0,0,0,0,0,0,0};
        Debug.Log("2");
        for (int i = 0; i < motherGenes.Length; i++)
        {
            Debug.Log("3");
            bool willMutate = mutationChance <= Random.Range(1, 101);
            Debug.Log("4");

            if (willMutate == false)
            {
                Debug.Log("5");
                bool motherGenesPassed = Random.Range(0f, 100.0f) <= 50f; ;
                Debug.Log("6");
                if (motherGenesPassed == true)
                {
                    Debug.Log("6.5");
                    returnGenesList[i] = motherGenes[i];
                    Debug.Log("7");
                }
                else
                {
                    returnGenesList[i] = fatherGenes[i];
                    Debug.Log("8");
                }
            }
            else if (willMutate == true)
            {
                bool motherGenesPassed = Random.Range(0f, 100.0f) <= 50f; ;
                Debug.Log("9");

                if (motherGenesPassed == true)
                {
                    returnGenesList[i] = motherGenes[i] + Random.Range(-mutationAmount, mutationAmount);
                    Debug.Log("10");
                }
                else
                {
                    returnGenesList[i] = fatherGenes[i] + Random.Range(-mutationAmount, mutationAmount);
                    Debug.Log("11");
                }
            }
        }

        return returnGenesList;
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

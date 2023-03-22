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
        Debug.Log("Begin Pregnancy");
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
        Debug.Log("Birth");
        int howManyChildrenWillIGiveBirthTo = NumberOfChildrenToMake();

        for (int i = 0; i < howManyChildrenWillIGiveBirthTo; i++)
        {
            float[] genesToApply = GeneListMaker(motherGenes, fatherGenes);
            GameObject babyJustGivenBirthTo = Instantiate(objectToGiveBirthTo, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), Quaternion.identity);
            GeneAssigner(babyJustGivenBirthTo, genesToApply);
            babyJustGivenBirthTo.GetComponent<Consumer>().ApplierAtBirth();
            babyJustGivenBirthTo.GetComponent<Growth>().Age = 0;
            babyJustGivenBirthTo.GetComponent<Consumer>().isFertile = false;
            babyJustGivenBirthTo.GetComponent<ReproductionFemale>().isFertile = false;
            babyJustGivenBirthTo.GetComponent<ReproductionFemale>().isPregnant = false;
        }
        isPregnant = false;
        isFertile = false;
        isPregnant = false;
        consumerScript.isFertile = false;
        motherGenes = null;
        fatherGenes = null;
        pregnancyTimerCoolingDown = true;
        StartCoroutine(CooldownAfterGivingBirth());
    }

    public IEnumerator CooldownAfterGivingBirth()
    {
        yield return new WaitForSeconds(pregnancyCooldownTimerMax);
        pregnancyTimerCoolingDown = false;
        isFertile = true;
        consumerScript.isFertile = true;
    }

    public void GeneAssigner(GameObject objectToAssignValuesTo, float[] genesToApply)
    {
        Debug.Log("Gene Assigner");
        Consumer babyConsumerScript = objectToAssignValuesTo.GetComponent<Consumer>();
        babyConsumerScript.speed = genesToApply[0];
        babyConsumerScript.visionRadius = genesToApply[1];
        babyConsumerScript.gestationDuration = genesToApply[2];
        babyConsumerScript.maxOffspring = genesToApply[3];
        babyConsumerScript.antiReproductiveUrge = genesToApply[4];
        babyConsumerScript.fightOrFlightStrength10Flight0Fight = genesToApply[5];
        babyConsumerScript.isMale = Random.Range(1, 3);
    }

    public float[] GeneListMaker(float[] motherGenes, float[] fatherGenes)
    {
        Debug.Log("Gene list maker");
        float[] returnGenesList = {0,0,0,0,0,0};
        for (int i = 0; i < motherGenes.Length; i++)
        {
            bool willMutate = mutationChance <= Random.Range(1, 101);

            if (willMutate == false)
            {
                bool motherGenesPassed = Random.Range(0f, 100.0f) <= 50f; ;
                if (motherGenesPassed == true)
                {
                    returnGenesList[i] = motherGenes[i];
                }
                else
                {
                    returnGenesList[i] = fatherGenes[i];
                }
            }
            else if (willMutate == true)
            {
                bool motherGenesPassed = Random.Range(0f, 100.0f) <= 50f; ;

                if (motherGenesPassed == true)
                {
                    returnGenesList[i] = motherGenes[i] + Random.Range(-mutationAmount, mutationAmount);
                }
                else
                {
                    returnGenesList[i] = fatherGenes[i] + Random.Range(-mutationAmount, mutationAmount);
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
        Debug.Log("Disable Movement To mate");
        yield return new WaitForSeconds(3);
        consumerScript.mateMovingTo = null;
        consumerScript.movingToMate = false;
        consumerScript.interested = false;
        consumerScript.objectInterestedIn = null;

        father.GetComponent<Consumer>().interested = false;
        father.GetComponent<Consumer>().movingToMate = false;
        father.GetComponent<Consumer>().mateMovingTo = null;       
        father.GetComponent<Consumer>().objectInterestedIn = null;
    }
}

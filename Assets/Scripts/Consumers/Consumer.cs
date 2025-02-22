using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Consumer : MonoBehaviour
{
    [Header("Enums")]
    public SpeciesEnum species;
    public List<SpeciesEnum> _diet;
    public AllSpeciesReuirement allSpeciesRequirement;

    public EcosystemMainManager ecosystemMainManager;
    public NavMeshAgent navMeshAgent;

    [Header("Reproduction")]
    public ReproductionFemale reproductionScript;
    public bool movingToMate;
    public float mutationAmount;
    public float mutationChance;

    [Header("Growth")]
    public Growth growthScript;
    public float scaleOfAnimalAtBirth;

    [Header("Fill out")]
    public float stoppingDistance;
    public Color colorOfMale;
    public Color colorOfFemale;
    public float pregnancyCooldownTimerMax;
    public GameObject objectToGiveBirthTo;
    public Color colourOfObject;
    public float lifespanYears;
    public float energyCapacity;


    [Header("Genes (Mutate)")]
    //1 = male, 2 = female
    public float isMale;
    public float speed;
    //public float stamina;
    //public float weight;   
    //public float height;
    //public float strength;
    //public float width_length;
    public float visionRadius;
    public float gestationDuration;
    //public float greed;
    public float maxOffspring;
    public float antiReproductiveUrgeDecreaseSpeed;
    public float fightOrFlightStrength10Flight0Fight;

    

    [Header("Requirements")]
    //0 = no anti (wants to reproduce)
    public float maxAntiReproductiveUrge;
    public float antiReproductiveUrge;
    //0 = low energy (needs food)
    public float energyLevel;


    [Header("Control Behaviour")]
    public bool interested;
    public bool anyPredatorInRange;
    public bool anyPrey_producerInRange;
    public List<GameObject> allObjectsInRange;
    public List<GameObject> allPredatorsInRange;
    public List<GameObject> allPreyInRange;
    public List<GameObject> allSameGenderInRange;
    public List<GameObject> allDifferentGenderInRange;
    public GameObject closestPredator;
    public GameObject closestPrey;
    public GameObject closestPotentialMate;
    public GameObject mateMovingTo;
    public float distanceToNearestPred;
    public float distanceToNearestPrey_Consumable;
    public SphereCollider visionRadiusCollider;
    public bool arrivedAtDesiredLocation = true;
    public Vector3 positionMovingTo;
    public GameObject objectInterestedIn;
    public bool runningAway;
    public bool isFertile;
    public float energyLevelRequiredForPregnancy;
    public float[] myGenesListToPassToChildren;
    public bool loopholeRunning;
    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (isMale == 2)
        {
            this.gameObject.AddComponent<ReproductionFemale>();
            reproductionScript = GetComponent<ReproductionFemale>();
            colourOfObject = colorOfFemale;
        }
        else
        {
            colourOfObject = colorOfMale;
        }
        energyLevel = energyCapacity;
        antiReproductiveUrge = maxAntiReproductiveUrge;
        allSpeciesRequirement = GetComponent<AllSpeciesReuirement>();
        species = allSpeciesRequirement.species;
        _diet = allSpeciesRequirement.diet;
        ecosystemMainManager = FindObjectOfType<EcosystemMainManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        visionRadiusCollider = GetComponentInChildren<SphereCollider>();
        visionRadiusCollider.radius = visionRadius;
        navMeshAgent.speed = speed;
        positionMovingTo = ecosystemMainManager.getRandomPosition();
        GetComponent<Renderer>().material.color = colourOfObject;
        myGenesListToPassToChildren = new float[] {speed, visionRadius, gestationDuration, maxOffspring, antiReproductiveUrgeDecreaseSpeed, fightOrFlightStrength10Flight0Fight };
        loopholeRunning = false;
    }

    public void ApplierAtBirth()
    {
        energyLevel = energyCapacity;
        antiReproductiveUrge = maxAntiReproductiveUrge;
        allSpeciesRequirement = GetComponent<AllSpeciesReuirement>();
        species = allSpeciesRequirement.species;
        _diet = allSpeciesRequirement.diet;
        ecosystemMainManager = FindObjectOfType<EcosystemMainManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        visionRadiusCollider = GetComponentInChildren<SphereCollider>();
        visionRadiusCollider.radius = visionRadius;
        navMeshAgent.speed = speed;
        positionMovingTo = ecosystemMainManager.getRandomPosition();
        GetComponent<Renderer>().material.color = colourOfObject;
        myGenesListToPassToChildren = new float[] { speed, visionRadius, gestationDuration, maxOffspring, antiReproductiveUrgeDecreaseSpeed, fightOrFlightStrength10Flight0Fight };
    }

    // Update is called once per frame
    void Update()
    {
        RunLoophole();
        ClosestPredatorAndPrey();
        Movement();
        NavMeshMoveTo(new Vector3(positionMovingTo.x, positionMovingTo.y + allSpeciesRequirement.SpawnOffset, positionMovingTo.z));
        Interest();
        if (allDifferentGenderInRange.Count > 0)
        {
            MaleReproductionCheck();
        }       
        if (objectInterestedIn == null)
        {
            interested = false;
        }
    }

    public void RunLoophole()
    {
        if(loopholeRunning == false)
        {
            loopholeRunning = true;
            StartCoroutine(Loophole());
        }
    }

    public IEnumerator Loophole()
    {
        runningAway = true;
        yield return new WaitForSeconds(2);              
        loopholeRunning = false;
    }

    public void HandleBoolsControlling()
    {
        if (anyPredatorInRange == true)
        {
            movingToMate = false;
            mateMovingTo = null;
            closestPotentialMate = null;
        }
    }

    public bool ReproductionCheck()
    {
        if (runningAway || mateMovingTo != null || antiReproductiveUrge >= energyLevel || !isFertile)
        {
            return false;
        }

        if (isMale == 1)
        {
            return true;
        }
        else if (isMale == 2)
        {
            var reproductionScript = GetComponent<ReproductionFemale>();
            return !reproductionScript.isPregnant && !reproductionScript.pregnancyTimerCoolingDown;
        }

        return false;
    }


    public GameObject closestPotentialMateInAList(List<GameObject> listOfObjects)
    {
        GameObject closestObjectToReturn = null;
        float closestDistance = 999999999.0f;
        foreach (GameObject item in listOfObjects)
        {
            if (item.GetComponent<Consumer>().ReproductionCheck() == false)
            {
                continue;
            }
            float distance = Vector3.Distance(item.transform.position, transform.position);
            if (distance < closestDistance)
            {
                closestObjectToReturn = item;
            }
        }
        return closestObjectToReturn;
    }

    public void MaleReproductionCheck()
    {
        if (isMale == 1)
        {
            if (ReproductionCheck() == true)
            {
                closestPotentialMate = closestPotentialMateInAList(allDifferentGenderInRange);
                if (closestPotentialMate != null)
                {
                    mateMovingTo = closestPotentialMate;
                    movingToMate = true;
                    positionMovingTo = mateMovingTo.transform.position;
                    interested = true;
                    objectInterestedIn = mateMovingTo;
                    mateMovingTo.GetComponent<Consumer>().FemaleReproductionRequest(this.gameObject, myGenesListToPassToChildren);
                }
                
            }
        }
    }

    public void FemaleReproductionRequest(GameObject maleRequesting, float[] fatherGenesArray)
    {
        mateMovingTo = maleRequesting;
        movingToMate = true;
        positionMovingTo = maleRequesting.transform.position;
        interested = true;
        objectInterestedIn = maleRequesting;
        reproductionScript.BeginPregnancy(myGenesListToPassToChildren, fatherGenesArray, maleRequesting);
    }


    public void Movement()
    {
        if (Vector3.Distance(transform.position, positionMovingTo) <= stoppingDistance)
        {
            arrivedAtDesiredLocation = true;
        }

        if (runningAway == false && movingToMate == false)
        {            
            if (interested == true && arrivedAtDesiredLocation == true)
            {
                GotInRangeOfObjectOrPositionInterestedIn(objectInterestedIn);
                arrivedAtDesiredLocation = false;
                //NavMeshMoveTo(positionMovingTo);
            }
            else if (interested == false)
            {
                if (arrivedAtDesiredLocation == true)
                {
                    arrivedAtDesiredLocation = false;
                    positionMovingTo = ecosystemMainManager.getRandomPosition();
                }
            }
        }

        if(movingToMate == true && runningAway == false)
        {
            positionMovingTo = mateMovingTo.transform.position;
            //NavMeshMoveTo(positionMovingTo);        
        }
    }

    public void NavMeshMoveTo(Vector3 targetPosition)
    {
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(targetPosition);
        }
        else
        {
            Debug.LogWarning("NavMeshAgent is not on the NavMesh. Destination cannot be set.");
        }
    }

private void GotInRangeOfObjectOrPositionInterestedIn(GameObject objectItWasInterestedIn)
    {
        //Eat Prey
        if (movingToMate != true)
        {
            if (objectItWasInterestedIn.GetComponent<AllSpeciesReuirement>() != null)
            {
                foreach (SpeciesEnum myDiet in _diet)
                {
                    if (objectItWasInterestedIn.GetComponent<AllSpeciesReuirement>().species == myDiet)
                    {
                        if (objectInterestedIn.GetComponent<ProducerNextTry>() != null)
                        {
                            objectInterestedIn.GetComponent<ProducerNextTry>().BeenEaten(this.gameObject);
                        }
                        else if (objectInterestedIn.GetComponent<Consumer>() != null)
                        {
                            objectInterestedIn.GetComponent<Consumer>().BeenEaten(this.gameObject);
                        }
                    }
                }
            }
        }
    }

    public bool AnyPredatorsInRange()
    {
        bool anyPredators = false;
        foreach (GameObject item in allPredatorsInRange)
        {
            //Check if predator
            if (item.GetComponent<AllSpeciesReuirement>() != null)
            {
                foreach (SpeciesEnum speciesDiet in item.GetComponent<AllSpeciesReuirement>().diet)
                {
                    if (speciesDiet == species)
                    {
                        anyPredators = true;
                    }
                }
            }
        }
        return anyPredators;
    }

    public bool AnyPreyInRange()
    {
        bool anyPrey = false;

        foreach (GameObject item in allPreyInRange)
        {
            if (item.GetComponent<AllSpeciesReuirement>() != null)
            {
                foreach (SpeciesEnum thisDiet in _diet)
                {
                    if (item.GetComponent<AllSpeciesReuirement>().species == thisDiet)
                    {
                        anyPrey = true;
                    }
                }
            }
        }

        return anyPrey;
    }

    public void getRunawayPosition(Vector3 target)
    {
        Vector3 returnVal = new Vector3(0, 0, 0);
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.position - target, Color.yellow);
        if (Physics.Raycast(transform.position, transform.position - target, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("LandBorders")))
        {
            returnVal = hit.point;
        }
        positionMovingTo = returnVal;
    }

    public void ClosestPredatorAndPrey()
    {
        closestPredator = FindClosestObject(allPredatorsInRange);
        closestPrey = FindClosestObject(allPreyInRange);
        anyPredatorInRange = closestPredator != null;
        anyPrey_producerInRange = closestPrey != null;
    }

    private GameObject FindClosestObject(List<GameObject> objects)
    {
        GameObject closestObject = null;
        float closestDistance = float.MaxValue;

        foreach (GameObject obj in objects)
        {
            float distance = Vector3.Distance(obj.transform.position, transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = obj;
            }
        }

        return closestObject;
    }

    public void Interest()
    {
        if (anyPredatorInRange || !movingToMate)
        {
            if (anyPrey_producerInRange)
            {
                interested = true;
                if (anyPredatorInRange && distanceToNearestPred < distanceToNearestPrey_Consumable * fightOrFlightStrength10Flight0Fight)
                {
                    objectInterestedIn = closestPredator;
                    runningAway = true;
                    getRunawayPosition(closestPredator.transform.position);
                }
                else
                {
                    objectInterestedIn = closestPrey;
                    positionMovingTo = closestPrey.transform.position;
                    runningAway = false;
                }
            }
            else if (anyPredatorInRange)
            {
                runningAway = true;
                objectInterestedIn = closestPredator;
                getRunawayPosition(closestPredator.transform.position);
            }
            else
            {
                ResetInterest();
            }
        }
        else if (movingToMate)
        {
            interested = true;
            objectInterestedIn = mateMovingTo;
            positionMovingTo = mateMovingTo.transform.position;
        }
    }

    private void ResetInterest()
    {
        runningAway = false;
        objectInterestedIn = null;
        interested = false;
    }

    public GameObject closestGameobjectInAList(List<GameObject> listOfObjects)
    {
        float closestDistance = 999999999.0f;
        foreach (GameObject item in listOfObjects)
        {
            float distance = Vector3.Distance(item.transform.position, transform.position);
            if (distance < closestDistance)
            {
                return item;
            }
        }
        return null;
    }

    private void AllPreyInRangeList()
    {
        foreach (GameObject item in allObjectsInRange)
        {
            if (item.GetComponent<AllSpeciesReuirement>() != null)
            {
                foreach (SpeciesEnum myDiet in GetComponent<AllSpeciesReuirement>().diet)
                {
                    if (myDiet == item.GetComponent<AllSpeciesReuirement>().species)
                    {
                        if (item.gameObject.tag == "Producer")
                        {
                            if (item.GetComponent<ProducerNextTry>().foodReadyToBeEaten == true)
                            {
                                if (allPreyInRange.Contains(item) == false)
                                {
                                    allPreyInRange.Add(item);
                                }
                            }
                        }
                        else if (allPreyInRange.Contains(item) == false && item.gameObject.tag != "Producer")
                        {
                            allPreyInRange.Add(item);
                        }
                    }
                }
            }
        }
    }

    private void AllPredatorsInRangeList()
    {
        foreach (GameObject item in allObjectsInRange)
        {
            if (item.GetComponent<AllSpeciesReuirement>() != null)
            {
                foreach (SpeciesEnum itsDiet in item.GetComponent<AllSpeciesReuirement>().diet)
                {
                    if (itsDiet == species)
                    {
                        if (allPredatorsInRange.Contains(item) == false)
                        {
                            allPredatorsInRange.Add(item);
                        }
                    }
                }
            }
        }
    }

    private void SameSpeciesInRangeList()
    {
        foreach (GameObject item in allObjectsInRange)
        {
            if (item.GetComponent<AllSpeciesReuirement>() != null)
            {
                if (item.GetComponent<AllSpeciesReuirement>().species == species)
                {
                    if (item.GetComponent<Consumer>() != null)
                    {
                        if (item.GetComponent<Consumer>().isMale == isMale)
                        {
                            if (allSameGenderInRange.Contains(item) == false)
                            {
                                allSameGenderInRange.Add(item);
                            }                           
                        }
                        else if (item.GetComponent<Consumer>().isMale != isMale)
                        {
                            if(allDifferentGenderInRange.Contains(item) == false)
                            {
                                allDifferentGenderInRange.Add(item);
                            }                            
                        }
                    }
                }
            }                
        }
    }

    public void VisionTriggerColliderSawSomething_1Enter_2Stay_3Exit(int option, Collider other)
    {
        if (option == 1)
        {
            if (allObjectsInRange.Contains(other.gameObject) == false)
            {
                allObjectsInRange.Add(other.gameObject);
                AllPredatorsInRangeList();
                AllPreyInRangeList();
                SameSpeciesInRangeList();
            }
        }
        else if (option == 2)
        {
            if (allObjectsInRange.Contains(other.gameObject) == false)
            {
                if (other.gameObject.tag == "Producer")
                {
                    foreach (SpeciesEnum myDiet in GetComponent<AllSpeciesReuirement>().diet)
                    {
                        if (myDiet == other.GetComponent<AllSpeciesReuirement>().species)
                        {
                            if (other.GetComponent<ProducerNextTry>().foodReadyToBeEaten == true)
                            {
                                if (allObjectsInRange.Contains(other.gameObject) == false)
                                {
                                    allObjectsInRange.Add(other.gameObject);
                                }
                            }
                        }
                    }
                }
                else if (other.gameObject.tag != "Producer")
                {
                    if (allObjectsInRange.Contains(other.gameObject) == false)
                    {
                        allObjectsInRange.Add(other.gameObject);
                    }
                }
            }
        }
        else if (option == 3)
        {
            if (allObjectsInRange.Contains(other.gameObject) == true)
            {
                allObjectsInRange.Remove(other.gameObject);
            }

            if (allPredatorsInRange.Contains(other.gameObject) == true)
            {
                allPredatorsInRange.Remove(other.gameObject);
            }

            if (allSameGenderInRange.Contains(other.gameObject) == true)
            {
                allSameGenderInRange.Remove(other.gameObject);
            }

            if (allDifferentGenderInRange.Contains(other.gameObject) == true)
            {
                allDifferentGenderInRange.Remove(other.gameObject);
            }

            if (allPreyInRange.Contains(other.gameObject) == true)
            {
                allPreyInRange.Remove(other.gameObject);
            }
        }
    }

    private void BeenEaten(GameObject objectEatenBy)
    {
        Consumer[] allConsumers = FindObjectsOfType<Consumer>();
        foreach (Consumer item in allConsumers)
        {
            item.allObjectsInRange.Remove(this.gameObject);
            item.allPredatorsInRange.Remove(this.gameObject);
            item.allPreyInRange.Remove(this.gameObject);

            item.VisionTriggerColliderSawSomething_1Enter_2Stay_3Exit(3, GetComponent<MeshCollider>());
        }
        Destroy(this.gameObject);
    }

    public void DiedWithoutBeingEaten()
    {
        Consumer[] allConsumers = FindObjectsOfType<Consumer>();
        foreach (Consumer item in allConsumers)
        {
            item.allObjectsInRange.Remove(this.gameObject);
            item.allPredatorsInRange.Remove(this.gameObject);
            item.allPreyInRange.Remove(this.gameObject);

            item.VisionTriggerColliderSawSomething_1Enter_2Stay_3Exit(3, GetComponent<MeshCollider>());
        }
        Destroy(this.gameObject);
    }
}

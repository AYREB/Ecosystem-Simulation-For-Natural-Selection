using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Consumer : MonoBehaviour
{
    [Header("Enums")]
    public SpeciesEnum species;
    public List<SpeciesEnum> _diet;
    public AllSpeciesReuirement allSpeciesRequirement;

    public EcosystemMainManager ecosystemMainManager;
    public NavMeshAgent navMeshAgent;

    [Header("Reproduction")]
    public Reproduction reproductionScript;
    public bool movingToMate;

    [Header("Fill out")]
    public float stoppingDistance;
    public Color colourOfObject;

    [Header("Properties (Non-Mutate)")]
    public float speed;
    public float stamina;
    public float weight;

    [Header("Genes (Mutate)")]
    public bool isMale;
    public float height;
    public float strength;
    public float width_length;
    public float visionRadius;
    public float gestationDuration;
    public float greed;
    public float foodCapacity;  
    public float maxOffspring;
    public float reproductiveUrgeIncreaseSpeed;
    public float fightOrFlightStrength10Flight0Fight;

    [Header("Requirements")]
    public float maxEnergyLevel;
    public float maxReproductiveUrge;
    public float energyLevel;
    public float reproductiveUrge;

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


    // Start is called before the first frame update
    void Start()
    {
        if (isMale == false)
        {
            this.gameObject.AddComponent<Reproduction>();
            reproductionScript = GetComponent<Reproduction>();
        }
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
    }

    // Update is called once per frame
    void Update()
    {
        //if (anyPrey_producerInRange == true)
        //{
        //    AllPreyInRangeList();
        //}
        //if (anyPredatorInRange == true)
        //{
        //    AllPredatorsInRangeList();
        //}
        ClosestPredatorAndPrey();
        Reproduction();
        Interest();       
        Movement();        
        NavMeshMoveTo(new Vector3(positionMovingTo.x, positionMovingTo.y + allSpeciesRequirement.SpawnOffset, positionMovingTo.z));
        if (objectInterestedIn == null)
        {
            interested = false;
        }
    }

    public void Reproduction()
    {
        if (reproductiveUrge > energyLevel)
        {
            if(isMale == false)
            {
                if (GetComponent<Reproduction>().ReproduceCheck() == true)
                {
                    if (allDifferentGenderInRange.Count > 1)
                    {
                        closestPotentialMate = closestGameobjectInAList(allDifferentGenderInRange);
                        if (closestPotentialMate.GetComponent<Consumer>().ReproduceCheck(this.gameObject.GetComponent<Consumer>()) == true)
                        {
                            movingToMate = true;
                            mateMovingTo = closestPotentialMate;
                        }
                    }
                }
            }
            else if (isMale == true)
            {
                if (allDifferentGenderInRange.Count > 1)
                {
                    closestPotentialMate = closestGameobjectInAList(allDifferentGenderInRange);
                    if (closestPotentialMate.GetComponent<Consumer>().ReproduceCheck(this.gameObject.GetComponent<Consumer>()) == true)
                    {
                        movingToMate = true;
                        mateMovingTo = closestPotentialMate;
                    }
                }
            }
               
        }
    }

    public bool ReproduceCheck(Consumer animalInterestedInThis)
    {
        bool returnTrueIfCanMate = false;
        if(animalInterestedInThis.isMale != isMale)
        {
            if(isMale == false && movingToMate != true && runningAway != true)
            {
                if(GetComponent<Reproduction>().ReproduceCheck() == true)
                {
                    returnTrueIfCanMate = true;
                }
            }
            else if (isMale == true && movingToMate != true && runningAway != true)
            {
                returnTrueIfCanMate = true;
            }
        }
        movingToMate = true;
        mateMovingTo = animalInterestedInThis.gameObject;
        return returnTrueIfCanMate;
    }

    public void Movement()
    {
        if (Vector3.Distance(transform.position, positionMovingTo) <= stoppingDistance)
        {
            arrivedAtDesiredLocation = true;
        }

        if (runningAway == false && movingToMate != true)
        {            
            if (interested == true && arrivedAtDesiredLocation == true)
            {
                GotInRangeOfObjectOrPositionInterestedIn(objectInterestedIn);
                arrivedAtDesiredLocation = false;
                NavMeshMoveTo(positionMovingTo);
            }
            else if (interested == false)
            {
                if (arrivedAtDesiredLocation == true)
                {
                    arrivedAtDesiredLocation = false;
                    positionMovingTo = ecosystemMainManager.getRandomPosition();
                    NavMeshMoveTo(positionMovingTo);
                }
            }
        }

        if(movingToMate == true && runningAway == false)
        {
            positionMovingTo = mateMovingTo.transform.position;
            NavMeshMoveTo(positionMovingTo);
        }
    }

    public void NavMeshMoveTo(Vector3 desitnation)
    {
        navMeshAgent.SetDestination(desitnation);
    }

    private void GotInRangeOfObjectOrPositionInterestedIn(GameObject objectItWasInterestedIn)
    {
        //Eat Prey
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

    //FIX THIS
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
        closestPredator = null;
        closestPrey = null;
        anyPredatorInRange = AnyPredatorsInRange();
        anyPrey_producerInRange = AnyPreyInRange();
        //Get info on nearest predator
        if (allPredatorsInRange.Count > 1)
        {            
            closestPredator = closestGameobjectInAList(allPredatorsInRange);
            distanceToNearestPred = Vector3.Distance(closestPredator.transform.position, this.transform.position);
            interested = true;
        }
        else if (allPredatorsInRange.Count == 1)
        {            
            closestPredator = allPredatorsInRange[0];
            distanceToNearestPred = Vector3.Distance(closestPredator.transform.position, this.transform.position);
            interested = true;
        }
        else if (allPredatorsInRange.Count == 0)
        {
            closestPredator = null;
        }

        //Get info on nearest prey
        if (allPreyInRange.Count > 1)
        {           
            closestPrey = closestGameobjectInAList(allPreyInRange);
            distanceToNearestPrey_Consumable = Vector3.Distance(closestPrey.transform.position, this.transform.position);
            interested = true;
        }
        else if (allPreyInRange.Count == 1)
        {           
            closestPrey = allPreyInRange[0];
            distanceToNearestPrey_Consumable = Vector3.Distance(closestPrey.transform.position, this.transform.position);
            interested = true;
        }
        else if (allPreyInRange.Count == 0)
        {
            closestPrey = null;
        }
    }

    public void Interest()
    {
        //What is it interested in
        if (anyPredatorInRange == true || movingToMate == false)
        {
            if (anyPrey_producerInRange == true && anyPredatorInRange == true)
            {
                interested = true;

                if (distanceToNearestPred < distanceToNearestPrey_Consumable * fightOrFlightStrength10Flight0Fight)
                {
                    objectInterestedIn = closestPredator;
                    runningAway = true;
                    getRunawayPosition(closestPredator.transform.position);
                }
                else if (distanceToNearestPred > distanceToNearestPrey_Consumable * fightOrFlightStrength10Flight0Fight)
                {
                    runningAway = false;
                    objectInterestedIn = closestPrey;
                    positionMovingTo = closestPrey.transform.position;
                }
            }
            //only prey in range
            else if (anyPrey_producerInRange == true && anyPredatorInRange == false)
            {
                runningAway = false;
                anyPredatorInRange = false;
                closestPredator = null;

                interested = true;
                objectInterestedIn = closestPrey;
                positionMovingTo = closestPrey.transform.position;
            }
            //only pred in range
            else if (anyPrey_producerInRange == false && anyPredatorInRange == true)
            {
                runningAway = true;
                anyPrey_producerInRange = false;
                closestPrey = null;

                interested = true;
                objectInterestedIn = closestPredator;
                getRunawayPosition(closestPredator.transform.position);
            }
            //no pred or prey in range
            if (anyPrey_producerInRange == false && anyPredatorInRange == false)
            {
                runningAway = false;
                objectInterestedIn = null;
                interested = false;

                anyPredatorInRange = false;
                closestPredator = null;

                anyPrey_producerInRange = false;
                closestPrey = null;
            }
        }
        else if (anyPredatorInRange == false && movingToMate == true)
        {
            interested = true;
            objectInterestedIn = mateMovingTo;
            positionMovingTo = mateMovingTo.transform.position;
        }
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
                            allSameGenderInRange.Add(item);
                        }
                        else if (item.GetComponent<Consumer>().isMale != isMale)
                        {
                            allDifferentGenderInRange.Add(item);
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
            if (allPreyInRange.Contains(other.gameObject) == true)
            {
                allPreyInRange.Remove(other.gameObject);
            }
            if (allPreyInRange.Contains(other.gameObject) == true)
            {
                allPreyInRange.Remove(other.gameObject);
            }
            if (allSameGenderInRange.Contains(other.gameObject) == true)
            {
                allSameGenderInRange.Remove(other.gameObject);
            }
            if (allDifferentGenderInRange.Contains(other.gameObject) == true)
            {
                allDifferentGenderInRange.Remove(other.gameObject);
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
}

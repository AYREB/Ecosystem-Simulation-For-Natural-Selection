using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProducerNextTry : MonoBehaviour
{
    public GameObject foodToSpawn;
    public GameObject currentFoodGrown;
    public GameObject emptyObjectForFoodSpawnLocation;
    public SpeciesEnum species;

    [Header("Consumable Variables")]
    public int hungerRestoration;
    public float maxSizeScaleValue;
    public bool foodFullyGrown;

    public float foodGrowthCooldownTimer = 10.0f;
    public bool canSpawnFood = false;
    public bool foodCurrentlyExists;
    public bool foodReadyToBeEaten;
    public float scaleOfFoodForGrowingValue;
    public float growthSpeed;

    private void Start()
    {
        species = GetComponent<AllSpeciesReuirement>().species;
        currentFoodGrown = transform.GetChild(0).transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (foodCurrentlyExists && foodFullyGrown)
        {
            foodReadyToBeEaten = true;
        }
        else
        {
            foodReadyToBeEaten = false;
        }

        if (canSpawnFood == true && foodCurrentlyExists == false)
        {
            SpawnFood();
        }
        
        if (foodFullyGrown == false && foodCurrentlyExists == true)
        {
            FoodGrowth();
        }
    }

    public void BeenEaten(GameObject objectEatenBy)
    {       
        Destroy(currentFoodGrown);
        foodCurrentlyExists = false;
        StartCoroutine(PlantSpawnCooldown());
        //objectEatenBy.GetComponent<Consumer>().allObjectsInRange.Remove(this.gameObject);
        //objectEatenBy.GetComponent<Consumer>().allPreyInRange.Remove(this.gameObject);

        foreach (Consumer consumers in FindObjectsOfType<Consumer>())
        {
            if (consumers.allObjectsInRange.Contains(this.gameObject))
            {
                consumers.allObjectsInRange.Remove(this.gameObject);
                consumers.allPredatorsInRange.Remove(this.gameObject);
                consumers.allPreyInRange.Remove(this.gameObject);

                if (consumers.objectInterestedIn == this.gameObject)
                {
                    consumers.objectInterestedIn = null;
                    consumers.interested = false;
                }
            }
        }
    }


    private void SpawnFood()
    {
        scaleOfFoodForGrowingValue = 0;       
        foodCurrentlyExists = true;
        currentFoodGrown = Instantiate(foodToSpawn, emptyObjectForFoodSpawnLocation.transform.position, Quaternion.identity, transform.GetChild(0));
        currentFoodGrown.GetComponent<Transform>().localScale = new Vector3(0, 0, 0);
        foodFullyGrown = false;
        canSpawnFood = false;
    }

    private void FoodGrowth() 
    {
        if (currentFoodGrown.transform.localScale.x >= maxSizeScaleValue)
        {
            currentFoodGrown.transform.localScale = new Vector3(maxSizeScaleValue, maxSizeScaleValue, maxSizeScaleValue);
            foodFullyGrown = true;
            scaleOfFoodForGrowingValue = maxSizeScaleValue;
        }
        else
        {
            if (currentFoodGrown.transform.localScale.x < maxSizeScaleValue)
            {
                scaleOfFoodForGrowingValue += growthSpeed * Time.deltaTime;
                currentFoodGrown.transform.localScale = new Vector3(scaleOfFoodForGrowingValue, scaleOfFoodForGrowingValue, scaleOfFoodForGrowingValue);
            }
        }
    }

    private IEnumerator PlantSpawnCooldown()
    {
        canSpawnFood = false;
        yield return new WaitForSeconds(foodGrowthCooldownTimer);
        canSpawnFood = true;
    }
}

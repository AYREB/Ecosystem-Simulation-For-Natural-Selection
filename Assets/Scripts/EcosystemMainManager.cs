using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EcosystemMainManager : MonoBehaviour
{
    public bool animalsCanMutate;
    public bool plantsCanMutate;
    public int secondsPerYearInGame;

    [Range(10, 1000)]
    public float range;

    public InitalPopulations[] initalPopulations;
    // Start is called before the first frame update
    void Start()
    {

        //Loop through each animal population
        foreach (InitalPopulations pop in initalPopulations)
        {
            //Spawn one animal for each of the count
            for (int i = 0; i < pop.startingCount; i++)
            {
                SpawnAnimal(pop.prefab);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    public Vector3 getRandomPosition()
    {
        Vector3 position = new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));

        if (!Physics.Raycast(new Vector3(position.x, position.y + 2, position.z), Vector3.down, 10))
        {
            position = getRandomPosition();
        }

        return position;
    }

    public void SpawnAnimal(GameObject prefab)
    {
        Vector3 spawnPoint = getRandomPosition();

        GameObject justSpawned = Instantiate(prefab, new Vector3(spawnPoint.x, prefab.GetComponent<AllSpeciesReuirement>().SpawnOffset, spawnPoint.z), Quaternion.identity);

        if (justSpawned.GetComponent<Consumer>() != null)
        {
            justSpawned.GetComponent<Consumer>().isMale = Random.Range(1, 3);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawCube(transform.position, new Vector3(range, 100, range));
    }

    [System.Serializable]
    public class InitalPopulations
    {
        //Name of the array element
        public string name;
        //Animal prefab
        public GameObject prefab;
        //Inital population count
        public int startingCount;
    }
}

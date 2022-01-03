using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    
    //The size of the map
    [Range(10, 1000)]
    public float range;

    public InitialPopulations[] initialPopulations;

    public LayerMask animalLayer;

    public static int globalPop = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach(InitialPopulations pop in initialPopulations)
        {
            for(int i = 0;i < pop.count; i++)
            {
                SpawnAnimal(pop.prefab);
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Draw gizmos
    void OnDrawGizmos()
    {
        //Set the color to blue
        Gizmos.color = Color.blue;

        //Draw a cube
        Gizmos.DrawSphere(transform.position, range);
    }

    //A mothed to get a random position on the map
    public Vector3 getRandomPosition()
    {
        //Create a random position
        Vector3 position = new Vector3(Random.Range(-range, range), 5, Random.Range(-range, range));

        //Check if its above the ground
        if (!Physics.Raycast(position, Vector3.down, 10))
        {
            //Get a new random position
            position = getRandomPosition();
        }

        //Return the random position
        return position;
    }

    public Vector3 getRunAwayPosition(Transform target)
    {
        Vector3 position = Vector3.zero;
        float currentDistance = 1;

        for(int i = 0; i < 20; i++)
        {
            Vector3 tryPosition = getRandomPosition();
            float dist = Vector3.Distance(tryPosition, target.position);
            if(dist > currentDistance)
            {
                position = tryPosition;
                currentDistance = dist;
            }
        }
        return position;
    }

    //Spanw animal method
    public void SpawnAnimal(GameObject prefab)
    {
        //Spawn point
        Vector3 spawnPoint = getRandomPosition();

        //Editing the spawn point to avoid floating animals
        spawnPoint = new Vector3(spawnPoint.x, prefab.transform.position.y, spawnPoint.z);

        //Spawning the animal
        Instantiate(prefab, spawnPoint, Quaternion.identity);
        globalPop++;
        Debug.Log("The global population is " + globalPop);
    }
}

[System.Serializable]
public class InitialPopulations
{
    public string name;
    public GameObject prefab;
    public int count;
}

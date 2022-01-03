using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Animal : MonoBehaviour
{
    //The position the animal is moving to
    Vector3 targetPosition;

    //The species of the animal
    public Species species;

    //The diet of the animal
    public Species diet;
 
    public float hunger;

    //Manage Gauss
    private float minSize = 0;
    private float maxSize = 0;
    private float maxSuperSize = 0;
    
    bool foundInterest = false;

    bool rangeOne = false;

    bool danger = false;

    float timeToDeathByHunger = 200;

    //The speed of the animal
    public float speed = 10;
    public float speedMin = 0;
    public float speedMax = 0;

    //The vision radius of the animal
    public float visionRadius = 30;
    public float visionRadiusMin = 0;
    public float visionRadiusMax = 0;

    // Gender of the animal
    public bool male;

    // The amount of how much the animal values reproducing over other things
    public float reprocuctiveUrge = 0.6f;

    // The amount of time before the animal can reproduce again
    float childCoolDown = 50;

    // The time before which an animal can not have another child
    public float timeToNextChild;

    //The environment
    Environment environment;

    // Start is called before the first frame update
    void Start()
    {
        //Get the environment
        environment = FindObjectOfType<Environment>();

        // Assign a random value to the gender
        male = Random.value < 0.5f;

        //Set the target position to a random position
        targetPosition = environment.getRandomPosition();

        //Add a trigger to the animal
        SphereCollider trigger = gameObject.AddComponent<SphereCollider>();

        //Set the radius of the trigger to the vision radius
        trigger.radius = visionRadius;
        //Set it to a trigger
        trigger.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Check the distance between the animal and it's target position
        float dist = Vector3.Distance(transform.position, new Vector3(targetPosition.x, transform.position.y, targetPosition.z));

        rangeOne = dist <= 1;
        //Check if the distance is less than or equal to 1
        if (dist <= 1)
        {
            rangeOne = true;

            if (!foundInterest)
            {
                //Set the target position to a random position
                targetPosition = environment.getRandomPosition();
                danger = false;
            }
               
        }

        float hungerTime = speed / 3;

        hunger += Time.deltaTime * hungerTime / timeToDeathByHunger;

        if(hunger >= 1)
        {
            GetComponent<LiveEntity>().Die(CauseOfDeath.hunger);
        }
        //Move the animal
        Move(targetPosition);
    }

    //Move method
    void Move(Vector3 targetPosition)
    {
        //Create a new target position so the animal doesn't move on the y axis
        Vector3 target = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        //Move to the target position
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        //Look at the target position
        transform.LookAt(target);
    }

    public static float RandomGaussian(float minValue, float maxValue)
    {
        float u, v, S;

        do
        {
            u = 2.0f * UnityEngine.Random.value - 1.0f;
            v = 2.0f * UnityEngine.Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        // Standard Normal Distribution
        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

        // Normal Distribution centered between the min and max value
        // and clamped following the "three-sigma rule"
        float mean = (minValue + maxValue) / 2.0f;
        float sigma = (maxValue - mean) / 3.0f;
        return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }

    //Check if something is still in the vision radius 
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<LiveEntity>(out LiveEntity entity))
        {
            // Check to make sure that the other species is the same as ours
            if (entity.species == species)
            {
                if (reprocuctiveUrge > hunger) // Make sure reproduction is needed
                {
                    // Get the animal script of the species
                    Animal ani = entity.GetComponent<Animal>();

                    // Make sure reproduction is possible
                    if (ani.reprocuctiveUrge > ani.hunger && Time.time >= timeToNextChild && Time.time >= ani.timeToNextChild)
                    {
                        // Set the target position
                        targetPosition = ani.transform.position;

                        // Check the distance again
                        float dist = Vector3.Distance(transform.position, new Vector3(targetPosition.x, transform.position.y, targetPosition.z));

                        // Make sure that they are close enough to reproduce
                        if (dist <= 1)
                        {
                            // Set reproduction cool down
                            timeToNextChild = Time.time + childCoolDown;

                            // Make sure the animal giving birth is female
                            if (!male)
                            {
                                // Clone the animal
                                var child = Instantiate(this.gameObject);

                                // Get the animal script
                                Animal childAnimal = child.GetComponent<Animal>();

                                // Enable the animal script
                                childAnimal.enabled = true;

                                // Enable the live entity script
                                child.GetComponent<LiveEntity>().enabled = true;

                                // Randomize the gender
                                childAnimal.male = Random.value < 0.5f;
                                
                                switch (entity.species)
                                {
                                    case Species.loup:
                                        maxSize = 7;
                                        minSize = 2;
                                        speedMin = 4;
                                        speedMax = 10;
                                        visionRadiusMin = 12;
                                        visionRadiusMax = 35;
                                        break;
                                    case Species.poule:
                                        maxSize = 3;
                                        minSize = 1;
                                        speedMin = 2;
                                        speedMax = 8;
                                        visionRadiusMin = 12;
                                        visionRadiusMax = 35;
                                        break;
                                    case Species.blueCube:
                                        maxSize = 9;
                                        minSize = 2;
                                        speedMin = 3;
                                        speedMax = 11;
                                        visionRadiusMin = 12;
                                        visionRadiusMax = 35;
                                        break;
                                    default: break;
                                }

                                
                                childAnimal.transform.localScale = new Vector3(RandomGaussian(minSize, maxSize), RandomGaussian(minSize, maxSize), RandomGaussian(minSize, maxSize));
                                
                                // Reset the hunger of the child
                                childAnimal.hunger = 0;
                                childAnimal.speed = RandomGaussian(speedMin, speedMax);
                                childAnimal.visionRadius = RandomGaussian(visionRadiusMin, visionRadiusMax);
                            }

                            // Print a log to the console for debuging
                            Debug.Log("New clones!");
                        }
                    }
                }
            }
            if (entity.species == diet)
            {
                targetPosition = other.transform.position;
                foundInterest = true;

                float dist = Vector3.Distance(transform.position, new Vector3(targetPosition.x, transform.position.y, targetPosition.z)); // Get the distance between the animal and it's target position

                if (dist <= 1) // Use the distance instead of rangeOne
                {
                    hunger = 0;
                    entity.Die(CauseOfDeath.beingEaten);
                    foundInterest = false;
                }
            }
            if (entity.species != Species.poule && entity.gameObject.GetComponent<Animal>().diet == species && !danger)
            {
                //targetPosition = enviroment.getRunAwayPosition(entity.transform);

                danger = true;
            }
        }
        else
        {
            if (foundInterest == true)
            {
                foundInterest = false;
            }
        }
    }
}
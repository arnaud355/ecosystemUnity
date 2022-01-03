using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveEntity : MonoBehaviour
{
    //The species of the animal
    public Species species;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Die(CauseOfDeath cause)
    {
        Destroy(gameObject);
        Debug.Log(gameObject.name + "'s cause of death was '" + cause + "'");
        //Environment.globalPop--;
        //Debug.Log("The global population after this lost is " + Environment.globalPop);
    }
}
public enum CauseOfDeath
{
    hunger, beingEaten
}

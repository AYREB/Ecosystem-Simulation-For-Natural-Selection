using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Growth : MonoBehaviour
{
    public Consumer consumerScript;
    public EcosystemMainManager ecosystemMainManager;
    public int secondsPerYearInGame;
    public GameObject body;
    public float timeOfBirth;
    public float scaleAtBirth;
    public float timerValue;

    public float ScaleAddedPeyYear;

    public int Age;
    public float ageChildMax;
    public float ageAdultMax;
    public float ageElderlyMaxorDeath;

    public bool happened1;
    public bool happened2;
    public bool happened3;


    // Start is called before the first frame update
    void Start()
    {
        happened1 = false;
        happened2 = false;
        happened3 = false;
        timeOfBirth = Time.time;
        ecosystemMainManager = FindObjectOfType<EcosystemMainManager>();
        secondsPerYearInGame = ecosystemMainManager.secondsPerYearInGame;
        scaleAtBirth = consumerScript.scaleOfAnimalAtBirth;
        timerValue = secondsPerYearInGame;
        ageElderlyMaxorDeath = consumerScript.lifespanYears;
        AssignAgesNames();
        AgePrequisits();
        consumerScript = GetComponent<Consumer>();


        consumerScript.isFertile = false;

        body.transform.localScale = new Vector3(scaleAtBirth, scaleAtBirth, scaleAtBirth);
    }


    // Update is called once per frame
    void Update()
    {
        TimerForAge();
    }

    void AssignAgesNames()
    {       
        ageAdultMax = ageElderlyMaxorDeath * 0.75f;
        ageChildMax = ageElderlyMaxorDeath * 0.2f;
        ScaleAddedPeyYear = (1 - scaleAtBirth) / ageChildMax;
    }

    public void TimerForAge()
    {
        timerValue -= Time.deltaTime;

        if (timerValue <= 0.0f)
        {
            OneYearPassed();
        }
    }

    public void OneYearPassed()
    {
        Age += 1;
        timerValue = secondsPerYearInGame;
        AgePrequisits();
    }

    public void AgePrequisits()
    {
        if (Age <= ageChildMax)
        {
            if (happened1 == false)
            {
                consumerScript.isFertile = false;
                happened1 = true;
            }           
            ScaleUpAsGrowing();
        }
        else if (Age > ageChildMax || Age <= ageAdultMax)
        {
            if (happened2 == false)
            {
                consumerScript.isFertile = true;
                happened2 = true;
            }
        }
        else if (Age > ageAdultMax || Age < ageElderlyMaxorDeath)
        {
            if(happened3 == false)
            {
                consumerScript.isFertile = false;
                happened3 = true;
            }          
        }
        else if (Age == ageElderlyMaxorDeath)
        {
            consumerScript.DiedWithoutBeingEaten();
            Debug.Log("died without being eaten");
        }
    }

    private void ScaleUpAsGrowing()
    {
        body.transform.localScale += new Vector3(ScaleAddedPeyYear, ScaleAddedPeyYear, ScaleAddedPeyYear);
    }
}

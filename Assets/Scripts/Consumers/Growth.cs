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
    private float ageChildMax;
    private float ageAdultMax;
    private float ageElderlyMaxorDeath;


    // Start is called before the first frame update
    void Start()
    {
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
            consumerScript.isFertile = false;
            ScaleUpAsGrowing();
        }
        else if (Age > ageChildMax || Age <= ageAdultMax)
        {
            consumerScript.isFertile = true;
        }
        else if (Age > ageAdultMax || Age < ageElderlyMaxorDeath)
        {
            consumerScript.isFertile = false;
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

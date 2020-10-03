using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum State
{
    eating,
    drinking,
    sozializing,
    sleeping,
    charging
}
public class HamsterManager : MonoBehaviour
{
    int food = 900;
    int water = 800;
    int sozial = 1000;
    int sleep = 800;
    int batteryPower = 600;
    int wheelPerformance = 1000;

    int time = 0;

    State currentState = State.sleeping;

    [SerializeField] Transform hamster;
    [SerializeField] Transform wheel;
    [SerializeField] Transform lamp;
    [SerializeField] Canvas tutorial;
    [Header("Ui")]
    [SerializeField] Text timeText;
    [SerializeField] Text dayText;
    [SerializeField] Text chargeText;
    [SerializeField] Text wheelPerformanceText;
    [SerializeField] Text foodText;
    [SerializeField] Text waterText;
    [SerializeField] Text sozialText;
    [SerializeField] Text sleepText;
    [Header("GameEnd")]
    [SerializeField] Canvas mainCanvas;
    [SerializeField] Canvas endCanvas;
    [SerializeField] GameObject buttonHolder;
    [SerializeField] Text headerText;
    [SerializeField] Text bottomText;
    [Header("Pos")]
    [SerializeField] Transform eatingPos;
    [SerializeField] Transform drinkingPos;
    [SerializeField] Transform sozializingPos;
    [SerializeField] Transform chargingPos;
    [SerializeField] Transform sleepingPos;
    [Header("Add")]
    [SerializeField] int cap = 1000;
    [SerializeField] int charge = 5;
    [SerializeField] int eat = 5;
    [SerializeField] int drink = 5;
    [SerializeField] int sozialize = 5;
    [SerializeField] int sleeping = 5;
    [Header("Remove")]
    [SerializeField] int discharge = 5;
    [SerializeField] int foodUse = 5;
    [SerializeField] int waterUse = 5;
    [SerializeField] int sozialUse = 5;
    [SerializeField] int sleepUse = 5;

    IEnumerator Tick()
    {
        while (true)
        {
            RemoveValues();
            wheelPerformance = (food + water + sozial + sleep) / 4; //update weelPerformance for charging
            AddValues();
            wheelPerformance = (food + water + sozial + sleep) / 4; //update weelPerformance for not charging

            Debug.Log((batteryPower + food + water + sozial + sleep));
            CheckForEnd();
            UpdateTime();
            UpdateStatusUi();

            yield return new WaitForSecondsRealtime(1);
        }
    }

    private void Update()
    {
        if(currentState == State.charging)
        {
            wheel.localRotation =  Quaternion.Euler(wheel.localRotation.eulerAngles  + Vector3.down);
        }
    }

    private void CheckForEnd()
    {
        if (batteryPower <= 0)
        {
            EndGame("Lights out!", "You managed to keep the lights on for " + (time / 144) + " Days, " + (time % 144) / 6 + " hours, and " + time % 6 + "0 minutes.", Color.red, false);
        }
        else if (food <= 0 || water <= 0 || sleep <= 0 || sozial <= 0)
        {
            EndGame("You died!", "After " + (time / 144) + " Days, " + (time % 144) / 6 + " hours, and " + time % 6 + "0 minutes you collapse, hoping someone else will continue your duty.", Color.red, false);
        }
        else if (batteryPower >= 950 && food >= 950 && water >= 950 && sleep >= 950 && sozial >= 950)
        {
            EndGame("Perfectly balanced", "In " + (time / 144) + " Days, " + (time % 144) / 6 + " hours, and " + time % 6 + "0 minutes you balance your life, continuing your duty easily for a long time.", Color.green, true);
        }
    }

    private void EndGame(string header, string bottom, Color color, bool lampStatus)
    {
        lamp.gameObject.SetActive(lampStatus);
        mainCanvas.gameObject.SetActive(false);
        endCanvas.gameObject.SetActive(true);
        buttonHolder.SetActive(false);
        headerText.text = header;
        headerText.color = color;
        bottomText.text = bottom;
        StopAllCoroutines();
    }

    private void UpdateStatusUi()
    {
        chargeText.text = "Battery charge: " + batteryPower / 10 + "%";
        wheelPerformanceText.text = "Performance: " + wheelPerformance / 10 + "%";
        foodText.text = "Food: " + food / 10 + "%";
        waterText.text = "Water: " + water / 10 + "%";
        sozialText.text = "Social: " + sozial / 10 + "%";
        sleepText.text = "Sleep: " + sleep / 10 + "%";
    }

    private void UpdateTime()
    {
        time++;
        timeText.text = (time % 144) / 6 + ":" + time % 6 + "0";
        dayText.text = "Day: " + ((time / 144) + 1);
    }

    private void RemoveValues()
    {
        batteryPower -= discharge;
        food -= foodUse;
        water -= waterUse;
        sozial -= sozialUse;
        sleep -= sleepUse;
    }

    private void AddValues()
    {
        if (currentState == State.charging)
        {
            batteryPower += Mathf.CeilToInt(charge * ((float)wheelPerformance / cap));
            if(batteryPower >= cap)
            {
                batteryPower = cap;
            }
        }
        else if (currentState == State.drinking)
        {
            water += drink;
            if (water >= cap)
            {
                water = cap;
            }
        }
        else if (currentState == State.eating)
        {
            food += eat;
            if (food >= cap)
            {
                food = cap;
            }
        }
        else if (currentState == State.sleeping)
        {
            sleep += sleeping;
            if (sleep >= cap)
            {
                sleep = cap;
            }
        }
        else
        {
            sozial += sozialize;
            if (sozial >= cap)
            {
                sozial = cap;
            }
        }
    }

    public void SetState(int state)
    {
        currentState = (State)state;
        if (currentState == State.charging)
        {
            ChangeTransform(chargingPos);
        }
        else if (currentState == State.drinking)
        {
            ChangeTransform(drinkingPos);
        }
        else if (currentState == State.eating)
        {
            ChangeTransform(eatingPos);
        }
        else if (currentState == State.sleeping)
        {
            ChangeTransform(sleepingPos);
        }
        else
        {
            ChangeTransform(sozializingPos);
        }
    }

    public void StartGame()
    {
        tutorial.gameObject.SetActive(false);
        buttonHolder.SetActive(true);
        mainCanvas.gameObject.SetActive(true);
        SetState(3);
        StartCoroutine(Tick());
    }

    void ChangeTransform(Transform refTransform)
    {
        hamster.position = refTransform.position;
        hamster.rotation = refTransform.rotation;
    }
}

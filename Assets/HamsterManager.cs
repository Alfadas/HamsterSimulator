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
    int food = 0;
    int water = 0;
    int sozial = 0;
    int sleep = 0;
    int batteryPower = 0;
    int wheelPerformance = 0;

    int time = 0;

    State currentState = State.sleeping;

    [SerializeField] Transform hamster;
    [SerializeField] Transform wheel;
    [SerializeField] Transform lamp;
    [SerializeField] Canvas tutorial;
    [SerializeField] Canvas menuCanvas;
    [Header("Audio")]
    [SerializeField] AudioSource musikSource;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] Slider slider;
    [SerializeField] AudioClip[] sfx;
    [SerializeField] Text musikButtonText;
    [SerializeField] Text sfxButtonText;
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

    void Start()
    {
        ChangeVolume(0.5f);
    }

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
        if (currentState == State.charging)
        {
            wheel.localRotation = Quaternion.Euler(wheel.localRotation.eulerAngles + Vector3.down);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuCanvas.gameObject.SetActive(!menuCanvas.gameObject.activeSelf);
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
            EndGame("You died!", "After " + (time / 144) + " Days, " + (time % 144) / 6 + " hours, and " + time % 6 + "0 minutes you collapse, trying to do more than you can alone. Don't struggle alone, ask for help.", Color.red, false);
        }
        else if (batteryPower >= 950 && food >= 950 && water >= 950 && sleep >= 950 && sozial >= 950)
        {
            EndGame("Lights on Forever", "In " + (time / 144) + " Days, " + (time % 144) / 6 + " hours, and " + time % 6 + "0 minutes you changed your life, continuing your duty easily for a long time.", Color.green, true);
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
            if (batteryPower >= cap)
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
                batteryPower += Mathf.CeilToInt(charge * ((float)wheelPerformance / cap));
                water += drink;
                food += eat;
                sleep += sleeping;
                if (sleep >= cap)
                {
                    sleep = cap;
                }
                if (food >= cap)
                {
                    food = cap;
                }
                if (water >= cap)
                {
                    water = cap;
                }
                if (batteryPower >= cap)
                {
                    batteryPower = cap;
                }
            }
        }
    }

    public void SetState(int state)
    {
        currentState = (State)state;
        sfxSource.clip = sfx[state];
        sfxSource.Play();
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
        StopAllCoroutines();
        food = 900;
        water = 750;
        sozial = 900;
        sleep = 850;
        batteryPower = 600;
        wheelPerformance = 1000;

        time = 0;
        lamp.gameObject.SetActive(true);
        endCanvas.gameObject.SetActive(false);
        menuCanvas.gameObject.SetActive(false);
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

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleSFX()
    {
        sfxSource.enabled = !sfxSource.enabled;
        if (sfxSource.enabled)
        {
            sfxButtonText.text = "Disable SFX";
        }
        else
        {
            sfxButtonText.text = "Enable SFX";
        }
    }

    public void ToggleMusik()
    {
        musikSource.enabled = !musikSource.enabled;
        if (musikSource.enabled)
        {
            musikButtonText.text = "Disable Musik";
        }
        else
        {
            musikButtonText.text = "Enable Musik";
        }
    }

    public void ChangeVolume(float volume)
    {
        if (volume == -1f)
        {
            volume = slider.value;
        }
        sfxSource.volume = volume * 0.5f;
        musikSource.volume = volume * 0.5f;
    }
}
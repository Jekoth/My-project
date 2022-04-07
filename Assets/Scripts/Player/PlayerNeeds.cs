using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerNeeds : MonoBehaviour
{
    public Need health;
    public Need hunger;
    public Need thirst;
    public Need sleep;

    public float hungerHealthDecay;
    public float thirtHealthDecay;

    public UnityEvent onTakeDamage;

    private void Start()
    {
        //Aseta aloitus arvot
        health.currentValue = health.startValue;
        thirst.currentValue = thirst.startValue;
        hunger.currentValue = hunger.startValue;
        sleep.currentValue = sleep.startValue;
    }

    private void Update()
    {
        //V‰henn‰ tarpeita ajan myˆt‰
        hunger.Subtrack(hunger.decayRate * Time.deltaTime);
        thirst.Subtrack(thirst.decayRate * Time.deltaTime);
        sleep.Add(sleep.regenRate * Time.deltaTime);

        //V‰henn‰ terveytt‰, jos meill‰ ei ole ruokaa
        if(hunger.currentValue == 0.0f)
        {
            health.Subtrack(hungerHealthDecay * Time.deltaTime);
        }

        //V‰henn‰ terveytt‰, jos meill‰ ei ole vett‰
        if (thirst.currentValue == 0.0f)
        {
            thirst.Subtrack(thirtHealthDecay * Time.deltaTime);
        }

        //Katso, onko pelaaja kuollut
        if(health.currentValue == 0.0f)
        {
            Die();
        }

        //P‰ivit‰ UI palkit
        health.uiBar.fillAmount = health.GetPercentage();
        hunger.uiBar.fillAmount = hunger.GetPercentage();
        thirst.uiBar.fillAmount = thirst.GetPercentage();
        sleep.uiBar.fillAmount = sleep.GetPercentage();
    }

    public void Heal(float amount)
    {
        //lis‰‰ terveytt‰
        health.Add(amount);
    }

    public void Eat(float amount)
    {
        //lis‰‰ ruoka
        hunger.Add(amount);
    }
    public void Drink(float amount)
    {
        //lis‰‰ vett‰
        thirst.Add(amount);
    }
    public void Sleep(float amount)
    {
        //V‰henn‰ unta 
        sleep.Subtrack(amount);
    }

    //Ottaa vahinkoa
    public void TakeDamage(float amount)
    {
        health.Subtrack(amount);
        onTakeDamage?.Invoke();
    }

    public void Die()
    {
        Debug.Log("Player is dead");
    }
}

[System.Serializable]
public class Need
{
    [HideInInspector]
    public float currentValue;
    public float maxValue;
    public float startValue;
    public float regenRate;
    public float decayRate;
    public Image uiBar;

    //Lis‰‰
    public void Add(float amount)
    {
        currentValue = Mathf.Min(currentValue + amount, maxValue);
    }

    //V‰henn‰
    public void Subtrack(float amount)
    {
        currentValue = Mathf.Max(currentValue - amount, 0.0f);
    }

    public float GetPercentage()
    {
        return currentValue / maxValue;
    }
}

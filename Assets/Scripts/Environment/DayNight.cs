using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNight : MonoBehaviour
{
    //11:59, 12:00
    [Range(0.0f, 1.0f)]
    public float time;
    public float fullDayLength;
    public float startTime = 0.5f;
    private float timeRate;
    public Vector3 noon;

    [Header("Sun Setting")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;

    [Header("Moon Setting")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("lighting Setting")]
    public AnimationCurve lightingIntensityMultiplier;
    public AnimationCurve reflectionsIntensityMultiplier;

    private void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;
    }

    private void Update()
    {
        //Lisää aikaa
        time += timeRate * Time.deltaTime;
        //Jos aika on isompi tai yhtä suuri kuin 1, sitten aseta takaisin 0 (Kiertää 1:n ja 0:n välillä)
        if (time >= 1.0f)
            time = 0.0f;

        //Valon kierto
        sun.transform.eulerAngles = (time - 0.25f) * noon * 4.0f;
        moon.transform.eulerAngles = (time - 0.75f) * noon * 4.0f;

        //Light intensity
        sun.intensity = sunIntensity.Evaluate(time);
        moon.intensity = moonIntensity.Evaluate(time);

        //Vaihda värit
        sun.color = sunColor.Evaluate(time);
        moon.color = moonColor.Evaluate(time);

        //Ota käyttöön ja poista käytöstä aurinkon
        if (sun.intensity == 0 && sun.gameObject.activeInHierarchy)
            sun.gameObject.SetActive(false);
        else if (sun.intensity > 0 && !sun.gameObject.activeInHierarchy)
            sun.gameObject.SetActive(true);

        //Ota käyttöön ja poista käytöstä kuun
        if (moon.intensity == 0 && moon.gameObject.activeInHierarchy)
            moon.gameObject.SetActive(false);
        else if (moon.intensity > 0 && !moon.gameObject.activeInHierarchy)
            moon.gameObject.SetActive(true);

        //Valon ja heijastusten voimakkuus
        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = lightingIntensityMultiplier.Evaluate(time);

    }
}

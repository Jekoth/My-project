using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    public Image bloodImage;
    public float flashSpeed;

    private Coroutine fadeAway;

    //Kutsutaan kun pelaaja saa vahinkoa. (OnTakeDamage event)
    public void Flash()
    {
        //Pys‰ytt‰‰ kaikki k‰ynniss‰ olevat FadeAway korutiinit
        if (fadeAway != null)
            StopCoroutine(fadeAway);
        //kuva p‰‰lle
        bloodImage.enabled = true;
        //kuvan alfa-arvo muuntaa 1
        bloodImage.color = Color.white;
        fadeAway = StartCoroutine(FadeAway());
    }

    //H‰ivytt‰‰ kuvan ajan myˆt‰
    IEnumerator FadeAway()
    {
        float alphaImage = 1.0f;

        while(alphaImage > 0)
        {
            alphaImage -= (1.0f / flashSpeed) * Time.deltaTime;
            bloodImage.color = new Color(1.0f, 1.0f, 1.0f, alphaImage);
            //Sirry seuraavaan framiin t‰m‰n silmukan seuuraava iteraatiota varten.
            yield return null;
        }

        // kuva pois p‰‰lt‰
        bloodImage.enabled = false;
    }
}

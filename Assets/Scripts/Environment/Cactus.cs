using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cactus : MonoBehaviour
{
    public int damage;
    public float damageRate;

    private List<IDamagable> thingsToDamage = new List<IDamagable>();

    private void Start()
    {
        StartCoroutine(DealDamage());
    }

    /// <summary>
    /// Tee vahinkoa jokaiselle thingsToDamage-luettelossa olevalle objectille.
    /// Joten sit� varten loin IEnumeratorin nimelt� "DealDamage"
    /// </summary>
    IEnumerator DealDamage()
    {
        // Joka "damageRate" sekunnit, vahingoittaa kaikkea thingsToDamage kuuluva.
        while (true)
        {
            for(int i = 0; i < thingsToDamage.Count; i++)
            {
                thingsToDamage[i].TakeDamage(damage);
            }
            yield return new WaitForSeconds(damageRate);
        }
    }

    /// <summary>
    /// Elementtien lis��minen luetteloon aina, kun jokin IDamagable-liittym�ll� varustettu esine koskettaa kaktusta.
    /// Joten kun pelaaja tai vihollinen koskettaa kaktusta, he ottavat vahinkoa jokaisen damagaRate sekunnin v�lein.
    /// </summary>
    /// <param name="collision"></param>

    //Kutsuu kun, jotain t�rm�� kaktukseen.
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<IDamagable>() != null)
        {
            thingsToDamage.Add(collision.gameObject.GetComponent<IDamagable>());
        }
    }

    //Kutsuu kun, jotain lopettaa t�rm�yksen kaktukseen. Poistetaan luettelosta. (LIST)
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<IDamagable>() != null)
        {
            thingsToDamage.Remove(collision.gameObject.GetComponent<IDamagable>());
        }
    }
}

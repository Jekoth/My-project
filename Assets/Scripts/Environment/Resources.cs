using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resources : MonoBehaviour
{
    public ItemDatabase itemToGive;
    public int capacity;
    public int quantityPerHit = 1;
    public GameObject hitParticle;

    public void Gather(Vector3 hitpoint, Vector3 hitNormal)
    {
        for(int i = 0; i < quantityPerHit; i++)
        {
            //Jos kapasiteetista tulee 0, katkaise silmukka �l�k� en�� k�yt� t�t�
            if (capacity <= 0)
                break;

            //V�henn� kapasiteettia yhdell� jokaisella osumalla
            capacity -= 1;

            //Lis�� resurssia inventoriin
            Inventory.instance.AddItem(itemToGive);
        }
        //Toteuta particle effekti, jossa osumme puuhun
         Destroy(Instantiate(hitParticle, hitpoint, Quaternion.LookRotation(hitNormal, Vector3.up)), 1.0f);

        if (capacity <= 0)
            Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipTool : Equip
{
    public float attackRate;
    public float attackDistance;
    private bool attacking;

    [Header("Combat")]
    public bool doesDealDamage;
    public int damage;

    [Header("Resource Gathering")]
    public bool doesGatherResources;

    //Komponentit
    private Animator anim;
    private Camera cam;

    private void Awake()
    {
        //Hae komponentteja
        anim = GetComponent<Animator>();
        cam = Camera.main;
    }

    public override void OnAttackInput()
    {
        if (!attacking)
        {
            attacking = true;
            anim.SetTrigger("Attack");
            Invoke("OnCanAttack", attackRate);
        }
    }

    void OnCanAttack()
    {
        attacking = false;
    }

    public void OnHit()
    {
        //Aseta säde ampumaan näytön keskeltä
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        //Tallenna kaikki todelliset osumatiedot
        RaycastHit hit;

        //Ammu raycast
        if(Physics.Raycast(ray, out hit, attackDistance))
        {
            //Jos osumme resursseihin
            if(doesGatherResources && hit.collider.GetComponent<Resources>())
            {
                hit.collider.GetComponent<Resources>().Gather(hit.point, hit.normal);
            }
            //If we hit damageable or enemy
            if(doesDealDamage && hit.collider.GetComponent<IDamagable>() != null)
            {
                hit.collider.GetComponent<IDamagable>().TakeDamage(damage);
            }
        }
    }
}

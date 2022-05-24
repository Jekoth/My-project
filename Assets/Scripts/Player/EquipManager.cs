using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipManager : MonoBehaviour
{
    public Equip currentEquip;
    public Transform equipParent;
    private PlayerController controller;

    //Singelton
    public static EquipManager instance;

    private void Awake()
    {
        instance = this;
        controller = GetComponent<PlayerController>();
    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        //Vain kun painat alas && nykyinen varuste on olemassa && Playerin ohjain voi katsoa ympärilleen
        if (context.phase == InputActionPhase.Performed && currentEquip != null &&controller.canLook == true)
        {
            currentEquip.OnAttackInput();
        }
    }

    public void OnAltAttackInput(InputAction.CallbackContext context)
    {
        //Vain kun painat alas && nykyinen varuste on olemassa && Playerin ohjain voi katsoa ympärilleen
        if (context.phase == InputActionPhase.Performed && currentEquip != null && controller.canLook == true)
        {
            currentEquip.OnAltAttackInput();
        }
    }

    public void EquipNewItem(ItemDatabase item)
    {
        //Poistaa kaiken, mitä hahmolla tällä hetkellä on
        UnEquipItem();
        //Luo uusi kohde kameran child objektina ja hanki siihen komponentti
        currentEquip = Instantiate(item.equipPrefab, equipParent).GetComponent<Equip>();
    }

    public void UnEquipItem()
    {
        //Tarkista, onko meillä jotain unequip, ja jos on, tuhoamme kohteen ja aseta nykyiset varusteet null
        if (currentEquip != null)
        {
            Destroy(currentEquip.gameObject);
            currentEquip = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

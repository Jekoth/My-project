using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    public ItemSlotUI[] uiSlots;
    public ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform dropPosition;

    [Header("Selected item")]
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDespcription;
    public TextMeshProUGUI selectedItemStatName;
    public TextMeshProUGUI selectedItemStatValue;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unequipButton;
    public GameObject dropButton;

    private int currentEquipIndex;

    //Komponentit
    private PlayerController controller;
    private PlayerNeeds needs;


    [Header("Events")]
    public UnityEvent onOpenInventory;
    public UnityEvent onCloseInventory;

    //Singelton
    public static Inventory instance;

    private void Awake()
    {
        instance = this;
        controller = GetComponent<PlayerController>();
        needs = GetComponent<PlayerNeeds>();
    }

    private void Start()
    {
        inventoryWindow.SetActive(false);
        slots = new ItemSlot[uiSlots.Length];

        // Alustaa paikat
        for (int x = 0; x < slots.Length; x++)
        {
            slots[x] = new ItemSlot();
            uiSlots[x].index = x;
            //Kaikki slotit on asetettu tyhjäksi, joten meillä ei ole mitään esinettä
            uiSlots[x].Clear();
        }

        ClearSelectedItemWindow();
    }

    public void OnInventoryButton(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        if (inventoryWindow.activeInHierarchy)
        {
            //Sulje inventori
            inventoryWindow.SetActive(false);
            onCloseInventory.Invoke();
            controller.ToggleCursor(false);
        }
        else
        {
            //Avaa inventori
            inventoryWindow.SetActive(true);
            onOpenInventory.Invoke();
            ClearSelectedItemWindow();
            controller.ToggleCursor(true);

        }
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    //Tarkista, voimmeko pinota kohteen, jos voimme pinota, ja etsi pino lisätäksesi sen siihen,
    //jos se ei voi lisätä sitä tyhjään paikkaan
    public void AddItem(ItemDatabase item)
    {
        //Tarkista, voimmeko pinota tuotteen
        if (item.canStackItem)
        {
            //Hanki slotti pinoamista varten ja lähetä sen päälle, jonka haluamme pinota
            ItemSlot slotToStackTo = GetItemStack(item);

            //Jos löytää jotain pinottavaa
            if (slotToStackTo != null)
            {
                //Lisää yksi tähän määrään
                slotToStackTo.quantity++;
                UpdateUI();
                return;
            }
        }

        //Ei ole tyhjä slotti, etsi tyhjä slotti
        ItemSlot emptySlot = GetEmptySlot();
        //Jos löydämme tyhjän slotin
        if (emptySlot != null)
        {
            //Aseta item slottiin
            emptySlot.item = item;
            //Aseta määräksi 1
            emptySlot.quantity = 1;
            UpdateUI();
            return;
        }

        //Jos meillä ei ole paikkaa vapaana, tuotetta ei voi pinota ja sitten vain heittää pois
        ThrowItem(item);
    }

    //Synnyttää esineen
    void ThrowItem(ItemDatabase item)
    {
        //Luo esine hahmon(player) eteen satunnaisella kierrolla
        Instantiate(item.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360.0f));
    }

    void UpdateUI()
    {
        //Selaa jokaisen UI slotin paikka
        for(int x = 0; x < slots.Length; x++)
        {
            //Sisältääkö tämä slotti itemin
            if (slots[x].item != null)
            {
                //Aseta ui-slot, jos se on totta
                uiSlots[x].Set(slots[x]);
            }

            //Jos ei ole totta
            else
            {
                //Tyhjennä UI slotin
                uiSlots[x].Clear();
            }
        }
    }

    //Etsi inventorin ja yritä löytää item slot, johon voimme pinota nämä uudet items.
    ItemSlot GetItemStack(ItemDatabase item)
    {
        for(int x = 0; x < slots.Length; x++)
        {
            //Tarkista, onko paikan sisältävä item se itemi, jonka haluamme lisätä, eikä sen määrä ylitä pinon enimmäismäärää
            if (slots[x].item == item && slots[x].quantity < item.maxStackAmount)
            {
                //Palauta item slotti, johon pyydetty itemin voidaan pinota
                return slots[x];
            }
        }

        //Jos pinoa ei ole saatavilla palauttaa null(tyhjä)
        return null;
    }

    ItemSlot GetEmptySlot()
    {
        //Tarkistaa
        for(int x = 0; x < slots.Length; x++)
        {
            //Palauta inventoriin tyhjä slotti
            if (slots[x].item == null)
            {
                return slots[x];
            }
        }

        //Jos ne eivät ole tyhjiä slottia palauttaa null(tyhjä)
        return null;
    }

    public void SelectItem(int index)
    {
        //Jos kadonneen sisällä ei ole itemi, älä tee mitään
        if (slots[index].item == null)
            return;

        //Aseta valitun slotin
        selectedItem = slots[index];

        //Aseta valitun slotin indeksin
        selectedItemIndex = index;

        //Aseta itemin nimi
        selectedItemName.text = selectedItem.item.displayName;

        //Aseta tuotteen kuvaus
        selectedItemDespcription.text = selectedItem.item.Description;

        //Aseta stat-arvot ja tilastojen nimet
        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        for(int x = 0; x < selectedItem.item.consumables.Length; x++)
        {
            //Hanki nykyisen arvon nimi muuntaa sitä string
            selectedItemStatName.text += selectedItem.item.consumables[x].type.ToString() + "\n";
            selectedItemStatValue.text += selectedItem.item.consumables[x].value.ToString() + "\n";

        }

        //Aseta käyttöpainike
        useButton.SetActive(selectedItem.item.type == Itemtype.Consumable);
        //Aseta varustepainike                                             //&& itemin ei ole tällä hetkellä varustettu
        equipButton.SetActive(selectedItem.item.type == Itemtype.Equipable && uiSlots[index].equipped);

        //Aseta unequip-painike                                            //&& itemi on tällä hetkellä varustettu
        unequipButton.SetActive(selectedItem.item.type == Itemtype.Equipable && uiSlots[index].equipped);

        //Aseta pudotuspainike
        dropButton.SetActive(true);
    }

    void ClearSelectedItemWindow()
    {
        //Tyhjennä tekstielementit
        selectedItem = null;
        selectedItemName.text = string.Empty;
        selectedItemDespcription.text = string.Empty;
        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        //Poista painikkeet käytöstä
        useButton.SetActive(false);
        equipButton.SetActive(false);
        unequipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    public void OnUseButton()
    {
        if(selectedItem.item.type == Itemtype.Consumable)
        {
            for(int x = 0; x < selectedItem.item.consumables.Length; x++)
            {
                switch (selectedItem.item.consumables[x].type)
                {
                    case ConsumableType.Health: needs.Heal(selectedItem.item.consumables[x].value); break;
                    case ConsumableType.Hunger: needs.Eat(selectedItem.item.consumables[x].value); break;
                    case ConsumableType.Thirst: needs.Drink(selectedItem.item.consumables[x].value); break;
                    case ConsumableType.Sleep: needs.Sleep(selectedItem.item.consumables[x].value); break;

                }
            }
        }

        RemoveSelectedItem();
    }

    public void OnEquipButton()
    {

    }

    //Esine slotti, jonka haluamme tyhjentää
    void UnEquip(int index)
    {

    }

    public void OnUnequipButton()
    {

    }

    public void OnDropButton()
    {
        ThrowItem(selectedItem.item);
        RemoveSelectedItem();
    }

    //Poista esineen inventorista
    void RemoveSelectedItem()
    {
        selectedItem.quantity--;
        //Jos meillä on 1 itemi jäljellä tai se on poistettu, meidän on tarkistettava
        if (selectedItem.quantity == 0)
        {
            //Jos esine on varustettu
            if (uiSlots[selectedItemIndex].equipped == true)
                //Poista se itemi
                UnEquip(selectedItemIndex);
            //Aseta itemi null(itemi ei ole enää saatavilla inventorissa
            selectedItem.item = null;
            ClearSelectedItemWindow();
        }
        //Päivitä määrä ja kuvake
        UpdateUI();
    }

    //Jos valmistamme tai rakennamme jotain, meidän on poistettava joukko esineitä
    public void RemoveItem(ItemDatabase item)
    {

    }

    public bool HasItems(ItemDatabase item, int quantity)
    {
        return false;
    }
}

public class ItemSlot
{
    public ItemDatabase item;
    public int quantity;
}

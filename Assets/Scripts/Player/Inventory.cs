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
            //Kaikki slotit on asetettu tyhj�ksi, joten meill� ei ole mit��n esinett�
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

    //Tarkista, voimmeko pinota kohteen, jos voimme pinota, ja etsi pino lis�t�ksesi sen siihen,
    //jos se ei voi lis�t� sit� tyhj��n paikkaan
    public void AddItem(ItemDatabase item)
    {
        //Tarkista, voimmeko pinota tuotteen
        if (item.canStackItem)
        {
            //Hanki slotti pinoamista varten ja l�het� sen p��lle, jonka haluamme pinota
            ItemSlot slotToStackTo = GetItemStack(item);

            //Jos l�yt�� jotain pinottavaa
            if (slotToStackTo != null)
            {
                //Lis�� yksi t�h�n m��r��n
                slotToStackTo.quantity++;
                UpdateUI();
                return;
            }
        }

        //Ei ole tyhj� slotti, etsi tyhj� slotti
        ItemSlot emptySlot = GetEmptySlot();
        //Jos l�yd�mme tyhj�n slotin
        if (emptySlot != null)
        {
            //Aseta item slottiin
            emptySlot.item = item;
            //Aseta m��r�ksi 1
            emptySlot.quantity = 1;
            UpdateUI();
            return;
        }

        //Jos meill� ei ole paikkaa vapaana, tuotetta ei voi pinota ja sitten vain heitt�� pois
        ThrowItem(item);
    }

    //Synnytt�� esineen
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
            //Sis�lt��k� t�m� slotti itemin
            if (slots[x].item != null)
            {
                //Aseta ui-slot, jos se on totta
                uiSlots[x].Set(slots[x]);
            }

            //Jos ei ole totta
            else
            {
                //Tyhjenn� UI slotin
                uiSlots[x].Clear();
            }
        }
    }

    //Etsi inventorin ja yrit� l�yt�� item slot, johon voimme pinota n�m� uudet items.
    ItemSlot GetItemStack(ItemDatabase item)
    {
        for(int x = 0; x < slots.Length; x++)
        {
            //Tarkista, onko paikan sis�lt�v� item se itemi, jonka haluamme lis�t�, eik� sen m��r� ylit� pinon enimm�ism��r��
            if (slots[x].item == item && slots[x].quantity < item.maxStackAmount)
            {
                //Palauta item slotti, johon pyydetty itemin voidaan pinota
                return slots[x];
            }
        }

        //Jos pinoa ei ole saatavilla palauttaa null(tyhj�)
        return null;
    }

    ItemSlot GetEmptySlot()
    {
        //Tarkistaa
        for(int x = 0; x < slots.Length; x++)
        {
            //Palauta inventoriin tyhj� slotti
            if (slots[x].item == null)
            {
                return slots[x];
            }
        }

        //Jos ne eiv�t ole tyhji� slottia palauttaa null(tyhj�)
        return null;
    }

    public void SelectItem(int index)
    {
        //Jos kadonneen sis�ll� ei ole itemi, �l� tee mit��n
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
            //Hanki nykyisen arvon nimi muuntaa sit� string
            selectedItemStatName.text += selectedItem.item.consumables[x].type.ToString() + "\n";
            selectedItemStatValue.text += selectedItem.item.consumables[x].value.ToString() + "\n";

        }

        //Aseta k�ytt�painike
        useButton.SetActive(selectedItem.item.type == Itemtype.Consumable);
        //Aseta varustepainike                                             //&& itemin ei ole t�ll� hetkell� varustettu
        equipButton.SetActive(selectedItem.item.type == Itemtype.Equipable && uiSlots[index].equipped);

        //Aseta unequip-painike                                            //&& itemi on t�ll� hetkell� varustettu
        unequipButton.SetActive(selectedItem.item.type == Itemtype.Equipable && uiSlots[index].equipped);

        //Aseta pudotuspainike
        dropButton.SetActive(true);
    }

    void ClearSelectedItemWindow()
    {
        //Tyhjenn� tekstielementit
        selectedItem = null;
        selectedItemName.text = string.Empty;
        selectedItemDespcription.text = string.Empty;
        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        //Poista painikkeet k�yt�st�
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

    //Esine slotti, jonka haluamme tyhjent��
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
        //Jos meill� on 1 itemi j�ljell� tai se on poistettu, meid�n on tarkistettava
        if (selectedItem.quantity == 0)
        {
            //Jos esine on varustettu
            if (uiSlots[selectedItemIndex].equipped == true)
                //Poista se itemi
                UnEquip(selectedItemIndex);
            //Aseta itemi null(itemi ei ole en�� saatavilla inventorissa
            selectedItem.item = null;
            ClearSelectedItemWindow();
        }
        //P�ivit� m��r� ja kuvake
        UpdateUI();
    }

    //Jos valmistamme tai rakennamme jotain, meid�n on poistettava joukko esineit�
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

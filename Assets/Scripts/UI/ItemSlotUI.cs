using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    public Button button;
    public Image icon;
    public TextMeshProUGUI quantityText;
    private ItemSlot currentSlot;
    private Outline outline;

    public int index;
    public bool equipped;

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void OnEnable()
    {
        outline.enabled = equipped;
    }

    public void Set(ItemSlot slot)
    {
        //Asettaa paikkaan
        currentSlot = slot;

        //Ota kuvake käyttöön
        icon.gameObject.SetActive(true);

        //Asettaa kohteen sprite
        icon.sprite = slot.item.icon;
        //Jos slot.quantity suurempi kuin 1, suorita slot.quantity.tostring(),
        //mutta jos se ei ole suurempi kuin 1, string on tyhjä.
        quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : string.Empty;

        //Jos outline on olemassa
        if (outline != null)
        {
            outline.enabled = equipped;
        }
    }

    //Funktio kun poistamme kohteen
    public void Clear()
    {
        currentSlot = null;

        //Disable the icon
        icon.gameObject.SetActive(false);

        //Aseta quantity teksti tyhjäksi meillä ei ole kohteita(items) paikan sisällä
        quantityText.text = string.Empty;
    }

    public void OnClickButton()
    {
        Inventory.instance.SelectItem(index);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemDatabase item;

    public string GetInteractPrompt()
    {
        return string.Format("Pickup {0}", item.displayName);
    }

    public void OnInteract()
    {
        //Lis?? itemin inventoriin vuorovaikutuksen j?lkeen
        Inventory.instance.AddItem(item);
        //Poista itemin
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Itemtype
{
    Recource,
    Equipable,
    Consumable
}

public enum ConsumableType
{
    Hunger,
    Thirst,
    Health,
    Sleep
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemDatabase : ScriptableObject
{
    [Header("Information")]
    public string displayName;
    public string Description;

    public Itemtype type;
    public Sprite icon;
    public GameObject dropPrefab;

    [Header("Stacking Items")]
    public bool canStackItem;
    public int maxStackAmount;

    [Header("Consumable")]
    public ItemDataConsumable[] consumables;

    [Header("Equip Items")]
    public GameObject equipPrefab;
}

[System.Serializable]
public class ItemDataConsumable
{
    public ConsumableType type;
    public float value;
}

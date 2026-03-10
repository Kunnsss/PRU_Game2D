using UnityEngine;

[System.Serializable]
public class ItemIngredient
{
    public ItemData item;
    public int amount;
}

[CreateAssetMenu(menuName = "NongTraiVang/Item")]
public class ItemData : ScriptableObject
{
    [Header("Thông Tin Cơ Bản")]
    public string itemName;
    public Sprite icon;

    [Header("Sản Xuất")]
    public float productionTime;
    public ItemIngredient[] ingredients;   // ← thay inputItems + inputAmount

    [Header("Kinh Tế")]
    public int sellPrice;
    public int unlockCost;
}
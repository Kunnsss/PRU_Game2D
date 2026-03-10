using UnityEngine;

[CreateAssetMenu(menuName = "NongTraiVang/Building")]
public class BuildingData : ScriptableObject
{
    [Header("Thông Tin")]
    public string buildingName;
    public Sprite sprite;
    public int unlockCost;      // 0 = có sẵn từ đầu
    public Color buildingColor = Color.white;

    [Header("Sản Xuất")]
    public ItemData outputItem; // item sẽ sản xuất ra
    public float productionTime;

    [Header("Prefab")]
    public GameObject prefab;
}
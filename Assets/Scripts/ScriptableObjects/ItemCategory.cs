using UnityEngine;

[CreateAssetMenu(fileName = "New Item Category", menuName = "Item Category")]
public class ItemCategory : ScriptableObject
{
    public string CategoryName;
    public ItemObject[] Items;
}

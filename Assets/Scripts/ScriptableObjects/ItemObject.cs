using UnityEngine;

[CreateAssetMenu(fileName = "New Item Object", menuName = "Item Object")]
public class ItemObject : ScriptableObject
{
    public string ItemName;
    public int Stock;
    public enum CostCategory
    {
        Low,
        Medium,
        High,
        Expensive,
        Priceless
    }
    public CostCategory ItemCost;
    public Sprite ItemImage;
    public Vector3 ImageOffset;
    public Vector3 WorldOffset;
}

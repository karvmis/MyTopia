using UnityEngine;

[CreateAssetMenu(fileName = "New item", menuName = "Scriptable Objects/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
}

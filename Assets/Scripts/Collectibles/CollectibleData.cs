using UnityEngine;

[CreateAssetMenu(fileName = "NewCollectible", menuName = "Collectibles/Collectible Data")]
public class CollectibleData : ScriptableObject
{
    public string collectibleName;
    public Sprite icon;

    [TextArea(3, 10)]
    public string storyText;
}
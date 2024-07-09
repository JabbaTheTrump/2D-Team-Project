using UnityEngine;

[CreateAssetMenu(menuName = "ItemData/Harvester")]
public class HarvesterItemData : ItemData
{
    public float HarvestTime = 3;
    public float NoiseDistance = 9;


    public AudioClip _drillingAudioClip;
    public AudioClip _finishAudioClip;
}

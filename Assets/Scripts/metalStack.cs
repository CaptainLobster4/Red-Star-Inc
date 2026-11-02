using System.Collections;
using UnityEngine;

public class MetalStack : MonoBehaviour
{
    [SerializeField] private metalLogic[] metalLayers;
    [SerializeField] private int currentLayerIndex = 0;
    [SerializeField] private Collider2D targetCollider;


    // 

    void Awake()
    {
        currentLayerIndex = 0;

        // Disables paper until all the metal layers are broken
        targetCollider.enabled = metalLayers.Length == 0;

        for (int i = 0; i < metalLayers.Length; i++)
        {
            
            metalLayers[i].GetComponent<Collider2D>().enabled = true;

            metalLayers[i].gameObject.SetActive(i == 0);
        }


    }
    public void MetalLayerBroken()
    {

        currentLayerIndex++;

        StartCoroutine(ActivateNextLayer());
    }

    private IEnumerator ActivateNextLayer()
    {
        yield return null;

        if (currentLayerIndex < metalLayers.Length)
        {
            metalLayers[currentLayerIndex].gameObject.SetActive(true);
        }
        else
        {
            targetCollider.enabled = true;
        }

    }

}

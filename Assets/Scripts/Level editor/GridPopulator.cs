using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class GridPopulator : MonoBehaviour
{
    public GameObject prefab; // This is our prefab object that will be exposed in the inspector
    public string[] assetPaths;
    public int numberToCreate; // number of objects to create. Exposed in inspector

    void Start()
    {
        Populate();
    }

    void Populate()
    {
        // For each folder
        for (int i=0; i<assetPaths.Length; i++)
        {
            Object[] currentResources = Resources.LoadAll(assetPaths[i]);

            for (int j=0; j<currentResources.Length; j++)
            {
                GameObject newObj; // Create GameObject instance

                // Create new instances of our prefab until we've created as many as we specified
                newObj = (GameObject)Instantiate(prefab, transform);
                newObj.GetComponent<Image>().sprite = Sprite.Create(
                    AssetPreview.GetAssetPreview(currentResources[j]), new Rect(0, 0, 100, 100), new Vector2(50, 50));
                newObj.GetComponent<ScrollableEntry>().SetPath(assetPaths[i] + "/" + currentResources[j].name, currentResources[j].name);
            }
            
        }
    }
}
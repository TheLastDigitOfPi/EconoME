using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomTerrainPlacer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Destroy(this);
    }


    [SerializeField] List<RandomTerrainObjectSpecs> objectsToPlace = new();
    [SerializeField] List<GameObject> allObjects = new List<GameObject>();

    [SerializeField] PolygonCollider2D polygonCollider;
    [SerializeField] Transform parent;

    [SerializeField] int numOfObjectsToPlace = 0;

    [SerializeField] float minScale = 1;
    [SerializeField] float maxScale = 1;
    [SerializeField] float minDistanceBetweenObjects = 1;


    [ContextMenu("Generate Terrain")]
    public void GenerateTerrain()
    {

        Debug.Log("In generate terrain");
        int placedObjects = 0;

        var minBounds = polygonCollider.bounds.min;
        var maxBounds = polygonCollider.bounds.max;
        int tries = 0;
        while (placedObjects < numOfObjectsToPlace)
        {
            if (tries > 10000) { Debug.LogWarning("Got to max while loop tries! Can any more objects fit?"); break; }
            tries++;

            float x = Random.Range(minBounds.x, maxBounds.x);
            float y = Random.Range(minBounds.y, maxBounds.y);

            //If not within collider, try again
            Vector3 location = new Vector3(x, y, 0);
            if (!polygonCollider.OverlapPoint(new Vector2(location.x, location.y)))
                continue;



            //If gameobject is there, try again

            float randomSize = Random.Range(minScale, maxScale);
            var foundColliders = Physics2D.OverlapBoxAll(location.ToVector2(), minDistanceBetweenObjects * Vector2.one, 0f);
            if (foundColliders.Length > 1)
            {
                Debug.Log("Found collider within area of " + foundColliders[1].gameObject.name);
                continue;
            }
            Physics2D.SyncTransforms();

            //otherwise place random object
            
            var randObject = objectsToPlace.RandomListItem();

            bool selectedObject = false;
            while (!selectedObject)
            {
                float chance = Random.Range(0, 100f);
                //If hit chance to spawn
                if(chance < randObject.chanceToSpawn)
                {
                    break;
                }
                randObject = objectsToPlace.RandomListItem();
            }


            GameObject placedObject = ((GameObject)PrefabUtility.InstantiatePrefab(randObject.objectToPlace));

            placedObject.transform.SetParent(parent);
            placedObject.transform.position = location;
            placedObject.transform.localScale = new Vector3(randomSize, randomSize, 1);
            allObjects.Add(placedObject);
            placedObjects++;


        }
    }

    [ContextMenu("Delete Terrain")]
    public void DeleteTerrain()
    {
        foreach (var item in allObjects)
        {
            DestroyImmediate(item);
        }
        allObjects.Clear();
    }

    [ContextMenu("Clean Up All Objects List")]
    public void CleanAllObjectList()
    {
        for (int i = 0; i < allObjects.Count; i++)
        {
            if(allObjects[i] == null)
            {
                allObjects.RemoveAt(i);
                i--;
            }
        }
    }

}

[System.Serializable]
public class RandomTerrainObjectSpecs
{
    public GameObject objectToPlace;
    [Range(0,100)] public float chanceToSpawn;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectReseter : MonoBehaviour
{
    public List<GameObject> objectsToReset;
    public List<GameObject> objectPrefabs;

    private List<Vector3> savedPosition = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (var VARIABLE in objectsToReset)
        {
            savedPosition.Add(VARIABLE.transform.position);
        }
    }

    // Update is called once per frame
    public void ResetObjects()
    {
        List<GameObject> addedItems = new List<GameObject>();
        for (int i = 0; i < objectsToReset.Count; i++)
        {
            Debug.Log("ouioui");
            Destroy(objectsToReset[i]);
            addedItems.Add(Instantiate(objectPrefabs[i],savedPosition[i],Quaternion.identity));
        }
        objectsToReset.Clear();
        objectsToReset.AddRange(addedItems);
    }
}

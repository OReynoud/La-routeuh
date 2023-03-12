using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableHolder : MonoBehaviour
{
    public LineRenderer lineRenderer;

    public List<GameObject> segments;

    public GameObject ropeStartAnchor;

    public GameObject ropeEndAnchor;

    public float segmentLength;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer.positionCount = 0;
        for (int i = 0; i < segments.Count; i++)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(i,segments[i].transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            lineRenderer.SetPosition(i,segments[i].transform.position);
        }
    }
}

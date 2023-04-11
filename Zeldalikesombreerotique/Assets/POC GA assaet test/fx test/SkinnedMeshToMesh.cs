using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SkinnedMeshToMesh : MonoBehaviour
{
    public SkinnedMeshRenderer Beta_Surface;
    public VisualEffect VFXGraph;
    public float refreshRate;
    private Mesh m;
    private Mesh m2;
    void Start()
    {
        m = new Mesh();
        m2 = new Mesh();
        StartCoroutine(UpdateVFXGraph());
    }

    IEnumerator UpdateVFXGraph()
    {
        while (gameObject.activeSelf)
        {
            Beta_Surface.BakeMesh(m);
            
            Vector3[] vertices = m.vertices;
            m2.vertices = vertices;
            
            VFXGraph.SetMesh("Mesh", m2);
            
            yield return new WaitForSeconds (refreshRate);
        }
    }
}

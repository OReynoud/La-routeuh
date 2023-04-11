using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class SkinnedMeshToMesh : MonoBehaviour
{
    public SkinnedMeshRenderer Beta_Surface;
    public VisualEffect VFXGraph;
    public float refreshRate;
    private float timer = 0;
    private Mesh m;
    private Mesh m2;
    private List<Vector3> vertices = new List<Vector3>();
    void Start()
    {
        m = new Mesh();
        m2 = new Mesh();
        //StartCoroutine(UpdateVFXGraph());
    }

   private void FixedUpdate()
    {
        if (timer < refreshRate)
        {
            timer += Time.deltaTime;
        }
        else
        {
            
            Beta_Surface.BakeMesh(m);
            /*vertices.Clear();
            vertices.AddRange(m.vertices);
            m2.vertices.AddRange(vertices);*/
            Vector3[] vertices = m.vertices;
            m2.vertices = vertices;
            
            VFXGraph.SetMesh("Mesh", m2);
            timer = 0;
        }
    }

    /*IEnumerator UpdateVFXGraph()
    {
        while (gameObject.activeSelf)
        {
            Beta_Surface.BakeMesh(m);
            
            Vector3[] vertices = m.vertices;
            m2.vertices = vertices;
            
            VFXGraph.SetMesh("Mesh", m2);
            
            yield return new WaitForSeconds (refreshRate);
        }
    }*/
}

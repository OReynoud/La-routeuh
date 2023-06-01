using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parkour : MonoBehaviour
{
    public List<Transform> startPoint;
    public List<Transform> controlPoint;
    public List<Transform> endPoint;

    public float speed = 0.01f;

    private float timer = 0;

    private bool triggerCinematic = true;

    public GameObject laPetiteFille;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (triggerCinematic && startPoint.Count != 0)
        {
            if (timer < 1)
            {
                timer += speed * Time.deltaTime;
                
                Vector3 m1 = Vector3.Lerp( startPoint[0].position, controlPoint[0].position, timer );
                Vector3 m2 = Vector3.Lerp( controlPoint[0].position, endPoint[0].position, timer );

                laPetiteFille.transform.position = Vector3.Lerp(m1, m2, timer);
            }
            else
            {
                startPoint.RemoveAt(0);
                controlPoint.RemoveAt(0);
                endPoint.RemoveAt(0);
                timer = 0;
            }
        }
    }
}

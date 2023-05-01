using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Parkour : MonoBehaviour
{
    public List<Transform> startPoint;
    public List<Transform> controlPoint;
    public List<Transform> endPoint;

    [SerializeField] private float speed;

    private float _timer;

    private bool _triggerCinematic;

    [FormerlySerializedAs("laPetiteFille")] [SerializeField] private GameObject objectToMove;

    // Update is called once per frame
    void Update()
    {
        if (_triggerCinematic && startPoint.Count != 0)
        {
            if (_timer < 1)
            {
                _timer += speed * Time.deltaTime;
                
                Vector3 m1 = Vector3.Lerp( startPoint[0].position, controlPoint[0].position, _timer );
                Vector3 m2 = Vector3.Lerp( controlPoint[0].position, endPoint[0].position, _timer );

                objectToMove.transform.position = Vector3.Lerp(m1, m2, _timer);
            }
            else
            {
                startPoint.RemoveAt(0);
                controlPoint.RemoveAt(0);
                endPoint.RemoveAt(0);
                _timer = 0;
            }
        }
    }
}

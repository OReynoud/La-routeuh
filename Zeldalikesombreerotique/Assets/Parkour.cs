using System.Collections.Generic;
using UnityEngine;

public class Parkour : MonoBehaviour
{
    [SerializeField] private List<Transform> startPoint;
    [SerializeField] private List<Transform> controlPoint;
    [SerializeField] private List<Transform> endPoint;

    [SerializeField] private float speed;

    private float _timer;

    private bool _triggerCinematic;

    [SerializeField] private GameObject objectToMove;

    private void Update()
    {
        if (_triggerCinematic && startPoint.Count != 0)
        {
            if (_timer < 1)
            {
                _timer += speed * Time.deltaTime;
                
                var m1 = Vector3.Lerp( startPoint[0].position, controlPoint[0].position, _timer );
                var m2 = Vector3.Lerp( controlPoint[0].position, endPoint[0].position, _timer );

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

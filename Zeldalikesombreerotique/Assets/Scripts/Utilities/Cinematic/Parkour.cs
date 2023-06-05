using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Parkour : MonoBehaviour
{

    [SerializeField] private List<Transform> startPoint;
    [SerializeField] private List<Transform> controlPoint;
    [SerializeField] private List<Transform> endPoint;
    
    [SerializeField] private float speed;

    private float _timer;

    private bool _triggerCinematic;

    public GameObject objectToMove;
    private Animator petiteFilleRig;

    public bool isSinging = true;

    public AudioSource song;
    public GameObject bush;
    public GameObject audiobreathe;

    private void Awake()
    {
        petiteFilleRig = GetComponentInChildren<Animator>();
    }
    public void OnDrawGizmos()
    {
        for (int j = 0; j < startPoint.Count; j++)
        {
            for (float i = 0; i < 1; i += 0.05f)
            {
                var m1 = Vector3.Lerp( startPoint[j].position, controlPoint[j].position, i );
                var m2 = Vector3.Lerp( controlPoint[j].position, endPoint[j].position, i );
                Gizmos.DrawSphere(Vector3.Lerp(m1, m2, i),0.1f);
                Gizmos.DrawLine(startPoint[j].position,controlPoint[j].position);
                Gizmos.DrawLine(endPoint[j].position,controlPoint[j].position);
            }
        }
    }
    private void FixedUpdate()
    {
        if (_triggerCinematic && startPoint.Count != 0)
        {
            if (_timer < 1)
            {
                petiteFilleRig.SetBool("isRunning",true);
                _timer += (speed/ Vector3.Distance(startPoint[0].position,endPoint[0].position)) * Time.fixedDeltaTime;
                
                var m1 = Vector3.Lerp( startPoint[0].position, controlPoint[0].position, _timer );
                var m2 = Vector3.Lerp( controlPoint[0].position, endPoint[0].position, _timer );
                m1 = new Vector3(m1.x, -0.1f, m1.z);
                m2 = new Vector3(m2.x, -0.1f, m2.z);
                var dir = m2 - m1;
                var dirNormed = dir.normalized;
                var angle = Mathf.Atan2(dirNormed.x, dirNormed.z) * Mathf.Rad2Deg;
                objectToMove.transform.rotation = Quaternion.AngleAxis(angle,Vector3.up);

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
        else if (_triggerCinematic && startPoint.Count == 0)
        {
            objectToMove.SetActive(false);
        }
    }

    public void TriggerGirl(bool isStandingAtStart = false)
    {
        if (bush != null)
        {
            bush.SetActive(true);
        }
        
        if (song != null && isSinging == true)
        {
            song.DOFade(0, 0.15f);

        }
        
        if (audiobreathe != null && isSinging == true)
        {
            audiobreathe.SetActive(true);

        }
        //breathe.DOFade(1, 0.1f);
        //audiosong.SetActive(false);


        _triggerCinematic = true;
        if (!isStandingAtStart)
        {
            objectToMove.transform.position = startPoint[0].position;
            objectToMove.SetActive(true);
            
            petiteFilleRig = GetComponentInChildren<Animator>();
        }
    }

    /*private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            TriggerGirl(true);
        }
    }*/
}

using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class CableHolder : MonoBehaviour
{
    [Tooltip("L'object manipulable par le joueur rélié à la corde")]public Rigidbody ropeUser;
    [Tooltip("Le point fixe rélié à la corde")]public Transform anchor;

    [BoxGroup("Autres")]public LineRenderer rope;
    [Tooltip("Quels layers sont utilisés pour détecter la colision des cordes (plusieurs possibles)?")]public LayerMask collMask;

    [Tooltip("Taille maximale du cable")]
    public float maxCableLength;

    [Tooltip("Taille actuelle du cable")][ReadOnly]public float currentCableLength;
    [BoxGroup("Autres")]public float minCollisionDistance;
    [FormerlySerializedAs("debugAngle1")] [BoxGroup("Autres")] public float debugAngle;

    [Tooltip("Force de rétraction quand le cable atteint sa longueure maximale")]
    public float retractionForce;

    public List<Vector3> ropePositions { get;} = new List<Vector3>();

    private void Awake()
    { 
        AddPosToRope(anchor.position);
    }

    private void FixedUpdate()
    {
        UpdateRopePositions();
        LastSegmentGoToPlayerPos();
        for (int i = 0; i < rope.positionCount-1; i++)
        {
            currentCableLength += Vector3.Distance(rope.GetPosition(i), rope.GetPosition(i + 1));
        }

        if (currentCableLength > maxCableLength)
        {
            var dir = ropePositions[^2] - ropeUser.position;
            PlayerController.instance.rb.AddForce(retractionForce * dir.normalized);
            ropeUser.AddForce(retractionForce * dir.normalized);
        }

        DetectCollisionEnter();
        if (ropePositions.Count > 2) DetectCollisionExits();        
    }

    private void DetectCollisionEnter()
    {
        if (Physics.Linecast(ropeUser.position, rope.GetPosition(ropePositions.Count - 2), out var hit, collMask))
        {
            // Check for duplicated collision (two collisions at the same place).
            if (System.Math.Abs(Vector3.Distance(rope.GetPosition(ropePositions.Count - 2), hit.point)) > minCollisionDistance) {
                ropePositions.RemoveAt(ropePositions.Count - 1); 
                AddPosToRope(hit.point);
            }
        }
    }

    private void DetectCollisionExits()
    {
        RaycastHit hit;
        var dir = ropePositions[^3] - ropeUser.position;
        var dirNormed = dir.normalized;
        var leftSafety = Quaternion.AngleAxis(Mathf.Atan2(dirNormed.x,dirNormed.z)*Mathf.Rad2Deg + debugAngle,Vector3.up) * Vector3.forward;
        var rightSafety = Quaternion.AngleAxis(Mathf.Atan2(dirNormed.x,dirNormed.z)*Mathf.Rad2Deg - debugAngle,Vector3.up) * Vector3.forward;
        if (Physics.Linecast(ropeUser.position, rope.GetPosition(ropePositions.Count - 3), out hit, collMask))
        {
            return;
        }

        if (ropePositions.Count != 3)
        {
            ropePositions.RemoveAt(ropePositions.Count - 2);
            return;
        }

        if (Physics.Raycast(ropeUser.position,leftSafety,Vector3.Distance(ropeUser.position,ropePositions[^3]),collMask))
        {
            return;
        }
        if (Physics.Raycast(ropeUser.position,rightSafety,Vector3.Distance(ropeUser.position,ropePositions[^3]),collMask))
        {
            return;
        }
        ropePositions.RemoveAt(ropePositions.Count - 2);
    }

    private void AddPosToRope(Vector3 _pos)
    {
        ropePositions.Add(_pos);
        ropePositions.Add(ropeUser.position); //Always the last pos must be the player
    }

    private void UpdateRopePositions()
    {
        currentCableLength = 0;
        rope.positionCount = ropePositions.Count;
        rope.SetPositions(ropePositions.ToArray());

    }

    private void LastSegmentGoToPlayerPos() => rope.SetPosition(rope.positionCount - 1, ropeUser.position);
}




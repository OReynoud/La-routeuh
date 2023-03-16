using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CableHolder : MonoBehaviour
{
    [Tooltip("L'object manipulable par le joueur rélié à la corde")]public Transform ropeUser;
    [Tooltip("Le point fixe rélié à la corde")]public Transform anchor;

    [BoxGroup("Autres")]public LineRenderer rope;
    [Tooltip("Quels layers sont utilisés pour détecter la colision des cordes (plusieurs possibles)?")]public LayerMask collMask;
    [BoxGroup("Autres")]public float minCollisionDistance;

    public List<Vector3> ropePositions { get;} = new List<Vector3>();

    private void Awake()
    { 
        AddPosToRope(anchor.position);
    }

    private void FixedUpdate()
    {
        UpdateRopePositions();
        LastSegmentGoToPlayerPos();

        DetectCollisionEnter();
        if (ropePositions.Count > 2) DetectCollisionExits();        
    }

    private void DetectCollisionEnter()
    {
        RaycastHit hit;
        if (Physics.Linecast(ropeUser.position, rope.GetPosition(ropePositions.Count - 2), out hit, collMask))
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
        if (!Physics.Linecast(ropeUser.position, rope.GetPosition(ropePositions.Count - 3), out hit, collMask))
        {
            ropePositions.RemoveAt(ropePositions.Count - 2);
        }
    }

    private void AddPosToRope(Vector3 _pos)
    {
        ropePositions.Add(_pos);
        ropePositions.Add(ropeUser.position); //Always the last pos must be the player
    }

    private void UpdateRopePositions()
    {
        rope.positionCount = ropePositions.Count;
        rope.SetPositions(ropePositions.ToArray());
    }

    private void LastSegmentGoToPlayerPos() => rope.SetPosition(rope.positionCount - 1, ropeUser.position);
}




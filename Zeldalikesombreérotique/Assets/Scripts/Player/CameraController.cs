using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraTarget;
    public float smoothMove;
    public bool stopMove;
    public Vector3 offset;

    public Vector2 oui;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position != cameraTarget.position && !stopMove)
        {
            Vector3 targetPos = new Vector3(cameraTarget.position.x, transform.position.y, cameraTarget.position.z) + offset;
            transform.position = Vector3.Lerp(transform.position,targetPos,smoothMove);
        }

        if (transform.position == cameraTarget.position)
        {
            StopAllCoroutines();
        }
    }

    public IEnumerator TansitionCamera(GameObject targetPos)
    {
        //Vector2 oui = Vector2.zero;
        cameraTarget = targetPos.transform;
        stopMove = true;
        float timeToGo = 0;
        while (timeToGo < smoothMove)
        {

            GetComponent<Camera>().orthographicSize = Mathf.Lerp(7.75f, 8, timeToGo / smoothMove);
            oui = new Vector2(
                Mathf.Lerp(transform.position.x, cameraTarget.transform.position.x, timeToGo / smoothMove),
                Mathf.Lerp(transform.position.y, cameraTarget.transform.position.y, timeToGo / smoothMove));
            transform.position = new Vector3(oui.x, oui.y, -10);
            timeToGo += Time.deltaTime;
            yield return null;
        }

        stopMove = false;
        transform.position = new Vector3(oui.x, oui.y, -10);
    }

    public IEnumerator BackTansitionCamera(GameObject targetPos)
    {
        cameraTarget = targetPos.transform;
        stopMove = true;
        float timeToGo = 0;
        while (timeToGo < smoothMove)
        {
            GetComponent<Camera>().orthographicSize = Mathf.Lerp(8, 7.75f, timeToGo / smoothMove);
            oui = new Vector2(
                Mathf.Lerp(transform.position.x, cameraTarget.transform.position.x, timeToGo / smoothMove),
                Mathf.Lerp(transform.position.y, cameraTarget.transform.position.y, timeToGo / smoothMove));
            transform.position = new Vector3(oui.x, oui.y, -10);
            timeToGo += Time.deltaTime;
            yield return null;
        }

        stopMove = false;
        transform.position = new Vector3(oui.x, oui.y, -10);
    }
}
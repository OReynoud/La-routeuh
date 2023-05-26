using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsScript : MonoBehaviour, ISubmitHandler
{
    // Start is called before the first frame update
    public void OnSubmit(BaseEventData eventData)
    {
        eventData.selectedObject = transform.parent.gameObject;
    }
}

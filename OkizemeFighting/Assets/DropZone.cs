using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

    public bool occuped = false;

    public void OnPointerEnter(PointerEventData eventdate)
    {
        if (eventdate.pointerDrag == null)
            return;
        Draggable d = eventdate.pointerDrag.GetComponent<Draggable>();
        if (d != null)
        {
            Debug.Log("Entré dans la zone");
            d.placeholderParent = this.transform;
        }
    }

    public void OnPointerExit(PointerEventData eventdate)
    {
        if (eventdate.pointerDrag == null)
            return;
        Draggable d = eventdate.pointerDrag.GetComponent<Draggable>();
        if (d != null && d.placeholderParent==this.transform)
        {
            Debug.Log("Sortie de la zone");
            d.placeholderParent = d.parentToReturnTo;
            occuped = false;
        }
    }

    public void OnDrop(PointerEventData eventdata)
    {
        Debug.Log(eventdata.pointerDrag.name + "was dropped on " + gameObject.name);
        Draggable d = eventdata.pointerDrag.GetComponent<Draggable>();
        if (d != null && !occuped)
        {
            Debug.Log("Posé dans la zone");
            d.parentToReturnTo = this.transform;
            occuped = true;
        }

    }
}

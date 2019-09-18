using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
    
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public Transform parentToReturnTo = null;
    public Transform placeholderParent = null;

    public GameObject placeholder = null;

    public void OnBeginDrag(PointerEventData eventdata)
    {
       // Debug.Log("OnBeginDrag");

        placeholder = new GameObject();
        placeholder.transform.SetParent(this.transform.parent);
        LayoutElement le = placeholder.AddComponent<LayoutElement>();
        le.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
        le.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
        le.flexibleWidth = 0;
        le.flexibleHeight = 0;

        placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());


        parentToReturnTo = this.transform.parent;
        placeholderParent = parentToReturnTo;
        this.transform.SetParent(this.transform.parent.parent);

        GetComponent<CanvasGroup>().blocksRaycasts = false;

    }

    public void OnDrag(PointerEventData eventdata)
    {
      //  Debug.Log("OnDrag");

        this.transform.position = eventdata.position;

        int newSiblingIndex = placeholderParent.childCount;

        for(int i = 0; i < placeholderParent.childCount; i++)
        {
            if(this.transform.position.x < placeholderParent.GetChild(i).position.x)
            {
                newSiblingIndex = i;
                if (placeholderParent.transform.GetSiblingIndex() < newSiblingIndex)
                    newSiblingIndex--;
                break;
            }
        }

        placeholder.transform.SetSiblingIndex(newSiblingIndex); 
    }

    public void OnEndDrag(PointerEventData eventdata)
    {
       // Debug.Log("OnEndDrag");
        this.transform.SetParent(parentToReturnTo);
        this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        Destroy(placeholder);
    }
}

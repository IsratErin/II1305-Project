using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SquareCity : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{


    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.ApplyIncorrectMaterial();        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.ApplyOriginalMaterial();
    }

    

}

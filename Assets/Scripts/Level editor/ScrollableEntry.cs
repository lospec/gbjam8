using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollableEntry : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private string resourcePath;
    private GameObject resource;
    private GameObject rect;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Creo la preview, un rettangolo semitrasparente sul layer canvas (così posso spostare gli oggetti
        // se ho sbagliato a collocarli)
        resource = 

        // Associo la preview al pennello
        LevelEditor.Instance.isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // La preview si muove con il pennello
        Debug.Log("Trascino");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Istanzio l'oggetto
        Debug.Log("Smetto di trascinare");
        LevelEditor.Instance.isDragging = false;
    }

    public void SetPath(string path)
    {
        resourcePath = path;
    }
}

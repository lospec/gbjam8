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
    private LevelEditor editor;

    private void Start()
    {
        editor = LevelEditor.Instance;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Creo la preview, un rettangolo semitrasparente sul layer canvas (così posso spostare gli oggetti
        // se ho sbagliato a collocarli)
        resource = (GameObject)Resources.Load(resourcePath);
        resource = Instantiate(resource, editor.transform);

        MonoBehaviour[] scripts = resource.GetComponentsInChildren<MonoBehaviour>();
        for (int i=0; i<scripts.Length; i++)
        {
            scripts[i].enabled = false;
        }

        editor.SetBrushPreview(resource);

        // Associo la preview al pennello
        editor.isDragging = true;
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

        editor.AddToAssetList(resource);

        editor.ResetBrushPreview();
        editor.isDragging = false;
    }

    public void SetPath(string path, string name)
    {
        GetComponentInChildren<Text>().text = name;
        resourcePath = path;
    }
}

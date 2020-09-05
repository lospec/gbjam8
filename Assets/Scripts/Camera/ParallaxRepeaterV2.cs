using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

[System.Serializable]
public class GameObjectMatrix
{
    public GameObject[] row;
}
public class ParallaxRepeaterV2 : MonoBehaviour {
    public bool repeatY;
    public float tileHeight;
    public float tileWidth;
    public float horizontalViewZone;
    public float verticalViewZone;
    public GameObjectMatrix[] verticalMatrix;
    public bool optimizeHorizontal = false;

    private Transform cameraTransform;
    private Transform middleTransform;
    private List<Transform> children;
    private int left;
    private int middle;
    private int right;
    private int up;
    private int down;

	// Use this for initialization
	void Start ()
    {
        int nChildren = transform.childCount;
        children = new List<Transform>();

        left = 0;
        middle = 1;
        right = 2;
        up = 0;
        down = 2;

        for (int i=0; i<nChildren; i++)
        {
            children.Add(transform.GetChild(i));
        }

        cameraTransform = GameManager.instance.playerObject.transform;
        middleTransform = children[middle];
	}
	
	// Update is called once per frame
	void Update () {
        float cameraX = cameraTransform.position.x;
        float middleX = middleTransform.position.x;

        if (repeatY)
        {
            /* Rimuovere da qui in poi per far funzionare la parallasse orizzontale */
            if (cameraTransform.position.y >= (verticalMatrix[middle].row[0].transform.position.y + verticalViewZone))
            {
                scrollUp();
            }

            if (cameraTransform.position.y <= (verticalMatrix[middle].row[0].transform.position.y - verticalViewZone))
            {
                scrollDown();
            }
        }
    }

    private void scrollUp()
    {
        GameObject[] toMove = verticalMatrix[down].row;
        
        for (int i=0; i<toMove.Length; i++)
        {
            toMove[i].transform.position = new Vector3(
                toMove[i].transform.position.x,
                toMove[i].transform.position.y + 3 * tileHeight,
                toMove[i].transform.position.z
            );
        }

        GameObject[] tmp = toMove;

        verticalMatrix[down].row = verticalMatrix[middle].row;
        verticalMatrix[middle].row = verticalMatrix[up].row;
        verticalMatrix[up].row = toMove;
    }

    private void scrollDown()
    {
        GameObject[] toMove = verticalMatrix[up].row;

        for (int i = 0; i < toMove.Length; i++)
        {
            toMove[i].transform.position = new Vector3(
                toMove[i].transform.position.x,
                toMove[i].transform.position.y - 3 * tileHeight,
                toMove[i].transform.position.z
            );
        }

        GameObject[] tmp = toMove;

        verticalMatrix[up].row = verticalMatrix[middle].row;
        verticalMatrix[middle].row = verticalMatrix[down].row;
        verticalMatrix[down].row = toMove;
    }
}
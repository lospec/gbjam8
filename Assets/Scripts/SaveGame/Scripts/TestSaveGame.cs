using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSaveGame : MonoBehaviour
{
    //INT (UI)
    [Header("Save int")]
    public Text countIntText;
    public InputField inputIntField;
    public int cubeIntCount = 0;
    [Space(10)]

    //Next variables
    [Header("Save next")]
    public float floatCount;
    public Vector2 vect2;
    public Vector3 vect3;
    public Color color;
    public string stringSave;
    public bool saveBool;




    // Use this for initialization
    private void Start()
    {

        //Load Save int
        cubeIntCount = SaveSystem.Instance.GetInt("cubeCount");
        countIntText.text = cubeIntCount.ToString();

        //Load save Next
        floatCount = SaveSystem.Instance.GetFloat("float");
        saveBool = SaveSystem.Instance.GetBool("bool");
        vect2 = SaveSystem.Instance.GetVector2("vect2");
        vect3 = SaveSystem.Instance.GetVector3("vect3");
        color = SaveSystem.Instance.GetColor("color");
        stringSave = SaveSystem.Instance.GetString("string");



    }


    //Button Save INT
    public void SaveCube()
    {
        countIntText.text = inputIntField.text;
        cubeIntCount = int.Parse(inputIntField.text);

        //Save "cubeCount"
        SaveSystem.Instance.SetInt("cubeCount", cubeIntCount);
    }

    //Save "NEXT"
    private void OnApplicationQuit()
    {
        
       SaveSystem.Instance.SetFloat("float", floatCount);
        SaveSystem.Instance.SetBool("bool", saveBool);
        SaveSystem.Instance.SetVector2("vect2", vect2);
        SaveSystem.Instance.SetVector3("vect3", vect3);
        SaveSystem.Instance.SetColor("color", color);
        SaveSystem.Instance.SetString("string", stringSave);
    }

    //Save "NEXT"
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveSystem.Instance.SetFloat("float", floatCount);
            SaveSystem.Instance.SetBool("bool", saveBool);
            SaveSystem.Instance.SetVector2("vect2", vect2);
            SaveSystem.Instance.SetVector3("vect3", vect3);
            SaveSystem.Instance.SetColor("color", color);
            SaveSystem.Instance.SetString("string", stringSave);
        }
    }

}

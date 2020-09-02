using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrequentlyAccessed : MonoBehaviour
{
    public static FrequentlyAccessed Instance;


    public GameObject playerObject;
    public CharacterController playerController;
    public UnityEngine.Camera camera;

    private void Awake()
    {
        Instance = this;

        playerController = playerObject.GetComponent<CharacterController>();
    }
}

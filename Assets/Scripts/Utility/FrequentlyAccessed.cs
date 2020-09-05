using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class FrequentlyAccessed : MonoBehaviour
{
    public static FrequentlyAccessed Instance;


    public GameObject playerObject;
    public PlayerController playerController;

    private void Awake()
    {
        Instance = this;

        playerController = playerObject.GetComponent<PlayerController>();
    }
}

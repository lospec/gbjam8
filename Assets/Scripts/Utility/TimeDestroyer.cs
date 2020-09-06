using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDestroyer : MonoBehaviour
{
    public float time;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitAndDestroy());
    }

    private IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(time);

        Destroy(this.gameObject);
    }
}

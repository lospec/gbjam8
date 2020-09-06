using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;
    public float lightValue;
    public float darkValue;
    public GameObject quad;
    public float transitionSpeed;

    private Material material;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Debug.Log("Chiamato");

        material = quad.GetComponent<MeshRenderer>().sharedMaterial;
        material.SetFloat("_Fade", darkValue);
        StartCoroutine(SingleTransition(true));
    }

    public IEnumerator DoubleTransition()
    {
        StartCoroutine(SingleTransition(false));

        yield return new WaitForSeconds(1.2f);

        StartCoroutine(SingleTransition(true));
    }

    public IEnumerator SingleTransition(bool toLight)
    {
        if (toLight)
        {
            StartCoroutine(FadeValue(darkValue, lightValue));
        }
        else
        {
            StartCoroutine(FadeValue(lightValue, darkValue));
        }

        yield return new WaitForSeconds(0.2f);
    }
    private IEnumerator FadeValue(float start, float end)
    {
        float t = 0;

        while (material.GetFloat("_Fade") != end)
        {
            material.SetFloat("_Fade", Mathf.Lerp(start, end, t));
            t += Time.deltaTime * transitionSpeed;

            yield return null;
        }
    }
}

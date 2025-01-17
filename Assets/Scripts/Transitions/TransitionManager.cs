﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;
    public float lightValue;
    public float darkValue;
    public Material[] materials;
    public float transitionSpeed;
    private static readonly int Fade = Shader.PropertyToID("_Fade");

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetMaterialsValue(darkValue);

        SingleTransition(true);
    }

    public IEnumerator DoubleTransition()
    {
        SingleTransition(false);

        yield return new WaitForSeconds(1.2f);

        SingleTransition(true);
    }

    public void SingleTransition(bool toLight)
    {
        if (toLight)
        {
            StartCoroutine(FadeValue(darkValue, lightValue));
        }
        else
        {
            StartCoroutine(FadeValue(lightValue, darkValue));
        }
    }

    private IEnumerator FadeValue(float start, float end)
    {
        float t = 0;

        while (Math.Abs(materials[0].GetFloat(Fade) - end) > 0.01f)
        {
            SetMaterialsValue(Mathf.Lerp(start, end, t));
            t += Time.deltaTime * transitionSpeed;
            yield return null;
        }
    }

    private void SetMaterialsValue(float value)
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetFloat("_Fade", value);
        }
    }
}

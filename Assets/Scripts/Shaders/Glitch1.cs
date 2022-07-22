using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glitch1 : MonoBehaviour
{
    public Material mat;

    private float[] ar = new float[12];

    private void Update()
    {
        if (Random.value < 0.1)
            for (int i = 0; i < ar.Length; i++)
                ar[i] = Random.value;
        mat.SetInt("_RectangleLength", 12);
        mat.SetFloatArray("_Rectangles", ar);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, mat);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleShader : MonoBehaviour
{
    public Material circleShaderMat;
    public bool trigger;
    public float thickness = 0.05f;
    public float speed = 0.01f;

    private float radius;

    private void FixedUpdate()
    {
        if (trigger)
        {
            trigger = false;
            radius = 0;
        }
        radius += speed;

        circleShaderMat.SetFloat("_InnerRadius", radius);
        circleShaderMat.SetFloat("_OuterRadius", radius + thickness);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, circleShaderMat);
    }
}

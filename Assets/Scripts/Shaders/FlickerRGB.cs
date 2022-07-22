using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerRGB : MonoBehaviour
{
    public Material RGBSeparate;
    [Range(0, 1)]
    public float frequency = 0.1f;
    [Range(-1, 1)]
    public float maxIntensityX = 0.1f;
    [Range(-1, 1)]
    public float maxIntensityY = 0.1f;

    public int duration = 2;
    public int cooldown = 10;

    int trigger;

    private void Update()
    {
        if (trigger > -cooldown)
            trigger--;
        else if (Random.value < frequency)
            trigger = duration;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (trigger > 0)
        {
            RGBSeparate.SetVector("_Dist", new Vector4(Random.value * maxIntensityX, Random.value * maxIntensityY, 0, 0));
            Graphics.Blit(src, dest, RGBSeparate);
        }
        else
            Graphics.Blit(src, dest);
    }
}

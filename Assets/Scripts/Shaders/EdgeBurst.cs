using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeBurst : MonoBehaviour
{
    public Material edgeDetection2;

    public int maxTicks = 100;
    public bool trigger;

    private int counter;
    private bool isDirRight = false;

    private void FixedUpdate()
    {
        if (trigger)
        {
            trigger = false;
            counter = -1;
            isDirRight = !isDirRight;
        }
        if (counter < maxTicks)
            counter++;
        else
            counter = maxTicks;

        float t1 = (float)counter / maxTicks;
        float t2 = ActivationPos(t1);
        float t3 = ActivationCol(t1);
        edgeDetection2.SetVector("_Offset", new Vector4(t2 * (isDirRight ? 1 : -1) / 2, -t2 / 3) / 2);
        edgeDetection2.SetColor("_OpacityMask", new Color(1 - t3, 1 - t3, 1 - t3, 1));
    }

    //some kind of activation function
    private float ActivationPos(float x)
    {
        return Mathf.Pow(x, 0.5f);
    }
    private float ActivationCol(float x)
    {
        return x;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, edgeDetection2);
    }
}

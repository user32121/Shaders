using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mandelbrot : MonoBehaviour
{
    public GameObject renderTargetPlane;

    public ComputeShader shader;
    public string shaderKernelName;
    public string shaderTextureName;
    public string shaderBufferName;
    public Vector2 viewMin;
    public Vector2 viewMax;

    int kernelId;
    RenderTexture tex;
    ComputeBuffer buf;


    int frames;
    bool isScrolling;
    float scaleFactor = 1;
    Vector2 prevPos;

    int scrollCooldown = 0;

    private void Start()
    {
        kernelId = shader.FindKernel(shaderKernelName);
        tex = new RenderTexture(256, 256, 1) { enableRandomWrite = true };
        tex.Create();
        buf = new ComputeBuffer(tex.width * tex.height * tex.depth, sizeof(float) * 2);

        renderTargetPlane.GetComponent<Renderer>().material.mainTexture = tex;
        renderTargetPlane.transform.localScale = new Vector3(Screen.width / 100, 1, Screen.height / 100);

        shader.SetTexture(kernelId, shaderTextureName, tex);
        shader.SetBuffer(kernelId, shaderBufferName, buf);
        shader.SetFloat("width", Screen.width);
        shader.SetFloat("height", Screen.height);
        shader.SetFloat("xMin", viewMin.x);
        shader.SetFloat("yMin", viewMin.y);
        shader.SetFloat("xMax", viewMax.x);
        shader.SetFloat("yMax", viewMax.y);
        shader.SetBool("recompute", true);
        frames = 0;
    }

    private void FixedUpdate()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            isScrolling = true;
            scrollCooldown = 5;

            Vector2 relativeCursorPos = Input.mousePosition / new Vector2(Screen.width, Screen.height);
            Vector2 pos = viewMin + relativeCursorPos * (viewMax - viewMin);
            float factor = Mathf.Pow(0.5f, scroll);
            Vector2 distMin = pos - viewMin;
            viewMin = pos - distMin * factor;
            Vector2 distMax = viewMax - pos;
            viewMax = pos + distMax * factor;

            Vector2 curPos = (viewMin + viewMax) / 2;
            if (scaleFactor == 1)
                prevPos = curPos;
            scaleFactor *= factor;
            renderTargetPlane.transform.position = (prevPos - curPos) / (viewMax - viewMin) * 20;
            renderTargetPlane.transform.localScale = new Vector3(Screen.width / 100 / scaleFactor, 1, Screen.height / 100 / scaleFactor);

        }
        else if (isScrolling)
        {
            if (scrollCooldown > 0)
                scrollCooldown--;
            else
            {
                shader.SetFloat("xMin", viewMin.x);
                shader.SetFloat("yMin", viewMin.y);
                shader.SetFloat("xMax", viewMax.x);
                shader.SetFloat("yMax", viewMax.y);
                shader.SetBool("recompute", true);
                frames = 0;

                isScrolling = false;
                scaleFactor = 1;
            }
        }
        else
        {
            renderTargetPlane.transform.localScale = new Vector3(Screen.width / 100, 1, Screen.height / 100);
            renderTargetPlane.transform.position = new Vector3();

            frames++;
            shader.SetInt("frames", frames);
            shader.GetKernelThreadGroupSizes(kernelId, out uint x, out uint y, out uint z);
            shader.Dispatch(kernelId, (int)(tex.width / x), (int)(tex.height / y), (int)(tex.depth / z));
            shader.SetBool("recompute", false);
        }
    }

    private void OnApplicationQuit()
    {
        if (tex != null)
            tex.Release();
        if (buf != null)
            buf.Release();
    }
}

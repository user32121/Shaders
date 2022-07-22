using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetPendulum : MonoBehaviour
{
    public GameObject renderTargetPlane;

    public ComputeShader shader;
    public Vector2 viewMin;
    public Vector2 viewMax;

    int kernelId;
    RenderTexture tex;
    ComputeBuffer buf;

    bool isScrolling;
    float scaleFactor = 1;
    Vector2 prevPos;

    int scrollCooldown;

    private void Start()
    {
        kernelId = shader.FindKernel("CSMain");
        RegenerateTexture();
    }

    struct pendulum
    {
        public Vector2 pos;
        public Vector2 vel;
    }

    private void RegenerateTexture()
    {
        if (tex != null)
            tex.Release();
        if (buf != null)
            buf.Release();

        tex = new RenderTexture(512, 512, 1) { enableRandomWrite = true };
        tex.Create();
        buf = new ComputeBuffer(tex.width * tex.height * tex.depth, sizeof(float) * 4);
        pendulum[] data = new pendulum[tex.width * tex.height * tex.depth];
        int stride = tex.height;
        for (int x = 0; x < tex.width; x++)
            for (int y = 0; y < tex.height; y++)
                data[x * stride + y] = new pendulum() { pos = new Vector2(viewMin.x + (float)x / tex.width * (viewMax.x - viewMin.x), viewMin.y + (float)y / tex.height * (viewMax.y - viewMin.y)) };
        buf.SetData(data);

        renderTargetPlane.GetComponent<Renderer>().material.mainTexture = tex;
        renderTargetPlane.transform.localScale = new Vector3(Screen.width / 100, 1, Screen.height / 100);

        shader.SetTexture(kernelId, "Result", tex);
        shader.SetBuffer(kernelId, "Data", buf);
        shader.SetInt("stride", stride);
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
                RegenerateTexture();

                isScrolling = false;
                scaleFactor = 1;
            }
        }
        else
        {
            renderTargetPlane.transform.localScale = new Vector3(Screen.width / 100, 1, Screen.height / 100);
            renderTargetPlane.transform.position = new Vector3();

            shader.GetKernelThreadGroupSizes(kernelId, out uint x, out uint y, out uint z);
            shader.Dispatch(kernelId, (int)(tex.width / x), (int)(tex.height / y), (int)(tex.depth / z));
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

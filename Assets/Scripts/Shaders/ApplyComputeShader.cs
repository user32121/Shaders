using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyComputeShader : MonoBehaviour
{
    public ComputeShader shader;
    public string shaderKernelName;
    public string shaderTextureName;
    public string shaderBufferName;

    int kernelId;
    RenderTexture tex;
    ComputeBuffer buf;

    private void Start()
    {
        kernelId = shader.FindKernel(shaderKernelName);
        tex = new RenderTexture(256, 256, 1) { enableRandomWrite = true };
        tex.Create();
        buf = new ComputeBuffer(tex.width * tex.height * tex.depth, sizeof(float) * 2);

        shader.SetTexture(kernelId, shaderTextureName, tex);
        shader.SetBuffer(kernelId, shaderBufferName, buf);
        shader.SetFloat("width", Screen.width);
        shader.SetFloat("height", Screen.height);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        shader.GetKernelThreadGroupSizes(kernelId, out uint x, out uint y, out uint z);
        shader.Dispatch(kernelId, (int)(tex.width / x), (int)(tex.height / y), (int)(tex.depth / z));
        Graphics.Blit(tex, destination);
    }

    private void OnApplicationQuit()
    {
        if (tex != null)
            tex.Release();
        if (buf != null)
            buf.Release();
    }
}

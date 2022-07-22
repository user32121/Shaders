using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO wall meshes
//TODO place camera according to size
//TODO draw walls

public class WaveShader : MonoBehaviour
{
    public int width = 64;
    public int height = 64;

    MeshFilter mf;
    Mesh mesh;
    float[,] values;
    int[,] walls;  //use int since bool is not blittable

    ComputeBuffer cbInPos;
    ComputeBuffer cbInVel;
    ComputeBuffer cbOutPos;
    ComputeBuffer cbOutVel;
    ComputeBuffer cbWalls;


    public Camera cam;
    public ComputeShader waveComputeShader;
    public bool constantTick;
    public bool tick;
    public bool trigger;

    bool prevButtonJump;


    void Start()
    {
        mf = GetComponent<MeshFilter>();
        mesh = mf.mesh;
        mf.mesh = mesh;
        mesh.vertices = new Vector3[width*height];  //w*h
        //3 vertices per triangle, 2 triangles per square
	    mesh.triangles = new int[2*3*(width-1)*(height-1)];  //2*3*(w-1)*(h-1)

        Vector3 camPos = cam.transform.position;
        camPos.x = width/2;
        cam.transform.position = camPos;

        cbInPos = new ComputeBuffer(width*height, sizeof(float));
        cbInVel = new ComputeBuffer(width*height, sizeof(float));
        cbOutPos = new ComputeBuffer(width*height, sizeof(float));
        cbOutVel = new ComputeBuffer(width*height, sizeof(float));
        cbWalls = new ComputeBuffer(width*height, sizeof(int));
        values = new float[width, height];
        walls = new int[width, height];

        waveComputeShader.SetInt("WIDTH", width);
        waveComputeShader.SetInt("HEIGHT", height);

        SetPoss();
        // SetWalls();
        UpdateMesh();
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void SetPoss(){
        for(int x = 0; x < width; x++)
            for(int y = 0; y < height; y++){
                values[x,y] = Random.value;
            }
    }
    void SetWalls(){
        for (int i = 0; i < Mathf.Max(width,height); i++){
            int x = Random.Range(2, width-2);
            int y = Random.Range(2, height-2);
            for (int x2 = -2; x2 <= 2; x2++)
                for (int y2 = -2; y2 <= 2; y2++)
                    walls[x+x2,y+y2] = 1;
        }
    }

    void UpdateMesh(){
        Vector3[] vertices = mesh.vertices;
        for(int x = 0; x < width; x++)
            for(int y = 0; y < height; y++){
                int p = x+y*width;
                vertices[p] = new Vector3(x, values[x,y], y);
            }
    	mesh.vertices = vertices;

        int[] triangles = mesh.triangles;
        int i = 0;
        for(int x = 0; x < width-1; x++)
            for(int y = 0; y < height-1; y++){
                int p = x+y*width;
                triangles[i] = p;
                triangles[i+1] = p+width;
                triangles[i+2] = p+1;
                triangles[i+3] = p+1;
                triangles[i+4] = p+width;
                triangles[i+5] = p+1+width;
                i += 6;
            }
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        
    }



    void Update()
    {
        if(trigger){
            trigger = false;
            AddDroplet();
        }
        if(Input.GetButtonDown("Jump") && !prevButtonJump){
            AddDroplet();
        }
        prevButtonJump = Input.GetButtonDown("Jump");

        if(tick || constantTick){
            tick = false;
            WaveTick();
        }
    }

    void AddDroplet(){
        int cenX = Random.Range(0, width);
        int cenY = Random.Range(0, height);
        AddDroplet(cenX, cenY);
    }
    void AddDroplet(int cenX, int cenY){
        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
                if(x+cenX >= 0 && y+cenY >= 0 && x+cenX < width && y+cenY < height)
                    values[cenX+x,cenY+y] = Mathf.Max(height, width);
    }

    void WaveTick(){
        values[0,0] = 0;  //TEMP figure out why 0,0 is increasing
        cbInPos.SetData(values);
        cbWalls.SetData(walls);
        waveComputeShader.SetBuffer(0, "inPos", cbInPos);
        waveComputeShader.SetBuffer(0, "inVel", cbInVel);
        waveComputeShader.SetBuffer(0, "outPos", cbOutPos);
        waveComputeShader.SetBuffer(0, "outVel", cbOutVel);
        waveComputeShader.SetBuffer(0, "walls", cbWalls);
        waveComputeShader.GetKernelThreadGroupSizes(0, out uint kernelGroupX, out uint kernelGroupY, out uint kernelGroupZ);
        waveComputeShader.Dispatch(0, width/(int)kernelGroupX, height/(int)kernelGroupY, 1);
        cbOutPos.GetData(values, 0, 0, width*height);

        ComputeBuffer temp = cbInVel;
        cbInVel = cbOutVel;
        cbOutVel = temp;

        UpdateMesh();
    }

    void OnMouseDown(){
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hitInfo)){
            AddDroplet((int)hitInfo.point.x, (int)hitInfo.point.z);
        }
    }

    void OnApplicationQuit(){
        cbInPos.Release();
        cbInVel.Release();
        cbOutPos.Release();
        cbOutVel.Release();
        cbWalls.Release();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TesterScript : MonoBehaviour
{
    private Camera cam;
    [SerializeField]
    private ComputeShader shader;
    [SerializeField, Range(0, 2)]
    private int mode;

    private RenderTexture texture;

    private int screenWidth;
    private int screenHeight;

    private int white;
    private int black;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        screenWidth = cam.pixelWidth;
        screenHeight = cam.pixelHeight;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        
        if ((texture == null) || (texture.width != screenWidth) || (texture.height != screenHeight))
        {
            texture = new RenderTexture(screenWidth, screenHeight, 24);
            texture.enableRandomWrite = true;
            texture.Create();
        }

        shader.SetTexture(mode, "Result", texture);
        shader.SetInt("Width", texture.width);
        shader.SetInt("Height", texture.height);
        shader.SetInt("White", white);
        shader.SetInt("Black", black);
        shader.SetFloat("Time", Time.realtimeSinceStartup);
        shader.Dispatch(mode, texture.width, texture.height, 1);

        Graphics.Blit(texture, destination);
    }

    public void Apply(int mod, int width, int height)
    {
        mode = mod;
        texture = new RenderTexture(screenWidth, screenHeight, 24);
        texture.enableRandomWrite = true;
        texture.Create();
    }

    public void SetMode(int value)
    {
        mode = value;
    }

    public void SetColors(float R, float G, float B)
    {
        shader.SetFloat("ColorR1", R);
        shader.SetFloat("ColorG1", G);
        shader.SetFloat("ColorB1", B);
    }

    public void SetColors1(float R, float G, float B)
    {
        shader.SetFloat("ColorR2", R);
        shader.SetFloat("ColorG2", G);
        shader.SetFloat("ColorB2", B);
    }

    public void SetWhite(int value)
    {
        white = value;
    }

    public void SetBlack(int value)
    {
        black = value;
    }
}

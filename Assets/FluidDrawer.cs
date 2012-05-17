using UnityEngine;
using System.Collections;

public class FluidDrawer : MonoBehaviour {
	
	int fluid_width = 64;
	int fluid_height = 64;
	
	public Color[] color;
	
	public Texture2D tex;
	
	int index;
		
	void Start () {
		color = new Color[fluid_width*fluid_height];
		
	}
		
	void Update () {
		
		
		UpdateTexture();
	}
	
	
	void UpdateTexture(){
		tex.SetPixels(color);
		tex.Apply();
	}
	
	public void DrawPixel(int x, int y){
		index = y*fluid_height + x;
		
		
		color[index] = new Color(100,0,0);
		
	}
}
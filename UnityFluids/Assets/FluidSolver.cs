using UnityEngine;
using System;
using System.Collections;

public class FluidSolver : MonoBehaviour
{
	public float dt, diff, visc;
	
	static int N = 64-2;
	public static int width = N+2;
	public static int height = N+2;
	int size = width*height;
	
	public float[] u, v, u_prev, v_prev; 
	public Color[] dens, dens_prev;	
	
	int solverIterations = 10;
	
	//----//Public Functions//----//	
	public void ColorArea()
	{
		for(var i=0; i<size/2; i++)
		{
			
			float y = Mathf.Round(i/width);
			y /= height;
			
			float x = i % width;
			x /= width;
			
			dens_prev[i] = new Color(1*y, 0, 1*x, 1); //dens_prev[i] += 255.0f;		
		}
	}		
	
	public void AddForce(Vector2 p, Vector2 f)
	{
		int i = (int)p.y * height + (int)p.x;		
		
		u_prev[i] += f.x;		
		v_prev[i] += f.y;		
	}
	
	public void DrawPixel(Vector2 p, Color c)
	{
		int i = (int)p.y * height + (int)p.x;
		
		dens_prev[i] = c;//255.0f;		
	}
	
	//----///Solver//----//
	void Start ()
	{		
		reset ();
		
		ColorArea();
	}
	
	void Update ()
	{				
		dt = Time.deltaTime*100.0f;
		
		vel_step( N, u, v, u_prev, v_prev, visc, dt );
		
		dens_step( N, dens, dens_prev, u, v, diff, dt );
	
		for (int i=0 ; i<size ; i++ ) {
			u_prev[i] = v_prev[i] = 0.0f;
			dens_prev[i] = new Color(0,0,0,0);
		}		
	}
	
	//--//Manages the vector feild//--//
	void vel_step ( int N, float[] u, float[] v, float[] u0, float[] v0, float visc, float dt )
	{
		add_source ( N, u, u0, dt );add_source ( N, v, v0, dt );
		
		SWAP (ref u0,ref u ); diffuse ( N, 1, u, u0, visc, dt );
		SWAP (ref v0,ref v ); diffuse ( N, 2, v, v0, visc, dt );
		
		project ( N, u, v, u0, v0 );
		
		SWAP (ref u0,ref u ); SWAP (ref v0,ref v );
		
		advect ( N, 1, u, u0, u0, v0, dt ); advect ( N, 2, v, v0, u0, v0, dt );
		
		project ( N, u, v, u0, v0 );
	}
	
	//--//Manages the density feild//--//	
	void dens_step ( int N, Color[] x, Color[] x0, float[] u, float[] v, float diff, float dt )
	{
		add_source_color ( N, x, x0, dt );
		SWAP (ref x0,ref x); diffuse_color ( N, 0, x, x0, diff, dt );
		SWAP (ref x0,ref x); advect_color ( N, 0, x, x0, u, v, dt );
	}
	
	//adds new sources to vector or density feild	
	void add_source ( int N, float[] x, float[] s, float dt )
	{
		int i; 
		//***already calculated size=(N+2)*(N+2);
		
		for ( i=0 ; i<size ; i++ ) x[i] += dt*s[i];
	}
	
	void add_source_color ( int N, Color[] x, Color[] s, float dt )
	{
		int i; 
		//***already calculated size=(N+2)*(N+2);
		
		for ( i=0 ; i<size ; i++ ) x[i] += dt*s[i];
	}
	
	//manages boundaries
	void set_bnd ( int N, int b, float[] x )
	{
		int i;
	
		for ( i=1 ; i<=N ; i++ ) {
			//x[IX(0  ,i)] = b==1 ? -x[IX(1,i)] : x[IX(1,i)]; //Left border
			//x[IX(N+1,i)] = b==1 ? -x[IX(N,i)] : x[IX(N,i)]; //Right border
			//x[IX(i,0  )] = b==2 ? -x[IX(i,1)] : x[IX(i,1)]; //Up border
			x[IX(i,N+1)] = b==2 ? -x[IX(i,N)] : x[IX(i,N)]; //Down border
		}
		
		/*x[IX(0  ,0  )] = 0.5f*(x[IX(1,0  )]+x[IX(0  ,1)]);
		x[IX(0  ,N+1)] = 0.5f*(x[IX(1,N+1)]+x[IX(0  ,N)]);
		x[IX(N+1,0  )] = 0.5f*(x[IX(N,0  )]+x[IX(N+1,1)]);
		x[IX(N+1,N+1)] = 0.5f*(x[IX(N,N+1)]+x[IX(N+1,N)]);*/
	}
	
	void set_bnd_color ( int N, int b, Color[] x )
	{		
		x[IX(0  ,0  )] = 0.5f*(x[IX(1,0  )]+x[IX(0  ,1)]);
		x[IX(0  ,N+1)] = 0.5f*(x[IX(1,N+1)]+x[IX(0  ,N)]);
		x[IX(N+1,0  )] = 0.5f*(x[IX(N,0  )]+x[IX(N+1,1)]);
		x[IX(N+1,N+1)] = 0.5f*(x[IX(N,N+1)]+x[IX(N+1,N)]);
	}
	
	//Each cell exchanges density or force with neighbours
	void lin_solve ( int N, int b, float[] x, float[] x0, float a, float c )
	{
		int i, j, k;
	
		for ( k=0 ; k < solverIterations; k++ )
		{
			for ( i=1 ; i<=N ; i++ ) 
			{ 
				for ( j=1 ; j<=N ; j++ ) 
				{
					x[IX(i,j)] = (x0[IX(i,j)] + a*(x[IX(i-1,j)]+x[IX(i+1,j)]+x[IX(i,j-1)]+x[IX(i,j+1)]))/c;		
				}
				
			}
			
			set_bnd ( N, b, x );
		}
	}
	
	void lin_solve_color ( int N, int b, Color[] x, Color[] x0, float a, float c )
	{
		int i, j, k;
	
		for ( k=0 ; k < solverIterations; k++ )
		{
			for ( i=1 ; i<=N ; i++ ) 
			{ 
				for ( j=1 ; j<=N ; j++ ) 
				{
					x[IX(i,j)] = (x0[IX(i,j)] + a*(x[IX(i-1,j)]+x[IX(i+1,j)]+x[IX(i,j-1)]+x[IX(i,j+1)]))/c;		
				}
				
			}
			
			set_bnd_color ( N, b, x );
		}
	}
	
	//Manages diffusion, liniarly 
	void diffuse ( int N, int b, float[] x, float[] x0, float diff, float dt )
	{
		float a = dt * diff * N * N;
		lin_solve ( N, b, x, x0, a, 1+4*a );
	}
	
	void diffuse_color ( int N, int b, Color[] x, Color[] x0, float diff, float dt )
	{
		float a = dt * diff * N * N;
		lin_solve_color ( N, b, x, x0, a, 1+4*a );
	}
	
	//Interpolates density or forces through a field
	void advect ( int N, int b, float[] d, float[] d0, float[] u, float[] v, float dt )
	{
		int i, j, i0, j0, i1, j1;
		float x, y, s0, t0, s1, t1, dt0;
	
		dt0 = dt*N;
		
		for ( i=1 ; i<=N ; i++ ) { 
			for ( j=1 ; j<=N ; j++ ) {
				x = i-dt0*u[IX(i,j)]; 
				y = j-dt0*v[IX(i,j)];
				
				if (x<0.5f) x=0.5f; 
				if (x>N+0.5f) x=N+0.5f; 
				i0=(int)x; 
				i1=i0+1;
				
				if (y<0.5f) y=0.5f; 
				if (y>N+0.5f) y=N+0.5f; 
				j0=(int)y; j1=j0+1;
				
				s1 = x-i0; 
				s0 = 1-s1; 
				t1 = y-j0; 
				t0 = 1-t1;
				d[IX(i,j)] = s0*(t0*d0[IX(i0,j0)]+t1*d0[IX(i0,j1)]) + s1*(t0*d0[IX(i1,j0)]+t1*d0[IX(i1,j1)]);
			}
		}
		
		set_bnd ( N, b, d );
	}
	
	void advect_color ( int N, int b, Color[] d, Color[] d0, float[] u, float[] v, float dt )
	{
		int i, j, i0, j0, i1, j1;
		float x, y, s0, t0, s1, t1, dt0;
	
		dt0 = dt*N;
		
		for ( i=1 ; i<=N ; i++ ) { 
			for ( j=1 ; j<=N ; j++ ) {
				x = i-dt0*u[IX(i,j)]; 
				y = j-dt0*v[IX(i,j)];
				
				if (x<0.5f) x=0.5f; 
				if (x>N+0.5f) x=N+0.5f; 
				i0=(int)x; 
				i1=i0+1;
				
				if (y<0.5f) y=0.5f; 
				if (y>N+0.5f) y=N+0.5f; 
				j0=(int)y; j1=j0+1;
				
				s1 = x-i0; 
				s0 = 1-s1; 
				t1 = y-j0; 
				t0 = 1-t1;
				d[IX(i,j)] = s0*(t0*d0[IX(i0,j0)]+t1*d0[IX(i0,j1)]) + s1*(t0*d0[IX(i1,j0)]+t1*d0[IX(i1,j1)]);
			}
		}
		
		set_bnd_color ( N, b, d );
	}
	
	
	//Swirls created through enforcing mass conservation	
	void project ( int N, float[] u, float[] v, float[] p, float[] div )
	{
		int i, j;
	
		for ( i=1 ; i<=N ; i++ ) 
		{
			for ( j=1 ; j<=N ; j++ ) 
			{
				div[IX(i,j)] = -0.5f*(u[IX(i+1,j)]-u[IX(i-1,j)]+v[IX(i,j+1)]-v[IX(i,j-1)])/N;
				p[IX(i,j)] = 0;
			}
		}	
		set_bnd ( N, 0, div ); set_bnd ( N, 0, p );
	
		lin_solve ( N, 0, p, div, 1, 4 );
	
		for ( i=1 ; i<=N ; i++ ) 
		{ 
			for ( j=1 ; j<=N ; j++ ) 
			{
				u[IX(i,j)] -= 0.5f*N*(p[IX(i+1,j)]-p[IX(i-1,j)]);
				v[IX(i,j)] -= 0.5f*N*(p[IX(i,j+1)]-p[IX(i,j-1)]);
			}
		}
		
		set_bnd ( N, 1, u ); set_bnd ( N, 2, v );
	}
	
	//Helper Functions	
	void reset ()
	{		
		u = new float[size];
		v = new float[size];
		u_prev = new float[size];
		v_prev = new float[size];
		dens = new Color[size];
		dens_prev = new Color[size];
		
		for (int i = size-1; i>=0; --i) {
			u [i] = 0;
			u_prev [i] = 0;
			v [i] = 0;
			v_prev [i] = 0;
			dens [i] = new Color(0,0,0,0);
			dens_prev [i] = new Color(0,0,0,0);
		}		
	}
	
	public int GetSize()
	{
		return size;
	}		
	
	int IX(int i,int j)
	{
		return ((i)+(N+2)*(j));
	}
	
	//swaps the two pointers
	static void SWAP<T> (ref T lhs, ref T rhs)
	{
		T temp;
		temp = lhs;
		lhs = rhs;
		rhs = temp;
	}
}
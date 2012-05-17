


using UnityEngine;
using System.Collections;

public class FluidSolver : MonoBehaviour {
	const double ZERO_THRESH =	0.0000000001;
	
	int NX = 62;
	int NY = 62;
	
	public Color[] color, colorOld;
	public Vector2[] uv, uvOld;
	
	public float	colorDiffusion;
	public float	viscocity;
	public float	fadeSpeed;
	public float	deltaT;
	
	
	//-----
	
	public int width;
	public int height;
	float invWidth;
	float invHeight;	
	int		_numCells;
	float	_invNX, _invNY, _invNumCells;
	bool	_isInited;

	

	// Use this for initialization
	void Start () {
		_isInited = false;
		
		_numCells = (NX + 2) * (NY + 2);
		
		_invNX = 1.0f / NX;
		_invNY = 1.0f / NY;
		_invNumCells = 1.0f / _numCells;
		
		width           = NX + 2;
		height          = NY + 2;
		invWidth        = 1.0f/width;
		invHeight       = 1.0f/height;
		
		reset();
	}
	
	
	void reset() {
		_isInited = true;
		
		color		= new Color[_numCells];
		colorOld	= new Color[_numCells];
		uv    = new Vector2[_numCells];
		uvOld = new Vector2[_numCells];
		
		for ( int i = _numCells-1; i>=0; --i ) {
			color[i] = Color.black;
			colorOld[i] = Color.blue;
			uv[i] = Vector2.zero;
			uvOld[i] = Vector2.zero;
		}
	}
	
	void CHECK_ZERO(float p){
		if(Mathf.Abs(p)<ZERO_THRESH) p = 0;
	}

	
	// Update is called once per frame
	void Update () {
	
	/*	addSource(uv, uvOld);

		SWAP(uv, uvOld);
		
		diffuseUV( viscocity );
		
		project(uv, uvOld);
		
		SWAP(uv, uvOld);
		
		advect2d(uv, uvOld);
		
		project(uv, uvOld);
		*/
		//if(doRGB)
		{
			
			{ //addSource(color, colorOld);
				for(int i = _numCells-1; i >=0; --i) {
					color[i] += colorOld[i] * deltaT;
				}
			}
			
			
			{ //SWAP(color, colorOld);
				Color[] tmp = colorOld;
				colorOld = color;
				color = tmp;
			}
			
			
			if( colorDiffusion!=0 && deltaT!=0 )
			{
//				diffuseRGB(0, colorDiffusion );
				
				{ //SWAP(color, colorOld);
					Color[] tmp = colorOld;
					colorOld = color;
					color = tmp;
				}

			}
			
			//advectRGB(0, uv);
			fadeRGB();
		} 
		
	}
	
	
	void fadeRGB() {
		// I want the fluid to gradually fade out so the screen doesn't fill. the amount it fades out depends on how full it is, and how uniform (i.e. boring) the fluid is...
		//		float holdAmount = 1 - _avgDensity * _avgDensity * fadeSpeed;	// this is how fast the density will decay depending on how full the screen currently is
		float holdAmount = 1 - fadeSpeed;
		
		float _avgDensity = 0;
		float _avgSpeed = 0;
		
		float totalDeviations = 0;
		float currentDeviation;
		Color tmp = Color.black;
		

		for (int i = _numCells-1; i >=0; --i)
		{
			// clear old values
			uvOld[i] = Vector2.zero;
			colorOld[i] = Color.black;
			
			// calc avg speed
			_avgSpeed += uv[i].x * uv[i].x + uv[i].y * uv[i].y;
			
			// calc avg density
			tmp.r = Mathf.Min( 1.0f, color[i].r );
			tmp.g = Mathf.Min( 1.0f, color[i].g );
			tmp.b = Mathf.Min( 1.0f, color[i].b );
//			tmp.a = min( 1.0f, color[i].a );
			
//			float density = max(tmp.a, max( tmp.r, max( tmp.g, tmp.b ) ) );
//			float density = max( tmp.r, max( tmp.g, tmp.b ) );
			float density = Mathf.Max( tmp.r, Mathf.Max( tmp.g, tmp.b ) );
			_avgDensity += density;	// add it up
			
			// calc deviation (for _uniformity)
			currentDeviation = density - _avgDensity;
			totalDeviations += currentDeviation * currentDeviation;
			
			// fade out old
			color[i] = tmp * holdAmount;
			
			CHECK_ZERO(color[i].r);
			CHECK_ZERO(color[i].g);
			CHECK_ZERO(color[i].b);
//			CHECK_ZERO(color[i].a);
			CHECK_ZERO(uv[i].x);
			CHECK_ZERO(uv[i].y);
		}
		_avgDensity *= _invNumCells;
		_avgSpeed *= _invNumCells;
		
		//println("%.3f\n", _avgDensity);
		//_uniformity = 1.0f / (1 + totalDeviations * _invNumCells);		// 0: very wide distribution, 1: very uniform
	}
	
	
//	void FluidSolver::addSourceUV()
//	{
//		for (int i = _numCells-1; i >=0; --i) {
//			uv[i] += deltaT * uvOld[i];
//		}
//	}
//	
//	void FluidSolver::addSourceRGB()
//	{
//		for (int i = _numCells-1; i >=0; --i) {
//			color[i] += deltaT * colorOld[i];
//		}
//	}
//	
//	void FluidSolver::addSource(float* x, float* x0) {
//		for (int i = _numCells-1; i >=0; --i) {
//			x[i] += deltaT * x0[i];
//		}
//	}
	/*
	void FluidSolver::advect( int bound, float* d, const float* d0, const Vec2f* duv) {
		int i0, j0, i1, j1;
		float x, y, s0, t0, s1, t1;
		int	index;
		
		const float dt0x = deltaT * _NX;
		const float dt0y = deltaT * _NY;
		
		for (int j = _NY; j > 0; --j)
		{
			for (int i = _NX; i > 0; --i)
			{
				index = FLUID_IX(i, j);
				x = i - dt0x * duv[index].x;
				y = j - dt0y * duv[index].y;
				
				if (x > _NX + 0.5) x = _NX + 0.5f;
				if (x < 0.5)     x = 0.5f;
				
				i0 = (int) x;
				i1 = i0 + 1;
				
				if (y > _NY + 0.5) y = _NY + 0.5f;
				if (y < 0.5)     y = 0.5f;
				
				j0 = (int) y;
				j1 = j0 + 1;
				
				s1 = x - i0;
				s0 = 1 - s1;
				t1 = y - j0;
				t0 = 1 - t1;
				
				d[index] = s0 * (t0 * d0[FLUID_IX(i0, j0)] + t1 * d0[FLUID_IX(i0, j1)])
				+ s1 * (t0 * d0[FLUID_IX(i1, j0)] + t1 * d0[FLUID_IX(i1, j1)]);
				
			}
		}
		setBoundary(bound, d);
	}
	
	//          d    d0    du    dv
	// advect(1, u, uOld, uOld, vOld);
	// advect(2, v, vOld, uOld, vOld);
	void FluidSolver::advect2d( Vec2f *uv, const Vec2f *duv ) {
		int i0, j0, i1, j1;
		float s0, t0, s1, t1;
		int	index;
		
		const float dt0x = deltaT * _NX;
		const float dt0y = deltaT * _NY;
		
		for (int j = _NY; j > 0; --j)
		{
			for (int i = _NX; i > 0; --i)
			{
				index = FLUID_IX(i, j);
				float x = i - dt0x * duv[index].x;
				float y = j - dt0y * duv[index].y;
				
				if (x > _NX + 0.5) x = _NX + 0.5f;
				if (x < 0.5)     x = 0.5f;
				
				i0 = (int) x;
				i1 = i0 + 1;
				
				if (y > _NY + 0.5) y = _NY + 0.5f;
				if (y < 0.5)     y = 0.5f;
				
				j0 = (int) y;
				j1 = j0 + 1;
				
				s1 = x - i0;
				s0 = 1 - s1;
				t1 = y - j0;
				t0 = 1 - t1;
				
				uv[index].x = s0 * (t0 * duv[FLUID_IX(i0, j0)].x + t1 * duv[FLUID_IX(i0, j1)].x)
				+ s1 * (t0 * duv[FLUID_IX(i1, j0)].x + t1 * duv[FLUID_IX(i1, j1)].x);
				uv[index].y = s0 * (t0 * duv[FLUID_IX(i0, j0)].y + t1 * duv[FLUID_IX(i0, j1)].y)
				+ s1 * (t0 * duv[FLUID_IX(i1, j0)].y + t1 * duv[FLUID_IX(i1, j1)].y);
				
			}
		}
		setBoundary2d(1, uv);
		setBoundary2d(2, uv);	
	}
	
	void FluidSolver::advectRGB(int bound, const Vec2f* duv) {
		int i0, j0;
		float x, y, s0, t0, s1, t1, dt0x, dt0y;
		int	index;
		
		dt0x = deltaT * _NX;
		dt0y = deltaT * _NY;
		
		for (int j = _NY; j > 0; --j)
		{
			for (int i = _NX; i > 0; --i)
			{
				index = FLUID_IX(i, j);
				x = i - dt0x * duv[index].x;
				y = j - dt0y * duv[index].y;
				
				if (x > _NX + 0.5) x = _NX + 0.5f;
				if (x < 0.5)     x = 0.5f;
				
				i0 = (int) x;
				
				if (y > _NY + 0.5) y = _NY + 0.5f;
				if (y < 0.5)     y = 0.5f;
				
				j0 = (int) y;
				
				s1 = x - i0;
				s0 = 1 - s1;
				t1 = y - j0;
				t0 = 1 - t1;
				
				i0 = FLUID_IX(i0, j0);	//we don't need col/row index any more but index in 1 dimension
				j0 = i0 + (_NX + 2);
				color[index] = ( colorOld[i0] * t0 + colorOld[j0] * t1 ) * s0 + ( colorOld[i0+1] * t0 + colorOld[j0+1] * t1) * s1;
			}
		}
		setBoundaryRGB();
	}
	
	void FluidSolver::diffuse( int bound, float* c, float* c0, float diff )
	{
		float a = deltaT * diff * _NX * _NY;	//todo find the exact strategy for using _NX and _NY in the factors
		linearSolver( bound, c, c0, a, 1.0 + 4 * a );
	}
	
	void FluidSolver::diffuseRGB( int bound, float diff )
	{
		float a = deltaT * diff * _NX * _NY;
		linearSolverRGB( a, 1.0 + 4 * a );
	}
	
	void FluidSolver::diffuseUV( float diff )
	{
		float a = deltaT * diff * _NX * _NY;
		linearSolverUV( a, 1.0 + 4 * a );
	}
	
	void FluidSolver::project(Vec2f* xy, Vec2f* pDiv) 
	{
		float	h;
		int		index;
		int		step_x = _NX + 2;
		
		h = - 0.5f / _NX;
		for (int j = _NY; j > 0; --j)
		{
			index = FLUID_IX(_NX, j);
			for (int i = _NX; i > 0; --i)
			{
				pDiv[index].x = h * ( xy[index+1].x - xy[index-1].x + xy[index+step_x].y - xy[index-step_x].y );
				pDiv[index].y = 0;
				--index;
			}
		}
		
		setBoundary02d( reinterpret_cast<Vec2f*>( &pDiv[0].x ));
		setBoundary02d( reinterpret_cast<Vec2f*>( &pDiv[0].y ));
		
		linearSolverProject( pDiv );
		
		float fx = 0.5f * _NX;
		float fy = 0.5f * _NY;	//maa	change it from _NX to _NY
		for (int j = _NY; j > 0; --j)
		{
			index = FLUID_IX(_NX, j);
			for (int i = _NX; i > 0; --i)
			{
				xy[index].x -= fx * (pDiv[index+1].x - pDiv[index-1].x);
				xy[index].y -= fy * (pDiv[index+step_x].x - pDiv[index-step_x].x);
				--index;
			}
		}
		
		setBoundary2d(1, xy);
		setBoundary2d(2, xy);
	}
	
	
	//	Gauss-Seidel relaxation
	void FluidSolver::linearSolver( int bound, float* __restrict x, const float* __restrict x0, float a, float c )
	{
		int	step_x = _NX + 2;
		int index;
		c = 1. / c;
		for (int k = solverIterations; k > 0; --k)	// MEMO 
		{
			for (int j = _NY; j > 0 ; --j)
			{
				index = FLUID_IX(_NX, j );
				for (int i = _NX; i > 0 ; --i)
				{
					x[index] = ( ( x[index-1] + x[index+1] + x[index - step_x] + x[index + step_x] ) * a + x0[index] ) * c;
					--index;				
				}
			}
			setBoundary( bound, x );
		}
	}
	
	void FluidSolver::linearSolverProject( Vec2f* __restrict pdiv )
	{
		int	step_x = _NX + 2;
		int index;
		for (int k = solverIterations; k > 0; --k) {
			for (int j = _NY; j > 0 ; --j) {
				index = FLUID_IX(_NX, j );
				float prev = pdiv[index+1].x;
				for (int i = _NX; i > 0 ; --i)
				{
					prev = ( pdiv[index-1].x + prev + pdiv[index - step_x].x + pdiv[index + step_x].x + pdiv[index].y ) * .25;
					pdiv[index].x = prev;
					--index;
				}
			}
			setBoundary02d( reinterpret_cast<Vec2f*>( &pdiv[0].x ) );
		}
	}
	
	void FluidSolver::linearSolverRGB( float a, float c )
	{
		int index3, index4, index;
		int	step_x = _NX + 2;
		c = 1. / c;
		for ( int k = solverIterations; k > 0; --k )	// MEMO
		{           
			for (int j = _NY; j > 0 ; --j)
			{
				index = FLUID_IX(_NX, j );
				//index1 = index - 1;		//FLUID_IX(i-1, j);
				//index2 = index + 1;		//FLUID_IX(i+1, j);
				index3 = index - step_x;	//FLUID_IX(i, j-1);
				index4 = index + step_x;	//FLUID_IX(i, j+1);
				for (int i = _NX; i > 0 ; --i)
				{	
					color[index] = ( ( color[index-1] + color[index+1]  +  color[index3] + color[index4] ) * a  +  colorOld[index] ) * c;                                
					--index;
					--index3;
					--index4;
				}
			}
			setBoundaryRGB();	
		}
	}
	
	void FluidSolver::linearSolverUV( float a, float c )
	{
		int index;
		int	step_x = _NX + 2;
		c = 1. / c;
		Vec2f* __restrict localUV = uv;
		const Vec2f* __restrict localOldUV = uvOld;
		
		for (int k = solverIterations; k > 0; --k)	// MEMO
		{           
			for (int j = _NY; j > 0 ; --j)
			{
				index = FLUID_IX(_NX, j );
				float prevU = localUV[index+1].x;
				float prevV = localUV[index+1].y;
				for (int i = _NX; i > 0 ; --i)
				{
					prevU = ( ( localUV[index-1].x + prevU + localUV[index - step_x].x + localUV[index + step_x].x ) * a  + localOldUV[index].x ) * c;
					prevV = ( ( localUV[index-1].y + prevV + localUV[index - step_x].y + localUV[index + step_x].y ) * a  + localOldUV[index].y ) * c;
					localUV[index].x = prevU;
					localUV[index].y = prevV;
					--index;
				}
			}
			setBoundary2d( 1, uv );
		}
	}
	
	*/
}

#pragma strict


var tex : Texture2D;

var HitInput : HitPixel ;

var Pixar : Color[];

static var u: float[];

static var v: float[];

static var u_prev: float[];

static var v_prev: float[];

static var dens: float[];

static var dens_prev: float[];

var visc : float;

var dt : float;

var diff : float;

var NumberOfRows : int;

var FieldSize : int;

private var xx : float;

private var yy : float;

private var zz : float;

private var FieldWidth : int;

private var FieldHeight : int;

var ArrPos : int;

 

function Start () 

{

    Pixar = new Color[4096];

    NumberOfRows=62;

    FieldSize=(NumberOfRows+2)*(NumberOfRows+2);

    u = new float[FieldSize];

    v = new float[FieldSize];

    u_prev = new float[FieldSize];

    v_prev = new float[FieldSize];

    dens = new float[FieldSize];

    dens_prev = new float[FieldSize];

    

    FieldWidth=NumberOfRows+2;

    FieldHeight=NumberOfRows+2;

    

    for (var i : int = 0;i < FieldSize; i++)

    {

    dens_prev[i]=0.01; 

    }

        

    visc=0.005;

    dt=0.1;

    diff=1000.0001;

}

 

 

function Update () 

{

    

    

        if(HitInput.hashit==true)

        {   

            ArrPos=HitInput.y*64+HitInput.x;

            dens_prev[ArrPos]=50.0;

            u_prev[ArrPos]=HitInput.h*10000.0;  

            v_prev[ArrPos]=HitInput.v*10000.0;  

            

        }

 

        vel_step ( NumberOfRows, u, v, u_prev, v_prev, visc, dt ); 

        dens_step ( NumberOfRows, dens, dens_prev, u, v, diff, dt );

    

        for(var i=0;i<FieldHeight;i++)

        {

            for(var i2=0;i2<FieldWidth;i2++)

            {

                ArrPos=i*64+i2;

                Pixar[ArrPos]=Color(dens[ArrPos]*0.4, dens[ArrPos]*0.4, dens[ArrPos]*0.4);

                dens_prev[ArrPos]=dens[ArrPos];

                u_prev[ArrPos]=u[ArrPos];

                v_prev[ArrPos]=v[ArrPos];   

            }           

        }       

        

    tex.SetPixels(Pixar);   
	
    tex.Apply();    

    dt=Time.deltaTime;

}

 

 

 

 

function IX(i : int, j : int)

{

    return i+((NumberOfRows+2)*j);

}

 

 

function source_add ( N : int, x : float[], s : float[], dt : float)

{

    var i : int;

    var size : float;

    

    size=(N+2)*(N+2);

    

    for ( i=0 ; i<size ; i++ ) 

    {

        x[i] += dt*s[i];

    }    

}

 

 

function diffuse (N : int, b : int, x : float[], x0 : float[], diff : float, dt : float)

{

    var i : int;

    var j : int;

    var k : int;

    var a : float;  

        

    a=dt*diff*N*N;

    

    for(k=0;k<20;k++) 

    {

        for(i=1;i<=N;i++)

        {

            for(j=1;j<=N;j++) 

            {

                

                x[IX(i,j)] = (x0[IX(i,j)] + a*(x[IX(i-1,j)]+x[IX(i+1,j)]+x[IX(i,j-1)]+x[IX(i,j+1)]))/(1.0+4.0*a);

            } 

        }

        

        bnd_set (N, b, x);

    }

}

 

 

function advect (N : int, b : int, d : float[], d0 : float[], u : float[], v : float[], dt : float)

{

    var i : int;

    var j : int;

    var i0 : int;

    var j0 : int;

    var i1 : int;

    var j1 : int;   

    var x : float;

    var y : float;

    var s0 : float;

    var t0 : float;

    var s1 : float;

    var t1 : float;

    var dt0 : float;

    

    dt0 = dt*N;

    

    for (i=1 ; i<=N ; i++) 

    {

        for (j=1 ; j<=N ; j++)

        {

            x = i-dt0*u[IX(i,j)]; 

            y = j-dt0*v[IX(i,j)];

            

            if(x<0.5)

            {

                x=0.5;

            }    

            

            if(x>N+0.5) 

            {

                x=N+0.5; 

            }

            

            i0=Mathf.FloorToInt(x);

            i1=i0+1; 

            

            if(y<0.5)

            {

                y=0.5; 

            }

            

            if(y>N+0.5) 

            {

                y=N+0.5; 

            }

            

            j0=Mathf.FloorToInt(y);

            j1=j0+1; 

            s1 = x-i0; 

            s0 = 1.0-s1; 

            t1 = y-j0; 

            t0 = 1.0-t1;

            

            d[IX(i,j)] = s0*(t0*d0[IX(i0,j0)]+t1*d0[IX(i0,j1)])+s1*(t0*d0[IX(i1,j0)]+t1*d0[IX(i1,j1)]);

        } 

    }

    bnd_set ( N, b, d ); 

}

 

 

function SWAP(x0 : float[], x : float[])

{

    var tmp = new float[x0.length];

    tmp=x0;

    x0=x;

    x=tmp;

}

 

 

function dens_step (N : int, x : float[], x0 : float[], u : float[], v : float[], diff : float, dt : float)

{

    source_add (N, x, x0, dt);

    SWAP (x0, x); 

    diffuse (N, 0, x, x0, diff, dt); 

    SWAP (x0, x); 

    advect (N, 0, x, x0, u, v, dt);

}

 

 

function vel_step(N : int, u : float[], v : float[], u0 : float[], v0 : float[], visc : float, dt : float)

{

    source_add (N, u, u0, dt); 

    source_add (N, v, v0, dt);

    SWAP (u0, u); 

    diffuse (N, 1, u, u0, visc, dt);

    SWAP (v0, v); 

    diffuse (N, 2, v, v0, visc, dt);

    project (N, u, v, u0, v0);

    SWAP (u0, u ); SWAP ( v0, v);

    advect (N, 1, u, u0, u0, v0, dt);

    advect (N, 2, v, v0, u0, v0, dt); 

    project (N, u, v, u0, v0);

}

 

 

function project (N : int, u : float[], v : float[], p : float[], div : float[])

{

    var i : int;

    var j : int;

    var k : int;    

    var h : float;

    

    h = 1.0/N;

    

    for (i=1;i<=N;i++)

    {

        for (j=1;j<=N;j++) 

        {

            div[IX(i,j)] = -0.5*h*(u[IX(i+1,j)]-u[IX(i-1,j)]+v[IX(i,j+1)]-v[IX(i,j-1)]);

            p[IX(i,j)] = 0.0;

        }

    }   

    

    bnd_set (N, 0, div); 

    bnd_set (N, 0, p);

 

    for (k=0;k<20;k++)

    {

        for (i=1;i<=N;i++)

        {

            for(j=1;j<=N;j++)

            {

                p[IX(i,j)] = (div[IX(i,j)]+p[IX(i-1,j)]+p[IX(i+1,j)]+p[IX(i,j-1)]+p[IX(i,j+1)])/4.0;

            }

        }

                

        bnd_set ( N, 0, p );

    }

    

    for (i=1;i<=N;i++)

    {

        for (j=1;j<=N;j++)

        {

            u[IX(i,j)] -= 0.5*(p[IX(i+1,j)]-p[IX(i-1,j)])/h;

            v[IX(i,j)] -= 0.5*(p[IX(i,j+1)]-p[IX(i,j-1)])/h;

       }

    }

    

    bnd_set (N, 1, u);

    bnd_set (N, 2, v); 

}

 

 

function bnd_set ( N : int, b : int, x : float[])

{

    var i : int;

    

    for(i=1;i<=N;i++) 

    {

        if(b==1)

        {

            x[IX(0, i)] = -x[IX(1,i)];

            x[IX(N+1,i)] = -x[IX(N,i)];

        }   

        else

        {

            x[IX(0, i)] = x[IX(1,i)];

            x[IX(N+1,i)] = x[IX(N,i)];

        }   

        

        if(b==2)

        {

            x[IX(i,0 )]=  -x[IX(i,1)]; 

            x[IX(i,N+1)]= -x[IX(i,N)];

        }

        else

        {

            x[IX(i,0 )]= x[IX(i,1)]; 

            x[IX(i,N+1)]= x[IX(i,N)];

        }

        

    }

 

    x[IX(0 ,0 )] = 0.5*(x[IX(1,0 )]+x[IX(0 ,1)]); 

    x[IX(0 ,N+1)] = 0.5*(x[IX(1,N+1)]+x[IX(0 ,N )]); 

    x[IX(N+1,0 )] = 0.5*(x[IX(N,0 )]+x[IX(N+1,1)]); 

    x[IX(N+1,N+1)] = 0.5*(x[IX(N,N+1)]+x[IX(N+1,N)]);

}
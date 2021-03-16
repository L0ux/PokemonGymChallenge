using System.Collections;
using System;

[Serializable]
public struct Bounds
{   
    public Coord min;
    public Coord max;

    public override string ToString(){
        return ("min.x = "+min.x+" min.y = "+min.y+" | max.x = "+max.x+" max.y = "+max.y);
    }

    [Serializable]
    public  struct Coord{
        
        public int x;
        public int y;

    }
} 

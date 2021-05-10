using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardCell : MonoBehaviour
{
    public int X {
        get { return (int)transform.position.x; }
        set { 
            Vector3 old = transform.position;
            old.x = value * GameBoardMetrics.CELL_SIZE; 
            transform.position = old;
        }
    }

    public int Z {
        get { return (int)transform.position.z; }
        set { 
            Vector3 old = transform.position;
            old.z = value * GameBoardMetrics.CELL_SIZE; 
            transform.position = old;
        }
    }

    public bool IsCorner {get; set;}

    public Color Color {get; set;} = Color.white;

    public void SetPosition(int x, int z) {
        Vector3 pos = transform.position;
        pos.x = x * GameBoardMetrics.CELL_SIZE; 
        pos.z = z * GameBoardMetrics.CELL_SIZE; 
        transform.position = pos;
    }
}

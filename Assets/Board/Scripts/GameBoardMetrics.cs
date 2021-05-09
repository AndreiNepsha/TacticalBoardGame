using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardMetrics
{
    public static float CELL_SIZE = 1f;

    public static Vector3[] CELL_CORNERS = {
		new Vector3(CELL_SIZE/2, 0f, CELL_SIZE/2),
		new Vector3(-CELL_SIZE/2, 0f, CELL_SIZE/2),
		new Vector3(-CELL_SIZE/2, 0f, -CELL_SIZE/2),
		new Vector3(CELL_SIZE/2, 0f, -CELL_SIZE/2)
	};
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    private GameBoardMesh mesh;

	[SerializeField]
	private GameBoardCell cellPrefab;

	[SerializeField]
	private Player playerPrefab;

	private List<GameBoardCell> cells = new List<GameBoardCell>();
	private Dictionary<Player, int> playersOnBoard = new Dictionary<Player, int>();

	private bool isPlayerMove = false;

	private Queue<IEnumerator> playerMovementsQueue = new Queue<IEnumerator>();

    void Awake () {
		mesh = GetComponentInChildren<GameBoardMesh>();

		InitMap();
		CreatPlayer();
	}

	void Start() {
		mesh.Render(cells);
	}

	void Update() {
		if (!isPlayerMove && Input.GetKeyDown(KeyCode.Space)) {
			MovePlayer(playersOnBoard.First().Key);
		}
	}

	private void CreatPlayer () {
		Vector3 position = Vector3.zero;
		
		Player player = Instantiate<Player>(playerPrefab);
		player.transform.SetParent(transform, false);
		position.y = (player.GetComponent<MeshFilter>().mesh.bounds.size.y * player.transform.localScale.y)/2;
		player.transform.localPosition = position;

		playersOnBoard.Add(player, 0);
	}

	private void MovePlayer(Player player) {
		int cellMoves = Random.Range(1, 7);

		Debug.Log($"Move player {cellMoves} cells forward");

		int currentCellIndex = playersOnBoard[player];
		GameBoardCell currentCell = cells[currentCellIndex];
		Vector2Int lastMoveDirection = Vector2Int.zero;

		for (int i = 0; i < cellMoves; i++) {
			int nextCellIndex = currentCellIndex == cells.Count-1 ? 0 : currentCellIndex+1;
			GameBoardCell nextCell = cells[nextCellIndex];

			if (currentCell.IsCorner || i == cellMoves-1) 
			{
				Vector3 endPosition = currentCell.IsCorner
					? new Vector3(currentCell.X, player.transform.localPosition.y, currentCell.Z)
					: new Vector3(nextCell.X, player.transform.localPosition.y, nextCell.Z);
				playerMovementsQueue.Enqueue(MoveOverSpeed(player, endPosition, 3f));
			}

			currentCell = nextCell;
			currentCellIndex = nextCellIndex;
		}

		playersOnBoard[player] = currentCellIndex;
		StartCoroutine(PlayerMovementsCoordinator());
	}

	IEnumerator PlayerMovementsCoordinator() {
		while (true) {
			while (playerMovementsQueue.Count > 0) {
				if (!isPlayerMove)
					yield return StartCoroutine(playerMovementsQueue.Dequeue());
				else
					yield return null;
			}
			
			yield return null;
		}
    }

	// refactor
	public IEnumerator MoveOverSeconds (Player objectToMove, Vector3 end, float seconds) {
		isPlayerMove = true;
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds) {
			Debug.Log($"Moving to  {end}");
            objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = end;
		isPlayerMove = false;
    }

    public IEnumerator MoveOverSpeed(Player objectToMove, Vector3 end, float speed) {
		isPlayerMove = true;
		while (objectToMove.transform.position != end)
		{
			objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, end, speed * Time.deltaTime);
			yield return new WaitForEndOfFrame ();
		}
		isPlayerMove = false;
	}

	private static Vector2Int[] cellMap = {
		new Vector2Int(0, 0),
		new Vector2Int(0, 4),
		new Vector2Int(3, 4),
		new Vector2Int(3, 8),
		new Vector2Int(9, 8),
		new Vector2Int(9, 3),
		new Vector2Int(6, 3),
		new Vector2Int(6, 0)
	};

	private static Color[] cellColors = {
		Color.blue, Color.cyan, Color.yellow, Color.grey
	};

	private void InitMap()
	{
		Debug.Log($"cellMap count - {cellMap.Count()}");
		int colorIndex = 0;
		for (int i = 0; i < cellMap.Count(); i++) {
			var cell = cellMap[i];
			var nextCell = i == cellMap.Count()-1 ? cellMap[0] : cellMap[i+1];

			Debug.Log($"Drawing line from ({cell.x},{cell.y}) to ({nextCell.x},{nextCell.y})");
			int lineLength = Mathf.Abs((nextCell.x-cell.x) + (nextCell.y-cell.y));
			var step = new Vector2Int((nextCell.x-cell.x)/lineLength, (nextCell.y-cell.y)/lineLength);
			var curCell = cell;
			for (int j = 0; j < lineLength; j++) {
				bool isCorner = j == 0 || j == lineLength-1;
				CreateCell(curCell.x, curCell.y, isCorner, cellColors[colorIndex]);
				curCell += step;
				colorIndex = colorIndex == 3 ? 0 : colorIndex+1;
			}
		}
	}

	private void CreateCell (int x, int z, bool isCorner, Color color) {
		Debug.Log($"Creating cell ({x},{z})");
		GameBoardCell cell = Instantiate<GameBoardCell>(cellPrefab);
		cell.transform.SetParent(transform, false);
		cell.SetPosition(x, z);
		cell.IsCorner = isCorner;
		cell.Color = color;
		cells.Add(cell);
	}
}

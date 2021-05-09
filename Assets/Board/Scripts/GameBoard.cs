using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    private GameBoardMesh mesh;
	private List<Vector3> vertices;
	private List<int> triangles;

	[SerializeField]
	private GameBoardCell cellPrefab;

	[SerializeField]
	private Player playerPrefab;

	// add info about corner (simplify movement chunking)
	private List<GameBoardCell> cells = new List<GameBoardCell>();
	private Dictionary<Player, int> playersOnBoard = new Dictionary<Player, int>();

	// private static int cellCount = 5;

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
			Vector2Int moveDirection = new Vector2Int(
				nextCell.X-currentCell.X, nextCell.Z-currentCell.Z);

			Debug.Log($"Current cell = {currentCell.transform.localPosition}, next = {nextCell.transform.localPosition}");
			Debug.Log($"Last move v = {lastMoveDirection}, new = {moveDirection}");
			if (moveDirection != lastMoveDirection || i == cellMoves-1) 
			{
				Vector3 endPosition = moveDirection != lastMoveDirection && lastMoveDirection != Vector2Int.zero
					? new Vector3(currentCell.X, player.transform.localPosition.y, currentCell.Z)
					: new Vector3(nextCell.X, player.transform.localPosition.y, nextCell.Z);
				Debug.Log($"Enque movement to position  {endPosition}");
				playerMovementsQueue.Enqueue(MoveOverSpeed(player, endPosition, 3f));
			}

			currentCell = nextCell;
			currentCellIndex = nextCellIndex;
			lastMoveDirection = moveDirection;
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

	// replace with draw broken line methods (only angles)
	private static System.Tuple<int, int>[] cellMap = {
		new System.Tuple<int, int>(0, 0),
		new System.Tuple<int, int>(0, 1),
		new System.Tuple<int, int>(0, 2),
		new System.Tuple<int, int>(0, 3),
		new System.Tuple<int, int>(0, 4),
		new System.Tuple<int, int>(1, 4),
		new System.Tuple<int, int>(2, 4),
		new System.Tuple<int, int>(3, 4),
		new System.Tuple<int, int>(4, 4),
		new System.Tuple<int, int>(4, 3),
		new System.Tuple<int, int>(4, 2),
		new System.Tuple<int, int>(4, 1),
		new System.Tuple<int, int>(4, 0),
		new System.Tuple<int, int>(3, 0),
		new System.Tuple<int, int>(2, 0),
		new System.Tuple<int, int>(1, 0)
	};

	private void InitMap()
	{
		foreach (var coord in cellMap) {
			CreateCell(coord.Item1, coord.Item2);
		}
	}

	private void CreateCell (int x, int z) {
		GameBoardCell cell = Instantiate<GameBoardCell>(cellPrefab);
		cell.transform.SetParent(transform, false);
		cell.SetPosition(x, z);
		cells.Add(cell);
	}
}

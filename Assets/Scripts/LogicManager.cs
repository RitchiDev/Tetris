using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicManager : MonoBehaviour, ILogicManager
{

	public enum GameState
	{
		None, PreGame, InPlay, Paused, GameOver
	}

	public delegate void GameStateChangeEventHandler(GameState gameState);
	public delegate void GameUpdateEventHandler();

	public event GameStateChangeEventHandler GameStateChange;
	public event GameUpdateEventHandler GameUpdate;

	[SerializeField] private float m_TimeBeforeDrop = 1f;
	[SerializeField] private Vector2Int gridSize = new Vector2Int(10, 24);
	private List<int[,,]> m_PiecesInBag;

	[Header("Controls")]
	[SerializeField] private KeyCode moveLeftKey = KeyCode.LeftArrow;
	[SerializeField] private KeyCode moveRightKey = KeyCode.RightArrow;
	[SerializeField] private KeyCode softDropKey = KeyCode.DownArrow;
	[SerializeField] private KeyCode hardDropKey = KeyCode.Space;
	[SerializeField] private KeyCode rotateKey = KeyCode.UpArrow;
	[SerializeField] private KeyCode startGameKey = KeyCode.Space;
	[SerializeField] private KeyCode pauseGameKey = KeyCode.P;

    #region Pieces

    private int[,,] iPiece = new int[,,]
	{
		{
			{ 0, 0, 0, 0 },
			{ 1, 1, 1, 1 },
			{ 0, 0, 0, 0 },
			{ 0, 0, 0, 0 }
		}, {
			{ 0, 0, 1, 0 },
			{ 0, 0, 1, 0 },
			{ 0, 0, 1, 0 },
			{ 0, 0, 1, 0 }
		}
	};

	private int[,,] jPiece = new int[,,]
	{
		{
			{ 2, 0, 0 },
			{ 2, 2, 2 },
			{ 0, 0, 0 }
		}, {
			{ 0, 2, 2 },
			{ 0, 2, 0 },
			{ 0, 2, 0 }
		}, {
			{ 0, 0, 0 },
			{ 2, 2, 2 },
			{ 0, 0, 2 }
		}, {
			{ 0, 2, 0 },
			{ 0, 2, 0 },
			{ 2, 2, 0 }
		}
	};

	private int[,,] lPiece = new int[,,]
	{
		{
			{ 0, 0, 3 },
			{ 3, 3, 3 },
			{ 0, 0, 0 }
		}, {
			{ 0, 3, 0 },
			{ 0, 3, 0 },
			{ 0, 3, 3 }
		}, {
			{ 0, 0, 0 },
			{ 3, 3, 3 },
			{ 3, 0, 0 }
		}, {
			{ 3, 3, 0 },
			{ 0, 3, 0 },
			{ 0, 3, 0 }
		}
	};

	private int[,,] oPiece = new int[,,]
	{
		{
			{ 0, 4, 4, 0 },
			{ 0, 4, 4, 0 },
			{ 0, 0, 0, 0 }
		}
	};

	private int[,,] sPiece = new int[,,]
	{
		{
			{ 0, 5, 5 },
			{ 5, 5, 0 },
			{ 0, 0, 0 }
		}, {
			{ 0, 5, 0 },
			{ 0, 5, 5 },
			{ 0, 0, 5 }
		}, {
			{ 0, 0, 0 },
			{ 0, 5, 5 },
			{ 5, 5, 0 }
		}, {
			{ 5, 0, 0 },
			{ 5, 5, 0 },
			{ 0, 5, 0 }
		}
	};

	private int[,,] tPiece = new int[,,]
	{
		{
			{ 0, 6, 0 },
			{ 6, 6, 6 },
			{ 0, 0, 0 }
		}, {
			{ 0, 6, 0 },
			{ 0, 6, 6 },
			{ 0, 6, 0 }
		}, {
			{ 0, 0, 0 },
			{ 6, 6, 6 },
			{ 0, 6, 0 }
		}, {
			{ 0, 6, 0 },
			{ 0, 6, 0 },
			{ 0, 6, 0 }
		}
	};

	private int[,,] zPiece = new int[,,]
	{
		{
			{ 7, 7, 0 },
			{ 0, 7, 7 },
			{ 0, 0, 0 }
		}, {
			{ 0, 0, 7 },
			{ 0, 7, 7 },
			{ 0, 7, 0 }
		}, {
			{ 0, 0, 0 },
			{ 7, 7, 0 },
			{ 0, 7, 7 }
		}, {
			{ 0, 7, 0 },
			{ 7, 7, 0 },
			{ 7, 0, 0 }
		}
	};

	private int[][,,] allPieces;

	#endregion

    public Vector2Int GridSize => gridSize;
	public int[,] FixedPieces { get; private set; }
	public int[,,] ActivePiece { get; private set; }
	public Vector2Int ActivePiecePosition { get; private set; }
	public int ActivePieceRotation { get; private set; }
	public GameState CurrentGameState { get; private set; }
	public int Level => throw new System.NotImplementedException();
	public int Score => throw new System.NotImplementedException();

	private float m_DropTimeRemaining;

	public Vector2Int GetActivePieceHardDropPosition()
	{
		throw new System.NotImplementedException();
	}

	public int[,,] GetNextPieceInBag()
	{
		int[,,] newPiece = m_PiecesInBag[Random.Range(0, m_PiecesInBag.Count - 1)];
		m_PiecesInBag.Remove(newPiece);

		return newPiece;
	}

	private void Awake()
	{
		m_PiecesInBag = new List<int[,,]>();

		allPieces = new int[][,,]
		{
			iPiece,
			jPiece,
			lPiece,
			oPiece,
			sPiece,
			zPiece,
			tPiece
		};

		AddPiecesToBag();
		SpawnNewActivePiece();

		m_DropTimeRemaining = 1f;
	}

	private void AddPiecesToBag()
	{
		for (int i = 0; i < allPieces.GetLength(0); i++)
		{
			m_PiecesInBag.Add(allPieces[i]);
		}
	}

	private void SpawnNewActivePiece()
	{
		ActivePiece = GetNextPieceInBag();

		if(m_PiecesInBag.Count <= 0)
		{
			AddPiecesToBag();
		}

		Vector2Int spawnPos = new Vector2Int((GridSize.x + 1) / 2, GridSize.y - GetPieceSize(ActivePiece).y);
		ActivePiecePosition = spawnPos;
	}

	private void Start()
	{
		//ChangeGameState(GameState.None);
		ResetGame();

		Vector2Int Spawnpos = new Vector2Int((GridSize.x + 1) / 2, GridSize.y - GetPieceSize(ActivePiece).y);
		ActivePiecePosition = Spawnpos;
	}

	private void Update()
	{
		// use this for getkeydowns and piece auto fall counter

		switch (CurrentGameState)
		{
			case GameState.None:
				break;
			case GameState.PreGame:

				if (Input.GetKeyDown(startGameKey))
				{
					ChangeGameState(GameState.InPlay);
				}

				break;
			case GameState.InPlay:


				PieceVerticalPositionTimer();

				CheckForHorizontalMovement();

				CheckForActivePieceRotation();

				break;
			case GameState.Paused:
				break;
			case GameState.GameOver:
				break;
			default:
				break;
		}
	}

	private void Test()
	{

	}

	private void ResetGame()
	{
		FixedPieces = new int[gridSize.y, gridSize.x];
		ChangeGameState(GameState.PreGame);
	}

	private void CheckForHorizontalMovement()
	{
		if (Input.GetKeyDown(moveLeftKey))
		{
			if (!HasOverlap(ActivePiece, ActivePieceRotation, ActivePiecePosition + Vector2Int.left))
			{
				ActivePiecePosition += Vector2Int.left;
				GameUpdate?.Invoke();
			}
		}

		if (Input.GetKeyDown(moveRightKey))
		{
			if (!HasOverlap(ActivePiece, ActivePieceRotation, ActivePiecePosition + Vector2Int.right))
			{
				ActivePiecePosition += Vector2Int.right;
				GameUpdate?.Invoke();
			}
		}
	}

	private void PieceVerticalPositionTimer()
	{
		m_DropTimeRemaining -= Time.deltaTime;

		if (m_DropTimeRemaining <= 0)
		{
			if(!HasOverlap(ActivePiece, ActivePieceRotation, ActivePiecePosition + Vector2Int.down))
			{
				ActivePiecePosition += Vector2Int.down;

			}

			m_DropTimeRemaining = m_TimeBeforeDrop;
			GameUpdate?.Invoke();
		}
	}

	private void CheckForActivePieceRotation()
	{
		if (Input.GetKeyDown(rotateKey))
		{
			int newRotation = ActivePieceRotation + 1;
			if(newRotation >= ActivePiece.GetLength(0))
			{
				newRotation = 0;
			}

			if (!HasOverlap(ActivePiece, newRotation, ActivePiecePosition))
			{
				ActivePieceRotation = newRotation;
				GameUpdate?.Invoke();
			}
		}
	}

	private void ChangeGameState(GameState gameState)
	{
		if (CurrentGameState != gameState)
		{
			CurrentGameState = gameState;
			GameStateChange?.Invoke(CurrentGameState);
			Debug.Log("State is veranderd!");
		}
		Debug.Log(gameState);
	}

	private Vector2Int GetPieceSize(int[,,] piece)
	{
		return new Vector2Int(piece.GetLength(2), piece.GetLength(1));
	}

	private bool HasOverlap(int[,,] piece, int pieceRotation, Vector2Int position)
	{
		Vector2Int pieceSize = GetPieceSize(piece);
		for (int x = 0; x < pieceSize.x; x++)
		{
			for (int y = 0; y < pieceSize.y; y++)
			{
				if(piece[pieceRotation, y, x] > 0)
				{
					
					if(position.x + x < 0 || position.x + x > gridSize.x - 1)
					{
						return true;
					}
					if (position.y + pieceSize.y - y < 0)
					{
						return true;
					}
				}
				
			}
		}

		return false;
	}

}

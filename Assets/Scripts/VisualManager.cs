using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualManager : MonoBehaviour
{
    private SpriteRenderer[,] m_FixedTiles;
    [SerializeField] private Sprite m_DefaultTileSprite;
    [SerializeField] private Color m_FieldTileColor;
    [SerializeField] private Color m_GhostTileColor;
    [SerializeField] private Color[] m_TileColors;
    private LogicManager logicManager;

    private void Start()
    {
        logicManager = FindObjectOfType<LogicManager>();
        logicManager.GameUpdate += HandleGameUpdate;
        logicManager.GameStateChange += HandleGameStateChange;

        m_FixedTiles = new SpriteRenderer[logicManager.GridSize.y, logicManager.GridSize.x];
        for (int y = 0; y < logicManager.GridSize.y; y++)
        {
            for (int x = 0; x < logicManager.GridSize.x; x++)
            {
                m_FixedTiles[y, x] = new GameObject(y + "," + x).AddComponent<SpriteRenderer>();
                m_FixedTiles[y, x].sprite = m_DefaultTileSprite;
                m_FixedTiles[y, x].color = m_FieldTileColor;
                m_FixedTiles[y, x].transform.position = new Vector2(x, y);
            }
        }
    }


    private void HandleGameStateChange(LogicManager.GameState gameState)
    {
        switch (gameState)
        {
            case LogicManager.GameState.None:
                break;
            case LogicManager.GameState.PreGame:

                //List<SpriteRenderer> fieldTiles;
                //for (int y = 0; y < logicManager.GridSize.y; y++)
                //{
                //    for (int x = 0; x < logicManager.GridSize.x; x++)
                //    {
                //        SpriteRenderer fieldTile = new GameObject(y + "," + x).AddComponent<SpriteRenderer>();
                //        //fieldTiles.Add(fieldTile);

                //        fieldTile.sprite = m_DefaultTileSprite;
                //        fieldTile.color = m_FieldTileColor;
                //        fieldTile.transform.position = new Vector2(x, y);
                //    }
                //}

                //m_FixedTiles = fieldTiles;

                break;
            case LogicManager.GameState.InPlay:
                break;
            case LogicManager.GameState.Paused:
                break;
            case LogicManager.GameState.GameOver:
                break;
            default:
                break;
        }
    }

    private void UpdateFixedPieces()
    {
        for (int x = 0; x < m_FixedTiles.GetLength(1); x++)
        {
            for (int y = 0; y < m_FixedTiles.GetLength(0); y++)
            {
                m_FixedTiles[y, x].color = m_TileColors[logicManager.FixedPieces[y, x]];
            }
        }
    }

    private void UpdateActivePiece()
    {
        for (int y = 0; y < logicManager.ActivePiece.GetLength(1); y++)
        {
            for (int x = 0; x < logicManager.ActivePiece.GetLength(2); x++)
            {
                //Debug.Log(y + "," + x);
                //Debug.Log("Active Piece: " + logicManager.ActivePiecePosition.y + "," + logicManager.ActivePiecePosition.x);

                int tileColor = logicManager.ActivePiece[logicManager.ActivePieceRotation, y, x];

                if (tileColor != 0)
                {
                    SpriteRenderer tile = m_FixedTiles[logicManager.ActivePiecePosition.y + logicManager.ActivePiece.GetLength(1) - y, logicManager.ActivePiecePosition.x + x];
                    tile.color = m_TileColors[tileColor];
                }
            }
        }
    }

    private void HandleGameUpdate()
    {
        UpdateFixedPieces();

        UpdateActivePiece();
    }
}

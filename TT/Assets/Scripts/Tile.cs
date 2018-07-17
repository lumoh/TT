using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// tiles represent areas on a board which are layed out like a grid
/// </summary>
public class Tile : MonoBehaviour {

    private Board _board;
    public Board Board { get { return _board; } }

    private int _x;
    public int X { get { return _x; } }

    private int _y;
    public int Y { get { return _y; } }

    private int NUM_LAYERS = 5;
    private BoardObject[] _boardObjects;
    private Spawner[] _spawners;

    public void Init(Board board, int x, int y)
    {
        _board = board;
        _boardObjects = new BoardObject[NUM_LAYERS];
        _spawners = new Spawner[NUM_LAYERS];
        _x = x;
        _y = y;

        transform.SetParent(_board.transform);
        transform.localPosition = new Vector3(_x, -_y);
        gameObject.name = "Tile[" + _x.ToString() + "," + _y.ToString() + "]";
    }

    /// <summary>
    /// Adds the board object.
    /// </summary>
    /// <param name="boardObject">Board object.</param>
    /// <param name="layer">Layer.</param>
    public void AddBoardObject(BoardObject boardObject)
    {
        if (_boardObjects != null && boardObject.TileLayer >= 0 && boardObject.TileLayer < NUM_LAYERS)
        {
            boardObject.MyTile = this;
            boardObject.transform.SetParent(transform);
            boardObject.transform.localPosition = Vector3.zero;
            _boardObjects[boardObject.TileLayer] = boardObject;
        }
    }

    /// <summary>
    /// Removes the board object.
    /// </summary>
    /// <param name="layer">Layer.</param>
    public void RemoveBoardObject(int layer)
    {
        if (_boardObjects != null && layer >= 0 && layer < NUM_LAYERS)
        {
            _boardObjects[layer] = null;
        }
    }

    public BoardObject GetBoardObect(int layer)
    {
        BoardObject boardObject = null;
        if (_boardObjects != null && layer >= 0 && layer < NUM_LAYERS)
        {
            boardObject = _boardObjects[layer];
        }
        return boardObject;
    }

    public bool IsOccupied(int layer)
    {
        bool flag = false;
        if (_boardObjects != null && layer >= 0 && layer < NUM_LAYERS)
        {
            BoardObject boardObject = _boardObjects[layer];
            if (boardObject != null)
            {
                flag = true;
            }
        }
        return flag;
    }

    public bool HasFallingBlock(int layer)
    {
        bool flag = false;
        if (_boardObjects != null && layer >= 0 && layer < NUM_LAYERS)
        {
            BoardObject boardObject = _boardObjects[layer];
            if (boardObject != null && boardObject.GetComponent<FallingObject>() != null)
            {
                flag = true;
            }
        }
        return flag;
    }

    public void AddSpawner(Spawner spawner)
    {
        if (_spawners != null && spawner.TileLayer >= 0 && spawner.TileLayer < NUM_LAYERS)
        {
            spawner.MyTile = this;
            spawner.transform.SetParent(transform);
            _spawners[spawner.TileLayer] = spawner;
        }
    }

    public Spawner GetSpawner(int layer)
    {
        Spawner spawner = null;
        if (_spawners != null && layer >= 0 && layer < NUM_LAYERS)
        {
            spawner = _spawners[layer];
        }
        return spawner;
    }

    public override string ToString()
    {
        return string.Format("[Tile: X={0}, Y={1}]", X, Y);
    }
}

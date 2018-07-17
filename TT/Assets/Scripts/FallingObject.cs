using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardObject))]
public class FallingObject : MonoBehaviour 
{
    public float Gravity = 15.0f;
    private Vector3 _velocity = Vector3.zero;
    private BoardObject _boardObject;
    private Collider2D _collider;
    private FallDir _fallDirection;
    [SerializeField] private GameObject SpriteObj;

	// Use this for initialization
	void Start ()
    {
        _boardObject = GetComponent<BoardObject>();
        _collider = GetComponent<Collider2D>();
        _boardObject.State = BoardObjectState.FALLING;
        _fallDirection = FallDir.SOUTH;
	}
	
	// Update is called once per frame
	void Update()
    {
        // board object is settled squarely on its tile
        if (_boardObject.State == BoardObjectState.SETTLED)
        {            
            // if tile below becomes unoccupied, start falling again!
            Tile tileBelow = getTileAdjacent(0, -1);
            if (tileBelow != null && !tileBelow.IsOccupied(_boardObject.TileLayer))
            {
                _boardObject.State = BoardObjectState.FALLING;
                _fallDirection = FallDir.SOUTH;
            }
        }
        // the board object is falling vertically down
        else if (_boardObject.State == BoardObjectState.FALLING)
        {
            // if the board object has fallen further than the targeted tile
            if (_boardObject.transform.position.y <= _boardObject.MyTile.transform.position.y)
            {
                Tile targetTile = getTileForDirFalling(_fallDirection);
                if (targetTile == null || (targetTile != null && targetTile.IsOccupied(_boardObject.TileLayer)))
                {
                    _velocity = Vector3.zero;
                    _boardObject.transform.position = _boardObject.MyTile.transform.position;
                    _boardObject.State = BoardObjectState.SETTLED;
                }
                // if not blocked then move to it
                else if (targetTile != null)
                {
                    _boardObject.MyTile.RemoveBoardObject(_boardObject.TileLayer);
                    targetTile.AddBoardObject(_boardObject);
                    advanceFallingObject();
                }
            }
            else
            {
                advanceFallingObject();
            }
        }
	}

    /// <summary>
    /// Advances the falling object.
    /// </summary>
    private void advanceFallingObject()
    {
        if (_fallDirection == FallDir.SOUTH)
        {
            _velocity += new Vector3(0, -Gravity, 0) * Time.deltaTime;
        }

        Vector3 newPos = transform.position + (_velocity * Time.deltaTime);
        if (newPos.y < _boardObject.MyTile.transform.position.y)
        {
            newPos = _boardObject.MyTile.transform.position;
        }
        transform.position = newPos;
    }

    private Tile getTileForDirFalling(FallDir direction)
    {
        Tile tile = null;
        if (direction == FallDir.SOUTH)
        {
            tile = getTileAdjacent(0, -1);
        }
        return tile;
    }

    private Tile getTileAdjacent(int xoffset, int yoffset)
    {
        Tile tileBelow = null;
        int x = _boardObject.X + xoffset;
        int y = _boardObject.Y + yoffset;
        tileBelow = _boardObject.MyBoard.GetTile(x, y);
        return tileBelow;
    }
}

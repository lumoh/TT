using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapController : MonoBehaviour 
{
    public bool Horizontal;
    public bool Vertical;

    private Board _board;
    private Camera _camera;
    private Vector3 _origin;
    private bool _down;
    private BoardObject _blockToSwap = null;

	// Use this for initialization
    void Awake()
    {
        _board = GetComponent<Board>();	
        _camera = _board.BoardCamera;
        _down = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(_board != null)
            {
                _board.SpeedUp(true);
            }
        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
            if(_board != null)
            {
                _board.SpeedUp(false);
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            _origin = _camera.ScreenToWorldPoint(Input.mousePosition);
            _blockToSwap = null;

            RaycastHit2D[] hits = Physics2D.RaycastAll(_origin, -Vector2.zero, Mathf.Infinity);
            for(int i = 0; i < hits.Length; i++)
            {
                RaycastHit2D hit = hits[i];
                if (hit && hit.transform.tag == "Block")
                {
                    BoardObject bo = hit.transform.GetComponent<BoardObject>();
                    if (bo != null)
                    {
                        _blockToSwap = bo;
                        _down = true;
                    }
                }
            }
        }

        if(_down && _blockToSwap != null && _board != null)
        {
            Vector3 current = _camera.ScreenToWorldPoint(Input.mousePosition);
            if(Horizontal)
            {
                float delta = Mathf.Abs(current.x - _origin.x);
                if(delta > 0.3f)
                {
                    if(current.x < _origin.x)
                    {
                        _board.Swap(_blockToSwap.MyTile, _board.GetTile(_blockToSwap.X - 1, _blockToSwap.Y));
                    }
                    else
                    {
                        _board.Swap(_blockToSwap.MyTile, _board.GetTile(_blockToSwap.X + 1, _blockToSwap.Y));
                    }
                    _down = false;
                    _blockToSwap = null;
                }
            }

            if(Vertical)
            {
                float delta = Mathf.Abs(current.y - _origin.y);
                if(delta > 0.3f)
                {
                    if(current.y < _origin.y)
                    {
                        _board.Swap(_blockToSwap.MyTile, _board.GetTile(_blockToSwap.X, _blockToSwap.Y + 1));
                    }
                    else
                    {
                        _board.Swap(_blockToSwap.MyTile, _board.GetTile(_blockToSwap.X, _blockToSwap.Y - 1));
                    }
                    _down = false;
                    _blockToSwap = null;
                }
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            _down = false;
            _blockToSwap = null;
        }
	}
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// object that resides on a tile on the board
/// </summary>
public class BoardObject : MonoBehaviour 
{
    [HideInInspector] public Tile MyTile;
    public Board MyBoard { get { return MyTile.Board; } }

    public int X
    {
        get 
        {
            int x = -1;
            if (MyTile != null)
            {
                x = MyTile.X;
            }
            return x;
        }
    }

    public int Y
    {
        get 
        {
            int y = -1;
            if (MyTile != null)
            {
                y = MyTile.Y;
            }
            return y;
        }
    }

    [Header("Board Settings")]
    public int TileLayer;

    public BoardObjectState State = BoardObjectState.SETTLED;

    [SerializeField] private BoardObjectColor _color = BoardObjectColor.NONE;
    public BoardObjectColor Color 
    { 
        get { return _color; } 
        set 
        {
            _color = value; 
            setSpriteForColor();
        }
    }

    [SerializeField] private BoardObjectType _type = BoardObjectType.NONE;
    public BoardObjectType Type 
    {       
        get { return _type; } 
        set { /* TODO */ }
    }

    [SerializeField] public bool Swappable = true;
    [SerializeField] public bool Matchable = true;
    private bool _active = true;

    [HideInInspector] public int LastMove = 0;

    public float SwapSpeed = 0.2f;

    private Vector3 _velocity;

    [HideInInspector] public int SwapCount = 0;

    [Header("Sprite Settings")]
    public SpriteRenderer MySpriteRenderer;   
    public Sprite NONE;
    public Sprite RED;
    public Sprite ORANGE;
    public Sprite YELLOW;
    public Sprite GREEN;
    public Sprite BLUE;
    public Sprite PURPLE;

	// Use this for initialization
	public virtual void Start() 
    {
        State = BoardObjectState.SETTLED;
        GameEventManager.RegisterForEvent(GameEventType.GAME_LOST, handleLoss);
        GameEventManager.RegisterForEvent(GameEventType.GAME_WON, handleWon);
	}

    public virtual void Init(BoardObjectColor c, int tileLayer)
    {
        TileLayer = tileLayer;
        Color = c;
    }

    public bool CanSwap()
    {
        bool settled = State == BoardObjectState.SETTLED;
        return Swappable && settled && _active;
    }

    public bool CanMatch()
    {
        bool settled = State == BoardObjectState.SETTLED;
        return Matchable && settled && _active;
    }

    public void Swap(Tile t)
    {
        SwapCount++;
        State = BoardObjectState.SWAPPING;
        LeanTween.move(gameObject, t.transform.position, 0.1f).setOnComplete(() =>
        {
            State = BoardObjectState.FALLING;
        });
    }

    public virtual void Break(float animDelay)
    {
        SetActive(false);
        State = BoardObjectState.BREAKING;

        LeanTween.delayedCall(gameObject, animDelay, () =>
        {
            gameObject.SetActive(false);
            GameEventManager.TriggerEvent(GameEventType.BREAK_BLOCK, this);

            LeanTween.delayedCall(gameObject, 0.15f, () =>
            {
                MyTile.RemoveBoardObject(TileLayer);
                Destroy(gameObject);
            });
        });            
    }

    private void breakParticles()
    {
        GameObject Prefab = Resources.Load<GameObject>("BreakParticles");
        GameObject obj = Instantiate(Prefab);
        if (obj != null)
        {
            obj.transform.position = transform.position;
        }
    }

    private void handleWon(object param)
    {
        SetActive(false);
    }

    private void handleLoss(object param)
    {
        SetActive(false);
    }

    public virtual void OnDestroy()
    {
        LeanTween.cancel(gameObject);
    }

    public void SetActive(bool active)
    {
        Color c = MySpriteRenderer.color;
        if(!active)
        {
            c.a = 0.25f;
        }
        else
        {
            c.a = 1f;
        }
        MySpriteRenderer.color = c;
        _active = active;
    }

    protected virtual void setSpriteForColor()
    {
        if(MySpriteRenderer != null)
        {
            if(_color == BoardObjectColor.RED)
            {
                MySpriteRenderer.sprite = RED;
            }
            else if(_color == BoardObjectColor.ORANGE)
            {
                MySpriteRenderer.sprite = ORANGE;
            }
            else if(_color == BoardObjectColor.YELLOW)
            {
                MySpriteRenderer.sprite = YELLOW;
            }
            else if(_color == BoardObjectColor.GREEN)
            {
                MySpriteRenderer.sprite = GREEN;
            }
            else if(_color == BoardObjectColor.BLUE)
            {
                MySpriteRenderer.sprite = BLUE;
            }
            else if(_color == BoardObjectColor.PURPLE)
            {
                MySpriteRenderer.sprite = PURPLE;
            }
        }
    }

    void Update()
    {
        if(MyTile != null)
        {
            if(transform.position.y > MyTile.transform.position.y)
            {
                State = BoardObjectState.FALLING;
            }

            // board object is settled squarely on its tile
            if(State == BoardObjectState.SETTLED)
            {
                // if tile below becomes unoccupied, start falling again!
                Tile tileBelow = getTileAdjacent(0, 1);
                if(tileBelow != null && !tileBelow.IsOccupied(TileLayer))
                {
                    State = BoardObjectState.FALLING;
                }
            }
            // the board object is falling vertically down
            else if(State == BoardObjectState.FALLING)
            {
                // if the board object has fallen further than the targeted tile
                if(transform.position.y <= MyTile.transform.position.y)
                {
                    Tile targetTile = getTileAdjacent(0, 1);
                    if(targetTile == null || (targetTile != null && targetTile.IsOccupied(TileLayer)))
                    {
                        _velocity = Vector3.zero;
                        transform.position = MyTile.transform.position;
                        State = BoardObjectState.SETTLED;

                        if(!isBlockAbove())
                        {
                            MyBoard.BreakMatches();
                        }

                        resetSwapCount();
                    }
                    // if not blocked then move to it
                    else if(targetTile != null)
                    {
                        MyTile.RemoveBoardObject(TileLayer);
                        targetTile.AddBoardObject(this, false);
                        advanceFallingObject();
                    }
                }
                else
                {
                    advanceFallingObject();
                }
            }
        }
    }

    private void resetSwapCount()
    {
        if(_active)
        {
            SwapCount = 0;
        }
    }

    private void advanceFallingObject()
    {
        _velocity += new Vector3(0, -3f, 0) * Time.deltaTime;
        Vector3 newPos = transform.position + _velocity;
        if (newPos.y < MyTile.transform.position.y)
        {
            newPos = MyTile.transform.position;
        }
        transform.position = newPos;
    }

    private bool isBlockAbove()
    {
        bool flag = false;
        for(int y = Y; y >= 0; y--)
        {
            Tile tile = MyBoard.GetTile(X, y);
            if(tile != null)
            {
                BoardObject bo = tile.GetBoardObect(TileLayer);
                if(bo != null && bo.State == BoardObjectState.FALLING)
                {
                    flag = true;
                    break;
                }
            }
        }
        return flag;
    }

    private Tile getTileAdjacent(int xoffset, int yoffset)
    {
        Tile tileBelow = null;
        int x = X + xoffset;
        int y = Y + yoffset;
        tileBelow = MyBoard.GetTile(x, y);
        return tileBelow;
    }
}

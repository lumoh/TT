using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawner.
/// </summary>
public class Spawner : BoardObject 
{
    private string _spawnType;

	public override void Start() 
    {
        State = BoardObjectState.NONE;
	}

    public void Init(Tile tile, string spawnType, int layer)
    {
        MyTile = tile;
        _spawnType = spawnType;
        TileLayer = layer;
        transform.SetParent(tile.transform);
        transform.localPosition = Vector3.up;
    }

    public void Update()
    {
        if(canSpawn())
        {
            spawnBoardObject();
        }
    }

    private bool canSpawn()
    {
        bool flag = false;
        if(MyTile != null && !MyTile.IsOccupied(TileLayer))
        {
            flag = true;
        }
        return flag;
    }

    private void spawnBoardObject()
    {
        GameObject BoardObjectPrefab = Resources.Load<GameObject>(_spawnType);
        if (BoardObjectPrefab != null)
        {
            GameObject spawnedObject = Instantiate(BoardObjectPrefab);
            if (spawnedObject != null)
            {
                BoardObject boardObject = spawnedObject.GetComponent<BoardObject>();
                if (boardObject != null)
                {
                    boardObject.Init(ColorUtil.PickRandom(MyBoard.NumColors), TileLayer);
                    MyBoard.AddBoardObject(boardObject, X, Y, false);
                    boardObject.transform.position = transform.position;
                    boardObject.transform.localScale = Vector3.zero;
                    LeanTween.scale(boardObject.gameObject, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutSine);
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour 
{
    public Board BoardPrefab;
    public BoardCamera BoardCamPrefab;

    private Board _board;
    private BoardCamera _boardCamera;
    	
	void Start() 
    {
        GameEventManager.RegisterForEvent(GameEventType.RESTART, handleRestart);

        Application.targetFrameRate = 60;
        LeanTween.init(1000);

        spawnBoard();
	}

    private void spawnBoard()
    {
        if(BoardPrefab != null && BoardCamPrefab != null)
        {
            _board = Instantiate(BoardPrefab, transform);
            _boardCamera = Instantiate(BoardCamPrefab, transform);
            _board.Init(_boardCamera);
        }
    }

    private void handleRestart(object param)
    {
        Destroy(_board.gameObject);
        Destroy(_boardCamera.gameObject);

        spawnBoard();
    }

    void OnDestroy()
    {
        GameEventManager.UnRegisterForEvent(GameEventType.RESTART, handleRestart);
    }
}

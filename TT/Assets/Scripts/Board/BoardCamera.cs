using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCamera : MonoBehaviour 
{
    private Board _board;
    [HideInInspector] public Camera cam;

    private GameObject _topCover;
    private GameObject _bottomCover;

    public void Init(Board board)
    {
        _board = board;
        cam = GetComponent<Camera>();

        if(_board != null)
        {
            float magicNum = 4.875f;
            Camera cam = GetComponent<Camera>();
            float height = Screen.height;
            float width = Screen.width;
            float orthoSize = height / width * magicNum;           
            cam.orthographicSize = orthoSize;

            transform.localPosition = new Vector3(-0.5f + ((float)_board.Width / 2f - 1.5f), ((float)_board.Height - 1f) / -2f, -10);

            spawnBoardCover();
        }
    }

    private void spawnBoardCover()
    {
        GameObject coverPrefab = Resources.Load<GameObject>("BoardCover");
        _topCover = Instantiate(coverPrefab);
        if(_topCover != null)
        {
            _topCover.transform.parent = _board.transform.parent;
            _topCover.transform.localScale = new Vector3(_board.Width, 10, 1);
            _topCover.transform.localPosition = new Vector3(((float)_board.Width - 1) / 2f, 5.5f, 1);
        }

        _bottomCover = Instantiate(coverPrefab);
        if(_bottomCover != null)
        {
            _bottomCover.transform.parent = _board.transform.parent;
            _bottomCover.transform.localScale = new Vector3(_board.Width, 10, 1);
            _bottomCover.transform.localPosition = new Vector3(((float)_board.Width - 1) / 2f, -_board.Height - 4.5f, 1);
        }
    }

    void OnDestroy()
    {
        Destroy(_topCover);
        Destroy(_bottomCover);
    }
}

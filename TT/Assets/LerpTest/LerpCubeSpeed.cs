using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpCubeSpeed : MonoBehaviour 
{   
    public Vector3 EndPos;
    public float Speed;

    private float _distance;
    private Vector3 _dir;
    private Vector3 _startPos;


	// Use this for initialization
	void Start() 
    {        
        _startPos = transform.position;
        _dir = EndPos - _startPos;
        _distance = Vector3.Distance(EndPos, _startPos);
	}
	
	// Update is called once per frame
	void Update() 
    {
        Vector3 newPos = transform.position + (_dir * Speed * Time.deltaTime);
        float distTraveled = Vector3.Distance(newPos, _startPos);
        if(distTraveled >= _distance)
        {
            transform.transform.position = EndPos;
        }
        else
        {
            transform.position = newPos;               
        }
	}
}

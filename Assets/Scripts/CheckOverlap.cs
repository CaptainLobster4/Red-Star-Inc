using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CheckOverlap : MonoBehaviour
{

    private float gridSize = 0.125f;
    private float gridSpacing = 0.02f;

    void Start()
    {
        // create a grid of points (100?) on this collider
        // get a list of colliders that are colliding with this object, iterate through and get a list of other bullethole colliders
        // check each one and take out points from grid if they overlap
        // take that amount over the total and use that as percentage of progress number 

        // CreateGrid();  ,, grid points should be in a list
        // get list of overlapping bullet holes
        // iterate through and take out points from point list if they overlap
        // easy math, return percentage

        //Vector2[] points = CreateGrid();
    }

    void Update()
    {

    }

   //ector2[] CreateGrid()
  //{

  //}

}

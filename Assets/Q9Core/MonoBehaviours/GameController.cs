using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Q9Core;
using Q9Core.Data;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Entity[] testEntities;
    
    void Start()
    {
        GameManager.GameInitialize(Camera.main);

        foreach(Entity e in testEntities)
        {
            
            GameManager.EntityGraph.QueueNewEntity(e, 0);
        }
    }

    private void Update()
    {
        GameManager.Update();
    }

    private void FixedUpdate()
    {
        GameManager.TickUpdate();
    }
}

using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultithreadTest : MonoBehaviour
{
    public void Start()
    {
        Run();
    }

    private void Run()
    {
        for (int x = 0; x < 10; x++)
        {
            Task t = new Task(() =>
                {
                    for (int y = 0; y < 100; y++)
                    {
                        Debug.Log($"Count: {x} - {y} on thread {Thread.CurrentThread.ManagedThreadId}");
                    }
                }
            );
            t.Start();
        }
    }
}

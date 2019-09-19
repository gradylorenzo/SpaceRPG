using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Q9Core.Data;

namespace Q9Core.Data
{
    [Serializable]
    public class Entity
    {
        public bool b_reservedIndex;
        public string _name;
        public int _meshID;
        public int _materialID;
        public string _guid;
        public Q9EntityAttributes _baseAttributes;

        [HideInInspector]
        public Vector2 _spawnPosition;

        [HideInInspector]
        public Vector2 _currentPosition;

        [HideInInspector]
        public Vector3 _currentRotation;

        public int _target;

        public void ResetTarget()
        {
            _target = -1;
        }

        public void SetCurrentRotation(Vector3 r)
        {
            _currentRotation = r;
        }
    }

    [Serializable]
    public struct Q9EntityAttributes
    {
        public float rotationSpeed;
    }
}
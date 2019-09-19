using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Q9Core;
using Q9Core.Data;
using System;

namespace Q9Core
{
    public static class GameManager
    {
        #region Game Intialization

        /*This section handles the intitial loading of any data that needs to be handled at the start of the game,
        including entity libraries, etc.*/

        public static Camera camera { get; private set; }

        public static void GameInitialize(Camera cam)
        {
            camera = cam;
            Library.InitializeEntityLibrary();
            EntityGraph.Initialize();
        }

        #endregion

        #region Game Updates
        private static int tickCount = 0;

        public static int TickUpdate()
        {
            tickCount++;

            //Operations need to take place in this order
            //1) Entity Queue: Loop through addition queue
            EntityGraph.ProcessNewEntityQueue();
            //2) Entity Graph: Loop objects for transformations
            EntityGraph.ProcessTransformations();
            //3) Entity Messages: Loop through messages to apply effects
            //4) Entity Queue: Loop through removal queue
            EntityGraph.ProcessOldEntityQueue();
            //5) Process Input
            Control.ProcessInput();
            
            return tickCount;
        }

        public static void Update()
        {
            EntityGraph.Draw();
        }

        #endregion

        #region Object Libraries

        /*This section handles object library loading. It holds data such as meshes, materials, and entities that
        combine the two for specific ship setups.*/

        public static class Library
        {
            public static void InitializeEntityLibrary()
            {
                EntityLibraryPreset preset = Resources.Load<EntityLibraryPreset>("Library");
                Entities = preset.Entities;
                Meshes = preset.Meshes;
                Materials = preset.Materials;

                Debug.Log($"{Meshes.Length} meshes, {Materials.Length} materials, {Entities.Length} entities loaded.");
            }

            public static Entity[] Entities;
            public static Mesh[] Meshes;
            public static Material[] Materials;
        }

        #endregion

        #region Controls
        public static class Control
        {
            public static Vector3 wantedRotation = new Vector3();

            internal static void ProcessInput()
            {
                //RotationInput
                float input = Input.GetAxis("Horizontal")*2;
                Vector3 newWantedRotation = new Vector3(wantedRotation.x, wantedRotation.y + input, wantedRotation.z);
                Control.wantedRotation = newWantedRotation;
            }
        }
        #endregion

        #region Entity Graph

        /*This section serves to hold data for ships present in space, holding to the data-oriented design of the game.
        To view what is in the graph, a MonoBehaviour needs to render the entities, using the integers provided in the Q9Entity data
        to find meshes and materials in the Library*/

        /*Graph utilizes two lists as queues to perform the addition and removal of new entities at fixed intervals
         found in region "Entity Queues"*/

        public static class EntityGraph
        {
            //Index Reservations:
            //0: Player Ship
            //1-5: Multiplayer Ships
            public static List<Entity> graph = new List<Entity>();

            public static void Initialize()
            {
                for (int i = 0; i < 6; i++)
                {
                    Entity newEntity = new Entity();
                    newEntity._meshID = -1;
                    graph.Add(newEntity);
                }
                Debug.Log(graph.Count + " reserved indices on graph");
            }

            #region Entity Queues

            private struct QueuedEntity
            {
                public Entity entity { get; private set; }
                public int index { get; private set; }

                public QueuedEntity (Entity e, int i)
                {
                    entity = e;
                    index = i;
                }
            }

            //This is the queue for new entities, to be added to the graph at the beginning of each tick with ProcessNewEntityQueue();
            private static List<QueuedEntity> newEntities = new List<QueuedEntity>();
            public static void QueueNewEntity(Entity e, int i)
            {
                e._guid = Guid.NewGuid().ToString();
                QueuedEntity newEntity = new QueuedEntity(e, i);
                newEntities.Add(newEntity);

                Debug.Log($"Queued new entity with guid: {e._guid}");
            }

            //This is the queue for old entities, to be removed from the graph at the end of each tick with ProcessOldEntityQueue();
            private static List<QueuedEntity> oldEntities = new List<QueuedEntity>();
            public static void QueueOldEntity(Entity e, int i)
            {
                QueuedEntity oldEntity = new QueuedEntity(e, i);
                oldEntities.Add(oldEntity);
            }
            
            internal static void ProcessNewEntityQueue()
            {
                if (newEntities.Count > 0)
                {
                    foreach (QueuedEntity queued in newEntities)
                    {
                        
                        if (queued.index < 0)
                        {
                            graph.Add(queued.entity);
                        }
                        else
                        {
                            graph.Insert(queued.index, queued.entity);
                        }

                        Debug.Log($"Added entity with name {queued.entity._guid}");
                    }
                    newEntities.Clear();
                }
            }

            internal static void ProcessOldEntityQueue()
            {
                if (oldEntities.Count > 0)
                {
                    foreach (QueuedEntity queued in oldEntities)
                    {
                        if (graph.Count > queued.index)
                        {
                            if (graph[queued.index]._guid == queued.entity._guid)
                            {
                                foreach(Entity e in graph)
                                {
                                    if(e._target == queued.index)
                                    {
                                        e.ResetTarget();
                                    }
                                }
                                graph.RemoveAt(queued.index);
                            }
                        }
                    }

                    oldEntities.Clear();
                }
            }

            #endregion

            #region Transformations

            internal static void ProcessTransformations()
            {
                //Process the player's transformation first. Index is reserved as 0 in the graph.

                Vector3 currentRotation = graph[0]._currentRotation;
                
                float x = currentRotation.x;
                float y = currentRotation.y;
                y = Mathf.Lerp(y, Control.wantedRotation.y, graph[0]._baseAttributes.rotationSpeed);
                float z = currentRotation.z;

                Vector3 newRotation = new Vector3(x, y, z);

                Debug.Log($"{newRotation.x}  ::  {newRotation.y}  ::  {newRotation.z}");

                graph[0].SetCurrentRotation(newRotation);

                Debug.Log($"{graph[0]._currentRotation.x}  ::  {graph[0]._currentRotation.y}  ::  {graph[0]._currentRotation.z}");
                
                for (int i = 6; i < graph.Count; i++)
                {

                }
            }

            #endregion

            #region Message Queue

            //private List<Message> messages = new List<Messages>();

            #endregion

            #region Draw

            internal static void Draw()
            {
                foreach(Entity e in graph)
                {
                    if (e._meshID > -1)
                    {
                        Graphics.DrawMesh(Library.Meshes[e._meshID], e._currentPosition, Quaternion.Euler(e._currentRotation), Library.Materials[e._materialID], 0);
                    }
                }
            }

            
            #endregion
        }

        #endregion;
    }
}
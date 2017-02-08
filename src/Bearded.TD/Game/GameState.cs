﻿using System;
using System.Collections.Generic;
using Bearded.Utilities.Collections;
using Bearded.Utilities.SpaceTime;
using TimeSpan = Bearded.Utilities.SpaceTime.TimeSpan;

namespace Bearded.TD
{
    internal class GameState
    {
        public GameObject ObjectBeingAdded { get; private set; }

        private readonly DeletableObjectList<GameObject> gameObjects = new DeletableObjectList<GameObject>();
        private readonly Dictionary<Type, object> lists = new Dictionary<Type, object>();
        private readonly Dictionary<Type, object> singletons = new Dictionary<Type, object>();

        public EnumerableProxy<GameObject> GameObjects => gameObjects.AsReadOnlyEnumerable();

        public Instant Time { get; private set; } = Instant.Zero;

        public void Add(GameObject obj)
        {
            if (obj.Game != null)
                throw new Exception("Sad!");

            gameObjects.Add(obj);
            ObjectBeingAdded = obj;
            obj.Add(this);
            ObjectBeingAdded = null;
            // event on added
        }

        public void RegisterSingleton<T>(T obj)
            where T : class
        {
            singletons.Add(typeof(T), obj);
        }

        public T Get<T>()
            where T : class
        {
            object obj;
            singletons.TryGetValue(typeof(T), out obj);
            return (T)obj;
        }

        public void ListAs<T>(T obj)
            where T : class, IDeletable
        {
            getList<T>().Add(obj);
        }

        public EnumerableProxy<T> Enumerate<T>()
            where T : class, IDeletable
            => getList<T>().AsReadOnlyEnumerable();

        private DeletableObjectList<T> getList<T>()
            where T : class, IDeletable
        {
            object list;
            if (lists.TryGetValue(typeof(T), out list))
                return (DeletableObjectList<T>)list;

            var l = new DeletableObjectList<T>();
            lists.Add(typeof(T), l);
            return l;
        }



        public void Advance(TimeSpan elapsedTime)
        {
            Time += elapsedTime;

            foreach (var obj in gameObjects)
            {
                obj.Update(elapsedTime);
            }
        }
    }
}
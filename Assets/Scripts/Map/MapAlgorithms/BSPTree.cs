namespace DunGen
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BSPTree<T> : IMapGen<T> where T : class, IMap, new()
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _maxSize;
        private readonly int _minSize;
        private readonly int _maxRoom;
        private readonly int _minRoom;
        private System.Random _random;

        private T _map;
        private List<SubDungeon> _subdungeons = new List<SubDungeon>();

        public BSPTree(int width, int height, int maxLeaf, int minLeaf, int maxRoom, int minRoom, System.Random random)
        {
            _width = width;
            _height = height;
            _maxSize = maxLeaf;
            _minSize = minLeaf;
            _maxRoom = maxRoom;
            _minRoom = minRoom;
            _random = random;

            _map = new T();
        }

        public T GenerateMap()
        {
            _map.Initialize(_width, _height);
            _map.Clear(new Tile(Tile.Type.Block));

            SubDungeon rootLeaf = new SubDungeon(0, 0, _map.Width, _map.Height, _random);
            Debug.Log("Root subdungeon added");
            _subdungeons.Add(rootLeaf);

            bool splitSuccessfully = true;

            //Loop through all leaves until they can no longer split successfully
            while (splitSuccessfully)
            {
                splitSuccessfully = false;

                for (int i = 0; i < _subdungeons.Count; i++)
                {
                    if (_subdungeons[i].IsLeaf())
                    {
                        if ((_subdungeons[i].subDunWidth > _maxSize) || (_subdungeons[i].subDunHeight > _maxSize))
                        {
                            //Try to split the leaf
                            if (_subdungeons[i].Cut(_minSize))
                            {
                                _subdungeons.Add(_subdungeons[i].left);
                                Debug.Log("Left sub dungeon " + _subdungeons[i].left.debugId + " added successfully");
                                _subdungeons.Add(_subdungeons[i].right);
                                Debug.Log("Left sub dungeon " + _subdungeons[i].right.debugId + " added successfully");
                                splitSuccessfully = true;
                            }
                        }
                    }
                }
            }

            rootLeaf.CreateRooms<T>(this, _maxSize, _maxRoom, _minRoom);

            return _map;
        }

        public void CreateRoom(Rect room)
        {
            for (int x = (int)room.x + 1; x < room.max.x; x++)
            {
                for (int y = (int)room.y + 1; y < room.max.y; y++)
                {
                    _map.SetTile(x, y, new Tile(Tile.Type.Floor));
                }
            }
        }

        public void CreatePath(Rect room1, Rect room2)
        {
            //make a path between 2 rooms
            Vector2Int room1Center = Vector2Int.CeilToInt(room1.center);
            Vector2Int room2Center = Vector2Int.CeilToInt(room2.center);

            //Coin flip, doesn't matter what tunnel we make
            bool chance = Convert.ToBoolean(_random.Next(0, 2));
            if (chance)
            {
                MakeHorizontalTunnel(room1Center.x, room2Center.x, room1Center.y);
                MakeVerticalTunnel(room1Center.y, room2Center.y, room2Center.x);
            }
            else
            {
                MakeVerticalTunnel(room1Center.y, room2Center.y, room1Center.x);
                MakeHorizontalTunnel(room1Center.x, room2Center.x, room2Center.y);
            }
        }

        private void MakeHorizontalTunnel(int xStart, int xEnd, int yPosition)
        {
            for (int x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
            {
                _map.SetTile(x, yPosition, new Tile(Tile.Type.Hall));
            }
        }

        private void MakeVerticalTunnel(int yStart, int yEnd, int xPosition)
        {
            for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
            {
                _map.SetTile(xPosition, y, new Tile(Tile.Type.Hall));
            }
        }
    }
}

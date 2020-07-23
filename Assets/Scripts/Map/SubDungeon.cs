namespace DunGen
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Acts as the Leaf in BSPTree dungeon generation
    /// </summary>
    public class SubDungeon
    {
        public int subDunWidth;
        public int subDunHeight;
        public SubDungeon left
        {
            get; private set;
        }
        public SubDungeon right
        {
            get; private set;
        }
        private readonly System.Random _random;
        private readonly int _x;
        private readonly int _y;
        //TODO: Room object with tiles as well
        private Rect _room;
        private Rect _room1;
        private Rect _room2;

        public Rect room;
        public int debugId;
        public List<Rect> corridors = new List<Rect>();

        private static int debugCounter = 0;
        private static double tolerance = 1.25;
        // Constructor for the SubDungeon class, based on the section of the Map/Dungeon
        // to be used for the split
        public SubDungeon(int x, int y, int leafWidth, int leafHeight, System.Random random)
        {
            subDunWidth = leafWidth;
            subDunHeight = leafHeight;
            _x = x;
            _y = y;
            _random = random;
            debugId = debugCounter;
            debugCounter++;
        }

        // Checks to see if there are no child SubDungeons confined in this SubDungeon
        public bool IsLeaf()
        {
            return left == null && right == null;
        }

        // Splits area considered for the sub dungeon based on given room size limitations
        public bool Cut(int minSize)
        {
            if (!IsLeaf())
            {
                return false;
            }
            // Default value to avoid issues
            bool hCut = false;
            // If wider than a particular tolerance (usually 1.25), split vertically or horizontally
            if ((float)subDunWidth / subDunHeight >= tolerance)
            {
                hCut = false;
            }
            else if ((float)subDunHeight / subDunWidth >= tolerance)
            {
                hCut = true;
            }

            int max = 0;
            if (hCut)
            {
                max = subDunHeight - minSize;
            }
            else
            {
                max = subDunWidth - minSize;
            }

            // Checks to see if current sub area can be further divided into more, if not then this is a leaf and we are done with this section
            if (max <= minSize)
            {
                Debug.Log("Sub-Dungeon " + debugId + " is leaf");
                return false;
            }

            int cutPoint = _random.Next(minSize, max);

            // Cut current area horizontally on the randomly select cut point, the left child being the upper half, the right being the lower half
            // Upper half has a height equal to the size of cutPoint, lower has original sub area height minus cutPoint
            if (hCut)
            {
                left = new SubDungeon(_x, _y, subDunWidth, cutPoint, _random);
                right = new SubDungeon(_x, _y + cutPoint, subDunWidth, subDunHeight - cutPoint, _random);
            }
            else // Do the above but vice versa
            {
                left = new SubDungeon(_x, _y, cutPoint, subDunHeight, _random);
                right = new SubDungeon(_x + cutPoint, _y, subDunWidth - cutPoint, subDunHeight, _random);
            }
            return true;
        }

        public void CreateRooms<T>(BSPTree<T> generator, int maxSize, int maxRoom, int minRoom) where T: class, IMap, new()
        {
            Debug.Log(subDunWidth);
            if (!IsLeaf())
            {
                if(left != null)
                {
                    left.CreateRooms(generator, maxSize, maxRoom, minRoom);
                }
                if (right != null)
                {
                    right.CreateRooms(generator, maxSize, maxRoom, minRoom);
                }
                if (left != null && right != null)
                {
                    generator.CreatePath(left.GetRoom(), right.GetRoom());
                }
            }
            else
            {
                Debug.Log("Min Room Length: " + minRoom + " Max Room Length: " + maxRoom + " Leaf Width: " + (subDunWidth - 1));
                Debug.Log("Min Room Length: " + minRoom + " Max Room Length: " + maxRoom + " Leaf Width: " + (subDunHeight - 1));
                int roomWidth = _random.Next(minRoom, Math.Min(maxRoom, subDunWidth - 1));
                int roomHeight = _random.Next(minRoom, Math.Min(maxRoom, subDunHeight - 1));
                int rx = _random.Next(_x, _x + (subDunWidth - 1) - roomWidth);
                int ry = _random.Next(_y, _y + (subDunHeight - 1) - roomHeight);
                _room = new Rect(rx, ry, roomWidth, roomHeight);
                Debug.Log("Creating room: " + _room + " in sub-dungeon " + debugId);
                generator.CreateRoom(_room);
            }
        }

        /// <summary>
        /// retrieves the rooms, as represented by the rectangle for each area
        /// </summary>
        /// <returns>the Rect in this subDungeon that is a room</returns>
        private Rect GetRoom()
        {
            if (_room != Rect.zero)
            {
                return _room;
            }
            else
            {
                if (left != null)
                {
                    _room1 = left.GetRoom();
                }
                if (right != null)
                {
                    _room2 = right.GetRoom();
                }
            }

            if (IsLeaf())
            {
                return Rect.zero;
            }
            else if (_room2 == Rect.zero)
            {
                return _room1;
            }
            else if (_room1 == Rect.zero)
            {
                return _room2;
            }
            else if (Convert.ToBoolean(_random.Next(0, 2)))
            {
                return _room1;
            }
            else
            {
                return _room2;
            }
        }
    }

}

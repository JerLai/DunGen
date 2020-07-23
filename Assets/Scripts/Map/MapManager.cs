using System.Collections;
using System.Collections.Generic;
namespace DunGen
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class MapManager : MonoBehaviour
    {
        // Render specific variables
        [SerializeField] GameObject wallTile;
        [SerializeField] GameObject floorTile;
        [SerializeField] GameObject corridorTile;

        // For generic map parameters, inspector debug
        [SerializeField] int mapHeight, mapWidth;
        [SerializeField] int maxRoom, minRoom;
        [SerializeField] int mapType;
        private GameObject[,] mapPositionsFloor;
        private IMapGen<Map> generator;
        private IMap map;

        // For BSP generation parameters
        [SerializeField] int minSize, maxSize;
        [SerializeField] double tolerance;
        // Cave params
        // Room and Maze params

        private void Awake()
        {

        }
        // Start is called before the first frame update
        void Start()
        {
            /*
            mapHeight = 20;
            mapWidth = 20;
            maxRoom = 5;
            minRoom = 3;
            maxSize = 6;
            minSize = 4;
            mapType = 0;
            tolerance = 1.25;
            
            GenerateMap();
            */
        }

        // Update is called once per frame
        void Update()
        {

        }
        void GenerateMap()
        {
            mapPositionsFloor = new GameObject[mapWidth, mapHeight];
            if (mapType == 0)
            {
                Debug.Log("Line 49 in MapManager " + "Check maxSize: " + maxSize + " min: " + minSize + " width: " + mapWidth + " height: " + mapHeight);
                //BSP
                generator = new BSPTree<Map>(mapWidth, mapHeight, maxSize, minSize, maxRoom, minRoom, new System.Random(DateTime.Now.Millisecond));
                map = Map.Create(generator);
            } else if (mapType == 1)
            {
                //Cave
            } else if (mapType == 2)
            {
                //Maze
            }
            DrawMap();
        }
   
        public void Generate()
        {
            // Clean map for now
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    if (mapPositionsFloor[i, j] != null)
                    {
                        Destroy(mapPositionsFloor[i, j]);
                    }
                }
            }
            var inputFields = FindObjectsOfType<InputField>();
            bool goGenerate = true;
            for (int k = 0; k < inputFields.Length; k++)
            {
                InputField field = inputFields[k];
                string fieldHelp = field.placeholder.GetComponent<Text>().text;
                Debug.Log(fieldHelp);
                int value;

                bool canParse = Int32.TryParse(field.text, out value);
                if (canParse)
                {
                    Debug.Log("Parsed Value for " + field + ": " + value);
                    switch (fieldHelp)
                    {
                        case "Height Max 100":
                            mapHeight = value;
                            break;
                        case "Width Max 100":
                            mapWidth = value;
                            break;
                        case "Room Max 20":
                            maxRoom = value;
                            break;
                        case "Room Min 5":
                            minRoom = value;
                            break;
                        case "Divide Max 15":
                            maxSize = value;
                            break;
                        case "Divide Min 5":
                            minSize = value;
                            break;
                        default:
                            Debug.Log("Something wrong happened");
                            goGenerate = false;
                            break;

                    }
                }
                else
                {
                    Debug.Log("Error occurred with input, please try again");
                }

            }
            if(goGenerate && checkInputs() == true)
            {
                GenerateMap();
            }
        }

        private bool checkInputs()
        {
            return minSize > minRoom && minSize <= maxSize;
        }
        private void DrawMap()
        {
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    switch (map.GetTile(x, y).type)
                    {
                        case Tile.Type.Block:
                            {
                                GameObject newTile = GameObject.Instantiate(corridorTile, new Vector3(x, y, 1), Quaternion.identity);
                                newTile.transform.SetParent(transform);
                                mapPositionsFloor[x, y] = newTile;
                                break;
                            }
                        case Tile.Type.Floor:
                            {
                                GameObject newTile = GameObject.Instantiate(floorTile, new Vector3(x, y, 1), Quaternion.identity);
                                newTile.transform.SetParent(transform);
                                mapPositionsFloor[x, y] = newTile;
                                break;
                            }
                        case Tile.Type.Hall:
                            {
                                GameObject newTile = GameObject.Instantiate(floorTile, new Vector3(x, y, 1), Quaternion.identity);
                                newTile.transform.SetParent(transform);
                                mapPositionsFloor[x, y] = newTile;
                                break;
                            }
                    }
                }
            }
        }
        void BSPDraw(SubDungeon subDungeon)
        {
            if (subDungeon == null)
            {
                return;
            }
            if (subDungeon.IsLeaf())
            {
                for (int i = (int)subDungeon.room.x; i < subDungeon.room.xMax; i++)
                {
                    for (int j = (int)subDungeon.room.y; j < subDungeon.room.yMax; j++)
                    {
                        GameObject instance = Instantiate(floorTile, new Vector2(i, j), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(transform);
                        mapPositionsFloor[i, j] = instance;
                    }
                }
            }
            else
            {
                BSPDraw(subDungeon.left);
                BSPDraw(subDungeon.right);
            }
        }
        void BSPCorDraw(SubDungeon subDungeon)
        {
            if (subDungeon == null)
            {
                return;
            }

            BSPCorDraw(subDungeon.left);
            BSPCorDraw(subDungeon.right);

            foreach (Rect corridor in subDungeon.corridors)
            {
                for (int i = (int)corridor.x; i < corridor.xMax; i++)
                {
                    for (int j = (int)corridor.y; j < corridor.yMax; j++)
                    {
                        if (mapPositionsFloor[i, j] == null)
                        {
                            GameObject instance = Instantiate(corridorTile, new Vector2(i, j), Quaternion.identity) as GameObject;
                            instance.transform.SetParent(transform);
                            mapPositionsFloor[i, j] = instance;
                        }
                    }
                }
            }
        }
    }

}
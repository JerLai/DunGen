namespace DunGen
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    /// <summary>
    /// Interface for Generators to create a Map of a specific type
    /// ie, BSP map or Maze Map
    /// </summary>
    /// <typeparam name="T">type of Map to be created</typeparam>
    public interface IMapGen<T> where T : IMap
    {
        T GenerateMap();
    }

}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



/*
 * Class Generates and stores all of the terrains in the scene
 * 
 * 
 */
public class Generate_Terrain : MonoBehaviour {
    
    /// <summary>
    /// Width of a single tile in game units eg. 256
    /// </summary>
    public const int tile_width = 256;
    /// <summary>
    /// Height of a Single tile in game units eg. 256
    /// </summary>
    public const int tile_height = 256;

    public static string init_filename = @"Assets/sample_coins.txt";

    /// <summary>
    ///Width of the total map in game units eg 768 (256 x 3)
    /// </summary>
    public int map_width;

    /// <summary>
    ///Height of the total map in game units eg 768 (256 x 3)
    /// </summary>
    public int map_height;

    //2D list of all the terrains in the scene
    public GameObject[,] terrains;
    private TerrainData[,] terDatas;
    //The miniMap the corrisponds to those terrains

    public GameObject mainMap;

    //The basic data we'll use for all of our terrain tiles.
    private TerrainData terrainPrefab;
    /// <summary>
    ///The style of visual tiles we want to grab such as 'r' for road maps or 'a' for aerial maps
    /// </summary>
    public char map_style = 'r';


    // Use this for initialization
    void Start () {


        TerrainData mapTerr = new TerrainData
        {
            heightmapResolution = tile_width + 1,
            size = new Vector3(tile_width, 1, tile_height)
        };
        mapTerr.SetDetailResolution(1024, 8);
        mapTerr.baseMapResolution = 1024;
        mainMap = Terrain.CreateTerrainGameObject(mapTerr);
        mainMap.AddComponent<mapTile>();
        mainMap.GetComponent<mapTile>().SetupMapTile(4, mainMap.GetComponent<Terrain>(), 367, 683, 11);
        
        

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void loadLatLonZ(string filename, out float lat, out float lon, out int zoom)
    {
        lat = lon = zoom = 0;
        string[] fileTex = File.ReadAllLines(filename);

        foreach (string line in fileTex)
        {
            if (line.StartsWith("@Lat"))
            {
                string[] e = line.Split('\t');
                lat = float.Parse(e[1]);
            }
            else if (line.StartsWith("@Lon")){
                string[] e = line.Split('\t');
                lon = float.Parse(e[1]);
            }
            else if (line.StartsWith("@Zoom"))
            {
                string[] e = line.Split('\t');
                zoom = int.Parse(e[1]);
            }
        }

    }
}

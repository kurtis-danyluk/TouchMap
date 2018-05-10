using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class mapTile : MonoBehaviour {

    /// <summary>
    /// We'll change this to true anytime we want to reload the tile
    /// </summary>
    public bool hasChanged = false;


    /// <summary>
    /// The terrain object associated with this tile
    /// </summary>
    Terrain Terr;
    
    /// <summary>
    /// The X coordinate of the top left tile section in mercantor coordinates
    /// </summary>
    public int mercX;
    /// <summary>
    /// The Y coordinate of the top left tile section in mercantor coordinates
    /// </summary>
    public int mercY;
    /// <summary>
    /// The current zoom level of the total tile
    /// </summary>
    public int zoom;
    /// <summary>
    /// The detail level of the tile
    /// Described as tiles across
    /// Must be positive
    /// </summary>
    public int detail;

    /// <summary>
    /// Determines whether arial images or road images are drawn as tiles
    /// 'a' draws arial, 'r' draws road maps
    /// </summary>
    public char tileType;

    /// <summary>
    /// The relative scale of the map compared to its pieces
    /// </summary>
    float scale;

    /// <summary>
    /// The number of textures and heightmaps
    /// this si derived from detail
    /// </summary>
    public int pieces;

    /// <summary>
    /// The directory we'll store formated textures for this tile
    /// </summary>
    static string base_tex_dir = @"Assets/Textures/";

    /// <summary>
    /// This list will store all of the height maps used by our map tile
    /// </summary>
    List<Texture2D> heights;
    List<string> texNames;
    List<string> elvNames;

    /// <summary>
    /// Default constructor for a map tile. If no terrain is specfied the 
    /// </summary>
    /// <param name="detail"></param>
    /// <param name="mercX"></param>
    /// <param name="mercY"></param>
    /// <param name="zoom"></param>
    /// <param name="Terr"></param>
    public void SetupMapTile(int detail, Terrain Terr, int mercX = 1, int mercY = 1, int zoom = 1, char tileType = 'a')
    {
        this.detail = detail;
        Terr.heightmapPixelError = 32;
        this.mercX = mercX;
        this.mercY = mercY;
        this.zoom = zoom;
        scale = 1;

        if (Terr == null)
            throw new System.Exception("Terrain must be intitialized");
        this.Terr = Terr;
        this.name = "monoTile";

        this.tileType = tileType;

        pieces = (int)detail * (int)detail;
        heights = new List<Texture2D>(pieces);

        texNames = new List<string>(pieces);
        elvNames = new List<string>(pieces);


        Terr.terrainData.heightmapResolution = 256 * detail;
        Terr.terrainData.size = new Vector3(256 * detail * scale, 185.75f, 256 * detail * scale);
        float[,,] splatMapAlphas = new float[256*detail,256*detail, pieces];

        Terr.terrainData.alphamapResolution = 256 * detail;

        for (int i = 0; i < 256 * detail; i++)
            for (int j = 0; j < 256 * detail; j++)
                for (int k = 0; k < pieces; k++)
                    splatMapAlphas[i, j, k] = 0;

        SplatPrototype[] splats = new SplatPrototype[pieces];
        for (int i = 0; i < pieces; i++)
        {
            string fileName = base_tex_dir + i.ToString() + this.name + "tex.jpeg";
            texNames.Insert(i, fileName);
            if (!File.Exists(fileName))
            {
                File.WriteAllBytes(fileName, new byte[256 * 256]);
            }
            splats[i] = new SplatPrototype();
            Texture2D tex = new Texture2D(256, 256);
            tex.LoadImage(File.ReadAllBytes(fileName));
            splats[i].texture = tex;
            splats[i].tileSize = new Vector2(256, 256);
            splats[i].tileOffset = new Vector2((i % detail) * splats[i].tileSize.x, ((i / detail) * splats[i].tileSize.y));
            //for (float p = splats[i].tileOffset.x; p < splats[i].tileOffset.x + splats[i].tileSize.x; p++)

            for (int p = (int)(Terr.terrainData.alphamapResolution - 1 - splats[i].tileOffset.x) ; p >= (Terr.terrainData.alphamapResolution - splats[i].tileOffset.x) - splats[i].tileSize.x; p--)
                for (int q = (int)splats[i].tileOffset.y; q < splats[i].tileOffset.y + splats[i].tileSize.y; q++)
                {
                    splatMapAlphas[p, q, i] = 1;
                }

        }
        Terr.terrainData.splatPrototypes = splats;
        
        Terr.terrainData.SetAlphamaps(0, 0, splatMapAlphas);
        


        //

        for (int i = 0; i < pieces; i++)
        {
            string fileName = base_tex_dir + i.ToString() + this.name + "height.png";
            elvNames.Insert(i,fileName);
            if (!File.Exists(fileName))
            {
                File.WriteAllBytes(fileName, new byte[256 * 256]);
            }
            Texture2D hm = new Texture2D(256, 256);
            hm.LoadImage(File.ReadAllBytes(fileName));
            heights.Insert(i,hm);
        }

        //Create an appropriate heightmap 


        hasChanged = true;

    }
    
    // Use this for initialization
    void Start () {
        
	}

    // Update is called once per frame
    void Update()
    {

        if (hasChanged) {
            StartCoroutine(loadTile());
            hasChanged = false;
        }



    }

    IEnumerator loadTile(int LOD = -1)
    {
        //Grab a heightmap and throw it in our heights list

        //Grab a texture and throw it in our splats
        SplatPrototype[] nSplats = new SplatPrototype[pieces];
        for (int i = 0; i < pieces; i++)
        {
            int merc_lon = mercX + (i / detail);
            int merc_lat = mercY + (i % detail);
            int tZoom = zoom;// + (int)Mathf.Log((float)pieces, 4f);
            //Debug.Log("Grabbing tile " + merc_lon + " " + merc_lat + " " + tZoom);
            collect_tiles.dlImgFile(merc_lon, merc_lat, tZoom, texNames[i], tileType);
            Texture2D tex = new Texture2D(256, 256);
            tex.LoadImage(File.ReadAllBytes(texNames[i]));
            nSplats[i] = Terr.terrainData.splatPrototypes[i];
            nSplats[i].texture = tex;
            //Terr.terrainData.splatPrototypes[i].texture = tex;
            yield return null;
        }
        try
        {
            UnityEditor.AssetDatabase.Refresh();
            Terr.terrainData.splatPrototypes = nSplats;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }



        for (int i = 0; i < pieces; i++)
        {

            int merc_lon = mercX + (i / detail);
            int merc_lat = mercY + (i % detail);
            int tZoom = zoom;// + (int)Mathf.Log((float)pieces, 4f);
            collect_tiles.dlElvFile(merc_lon, merc_lat, tZoom, elvNames[i]);
            Texture2D tex = new Texture2D(256, 256);
            tex.LoadImage(File.ReadAllBytes(elvNames[i]));
            heights[i] = tex;
            yield return null;
        }

        try
        {
            UnityEditor.AssetDatabase.Refresh();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }


        List<float[,]> heightArrays = new List<float[,]>();

        for (int i = 0; i < heights.Count; i++)
        {
            heightArrays.Insert(i, new float[256, 256]);
            for (int j = 0; j < heights[i].width; j++)
                for (int k = 0; k < heights[i].height; k++)
                    heightArrays[i][j, k] = heights[i].GetPixel(j, k).a;
            heightArrays[i] = collect_tiles.flipMatrix(heightArrays[i], 256);
            heightArrays[i] = collect_tiles.RotateMatrix(heightArrays[i], 256);
            heightArrays[i] = collect_tiles.flipMatrix(heightArrays[i], 256);
            heightArrays[i] = collect_tiles.flattenMatrixEdge(heightArrays[i], 256);

            yield return null;

        }


        float[,] heightM = new float[256 * detail, 256 * detail];//Terr.terrainData.GetHeights(0, 0, 256*detail, 256*detail);
        float minHeight = float.MaxValue;
        float maxHeight = float.MinValue;

        for (int i = 0; i < heights.Count; i++)
        {
            for (int j = 0; j < heights[i].width; j++)
                for (int k = 0; k < heights[i].height; k++)
                {
                    float heightID = (1 - heightArrays[i][j, k]) * 255;
                    float qHeight = collect_tiles.quantized_height((int)heightID);

                    if (qHeight > maxHeight)
                        maxHeight = qHeight;
                    if (qHeight < minHeight)
                        minHeight = qHeight;

                    heightM[j + ((i % detail) * 256), k + ((i / detail) * 256)] = qHeight;
                }
            yield return null;
        }

        for (int i = 0; i < 256 * detail; i++)
            for (int j = 0; j < 256 * detail; j++)
            {
                heightM[i, j] = heightM[i, j] - minHeight;
                heightM[i, j] = heightM[i, j] / (maxHeight - minHeight);
            }


        heightM = collect_tiles.flipMatrix(heightM, Terr.terrainData.heightmapResolution - 1);




        Terr.terrainData.SetHeights(0, 0, heightM);
        if (LOD == -1)
        {
            Terr.heightmapPixelError = Mathf.Pow(2, Mathf.Max(0f, (zoom - 8f)));
            Terr.heightmapMaximumLOD = (int)(Mathf.Max(0f, (zoom - 11f)));
         }
        else
        {
            Terr.heightmapPixelError = Mathf.Pow(2, Mathf.Max(0f, (LOD)));
            Terr.heightmapMaximumLOD = LOD;
        }
        //Terr.heightmapPixelError = 64;

        float latitude;
        float longitude;
        collect_tiles.inverse_mercator(out latitude, out longitude, zoom, mercX, mercY);

        float mRes = collect_tiles.ground_resolution(latitude , zoom);
        float TerrHeight =  (maxHeight - minHeight) / mRes;

        Terr.terrainData.size = new Vector3(Terr.terrainData.size.x, TerrHeight, Terr.terrainData.size.z);

    }


    public void ChangeZoom(int val)
    {
        if (val == 0)
            return;

        float latitude;
        float longitude;

        collect_tiles.inverse_mercator(out latitude, out longitude, zoom, mercX, mercY);
        this.zoom += val;
        collect_tiles.mercator(latitude, longitude, zoom, out mercX, out mercY);

        hasChanged = true;

    }

    public void ChangeLatLong(int X, int Y)
    {
        if (X == 0 && Y == 0)
            return;

        mercX += X;
        mercY += Y;

        hasChanged = true;
    }

    public void ChangeTile(float lat, float lon, int zoom, char mode = 'a', int mXOffset = 0, int mYOffset = 0, int LOD = -1)
    {
        this.zoom = zoom;
        tileType = mode;
        collect_tiles.mercator(lat, lon, zoom, out mercX, out mercY);
        mercX += mXOffset;
        mercY += mYOffset;
        StartCoroutine(loadTile(LOD));
    }

}

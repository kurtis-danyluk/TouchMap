using UnityEngine;
using System.Collections;
using System.Net;
using System.Web;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

public class collect_tiles : MonoBehaviour {

    static WebClient client = new WebClient();
    Terrain Terr;
    public GameObject mTerr;
    //public Terrain mTerr;
    public Terrain me;
    public collect_tiles center;

    /// <summary>
    /// Web Cache to hold elevation tiles for faster load times
    /// </summary>
    //System.Web.Caching.Cache elvCache = new System.Web.Caching.Cache();
   
    /// <summary>
    /// Web cache to hold img tiles
    /// </summary>
    //System.Web.Caching.Cache imgCache= new System.Web.Caching.Cache();

    //Helper<byte[]> cache;

    const int tile_width = 256;
    const int tile_height = 256;

    public int latX;
    public int lonY;

    public static bool center_changed;
    public bool isCenter;
    // string webPath = "s3.amazonaws.com/elevation-tiles-prod/";


    static string cache_dir = "Assets/Cache/";
    static string base_dir = @"Assets/Textures/";
    string elvFilename = "elvTile.png";
    string aerImageFilename = "aerImage.jpeg";
    static string key = "AkkXBASn6AiuOToNWy_FDOv7iU5W8G8lyc_jYWpCKf-dWGzal51unBkQ4G209Iut";
    string tile_type;
    private string ImageURL;
    private string oImageURL;
    static float earthCircumference = 6378137f;
    public float latitude;
    private float olatitude;
    public float longitude;
    private float olongitude;
    static public int zoom;
    float ozoom;
    public float mRes;
    //float tile_lat_arc;
    //float tile_lon_arc;
    public int xpos;
    public int ypos;
    private float[,] heights;
    private bool image_changed;
    //private bool tex_swap;
    public bool hasSeaFloor = false;
    public char texture_mode = 'r';

    public float terrBaseHeight;
    private static bool qTableMade = false;
    static System.Collections.Generic.List<float> qMappingTable;
    float[,,] map;
    float[,,] mapB;

    MeshRenderer mesh;
    Texture2D filetex;
    //Texture2D oFiletex;
    Texture2D tileTex;
    byte[] fileData;

    // Use this for initialization
    void Start() {
        //cache = new Helper<byte[]>();
        //Setup filenames
        elvFilename = base_dir + this.name + elvFilename;
        aerImageFilename = base_dir + this.name + aerImageFilename;
        tile_type = "Road/";//"Aerial/";//
        //  Debug.Log(elvFilename);
        dlFile(0, 0, 1);
        //Done statically now
        //elvCache = new System.Web.Caching.Cache();
        //imgCache = new System.Web.Caching.Cache();
        
        //Load the active terrains TODO: Make this generic!
        //     Terr = Terrain.activeTerrains[1];
        Terr = me;
        //mTerr = Terrain.activeTerrains[0];
        
        //Setup a table of heightmap values
        heights = Terr.terrainData.GetHeights(0, 0, Terr.terrainData.heightmapWidth, Terr.terrainData.heightmapHeight);

        //Setup the textures to be used
        filetex = new Texture2D(256, 256, TextureFormat.ARGB32, true);
        //oFiletex = new Texture2D(256, 256, TextureFormat.ARGB32, false);
        tileTex = new Texture2D(256, 256, TextureFormat.ARGB32, true);

     //   mesh = this.GetComponent<MeshRenderer>();
        //Setup bing maps tile URL
        ImageURL = string.Empty;
        oImageURL = ImageURL;

        //Setup default lat, long, zoom
        
        longitude = -115.31f;
        olongitude = longitude;
        latitude = 51.17f;
        olatitude = latitude;
        zoom = 11;
        
        ozoom = 2;
        //tile_lat_arc = 90;
        //tile_lon_arc = 180;

        //  exageration_constant = 0;
        //Currently unused but these are set at minimums for terrain exageration
        terrBaseHeight = 15f;

        //Records state of image change
        image_changed = false;
        //tex_swap = false;
        center_changed = false;

        //Determine if we care about the sea floor
        //Set on the object now!
        //hasSeaFloor = false;


        //Link the terrain's texture to the appropriate files
        Terr.terrainData.splatPrototypes[0].texture = filetex;
        

        //Generate a quantized table
        qMappingTable = generate_quantized_table();


    }


    //Some sample bing maps tile requests
    //http://dev.virtualearth.net/REST/V1/Imagery/Metadata/Aerial/40.714550167322159,-74.007124900817871?zl=15&o=xml&key=AkkXBASn6AiuOToNWy_FDOv7iU5W8G8lyc_jYWpCKf-dWGzal51unBkQ4G209Iut
    //http://dev.virtualearth.net/REST/v1/Imagery/Map/imagerySet/centerPoint/zoomLevel?mapSize=mapSize&pushpin=pushpin&mapLayer=mapLayer&format=format&mapMetadata=mapMetadata&key=BingMapsKey
    //http://dev.virtualearth.net/REST/v1/Imagery/Map/imagerySet=Aerial/centerPoint=47,-122/zoomLevel=1?mapSize=mapSize&pushpin=pushpin&mapLayer=mapLayer&format=format&mapMetadata=mapMetadata&key=BingMapsKey
    //template for an aws elevation tile request. z is zoom level and x and y refer to a tile in mercantor format
    //https://s3.amazonaws.com/elevation-tiles-prod/normal/{z}/{x}/{y}.png

    /// <summary>
    /// Downloads an elevation file for specified lat/lon/zoom and stores it at the path specified by elvFilename
    /// </summary>
    /// <param name="merc_long"></param>
    /// <param name="merc_lat"></param>
    /// <param name="zoom"></param>
    /// <param name="elvFilename"></param>
     public static void dlElvFile(int merc_long, int merc_lat, int zoom, string elvFilename)
    {
        //Debug.Log(Terr.name);
        //Debug.Log("lat lon " + merc_lat + " " + merc_long);
        string key = "elv&" + merc_long +"&" + merc_lat + "&" + zoom+".png";
        //elvCache.Get(key);
         {
            string eQuery = "http://s3.amazonaws.com/elevation-tiles-prod/normal/" + zoom + "/" + merc_long.ToString() + "/" + merc_lat.ToString() + ".png";
            byte[] elv;
            if (System.IO.File.Exists(cache_dir + key))
            {
                elv = File.ReadAllBytes(cache_dir + key);
                File.WriteAllBytes(elvFilename, elv);
                return;
            }
            /*
            string key = "elv&" + zoom.ToString() + "&" + merc_long.ToString() + "&" + merc_lat.ToString() + "&end";
            elv = (byte[])elvCache[key];//GetFromCache<byte[]>(elvCache,key);
            if (elv != null)
            {
                //Debug.Log(this.name + " Grabbed Tile from Cache:" + key);
                File.WriteAllBytes(elvFilename, elv);

            }
            else
            {
            */
                try
                {
                    //Debug.Log(elvFilename);
                    //client.DownloadFile(eQuery, elvFilename);
                    elv = client.DownloadData(eQuery);
                    File.WriteAllBytes(elvFilename, elv);
                    File.WriteAllBytes(cache_dir + key, elv);
                    //AddToCache<byte[]>(elvCache ,key, elv);
                    //Debug.Log(this.name + " Added to cache: " + key);
                    //elvCache.Insert(key, elv, null, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);

                }
                catch (WebException e)
                {
                    Debug.Log("Error Trying To get Elevation Data");
                    Debug.LogException(e);
                    //Debug.Log("latitude:" + latitude + " " + merc_lat);
                    //Debug.Log("Longitude:" + longitude + " " + merc_long);
                    Debug.Log("Zoom:" + zoom);
                    Debug.Log(eQuery);
                }
            //}
        }

    }

    void dlElvFile(float latitude,float longitude , int zoom)
    {
        int merc_long;
        int merc_lat;

        mercator(latitude, longitude, zoom, out merc_long, out merc_lat);
        


        string eQuery = "http://s3.amazonaws.com/elevation-tiles-prod/normal/" + zoom + "/" + merc_long.ToString() + "/" + merc_lat.ToString() + ".png";

        try
        {
            //Debug.Log(elvFilename);
            client.DownloadFile(eQuery, elvFilename);
        }
        catch (WebException e)
        {
            Debug.Log("Error Trying To get Elevation Data");
            Debug.LogException(e);
            Debug.Log("latitude:" + latitude + " " + merc_lat);
            Debug.Log("Longitude:" + longitude + " " + merc_long);
            Debug.Log("Zoom:" + zoom);
            Debug.Log(eQuery);
        }


    }
    void dlImgFile(float latitude, float longitude, int zoom)
    {
        string bQuery = "http://dev.virtualearth.net/REST/V1/Imagery/Metadata/" + tile_type + latitude.ToString() + "," + longitude.ToString() + "?zl=" + zoom.ToString() + "&o=xml&key=" + key;

        try
        {

            string line = client.DownloadString(bQuery);
            string[] lines = line.Split((new char[] { '<', '>' }));
            foreach (string item in lines)
            {
                if (item.StartsWith("http"))
                    ImageURL = item;
            }

            if (!ImageURL.Equals(oImageURL))
            {
                client.DownloadFile(ImageURL, aerImageFilename);
                oImageURL = ImageURL;
                image_changed = true;
            }
        }
        catch (WebException e)
        {
            Debug.Log("Error Getting Image Data");
            Debug.LogException(e);
            Debug.Log(bQuery);
        };
    }
    /// <summary>
    /// Downloads a terrain texture file of location merc_lat, merc_lon, and zoom and stores it at the filepath specified by aerImageFilename, using format 'a' or 'r' to determine what type of texture
    /// </summary>
    /// <param name="merc_lat"></param>
    /// <param name="merc_lon"></param>
    /// <param name="zoom"></param>
    /// <param name="aerImageFilename"></param>
    /// <param name="texture_mode"></param>
    /// <param name="imgCache"></param>
    /// <returns></returns>
    public static bool dlImgFile(int merc_lat, int merc_lon, int zoom, string aerImageFilename, char texture_mode)
    {
        string qKey = TileXYToQuadKey(merc_lat, merc_lon, zoom);

        //Debug.Log(Terr.name);
        //Debug.Log("lat lon " + merc_lat + " " + merc_lon);

        //http://ecn.t0.tiles.virtualearth.net/tiles/r01212323100.jpeg?g=5733&amp;mkt={culture}&amp;shading=hill

        string bQuery;
        switch (texture_mode)
        {
            case 'a':
                bQuery = "http://ecn.t0.tiles.virtualearth.net/tiles/a" + qKey + ".jpeg?g=5733";
                break;
            case 'r':
                bQuery = "http://ecn.t0.tiles.virtualearth.net/tiles/r" + qKey + ".jpeg?g=5733&amp;mkt={culture}&amp;shading=hill";
                break;
            default:
                bQuery = "http://ecn.t0.tiles.virtualearth.net/tiles/r" + qKey + ".jpeg?g=5733&amp;mkt={culture}&amp;shading=hill";
                break;



        }
        byte[] img;
        
        string key = "img&"+ qKey + "&" + texture_mode.ToString()+".jpeg";
        if (System.IO.File.Exists(cache_dir + key))
        {
            img = File.ReadAllBytes(cache_dir + key);
            File.WriteAllBytes(aerImageFilename, img);

            return true;
        }
        //string 
        //string 
        //Debug.Log(bQuery);
        /*
        img = (byte[])imgCache[key];//GetFromCache<byte[]>(imgCache,key); //
        if (img != null)
        {
            //Debug.Log(this.name + " Grabbed Tile from Cache: " + key);
            File.WriteAllBytes(aerImageFilename, img);
        }
        else
        {*/
            try
            {
                img = client.DownloadData(bQuery);
                File.WriteAllBytes(aerImageFilename, img);
                File.WriteAllBytes(cache_dir + key, img);
                //AddToCache<byte[]>(imgCache,key, img);
                //Debug.Log(this.name + " Added to cache: " + key);
                //imgCache.Insert(key, img, null, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null );
                //client.DownloadFile(bQuery, aerImageFilename);

                return true;
            }
            catch (WebException e)
            {
                Debug.Log("Error Getting Image Data");
                Debug.LogException(e);
                Debug.Log(bQuery);
            };
        //}
        return false;
    }

    void dlFile(float latitude, float longitude, int zoom) {
        //Debug.Log(this.name + elvFilename);
        int merc_lat;
        int merc_long;

        mercator(latitude, longitude, zoom, out merc_long, out merc_lat);

        string bQuery = "http://dev.virtualearth.net/REST/V1/Imagery/Metadata/" + tile_type+ latitude.ToString() +","+longitude.ToString()+"?zl="+zoom.ToString()+"&o=xml&key=" + key;
        string eQuery = "http://s3.amazonaws.com/elevation-tiles-prod/normal/"+ zoom + "/"+merc_long.ToString()+"/"+merc_lat.ToString() +".png";
        //Debug.Log(Terr.name + " " + eQuery);

        try
        {
            //Debug.Log(elvFilename);
            client.DownloadFile(eQuery, elvFilename);
        }
        catch(WebException e)
        {
            Debug.Log("Error Trying To get Elevation Data");
            Debug.LogException(e);
            Debug.Log("latitude:" + latitude + " " + merc_lat);
            Debug.Log("Longitude:" + longitude + " " + merc_long);
            Debug.Log("Zoom:" + zoom);
            Debug.Log(eQuery);
        }
        try
        {
            
            string line = client.DownloadString(bQuery);
            string[] lines = line.Split((new char[] { '<', '>' }));
            foreach (string item in lines)
            {
                if (item.StartsWith("http"))
                    ImageURL = item;
            }

            if (!ImageURL.Equals(oImageURL))
            {
                client.DownloadFile(ImageURL, aerImageFilename);
                oImageURL = ImageURL;
                image_changed = true;
            }
        }
        catch(WebException e)
        {
            Debug.Log("Error Getting Image Data");
            Debug.LogException(e);
            Debug.Log(bQuery);
        };
        

    }

    /*
    void dlFile(int merc_lat, int merc_long, int zoom)
    {
        float latitude;
        float longitude;

        //Debug.Log(this.name + elvFilename);
        inverse_mercator(out latitude, out longitude, zoom, merc_long, merc_lat);

        string bQuery = "http://dev.virtualearth.net/REST/V1/Imagery/Metadata/" + tile_type + latitude.ToString() + "," + longitude.ToString() + "?zl=" + zoom.ToString() + "&o=xml&key=" + key;
        string eQuery = "http://s3.amazonaws.com/elevation-tiles-prod/normal/" + zoom + "/" + merc_long.ToString() + "/" + merc_lat.ToString() + ".png";
        Debug.Log(Terr.name + " " + eQuery);


        try
        {
            //Debug.Log(elvFilename);
            client.DownloadFile(eQuery, elvFilename);
        }
        catch (WebException e)
        {
            Debug.Log("Error Trying To get Elevation Data");
            Debug.LogException(e);
            Debug.Log("latitude:" + latitude + " " + merc_lat);
            Debug.Log("Longitude:" + longitude + " " + merc_long);
            Debug.Log("Zoom:" + zoom);
            Debug.Log(eQuery);
        }
        try
        {

            string line = client.DownloadString(bQuery);
            string[] lines = line.Split((new char[] { '<', '>' }));
            foreach (string item in lines)
            {
                if (item.StartsWith("http"))
                    ImageURL = item;
            }

            if (!ImageURL.Equals(oImageURL))
            {
                client.DownloadFile(ImageURL, aerImageFilename);
                oImageURL = ImageURL;
                image_changed = true;
            }
        }
        catch (WebException e)
        {
            Debug.Log("Error Getting Image Data");
            Debug.LogException(e);
            Debug.Log(bQuery);
        };


    }
    */

    void LateUpdate() {
        if (!isCenter && center_changed)
        {

            int x3, y3;
            //mercator(center.latitude, center.longitude, zoom, out x3, out y3);
            x3 = center.latX;
            y3 = center.lonY;
            latX = x3 + xpos;
            lonY = y3 - ypos;

            // Debug.Log(center.latitude + " " + center.longitude + " " + y3 + " " + x3);
            inverse_mercator(out this.latitude, out this.longitude, zoom, x3 + xpos, y3 - ypos);
            // Debug.Log(latitude + " " + longitude + " " + (y3 - ypos) + " " + (x3 + xpos));

            if (olatitude != latitude || olongitude != longitude || ozoom != zoom)
            {
                //  dlElvFile(latitude, longitude, zoom);
                //  dlImgFile(latitude, longitude, zoom);

                dlElvFile(latX, lonY, zoom, elvFilename);
                image_changed = dlImgFile(latX, lonY, zoom, aerImageFilename, texture_mode);
                olatitude = latitude;
                olongitude = longitude;
                ozoom = zoom;

                //dlFile(this.latitude, this.longitude, zoom);
            }


            if (File.Exists(elvFilename) && File.Exists(aerImageFilename) && image_changed)
            {
                formHeight(Terr, elvFilename);
                changeTex();
                image_changed = false;
                //tex_swap = true;
                //      Terr.terrainData.SetAlphamaps(0, 0, map);
            }
        }
    }


        // Update is called once per frame
	void Update () {

        if (isCenter)
        {
            center_changed = false;
            int x3, y3;
            mercator(center.latitude, center.longitude, zoom, out x3, out y3);
            // Debug.Log(center.latitude + " " + center.longitude + " " + y3 + " " + x3);
            inverse_mercator(out this.latitude, out this.longitude, zoom, x3 + xpos, y3 - ypos);
            // Debug.Log(latitude + " " + longitude + " " + (y3 - ypos) + " " + (x3 + xpos));
            latX = x3;
            lonY = y3;        

            if (olatitude != latitude || olongitude != longitude || ozoom != zoom)
            {
                center_changed = true;



                //  dlElvFile(latitude, longitude, zoom);
                //  dlImgFile(latitude, longitude, zoom);

                dlElvFile(latX, lonY, zoom, elvFilename);
                image_changed = dlImgFile(latX, lonY, zoom, aerImageFilename, texture_mode);

                olatitude = latitude;
                olongitude = longitude;
                ozoom = zoom;

                //dlFile(this.latitude, this.longitude, zoom);
            }

            if (File.Exists(elvFilename) && File.Exists(aerImageFilename) && image_changed)
            {
                formHeight(Terr, elvFilename);
                changeTex();
                image_changed = false;
                //tex_swap = true;
                //      Terr.terrainData.SetAlphamaps(0, 0, map);
            }
        }
    }

    //All this does is refresh the asset database. The textures are meaningfully changed each time they're downloaded anyway
    private void changeTex()
    {
        SplatPrototype[] splats = new SplatPrototype[1];
        splats[0] = new SplatPrototype();
        Texture2D tex = new Texture2D(tile_width, tile_height, TextureFormat.ARGB32, true);
        tex.LoadImage(File.ReadAllBytes(aerImageFilename));
        splats[0].texture = tex;
        splats[0].tileSize = new Vector2(tile_width, tile_height);

        Terr.terrainData.splatPrototypes = splats;

        try
        {
            UnityEditor.AssetDatabase.Refresh();
        }
        catch(System.Exception e)
        {
            Debug.Log(e.Message);
        }
            
            //}
            

    }


    public void formHeight(Terrain Terr, string elvFilename)
    {
        

        //Read the heightmap file into a texture file for storage
        byte[] imageBytes = File.ReadAllBytes(elvFilename);
        tileTex.LoadImage(imageBytes);

        //Setup a table to hold quantized height values
        float[,] qHeights= new float[Terr.terrainData.heightmapHeight, Terr.terrainData.heightmapWidth];
        
        //Determine the min and max height.
        //We'll use these to determine the proper height value for the scene
        float min_height= 0f;       //Sea Level
        float max_height = 8900f;   //The highest point on earth!

        //We normally only care about above sea level values but we do have bathometry data
        //Toggle the has Sea floor tag if we want to sea it!
        if (hasSeaFloor)
            min_height = -11000f;

        //Get the heights from file and their repsective quantized values
        for (int i = 0; i < Terr.terrainData.heightmapWidth; i++)
            for (int j = 0; j < Terr.terrainData.heightmapHeight; j++)
            {
                
                heights[i, j] = 1- tileTex.GetPixel(i, j).a;
                qHeights[i, j] = quantized_height((int)(heights[i, j] * 255));

                //If we get a below sea level value just treat it as sea level
                if (qHeights[i, j] < 0 && !hasSeaFloor)
                    qHeights[i, j] = 0f;
            }
        //Determine the range between the minimum and maximum height
        //This will change depending on if we care about sub sea level heights
        float hRange = max_height - min_height;
        //Debug.Log("hRange:" + hRange);

        if (isCenter)
        {

            //Determine the width in meters of the central tile given its latitude
            mRes = Mathf.Abs(ground_resolution(center.latitude, zoom));
            //Debug.Log("mRes:" + mRes);

            //Figures out how tall the scene should be based on the width of the tile
            terrBaseHeight = hRange / (mRes);

            //Exagerate the height abit - makes things look more impressive
            //terrBaseHeight *= exageration_constant; 

            //Debug.Log("Height" + terrBaseHeight);
            Terr.terrainData.size = new Vector3(Terr.terrainData.size.x, terrBaseHeight, Terr.terrainData.size.z);


            //mTerrBaseHeight = terrBaseHeight / 256;


            //mTerr.terrainData.size = new Vector3(mTerr.terrainData.size.x, (mTerrBaseHeight), mTerr.terrainData.size.z);
        }
        else
            Terr.terrainData.size = new Vector3(Terr.terrainData.size.x, center.Terr.terrainData.size.y, Terr.terrainData.size.z);
        //Debug.Log(Terr.name + " " + Terr.terrainData.size.y + " " + center.Terr.terrainData.size.y);

        //Transform Heightmap to match terrain
        qHeights = flipMatrix(qHeights, ((Terr.terrainData.heightmapHeight)));
        qHeights = RotateMatrix(qHeights, (Terr.terrainData.heightmapHeight));
        qHeights = flattenMatrixEdge(qHeights, (Terr.terrainData.heightmapHeight));




        for (int i = 0; i < Terr.terrainData.heightmapWidth; i++)
            for (int j = 0; j < Terr.terrainData.heightmapHeight; j++)
            {
                qHeights[i, j] = qHeights[i, j] - min_height;
                /*
                if (qHeights[i, j] < min_height & !hasSeaFloor)
                    qHeights[i, j] = min_height;
                    */
                heights[i, j] = qHeights[i, j] / hRange;
            }

        Terr.terrainData.SetHeights(0, 0, heights);


        
        Terr.terrainData.splatPrototypes[0].normalMap = tileTex;
        //    mTerr.terrainData.splatPrototypes[0].normalMap = tileTex;

        Terr.heightmapPixelError = zoom;

    }
    


    //Gives ratio of meters per pixel at given zoom level and latitude
    public static float ground_resolution(float latitude, int zoom)
    {
        float ground_resolution = (Mathf.Cos(latitude * Mathf.PI / 180) * 2 * Mathf.PI * earthCircumference)
            / (256 * Mathf.Pow(2,zoom));

        return ground_resolution;
    }
    private static System.Collections.Generic.List<float> generate_quantized_table()
    {
        System.Collections.Generic.List<float> table = new System.Collections.Generic.List<float>();
        for (int i = 0; i <= 11; i++)
            table.Add(-11000 + i * 1000);
        table.Add(-100);
        table.Add(-50);
        table.Add(-20);
        table.Add(-10);
        table.Add(-1);
        for (int i = 0; i <= 150; i++)
            table.Add(20 * i);
        for (int i = 0; i <= 60; i++)
            table.Add(3000 + 50 * i);
        for (int i = 0; i <= 29; i++)
            table.Add(6000 + 100 * i);
        qTableMade = true;
        return table;
    }
    public static float quantized_height(int h)
    {
        if (!qTableMade)
        {
            qMappingTable = generate_quantized_table();
            qTableMade = true;
        }
        return qMappingTable[h];
    }

    //------------------------------------------------------------------------------
    // <copyright company="Microsoft">
    //     Copyright (c) 2006-2009 Microsoft Corporation.  All rights reserved.
    // </copyright>
    //------------------------------------------------------------------------------

    /// <summary>
    /// Converts tile XY coordinates into a QuadKey at a specified level of detail.
    /// </summary>
    /// <param name="tileX">Tile X coordinate.</param>
    /// <param name="tileY">Tile Y coordinate.</param>
    /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
    /// to 23 (highest detail).</param>
    /// <returns>A string containing the QuadKey.</returns>
    public static string TileXYToQuadKey(int tileX, int tileY, int levelOfDetail)
    {
        StringBuilder quadKey = new StringBuilder();
        for (int i = levelOfDetail; i > 0; i--)
        {
            char digit = '0';
            int mask = 1 << (i - 1);
            if ((tileX & mask) != 0)
            {
                digit++;
            }
            if ((tileY & mask) != 0)
            {
                digit++;
                digit++;
            }
            quadKey.Append(digit);
        }
        return quadKey.ToString();
    }

    //Auther: Nick Berardi
    public static float[,] RotateMatrix(float[,] matrix, int n)
    {
        float[,] ret = new float[n, n];

        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                ret[i, j] = matrix[n - j - 1, i];
            }
        }

        return ret;
    }

    public static float [,] flipMatrix(float[,] matrix, int dims)
    {
        float[,] ret = new float[dims, dims];
        for(int i = 0; i < dims; i++)
            for(int j = 0; j < dims; j++)
            {
                ret[dims - i - 1, j] = matrix[i, j];

            }
        return ret;
    }
    public static float [,] flattenMatrixEdge(float [,] matrix, int dims)
    {
        float[,] ret;
        ret = matrix;
        for(int i = 0; i < dims; i++)
        {
            ret[0, i] = ret[1, i];
        }
        for (int i = 0; i < dims; i++)
        {
            ret[dims - 1, i] = ret[dims - 2, i];
        }
        for (int i = 0; i < dims; i++)
        {
            ret[i, 0] = ret[i, 1];
        }
        for (int i = 0; i < dims; i++)
        {
            ret[i, dims - 1] = ret[i,  dims -2];
        }

        return ret;
    }



    public static void inverse_mercator(out float lat, out float lon, int zoom, int x3, int y3)
    {
        float pi = Mathf.PI;

        float x1,y1,y2;

        int tiles = (int)System.Math.Pow(2, zoom);


        x1 = ((x3 * 2 * pi) / tiles) - pi; 
        lon = (180 * x1) / pi;

        y2 =  (((2 * pi * -y3) / tiles) + pi);
        y1 = (float)(2* (System.Math.Atan(System.Math.Pow(System.Math.E, y2)) - (pi * 0.25)));
        lat = (180 * y1) / pi;
        

        //y1 = (-y3 / tiles) + pi;
        //y2 = (float)System.Math.Pow(System.Math.E, y1);
        //lat = (360 * Mathf.Atan(y2) - (90 * pi)) / pi;
        //Debug.Log("Lat for " + y3 + " is " + lat); 
    }

    public static void mercator(float lat, float lon, int zoom, out int x3, out int y3)
    {
        float pi = Mathf.PI;

        //convert to radians
        float x1 = lon * pi / 180;
        float y1 = lat * pi / 180;

        //project to mercantor
        float x2 = x1;
        float y2 = Mathf.Log(Mathf.Tan((0.25f * pi + 0.5f * y1)));

        //transform to tile space
        int tiles = (int)System.Math.Pow(2, zoom);
        float diameter = 2 * pi;

        x3 = (int)(tiles * (x2 + pi)/ diameter);
        y3 = (int)(tiles * (pi - y2)/ diameter);
    }

    /// <summary>
    /// Modified Code from Daniel Earwick
    /// </summary>
    /// <typeparam name="T"></typeparam>
     class Helper<T>
    {
        internal readonly Dictionary<string, T> cache = new Dictionary<string, T>();
    }
    private static void AddToCache<T>(Helper<T> helper , string key, T value)
    {
        helper.cache[key] = value;
    }
    private static T GetFromCache<T>(Helper<T> helper, string key)
    {
        try
        {
            return helper.cache[key];
        }
        catch(KeyNotFoundException e)
        {
            Debug.Log(e.Message);
            return default(T);
        }
    }

    public void watch_lat(float l)
    {
        latitude = l;
    }
    public void watch_long(float l)
    {
        longitude = l;
    }
    public void watch_zoom(float z)
    {
        zoom = (int)z;
    }
    /*
    public void watch_exageration(float e)
    {
        exageration_constant = e;
    }
*/
}

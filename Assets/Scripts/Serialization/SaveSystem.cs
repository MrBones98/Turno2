using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    [SerializeField] private GameObject _levelContainer;
    [SerializeField] private List<Tile> _tilePrefabs;


    const string LEVEL_SUB = "/tile";
    const string LEVEL_TILE_COUNT_SUB = "/level.tilecount";

    public static List<Tile> tiles = new List<Tile>();
    
    public void SaveLevel()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.dataPath + LEVEL_SUB + SceneManager.GetActiveScene().buildIndex;
        string countPath = Application.dataPath + LEVEL_TILE_COUNT_SUB + SceneManager.GetActiveScene().buildIndex;

        Debug.Log(countPath);

        FileStream countStream = new FileStream(countPath, FileMode.Create);

        formatter.Serialize(countStream, tiles.Count);
        countStream.Close();

        for (int i = 0; i < tiles.Count; i++)
        {
            FileStream fs = new FileStream(path+i, FileMode.Create);
            TileData data = new TileData(tiles[i]);

            formatter.Serialize(fs, data);
            fs.Close();
        }
    }
    public void LoadLevel()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.dataPath + LEVEL_SUB + SceneManager.GetActiveScene().buildIndex;
        string countPath = Application.dataPath + LEVEL_TILE_COUNT_SUB + SceneManager.GetActiveScene().buildIndex;

        int tileCount = 0;

        if (File.Exists(countPath))
        {
            FileStream countStream = new FileStream(countPath, FileMode.Open);

            tileCount = (int)formatter.Deserialize(countStream);
            countStream.Close();
        }
        else
        {
            Debug.LogError("Path not found in" + countPath);
        }

        for (int i = 0; i < tiles.Count; i++)
        {
            if (File.Exists(path + i))
            {
                FileStream stream = new FileStream(path + i, FileMode.Open);

                //LevelData data = formatter.Deserialize(stream) as LevelData;
                TileData data = formatter.Deserialize(stream) as TileData;
                stream.Close();

                Vector3 position = new Vector3(data.position[0], data.position[1], data.position[2]);

                //Tile tile = Instantiate(_tilePrefabs[data.TypeOfTile], position, Quaternion.identity, _levelContainer.transform);
                
                
            }
            else
            {
                Debug.LogError("path not found in" + path + i);
            }
        }
    }
}

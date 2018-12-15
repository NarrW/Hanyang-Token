using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Mono.Data.Sqlite;

public class DataManager : MonoBehaviour
{
    private static DataManager m_Instance;
    public static DataManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType(typeof(DataManager)) as DataManager;
                if (m_Instance == null)
                {
                    GameObject container = new GameObject();
                    container.name = "DataManager";
                    m_Instance = container.AddComponent(typeof(DataManager)) as DataManager;
                }
            }
            return m_Instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void OnApplicationQuit()
    {
        m_Instance = null;

        CloseDB();

        m_dicBMS.Clear();
    }


    IDbConnection dbconn;

    public Dictionary<string, List<BMSLoader>> m_dicBMS;

    static bool FileEquals(string path1, string path2)
    {
        byte[] file1 = File.ReadAllBytes(path1);
        byte[] file2 = File.ReadAllBytes(path2);
        if (file1.Length == file2.Length)
        {
            for (int i = 0; i < file1.Length; i++)
            {
                if (file1[i] != file2[i])
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    public void Init()
    {
        ConnetDB();

        ReadBMSData();

        //CloseDB();
    }

    public void ConnetDB()
    {
        string conn = "URI=file:" + Application.dataPath + "/sirius.db"; //Path to database.
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
    }

    public void CloseDB()
    {
        dbconn.Close();
        dbconn = null;
    }

    public void BindString(IDbCommand cmd, string name, string value)
    {
        IDbDataParameter param = cmd.CreateParameter();
        param.DbType = DbType.String;
        param.ParameterName = name;
        param.Value = value;
        cmd.Parameters.Add(param);
    }

    public void BindByteArray(IDbCommand cmd, string name, byte[] value)
    {
        IDbDataParameter param = cmd.CreateParameter();
        param.DbType = DbType.Binary;
        param.ParameterName = name;
        param.Value = value;
        cmd.Parameters.Add(param);
    }

    public void InsertBMS(byte[] hash, string path)
    {
        IDbCommand dbcmd = dbconn.CreateCommand();
        dbcmd.CommandText = "SELECT * FROM bms WHERE filename='" + path + "'";
        int count = Convert.ToInt32(dbcmd.ExecuteScalar());
        dbcmd.Dispose();
        dbcmd = null;

        if (count == 0)
        {
            // not exist
            //Debug.Log("not exist");

            // load bms
            BMSLoader bms = new BMSLoader();
            bms.Init(null, path);
            bms.LoadMetaData();

            m_dicBMS[bms.m_sFolderPath].Add(bms);

            // insert db
            dbcmd = dbconn.CreateCommand();
            string sqlQuery = "INSERT INTO bms (ID, PathID, hash, filename, stagefile, acp, title, artist, genre, bpm, player, style, rank, level, bga, difficulty, notes, length, key, foundtime, stamp)" +
                "VALUES (null,'0',@hash,'" +
                bms.m_sFilePath + "','" +
                bms.m_cBMSInfo.sStagefile + "','0',@title,@artist,@genre,'" +
                bms.m_cBMSInfo.sBpm + "','" +
                bms.m_cBMSInfo.sPlayer + "','" +
                "0" + "','" +
                bms.m_cBMSInfo.sRank + "','" +
                bms.m_cBMSInfo.sPlaylevel + "','" +
                "0" + "','" +
                bms.m_cBMSInfo.sDifficulty + "','" +
                bms.m_nTotalNote + "','" +
                bms.m_nLength + "','" +
                bms.m_nKeyAmount + "','" +
                "0" + "','" +
                "0" + "')";

            dbcmd.CommandText = sqlQuery;

            BindByteArray(dbcmd, "@hash", hash);
            BindString(dbcmd, "@title", bms.m_cBMSInfo.sTitle);
            BindString(dbcmd, "@artist", bms.m_cBMSInfo.sArtist);
            BindString(dbcmd, "@genre", bms.m_cBMSInfo.sGenre);

            // execute query
            dbcmd.ExecuteNonQuery();

            dbcmd.Dispose();
            dbcmd = null;
        }
        else
        {
            // already exist
            //Debug.Log("already exist");

            dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = "SELECT hash FROM bms WHERE filename='" + path + "'";

            // execute query
            IDataReader reader = dbcmd.ExecuteReader();

            byte[] value = (byte[])reader[0];

            reader.Close();
            reader = null;

            if (value.SequenceEqual(hash) == false)
            {
                // file updated
                //Debug.Log("file updated");

                BMSLoader bms = new BMSLoader();
                bms.Init(null, path);
                bms.LoadMetaData();

                m_dicBMS[bms.m_sFolderPath].Add(bms);


                // update db
                dbcmd = dbconn.CreateCommand();
                string sqlQuery = "UPDATE bms SET hash=@hash," +
                    "filename='" + bms.m_sFilePath + "' stagefile='" + bms.m_cBMSInfo.sStagefile + "', title=@title, artist=@artist," +
                    "genre=@genre, bpm='" + bms.m_cBMSInfo.sBpm + "', player='" + bms.m_cBMSInfo.sPlayer + "'," +
                    "style='0', rank='" + bms.m_cBMSInfo.sRank + "', level='" + bms.m_cBMSInfo.sPlaylevel + "', bga='0'," + 
                    "difficulty='" + bms.m_cBMSInfo.sDifficulty + "', notes='" + bms.m_nTotalNote + "', length='" + bms.m_nLength + "', key='" + bms.m_nKeyAmount + "'" +
                    " WHERE filename='" + path + "'";

                dbcmd.CommandText = sqlQuery;

                BindByteArray(dbcmd, "@hash", hash);
                BindString(dbcmd, "@title", bms.m_cBMSInfo.sTitle);
                BindString(dbcmd, "@artist", bms.m_cBMSInfo.sArtist);
                BindString(dbcmd, "@genre", bms.m_cBMSInfo.sGenre);

                // execute query
                dbcmd.ExecuteNonQuery();

                dbcmd.Dispose();
                dbcmd = null;
            }
            else
            {
                // same file

                dbcmd = dbconn.CreateCommand();
                dbcmd.CommandText = "SELECT * FROM bms WHERE filename='" + path + "'";

                // execute query
                reader = dbcmd.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int pathId = reader.GetInt32(1);
                    byte[] hashval = (byte[])reader[2];
                    string filename = reader.GetString(3);
                    string stagefile = reader.GetString(4);
                    int acp = reader.GetInt32(5);
                    string title = reader.GetString(6);
                    string artist = reader.GetString(7);
                    string genre = reader.GetString(8);
                    float bpm = reader.GetFloat(9);
                    string player = reader.GetString(10);
                    string style = reader.GetString(11);
                    string rank = reader.GetString(12);
                    int level = reader.GetInt32(13);
                    string bga = reader.GetString(14);
                    string difficulty = reader.GetString(15);
                    int notes = reader.GetInt32(16);
                    int length = reader.GetInt32(17);
                    int key = reader.GetInt32(18);
                    int foundtime = reader.GetInt32(19);
                    int stamp = reader.GetInt32(20);

                    BMSLoader bms = new BMSLoader();
                    bms.Init(null, filename);
                    bms.m_cBMSInfo.sStagefile = stagefile;
                    bms.m_cBMSInfo.sTitle = title;
                    bms.m_cBMSInfo.sArtist = artist;
                    bms.m_cBMSInfo.sGenre = genre;
                    bms.m_cBMSInfo.sBpm = bpm.ToString();
                    bms.m_cBMSInfo.sPlayer = player;
                    bms.m_cBMSInfo.sRank = rank;
                    bms.m_cBMSInfo.sPlaylevel = level.ToString();
                    bms.m_cBMSInfo.sDifficulty = difficulty;
                    bms.m_nTotalNote = notes;
                    bms.m_nLength = length;
                    bms.m_nKeyAmount = key;
                    bms.m_fBPM = bpm;

                    m_dicBMS[bms.m_sFolderPath].Add(bms);
                }

                reader.Close();
                reader = null;
            }

            dbcmd.Dispose();
            dbcmd = null;
        }
    }

    public void ReadBMSData()
    {
        m_dicBMS = new Dictionary<string, List<BMSLoader>>();

        string[] ext = new[] { ".bms", ".bme", ".bml", ".pms" };
        string[] files = Directory.GetFiles("BMS", "*.*", SearchOption.AllDirectories).Where(s => ext.Contains(Path.GetExtension(s), StringComparer.OrdinalIgnoreCase)).ToArray();
        /*
        foreach(string str in files)
        {
            string sDirPath = Path.GetDirectoryName(str);

            if(m_dicBMS.ContainsKey(sDirPath) == false)
            {
                // 새로 추가
                m_dicBMS.Add(sDirPath, new List<BMSLoader>());
            }

            BMSLoader bms = new BMSLoader();
            bms.Init(str);
            bms.LoadMetaData();

            m_dicBMS[sDirPath].Add(bms);
        }*/


        foreach (string str in files)
        {
            string sDirPath = Path.GetDirectoryName(str);

            if (m_dicBMS.ContainsKey(sDirPath) == false)
            {
                // 새로 추가
                m_dicBMS.Add(sDirPath, new List<BMSLoader>());
            }

            byte[] filedata = File.ReadAllBytes(str);
            byte[] hashdata = MD5.Create().ComputeHash(filedata);

            InsertBMS(hashdata, str);
        }
    }
}

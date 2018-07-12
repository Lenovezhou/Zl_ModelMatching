using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

public class STL : MonoBehaviour
{
    private GameObject doneobj;
    private string _fileName = "";
    private string _trianglescount = "";
    private int _singleTrianglesNumber = 15000;
    private int _total;
    private int _number;
    private BinaryReader _binaryReader;
    private List<Vector3> _vertices;
    private List<Vector3> _normals;
    private List<int> _triangles;
    private string _meshCompression = "Off";
    private bool _isSaveMesh = true;
    private Action<string> callback;
    private string loadresult = "";
    Thread readvertexThread;

    /// <summary>
    /// 获取STL模型的文件名及三角面数量
    /// </summary>
    private void GetFileNameAndTrianglesCount(string path)
    {
        string fullPath = path;//Path.GetFullPath(AssetDatabase.GetAssetPath(target));
 
        using (BinaryReader br = new BinaryReader(File.Open(fullPath, FileMode.Open)))
        {
            _fileName = Encoding.UTF8.GetString(br.ReadBytes(80));
            _trianglescount = BitConverter.ToInt32(br.ReadBytes(4), 0).ToString();
        }
    }


    public void CreateInstance(string path, Action<string> _callback,Action<GameObject> DoneCall)
    {

        if (!path.IsStl())
        {
            if (null != callback)
            {
                loadresult = "Check File Type！！！";
                _callback(loadresult);
            }
            return ;
        }
        this.callback = _callback;
        GetFileNameAndTrianglesCount(path);
        if (_singleTrianglesNumber < 1000 || _singleTrianglesNumber > 20000)
        {
            Debug.LogError("Single Triangles Number: this value is unreasonable!");
            return ;
        }
        if (int.Parse(_trianglescount) > 200000)
        {
            //return ;
        }

        string fullPath = path;

        _total = int.Parse(_trianglescount);
        _number = 0;
        _binaryReader = new BinaryReader(File.Open(fullPath, FileMode.Open));

        //抛弃前84个字节
        _binaryReader.ReadBytes(84);

        _vertices = new List<Vector3>();
        _normals = new List<Vector3>();
        _triangles = new List<int>();

        //读取顶点信息
        readvertexThread = new Thread(ReadVertex);
        readvertexThread.Start();
        while (_number < _total) { }

        CreateGameObject(path,DoneCall);

        _binaryReader.Close();
        //EditorUtility.ClearProgressBar();
    }

    private void ReadVertex()
    {
        while (_number < _total)
        {
            byte[] bytes;
            bytes = _binaryReader.ReadBytes(50);

            if (bytes.Length < 50)
            {
                _number += 1;
                continue;
            }

            Vector3 vec0 = new Vector3(BitConverter.ToSingle(bytes, 0), BitConverter.ToSingle(bytes, 4), BitConverter.ToSingle(bytes, 8));
            Vector3 vec1 = new Vector3(BitConverter.ToSingle(bytes, 12), BitConverter.ToSingle(bytes, 16), BitConverter.ToSingle(bytes, 20));
            Vector3 vec2 = new Vector3(BitConverter.ToSingle(bytes, 24), BitConverter.ToSingle(bytes, 28), BitConverter.ToSingle(bytes, 32));
            Vector3 vec3 = new Vector3(BitConverter.ToSingle(bytes, 36), BitConverter.ToSingle(bytes, 40), BitConverter.ToSingle(bytes, 44));

            _normals.AddNormal(vec0);
            _triangles.AddTriangle(_vertices.AddGetIndex(vec1), _vertices.AddGetIndex(vec2), _vertices.AddGetIndex(vec3));

            _number += 1;
        }
        readvertexThread.Abort();
    }

    private void CreateGameObject(string _path, Action<GameObject> DoneCall)
    {
        string path = _path;//AssetDatabase.GetAssetPath(target);
        string fullPath = Path.GetFullPath(path);
        string assetPath = path;//.Substring(0, path.LastIndexOf("/")) + "/";

        GameObject root = new GameObject(Path.GetFileNameWithoutExtension(fullPath));
        root.transform.localPosition = Vector3.zero;
        root.transform.localScale = Vector3.one;

        int count = _total / _singleTrianglesNumber;
        count += (_total % _singleTrianglesNumber > 0) ? 1 : 0;

        for (int i = 0; i < count; i++)
        {
            GameObject tem = new GameObject(Path.GetFileNameWithoutExtension(fullPath) + "Sub" + i);
            tem.transform.SetParent(root.transform);
            tem.transform.localPosition = Vector3.zero;
            tem.transform.localScale = Vector3.one;

            MeshFilter mf = tem.AddComponent<MeshFilter>();
            MeshRenderer mr = tem.AddComponent<MeshRenderer>();

            int startIndex = i * _singleTrianglesNumber * 3;
            int length = _singleTrianglesNumber * 3;
            if ((startIndex + length) > _vertices.Count)
            {
                length = _vertices.Count - startIndex;
            }

            List<Vector3> vertices = _vertices.GetRange(startIndex, length);
            List<Vector3> normals = _normals.GetRange(startIndex, length);
            List<int> triangles = _triangles.GetRange(0, length);

            //压缩网格
            if (_meshCompression.IsOn())
            {
                MeshCompression(tem.name, vertices, normals, triangles);
            }

            Mesh m = new Mesh();
            m.name = tem.name;
            m.vertices = vertices.ToArray();
            m.normals = normals.ToArray();
            m.triangles = triangles.ToArray();
            m.RecalculateNormals();
            mf.mesh = m;
            mr.material = new Material(Shader.Find("Standard"));

            //保存网格
            if (_isSaveMesh)
            {
                //AssetDatabase.CreateAsset(mf.sharedMesh, assetPath + mf.sharedMesh.name + ".asset");
                //AssetDatabase.SaveAssets();
            }
            if (null != callback)
            {
                loadresult = "Import Succeed";
                callback(loadresult);
            }
            //Debug.Log("Create done! " + tem.name + ": Vertex Number " + m.vertices.Length);
        }
        if (null != DoneCall)
        {
            DoneCall(root);
        }

    }
    private void MeshCompression(string meshName, List<Vector3> vertices, List<Vector3> normals, List<int> triangles)
    {
        //移位补偿，当顶点被标记为待删除顶点时
        int offset = 0;
        //需要删除的顶点索引集合
        List<int> removes = new List<int>();
        for (int i = 0; i < vertices.Count; i++)
        {
           // EditorUtility.DisplayProgressBar("压缩网格", "正在压缩网格[ " + meshName + " ]（" + i + "/" + vertices.Count + "）......", (float)i / vertices.Count);
            if (removes.Contains(i))
            {
                offset += 1;
                continue;
            }

            triangles[i] = i - offset;
            for (int j = i + 1; j < vertices.Count; j++)
            {
                if (vertices[i] == vertices[j])
                {
                    removes.Add(j);
                    triangles[j] = triangles[i];
                }
            }
        }

        removes.Sort();
        removes.Reverse();

        for (int i = 0; i < removes.Count; i++)
        {
            vertices.RemoveAt(removes[i]);
            normals.RemoveAt(removes[i]);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using UnityEngine.UI;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;

public class Tool  
{
    #region 全局声明

    static public string normaltag = "Finish";
    static public string userimporttag = "Player";

    static public string LocalJsonSavePath = UnityEngine.Application.dataPath + "/LocalSave/IllNessDataJson/";
    static public string LocalModelonSavePath = UnityEngine.Application.dataPath + "/LocalSave/Model/";
    static public string LocalModelPointsPath = UnityEngine.Application.dataPath + "/ModelPoints/";

    //花纹本地存储路径Resources.load
    static public string LocalDecorativepatternPath = "Decorativepattern";
    //护具的本地存储路径Resources.load
    static public string R_ProtectiveclothingPath = "Model/RightProtectiveclothing";
    static public string L_ProtectiveclothingPath = "Model/LeftProtectiveclothing";
    //坐标系本地位置Resources.load
    static public string CoordinateSystemLocalPath = "Model/CoordinateSystem";
    //标准点到用户点的指示位置Resources.load
    static public string InstructionsGizmoLocalPath = "Model/InstructionsGizmo";
    //服务器下载的模型路径
    static public string SaveDownLoadFromWebPath = UnityEngine.Application.dataPath +"/LocalSave/Download/";
    //本地标准模型存储路径
    static public string LocalNormalModelPath = UnityEngine.Application.dataPath + "/LocalSave/NormalModels/";

    //本地保存的标准件的点json
    static public string LocalNormalpointsJsonPath = UnityEngine.Application.dataPath + "/LocalSave/ModelPoints/NormalModelJson/";

    static public string ModleDefaultPath = "D:/Test/000.obj";

    //格式化消息分割符
    static public string FormatMessageStr = "*";
    static public char UnFormatMessageChar = '*';


    //json文件后缀
    static public string jsondir = ".json";
    //stl文件后缀
    static public string stldir = ".stl";


    //读取用户设置key
    static public string Localuserdatakey = "localuserdata";
    //读取避空位key
    static public string Avoidkey = "avoid";
    //本地所有设置key
    static public string alllocaldatakey = "mold_info";
    //读取花纹key
    static public string patternkey = "pattern";
    //默认花纹
    static public string defultpattern = "1";

    //读取json的key
    static public string datapointkey = "points";
    static public string matchingpointskey = "matching_point";
    static public string datakey = "data";


    //患者模型路径key
    static public string stlInServerpath = "path";

    //下载模型时片体key
    static public string downloadbodykey = "a";
    //下载模型时护具key
    static public string Protectiveclothingkey = "b";
    //上次下载的片体和护具模型在server key
    static public string PieceProtectorURLkey = "mold_path";

    /// <summary>
    /// 默认md5
    /// </summary>
    static public string DefultMd5 = "fd44b5075d121e7f347684259aeb15af";
    //脚
    static public string rightfoot = "c18b14fcb53cb19cac7626196452fe5d";
    static public string leftfoot = "46ed2d26f6ebf77678e24073f2835345";
    //手
    static public string righthand = "fd44b5075d121e7f347684259aeb15af";
    static public string lefthand = "cfbe5dbffec6cdedfc6c52bafd4f04ea";
    //肩
    static public string rightshoulder = "60d2ada6ba8204ddc15df391e051ba31";
    static public string leftshoulder = "1e87ba8b85bf5d8ae1c90462e8ae0194";
    //膝
    static public string rightknee = "dc3af448431a9f73d2fb233e6049b1c0";
    static public string leftknee = "fc63c4d5c39afc49b07cbc522d713107";



    //受伤部位
    static public List<string> InjuryPosition = new List<string>() { "手", "臂", "脚", "膝" };
    //受伤方向
    static public List<string> Illposition = new List<string>() { "左", "右" };
    //护具外形
    static public List<string> protector_shape = new List<string>() { "短护具", "长护具 " };


    //Notice弹出后提示文字
    static public string ConnectingStr = "服务器连接中...";
    static public string FaleToConnect = "服务器连接失败,请联系管理员";
    static public string DownloadDir = "下载中,进度:";


    static public string FaleToSave = "保存失败,原因: ";

    static public int STLViewLayer = LayerMask.NameToLayer("STLView");

    static public Rect ThiredpanelNormalcam = new Rect(0, 0, 1, 1);
    static public Rect ThirdpanelSmallercam = new Rect(-0.17f, 0, 1, 1);

    //规定屏幕初始宽高
    static public Vector2 ScreenSize = new Vector2(1280, 738);
    //默认导入模型的坐标
    static public Vector3 ImprotUserPos = new Vector3(-0.0583f, 0.12736f, 4.03415f);

    //导入模型的缩放比例
    static public float UserImportScaler = 0.008f;
    //初始坐标系位置
    static public Vector3 coordinatesystem_originPos = new Vector3(0, 0, 4);


    //URL
     static public string URLdir = "https://bio3d.elacg.net/api/";
    //static public string URLdir = "http://demo.bio3d.com/api/";
    /// <summary>
    /// 登陆时拉取所有病例地址
    /// </summary>
    static public string illnessdatasimplepath = URLdir + "cases";
    /// <summary>
    /// 添加病例地址
    /// </summary>
    static public string addillnessdatasimplepath = URLdir + "cases/store";
    /// <summary>
    /// 第一界面刷新简易病例地址前缀（后加ID）
    /// </summary>
    static public string refreshillnessdatasimplepath = URLdir + "cases/update/";
    /// <summary>
    /// 请求详细病例地址前缀（后加ID）
    /// </summary>
    static public string requestdetailillnessdatapath = URLdir + "cases/show/";
    /// <summary>
    /// 添加matchingpoint地址
    /// </summary>
    static public string addmatchingpointspath = URLdir + "points/store";
    /// <summary>
    /// 刷新matchingpoints地址（后加ID）
    /// </summary>
    static public string refreshmathcingpointpath = URLdir + "points/update/";
    #endregion

    /// <summary>
    /// 打开选择文件的窗口
    /// </summary>
    /// <returns></returns>
    static public string OpenFileDisplay()
    {
		string path = "";
        OpenFileName ofn = new OpenFileName();

        ofn.structSize = Marshal.SizeOf(ofn);

        ofn.filter = "All Files\0*.*\0\0";

        ofn.file = new string(new char[256]);

        ofn.maxFile = ofn.file.Length;

        ofn.fileTitle = new string(new char[64]);

        ofn.maxFileTitle = ofn.fileTitle.Length;

        ofn.initialDir = UnityEngine.Application.dataPath;//默认路径  

        ofn.title = "Open Project";

        ofn.defExt = "JPG";//显示文件的类型  
                           //注意 一下项目不一定要全选 但是0x00000008项不要缺少  
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR  

        if (DllTest.GetOpenFileName(ofn))
        {
           path = ofn.file;
    	}
		return path;

    }




    /// <summary>
    /// 将stl文件转化为byte流
    /// </summary>
    /// <param name="fileUrl"></param>
    /// <returns></returns>
    static public byte[] AuthGetFileData(string fileUrl)
    {
        FileStream fs = new FileStream(fileUrl, FileMode.Open, FileAccess.Read);
        byte[] buffur = new byte[fs.Length];

        fs.Read(buffur, 0, buffur.Length);
        fs.Close();
        return buffur;
    }



    /// <summary>
    /// 自定义文件保存文件夹;
    /// </summary>
    static public string SaveFileDisplay()
    {
        FolderBrowserDialog fb = new FolderBrowserDialog();   //创建控件并实例化
        fb.Description = "选择文件夹";
        fb.RootFolder = Environment.SpecialFolder.DesktopDirectory;  //设置默认路径
        fb.ShowNewFolderButton = false;   //创建文件夹按钮关闭
        //如果按下弹窗的OK按钮
        string CompentPath = "";
        if (fb.ShowDialog() == DialogResult.OK)
        {
            //接受路径
            CompentPath = fb.SelectedPath;
        }
        //将路径中的 \ 替换成 /  由于unity路径的规范必须转
        string UnityPath = CompentPath.Replace(@"\", "/");
        return UnityPath;
    }
    #region 扫描文件夹下所有文件
   static public void GetAllFiles(string dir, List<string> allFiles)
    {
        DirectoryInfo di = new DirectoryInfo(dir);
        if (!di.Exists) return;//如果目录不存在,退出
        var currentDirFiles = di.GetFiles().Select(p => p.FullName);//获取当前目录所有文件
        allFiles.AddRange(currentDirFiles);//将当前目录文件放到allFiles中
        var currentDirSubDirs = di.GetDirectories().ToList();//获取子目录
        currentDirSubDirs.ForEach( p => GetAllFiles(p.FullName, allFiles));//将子目录中的文件放入allFiles中
    }
    #endregion


    #region 数字转abc


    static public string NumberToChar(int number)
    {
        if (1 <= number && 36 >= number)
        {
            int num = number + 64;
            ASCIIEncoding asciiEncoding = new ASCIIEncoding();
            byte[] btNumber = new byte[] { (byte)num };
            return asciiEncoding.GetString(btNumber);
        }
        return "数字不在转换范围内";
    }
    #endregion

    #region abc 转数字
    static public int CharToNumber(string str)
    {
        //定义一组数组array
        byte[] array = new byte[1];  
        array = System.Text.Encoding.ASCII.GetBytes(str); 
        int asciicode = (short)(array[0]);
        return asciicode;
    }


    #endregion


    #region Txt操作
    public void SaveToTxt(string path,string fill)
	{
		// 判断文件是否存在，不存在则创建，否则读取值显示到窗体
		if (!File.Exists(path))
		{
			FileStream fs1 = new FileStream(path, FileMode.Create, FileAccess.Write);//创建写入文件 
			StreamWriter sw = new StreamWriter(fs1);
			sw.WriteLine (fill);
			//sw.WriteLine(this.textBox3.Text.Trim() + "+" + this.textBox4.Text);//开始写入值
			sw.Close();
			fs1.Close();
		}
		else
		{
			FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Write);
			StreamWriter sr = new StreamWriter(fs);
			sr.WriteLine (fill);
			//sr.WriteLine(this.textBox3.Text.Trim() + "+" + this.textBox4.Text);//开始写入值
			sr.Close();
			fs.Close();
		}
	}


	/*重写TXT文档*/
	void WriteAllFile(string FileName,string txt)// 参数1:打开的文件(路径+文件命),参数2:重写所有文档的字符串
	{
		string[] str = txt.Split(';');
		File.WriteAllLines(FileName,str);
	}

	//在TXT文档中插入行
	void WriteALine(string FileName, string txt,int lineNumber)// 参数1:打开的文件(路径+文件命),参数2:重写某行的字符串,参数3,插入的行数
	{
		string[] str = File.ReadAllLines(FileName);
		int strLinesLength = str.Length;
		string[] strNew = new string[strLinesLength + 1];

		bool haveAddLine = false;//是否已经插入行

		for(int i=0;i< strLinesLength + 1;++i )
		{
			if(i==lineNumber-1) //到达插入行,插入并跳过此次下面的添加
			{
				strNew[i] = txt;
				haveAddLine = true;
			}
			else if(!haveAddLine)//还没插入新建行时
			{
				strNew[i] = str[i];
			}
			else//插入之后
			{
				strNew[i] = str[i - 1];
			}              
		}
		File.WriteAllLines(FileName, strNew);
	}

	#endregion

	#region XML操作

	#endregion

	#region Json操作
	static string MergToJson(Enums.PointMode pm,Dictionary<int, Dictionary<int, Vector3>> points)
	{
		StringBuilder sb = new StringBuilder ();
		sb.Append ("{\""+ pm.ToString () + "\":{");
		foreach (KeyValuePair<int, Dictionary<int, Vector3>> item in points) 
		{
            string abc = Tool.NumberToChar(item.Key + 1);
			sb.Append ("\""+abc + "\":{");
			foreach(KeyValuePair<int,Vector3> it  in item.Value)
			{
                Vector3 v = it.Value;
                int index = it.Key;
				sb.Append ("\""+ index.ToString() + "\":{\"x\":" + v.x.ToString ()+",\"y\":"+ v.y.ToString() + ",\"z\":" + v.z.ToString() + "},");
			}
            sb.Remove(sb.Length - 1, 1);
			sb.Append ("},");
		}
        sb.Remove(sb.Length - 1, 1);
        sb.Append("}}");

        return sb.ToString ();
	}
    static string JsonNetMergToJson(Enums.PointMode pm, Dictionary<int, Dictionary<int, Vector3>> points)
    {
        StringBuilder sb = new StringBuilder();
        JObject j = new JObject();
        JObject groupjobject = new JObject();
        foreach (KeyValuePair<int, Dictionary<int, Vector3>> item in points)
        {
            JObject pointsjobject = new JObject();
            string groupstr = NumberToChar(item.Key + 1);
            foreach (KeyValuePair<int, Vector3> it in item.Value)
            {
                string indexstr = it.Key.ToString();
                var jx = new JProperty("x", it.Value.x);
                var jy = new JProperty("y", it.Value.y);
                var jz = new JProperty("z", it.Value.z);

                JProperty pointproper = new JProperty(indexstr, new JObject(jx, jy, jz));
                pointsjobject.Add(pointproper);
            }
            JProperty groupproper = new JProperty(groupstr, new JObject(pointsjobject));
            groupjobject.Add(groupproper);

        }
        JProperty jend = new JProperty(pm.ToString(), groupjobject);
        j.Add(jend);
        Debug.Log(j.ToString());
       // ParseJsontoDic(j.ToString());
        return j.ToString();
    }



    #endregion


    #region PlayerSetting
    void SetWindowTitle()
    {
    }
    #endregion

    #region 保存模型文件，还原文件
    /// <summary>
    /// 赋值文件，并重命名
    /// </summary>
    /// <param name="srcPath"></param>
    /// <param name="id"></param>
    static public void CopyFileAndRename(string srcPath,int id)
    {
        string tarPath = LocalModelonSavePath;
        string str = ReadMD5(srcPath);
        string fTarPath = tarPath + "\\" + srcPath.Substring(Path.GetDirectoryName(srcPath).Length + 1);

        CopyFile(srcPath, fTarPath);

        string enddir = ".obj";
        if (fTarPath.IsStl())
        {
            enddir = ".stl";
        }

        File.Move(fTarPath, Path.GetDirectoryName(fTarPath) +"/" + id.ToString()+ enddir);
        str = ReadMD5(Path.GetDirectoryName(fTarPath) + "/" + id.ToString() + enddir);
    }
    static public bool CheckFileExist(string newpath)
    {
        return File.Exists(newpath);
    }

    /// <summary>
    /// 赋值文件到指定文件夹
    /// </summary>
    static public void CopyFile(string sourcepath,string destpath)
    {
        if (File.Exists(destpath))
        {
            File.Copy(sourcepath, destpath, true);
        }
        else
        {
            File.Copy(sourcepath, destpath);
        }
    }
    #endregion

    #region 本地修改模型的transform写入本地
    public static void UpdateLocalJsonFiles(int id,string data)
    {
        string message = data;
        string FileName = LocalJsonSavePath + id + ".json";

        if (!File.Exists(FileName))
        {
            File.Delete(FileName);
        }

        using (FileStream fileStream = File.Create(FileName))
        {
            byte[] bytes = new UTF8Encoding(true).GetBytes(message);
            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Dispose();
            fileStream.Close();
        }
    }


    /// <summary>
    /// 读取本地json
    /// </summary>
    /// <param name="filepath"></param>
    /// <returns></returns>
    static public string ReadLocalJson(string filepath)
    {
        string Localjson = "";
        if (File.Exists(filepath))
        {
            StreamReader sr = new StreamReader(filepath);

            Localjson = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
        }
        return Localjson;
    }
    #endregion

    #region 读取文件MD5
    static public string ReadMD5(string filepath)
    {
        try
        {
            FileStream file = new FileStream(filepath, System.IO.FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            Debug.Log("<color=yellow>md5码： "+sb.ToString() + "</color>");
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
        }
    }
    #endregion

    #region 查找子物体

    public static bool ContainsChild(GameObject parent, string childName)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; i++)
        {
            if (childName == children[i].name)
            {
                return true;
            }
        }
        return false;
    }
    #endregion

}


[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenFileName
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}

public class DllTest
{
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);
    public static bool GetOpenFileName1([In, Out] OpenFileName ofn)
    {
        return GetOpenFileName(ofn);
    }
}

public static class Extension
{
    public static bool IsStl(this string path)
    {
        string extension = Path.GetExtension(path);

        if (extension == ".stl" || extension == ".STL")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsObj(this string path)
    {
        string extension = Path.GetExtension(path);

        if (extension == ".obj" || extension == ".OBJ")
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    public static bool IsOn(this string value)
    {
        return value == "On";
    }

    public static int AddGetIndex(this List<Vector3> vertices, Vector3 vec)
    {
        vertices.Add(vec);
        return vertices.Count - 1;
    }

    public static void AddNormal(this List<Vector3> normals, Vector3 vec)
    {
        normals.Add(vec);
        normals.Add(vec);
        normals.Add(vec);
    }

    public static void AddTriangle(this List<int> triangles, int vertex1, int vertex2, int vertex3)
    {
        triangles.Add(vertex1);
        triangles.Add(vertex2);
        triangles.Add(vertex3);
    }


    #region DropDown

    public static void InitDropDown(this Dropdown Dd, List<string> showNames)
    {
        Dd.options.Clear();
        Dropdown.OptionData temoData;
        for (int i = 0; i < showNames.Count; i++)
        {
            //给每一个option选项赋值
            temoData = new Dropdown.OptionData();
            temoData.text = showNames[i];
            // temoData.image = sprite_list[i];
            Dd.options.Add(temoData);
        }
        //初始选项的显示
        Dd.captionText.text = showNames[0];

    }

    #endregion

}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using XLua;

public class Test1 : MonoBehaviour
{
    private readonly string luaDir = string.Format("{0}/Lua", Application.dataPath);


    // Start is called before the first frame update
    void Start()
    {
        using LuaEnv env = new();
        env.AddLoader(Loader);
        env.DoString("require 'test'");
    }

    private byte[] Loader(ref string filePath)
    {
        string luaPath = string.Format("{0}/{1}.lua", luaDir, filePath);
        if (File.Exists(luaPath))
        {
            byte[] bytes = File.ReadAllBytes(luaPath);
            return bytes;
        }
        return null;
    }
}

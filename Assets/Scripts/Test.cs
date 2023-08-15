using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        using LuaEnv env = new();
        env.DoString("CS.UnityEngine.Debug.Log('hello world')");
    }
}

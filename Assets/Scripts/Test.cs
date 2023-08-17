using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using XLua;

public class Test : MonoBehaviour
{
    private readonly string luaDir = string.Format("{0}/Lua", Application.dataPath);

    /*
    用Class承接lua中的table。

    table的属性可以多于或者少于class的属性。可以嵌套其它复杂类型。 
    要注意的是，这个过程是值拷贝，如果class比较复杂代价会比较大。
    而且修改class的字段值不会同步到table，反过来也不会。
    这个功能可以通过把类型加到GCOptimize生成降低开销。
    */
    private class PersonCls
    {
        public string name;
        public int age;
    }

    /*
    用Struct承接lua中的table。

    table的属性可以多于或者少于struct的属性。可以嵌套其它复杂类型。 
    要注意的是，这个过程是值拷贝，如果struct比较复杂代价会比较大。
    而且修改struct的字段值不会同步到table，反过来也不会。
    这个功能可以通过把类型加到GCOptimize生成降低开销。
    */
    private struct PersonSt
    {
        public string name;
        public int age;
    }

    // Start is called before the first frame update
    void Start()
    {
        using LuaEnv env = new();
        env.AddLoader(Loader);
        env.DoString("require 'main'");

        int num1 = env.Global.Get<int>("g_int_num");
        double num2 = env.Global.Get<double>("g_float_num");
        Debug.LogFormat("num1:{0}, num2:{1}", num1, num2);

        PersonCls personCls = env.Global.Get<PersonCls>("person_one");
        Debug.LogFormat("person cls, name:{0}, age:{1}", personCls.name, personCls.age);

        PersonSt personSt = env.Global.Get<PersonSt>("person_one");
        Debug.LogFormat("person st, name:{0} age:{1}", personSt.name, personSt.age);
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

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

    /*
    用Interface承接lua中的table。

    引用方式的映射，意味着修改interface的字段值会同步到table。
    这种方式依赖于生成代码（如果没生成代码会抛InvalidCastException异常），代码生成器会生成这个interface的实例。
    如果get一个属性，生成代码会get对应的table字段，如果set属性也会设置对应的字段。甚至可以通过interface的方法访问lua的函数。

    另外这个interface必须是public的，否则不能正确生成代码。
    */
    [CSharpCallLua]
    public interface PersonInf
    {
        public string name { get; set; }
        public int age { get; set; }
    }

    /*
    用delegate承接lua中的function。

    优点：
        性能好很多，而且类型安全。
        
    缺点：
        要生成代码（如果没生成代码会抛InvalidCastException异常）。

    声明方式：
        对于function的每个参数就声明一个输入类型的参数。 
        对于多返回值，从左往右映射到c#的输出参数，输出参数包括返回值，out参数，ref参数。

    另外这个function必须是public的，否则不能正确生成代码。
    */
    [CSharpCallLua]
    public delegate int TestFunc1(int a, int b, out int sub, out LuaTable table, ref int total);

    // Start is called before the first frame update
    void Start()
    {
        using LuaEnv env = new();
        env.AddLoader(Loader);
        env.DoString("require 'main'");

        // 访问Lua中的基本类型
        int num1 = env.Global.Get<int>("g_int_num");
        double num2 = env.Global.Get<double>("g_float_num");
        Debug.LogFormat("num1:{0}, num2:{1}", num1, num2);

        // 用class承接lua中的table
        PersonCls personCls = env.Global.Get<PersonCls>("person_one");
        Debug.LogFormat("person cls, name:{0}, age:{1}", personCls.name, personCls.age);

        // 用struct承接lua中的table
        PersonSt personSt = env.Global.Get<PersonSt>("person_one");
        Debug.LogFormat("person st, name:{0} age:{1}", personSt.name, personSt.age);

        // 用interface承接lua中的table
        PersonInf personInf = env.Global.Get<PersonInf>("person_one");
        Debug.LogFormat("person inf, name:{0} age:{1}", personInf.name, personInf.age);

        // 用Dictionary<T1,T2>承接lua中的table
        Dictionary<string, string> dict_title = env.Global.Get<Dictionary<string, string>>("dict_title");
        foreach (var pair in dict_title)
        {
            Debug.LogFormat("dict key:{0}, value:{1}", pair.Key, pair.Value);
        }

        // 用List<T>承接lua中的table
        List<string> list_title = env.Global.Get<List<string>>("list_title");
        foreach (string title in list_title)
        {
            Debug.LogFormat("list title:{0}", title);
        }

        /*
        用LuaTable承接lua中的table。

        优势：
            不需要生成代码
        
        劣势：
            比interface方式要慢一个数量级，没有类型检查。
        */
        LuaTable luaTable = env.Global.Get<LuaTable>("csharp_table");
        luaTable.ForEach<string, string>((key, value) =>
        {
            Debug.LogFormat("LuaTable key:{0}, value:{1}", key, value);
        });
        Debug.LogFormat("LuaTable Get int 1:{0}", luaTable.Get<int, string>(1));
        Debug.LogFormat("LuaTable Get int 2:{0}", luaTable.Get<int, string>(2));

        TestCallLuaFuncByDelegate(env);
    }

    /*
    由于CLR的gc策略相关，需要将调用lua函数的功能单独放到一个函数中，
    否则会报错：try to dispose a LuaEnv with C# callback
    参考：https://github.com/Tencent/xLua/issues/630
    */
    private void TestCallLuaFuncByDelegate(LuaEnv env)
    {
        TestFunc1 func1 = env.Global.Get<TestFunc1>("test_func1");
        int total = 0;
        int sum = func1(20, 10, out int sub, out LuaTable table, ref total);
        Debug.LogFormat("func1 call, sum:{0} sub:{1} table.sum:{2}, table.sub:{3}, total:{4}",
                        sum, sub, table.Get<string, int>("sum"), table.Get<string, int>("sub"), total);
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

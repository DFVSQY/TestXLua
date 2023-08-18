#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using System;


namespace XLua
{
    public partial class DelegateBridge : DelegateBridgeBase
    {
		
		public int __Gen_Delegate_Imp0(int p0, int p1, out int p2, out XLua.LuaTable p3, ref int p4)
		{
#if THREAD_SAFE || HOTFIX_ENABLE
            lock (luaEnv.luaEnvLock)
            {
#endif
                RealStatePtr L = luaEnv.rawL;
                int errFunc = LuaAPI.pcall_prepare(L, errorFuncRef, luaReference);
                ObjectTranslator translator = luaEnv.translator;
                LuaAPI.xlua_pushinteger(L, p0);
                LuaAPI.xlua_pushinteger(L, p1);
                LuaAPI.xlua_pushinteger(L, p4);
                
                PCall(L, 3, 4, errFunc);
                
                p2 = LuaAPI.xlua_tointeger(L, errFunc + 2);
                p3 = (XLua.LuaTable)translator.GetObject(L, errFunc + 3, typeof(XLua.LuaTable));
                p4 = LuaAPI.xlua_tointeger(L, errFunc + 4);
                
                int __gen_ret = LuaAPI.xlua_tointeger(L, errFunc + 1);
                LuaAPI.lua_settop(L, errFunc - 1);
                return  __gen_ret;
#if THREAD_SAFE || HOTFIX_ENABLE
            }
#endif
		}
        
        
		static DelegateBridge()
		{
		    Gen_Flag = true;
		}
		
		public override Delegate GetDelegateByType(Type type)
		{
		
		    if (type == typeof(Test.TestFunc1))
			{
			    return new Test.TestFunc1(__Gen_Delegate_Imp0);
			}
		
		    return null;
		}
	}
    
}
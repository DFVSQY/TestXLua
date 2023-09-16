--[[
    以下代码摘自LuaEnv中的init_xlua字段，
    该段代码用于初始化一些xlua环境
--]]

local metatable = {}
local rawget = rawget
local setmetatable = setmetatable
local import_type = xlua.import_type
local import_generic_type = xlua.import_generic_type
local load_assembly = xlua.load_assembly

function metatable:__index(key)
    --[[
        获取完整的完全限制名（Fully Qualified Name）
    ]]
    local fqn = rawget(self, '.fqn')
    fqn = ((fqn and fqn .. '.') or '') .. key

    -- 判断程序集中是否存在该类型
    local obj = import_type(fqn)

    if obj == nil then                      -- 不存在时可能为命名空间一部分
        -- It might be an assembly, so we load it too.
        obj = {
            ['.fqn'] = fqn
        }
        setmetatable(obj, metatable)        -- 为命名空间设置同样的元表
    elseif obj == true then
        return rawget(self, key)            -- 从元表中获取指定的元素返回
    end

    -- Cache this lookup
    rawset(self, key, obj)
    return obj
end

function metatable:__newindex()
    error('No such type: ' .. rawget(self, '.fqn'), 2)
end

--[[
    !!! 支持函数调用的形式，暂时还没搞清楚怎么用？
--]]
-- A non-type has been called; e.g. foo = System.Foo()
function metatable:__call(...)
    local n = select('#', ...)
    local fqn = rawget(self, '.fqn')                    -- 获取对应的完全限定名称
    if n > 0 then
        local gt = import_generic_type(fqn, ...)
        if gt then
            return rawget(CS, gt)
        end
    end
    error('No such type: ' .. fqn, 2)
end

-- 定义xlua环境下的CS表，并为其设置元表
CS = CS or {}
setmetatable(CS, metatable)

-- 定义xlua下的typeof函数
typeof = function(t)
    return t.UnderlyingSystemType
end

-- 定义xlua下的cast函数
cast = xlua.cast

--[[
    setfenv，getfenv 函数是 Lua 5.1 版本中的一个函数，用于设置函数的环境。
    在 Lua 5.2 及更高版本中，该函数已被弃用，
    此处是对于不存在该函数的lua环境提供该函数的实现。
]]
if not setfenv or not getfenv then
    local function getfunction(level)
        local info = debug.getinfo(level + 1, 'f')
        return info and info.func
    end

    function setfenv(fn, env)
        if type(fn) == 'number' then
            fn = getfunction(fn + 1)
        end
        local i = 1
        while true do
            local name = debug.getupvalue(fn, i)
            if name == '_ENV' then
                debug.upvaluejoin(fn, i, (function()
                    return env
                end), 1)
                break
            elseif not name then
                break
            end

            i = i + 1
        end

        return fn
    end

    function getfenv(fn)
        if type(fn) == 'number' then
            fn = getfunction(fn + 1)
        end
        local i = 1
        while true do
            local name, val = debug.getupvalue(fn, i)
            if name == '_ENV' then
                return val
            elseif not name then
                break
            end
            i = i + 1
        end
    end
end

--[[
    为xlua提供一些字段的实现
]]
xlua.hotfix = function(cs, field, func)
    if func == nil then
        func = false
    end
    local tbl = (type(field) == 'table') and field or {
        [field] = func
    }
    for k, v in pairs(tbl) do
        local cflag = ''
        if k == '.ctor' then
            cflag = '_c'
            k = 'ctor'
        end
        local f = type(v) == 'function' and v or nil
        xlua.access(cs, cflag .. '__Hotfix0_' .. k, f) -- at least one
        pcall(function()
            for i = 1, 99 do
                xlua.access(cs, cflag .. '__Hotfix' .. i .. '_' .. k, f)
            end
        end)
    end
    xlua.private_accessible(cs)
end

--[[ 
    获取cs对象的元表
 ]]
xlua.getmetatable = function(cs)
    return xlua.metatable_operation(cs)
end

--[[ 
    设置cs对象的元表
 ]]
xlua.setmetatable = function(cs, mt)
    return xlua.metatable_operation(cs, mt)
end

--[[ 
    设置一个类型指定字段的新实现
 ]]
xlua.setclass = function(parent, name, impl)
    impl.UnderlyingSystemType = parent[name].UnderlyingSystemType
    rawset(parent, name, impl)
end

local base_mt = {
    __index = function(t, k)
        local csobj = t['__csobj']
        local func = csobj['<>xLuaBaseProxy_' .. k]
        return function(_, ...)
            return func(csobj, ...)
        end
    end
}

--[[
    base函数的实现，该函数为指定的C#对象创建一个table，并指定元表为base_mt
]]
base = function(csobj)
    return setmetatable({
        __csobj = csobj
    }, base_mt)
end

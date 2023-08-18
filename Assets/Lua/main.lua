print("start main lua scirpt!")

_G.g_int_num = 10

_G.g_float_num = 20.5

_G.person_one = {
    name = "Gates",
    age = 15,
}

_G.dict_title = {
    file = "File",
    edit = "Edit",
    selection = "Selection",
    view = "View",  
}

_G.list_title = {"file", "edit", "selection", "view"}

_G.csharp_table = {
    "file", edit = "Edit", "selection", view = "View"
}

_G.test_action1 = function(a, b)
    return a * b
end

_G.test_func1 = function(a, b)
    local sum = a + b;
    local sub = a - b;
    
    return sum, sub, {sum = sum, sub = sub}, sum + sub
end
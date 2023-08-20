local GameObject = CS.UnityEngine.GameObject
local Time = CS.UnityEngine.Time

-- 调用C#的构造函数
local go_1 = GameObject()
print("go_1.name:", go_1.name)

-- 调用C#的构造函数
local go_2 = GameObject("Window")
print("go_2.name:", go_2.name)

-- 调用C#的静态函数
local win_go = GameObject.Find("Window")
print("win_go:", win_go.name)

-- 读取C#的静态属性
print("Time.timeScale:", Time.timeScale)

-- 写入C#的静态属性
Time.timeScale = 0.5
print("Time.timeScale:", Time.timeScale)
Time.timeScale = 1

-- 写成员函数
local go_3 = GameObject()
go_3.name = "Code"
print("go_3.name:", go_3.name)

-- 调用成员函数
local go_4 = GameObject()
go_4:SetActive(false)

---------------------------------- 调用一些自定义的c#类型 ----------------------------------
local Person = CS.Person
local person = Person()

person.age = 10
local person_age = person.age
print("person age:", person_age)

person:SetName("Wang")
local person_name = person:GetName()
print("person name:", person_name)

person.data:SetScore(20)
local person_score = person.data:GetScore(20)
print("person data score:", person_score)

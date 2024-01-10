-- 创建一个普通的表
local myTable = { name = "Tom", age = 20 }

-- 创建一个具有弱值引用的metatable
--local mt = { __mode = "v" }
local mt = {}
-- 使用弱值引用的metatable作为myTable的元表
setmetatable(myTable, mt)

-- 创建一个被弱引用的对象
local obj = { data = "Some data" }

-- 将obj添加到myTable中
myTable.obj = obj

-- 统计有效的键值对数量
local count = 0
for k, v in pairs(myTable) do
  count = count + 1
end

-- 输出有效的键值对数量
print("Count of myTable:", count) -- 输出: Count of myTable: 1

-- 解除对obj的引用
obj = nil

-- 强制进行一次垃圾回收
collectgarbage()

-- 统计有效的键值对数量
count = 0
for k, v in pairs(myTable) do
  count = count + 1
end

-- 输出有效的键值对数量
print("Count of myTable:", count) -- 输出: Count of myTable: 0

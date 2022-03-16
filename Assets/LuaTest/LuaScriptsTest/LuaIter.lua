
--- 利用闭包实现iterator,iterator是一个工厂，每次调用都会产生一个新的闭包,该闭包内部包括了upvalue(t,i,n)
--- 因此每调用一次该函数都会产生闭包，那么该闭包就会根据记录上一次的状态，以及返回table中的下一个元素
function iterator(t)
    local i = 0
    local n = #t
    return function()
        i = i + 1
        if i <= n then --实现了遍历空洞table
            return t[i]
        end
    end
end

testTable = {nil,1,2,3,nil,"a","b"}

print("table长度:" .. #testTable)
print("table长度 2:" .. select('#', testTable))


-- while中使用迭代器
iter1 = iterator(testTable) --调用迭代器产生一个闭包
while true do
    local element = iter1()
    if nil == element then
        break;
    end
    print(element)
end

-- for中使用迭代器
for element in iterator(testTable) do --- 这里的iterator()工厂函数只会被调用一次产生一个闭包函数，后面的每一次迭代都是用该闭包函数，而不是工厂函数
    print(element)
end

function counter()
    local i = 0
    return function() --匿名函数，闭包
        i = i + 1
        return i
    end
end

counter1 = counter()
counter2 = counter() 
-- counter1,counter2 是建立在同一个函数，同一个局部变量的不同实例上面的两个不同的闭包
--                   闭包中的upvalue各自独立，调用一次counter()就会产生一个新的闭包
print(counter1()) -- 输出1
print(counter1()) -- 输出2
print(counter1()) -- 输出2
print(counter2()) -- 输出1
print(counter2()) -- 输出2
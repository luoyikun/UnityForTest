local func = function(a, b)
    for i= 1, 5 do
        print(i, a, b)
    end
end

local func1 = function(a, b)
    for i = 1, 5 do
        print(i, a, b)
		c,d = coroutine.yield()
		print(c,d)
    end
end


co =  coroutine.create(func)
coroutine.resume(co, 1, 2)
--此时会输出 1 ，1， 2/ 2，1，2/ 3， 1，2/4，1，2/5，1，2

co1 = coroutine.create(func1)
coroutine.resume(co1, 1, 2)
--此时会输出 1， 1，2 然后挂起
coroutine.resume(co1, 3, 4)
--此时将上次挂起的协程恢复执行一次，输出： 2, 1, 2 所以新传入的参数3，4是无效的


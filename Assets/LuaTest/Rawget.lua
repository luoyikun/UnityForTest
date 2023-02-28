Window = {}  
Window.prototype = {x = 0 ,y = 0 ,width = 100 ,height = 100,}  
Window.mt = {}  

function Window.new(o)  
    setmetatable(o ,Window.mt)  
    return o  
end

Window.mt.__index = function (t ,key)  
    return 1000  
end 

Window.mt.__newindex = function (table ,key ,value)  
    if key == "wangbin" then  
        rawset(table ,"wangbin" ,"yes,i am")  --设置的是原始表字段
        -- table.wangbin = "yes,i am"   (2)反例
    end  
end 

w = Window.new({x = 10 ,y = 20}  )
print(rawget(w ,w.wangbin)) --因为原始表中得不到
print(w.wangbin)  --指向了元表的index

w.wangbin = "nVal" --指向了元表的__newindex
print(w.wangbin)

rawset(w,"wangbin","nVal")
print(w.wangbin) --原始表已经被设置了，所以是从原始表中找

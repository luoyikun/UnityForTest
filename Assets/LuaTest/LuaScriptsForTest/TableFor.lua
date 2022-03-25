function values(t)
 local i = 0
 return function() i = i + 1; return t[i] end
end

t = {10, 20, 30}

print("循环调用---------------")
iter = values(t)
while true do
 local el = iter()
 if el == nil then break end
 print(el)
end

print("泛型for调用---------------")
for el in values(t) do print(el) end

print("ipair实现原理---------------")
local function iter(s, i)
 i = i + 1
 local v = s[i]
 if v then return i, v end
end
function myipairs(s)
 return iter, s, 0
end

for i,v in myipairs(t) do
print(i)
print(v)
end

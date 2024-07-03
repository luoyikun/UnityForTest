tabA = {}
print(tabA)
tabArr = {}
setmetatable(tabArr,  {__mode = "k"})
tabArr[tabA] = 1
tabA = nil
--tabArr[tabA] = nil
collectgarbage()

for k,v in pairs(tabArr) do 
    print(k)
    print(v)
end    


local tabA = {x = 3}
local tabB = {b = 2}
setmetatable(tabA,tabB)

for key,value in pairs(tabA) do 
    print(value)
end    
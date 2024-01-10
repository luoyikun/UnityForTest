local oriTab = {}
local mtTab = {}


mtTab.__index = function(tbl,key)
    if type(key) == "number" then
        tbl[key] = key
        print("SetValue")
        return key
    end
end

setmetatable(oriTab,mtTab)

print(oriTab[1])
print(oriTab[1])
print(mtTab[1])
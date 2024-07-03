
EnSvrName = {
    [1] = "ab",
    [2] = "cd"
}
local tip = "#{_SVRNAME_1}---adsf---#{_SVRNAME_2}---"
local key = "#{_SVRNAME_"
local kuoHao = "}"
local startPos,endPos = string.find(tip,key)

-- tip = string.gsub(tip, key, "Lua",1)
-- print(tip)
while startPos do
    print("startPos" .. startPos .. "endPos" .. endPos)
    --startPos,endPos = string.find(tip,key)
    
    local kuoHaoEnd = string.find(tip,kuoHao)
    local sNum = string.sub(tip,endPos+1,kuoHaoEnd-1)
    print(sNum)
    local svrName = EnSvrName[tonumber(sNum)]
    local oriSvrNameSubStr =  string.sub(tip,startPos,kuoHaoEnd)
    print(oriSvrNameSubStr)
    tip = string.gsub(tip, oriSvrNameSubStr, svrName,1)
    print(tip)

    startPos,endPos = string.find(tip,key)
end




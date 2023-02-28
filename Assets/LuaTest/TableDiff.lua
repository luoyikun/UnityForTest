
tabA = {1,nil,2,nil}

function table.nums(t)
    local count = 0
    for k, v in pairs(t) do
        count = count + 1
    end
    return count
end

print(table.nums(tabA))
-- tabA = {1,2,3} --旧的
-- tabB = {2,3,4}

-- tabRemove = {}
-- tabCommon = {}
-- tabNew = {}
-- for aK,aV in pairs(tabA) do
--     local isFind = false
--     for bK,bV in pairs(tabB) do
--         local isNew = true
--         if aV == bV then
--             isFind = true
--             isNew = false
--             table.insert(tabCommon,aV)
--             break
--         end

--         if isNew == true then
--             table.insert(tabNew,bV)
--         end
--     end

--     if isFind == false then
--         table.insert(tabRemove,aV)
--     end
-- end

-- for i = 1,#tabRemove do
--     print(tabRemove[i])
-- end
-- print("11111111111111111111111111")
-- for i = 1,#tabCommon do
--     print(tabCommon[i])
-- end
-- print("222222222222222222222")
-- for i = 1,#tabNew do
--     print(tabNew[i])
-- end
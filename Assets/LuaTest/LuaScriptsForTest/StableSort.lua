require "PrintTable"

local function Merge(arr, tmp, L1, L2, R1, R2, compare)
    local L = L1
    local R = R1
    local index = L1
    while(L <= L2 and R <= R2) do
        if(compare(arr[L], arr[R])) then
            tmp[index] = arr[L]
            L = L + 1
        else
            tmp[index] = arr[R]
            R = R + 1
        end
        index = index + 1
    end
    while(L <= L2) do
        tmp[index] = arr[L]
        index = index + 1
        L = L + 1
    end
    while(R <= R2) do
        tmp[index] = arr[R]
        index = index + 1
        R = R + 1
    end
    for i = L1, R2 do arr[i] = tmp[i] end
end
local function MergeSort(arr, tmp, left, right, compare)
    if(left >= right) then return end
    local mid = left + math.floor((right - left) / 2)
    MergeSort(arr, tmp, left, mid, compare)
    MergeSort(arr, tmp, mid + 1, right, compare)
    Merge(arr, tmp, left, mid, mid + 1, right, compare)
end

--compare: return A <= B
function StableSort(t, compare)
    local res = {}
    MergeSort(t, res, 1, #t, compare)
    return res
end

local tbl = { {name = "b", age = 30}, {name = "c", age = 30},{name = "a", age = 25}, {name = "d", age = 30} }
-- 使用稳定排序函数进行排序
local sortedTbl = function(a, b)
    return a.age < b.age
end


-- function mergeSort(tbl, comp)
--     if #tbl <= 1 then
--         return tbl
--     end

--     local mid = math.floor(#tbl / 2)
--     local left = {}
--     local right = {}

--     for i = 1, mid do
--         table.insert(left, tbl[i])
--     end

--     for i = mid + 1, #tbl do
--         table.insert(right, tbl[i])
--     end

--     left = mergeSort(left, comp)
--     right = mergeSort(right, comp)

--     return merge(left, right, comp)
-- end

-- function merge(left, right, comp)
--     local result = {}
--     local i = 1
--     local j = 1

--     while i <= #left and j <= #right do
--         if comp and comp(left[i], right[j]) or left[i] <= right[j] then
--             table.insert(result, left[i])
--             i = i + 1
--         else
--             table.insert(result, right[j])
--             j = j + 1
--         end
--     end

--     while i <= #left do
--         table.insert(result, left[i])
--         i = i + 1
--     end

--     while j <= #right do
--         table.insert(result, right[j])
--         j = j + 1
--     end

--     return result
-- end

local sortAfter = StableSort(tbl,sortedTbl)

for k,v in ipairs(sortAfter) do
    print(k,v.name)
end
--PrintTable(sortAfter)
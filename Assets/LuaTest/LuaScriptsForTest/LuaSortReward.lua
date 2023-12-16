require "PrintTable"
CanGet = 3
Doing = 2
HasFinish = 1

local a = {}
a.sort = 3
a.state = CanGet
a.id = "a"

local b = {}
b.sort = 2
b.state = HasFinish
b.id = "b"

local c = {}
c.sort = 1
c.state = CanGet
c.id = "c"

local d = {}
d.sort = 1
d.state = CanGet
d.id = "d"

tabSort = {}
table.insert(tabSort,a)
table.insert(tabSort,b)
table.insert(tabSort,c)
table.insert(tabSort,d)
local funcSort = function (a,b)
    if a.state ~= b.state then
        return a.state > b.state
    end

    if a.sort ~= b.sort then
        return a.sort < b.sort
    end
    return true
end
--table.sort(tabSort,funcSort)

--PrintTable(tabSort)

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

res = StableSort(tabSort,funcSort)

PrintTable(res)
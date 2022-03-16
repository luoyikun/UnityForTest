
tbtestAward={b =1,a = 2,c = 3}

function pairsByKeys(t)
    local a = {}
    for n in pairs(t) do
        a[#a+1] = n
    end
    table.sort(a)
    local i = 0
    return function()
        i = i + 1
        return a[i], t[a[i]]
    end
end

for key, value in pairsByKeys(tbtestAward) do
    print(key .. " --" .. value)
end


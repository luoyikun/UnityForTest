dic = {[4]=4, [1]=1, [2]=2}

for k,v in pairs(dic) do
    print(k..v)
end

function PairsByKeys(t,f)
    local a = {}
    for n in pairs(t) do
        a[#a+1] = n
    end
    table.sort(a,f)
    local i = 0
    return function()
        i = i+1
        return a[i],t[a[i]]
    end
end


for k,v in PairsByKeys(dic) do
    print(k..v)
end
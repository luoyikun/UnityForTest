tabA = {[1] = 1,[2] =2,[3] = 3}


for k,v in pairs(tabA) do
    if k == 1 then
        k = -1
    end
end

for k,v in pairs(tabA) do
    print(k)
end
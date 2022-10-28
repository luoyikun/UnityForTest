
itemA = {idx = 1, a = 2,b = 3,c = 4}
itemB = {idx = 2, a = 2,b = 3,c = 5}
tab = {}

table.insert(tab,itemA)
table.insert(tab,itemB)

table.sort(tab, function(x, y)
    -- 按num降序排序
	if x.a > y.a then
		return true
	elseif x.b >  y.b then
		return true
	elseif x.c >  y.c then
		return true
	end
end)

print(tab[1].idx)



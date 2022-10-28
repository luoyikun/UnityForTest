tab = {[0] = 0,1,2,3}

for k,v in pairs(tab) do
	print(v)
end

print("长度："  .. #tab)


for i = 0, #tab do
	print(tab[i])
end

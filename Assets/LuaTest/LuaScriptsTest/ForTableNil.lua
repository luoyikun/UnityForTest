tab = {1,2,8,3,nil,4,5,6,18,nil}
maxSize = table.maxn(tab)

for idx=1, maxSize do
     if tab[idx] ~= nil then
          print(tab[idx])
     else
		 print('nil')
     end
end


function table_maxn(t)
  local mn=nil;
  for k, v in pairs(t) do
    if(mn==nil) then
      mn=k
    end
    if mn < k then
      mn = k
    end
  end
  return mn
end

maxSize2 = table_maxn(tab)

print(maxSize2)

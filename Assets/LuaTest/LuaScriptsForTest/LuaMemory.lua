local a = {}
a[1] = 1
a[2] = nil
a[3] = 3

for k,v in pairs(a) do
  print(k ..v )
end
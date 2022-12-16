local int = 66309
local t = {}
for i = 0, 3 do
    t[i+1] = (int >> (i * 8)) & 0xFF
end

for i = 0, 3 do
    print(t[i+1])
end
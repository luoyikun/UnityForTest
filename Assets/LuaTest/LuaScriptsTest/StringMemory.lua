print("相同短字符")

local before = collectgarbage("count")

for i = 1,10000 do
	local string = "100000000000000" .. "0"
end

local after =  collectgarbage("count")

print("before:" .. before .. "--after:" .. after)


print("相同长字符")

before = collectgarbage("count")

for i = 1,10000 do
	local string = "10000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" .. "0"
end

after =  collectgarbage("count")

print("before:" .. before .. "--after:" .. after)

print("不同短字符")

before = collectgarbage("count")

for i = 1,10000 do
	local string = "100000000000000" .. i
end

after =  collectgarbage("count")

print("before:" .. before .. "--after:" .. after)

print("不同长字符")

before = collectgarbage("count")

for i = 1,10000 do
	local string = "10000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" .. "i"
end

print("before:" .. before .. "--after:" .. after)


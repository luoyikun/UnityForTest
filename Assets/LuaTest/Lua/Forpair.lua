a = 2

function func()

 a = b
end
xpcall(func,debug.traceback)

a = 1

print(a)

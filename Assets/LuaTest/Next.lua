tab = {1,2,3}
print(next(tab))

tab = {1,nil,3}
print(next(tab,1))

tab = {1,nil,nil}
print(next(tab,1))

tab = {nil,2,nil}
print(next(tab))

tab = {}
print(next(tab))

tab = nil

print(not tab)

tab = true
print(not tab)
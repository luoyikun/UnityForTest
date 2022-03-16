
--- ���ñհ�ʵ��iterator,iterator��һ��������ÿ�ε��ö������һ���µıհ�,�ñհ��ڲ�������upvalue(t,i,n)
--- ���ÿ����һ�θú�����������հ�����ô�ñհ��ͻ���ݼ�¼��һ�ε�״̬���Լ�����table�е���һ��Ԫ��
function iterator(t)
    local i = 0
    local n = #t
    return function()
        i = i + 1
        if i <= n then --ʵ���˱����ն�table
            return t[i]
        end
    end
end

testTable = {nil,1,2,3,nil,"a","b"}

print("table����:" .. #testTable)
print("table���� 2:" .. select('#', testTable))


-- while��ʹ�õ�����
iter1 = iterator(testTable) --���õ���������һ���հ�
while true do
    local element = iter1()
    if nil == element then
        break;
    end
    print(element)
end

-- for��ʹ�õ�����
for element in iterator(testTable) do --- �����iterator()��������ֻ�ᱻ����һ�β���һ���հ������������ÿһ�ε��������øñհ������������ǹ�������
    print(element)
end

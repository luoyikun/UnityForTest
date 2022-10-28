function GetDengJiZhengByNear5(num)
    local geWei = num % 10
	local zhengPart,xiaoPart = math.modf(num/10)
	local xiaoShu = num/10
    if geWei <= 2 then
		print("0.2")
        return zhengPart *10
    elseif geWei >= 3 and geWei <= 7 then
		print("0.3 - 0.7")
        return zhengPart *10 + 5
    elseif geWei >= 8 then
		print("0.8")
        return round(xiaoShu) * 10
    end
end

--四舍五入
function round(value)
    value = tonumber(value) or 0
    return math.floor(value + 0.5)
end


num = 59
print(GetDengJiZhengByNear5(num))

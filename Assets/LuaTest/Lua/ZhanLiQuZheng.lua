
function GetNumDigit(num)
    local result = num
    local digit = 0
    while(result > 0) do
        result =math.modf(result / 10)
        digit = digit +1
    end
    return digit
end

local num =10920

local digit = GetNumDigit(num)
print("digit:" .. digit)
local digit2 = digit - 3
print("digit2:" .. digit2)
local ten = 10 ^ digit2
print(" ten:" ..  ten)
local qianLiangWei = math.modf(num / ten)
print("qianLiangWei:" .. qianLiangWei)

local qianLiangWeiXiaoShu = qianLiangWei/10
local qianLiangWeiXSZ,qianLiangWeiXSX =math.modf(qianLiangWeiXiaoShu)
print(qianLiangWeiXSZ .. "-->" .. qianLiangWeiXSX)

function ShiSheLiuRu(XiaoShu,XiaoPart)
	if XiaoPart == 0.5 then
		return XiaoShu
	else
		return 	round(XiaoShu)
	end
end


--四舍五入
function round(value)
    value = tonumber(value) or 0
    return math.floor(value + 0.5)
end


local qianLiangWei5BeiShu = ShiSheLiuRu(qianLiangWeiXiaoShu,qianLiangWeiXSX)
print(qianLiangWei5BeiShu)

local finial = qianLiangWei5BeiShu * ten * 10
print (finial)

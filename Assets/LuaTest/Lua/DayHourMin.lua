function GetDayHourMinByMin(time)
    local day = math.modf(time/(24*60))
    local hour = math.modf((time%(24*60)) / 60)
    local mins = math.modf((time%(24*60)) % 60)
    return day,hour,mins
end

day = 1
hour = 3
mins = 50

total = day * 24*60 + hour * 60 + mins
print(GetDayHourMinByMin(total))

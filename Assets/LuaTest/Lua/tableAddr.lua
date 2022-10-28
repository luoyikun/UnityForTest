local st, gt = setmetatable, getmetatable
    local mt = {}
    local default = {
        ["name"] = "zblade",
        ["telphone"] = 123456,
        ["id"] = 7,
        ["school"] = "uestc"
    }
    --复写index
    mt.__index = function(tbl, key)
        local val = rawget(tbl, key) --从原始表中找
        if val then
            return val
        else
            return default[key] --没找到去默认表中找
        end
    end

    local func = function (tbl, key)
        local nk, nv = next(default, key)

			return nk,nv


    end
    --复写 pairs
    mt.__pairs = function (tbl, key)
       return func, tbl, nil
    end

    local test = {
    ["id"] = 8
    }

    setmetatable(test, mt)
    --测试pairs的复写
    for k, v in pairs(test) do
        print(v)
    end
   -- output: 8 , uestc, zblade, 123456

    --测试 index的复写
    --print(test.id)
    --print(test.name)
    --print(test.school)
    --print(test.telphone)
   --output: 8 , uestc, zblade, 123456

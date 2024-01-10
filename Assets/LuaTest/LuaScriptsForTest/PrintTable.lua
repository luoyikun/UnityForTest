function PrintTable(node, noPrint,simple)


    if not node or type(node) ~= "table" then
        local s = tostring(node)
        if not noPrint then
            --Log.Info(s)
        end
        return debug.traceback(s)
    end

    -- to make output beautiful
    local function tab(amt)
        local str = ""
        for i = 1, amt do
            str = str .. "\t"
        end
        return str
    end
    local bufferHelper = {}
    local function __P(_node,_depth,_buffer)
        bufferHelper[_node] = true
        table.insert(_buffer,tab(_depth-1).." {\n")
        for k,v in pairs(_node)  do
            local key
            if (type(k) == "number" or type(k) == "boolean") then
                key = "[" .. tostring(k) .. "]"
            else
                key = "['" .. tostring(k) .. "']"
            end

            local output
            if (type(v) == "number" or type(v) == "boolean" or type(v) == "string") then
                output = tab(_depth) .. key .. " = " .. tostring(v).."\n"
                table.insert(_buffer,output)
            elseif (type(v) == "table" and tostring(k) ~= "instances") then
                if not simple then
                    output = tab(_depth) .. key .. " = " .. tostring(v).."\n"
                    table.insert(_buffer,output)
                    if bufferHelper[v] == nil then
                        __P(v,_depth + 1,_buffer)
                    end
                else
                    if tostring(k) ~= "class" then
                        output = tab(_depth) .. key .. " = " .. tostring(v).."\n"
                        table.insert(_buffer,output)
                        if bufferHelper[v] == nil then
                            __P(v,_depth + 1,_buffer)
                        end
                    end
                end
            elseif  tostring(v) ~= "instances" then
                if not simple  then
                    output = tab(_depth) .. key .. " = " .. tostring(v).."\n"
                    table.insert(_buffer,output)
                end
            end
        end
        table.insert(_buffer,tab(_depth-1).." }\n")
    end
    local output_str = ""
    local depth = 1
    local buffer = {}
    __P(node,depth,buffer)
    table.insert(buffer, debug.traceback())
    output_str = table.concat(buffer)

    if not noPrint then
        --Log.Info(output_str);
        print(output_str)
    end
    return output_str;
end

-- tabA = { instances = { y = 2} ,x = 1}

-- for k,v in pairs(tabA)  do
--     print(k,v)
-- end
-- PrintTable(tabA)
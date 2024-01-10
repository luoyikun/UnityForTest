local t = {
    name = "Alice",
    age = 25,
    ["likes programming"] = true,
  }
  
  -- 使用[""]形式访问表成员
  print(t["name"])                -- 输出: Alice
  print(t["age"])                 -- 输出: 25
  print(t["likes programming"])    -- 输出: true
  
  -- 使用.形式访问表成员
  print(t.name)                   -- 输出: Alice
  print(t.age)                    -- 输出: 25
  -- print(t.likes programming)    -- 错误，因为键包含空格，需要使用[""]形式
  
---
--- 作者：罗翊坤
--- 日期：2023/05/30
---

---@class HuDaoShenShouCircleModule 护岛神兽缩圈
local HuDaoShenShouCircleModule = class("HuDaoShenShouCircleModule", Module)

--第一个圈配置
local firstRadius = LuaEntry.Config:GetInt("RedCircle1_Radius",220)
local firstX = LuaEntry.Config:GetInt("RedCircle1_X",268620) / 1000
local firstY = LuaEntry.Config:GetInt("RedCircle1_Y",26000) / 1000
local firstZ = LuaEntry.Config:GetInt("RedCircle1_Z",230630) / 1000

--第二个圈配置
local secondRadius = LuaEntry.Config:GetInt("WhiteCircle1_Radius",100)
local secondX = LuaEntry.Config:GetInt("WhiteCircle1_X",268620) / 1000
local secondY = LuaEntry.Config:GetInt("WhiteCircle1_Y",26000) / 1000
local secondZ = LuaEntry.Config:GetInt("WhiteCircle1_Z",230630) / 1000

--第三个圈配置
local thirdRadius = LuaEntry.Config:GetInt("WhiteCircle2_Radius",100)
local thirdX = LuaEntry.Config:GetInt("WhiteCircle2_X",268620) / 1000
local thirdY = LuaEntry.Config:GetInt("WhiteCircle2_Y",26000) / 1000
local thirdZ = LuaEntry.Config:GetInt("WhiteCircle2_Z",230630) / 1000

--第一次缩圈速度
local firstTime = LuaEntry.Config:GetInt("CircleChangeTime1",30) --第一次缩圈总时间

--第二次缩圈速度
local secondTime = LuaEntry.Config:GetInt("CircleChangeTime2",30)

local effectID = 36187

function HuDaoShenShouCircleModule:Init()
    self.TinyEvent = TinyEvent.New()
    self:DataInit()
    self:SubscribeEvent(PS.ChangeSceneCompleteEventArgs.EventId, handler(self, self.OnChangeSceneComplete))
end

function HuDaoShenShouCircleModule:OnShowEffectSuccess(sender, gameEventArgs)
    if  self.effectSerialId == gameEventArgs.EntityId then
        local entity = GameEntry.Entity:GetGameEntity(self.effectSerialId);
        self.effectTrans = entity.transform
        --只需监听到场景特效加载好，不需要再监听
        self.isSubscribeShowEntityEffectSuccess = false
        self:UnsubscribeEvent(UGF.ShowEntityEffectSuccessEventArgs.EventId)
    end
end

function HuDaoShenShouCircleModule:Clear()
    self:LeaveBattle()
end

function HuDaoShenShouCircleModule:OnChangeSceneComplete(sender, gameEventArgs)
    if self:IsHuDaoMap() == false then
        self:LeaveBattle()
    end
end

---LeaveBattle 离开战场
function HuDaoShenShouCircleModule:LeaveBattle()
    --清掉场景特效
    if self.effectSerialId ~= 0 then
        GameEntry.Entity:HideGameEntity(self.effectSerialId)
    end
    if self.isSubscribeShowEntityEffectSuccess == true then
        self.isSubscribeShowEntityEffectSuccess = false
        self:UnsubscribeEvent(UGF.ShowEntityEffectSuccessEventArgs.EventId)
    end
end
function HuDaoShenShouCircleModule:FireUpdateCircle()
    self.TinyEvent:Fire({ eventName = "UpdateCircle"})
end

function HuDaoShenShouCircleModule:Start(circleIdx)
    if self.isSubscribeShowEntityEffectSuccess == false and self.effectSerialId == 0 then
        self.isSubscribeShowEntityEffectSuccess = true
        self:SubscribeEvent(UGF.ShowEntityEffectSuccessEventArgs.EventId, handler(self, self.OnShowEffectSuccess))
    end

    if self.effectSerialId == 0 then
        self:CreateEffect()
    end

    self:DataCicleInit(circleIdx)
    self.circleIdx = circleIdx

    self.startTime = Time.time
    self:RegisterUpdate()

end

function HuDaoShenShouCircleModule:Stop()
    self:UnregisterUpdate()
end

function HuDaoShenShouCircleModule:EffectMoveAndScale()
    if self.effectTrans ~= nil then
        self.effectTrans:SetTransPosAndScale(self.circleData.big.pos.x,self.circleData.big.pos.y,self.circleData.big.pos.z,
            self.circleData.big.r*2,1,self.circleData.big.r*2
        )
    end
end

function HuDaoShenShouCircleModule:OnUpdate(dt, udt)
    local data = self.circleData
    if data then
        local diffBigR = data.rSpeed * dt
        data.big.r = data.big.r - diffBigR
        if data.big.r > data.small.r +  data.disCenter then
            self.circleState = EnHuDaoCircleMoveState.ToIntersect
            --self:FireUpdateCircle()
            self:EffectMoveAndScale()
        elseif data.big.r > data.small.r and data.big.r <=  data.small.r +  data.disCenter then
            self.circleState = EnAgreementFormType.ToSmall
            data.big.pos.x = data.big.pos.x + diffBigR * data.cos
            data.big.pos.z = data.big.pos.z + diffBigR * data.sin
            --self:FireUpdateCircle()
            self:EffectMoveAndScale()
        else
            self.circleState = EnHuDaoCircleMoveState.Stop
            local totalTime = Time.time - self.startTime
            Log.Info("缩圈结束，耗时：{0}",totalTime)
            self:UnregisterUpdate()
        end
    end

end

---@class EnHuDaoCircleMoveState 毒圈运动状态
EnHuDaoCircleMoveState = {
    Stop = 0,
    ToIntersect = 1,
    ToSmall = 2,
}

function HuDaoShenShouCircleModule:HandleCircle(data)
    self.circleX = data.circleX
    self.circleY = data.circleY
    self.circleRadius = data.circleRadius
    self.circleState = data.moveState
    self.TinyEvent:Fire({ eventName = "UpdateHuDaoCircle"})
end

function HuDaoShenShouCircleModule:SetRegionalMap(map)
    self.RegionalMap = map
    Log.Info("HuDaoShenShouCircleModule:SetRegionalMap")
end

function HuDaoShenShouCircleModule:DataInit()
    self.circleState = EnHuDaoCircleMoveState.Stop
    self.circleIdx = 1 --默认是第一阶段
    self.circleData = nil --当前的缩圈数据
    self.effectSerialId = 0 --特效生产序列id
    self.effectTrans = nil
    self.isSubscribeShowEntityEffectSuccess = false
end

---DataInit 数据初始化，只在使用时读
function HuDaoShenShouCircleModule:DataCicleInit(idx)
    local tabCircle = {}
    if idx == 1 then
        tabCircle.big = {}
        tabCircle.big.r = firstRadius
        tabCircle.big.pos =  {x= firstX,y = firstY,z = firstZ}

        tabCircle.small = {}
        tabCircle.small.r = secondRadius
        tabCircle.small.pos = { x = secondX,y = secondY, z = secondZ}
        tabCircle.time = firstTime
    elseif idx == 2 then
        tabCircle.big = {}
        tabCircle.big.r = secondRadius
        tabCircle.big.pos = { x = secondX,y = secondY, z = secondZ}

        tabCircle.small = {}
        tabCircle.small.r = thirdRadius
        tabCircle.small.pos = { x = thirdX,y = thirdY, z = thirdZ}
        tabCircle.time = secondTime
    end
    tabCircle.rSpeed =( tabCircle.big.r - tabCircle.small.r ) / tabCircle.time
    tabCircle.disCenter = Vector3.Distance( tabCircle.big.pos, tabCircle.small.pos)
    self:CalcuSinCos(tabCircle)

    self.circleData = tabCircle
end

function HuDaoShenShouCircleModule:CalcuSinCos(data)
    if data.big.pos.x == data.small.pos.x and data.big.pos.z ~= data.small.pos.z  then
        data.cos = 0
        data.sin = 1
    elseif data.big.pos.x ~= data.small.pos.x and data.big.pos.z == data.small.pos.z  then
        data.cos = 1
        data.sin = 0
    elseif  data.big.pos.x == data.small.pos.x and data.big.pos.z == data.small.pos.z  then
        data.cos = 0
        data.sin = 0
    else
        --local k = ( data.small.pos.z - data.big.pos.z ) / ( data.small.pos.x - data.big.pos.x)
        --k  = UE.Mathf.Abs(k)
        --local angle = UE.Mathf.Atan(k)
        --data.cos = UE.Mathf.Cos(angle)
        --data.sin = UE.Mathf.Sin(angle)
        local b = data.small.pos.z - data.big.pos.z
        local a = data.small.pos.x - data.big.pos.x
        local c = data.disCenter
        data.cos = a / c
        data.sin = b / c
    end
end

function HuDaoShenShouCircleModule:CalcuPlusMinus(data)
    if data.small.pos.x > data.big.pos.x then
        data.xDiff = 1
    else
        data.xDiff = -1
    end

    if data.small.pos.z > data.big.pos.z then
        data.zDiff = 1
    else
        data.zDiff = -1
    end
end

function HuDaoShenShouCircleModule:GetCurData()
    if self.circleData == nil then
        self:DataCicleInit(self.circleIdx)
    end
    return self.circleData
end

function HuDaoShenShouCircleModule:IsHuDaoMap()
    ----先返回true测试
    return true

    --if LuaEntry.ServerMainId == LuaConfig.SceneServerId.FuBen_HuDaoShenShou  then
    --    return true
    --end
    --return false
end

function HuDaoShenShouCircleModule:CreateEffect()
    self.effectSerialId = GameEntry.Entity.GenerateSerialId()
    EffectUtility.ShowEffectBySerialId(effectID, self.effectSerialId,self:GetCurData().big.pos, Quaternion.identity);
end
return HuDaoShenShouCircleModule


-- 作者：黄伟
-- 时间：2018/3/15
-- 描述：雷达地图
-- ======================================

local RadarMap = class("RadarMap", LuaNGUIForm)
--[[
-- 边距
local Margin = {
	Top = 85,
	Down = 72,
	Left = 85,
	Right = 75,
}
]]
--local TimeRefreshTime = 10
--local TimeRefreshTimer = TimeRefreshTime

function RadarMap:OnInit( uiFormParams )
	self.uiFormParams = uiFormParams
	self.RegionalObj = self.GameObjects[1]

	self.map = self.Transforms[0]	-- 地图

	self.dot = self.Transforms[1]	-- 点
	self.direction = self.Transforms[2]	-- 方向

	--self.dots = {}
    self.spritePool = {}

	self.EagleRegionalMapDot = require("UI/MapForm/RegionalMap/EagleRegionalMapDot")
	self.EagleRegionalMapDot.TargetObj = self.UIElements[0].gameObject

	self.EagleSpyDots = {}
	self.EagleDotPool = LuaObjectPool.New(self.EagleRegionalMapDot,"EagleRegionalMapDot")
	self.EagleDotPool:SetCapacity(100)

	self.SpriteCollection = self.GameObjects[3]:GetComponent(typeof(CS.UISpriteCollection))
	self.SpriteCollection.UseSortByDepth = true
	self.labelLine = self.UILabels[1]
	self.labelScene = self.UILabels[0]

	self.labelSceneNoLine = self.UILabels[4]
	self.halfWidth = 0
	self.halfHeight = 0
	self.scale = 0
	self.offsetX = 0
	self.offsetY = 0
	self.curLine = 0
	--self.EagleNewSpyTipObj = self.GameObjects[2]

	--self.EagleNewSpyTipLabel = self.UILabels[3]
	self.hideLine = false
	local maphelper = require "UI/MapForm/RadarMap/RadarMapHelper"
	self.RadarMapHelper = maphelper.New()
	self.OffsetObj = self.GameObjects[2]
	self.Frame_DungeonsObj = self.GameObjects[4]
	self.FrameObj = self.GameObjects[6]

	--时间、天气
	--TimeRefreshTimer = TimeRefreshTime
	self.setTimePeriodSelf = false
	self.sceneTimeLbl = self.UILabels[2]
	self.weatherSprite = self.UISprites[0]
	self.sceneTimeBgSprite = self.UISprites[1]

	--毒圈遮罩
	self.UI_CircleMaskEffect = self.GameObjects[7]:GetComponent(typeof(CS.UICircleMaskEffect))
	self.objCircle = self.GameObjects[7]
	self.TextureCircleMask = self.UITextures[1]


end

function RadarMap:OnOpen(uiFormParams)
	if uiFormParams.UserData and uiFormParams.UserData.YYZJ then
		self.YYZJ = true
	end
	if self:InitData() then
		self.RadarMapHelper:Init()
		self:RegisterUpdate(0.5)
		--self:RegisterUpdate()
		self:SubscribeEvent(PS.CommonEventArgs.EventId, handler(self, self.OnCommonEvent))

		self:SubscribeEvent(UGF.ShowEntitySuccessEventArgs.EventId, handler(self, self.OnShowEntitySuccess))
		self:SubscribeEvent(UGF.HideEntityCompleteEventArgs.EventId, handler(self, self.OnHideEntityComplete))

		--LuaEntry.SLGBattleModule.TinyEvent:Subscribe(self.OnSLGEvent, self)
		self:SubscribeEvent(PS.CharacterDataUpdateAllEventArgs.EventId, handler(self, self.OnCharacterDataUpdateAll))
		self.SLGDot = {}
		--self:RefreshSLGDot()
		--self:UpdateEagleSpyPot()
		LuaEntry.GuildDeclareWarModule.TinyEvent:Subscribe(self.OnReceiveGuildDeclareWarEvent,self)
		LuaEntry.GuildModule.tinyEvent:Subscribe(self.OnGuildTinyEvent, self)
		LuaEntry.YangYingZhuJianModule.TinyEvent:Subscribe(self.YYZJTinyEvent,self)
		LuaEntry.MapModule.TinyEvent:Subscribe(self.OnMapEvent, self)
		self:SubscribeEvent(PS.ChangeSceneTimeEventArgs.EventId, handler(self,self.OnChangeSceneTime))
		self:SubscribeEvent(PS.ChangeWeatherEventArgs.EventId, handler(self,self.OnChangeWeather))
		self.bOpen = true

		self.setTimePeriodSelf = GameEntry.TimeWeather.TimeWeatherEnabled
		self:SetWeather(GameEntry.TimeWeather.CurWeatherId)
		self:SetSceneTimePeriod(GameEntry.TimeWeather.CurSceneTime)
		self:SetObjCircleActiveState()
	end

end

function RadarMap:OnResume()
	self.OffsetObj:SetActive(true)
	--self:UpdateEagleSpyPot()
end

function RadarMap:OnGuildTinyEvent(params)
	if params.eventName == "GuildUpdateSelfGuildInfo" then
        self:OnUpdateSceneName()
		local entities = self.RadarMapHelper:GetAllEntities()		
		for i , item in pairs(entities) do
			item.IsdotFlag = false
		end
    end
end

function RadarMap:OnReceiveGuildDeclareWarEvent(params)
	if params.eventName == "GuildDeclareStatusHandle" then
		--对战状态刷新
		self:OnUpdateSceneName()
		local entities = self.RadarMapHelper:GetAllEntities()
		for i , item in pairs(entities) do
			item.IsdotFlag = false
		end
	end
end

function RadarMap:OnCharacterDataUpdateAll(sender,gameEventArgs)
	if gameEventArgs:CheckFlag(CharacterDataUpdateType.PvpValue) or
		gameEventArgs:CheckFlag(CharacterDataUpdateType.TeamId) or
		gameEventArgs:CheckFlag(CharacterDataUpdateType.CampId) or
		gameEventArgs:CheckFlag(CharacterDataUpdateType.XuanZhanList) or
		gameEventArgs:CheckFlag(CharacterDataUpdateType.FanJiList) or
		gameEventArgs:CheckFlag(CharacterDataUpdateType.PvpMode) or
		gameEventArgs:CheckFlag(CharacterDataUpdateType.ReputationId) or
		gameEventArgs:CheckFlag(CharacterDataUpdateType.GuildId) or
		gameEventArgs:CheckFlag(CharacterDataUpdateType.AIType) or
		gameEventArgs:CheckFlag(CharacterDataUpdateType.ZoneWorldId)
	then
		local entities = self.RadarMapHelper:GetAllEntities()
		local entityId = gameEventArgs.EntityId
		if entities[entityId] then
			entities[entityId].IsdotFlag = false
		end	
		if entityId == LuaEntry.GetEntityId() then
			for i , item in pairs(entities) do
				item.IsdotFlag = false
			end
		end
	end
	
	if gameEventArgs:CheckFlag(CharacterDataUpdateType.AvatarKey) then
		local entities = self.RadarMapHelper:GetAllEntities()
		local tmpEntityData = entities[gameEventArgs.EntityId]

		if tmpEntityData and not ObjectIsNil(tmpEntityData.EntityLogic)
		 and tmpEntityData.EntityLogic.LogicType == EntityLogicType.Monster then
			tmpEntityData .IsdotFlag = false
			local entity = tmpEntityData.EntityLogic
			tmpEntityData.IsPlayer = entity.LogicType == EntityLogicType.Player
				or LuaEntry.ClientNpcModule:IsMercenaryByEntity(entity)
		end	
	end
end
--[[
function RadarMap:OnCharacterDataUpdate(sender, gameEventArgs)
	local updateType = gameEventArgs.UpdateType
	if updateType == CharacterDataUpdateType.PvpValue or
		updateType == CharacterDataUpdateType.TeamId or
		updateType == CharacterDataUpdateType.CampId or
		updateType == CharacterDataUpdateType.XuanZhanList or
		updateType == CharacterDataUpdateType.FanJiList or
		updateType == CharacterDataUpdateType.PvpMode or
		updateType == CharacterDataUpdateType.ReputationId or
		updateType == CharacterDataUpdateType.GuildId  or
		updateType == CharacterDataUpdateType.AIType 
	then
		local entities = self.RadarMapHelper:GetAllEntities()
		local entityId = gameEventArgs.EntityId
		if entities[entityId] then
			entities[entityId].IsdotFlag = false
		end	
		if entityId == LuaEntry.GetEntityId() then
			for i , item in pairs(entities) do
				item.IsdotFlag = false
			end
		end
	elseif updateType == CharacterDataUpdateType.AvatarKey then
		local entities = self.RadarMapHelper:GetAllEntities()
		local tmpEntityData = entities[gameEventArgs.EntityId]

		if tmpEntityData and not ObjectIsNil(tmpEntityData.EntityLogic)
		 and tmpEntityData.EntityLogic.LogicType == EntityLogicType.Monster then
			tmpEntityData .IsdotFlag = false
			local entity = tmpEntityData.EntityLogic
			tmpEntityData.IsPlayer = entity.LogicType == EntityLogicType.Player
				or LuaEntry.ClientNpcModule:IsMercenaryByEntity(entity)
		end	
	end
end
]]
function RadarMap:OnClose()
	self.RadarMapHelper:Release()
	self.halfWidth = 0
	self.halfHeight = 0
	self.scale = 0
	self.offsetX = 0
	self.offsetY = 0
	if 	self.bOpen then
		LuaEntry.GuildDeclareWarModule.TinyEvent:Unsubscribe(self.OnReceiveGuildDeclareWarEvent,self)
		LuaEntry.GuildModule.tinyEvent:Unsubscribe(self.OnGuildTinyEvent, self)
		LuaEntry.YangYingZhuJianModule.TinyEvent:Unsubscribe(self.YYZJTinyEvent,self)
		LuaEntry.MapModule.TinyEvent:Unsubscribe(self.OnMapEvent, self)
		--LuaEntry.SLGBattleModule.TinyEvent:Unsubscribe(self.OnSLGEvent, self)
		self.bOpen = false
	end
	self.YYZJ = false
	self:ClearEagleSpyPot()
	self.objCircle:SetActive(false)
end

function RadarMap:InitData()
	if ObjectIsNil(LuaEntry.GetMyPlayerCharacter()) then
		Log.Warning("Open radar map failed, Can't get player.")
		self:Close()
		return false
	end

	if GameEntry.Scene.MainCamera then
		self.cameraTrans = GameEntry.Scene.MainCamera.transform
	else
		Log.Warning("Open radar map failed, Can't get GameEntry.Scene.MainCamera.")
		self:Close()
		return false
	end

	local mapData = DRMap[LuaEntry.ServerMainId]
	if mapData == nil then
		Log.Error("Can't find map data {0} in Map.txt!", LuaEntry.ServerMainId)
		self:Close()
		return false
	end
	self:OnUpdateSceneName(mapData)
	self.SpriteCollection:Clear()
	local textureMap = self.UITextures[0]
	self.TextureMap = textureMap

	--textureMap.width = mapData.Width
	--textureMap.height = mapData.Height
	self.form:LoadAsync(textureMap,mapData.MapTexture)
	self.halfWidth = mapData.Width/2
	self.halfHeight = mapData.Height/2
	self.scale = mapData.Scale
	self.offsetX = mapData.OffsetX - self.halfWidth
	self.offsetY = mapData.OffsetY - self.halfHeight
	self.mapData = mapData
	return true
end


function RadarMap:OnUpdateSceneName(mapData)
	if mapData == nil then
		mapData = DRMap[LuaEntry.ServerMainId]
	end
	if mapData == nil then
		Log.Warning("Can't find map data {0} in Map.txt!", LuaEntry.ServerMainId)
		self:Close()
		return
	end
	self.Frame_DungeonsObj:SetActive(mapData.RaderNameType ~= GetRadarMapType.Config)
	self.FrameObj:SetActive(mapData.RaderNameType == GetRadarMapType.Config)
	--self.labelSceneNoLine.gameObject:SetActive(mapData.RaderNameType ~= GetRadarMapType.Config)
	--self.labelScene.gameObject:SetActive(mapData.RaderNameType == GetRadarMapType.Config)
	--self.labelLine.gameObject:SetActive(mapData.RaderNameType == GetRadarMapType.Config)
	if mapData.RaderNameType == GetRadarMapType.Config then
		self.labelScene.text = Lang.GetString(mapData.RaderName)
		self.curLine = LuaEntry.MapModule:GetCurLine()
		self.labelLine.text = string.format(Lang.GetString("RadarMap.LineName"), self.curLine)
	elseif mapData.RaderNameType == GetRadarMapType.GuildName then
		self.labelSceneNoLine.text = LuaEntry.GuildDeclareWarModule:GetGuildSceneName()
	elseif mapData.RaderNameType == GetRadarMapType.ConfigNoLine then
		if LuaEntry.MapModule.ModelData.Active then
			self.labelSceneNoLine.text = Lang.GetFormatString("BangHuiFuBen_211",LuaEntry.MapModule:GetMapModelName(), Lang.GetString(mapData.RaderName))
		else
			self.labelSceneNoLine.text = Lang.GetString(mapData.RaderName)
		end
	end
end

--function RadarMap:OnStartChangeScene()
	--self.dots = {}
--end

function RadarMap:OnChangeSceneComplete()
	self:InitData()
	self.setTimePeriodSelf = GameEntry.TimeWeather.TimeWeatherEnabled
	self:SetSceneTimePeriod(GameEntry.TimeWeather.CurSceneTime)
	self:SetWeather(GameEntry.TimeWeather.CurWeatherId)
	--self:UpdateEagleSpyPot()
	--是否在神兽地图并更新缩圈
	self:SetObjCircleActiveState()
end

function RadarMap:SetObjCircleActiveState()
	if LuaEntry.HuDaoShenShouCircleModule:IsHuDaoMap() then
		self.objCircle:SetActive(true)
	else
		self.objCircle:SetActive(false)
	end
end
function RadarMap:UpdateHuDaoCircle(px,py)
	if LuaEntry.HuDaoShenShouCircleModule:IsHuDaoMap() then

		local data = LuaEntry.HuDaoShenShouCircleModule:GetCurData()
		local centerSmallInMapX,centerSmallInMapY = self:ToMiniMapPositionVector2(data.small.pos, px, py)
		local pointSmallInR = {x = data.small.pos.x + data.small.r, y = data.small.pos.y, z = data.small.pos.z}
		local pointSmallInRInMapX,pointSmallInRInMapY = self:ToMiniMapPositionVector2(pointSmallInR,px,py)

		local centerInMapX,centerInMapY = self:ToMiniMapPositionVector2(data.big.pos, px, py)
		local pointInR = {x = data.big.pos.x + data.big.r, y = data.big.pos.y, z = data.big.pos.z}
		local pointInRInMapX,pointInRInMapY = self:ToMiniMapPositionVector2(pointInR, px, py)
		self.UI_CircleMaskEffect:SetDataAll(centerInMapX,centerInMapY,
				pointInRInMapX,pointInRInMapY,
				centerSmallInMapX,centerSmallInMapY,
				pointSmallInRInMapX,pointSmallInRInMapY
		)

	end
end



function RadarMap:OnUpdate( dt, udt )
	--[[TimeRefreshTimer = TimeRefreshTimer + dt
	if TimeRefreshTimer >= TimeRefreshTime then
		TimeRefreshTimer = 0
		TimeRefreshTime = 60 - TimeUtil:GetServerSecond()
		if self.setTimePeriodSelf then
			self:SetSceneTimePeriod(GameEntry.TimeWeather.CurSceneTime)
		end
	end]]

	local player = LuaEntry.GetMyPlayerCharacter()
	if ObjectIsNil(player) then

		return
	end

	if not player.Available or ObjectIsNil(self.cameraTrans) then

		return
	end

	local px,py = self:UpdatePlayer(player)

	self:UpdateOther(px,py)

	self:UpdateEagleSpyPot(px,py)

	self:UpdateHuDaoCircle(px,py)
end


function RadarMap:OnShowEntitySuccess( sender, event )
	self.RadarMapHelper:OnShowEntitySuccess(sender, event)
end

function RadarMap:OnHideEntityComplete( sender, event )
	self.RadarMapHelper:OnHideEntityComplete(sender, event)
end

function RadarMap:OnDestroy()
	RadarMap.super.OnDestroy(self)
end

function RadarMap:OnClick(go)
	if go == self.RegionalObj then
		self:OnRegionalMapClicked()
	end
end

function RadarMap:OnRegionalMapClicked()
	LuaEntry.UIAssistant:Open(UIFormEnum.RegionalMap)
end

function RadarMap:OnCommonEvent( sender, event )
	if self.curLine ~= LuaEntry.MapModule:GetCurLine() then
		self:OnUpdateSceneName()
	end
	if event.EventType == CommonEventType.MapOffSet then
		self.offsetX = event.FloatArgs[0]
		self.offsetY = event.FloatArgs[1]
	end
end

function RadarMap:YYZJTinyEvent(params)

	if params.eventName == "GCYYZJNpcsInfo" then
		--self:ClearEagleSpyPot()
		--self:UpdateEagleSpyPot()
	elseif params.eventName == "GCYYZJAddNpcInfo" then
		--self:UpdateEagleSpyPot()
	elseif params.eventName == "YangYingDiscoverMissionRemove" then
		--self:UpdateEagleSpyPot()
	elseif params.eventName == "GCYYZJUpdateNpcStatus" then
		--self:UpdateEagleSpyPot()
	elseif params.eventName == "EnableFocusNpc" then
		if self.YYZJ then
			self.OffsetObj:SetActive(false)
		end
	elseif params.eventName == "DisableFocusNpc" then
		if self.YYZJ then
			self.OffsetObj:SetActive(true)
		end
	end
end

--function RadarMap:OnSLGEvent(params)
	--if params.type == "MemberStates" then
		--self:RefreshSLGDot(params.packet)
	--end
--end



function RadarMap:ClearEagleSpyPot()

	local count = 0
	for _, dot in pairs(self.EagleSpyDots) do
		self.EagleDotPool:Unspawn(dot)
		count = count + 1
	end

	if count == 0 then
		return
	end

	self.EagleSpyDots = {}
	self.EagleDotPool:Clear()
end

--神鹰小地图
function RadarMap:UpdateEagleSpyPot(px,py)

	if  LuaEntry.ServerMainId == LuaEntry.YangYingZhuJianModule.TargetSceneId and LuaEntry.YangYingZhuJianModule:IsInKillActivity() then
		local npcs = LuaEntry.YangYingZhuJianModule.NpcObj
		self.EagleRegionalMapDot.TargetObj = self.UIElements[0].gameObject
		local count = 0
		for id, data in pairs(npcs) do
			count = count + 1
			local dot = self.EagleSpyDots[id]
			if dot then
				dot:SetPosition(data.x * self.scale + self.offsetX - px, data.z * self.scale + self.offsetY - py)				
				dot.used = true
			else
				local dotName = "YYZJ"..tostring(id)
				dot = self.EagleDotPool:Spawn(self.form,dotName,id)
				dot:SetParent(self.map)
				self.EagleSpyDots[id] = dot
				dot:SetPosition(data.x * self.scale + self.offsetX - px, data.z * self.scale + self.offsetY - py)
				dot.gameObject:SetActive(true)
			end
			dot:Refresh(data)
		end

	else
		for id, dot in pairs(self.EagleSpyDots) do
			self.EagleDotPool:Unspawn(dot,true)
			self.EagleSpyDots[id] = nil
		end
	end
end



-- 更新其他
function RadarMap:UpdateOther(px,py)

	-- 移除
	local hidedEntityIds = self.RadarMapHelper:GetHidedEntityIds()
	if #hidedEntityIds > 0 then
		for i=1,#hidedEntityIds do
			self.SpriteCollection:RemoveSprite(hidedEntityIds[i])
		end
		self.RadarMapHelper:ClearHidedEntityIds()
	end

	-- 添加或修改
	local entities = self.RadarMapHelper:GetAllEntities()

	for entityId, entity in pairs(entities) do
		-- 这里判空是因为update时序问题
			local pos = LuaEntry.GetEntityPosition(entityId)
			if not entity.IsdotFlag then
				entity.IsdotFlag = self:GetDotUnused(entity, self:ToMiniMapPositionVector2(pos, px, py))
			else
				local x, y = self:ToMiniMapPositionVector2(pos, px, py)
				self.SpriteCollection:SetPositionXY(entity.Id, x, y)
			end
	
	end
end

function RadarMap:GetDotUnused(entity, posX, posY)
	local sprite ,depth = self:GetSpriteName(entity)

	if sprite then
		self.SpriteCollection:AddXY(entity.Id, sprite , depth, posX, posY, 14,14)
	end

	return true
end
local DepthRadar =
{
	EnemyPlayer = 6,--其他玩家敌对角色
	MemberPlayer = 5,--其他玩家中立角色
	OtherPlayer = 4,--其他玩家角色

	EnemyMonster = 3,--其他NPC敌对角色
	NeutralMonster = 2,--其他玩家中立角色
	FriendMonster = 1,--其他玩家角色
}


function RadarMap:GetSpriteName(entity)
	local myPlayerData = LuaEntry.GetMyPlayerCharacterData()
	--if ObjectIsNil(myPlayerData) then
		--return
	--end
    if entity.IsPlayer then
		if LuaEntry.SaiMaGameModule:IsGameStar() then
			if myPlayerData.CompelNameColorId == entity.Data.CompelNameColorId then
				return "HUD_Icon_Radar10",DepthRadar.OtherPlayer
			else
				return "HUD_Icon_Radar02",DepthRadar.EnemyPlayer
			end
		end
        if LuaEntry.TeamModule:HasMemberByGuid(entity.Data.Guid) then
            -- 队友
            return "HUD_Icon_Radar10",DepthRadar.MemberPlayer
        elseif myPlayerData:IsFriend(entity.Data) then
            -- 友好
            return "HUD_Icon_Radar10",DepthRadar.OtherPlayer
        else
            -- 敌对
            return "HUD_Icon_Radar02",DepthRadar.EnemyPlayer
        end
    else
		if LuaEntry.SaiMaGameModule:IsGameStar() then
			if myPlayerData.CompelNameColorId == entity.Data.CompelNameColorId then
				return "HUD_Icon_Radar10",DepthRadar.OtherPlayer
			else
				return "HUD_Icon_Radar02",DepthRadar.EnemyPlayer
			end
		end
        if myPlayerData:IsFriend(entity.Data) then
            -- 友好 Npc
            return "HUD_Icon_Radar09",DepthRadar.FriendMonster
        else
            if entity.Data.AIType == EnumCharAIType.ScanNPC then
                -- 主动怪
                return "HUD_Icon_Radar02",DepthRadar.EnemyMonster
            else
                -- 其他怪
                return "HUD_Icon_Radar01",DepthRadar.NeutralMonster
            end
        end
    end
    return ""
end

-- 更新玩家位置
function RadarMap:UpdatePlayer(player)
	local px, py = 0, 0
	if LuaEntry.YangYingZhuJianModule:IsInEagle() then
		if LuaEntry.YangYingZhuJianModule:GetCachedEagleEntity() then
			self.playerTrans = LuaEntry.YangYingZhuJianModule:GetCachedEagleEntity().transform
			if not ObjectIsNil(self.playerTrans) then
				local ppos = self.playerTrans.position
				self:SetDotRotation(self.playerTrans.eulerAngles.y)
				--self:SetDotPosition(self.dot, ppos)
				self:SetDirection(self.cameraTrans.eulerAngles.y)

				px, py = self:ToMiniMapPositionByXZ(ppos.x,ppos.z)
			end

			self:SetMapPosition(px,py)
		end
	else

		self.playerTrans = player:GetOperatorTransform()
		if not ObjectIsNil(self.playerTrans) then
			local ppos = self.playerTrans.position
			self:SetDotRotation(self.playerTrans.eulerAngles.y)
			--self:SetDotPosition(self.dot, ppos)
			self:SetDirection(self.cameraTrans.eulerAngles.y)

			px, py = self:ToMiniMapPositionByXZ(ppos.x,ppos.z)
		end

		self:SetMapPosition(px,py)
	end
	return px,py
end

-- 设置点位置
function RadarMap:SetDotPosition( dot, position )
	--local x, y = self:ToMiniMapPositionByXZ(position.x,position.z)
	--dot:SetLocalPosition(x, y, 0)
end

-- 设置点y角度
function RadarMap:SetDotRotation( y )
	self.dot:SetEulerAngles(0,0,-y)
end

-- 设置方向
function RadarMap:SetDirection( y )
	self.direction:SetEulerAngles(0,0,-y)
end

-- 设置地图位置
function RadarMap:SetMapPosition( px,py)
	--[[
	local tempX = position.x
	local tempY = position.y
	local x, y
	if tempX + self.halfWidth > Margin.Left and tempX < self.halfWidth - Margin.Right then
		x = -tempX
	end
	if tempY + self.halfHeight > Margin.Down and tempY < self.halfHeight - Margin.Top then
		y = -tempY
	end
	if x or y then
		local mapPos = self.map.localPosition
		--self.map:SetLocalPosition(x or mapPos.x, y or mapPos.y, 0)
	end
	--]]
	local x, y = px, py
	local tSize = self.TextureMap.width
	self.TextureMap:SetUVRect(0.5 + x / 2 / self.halfWidth - tSize / 4 / self.halfWidth, 0.5 + y / 2 / self.halfHeight - tSize / 4 / self.halfHeight, tSize / 2 / self.halfWidth, tSize / 2 / self.halfHeight)

end
--[[
-- 转小地图坐标
function RadarMap:ToMiniMapPosition( position )
	return self:ToMiniMapPositionByXZ( position.x, position.z )
end
]]
function RadarMap:ToMiniMapPositionByXZ( x,z )
	return x * self.scale + self.offsetX, z * self.scale + self.offsetY
end

--优化后的位置处理
--与主角坐标差值
function RadarMap:ToMiniMapPositionVector2( position, x, y)

	local ax = position.x * self.scale + self.offsetX - x
	local az = position.z * self.scale + self.offsetY - y

    return  ax, az
end

function RadarMap:OnMapEvent(data)
	if not self.bOpen then
		return
	end
	if data.eventName == "RefreshMapModel" then
		self:OnUpdateSceneName()
	end
end

function RadarMap:OnChangeSceneTime(sender, eventArgs)
	self:SetSceneTimePeriod(eventArgs.SceneTime)
end

function RadarMap:OnChangeWeather(sender, eventArgs)
	self:SetWeather(eventArgs.WeatherId)
end

function RadarMap:SetSceneTimePeriod(time)
	local period = LuaEntry.QiYuModule:GetEnumTimePeriod(time)
	if self.curTimePeriod ~= period then
		self.curTimePeriod = period
		self.sceneTimeLbl.text = LuaEntry.QiYuModule:GetTimePreiodString(period)
		self.sceneTimeBgSprite.spriteName = LuaEntry.QiYuModule:GetTimePreiodIcon(period)
	end
end

function RadarMap:SetWeather(weatherId)
	if weatherId <= 0 then
		weatherId = 1
	end
	local spriteName = LuaEntry.QiYuModule:GetEnumWeatherIcon(weatherId)
	if spriteName then
		self.weatherSprite.spriteName = spriteName
	end
end

return RadarMap

function [Lua_Class_Name]:StartActivity()
	self.playertall = 0;
	self.playerlist = {};
	
	for player = Activity.PLAYER_1, Activity.MAXPLAYERCOUNT - 1 do
		if self:PlayerActive(player) and self:PlayerHuman(player) then
			self.playerlist[player + 1] = true;
			self.playertally = self.playertally + 1;
			
			if not self:GetPlayerBrain(player) then
				local foundBrain = MovableMan:GetUnassignedBrain(self:GetTeamOfPlayer(player));
				
				if not foundBrain then
					self.ActivityState = Activity.EDITING;
					AudioMan:ClearMusicQueue();
					AudioMan:PlayMusic("Base.rte/Music/dBSoundworks/ccambient4.ogg", -1, -1);
				else
					self:SetPlayerBrain(foundBrain, player);
					self:SwitchToActor(foundBrain, player, self:GetTeamOfPlayer(player));
					self:SetLandingZone(self:GetPlayerBrain(player).Pos, player);
					self:SetObservationTarget(self:GetPlayerBrain(player).Pos, player);
				end
			end
		end
	end
	 
	-- self.ActorList = {{Name="dafred",Type="AHuman"},{Name="dreadnaught",Type="ACrab"}};
	self.ActorList = [Activity_Actor_List];
	-- self.WeaponList = {{Name="grenade launcher",Type="HDFirearm"},{Name="blaster",Type="HDFIrearm"}};
	self.WeaponList = [Activity_Weapon_List];
	-- self.CraftList = {{Name="Drop Crate",Type="ACRocket",RTE="Dummy.rte"}, {Name="Drop Ship MK1",Type="ACDropShip",RTE="Coalition.rte"}};
	self.CraftList = [Activity_Craft_List];
	
	self.ESpawnTimer = Timer();
	self.LZ = SceneMan.Scene:GetArea("LZ Team 1");
	self.EnemyLZ = SceneMan.Scene:GetArea("LZ Team 2");
	
	--TODO make this linearly interpolate from a min and max defined in the mod manager
	if self.Difficulty <= GameActivity.CAKEDIFFICULTY then
		self.BaseSpawnTime = 6000;
		self.RandomSpawnTime = 8000;
	elseif self.Difficulty <= GameActivity.EASYDIFFICULTY then
		self.BaseSpawnTime = 5500;
		self.RandomSpawnTime = 7000;
	elseif self.Difficulty <= GameActivity.MEDIUMDIFFICULTY then
		self.BaseSpawnTime = 5000;
		self.RandomSpawnTime = 6000;
	elseif self.Difficulty <= GameActivity.HARDDIFFICULTY then
		self.BaseSpawnTime = 4500;
		self.RandomSpawnTime = 5000;
	elseif self.Difficulty <= GameActivity.NUTSDIFFICULTY then
		self.BaseSpawnTime = 4000;
		self.RandomSpawnTime = 4500;
	else
		self.BaseSpawnTime = 3500;
		self.RandomSpawnTime = 4000;
	end
	
	self.StartTimer = Timer();
	self.TimeLeft = self.BaseSpawnTime + math.random(self.RandomSpawnTime);
end

function [Lua_Class_Name]:UpdateActivity()

	if self.ActivityState == Activity.OVER or self.ActivityState == Activity.EDITING then
		self.StartTimer:Reset();
		return;
	end
	
	self:ClearObjectivePoints();
	
	for player = Activity.PLAYER_1, Activity.MAYPLAYERCOUNT - 1 do
		if self:PlayerActive(player) and self:PlayerHuman(player) then
			local team = self:GetTeamOfPlayer(player);
			
			if not MovableMan:IsActor(self:GetPlayerBrain(player)) then
				self:SetPlayerBrain(nil, player);
				self:ResetMessageTimer(player);
				FrameMan:ClearScreenText(player);
				FrameMan:SetScreenText("Your brain has been destroyed!", player, 333, -1, false);
				--What does the below do?
				if self.playerlist[player + 1] == true then
					self.playerlist[player + 1] = false;
					self.playertally = self.playertally - 1;
				end
			end
		end
	end
	
	if self.CPUTeam == nil then
		self.CPUTeam = Activity.NOTEAM;
		for player = Activity.PLAYER_1, Activity.MAXPLAYERCOUNT - 1 do
			if self:PlayerActive(player) and not(self:PlayerHuman(player)) then
				local team = self:GetTeamOfPlayer(player);
				self.CPUTeam = team;
			end
		end
	end
	
	--Spawn enemy drop ships
	if self.CPUTeam ~= Activity.NOTEAM and self.ESSpawnTimer:LeftTillSimMS(self.TimeLeft) <= 0 and
			MoveableMan:GetMOIDCount <= 210 then
		
		--Add weapons to an actor.
		local addWeaponsTo = function(actor)
			local numWeapons = math.random([Weapons_Per_Actor].Min, [Weapons_Per_Actor].Max);
			for i = 1, numWeapons do
				local weaponDes = self.WeaponsList[math.random(#self.WeaponsList)];
				
				local weapon;
				if     weaponDes.Type == "HDFirearm" then
					weapon = CreateHDFirearm(weaponDes.Name);
				elseif weaponDes.Type == "HeldDevice" then
					weapon = CreateHeldDevice(weaponDes.Name);
				elseif weaponDes.Type == "ThrownDevice" then
					weapon = CreateThrownDevice(weaponDes.Name);
				elseif weaponDes.Type == "TDExplosive" then
					weapon = CreateTDExplosive(weaponDes.Name);
				end
				
				actor:AddInventoryItem(weapon);
			end
		end
			
		--Create actors to put in craft
		local actorsForCraft = {};
		local numActors = math.random([Actors_Per_Craft].Min, [Actors_Per_Craft].Max);
		for i = 1, numActors do
			local actorDescription = self.ActorList[math.random(#self.ActorList)];
			
			local actor;
			if     actorDescription.Type == "AHuman" then
				actor = CreateAHuman(actor.Name);
				addWeaponsTo(actor);
			elseif actorDescription.Type == "ACrab" then
				actor = CreateACrab(actor.Name);
			end
				
			actor.Team = self.CPUTeam;
			actor.AIMode = Actor.AIMODE_BRAINHUNT;
		end
		
		--Create the craft.
		local shipDes = self.CraftList[math.random(#self.CraftList)];
		
		local ship;
		if     shipDes.Type == "ACRocket" then
			ship = CreateACRocket(shipDes.Name, shipDes.RTE);
		elseif shipDes.Type == "ACDropShip" then
			ship = CreateACDropShip(shipDes.Name, shipDes.RTE);
		end
		
		for i = 1, #actor do	
			ship:AddInventoryItem(actorsForCraft[i]);
		end
		
		ship.Team = self.CPUTeam;
		
		local w = math.random();
		if w > 0.5 then
			ship.Pos = Vector(self.EnemyLZ:GetRandomPoint().X,-50);
		else
			ship.Pos = Vector(self.LZ:GetRandomPoint().X,-50);
		end
		
		--Add the craft.
		
		MovableMan:AddActor(ship);
		self.ESpawnTimer:Reset();
		self.TimeLeft = self.BaseSpawnTime + math.random(self.RandomSpawnTime);
	end
	
	--Win/Lose Conditions
	if self.CPUTeam == Activity.NOTEAM then
		if self.playertally == 1 then
			for i = 1, #self.playerlist do
				if self.playerlist[i] == true then
					self.WinnerTeam = i - 1;
					ActivityMan:EndActivity();
				end
			end
		end
	else
		if self.playertally == 0 then
			self.WinnerTeam = self.CPUTeam;
			ActivityMan:EndActivity();
		end
	end
end
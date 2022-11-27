// taken from my custom medical items pack and heavily edited

function Player::WeaponAmmoStart(%pl)
{
	if(!isObject(%img = %pl.tool[%pl.currTool].image))
		return;
	
	if(%img.weaponUseCount > 0 && %pl.weaponCharges[%pl.currTool] $= "")
		%pl.weaponCharges[%pl.currTool] = %img.weaponUseCount;
	
	if(isObject(%cl = %pl.client))
		%pl.WeaponAmmoPrint(%cl, %pl.currTool, %pl.tool[%pl.currTool]);
}

function Player::WeaponAmmoPrint(%pl, %cl, %idx, %sit)
{
	if(!isObject(%pl) || !isObject(%cl) || $Pref::XNades::hideAmmo)
		return;
	
	cancel(%pl.weaponAmmoPrintSched);

	if(%pl.currTool != %idx || !isObject(%pl.tool[%idx]) || %pl.tool[%idx] != %sit)
		return;

	%cl.bottomPrint("<just:right><font:arial:16><color:7F4FA8><spush>" @ %pl.ammoText @ "<spop>  <font:arial bold:16>AMMO <font:arial:16>" @ %pl.weaponCharges[%pl.currTool] @ "/" @ %sit.image.weaponReserveMax @ "   ", 1, true);

	%pl.weaponAmmoPrintSched = %pl.schedule(300, weaponAmmoPrint, %cl, %idx, %sit);
}

function Player::WeaponAmmoUse(%pl)
{
	if(!isObject(%img = %pl.tool[%pl.currTool].image))
		return;
	
	if(%img.weaponUseCount > 0)
	{
		%pl.weaponCharges[%pl.currTool]--;
		if(%pl.weaponCharges[%pl.currTool] <= 0)
		{
			if(!%img.weaponKeepOnEmpty)
			{
				%pl.weaponCharges[%pl.currTool] = "";
				%pl.removeItemSlot(%pl.currTool);
			}
		}
	}
}

function Player::WeaponAmmoGive(%pl, %amt, %slot)
{
	if(%slot $= "")
		%slot = %pl.currTool;
	
	if(!isObject(%img = %pl.tool[%slot].image))
		return;
	
	if(%img.weaponUseCount > 0)
	{
		if(%amt $= "")
			%amt = %img.weaponUseCount;
		
		%pl.weaponCharges[%slot] += %amt;
		if(%pl.weaponCharges[%slot] >= %img.weaponReserveMax)
			%pl.weaponCharges[%slot] = %img.weaponReserveMax;
	}
}

function Player::WeaponAmmoCheck(%pl)
{
	if(!isObject(%img = %pl.tool[%pl.currTool].image))
		return;
	
	if(%img.weaponUseCount > 0)
	{
		if(%pl.weaponCharges[%pl.currTool] > 0)
			return true;
		else
			return false;
	}
	else
		return;
}

function registerChargeEvents()
{
	%list = "list";

	%items = 0;

	%cts = DatablockGroup.getCount();
	for(%i = 0; %i < %cts; %i++)
	{
		%db = DatablockGroup.getObject(%i);

		if(%db.IsA("ItemData") && %db.image.weaponReserveMax > 0)
		{
			%name = getSafeVariableName(%db.uiName);
			%name = strReplace(%name, "APOS", "");
			%name = strReplace(%name, "DASH", "");
			%list = %list SPC %name SPC %items;
			$ChargeItem[%items] = %db;
			%items++;
		}
	}

	registerOutputEvent(fxDtsBrick, setGrenadeItem, %list TAB "int 1 1000 1", 1);
	registerOutputEvent(fxDtsBrick, spawnGrenadeItem, "vector" TAB %list TAB "int 1 1000 1", 1);
	registerOutputEvent(Player, addGrenade, %list TAB "int 1 1000 1", 1);
}

schedule(0, 0, registerChargeEvents);

if(!isPackage(EventDescriptionsServer) && isFile($f = "Add-Ons/Script_EventDescriptions/server.cs"))
	exec($f);

$OutputDescription_["fxDtsBrick", "setGrenadeItem"] = "[grenade] [count]" NL
																											"Spawns a static grenade item attached to this brick." NL
																											"grenade: Grenade type" NL
																											"count: Ammo to spawn the item with";

$OutputDescription_["fxDtsBrick", "spawnGrenadeItem"] = "[grenade] [count]" NL
																												"Spawns a physics grenade item on this brick." NL
																												"grenade: Grenade type" NL
																												"count: Ammo to spawn the item with";

$OutputDescription_["Player", "addGrenade"] = "[grenade] [count]" NL
																							"Gives this player a grenade." NL
																							"grenade: Grenade type" NL
																							"count: Reserve ammo to give";

function fxDtsBrick::setGrenadeItem(%obj, %item, %val)
{
	%idb = $ChargeItem[%item];

	if(isObject(%idb))
	{
		if(!$Pref::XNades::eventsBypassAmmo && %val > %idb.image.weaponReserveMax)
			%val = %idb.image.weaponReserveMax;
		
		%obj.setItem(%idb);
		%obj.item.weaponCharges = %val;
	}
}

function fxDtsBrick::spawnGrenadeItem(%obj, %vel, %item, %val, %cl)
{
	%idb = $ChargeItem[%item];

	if(%obj.getFakeDeadTime() > 120 || (!%obj.isRendering() && !%obj.isRayCasting()))
		return;

	if(isObject(%idb))
	{
		if(!$Pref::XNades::eventsBypassAmmo && %val > %idb.image.weaponReserveMax)
			%val = %idb.image.weaponReserveMax;
		
		$weaponChargeTemp = %val;
		%obj.spawnItem(%vel, %idb);
	}
}

function Player::addGrenade(%pl, %item, %val)
{
	%idb = $ChargeItem[%item];

	if(isObject(%idb))
	{
		%idx = %pl.itemLookup(0);
		if(%idx == -1)
		{
			%idx = %pl.itemLookup(-1);
			if(%idx == -1)
				return;
		}

		if(!$Pref::XNades::eventsBypassAmmo && %val > %idb.image.weaponReserveMax)
			%val = %idb.image.weaponReserveMax;
		
		%pl.setItem(%idb, %idx);
		%pl.weaponCharges[%idx] = %val;
	}
}

package WeaponDropCharge
{
	function WeaponImage::onUnMount(%this, %obj, %slot)
	{
		cancel(%obj.weaponAmmoPrintSched);
		%obj.ammoText = "";
		Parent::onUnMount(%this, %obj, %slot);
	}

	function serverCmdDropTool(%cl, %slot)
	{
		if(isObject(%pl = %cl.player))
		{
			if(%pl.weaponCharges[%slot] !$= "" && %pl.tool[%slot].image.weaponUseCount > 0)
			{
				$weaponChargeTemp = %pl.weaponCharges[%slot];
				%pl.weaponCharges[%slot] = "";
			}
		}

		Parent::serverCmdDropTool(%cl, %slot);
	}

	function ItemData::onAdd(%this, %obj)
	{
		Parent::onAdd(%this, %obj);
		
		if(%obj.weaponCharges $= "")
		{
			if($weaponChargeTemp !$= "")
			{
				%obj.weaponCharges = $weaponChargeTemp;
				$weaponChargeTemp = "";
			}
			else if(isObject(%db = %obj.getDatablock().image) && %db.weaponUseCount > 0)
				%obj.weaponCharges = %db.weaponUseCount;
		}
	}

	function Player::Pickup(%pl, %item)
	{
		%db = %pl.getDatablock();
		if(%item.weaponCharges !$= "" && %item.canPickup && %pl.getDamagePercent() < 1.0 && minigameCanUse(%pl, %item))
		{
			%ammo = %item.weaponCharges;
			%data = %item.getDatablock();
			%empties = -1;
			for(%i = 0; %i < %pl.getDatablock().maxTools; %i++)
			{
				if(!isObject(%pl.tool[%i]))
					%empties = %empties TAB %i;
			}

			%empties = removeField(%empties, 0);

			if((%idx = %pl.itemLookup(%data)) == -1 && getFieldCount(%empties) > 0 || %data.canPickupMultiple && getFieldCount(%empties) > 0)
			{
				Parent::Pickup(%pl, %item);

				for(%i = 0; %i < getFieldCount(%empties); %i++)
				{
					%id = getField(%empties, %i);
					if(isObject(%itm = %pl.tool[%id]) && %itm.image.weaponUseCount > 0 && %itm == %data)
					{
						%pl.weaponCharges[%id] = %ammo;
						break;
					}
				}
			}
			else
			{
				if(%pl.weaponCharges[%idx] < %data.image.weaponReserveMax)
				{
					if(isObject(%itm = %pl.tool[%idx]))
					{
						if(%pl.weaponCharges[%idx] $= "")
							%pl.weaponCharges[%idx] = %data.image.weaponUseCount;
						
						for(%i = %pl.weaponCharges[%idx]; %i < %data.image.weaponReserveMax; %i++)
						{
							if(%ammo <= 0)
								break;
							
							%pl.weaponCharges[%idx]++;
							%ammo--;
						}

						if(%item.static)
							%item.respawn();
						else
						{
							%item.weaponCharges = %ammo;

							if(%ammo <= 0)
								%item.schedule(10, delete);
						}
						return true;
					}
				}
			}

			return;
		}
		
		return Parent::Pickup(%pl, %item);
	}

	function Armor::onCollision(%db,%pl,%item,%a,%b)
	{
		if(%item.weaponCharges && %item.canPickup && %pl.getDamagePercent() < 1.0 && minigameCanUse(%pl, %item))
			%pl.pickup(%item);
		else
			return Parent::onCollision(%db,%pl,%item,%a,%b);
	}
};
activatePackage(WeaponDropCharge);
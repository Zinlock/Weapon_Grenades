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
	if(!isObject(%pl) || !isObject(%cl))
		return;
	
	cancel(%pl.weaponAmmoPrintSched);

	if(%pl.currTool != %idx || !isObject(%pl.tool[%idx]) || %pl.tool[%idx] != %sit)
		return;

	%cl.bottomPrint("<just:right><font:arial:19><color:7F4FA8>AMMO <font:arial:18>" @ %pl.weaponCharges[%pl.currTool] @ "/" @ %sit.image.weaponReserveMax @ "   ", 1, true);

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
				%pl.removeItem(%pl.currTool);
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

package WeaponDropCharge
{
	function WeaponImage::onUnMount(%this, %obj, %slot)
	{
		cancel(%obj.weaponAmmoPrintSched);
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
		
		if($weaponChargeTemp !$= "")
		{
			%obj.weaponCharges = $weaponChargeTemp;
			$weaponChargeTemp = "";
		}
		else if(isObject(%db = %obj.getDatablock().image) && %db.weaponUseCount > 0)
			%obj.weaponCharges = %db.weaponUseCount;
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

				// if(isObject(%item.spawnBrick))
				// 	%item.respawn();
				// else
				// 	%item.schedule(10, delete);
				
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
						for(%i = %pl.weaponCharges[%idx]; %i < %data.image.weaponReserveMax; %i++)
						{
							if(%ammo <= 0)
								break;
							
							%pl.weaponCharges[%idx]++;
							%ammo--;
						}

						%item.weaponCharges = %ammo;

						if(isObject(%item.spawnBrick))
						{
							%item.weaponCharges = %item.getDatablock().Image.weaponUseCount;
							%item.respawn();
						}
						else if(%ammo <= 0)
							%item.schedule(10, delete);
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
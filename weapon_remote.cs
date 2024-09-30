exec("./weapon_remote_datablocks.cs");

datablock ProjectileData(grenade_remoteBlastProjectile)
{
	directDamageType  = $DamageType::remoteDirect;
	radiusDamageType  = $DamageType::remoteDirect;
	explosion           = grenade_remoteChargeExplosion;

	explodeOnDeath        = false;  

	armingDelay         = 5000; 
	lifetime            = 5000;
	fadeDelay           = 5000;
	bounceElasticity    = 0.0;
	bounceFriction  	  = 1.0;

	uiName = "";
};

datablock ProjectileData(grenade_remoteProjectile)
{
	projectileShapeName = "./dts/remote_charge_projectile.dts";
	directDamage        = 0;
	directDamageType  = $DamageType::remoteDirect;
	radiusDamageType  = $DamageType::remoteDirect;
	impactImpulse	   = 0;
	verticalImpulse	   = 0;
	explosion           = "";
	
	muzzleVelocity      = 21;
	velInheritFactor    = 0;
	explodeOnPlayerImpact = false;
	explodeOnDeath        = false;  

	brickExplosionRadius = 0;
	brickExplosionImpact = false;
	brickExplosionForce  = 30;             
	brickExplosionMaxVolume = 50;
	brickExplosionMaxVolumeFloating = 60;

	armingDelay         = 11900; 
	lifetime            = 12000;
	fadeDelay           = 11999;
	bounceElasticity    = 0.2;
	bounceFriction  	  = 0.7;
	isBallistic         = true;
	gravityMod = 1.0;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";

	uiName = "";
};

datablock ItemData(grenade_remoteItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./dts/remote_item.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "[G] Remote Explosive";
	iconName = "./ico/CHARGE";
	doColorShift = false;

	image = grenade_remoteImage;
	canDrop = true;
};

datablock ShapeBaseImageData(grenade_remoteImage)
{
	shapeFile = "./dts/remote_det_image.dts";
	emap = true;

	item = grenade_remoteItem;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = eulerToMatrix( "0 0 0" );
	className = "WeaponImage";
	armReady = true;

	doColorShift = grenade_remoteItem.doColorShift;
	colorShiftColor = grenade_remoteItem.colorShiftColor;

	weaponUseCount = 2;
	weaponReserveMax = 8;
	weaponKeepOnEmpty = true;

	maxActive = 4;

	projectileType = Projectile;
	projectile = grenade_remoteProjectile;

	stateName[0]                     = "Ready";
	stateScript[0]								= "onReady";
	stateSequence[0]			 = "root";
	stateTransitionOnTriggerDown[0]  = "Charge";

	stateName[1]                     = "Charge";
	stateTransitionOnTimeout[1]      = "Cancel";
	stateScript[1]                   = "onChargeStart";
	stateSequence[1]			 = "noSpoon";
	stateTimeoutValue[1]		   = 0.1;//3.5;
	stateTransitionOnTriggerUp[1] = "Fire";
	stateWaitForTimeout[1] = false;

	stateName[4] 				= "Cancel";
	stateScript[4]                   = "onChargeStop";
	stateSequence[4]			 = "noSpoon";
	stateTransitionOnTimeout[4] = "Ready";
	stateTimeoutValue[4]				= 0.1;

	stateName[3]                     = "Next";
	stateTimeoutValue[3]		   = 0.1;
	stateTransitionOnTriggerUp[3]      = "Ready";
	stateTransitionOnNoAmmo[3] = "Fire";

	stateName[2]                     = "Fire";
	stateTransitionOnTimeout[2]      = "Ready";
	stateScript[2]                   = "onFire";
	stateEjectShell[2] 				= true;
	stateTimeoutValue[2]		   = 0.3;
};

function grenade_remoteImage::onUnMount(%this, %obj, %slot)
{
	%obj.unMountImage(1);
}

function grenade_remoteImage::onReady(%this, %obj, %slot)
{
	%obj.ammoText = "";
	%obj.weaponAmmoStart();
}

function grenade_remoteImage::onChargeStop(%this, %obj, %slot) { %this.onFire(%obj, %slot); }

function grenade_remoteImage::onChargeStart(%this, %obj, %slot) { }

function grenade_remoteImage::onFire(%this, %obj, %slot)
{
	if(isObject(%obj.chargeSet) && %obj.chargeSet.getCount() >= %this.maxActive)
	{
		%obj.client.centerprint("<font:arial:15>\c5You already have " @ %this.maxActive @ " active charges!", 2);
		return;
	}

	if(!%obj.weaponAmmoCheck())
		return;
	
	%obj.playThread(2, leftrecoil);
	%obj.weaponAmmoUse();
	serverPlay3D(grenade_remoteThrowSound, %obj.getMuzzlePoint(%slot));
	%projs = ProjectileFire(%this.Projectile, %obj.getMuzzlePoint(1), %obj.getMuzzleVector(1), 0, 1, %slot, %obj, %obj.client);
}

function grenade_remoteImage::onAltFire(%this, %obj, %slot)
{
	%obj.playThread(2, shiftLeft);
	%obj.playThread(3, shiftRight);
	serverPlay3D(grenade_remoteClickSound, %obj.getMuzzlePoint(%slot));

	if(!isObject(%obj.chargeSet) || %obj.chargeSet.getCount() <= 0)
		return;

	for(%i = 0; %i < %obj.chargeSet.getCount(); %i++)
	{
		%charge = %obj.chargeSet.getObject(%i);
		if(getSimTime() - %charge.timePlaced > 1000)
		{
			%explo = new Projectile()
			{
				dataBlock = grenade_remoteBlastProjectile;
				initialPosition = %charge.getPosition();
				initialVelocity = "0 0 1";
				sourceObject = %charge.sourceObject;
				client = %charge.client;
			};

			%explo.schedule(70 * %i, explode);
			schedule(70 * %i, %charge, serverPlay3D, grenade_distantExplosion @ getRandom(1, 3) @ Sound, %charge.getPosition());

			%xp++;

			%charge.schedule(93 * %i, delete);
		}
	}

	if(!%obj.weaponAmmoCheck() && %xp == %i)
	{
		%obj.weaponCharges[%obj.currTool] = "";
		%obj.removeItemSlot(%obj.currTool);
		%obj.unMountImage(0);
		%obj.unMountImage(1);
	}
}

function grenade_remoteProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal)
{
	if(!isObject(%obj.sourceObject))
	{
		%obj.delete();
		return;
	}

	%charge = new StaticShape()
	{
		datablock = grenade_remoteChargePlantedShape;
		position = %pos;
		rotation = Normal2Rotation(%normal);
		sourceObject = %obj.sourceObject;
		sourceInv = %obj.sourceInv;
		client = %obj.client;
		timePlaced = getSimTime();
		isRemoteCharge = true;
	};

	%pl = %obj.sourceObject;

	%obj.delete();

	if(!isObject(%pl.chargeSet))
		%pl.chargeSet = new SimSet();
	
	if(!isObject(globalChargeSet))
		new SimSet(globalChargeSet);
	
	%pl.chargeSet.add(%charge);
	globalChargeSet.add(%charge);
	MissionCleanup.add(%charge);
	
	if(%pl.chargeSet.getCount() > grenade_remoteImage.maxActive)
	{
		%pl.chargeSet.getObject(%pl.chargeSet.getCount()-1).delete();
		%pl.weaponAmmoGive(1, %obj.sourceInv);
		return;
	}

	schedule(1000, %charge, serverPlay3D, grenade_remoteBeepSound, %pos);

	Parent::onCollision(%this,%obj,%col,%fade,%pos,%normal);
}

function serverCmdClearCharges(%cl)
{
	if(!%cl.isAdmin)
		return;
	
	%charges = 0;

	for(%i = 0; %i < globalChargeSet.getCount(); %i++)
	{
		%ch = globalChargeSet.getObject(%i);
		
		if(%ch.isRemoteCharge || %ch.isLandMine)
		{
			%ch.schedule(0, delete);
			%charges++;
		}
	}

	MessageAll ('MsgClearBricks', "\c3" @ %cl.getPlayerName () @ "\c1\c0 cleared " @ %charges @ " explosive charges.");
}

package RemoteC4Charge
{
	function Armor::onRemove(%db, %pl)
	{
		if(isObject(%pl.chargeSet))
		{
			for(%i = 0; %i < %pl.chargeSet.getCount(); %i++)
			{
				%charge = %pl.chargeSet.getObject(%i);
				%charge.schedule(0, delete);
			}

			%pl.chargeSet.schedule(0, delete);
		}

		Parent::onRemove(%db, %pl);
	}

	function Player::activateStuff(%pl)
	{
		%ray = containerRayCast(%pl.getEyePoint(), vectorAdd(%pl.getEyePoint(), vectorScale(%pl.getEyeVector(), 5)), $TypeMasks::StaticObjectType | $TypeMasks::FxBrickObjectType, %pl);

		if(%ray)
		{
			if(%ray.isRemoteCharge && %ray.sourceObject == %pl)
			{
				// if(%pl.weaponCharges[%ray.sourceInv] >= grenade_remoteImage.weaponReserveMax)
				// {
				// 	%pl.client.centerprint("<font:arial:12>Ammo is full!", 3);
				// }
				// else
				// {
				// 	%pl.weaponAmmoGive(1, %ray.sourceInv);
				// 	%pl.playThread(2, activate);
				// 	%ray.delete();
				// 	return;
				// }

				%itm = new Item()
				{
					dataBlock = grenade_remoteItem;
					position = %ray.getPosition();
					canPickup = true;
					static = false;
					minigame = getMinigameFromObject(%pl);
					bl_id = (isObject(%cl = %pl.Client) ? %cl.getBLID() : -1);
				};

				%itm.setCollisionTimeout(%pl);
				%itm.schedulePop();
				%itm.weaponCharges = 1;
				%itm.setVelocity("0 0 2");

				%ray.delete();
			}
		}

	 	Parent::activateStuff(%pl);
	}
};
activatePackage(RemoteC4Charge);
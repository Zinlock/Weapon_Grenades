exec("./weapon_riot_datablocks.cs");

function Player::startRiotDebuff(%pl, %time, %slow)
{
	%time *= 1000;

	if(!%pl.riotDebuffed)
	{
		%pl.riotDebuffed = true;
		%pl.rex_stun = %slow;
		%pl.grenade_UpdateSpeed();
	}

	%pl.riotDebuffTime = getSimTime();
	%pl.riotDebuffLoop(%time, %slow);
}

function Player::riotDebuffLoop(%pl, %time, %slow)
{
	if(!isObject(%pl))
		return;
	
	cancel(%pl.riotDebuff);

	%pl.grenade_UpdateSpeed();

	if(getSimTime() - %pl.lastRiotShake > 650)
	{
		%pl.lastRiotShake = getSimTime();
		%pl.playThread(3, plant);
		%pl.setWhiteOut(0.5);
		%p = new Projectile()
		{
			datablock = grenade_riotDizzyProjectile;
			initialPosition = %pl.getEyePoint();
		};
		%p.explode();
	}

	if(getSimTime() - %pl.riotDebuffTime < %time)
		%pl.riotDebuff = %pl.schedule(150, riotDebuffLoop, %time, %slow);
	else
	{
		%pl.riotDebuffed = false;
		%pl.rex_stun = "";
		%pl.grenade_UpdateSpeed();
	}
}

datablock ExplosionData(grenade_riotExplosion)
{
	lifeTimeMS = 500;

	soundProfile = grenade_smokeExplosionSound;

	particleEmitter = grenade_riotExplosionHazeEmitter;
	particleDensity = 400;
	particleRadius = 3;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeFreq = "1.0 1.0 1.0";
	camShakeAmp = "1.8 1.8 1.8";
	camShakeDuration = 0.8;
	camShakeRadius = 8.0;

	// Dynamic light
	lightStartRadius = 0;
	lightEndRadius = 0;
	lightStartColor = "0.45 0.3 0.1";
	lightEndColor = "0 0 0";

	//impulse
	impulseRadius = 10;
	impulseForce = 200;

	damageRadius = 0;
	radiusDamage = 0;

	uiName = "";
};

datablock ProjectileData(grenade_riotProjectile)
{
	projectileShapeName = "./dts/gas_projectile.dts";
	directDamage        = 0;
	directDamageType  = $DamageType::riotDirect;
	radiusDamageType  = $DamageType::riotDirect;
	impactImpulse	   = 0;
	verticalImpulse	   = 0;
	explosion           = grenade_riotExplosion;
	particleEmitter     = "";

	muzzleVelocity      = 33;
	velInheritFactor    = 0;
	explodeOnPlayerImpact = false;
	explodeOnDeath        = true;  

	brickExplosionRadius = 0;
	brickExplosionImpact = false;
	brickExplosionForce  = 0;             
	brickExplosionMaxVolume = 0;
	brickExplosionMaxVolumeFloating = 0;

	armingDelay         = 3400; 
	lifetime            = 3500;
	fadeDelay           = 3400;
	bounceElasticity    = 0.5;
	bounceFriction      = 0.2;
	isBallistic         = true;
	gravityMod = 1.0;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";

	uiName = "G Tear";
};

datablock ItemData(grenade_riotItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./dts/gas_item.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "[G] Tear Gas Grenade";
	iconName = "./ico/RIOT";
	doColorShift = false;

	image = grenade_riotImage;
	canDrop = true;
};

datablock ShapeBaseImageData(grenade_riotImage)
{
	shapeFile = "./dts/gas_image.dts";
	emap = true;

	item = grenade_riotItem;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = eulerToMatrix( "0 0 0" );
	className = "WeaponImage";
	armReady = true;

	doColorShift = grenade_riotItem.doColorShift;
	colorShiftColor = grenade_riotItem.colorShiftColor;

	weaponUseCount = 1;
	weaponReserveMax = 3;

	damageMult = 2;
	slowTime = 5;
	slowSpeed = 0.6;

	projectileType = Projectile;
	projectile = grenade_riotProjectile;

	stateName[0]                     = "Ready";
	stateScript[0]								= "onReady";
	stateSequence[0]			 = "root";
	stateTransitionOnTriggerDown[0]  = "Charge";

	stateName[1]                     = "Charge";
	stateTransitionOnTimeout[1]      = "Cancel";
	stateScript[1]                   = "onChargeStart";
	stateSequence[1]			 = "noSpoon";
	stateTimeoutValue[1]		   = 3.5;
	stateTransitionOnTriggerUp[1] = "Fire";
	stateWaitForTimeout[1] = false;

	stateName[4] 				= "Cancel";
	stateScript[4]                   = "onChargeStop";
	stateSequence[4]			 = "noSpoon";
	stateTransitionOnTimeout[4] = "Next";
	stateTimeoutValue[4]				= 0.1;

	stateName[3]                     = "Next";
	stateTimeoutValue[3]		   = 0.4;
	stateTransitionOnTimeout[3]      = "Ready";
	stateWaitForTimeout[3] = true;

	stateName[2]                     = "Fire";
	stateTransitionOnTimeout[2]      = "Next";
	stateScript[2]                   = "onFire";
	stateEjectShell[2] 				= true;
	stateTimeoutValue[2]		   = 0.3;
};

function grenade_riotImage::onReady(%this, %obj, %slot)
{
	%obj.weaponAmmoStart();
}

function grenade_riotImage::onChargeStop(%this, %obj, %slot) // overcooked!
{
	//%obj.damage(%obj, %obj.getHackPosition(), 33, $DamageType::Suicide);
	%this.onFire(%obj, %slot);
}

function grenade_riotImage::onChargeStart(%this, %obj, %slot)
{
	%obj.chargeStartTime[%this] = getSimTime();
	serverPlay3D(grenade_pinLightSound, %obj.getMuzzlePoint(%slot));
	%obj.cookPrint(%this);
}

function grenade_riotImage::onFire(%this, %obj, %slot)
{
	%obj.playThread(2, shiftDown);
	%obj.weaponAmmoUse();
	serverPlay3D(grenade_throwSound, %obj.getMuzzlePoint(%slot));
	%projs = ProjectileFire(%this.Projectile, %obj.getMuzzlePoint(%slot), %obj.getMuzzleVector(%slot), 0, 1, %slot, %obj, %obj.client);
	for(%i = 0; %i < getFieldCount(%projs); %i++)
	{
		%proj = getField(%projs, %i);
		%proj.cookDeath = %proj.schedule((%proj.getDatablock().lifeTime * 32) - (getSimTime() - %obj.chargeStartTime[%this]), FuseExplode);
	}

	%obj.chargeStartTime[%this] = "";
	//%obj.unMountImage(%slot);
}

function grenade_riotProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal) { serverPlay3D(grenade_bounceSound,%pos); }

function grenade_riotProjectile::onExplode(%this, %obj, %pos)
{
	%trig = new Trigger()
	{ 
		datablock = grenade_riotTriggerData;
		position = %pos;
		polyhedron = "-0.5 -0.5 -0.5 1 0 0 0 1 0 0 0 1";
		creationTime = getSimTime();
		sourceObject = %obj.sourceObject;
		sourceClient = %obj.client;
	};
	missionCleanup.add(%trig);
	%trig.schedule(16000, delete);
	%trig.setScale(13 SPC 13 SPC 13);
	triggerFix(%pos, 20);

	Parent::onExplode(%this, %obj, %pos);
}

function grenade_riotTriggerData::onTickTrigger(%db, %trig)
{
	Parent::onTickTrigger(%db, %trig);
	
	for(%i = 0; %i < %trig.getNumObjects(); %i++)
	{
		%obj = %trig.getObject(%i);
		if(%obj.IsA("Player") || %obj.IsA("AIPlayer"))
		{
			if(minigameCanDamage(%trig.sourceClient, %obj) == 1)
			{
				%obj.startRiotDebuff(grenade_riotImage.slowTime, grenade_riotImage.slowSpeed);
			}
		}
	}
}

package riotDebuffPackage
{
	function Armor::Damage(%db, %pl, %src, %pos, %dmg, %type)
	{
		if(%pl.riotDebuffed)
			%dmg *= grenade_riotImage.damageMult;
		
		Parent::Damage(%db, %pl, %src, %pos, %dmg, %type);
	}
};
activatePackage(riotDebuffPackage);
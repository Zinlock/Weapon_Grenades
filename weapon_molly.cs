exec("./weapon_molly_datablocks.cs");

datablock ExplosionData(grenade_mollyExplosion)
{
	explosionShape = "Add-Ons/Weapon_Rocket_Launcher/explosionSphere1.dts";
	lifeTimeMS = 150;

	soundProfile = grenade_mollyExplosionSound;

	// debris = grenade_mollyFireDebrisData;
	 debrisNum = 0;
	// debrisNumVariance = 10;
	// debrisPhiMin = 0;
	// debrisPhiMax = 360;
	// debrisThetaMin = 0;
	// debrisThetaMax = 76;
	// debrisVelocity = 14;
	// debrisVelocityVariance = 0;

	emitter[0] = grenade_mollyExplosionCloudEmitter;
	emitter[1] = grenade_concExplosionDebris2Emitter;

	particleEmitter = grenade_mollyExplosionHazeEmitter;
	particleDensity = 100;
	particleRadius = 0.7;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeFreq = "1.0 1.0 1.0";
	camShakeAmp = "1.8 1.8 1.8";
	camShakeDuration = 1.8;
	camShakeRadius = 8.0;

	// Dynamic light
	lightStartRadius = 0;
	lightEndRadius = 0;
	lightStartColor = "0.45 0.3 0.1";
	lightEndColor = "0 0 0";

	//impulse
	impulseRadius = 20;
	impulseForce = 200;

	damageRadius = 4;
	radiusDamage = 40;

	uiName = "";
};

datablock ProjectileData(grenade_mollyProjectile)
{
	projectileShapeName = "./dts/molly_projectile.dts";
	directDamage        = 0;
	directDamageType  = $DamageType::mollyDirect;
	radiusDamageType  = $DamageType::mollyDirect;
	impactImpulse	   = 0;
	verticalImpulse	   = 0;
	explosion           = grenade_mollyExplosion;
	particleEmitter     = "";

	muzzleVelocity      = 30;
	velInheritFactor    = 0;
	explodeOnPlayerImpact = false;
	explodeOnDeath        = true;  

	brickExplosionRadius = 0;
	brickExplosionImpact = false;
	brickExplosionForce  = 0;             
	brickExplosionMaxVolume = 0;
	brickExplosionMaxVolumeFloating = 0;

	armingDelay         = 0; 
	lifetime            = 16384;
	fadeDelay           = 16384;
	bounceElasticity    = 0.5;
	bounceFriction      = 0.2;
	isBallistic         = true;
	gravityMod = 1.0;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";

	uiName = "G Molly";
};

datablock ItemData(grenade_mollyItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./dts/molly_item.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "[G] Molotov";
	iconName = "./ico/MOLLY";
	doColorShift = false;

	image = grenade_mollyImage;
	canDrop = true;
};

datablock ShapeBaseImageData(grenade_mollyImage)
{
	shapeFile = "./dts/molly_image.dts";
	emap = true;

	item = grenade_mollyItem;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = eulerToMatrix( "0 0 0" );
	className = "WeaponImage";
	armReady = true;

	doColorShift = grenade_mollyItem.doColorShift;
	colorShiftColor = grenade_mollyItem.colorShiftColor;

	weaponUseCount = 1;
	weaponReserveMax = 3;

	projectileType = Projectile;
	projectile = grenade_mollyProjectile;

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

function grenade_mollyImage::onReady(%this, %obj, %slot)
{
	%obj.weaponAmmoStart();
}

function grenade_mollyImage::onChargeStop(%this, %obj, %slot) { %this.onFire(%obj, %slot); }

function grenade_mollyImage::onChargeStart(%this, %obj, %slot) { }

function grenade_mollyImage::onFire(%this, %obj, %slot)
{
	%obj.playThread(2, shiftDown);
	%obj.weaponAmmoUse();
	serverPlay3D(grenade_throwSound, %obj.getMuzzlePoint(%slot));
	%projs = ProjectileFire(%this.Projectile, %obj.getMuzzlePoint(%slot), %obj.getMuzzleVector(%slot), 0, 1, %slot, %obj, %obj.client);
	
	//%obj.unMountImage(%slot);
}

function grenade_mollyProjectile::onExplode(%this, %obj, %pos)
{
	%normal = vectorNormalize(%obj.getVelocity());
	%ray = containerRayCast(%pos, vectorAdd(%pos, %normal), $TypeMasks::FxBrickObjectType | $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::StaticObjectType);
	if(%ray)
		%normal = normalFromRaycast(%ray);

	%projs = ProjectileFire(grenade_mollyFireProjectile, %pos, %normal, 60, 30, 0, %obj, %obj.client);
	%projs = %projs TAB ProjectileFire(grenade_mollyFireProjectile, %pos, %normal, 15, 20, 0, %obj, %obj.client);
	for(%i = 0; %i < getFieldCount(%projs); %i++)
	{
		%proj = getField(%projs, %i);
		%proj.isMolotovFire = true;
		%trig = new Trigger()
		{
			datablock = grenade_mollyTriggerData;
			position = %pos;
			polyhedron = "-0.5 -0.5 -0.5 1 0 0 0 1 0 0 0 1";
			creationTime = getSimTime();
			sourceObject = %proj.sourceObject;
			sourceClient = %proj.client;
			sourceProjectile = %proj;
		};
		missionCleanup.add(%trig);
		%trig.schedule(10000, delete);
		%trig.setScale(3.5 SPC 3.5 SPC 4.2);

		%proj.fireTrigger = %trig;
	}

	Parent::onExplode(%this, %obj, %pos);
}

function grenade_mollyFireProjectile::PrjLoop_onTick(%this, %obj)
{
	if(isObject(%obj.fireTrigger))
		%obj.fireTrigger.setTransform(%obj.getPosition());
}

function grenade_mollyTriggerData::onEnterTrigger(%db, %trig, %obj)
{
	Parent::onEnterTrigger(%db, %trig, %obj);

	%obj.mollyTarget = %trig;
}

function grenade_mollyTriggerData::onLeaveTrigger(%db, %trig, %obj)
{
	Parent::onLeaveTrigger(%db, %trig, %obj);

	%obj.mollyTarget = -1;
}

function grenade_mollyTriggerData::onTickTrigger(%db, %trig)
{
	Parent::onTickTrigger(%db, %trig);

	if(!isObject(%trig.sourceProjectile) || %trig.sourceProjectile.fireTrigger != %trig)
	{
		%trig.delete();
		return;
	}

	%trig.position = %trig.sourceProjectile.getPosition();
	%trig.setTransform(%trig.sourceProjectile.getPosition());

	for(%i = 0; %i < %trig.getNumObjects(); %i++)
	{
		%obj = %trig.getObject(%i);
		if((%obj.IsA("Player") || %obj.IsA("AIPlayer")) && getSimTime() - %obj.lastBurnTime > %db.tickPeriodMS)
		{
			if(%obj.mollyTarget != %trig && isObject(%obj.mollyTarget)) //if the player is caught in multiple triggers, don't deal extra damage
				continue;
			
			if(minigameCanDamage(%trig.sourceClient, %obj) == 1)
			{
				%obj.mollyTarget = %trig;
				%obj.lastBurnTime = getSimTime();
				
				%obj.damage(%obj, %obj.getHackPosition(), 1, $DamageType::mollyDirect);
				%obj.molotovAfterBurn(2.5, 250, 16);
			}
		}
	}
}

function Player::molotovAfterBurn(%pl, %dmg, %spd, %tick)
{
	cancel(%pl.afterBurn);

	%pl.burn(%spd + 1000);

	if($BBB::Enable && %pl.isBody)
		%pl.flameClear();

	%pl.damage(%pl, %pl.getHackPosition(), %dmg, $DamageType::mollyDirect);
	%pl.playThread(2, plant);
	serverPlay3D(grenade_mollyBurnSound, %pl.getHackPosition());

	if(%tick >= 0)
		%pl.afterBurn = %pl.schedule(%spd, molotovAfterBurn, %dmg, %spd, %tick--);
}
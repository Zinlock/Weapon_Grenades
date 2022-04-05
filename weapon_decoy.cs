exec("./weapon_decoy_datablocks.cs");

datablock ExplosionData(grenade_decoyExplosion)
{
	lifeTimeMS = 150;

	soundProfile = "";

	emitter[0] = grenade_concExplosionConeEmitter;
	emitter[1] = grenade_flashExplosionHazeEmitter;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeFreq = "1.0 1.0 1.0";
	camShakeAmp = "5.8 6.8 5.8";
	camShakeDuration = 1.2;
	camShakeRadius = 16.0;

	// Dynamic light
	lightStartRadius = 0;
	lightEndRadius = 0;
	lightStartColor = "0.45 0.3 0.1";
	lightEndColor = "0 0 0";

	//impulse
	impulseRadius = 0;
	impulseForce = 0;

	damageRadius = 0;
	radiusDamage = 0;

	uiName = "";
};

datablock ProjectileData(grenade_decoyPopProjectile)
{
	projectileShapeName = "./dts/shrapnel.dts";
	directDamage        = 0;
	directDamageType  = $DamageType::decoyDirect;
	radiusDamageType  = $DamageType::decoyDirect;
	impactImpulse	   = 30;
	verticalImpulse	   = 0;
	explosion           = grenade_decoyExplosion;

	muzzleVelocity      = 9;
	velInheritFactor    = 0;
	explodeOnPlayerImpact = true;
	explodeOnDeath        = true;  

	brickExplosionRadius = 0;
	brickExplosionImpact = false;
	brickExplosionForce  = 30;             
	brickExplosionMaxVolume = 50;
	brickExplosionMaxVolumeFloating = 60;

	armingDelay         = 0; 
	lifetime            = 32;
	fadeDelay           = 0;
	bounceElasticity    = 0.9;
	bounceFriction      = 0.08;
	isBallistic         = false;
	gravityMod = 0.3;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";

	uiName = "";
};

datablock ProjectileData(grenade_decoyProjectile)
{
	projectileShapeName = "./dts/decoy_projectile.dts";
	directDamage        = 0;
	directDamageType  = $DamageType::decoyDirect;
	radiusDamageType  = $DamageType::decoyDirect;
	impactImpulse	   = 0;
	verticalImpulse	   = 0;
	explosion           = "";

	muzzleVelocity      = 33;
	velInheritFactor    = 0;
	explodeOnPlayerImpact = false;
	explodeOnDeath        = true;  

	brickExplosionRadius = 0;
	brickExplosionImpact = false;
	brickExplosionForce  = 0;             
	brickExplosionMaxVolume = 0;
	brickExplosionMaxVolumeFloating = 0;

	armingDelay         = 13900;
	lifetime            = 14000;
	fadeDelay           = 13900;
	bounceElasticity    = 0.5;
	bounceFriction      = 0.2;
	isBallistic         = true;
	gravityMod = 1.0;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";

	PrjLoop_enabled = true;
	PrjLoop_maxTicks = -1;
	PrjLoop_tickTime = 500;

	uiName = "G decoy";
};

datablock ItemData(grenade_decoyItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./dts/decoy_item.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "[G] Decoy Grenade";
	iconName = "./ico/DECOY";
	doColorShift = false;

	image = grenade_decoyImage;
	canDrop = true;
};

datablock ShapeBaseImageData(grenade_decoyImage)
{
	shapeFile = "./dts/decoy_image.dts";
	emap = true;

	item = grenade_decoyItem;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = eulerToMatrix( "0 0 0" );
	className = "WeaponImage";
	armReady = true;

	doColorShift = grenade_decoyItem.doColorShift;
	colorShiftColor = grenade_decoyItem.colorShiftColor;

	lifeTimeOffset = 11000;

	weaponUseCount = 1;
	weaponReserveMax = 2;

	projectileType = Projectile;
	projectile = grenade_decoyProjectile;

	stateName[0]                     = "Ready";
	stateScript[0]								= "onReady";
	stateSequence[0]			 = "root";
	stateTransitionOnTriggerDown[0]  = "Charge";

	stateName[1]                     = "Charge";
	stateTransitionOnTimeout[1]      = "Cancel";
	stateScript[1]                   = "onChargeStart";
	stateSequence[1]			 = "noSpoon";
	stateTimeoutValue[1]		   = 3.0;
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

function grenade_decoyImage::onReady(%this, %obj, %slot)
{
	%obj.weaponAmmoStart();
}

function grenade_decoyImage::onChargeStop(%this, %obj, %slot) // overcooked!
{
	//%obj.damage(%obj, %obj.getHackPosition(), 33, $DamageType::Suicide);
	%this.onFire(%obj, %slot);
}

function grenade_decoyImage::onChargeStart(%this, %obj, %slot)
{
	%obj.chargeStartTime[%this] = getSimTime();
	serverPlay3D(grenade_pinLightSound, %obj.getMuzzlePoint(%slot));
	%obj.cookPrint(%this);
}

function grenade_decoyImage::onFire(%this, %obj, %slot)
{
	%obj.stopAudio(2);
	%obj.playThread(2, shiftDown);
	%obj.weaponAmmoUse();
	serverPlay3D(grenade_throwSound, %obj.getMuzzlePoint(%slot));
	%projs = ProjectileFire(%this.Projectile, %obj.getMuzzlePoint(%slot), %obj.getMuzzleVector(%slot), 0, 1, %slot, %obj, %obj.client);
	for(%i = 0; %i < getFieldCount(%projs); %i++)
	{
		%proj = getField(%projs, %i);
		%proj.cookDeath = %proj.schedule((%proj.getDatablock().lifeTime * 32) - (getSimTime() - %obj.chargeStartTime[%this]), FuseExplode);
		%proj.spawnTime = getSimTime() - (getSimTime() - %obj.chargeStartTime[%this]);
		%x = getRandom(0, 2);
		switch(%x)
		{
			case 0: %proj.decoyType = "PKM";
			case 1: %proj.decoyType = "AK";
			case 2: %proj.decoyType = "FALS";
		}
	}

	%obj.chargeStartTime[%this] = "";
}

function grenade_decoyProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal) { serverPlay3D(grenade_bounceSound,%pos); }

function grenade_decoyProjectile::PrjLoop_onTick(%this, %obj)
{
	if(getSimTime() - %obj.lastDecoyFX > 2000 && getSimTime() - %obj.spawnTime > 3000)
	{
		%pos = %obj.getPosition();
		%obj.lastDecoyFX = getSimTime();
		serverPlay3D("grenade_decoy" @ %obj.decoyType @ getRandom(1,3) @ "Sound", %pos);
		%p = new Projectile()
		{
			dataBlock = grenade_decoyPopProjectile;
			initialVelocity = "0 0 0";
			initialPosition = %pos;
		};
		MissionCleanup.add(%p);
	}
}
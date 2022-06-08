exec("./weapon_smoke_datablocks.cs");

datablock ExplosionData(grenade_smokeExplosion)
{
	lifeTimeMS = 500;

	soundProfile = grenade_smokeExplosionSound;

	emitter[0] = grenade_smokeExplosionCloudEmitter;

	particleEmitter = grenade_smokeExplosionHazeEmitter;
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
	impulseRadius = 4;
	impulseForce = 500;

	damageRadius = 0;
	radiusDamage = 0;

	uiName = "";
};

datablock ProjectileData(grenade_smokeProjectile)
{
	projectileShapeName = "./dts/smoke_projectile.dts";
	directDamage        = 0;
	directDamageType  = $DamageType::smokeDirect;
	radiusDamageType  = $DamageType::smokeDirect;
	impactImpulse	   = 0;
	verticalImpulse	   = 0;
	explosion           = grenade_smokeExplosion;
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

	uiName = "G Smoke";
};

datablock ItemData(grenade_smokeItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./dts/smoke_item.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "[G] Smoke Grenade";
	iconName = "./ico/SMOKE";
	doColorShift = false;

	image = grenade_smokeImage;
	canDrop = true;
};

datablock ShapeBaseImageData(grenade_smokeImage)
{
	shapeFile = "./dts/smoke_image.dts";
	emap = true;

	item = grenade_smokeItem;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = eulerToMatrix( "0 0 0" );
	className = "WeaponImage";
	armReady = true;

	doColorShift = grenade_smokeItem.doColorShift;
	colorShiftColor = grenade_smokeItem.colorShiftColor;

	weaponUseCount = 1;
	weaponReserveMax = 3;

	projectileType = Projectile;
	projectile = grenade_smokeProjectile;

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

function grenade_smokeImage::onReady(%this, %obj, %slot)
{
	%obj.weaponAmmoStart();
}

function grenade_smokeImage::onChargeStop(%this, %obj, %slot) // overcooked!
{
	//%obj.damage(%obj, %obj.getHackPosition(), 33, $DamageType::Suicide);
	%this.onFire(%obj, %slot);
}

function grenade_smokeImage::onChargeStart(%this, %obj, %slot)
{
	%obj.chargeStartTime[%this] = getSimTime();
	serverPlay3D(grenade_pinLightSound, %obj.getMuzzlePoint(%slot));
	%obj.cookPrint(%this);
}

function grenade_smokeImage::onFire(%this, %obj, %slot)
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

function grenade_smokeProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal) { serverPlay3D(grenade_bounceSound,%pos); }

function smokeClearFire(%pos)
{
	initContainerRadiusSearch(%pos, 7.5, $TypeMasks::ProjectileObjectType);
	while(isObject(%col = ContainerSearchNext()))
	{
		if(%col.isMolotovFire)
		{
			%col.fireTrigger.delete();
			%col.schedule(0, delete);
		}
	}
}

function grenade_smokeProjectile::onExplode(%this, %obj, %pos)
{
	for(%i = 0; %i < 52 * 4; %i++)
		schedule(%i * 250, 0, smokeClearFire, %pos); // lol
}
exec("./weapon_cluster_datablocks.cs");

datablock ExplosionData(grenade_clusterExplosion)
{
	explosionShape = "Add-Ons/Weapon_Rocket_Launcher/explosionSphere1.dts";
	lifeTimeMS = 150;

	soundProfile = grenade_clusterExplosionSound;

	particleEmitter = grenade_concExplosionHazeEmitter;
	particleDensity = 100;
	particleRadius = 0.7;

	emitter[0] = grenade_concExplosionCloudEmitter;
	emitter[1] = grenade_concExplosionDebrisEmitter;
	emitter[2] = grenade_concExplosionDebris2Emitter;
	emitter[3] = grenade_concExplosionConeEmitter;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeFreq = "1.0 1.0 1.0";
	camShakeAmp = "19.8 19.8 19.8";
	camShakeDuration = 1.8;
	camShakeRadius = 20.0;

	// Dynamic light
	lightStartRadius = 0;
	lightEndRadius = 0;
	lightStartColor = "0.45 0.3 0.1";
	lightEndColor = "0 0 0";

	//impulse
	impulseRadius = 32;
	impulseForce = 1500;

	damageRadius = 20;
	radiusDamage = 50;

	uiName = "";
};

datablock ProjectileData(grenade_clusterProjectile)
{
	projectileShapeName = "./dts/cluster_projectile.dts";
	directDamage        = 0;
	directDamageType  = $DamageType::clusterDirect;
	radiusDamageType  = $DamageType::clusterDirect;
	impactImpulse	   = 0;
	verticalImpulse	   = 0;
	explosion           = grenade_clusterExplosion;
	particleEmitter     = "";

	muzzleVelocity      = 33;
	velInheritFactor    = 0;
	explodeOnPlayerImpact = false;
	explodeOnDeath        = true;  

	brickExplosionRadius = 6;
	brickExplosionImpact = false;
	brickExplosionForce  = 30;             
	brickExplosionMaxVolume = 50;
	brickExplosionMaxVolumeFloating = 60;

	armingDelay         = 2900; 
	lifetime            = 3000;
	fadeDelay           = 2900;
	bounceElasticity    = 0.5;
	bounceFriction      = 0.2;
	isBallistic         = true;
	gravityMod = 1.0;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";

	uiName = "G Cluster";
};

datablock ItemData(grenade_clusterItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./dts/cluster_item.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "[G] Cluster Grenade";
	iconName = "./ico/CLUSTER";
	doColorShift = false;

	image = grenade_clusterImage;
	canDrop = true;
};

datablock ShapeBaseImageData(grenade_clusterImage)
{
	shapeFile = "./dts/cluster_image.dts";
	emap = true;

	item = grenade_clusterItem;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = eulerToMatrix( "0 0 0" );
	className = "WeaponImage";
	armReady = true;

	doColorShift = grenade_clusterItem.doColorShift;
	colorShiftColor = grenade_clusterItem.colorShiftColor;

	weaponUseCount = 1;
	weaponReserveMax = 3;
	
	clusterlets = 30;

	projectileType = Projectile;
	projectile = grenade_clusterProjectile;

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

function grenade_clusterImage::onReady(%this, %obj, %slot)
{
	%obj.weaponAmmoStart();
}

function grenade_clusterImage::onChargeStop(%this, %obj, %slot) // overcooked!
{
	%obj.damage(%obj, %obj.getHackPosition(), 33, $DamageType::Suicide);
	%this.onFire(%obj, %slot);
}

function grenade_clusterImage::onChargeStart(%this, %obj, %slot)
{
	%obj.chargeStartTime[%this] = getSimTime();
	serverPlay3D(grenade_pinLightSound, %obj.getMuzzlePoint(%slot));
	%obj.cookPrint(%this);
}

function grenade_clusterImage::onFire(%this, %obj, %slot)
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

function grenade_clusterProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal) { serverPlay3D(grenade_bounceSound,%pos); }

function grenade_clusterProjectile::onExplode(%this, %obj, %pos)
{
	%projs = ProjectileFire(grenade_clusterletProjectile, %pos, "0 1 0", 1000, grenade_clusterImage.clusterlets, 0, %obj, %obj.client);
	for(%i = 0; %i < getFieldCount(%projs); %i++)
	{
		%proj = getField(%projs, %i);
		%proj.schedule(250 + ((%i / grenade_clusterImage.clusterlets) * 1000), FuseExplode);
	}

	Parent::onExplode(%this, %obj, %pos);
}

function grenade_clusterletProjectile::onExplode(%this, %obj, %pos)
{
	serverPlay3D(grenade_clusterletExplosion @ getRandom(1,2) @ Sound,%pos);
	
	Parent::onExplode(%this, %obj, %pos);
}
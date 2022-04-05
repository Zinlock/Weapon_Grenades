exec("./weapon_electro_datablocks.cs");

datablock ExplosionData(grenade_electroExplosion)
{
   explosionShape = "Add-Ons/Weapon_Rocket_Launcher/explosionSphere1.dts";
   lifeTimeMS = 150;

   soundProfile = grenade_electroExplosionSound;
   
   emitter[0] = grenade_electroExplosionCloudEmitter;
	 emitter[1] = grenade_concExplosionDebris2Emitter;

   particleEmitter = grenade_electroExplosionHazeEmitter;
   particleDensity = 100;
   particleRadius = 1.0;

   faceViewer     = true;
   explosionScale = "1 1 1";

   shakeCamera = true;
   camShakeFreq = "7.0 8.0 7.0";
   camShakeAmp = "4.0 4.0 4.0";
   camShakeDuration = 2.3;
   camShakeRadius = 18.0;

   // Dynamic light
   lightStartRadius = 0;
   lightEndRadius = 0;
   lightStartColor = "0.45 0.3 0.1";
   lightEndColor = "0 0 0";

   //impulse
   impulseRadius = 12;
   impulseForce = 1500;

   damageRadius = 0;
   radiusDamage = 0;

   uiName = "";
};

datablock ProjectileData(grenade_electroProjectile)
{
	projectileShapeName = "./dts/electro_projectile.dts";
	directDamage        = 0;
	directDamageType  = $DamageType::electroDirect;
	radiusDamageType  = $DamageType::electroDirect;
	impactImpulse	   = 0;
	verticalImpulse	   = 0;
	explosion           = grenade_electroExplosion;
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

	armingDelay         = 2380; 
	lifetime            = 2500;
	fadeDelay           = 2400;
	bounceElasticity    = 0.3;
	bounceFriction      = 0.2;
	isBallistic         = true;
	gravityMod = 1.0;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";

	uiName = "G Electro";
};

datablock ItemData(grenade_electroItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./dts/electro_item.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "[G] Electric Grenade";
	iconName = "./ico/ELECTRO";
	doColorShift = false;

	image = grenade_electroImage;
	canDrop = true;
};

datablock ShapeBaseImageData(grenade_electroImage)
{
	shapeFile = "./dts/electro_image.dts";
	emap = true;

	item = grenade_electroItem;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = eulerToMatrix( "0 0 0" );
	className = "WeaponImage";
	armReady = true;

	doColorShift = grenade_electroItem.doColorShift;
	colorShiftColor = grenade_electroItem.colorShiftColor;

	weaponUseCount = 1;
	weaponReserveMax = 3;

	projectileType = Projectile;
	projectile = grenade_electroProjectile;

	stateName[0]                     = "Ready";
	stateScript[0]								= "onReady";
	stateSequence[0]			 = "root";
	stateTransitionOnTriggerDown[0]  = "Charge";

	stateName[1]                     = "Charge";
	stateTransitionOnTimeout[1]      = "Cancel";
	stateScript[1]                   = "onChargeStart";
	stateSequence[1]			 = "noSpoon";
	stateTimeoutValue[1]		   = 2.5;
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

function grenade_electroImage::onReady(%this, %obj, %slot)
{
	%obj.weaponAmmoStart();
}

function grenade_electroImage::onChargeStop(%this, %obj, %slot) // overcooked!
{
	%obj.damage(%obj, %obj.getHackPosition(), 33, $DamageType::Suicide);
	%this.onFire(%obj, %slot);
}

function grenade_electroImage::onChargeStart(%this, %obj, %slot)
{
	%obj.chargeStartTime[%this] = getSimTime();
	serverPlay3D(grenade_pinLightSound, %obj.getMuzzlePoint(%slot));
	%obj.cookPrint(%this);
}

function grenade_electroImage::onFire(%this, %obj, %slot)
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

function grenade_electroProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal) { serverPlay3D(grenade_bounceSound,%pos); }

function grenade_electroProjectile::onExplode(%this, %obj, %pos)
{
	initContainerRadiusSearch(%pos, 20, $TypeMasks::PlayerObjectType);
	while(isObject(%col = ContainerSearchNext()))
	{
		if(minigameCanDamage(%obj, %col) == 1)
		{
			%dist = vectorDist(%pos, %col.getHackPosition());

			if(!isObject(firstWord(containerRayCast(%pos,%col.getHackPosition(),$TypeMasks::FxBrickObjectType | $TypeMasks::VehicleObjectType, %col))))
			{
				%col.zapTicks = 20;
				%col.zapDamage = 0.9;
				%col.zap();
			}
		}
	}
	Parent::onExplode(%this, %obj, %pos);
}
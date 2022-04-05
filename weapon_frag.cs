exec("./weapon_frag_datablocks.cs");

datablock ExplosionData(grenade_fragmentExplosion)
{
	explosionShape = "Add-Ons/Weapon_Rocket_Launcher/explosionSphere1.dts";
	lifeTimeMS = 150;

	soundProfile = grenade_fragExplosionSound;

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
	camShakeDuration = 3.8;
	camShakeRadius = 20.0;

	// Dynamic light
	lightStartRadius = 0;
	lightEndRadius = 0;
	lightStartColor = "0.45 0.3 0.1";
	lightEndColor = "0 0 0";

	//impulse
	impulseRadius = 15;
	impulseForce = 600;

	damageRadius = 18;
	radiusDamage = 105;

	uiName = "";
};

datablock ProjectileData(grenade_fragmentProjectile)
{
	projectileShapeName = "./dts/frag_projectile.dts";
	directDamage        = 0;
	directDamageType  = $DamageType::fragmentDirect;
	radiusDamageType  = $DamageType::fragmentDirect;
	impactImpulse	   = 0;
	verticalImpulse	   = 0;
	explosion           = grenade_fragmentExplosion;
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

	uiName = "G Frag";
};

datablock ItemData(grenade_fragmentItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./dts/frag_item.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "[G] Frag Grenade";
	iconName = "./ico/FRAG";
	doColorShift = false;

	image = grenade_fragmentImage;
	canDrop = true;
};

datablock ShapeBaseImageData(grenade_fragmentImage)
{
	shapeFile = "./dts/frag_image.dts";
	emap = true;

	item = grenade_fragmentItem;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = eulerToMatrix( "0 0 0" );
	className = "WeaponImage";
	armReady = true;

	doColorShift = grenade_fragmentItem.doColorShift;
	colorShiftColor = grenade_fragmentItem.colorShiftColor;

	weaponUseCount = 1;
	weaponReserveMax = 2;

	projectileType = Projectile;
	projectile = grenade_fragmentProjectile;

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

function grenade_fragmentImage::onReady(%this, %obj, %slot)
{
	%obj.weaponAmmoStart();
}

function grenade_fragmentImage::onChargeStop(%this, %obj, %slot) // overcooked!
{
	%obj.damage(%obj, %obj.getHackPosition(), 33, $DamageType::Suicide);
	%this.onFire(%obj, %slot);
}

function grenade_fragmentImage::onChargeStart(%this, %obj, %slot)
{
	%obj.chargeStartTime[%this] = getSimTime();
	serverPlay3D(grenade_pinLightSound, %obj.getMuzzlePoint(%slot));
	%obj.cookPrint(%this);
}

function grenade_fragmentImage::onFire(%this, %obj, %slot)
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

function grenade_fragmentProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal) { serverPlay3D(grenade_bounceSound,%pos); }

function grenade_fragmentProjectile::onExplode(%this, %obj, %pos)
{
	ProjectileFire(grenade_shrapnelProjectile, %pos, "0 1 0", 1000, 75, 0, %obj, %obj.client);

	initContainerRadiusSearch(%pos, 32, $TypeMasks::PlayerObjectType);
	while(isObject(%col = ContainerSearchNext()))
	{
		if(minigameCanDamage(%obj, %col) == 1)
		{
			%fromVector = vectorNormalize(vectorSub(%pos, %col.getEyePoint()));
			%lookDot = vectorDot(%fromVector, %col.getEyeVector());
			%dist = vectorDist(%pos, %col.getEyePoint());

			if(!isObject(firstWord(containerRayCast(%pos,%col.getEyePoint(),$TypeMasks::FxBrickObjectType | $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType, %col))))
			{
				%time = 1;
				%mult = 0.3;
				%proxy = 6;
				%dist = vectorDist(%pos, %col.getEyePoint());
				%stunLen = ((%mult * (%lookDot + 1)) * %time) / mClampF(%dist - %proxy, 1, 64);
				if(%lookDot > 0)
					%col.setWhiteOut(%stunLen);
				else
					%col.setWhiteOut(2 / %dist);
			}
		}
	}
	
	Parent::onExplode(%this, %obj, %pos);
}
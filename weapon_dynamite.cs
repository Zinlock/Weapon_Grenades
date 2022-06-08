exec("./weapon_dynamite_datablocks.cs");

datablock ExplosionData(grenade_dynamiteExplosion)
{
	explosionShape = "Add-Ons/Weapon_Rocket_Launcher/explosionSphere1.dts";
	lifeTimeMS = 150;

	soundProfile = grenade_dynamiteExplosionSound;

	emitter[0] = grenade_concExplosionCloudEmitter;
	emitter[1] = grenade_concExplosionDebrisEmitter;
	emitter[2] = grenade_concExplosionDebris2Emitter;

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
	impulseForce = 1500;

	damageRadius = 12;
	radiusDamage = 42;

	uiName = "";
};

datablock ProjectileData(grenade_dynamiteProjectile)
{
	projectileShapeName = "./dts/dynamite_projectile.dts";
	directDamage        = 0;
	directDamageType  = $DamageType::dynamiteDirect;
	radiusDamageType  = $DamageType::dynamiteDirect;
	impactImpulse	   = 0;
	verticalImpulse	   = 0;
	explosion           = grenade_dynamiteExplosion;
	sound = grenade_dynamiteFuseSound;

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

	uiName = "G dynamite";
};

datablock ItemData(grenade_dynamiteItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./dts/dynamite_item.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "[G] Thermite Stick";
	iconName = "./ico/DYNAMITE";
	doColorShift = false;

	image = grenade_dynamiteImage;
	canDrop = true;
};

datablock ShapeBaseImageData(grenade_dynamiteImage)
{
	shapeFile = "./dts/dynamite_image.dts";
	emap = true;

	item = grenade_dynamiteItem;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = eulerToMatrix( "0 0 0" );
	className = "WeaponImage";
	armReady = true;

	doColorShift = grenade_dynamiteItem.doColorShift;
	colorShiftColor = grenade_dynamiteItem.colorShiftColor;

	weaponUseCount = 1;
	weaponReserveMax = 2;

	burnDamage = 4.2;
	afterBurnDamage = 3.2;
	afterBurnTime = 4;

	projectileType = Projectile;
	projectile = grenade_dynamiteProjectile;

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
	stateEmitter[1]                  = grenade_dynamiteFuseEmitter;
	stateEmitterTime[1]              = 3.0;
	stateEmitterNode[1]              = tailNode;
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

function grenade_dynamiteImage::onUnMount(%this, %obj, %slot)
{
	%obj.stopAudio(2);
}

function grenade_dynamiteImage::onReady(%this, %obj, %slot)
{
	%obj.weaponAmmoStart();
	%obj.stopAudio(2);
}

function grenade_dynamiteImage::onChargeStop(%this, %obj, %slot) // overcooked!
{
	%obj.damage(%obj, %obj.getHackPosition(), 33, $DamageType::Suicide);
	%obj.stopAudio(2);
	%this.onFire(%obj, %slot);
}

function grenade_dynamiteImage::onChargeStart(%this, %obj, %slot)
{
	%obj.chargeStartTime[%this] = getSimTime();
	%obj.playAudio(2, grenade_dynamiteFuseSound);
	//serverPlay3D(grenade_pinLightSound, %obj.getMuzzlePoint(%slot));
	%obj.cookPrint(%this);
}

function grenade_dynamiteImage::onFire(%this, %obj, %slot)
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
	}

	%obj.chargeStartTime[%this] = "";

	if(%obj.weaponAmmoCheck())
	{
		%obj.unMountImage(%slot);
		%obj.schedule(33, mountImage, %this, %slot); // the fuse emitter doesn't go away instantly without this. it breaks the 300ms cooldown between throws but idc
	}
}

function grenade_dynamiteProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal) { serverPlay3D(grenade_stickBounceSound,%pos); }

function grenade_dynamiteProjectile::onExplode(%this, %obj, %pos)
{
	ProjectileFire(grenade_dynamiteDebrisProjectile, %pos, "0 1 0", 1000, 10, 0, %obj, %obj.client);
	%projs = ProjectileFire(grenade_dynamiteFireProjectile, %pos, vectorNormalize(%obj.getVelocity()), 0, 10, 0, %obj, %obj.client);
	%projs2 = ProjectileFire(grenade_dynamiteFireAltProjectile, %pos, vectorNormalize(%obj.getVelocity()), 0, 10, 0, %obj, %obj.client);
	%projs3 = ProjectileFire(grenade_dynamiteFireAlt2Projectile, %pos, vectorNormalize(%obj.getVelocity()), 0, 10, 0, %obj, %obj.client);

	for(%i = 0; %i < getFieldCount(%projs); %i++)
	{
		%proj = getField(%projs, %i);
		%trig = new Trigger()
		{
			datablock = grenade_dynamiteTriggerData;
			position = %pos;
			polyhedron = "-0.5 -0.5 -0.5 1 0 0 0 1 0 0 0 1";
			creationTime = getSimTime();
			sourceObject = %proj.sourceObject;
			sourceClient = %proj.client;
			sourceProjectile = %proj;
		};
		missionCleanup.add(%trig);
		%trig.schedule(7800, delete);
		%trig.setScale(10.5 SPC 10.5 SPC 10.5);

		%proj.fireTrigger = %trig;
		%proj.altFollower0 = getField(%projs2, %i);
		%proj.altFollower1 = getField(%projs3, %i);
	}

	Parent::onExplode(%this, %obj, %pos);
}

function grenade_dynamiteFireProjectile::PrjLoop_onTick(%this, %obj)
{
	triggerFix(%obj.getPosition(), 15);

	if(isObject(%obj.fireTrigger))
		%obj.fireTrigger.setTransform(%obj.getPosition());
	
	if(isObject(%obj.altFollower0))
		%obj.altFollower0.setTransform(%obj.getTransform());
	
	if(isObject(%obj.altFollower1))
		%obj.altFollower1.setTransform(%obj.getTransform());
}

function grenade_dynamiteTriggerData::onEnterTrigger(%db, %trig, %obj)
{
	Parent::onEnterTrigger(%db, %trig, %obj);

	%obj.dynamiteTarget = %trig;
}

function grenade_dynamiteTriggerData::onLeaveTrigger(%db, %trig, %obj)
{
	Parent::onLeaveTrigger(%db, %trig, %obj);

	%obj.dynamiteTarget = -1;
}

function grenade_dynamiteTriggerData::onTickTrigger(%db, %trig)
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
			if(%obj.dynamiteTarget != %trig && isObject(%obj.dynamiteTarget)) //if the player is caught in multiple triggers, don't deal extra damage
				continue;
			
			%obj.dynamiteTarget = %trig;
			%obj.lastBurnTime = getSimTime();

			if(minigameCanDamage(%trig.sourceClient, %obj) == 1)
			{
				%tick = 250;
				%obj.firstBurnTick = 1;
				%obj.molotovAfterBurn(grenade_dynamiteImage.afterBurnDamage, %tick, mFloor(grenade_dynamiteImage.afterBurnTime * 1000 / %tick), grenade_dynamiteImage.burnDamage);
			}
		}
	}
}
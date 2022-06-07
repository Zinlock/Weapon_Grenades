exec("./weapon_flash_datablocks.cs");

function Player::makeDeaf(%pl, %time)
{
	if(!isObject(%cl = %pl.client))
		return;
	
	if(%col.isDeaf)
	{
		cancel(%col.deafen);
		%col.isDeaf = false;
	}

	%time *= 1000;
	%pl.deafenTime = getSimTime();
	%pl.isDeaf = true;
	%pl.deafLoop(%time);
}

function Player::deafLoop(%pl, %time)
{
	if(!isObject(%pl) || !isObject(%cl = %pl.client))
		return;
	
	cancel(%pl.deafen);

	for(%i = 0; %i < 4; %i++)
		%cl.play2D(grenade_flashEmptySound);
	
	for(%i = 0; %i < 6; %i++)
		%cl.schedule(33 + %i, play2D,grenade_flashEmptySound); // dumb shit

	if(getSimTime() - %pl.deafenTime < %time)
		%pl.deafen = %pl.schedule(40, deafLoop, %time);
	else
		%pl.isDeaf = false;
}

function Player::makeBlind(%pl, %time)
{
	if(%col.isBlind)
	{
		cancel(%col.blind);
		%col.isBlind = false;
	}

	%time *= 1000;
	%pl.blindTime = getSimTime();
	%pl.isBlind = true;
	%pl.blindLoop(%time);
}

function Player::blindLoop(%pl, %time)
{
	if(!isObject(%pl) || !isObject(%cl = %pl.client))
		return;
	
	cancel(%pl.blind);
	
	%pl.setWhiteOut(1);

	if(getSimTime() - %pl.blindTime < %time)
		%pl.blind = %pl.schedule(40, blindLoop, %time);
	else
	{
		%pl.setWhiteOut(0.9);
		%pl.isBlind = false;
	}
}

package PlayerDeafen
{
	function GameConnection::Play3D(%cl, %pf, %pos)
	{
		if(isObject(%pl = %cl.player) && %pl.isDeaf)
			%pf = grenade_flashEmptySound;
		
		Parent::Play3D(%cl, %pf, %pos);
	}

	function GameConnection::Play2D(%cl, %pf)
	{
		if(isObject(%pl = %cl.player) && %pl.isDeaf)
			%pf = grenade_flashEmptySound;
		
		Parent::Play2D(%cl, %pf);
	}
};
activatePackage(PlayerDeafen);

datablock ExplosionData(grenade_flashbangExplosion)
{
   explosionShape = "Add-Ons/Weapon_Rocket_Launcher/explosionSphere1.dts";
   lifeTimeMS = 150;

   soundProfile = grenade_flashExplosionSound;
   
   emitter[0] = grenade_flashExplosionCloudEmitter;

   particleEmitter = grenade_flashExplosionHazeEmitter;
   particleDensity = 90;
   particleRadius = 1.0;

   faceViewer     = true;
   explosionScale = "1 1 1";

   shakeCamera = true;
   camShakeFreq = "7.0 8.0 7.0";
   camShakeAmp = "1.0 1.0 1.0";
   camShakeDuration = 0.9;
   camShakeRadius = 18.0;

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

datablock ProjectileData(grenade_flashbangProjectile)
{
	projectileShapeName = "./dts/flash_projectile.dts";
	directDamage        = 0;
	directDamageType  = $DamageType::FlashBangDirect;
	radiusDamageType  = $DamageType::FlashBangDirect;
	impactImpulse	   = 0;
	verticalImpulse	   = 0;
	explosion           = grenade_flashbangExplosion;
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

	uiName = "G Flash";
};

datablock ItemData(grenade_flashbangItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./dts/flash_item.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "[G] Flash Grenade";
	iconName = "./ico/FLASH";
	doColorShift = false;

	image = grenade_flashbangImage;
	canDrop = true;
};

datablock ShapeBaseImageData(grenade_flashbangImage)
{
	shapeFile = "./dts/flash_image.dts";
	emap = true;

	item = grenade_flashbangItem;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = eulerToMatrix( "0 0 0" );
	className = "WeaponImage";
	armReady = true;

	doColorShift = grenade_flashbangItem.doColorShift;
	colorShiftColor = grenade_flashbangItem.colorShiftColor;

	weaponUseCount = 1;
	weaponReserveMax = 3;

	flashTime = 4;

	projectileType = Projectile;
	projectile = grenade_flashbangProjectile;

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

function grenade_flashbangImage::onReady(%this, %obj, %slot)
{
	%obj.weaponAmmoStart();
}

function grenade_flashbangImage::onChargeStop(%this, %obj, %slot) // overcooked!
{
	%obj.damage(%obj, %obj.getHackPosition(), 33, $DamageType::Suicide);
	%this.onFire(%obj, %slot);
}

function grenade_flashbangImage::onChargeStart(%this, %obj, %slot)
{
	%obj.chargeStartTime[%this] = getSimTime();
	serverPlay3D(grenade_pinLightSound, %obj.getMuzzlePoint(%slot));
	%obj.cookPrint(%this);
}

function grenade_flashbangImage::onFire(%this, %obj, %slot)
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

function grenade_flashbangProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal) { serverPlay3D(grenade_bounceSound,%pos); }

function grenade_flashbangProjectile::onExplode(%this, %obj, %pos)
{
	initContainerRadiusSearch(%pos, 128, $TypeMasks::PlayerObjectType);
	while(isObject(%col = ContainerSearchNext()))
	{
		if(minigameCanDamage(%obj, %col) == 1)
		{
			%fromVector = vectorNormalize(vectorSub(%pos, %col.getEyePoint()));
			%lookDot = vectorDot(%fromVector, %col.getEyeVector());
			%dist = vectorDist(%pos, %col.getEyePoint());

			if(!isObject(firstWord(containerRayCast(%pos,%col.getEyePoint(),$TypeMasks::FxBrickObjectType | $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType, %col))))
			{
				%time = grenade_flashbangImage.flashTime;
				%proxy = 32;
				%deaf = 0.8;
				%dist = vectorDist(%pos, %col.getEyePoint());
				%stunLen = ((%time / 2) * (%lookDot + 1)) / mClampF(%dist - %proxy, 1, 128);
				if(%lookDot > 0)
				{
					// %col.client.play2D(grenade_flashRingSound);
					%col.makeDeaf(%stunLen * %deaf);
					%col.makeBlind(%stunLen);
				}
				else
				{
					%col.makeDeaf(%stunLen * mClampF(%lookDot + 0.5, 0, 1));
					%col.setWhiteOut(%stunLen * mClampF(%lookDot + 0.5, 0, 1));
				}
			}
		}
	}
	Parent::onExplode(%this, %obj, %pos);
}
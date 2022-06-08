datablock AudioProfile(grenade_mollyFireSound)
{
	filename    = "./wav/burn_loop3.wav";
	description = AudioShortLooping3D;
	preload = true;
};

datablock AudioProfile(grenade_mollyFireEndSound)
{
	filename    = "./wav/burn_end3.wav";
	description = AudioShort3D;
	preload = true;
};

datablock ParticleData(grenade_mollyFireParticle)
{
	dragCoefficient		= 0.58;
	windCoefficient		= 1.0;
	gravityCoefficient	= -2;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 750;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 5.0;
	spinRandomMin		= -40.0;
	spinRandomMax		= 40.0;
	useInvAlpha		= false;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "base/data/particles/cloud";
	//animTexName		= "~/data/particles/cloud";

	// Interpolation variables
	colors[0]	= "1 1 0.3 0.0";
	colors[1]	= "1 1 0.3 1.0";
	colors[2]	= "0.6 0.0 0.0 0.0";

	sizes[0]	= 0.0;
	sizes[1]	= 2.6;
	sizes[2]	= 1.6;

	times[0]	= 0.0;
	times[1]	= 0.2;
	times[2]	= 1.0;
};

datablock ParticleEmitterData(grenade_mollyFireEmitter)
{
	ejectionPeriodMS = 14;
	periodVarianceMS = 4;
	ejectionVelocity = 0;
	velocityVariance = 0.0;
	ejectionOffset   = 0.8;
	thetaMin         = 0;
	thetaMax         = 180;
	phiReferenceVel  = 30;
	phiVariance      = 32;
	overrideAdvance = false;

	particles = "grenade_mollyFireParticle";
};

datablock ExplosionData(grenade_mollyFireExplosion)
{
	lifeTimeMS = 200;

	explosionScale = "1 1 1";

	soundProfile = grenade_mollyFireEndSound;

	damageRadius = 0;
	radiusDamage = 0;
};

datablock ProjectileData(grenade_mollyFireProjectile)
{
	projectileShapeName = "./dts/shrapnel.dts";
	directDamage        = 0;
	directDamageType  = $DamageType::mollyDirect;
	radiusDamageType  = $DamageType::mollyDirect;
	impactImpulse	   = 0;
	verticalImpulse	   = 0;
	explosion           = grenade_mollyFireExplosion;
	particleEmitter     = grenade_mollyFireEmitter;

	sound = grenade_mollyFireSound;

	muzzleVelocity      = 10;
	velInheritFactor    = 0;
	explodeOnPlayerImpact = false;
	explodeOnDeath        = true;

	brickExplosionRadius = 0;
	brickExplosionImpact = false;
	brickExplosionForce  = 30;             
	brickExplosionMaxVolume = 50;
	brickExplosionMaxVolumeFloating = 60;

	armingDelay         = 9900; 
	lifetime            = 10000;
	fadeDelay           = 9999;
	bounceElasticity    = 0.2;
	bounceFriction  	  = 0.7;
	bounceAngle         = 0;
  minStickVelocity    = 0;
	isBallistic         = true;
	gravityMod = 1.0;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";

	PrjLoop_enabled = true;
	PrjLoop_maxTicks = -1;
	PrjLoop_tickTime = 100;

	uiName = "G Molotov Fire";
};

datablock ProjectileData(grenade_mollyFireSilentProjectile : grenade_mollyFireProjectile)
{
	explosion           = "";
	sound = "";

	uiName = "G Molotov Fire Silent";
};

datablock ParticleData(grenade_mollyExplosionHazeParticle)
{
	dragCoefficient      = 3;
	gravityCoefficient   = 0;
	windCoefficient		= 0;
	inheritedVelFactor   = 0;
	constantAcceleration = 0.0;
	lifetimeMS           = 6600;
	lifetimeVarianceMS   = 0;
	textureName          = "base/data/particles/cloud";
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	colors[0]     = "1 1 1 0.1";
	colors[1]     = "0.1 0.1 0.1 0.9";
	colors[2]     = "0.05 0.05 0.05 0.1";
	colors[3]     = "0.05 0.05 0.05 0.0";

	sizes[0]	= 4.0;
	sizes[1]	= 3.3;
	sizes[2]	= 3.5;
	sizes[3]	= 2.5;

	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 0.8;
	times[3]	= 1.0;

	useInvAlpha = false;
};
datablock ParticleEmitterData(grenade_mollyExplosionHazeEmitter)
{
	ejectionPeriodMS = 1;
	periodVarianceMS = 0;
	ejectionVelocity = 20;
	velocityVariance = 2.0;
	ejectionOffset   = 0.0;
	thetaMin         = 0;
	thetaMax         = 180;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "grenade_mollyExplosionHazeParticle";
};

datablock ParticleData(grenade_mollyExplosionCloudParticle)
{
	dragCoefficient		= 0.3;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 4600;
	lifetimeVarianceMS	= 00;
	spinSpeed		= 5.0;
	spinRandomMin		= -5.0;
	spinRandomMax		= 5.0;
	useInvAlpha		= false;
	animateTexture		= false;

	textureName		= "base/data/particles/cloud";
	
	colors[0]     = "0.1 0.1 0.1 0.9";
	colors[1]     = "0.0 0.0 0.0 0.0";
	sizes[0]      = 2;
	sizes[1]      = 1;
};

datablock ParticleEmitterData(grenade_mollyExplosionCloudEmitter)
{
	ejectionPeriodMS = 1;
	periodVarianceMS = 0;
	lifeTimeMS	   = 90;
	ejectionVelocity = 7;
	velocityVariance = 1.0;
	ejectionOffset   = 1.0;
	thetaMin         = 89;
	thetaMax         = 90;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "grenade_mollyExplosionCloudParticle";
};

datablock AudioProfile(grenade_mollyExplosionSound)
{
	filename    = "./wav/blast_fire.wav";
	description = AudioDefault3D;
	preload = true;

	pitchRange = 12;
};

datablock AudioProfile(grenade_mollyBurnSound)
{
	filename    = "./wav/burn.wav";
	description = AudioDefault3D;
	preload = true;

	pitchRange = 12;
};

datablock AudioProfile(grenade_mollyExtinguishSound)
{
	filename    = "./wav/extinguish.wav";
	description = AudioShort3D;
	preload = true;

	pitchRange = 4;
};

datablock TriggerData(grenade_mollyTriggerData)
{
	tickPeriodMS = 128;
};
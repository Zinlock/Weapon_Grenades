datablock ParticleData(grenade_dynamiteFuseParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 1.0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 500;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= false;
	animateTexture		= false;

	textureName		= "base/data/particles/star1";

	colors[0]	= "1 1 0.3 0.0";
	colors[1]	= "0.9 0.6 0.0 1.0";
	colors[2]	= "0.6 0.0 0.0 0.0";

	sizes[0]	= 0.5;
	sizes[1]	= 0.35;
	sizes[2]	= 0.1;

	times[0]	= 0.0;
	times[1]	= 0.2;
	times[2]	= 1.0;
};

datablock ParticleEmitterData(grenade_dynamiteFuseEmitter)
{
	ejectionPeriodMS = 8;
	periodVarianceMS = 0;
	ejectionVelocity = 5;
	velocityVariance = 4;
	ejectionOffset = 0;
	thetaMin         = 0.0;
	thetaMax         = 10.0;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;

  particles = "grenade_dynamiteFuseParticle";
};

datablock ParticleData(grenade_dynamiteSmokeParticle)
{
	dragCoefficient		= 2.5;
	windCoefficient		= 0.0;
	gravityCoefficient	= -0.8;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 6000;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 0.0;
	spinRandomMin		= -200.0;
	spinRandomMax		= 200.0;
	useInvAlpha		= true;
	animateTexture		= false;

	textureName		= "base/data/particles/cloud";

	colors[0]	= "0.3 0.3 0.3 0.0";
	colors[1]	= "0.1 0.1 0.1 0.1";
	colors[2]	= "0.0 0.0 0.0 0.0";

	sizes[0]	= 5.5;
	sizes[1]	= 4.35;
	sizes[2]	= 3.1;

	times[0]	= 0.0;
	times[1]	= 0.2;
	times[2]	= 1.0;
};

datablock ParticleEmitterData(grenade_dynamiteSmokeEmitter)
{
	ejectionPeriodMS = 90;
	periodVarianceMS = 0;
	ejectionVelocity = 9;
	velocityVariance = 5;
	ejectionOffset = 0.3;
	thetaMin         = 0.0;
	thetaMax         = 90.0;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;

  particles = "grenade_dynamiteSmokeParticle";
};

datablock ParticleData(grenade_dynamiteDebrisParticle)
{
	dragCoefficient		= 0.5;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.8;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 1000;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 0.0;
	spinRandomMin		= -10000.0;
	spinRandomMax		= 10000.0;
	useInvAlpha		= false;
	animateTexture		= false;

	textureName		= "base/data/particles/dot";

	colors[0]	= "1 1 0.3 0.0";
	colors[1]	= "1 1 0.3 1.0";
	colors[2]	= "0.6 0.0 0.0 0.0";

	sizes[0]	= 0.5;
	sizes[1]	= 0.35;
	sizes[2]	= 0.1;

	times[0]	= 0.0;
	times[1]	= 0.2;
	times[2]	= 1.0;
};

datablock ParticleEmitterData(grenade_dynamiteDebrisEmitter)
{
	ejectionPeriodMS = 25;
	periodVarianceMS = 0;
	ejectionVelocity = 10;
	velocityVariance = 6;
	ejectionOffset = 0.0;
	thetaMin         = 0.0;
	thetaMax         = 90.0;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;

  particles = "grenade_dynamiteDebrisParticle";
};

datablock ParticleData(grenade_dynamiteFireParticle)
{
	dragCoefficient		= 2.5;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 300;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 0.0;
	spinRandomMin		= -10000.0;
	spinRandomMax		= 10000.0;
	useInvAlpha		= false;
	animateTexture		= false;

	textureName		= "base/data/particles/star1";

	colors[0]	= "1 1 0.3 0.0";
	colors[1]	= "1 1 0.3 1.0";
	colors[2]	= "0.6 0.0 0.0 0.0";

	sizes[0]	= 14.5;
	sizes[1]	= 10.35;
	sizes[2]	= 7.1;

	times[0]	= 0.0;
	times[1]	= 0.2;
	times[2]	= 1.0;
};

datablock ParticleEmitterData(grenade_dynamiteFireEmitter)
{
	ejectionPeriodMS = 30;
	periodVarianceMS = 0;
	ejectionVelocity = 13;
	velocityVariance = 9;
	ejectionOffset = 0.3;
	thetaMin         = 0.0;
	thetaMax         = 90.0;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;

  particles = "grenade_dynamiteFireParticle";
};

datablock ParticleData(grenade_dynamiteTrailParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 500;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= false;
	animateTexture		= false;

	textureName		= "base/data/particles/cloud";

	colors[0]	= "1 1 0.3 0.0";
	colors[1]	= "1 1 0.3 1.0";
	colors[2]	= "0.6 0.0 0.0 0.0";

	sizes[0]	= 0.9;
	sizes[1]	= 0.6;
	sizes[2]	= 0.1;

	times[0]	= 0.0;
	times[1]	= 0.2;
	times[2]	= 1.0;
};

datablock ParticleEmitterData(grenade_dynamiteTrailEmitter)
{
	ejectionPeriodMS = 1;
	periodVarianceMS = 0;
	ejectionVelocity = 0;
	velocityVariance = 0;
	ejectionOffset = 0;
	thetaMin         = 0.0;
	thetaMax         = 90.0;  

  particles = "grenade_dynamiteTrailParticle";
};

datablock ProjectileData(grenade_dynamiteDebrisProjectile)
{
	projectileShapeName = "./dts/shrapnel.dts";
	directDamage        = 0;
	directDamageType  = $DamageType::dynamiteDirect;
	radiusDamageType  = $DamageType::dynamiteDirect;
	impactImpulse	   = 30;
	verticalImpulse	   = 0;
	explosion           = "";
	particleEmitter     = grenade_dynamiteTrailEmitter;

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
	lifetime            = 1000;
	fadeDelay           = 999;
	bounceElasticity    = 0.9;
	bounceFriction      = 0.08;
	isBallistic         = true;
	gravityMod = 0.3;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";

	uiName = "";
};

datablock ProjectileData(grenade_dynamiteFireAltProjectile)
{
	projectileShapeName = "";
	directDamage        = 0;
	directDamageType  = $DamageType::dynamiteDirect;
	radiusDamageType  = $DamageType::dynamiteDirect;
	impactImpulse	   = 0;
	verticalImpulse	   = 0;
	explosion           = "";
	particleEmitter     = grenade_dynamiteSmokeEmitter;

	muzzleVelocity      = 10;
	velInheritFactor    = 0;
	explodeOnPlayerImpact = false;
	explodeOnDeath        = false; 

	brickExplosionRadius = 0;
	brickExplosionImpact = false;
	brickExplosionForce  = 0;             
	brickExplosionMaxVolume = 0;
	brickExplosionMaxVolumeFloating = 0;

	armingDelay         = 7900; 
	lifetime            = 8000;
	fadeDelay           = 7999;
	bounceElasticity    = 0.1;
	bounceFriction      = 0.8;
	isBallistic         = true;
	gravityMod = 0.7;

	hasLight    = false;
	lightRadius = 20.0;
	lightColor  = "0.9 0.7 0.2";

	uiName = "";
};

datablock ProjectileData(grenade_dynamiteFireAlt2Projectile)
{
	projectileShapeName = "";
	directDamage        = 0;
	directDamageType  = $DamageType::dynamiteDirect;
	radiusDamageType  = $DamageType::dynamiteDirect;
	impactImpulse	   = 0;
	verticalImpulse	   = 0;
	explosion           = "";
	particleEmitter     = grenade_dynamiteDebrisEmitter;

	muzzleVelocity      = 10;
	velInheritFactor    = 0;
	explodeOnPlayerImpact = false;
	explodeOnDeath        = false; 

	brickExplosionRadius = 0;
	brickExplosionImpact = false;
	brickExplosionForce  = 0;             
	brickExplosionMaxVolume = 0;
	brickExplosionMaxVolumeFloating = 0;

	armingDelay         = 7900; 
	lifetime            = 8000;
	fadeDelay           = 7999;
	bounceElasticity    = 0.1;
	bounceFriction      = 0.8;
	isBallistic         = true;
	gravityMod = 0.7;

	hasLight    = false;
	lightRadius = 20.0;
	lightColor  = "0.9 0.7 0.2";

	uiName = "";
};

datablock AudioProfile(grenade_dynamiteFireSound)
{
	filename    = "./wav/burn_loop2.wav";
	description = AudioShortLooping3D;
	preload = true;
};

datablock ProjectileData(grenade_dynamiteFireProjectile)
{
	projectileShapeName = "./dts/shrapnel.dts";
	directDamage        = 0;
	directDamageType  = $DamageType::dynamiteDirect;
	radiusDamageType  = $DamageType::dynamiteDirect;
	impactImpulse	   = 30;
	verticalImpulse	   = 0;
	explosion           = "";
	particleEmitter     = grenade_dynamiteFireEmitter;
	sound = grenade_dynamiteFireSound;

	muzzleVelocity      = 10;
	velInheritFactor    = 0;
	explodeOnPlayerImpact = false;
	explodeOnDeath        = false; 

	brickExplosionRadius = 0;
	brickExplosionImpact = false;
	brickExplosionForce  = 0;             
	brickExplosionMaxVolume = 0;
	brickExplosionMaxVolumeFloating = 0;

	armingDelay         = 7900; 
	lifetime            = 8000;
	fadeDelay           = 7999;
	bounceElasticity    = 0.1;
	bounceFriction      = 0.8;
	isBallistic         = true;
	gravityMod = 0.7;

	hasLight    = true;
	lightRadius = 20.0;
	lightColor  = "0.9 0.7 0.2";

	PrjLoop_enabled = true;
	PrjLoop_maxTicks = -1;
	PrjLoop_tickTime = 100;

	uiName = "";
};

datablock AudioProfile(grenade_dynamiteExplosionSound)
{
	filename    = "./wav/blast_frag3.wav";
	description = AudioDefault3D;
	preload = true;

	pitchRange = 12;
};

datablock AudioProfile(grenade_dynamiteFuseSound)
{
	filename    = "./wav/fuse_loop.wav";
	description = AudioClosestLooping3D;
	preload = true;
};

datablock TriggerData(grenade_dynamiteTriggerData)
{
	tickPeriodMS = 128;
};
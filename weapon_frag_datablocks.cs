datablock ParticleData(grenade_fragTrailParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 250;
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

	sizes[0]	= 0.3;
	sizes[1]	= 0.35;
	sizes[2]	= 0.1;

	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 1.0;
};

datablock ParticleEmitterData(grenade_fragTrailEmitter)
{
	ejectionPeriodMS = 1;
	periodVarianceMS = 0;
	ejectionVelocity = 0; //0.25;
	velocityVariance = 0; //0.10;
	ejectionOffset = 0;
	thetaMin         = 0.0;
	thetaMax         = 90.0;  

  particles = "grenade_fragTrailParticle";
};

datablock ProjectileData(grenade_shrapnelProjectile)
{
	projectileShapeName = "./dts/shrapnel.dts";
	directDamage        = 19;
	directDamageType  = $DamageType::fragmentDirect;
	radiusDamageType  = $DamageType::fragmentDirect;
	impactImpulse	   = 30;
	verticalImpulse	   = 0;
	explosion           = "";
	particleEmitter     = grenade_fragTrailEmitter;

	muzzleVelocity      = 40;
	velInheritFactor    = 0;
	explodeOnPlayerImpact = true;
	explodeOnDeath        = true;  

	brickExplosionRadius = 0;
	brickExplosionImpact = false;
	brickExplosionForce  = 30;             
	brickExplosionMaxVolume = 50;
	brickExplosionMaxVolumeFloating = 60;

	armingDelay         = 400; 
	lifetime            = 500;
	fadeDelay           = 400;
	bounceElasticity    = 0.9;
	bounceFriction      = 0.08;
	isBallistic         = true;
	gravityMod = 1.0;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";

	uiName = "";
};

datablock AudioProfile(grenade_fragExplosionSound)
{
	filename    = "./wav/blast_frag1.wav";
	description = AudioDefault3D;
	preload = true;

	pitchRange = 12;
};
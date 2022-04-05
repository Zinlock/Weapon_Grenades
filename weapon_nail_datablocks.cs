datablock ParticleData(grenade_nailTrailParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.7;
	gravityCoefficient	= 0.4;
	inheritedVelFactor	= 0.3;
	constantAcceleration	= 0.0;
	lifetimeMS		= 1200;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= false;
	animateTexture		= false;

	textureName		= "base/data/particles/cloud";

	// colors[0]	= "0.5 0.5 0.5 0.0";
	// colors[1]	= "0.3 0.3 0.3 1.0";
	// colors[2]	= "0.0 0.0 0.0 0.0";

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

datablock ParticleEmitterData(grenade_nailTrailEmitter)
{
	ejectionPeriodMS = 1;
	periodVarianceMS = 0;
	ejectionVelocity = 0; //0.25;
	velocityVariance = 0; //0.10;
	ejectionOffset = 0;
	thetaMin         = 0.0;
	thetaMax         = 180.0;  

  particles = "grenade_nailTrailParticle";
};

datablock ProjectileData(grenade_nailProjectile)
{
	projectileShapeName = "./dts/shrapnel.dts";
	directDamage        = 33;
	directDamageType  = $DamageType::nailbombDirect;
	radiusDamageType  = $DamageType::nailbombDirect;
	impactImpulse	   = 40;
	verticalImpulse	   = 0;
	explosion           = "";
	particleEmitter     = grenade_nailTrailEmitter;

	muzzleVelocity      = 30;
	velInheritFactor    = 0;
	explodeOnPlayerImpact = true;
	explodeOnDeath        = true;  

	brickExplosionRadius = 0;
	brickExplosionImpact = false;
	brickExplosionForce  = 30;             
	brickExplosionMaxVolume = 50;
	brickExplosionMaxVolumeFloating = 60;

	armingDelay         = 700; 
	lifetime            = 700;
	fadeDelay           = 700;
	bounceElasticity    = 0.9;
	bounceFriction      = 0.08;
	isBallistic         = true;
	gravityMod = 1.0;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";

	uiName = "";
};

datablock ParticleData(grenade_nailExplosionPopParticle)
{
	dragCoefficient		= 0.5;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.5;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 900;
	lifetimeVarianceMS	= 00;
	spinSpeed		= 0.0;
	spinRandomMin		= -90.0;
	spinRandomMax		= 90.0;
	useInvAlpha		= false;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "base/data/particles/star1";
	//animTexName		= "~/data/particles/cloud";

	// Interpolation variables
  colors[0]     = "1 1 1 0.1";
	colors[1]     = "0.9 0.5 0.0 0.9";
	colors[2]     = "0.1 0.05 0.025 0.1";
	colors[3]     = "0.1 0.05 0.025 0.0";

	sizes[0]	= 4.0;
	sizes[1]	= 3.3;
	sizes[2]	= 5.5;
	sizes[3]	= 4.5;

	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 0.8;
	times[3]	= 1.0;
};

datablock ParticleEmitterData(grenade_nailExplosionPopEmitter)
{
   ejectionPeriodMS = 4;
   periodVarianceMS = 0;
   ejectionVelocity = 8;
   velocityVariance = 1.0;
   ejectionOffset   = 1.0;
   thetaMin         = 0;
   thetaMax         = 180;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "grenade_nailExplosionPopParticle";
};

datablock ParticleData(grenade_nailExplosionParticle)
{
	dragCoefficient		= 0.5;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.5;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 600;
	lifetimeVarianceMS	= 00;
	spinSpeed		= 0.0;
	spinRandomMin		= -3999.0;
	spinRandomMax		= 3999.0;
	useInvAlpha		= false;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "base/data/particles/star1";
	//animTexName		= "~/data/particles/cloud";

	// Interpolation variables
  colors[0]     = "1 1 1 0.1";
	colors[1]     = "0.9 0.5 0.0 0.9";
	colors[2]     = "0.1 0.05 0.025 0.1";
	colors[3]     = "0.1 0.05 0.025 0.0";

	sizes[0]	= 9.0;
	sizes[1]	= 8.3;
	sizes[2]	= 7.5;
	sizes[3]	= 6.5;

	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 0.8;
	times[3]	= 1.0;
};

datablock ParticleEmitterData(grenade_nailExplosionEmitter)
{
   ejectionPeriodMS = 4;
   periodVarianceMS = 0;
   ejectionVelocity = 0.1;
   velocityVariance = 0.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 180;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "grenade_nailExplosionParticle";
};

datablock AudioProfile(grenade_nailExplosionSound)
{
	filename    = "./wav/blast_conc_strong.wav";
	description = AudioDefault3D;
	preload = true;

	pitchRange = 12;
};
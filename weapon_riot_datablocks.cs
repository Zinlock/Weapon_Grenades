datablock ParticleData(grenade_riotExplosionHazeParticle)
{
	dragCoefficient      = 3;
	gravityCoefficient   = 0;
	windCoefficient		= 0;
	inheritedVelFactor   = 0;
	constantAcceleration = 0.0;
	lifetimeMS           = 19000;
	lifetimeVarianceMS   = 0;
	textureName          = "base/data/particles/cloud";
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	colors[0]     = "0.5 0.1 0.0 0.05";
	colors[1]     = "0.4 0.0 0.0 0.1";
	colors[2]     = "0.3 0.0 0.0 0.05";
	colors[3]     = "0.05 0.0 0.0 0.0";

	sizes[0]	= 6.0;
	sizes[1]	= 4.3;
	sizes[2]	= 4.5;
	sizes[3]	= 3.5;

	times[0]	= 0.0;
	times[1]	= 0.7;
	times[2]	= 0.9;
	times[3]	= 1.0;

	useInvAlpha = true;
};
datablock ParticleEmitterData(grenade_riotExplosionHazeEmitter)
{
   ejectionPeriodMS = 1;
   periodVarianceMS = 0;
   ejectionVelocity = 16;
   velocityVariance = 2.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 180;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "grenade_riotExplosionHazeParticle";
};

datablock ExplosionData(grenade_riotDizzyExplosion)
{
	lifeTimeMS = 100;

	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeFreq = "0.25 0.15 0.25";
	camShakeAmp = "0.5 1.0 0.5";
	camShakeDuration = 7;
	camShakeRadius = 0.1;

	// Dynamic light
	lightStartRadius = 0;
	lightEndRadius = 0;

	damageRadius = 1;
	radiusDamage = 0;

	impulseRadius = 1;
	impulseForce = 10;
};

datablock ProjectileData(grenade_riotDizzyProjectile)
{
	projectileShapename = "base/data/shapes/empty.dts";
	directDamage = 0;

	explosion = grenade_riotDizzyExplosion;
	hasLight = false;

	lifetime = 100;
};

datablock TriggerData(grenade_riotTriggerData)
{
	tickPeriodMS = 100;
};
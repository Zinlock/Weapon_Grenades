datablock ParticleData(grenade_smokeExplosionHazeParticle)
{
	dragCoefficient      = 3;
	gravityCoefficient   = 0;
	windCoefficient		= 0;
	inheritedVelFactor   = 0;
	constantAcceleration = 0.0;
	lifetimeMS           = 15000;
	lifetimeVarianceMS   = 0;
	textureName          = "base/data/particles/cloud";
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	colors[0]     = "1 1 1 1";
	colors[1]     = "0.1 0.1 0.1 1";
	colors[2]     = "0.05 0.05 0.05 0.6";
	colors[3]     = "0.05 0.05 0.05 0.0";

	sizes[0]	= 6.0;
	sizes[1]	= 4.3;
	sizes[2]	= 4.5;
	sizes[3]	= 3.5;

	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 0.8;
	times[3]	= 1.0;

	useInvAlpha = true;
};
datablock ParticleEmitterData(grenade_smokeExplosionHazeEmitter)
{
   ejectionPeriodMS = 1;
   periodVarianceMS = 0;
   ejectionVelocity = 13;
   velocityVariance = 2.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 180;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "grenade_smokeExplosionHazeParticle";
};

datablock ParticleData(grenade_smokeExplosionCloudParticle)
{
	dragCoefficient		= 1.6;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 3200;
	lifetimeVarianceMS	= 00;
	spinSpeed		= 5.0;
	spinRandomMin		= -5.0;
	spinRandomMax		= 5.0;
	useInvAlpha		= true;
	animateTexture		= false;

	textureName		= "base/data/particles/cloud";
	
	colors[0]     = "0.1 0.1 0.1 0.9";
	colors[1]     = "0.0 0.0 0.0 0.0";
	sizes[0]      = 4;
	sizes[1]      = 3;
};

datablock ParticleEmitterData(grenade_smokeExplosionCloudEmitter)
{
   ejectionPeriodMS = 1;
   periodVarianceMS = 0;
   lifeTimeMS	   = 90;
   ejectionVelocity = 7;
   velocityVariance = 0.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 180;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "grenade_smokeExplosionCloudParticle";
};

datablock AudioProfile(grenade_smokeExplosionSound)
{
	filename    = "./wav/blast_conc_weak.wav";
	description = AudioDefault3D;
	preload = true;

	pitchRange = 12;
};
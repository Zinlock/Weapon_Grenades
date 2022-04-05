datablock ParticleData(grenade_flashExplosionHazeParticle)
{
	dragCoefficient      = 3;
	gravityCoefficient   = 0;
	windCoefficient		= 0;
	inheritedVelFactor   = 0;
	constantAcceleration = 0.0;
	lifetimeMS           = 1600;
	lifetimeVarianceMS   = 0;
	textureName          = "base/data/particles/cloud";
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
   	colors[0]     = "0.7 0.7 0.7 0.8";
   	colors[1]     = "0.4 0.4 0.4 0.1";
   	colors[2]     = "0.1 0.1 0.1 0.0";
	times[0]	= 0.0;
	times[1]	= 0.1;
   	times[2]	= 1.0;
	sizes[0]      = 3.0;
	sizes[1]      = 3.85;
	sizes[2]      = 4.35;

	useInvAlpha = true;
};
datablock ParticleEmitterData(grenade_flashExplosionHazeEmitter)
{
   ejectionPeriodMS = 3;
   periodVarianceMS = 0;
   ejectionVelocity = 6;
   velocityVariance = 1.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 180;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "grenade_flashExplosionHazeParticle";

   useEmitterColors = true;
};

datablock ParticleData(grenade_flashExplosionCloudParticle)
{
	dragCoefficient		= 1.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 2800;
	lifetimeVarianceMS	= 00;
	spinSpeed		= 5.0;
	spinRandomMin		= -5.0;
	spinRandomMax		= 5.0;
	useInvAlpha		= false;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "base/data/particles/cloud";
	//animTexName		= "~/data/particles/cloud";

	// Interpolation variables
   colors[0]     = "0.4 0.4 0.4 0.4";
   colors[1]     = "0.1 0.1 0.1 0.0";
	sizes[0]      = 8;
	sizes[1]      = 6;
};

datablock ParticleEmitterData(grenade_flashExplosionCloudEmitter)
{
   ejectionPeriodMS = 4;
   periodVarianceMS = 0;
   lifeTimeMS	   = 90;
   ejectionVelocity = 3;
   velocityVariance = 1.0;
   ejectionOffset   = 1.0;
   thetaMin         = 0;
   thetaMax         = 180;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "grenade_flashExplosionCloudParticle";
};

datablock AudioProfile(grenade_flashEmptySound)
{
	filename    = "./wav/none.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock AudioProfile(grenade_flashExplosionSound)
{
	filename    = "./wav/blast_flash.wav";
	description = AudioDefault3D;
	preload = true;

	pitchRange = 7;
};
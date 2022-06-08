datablock ParticleData(grenade_stimExplosionHazeParticle)
{
	dragCoefficient      = 3;
	gravityCoefficient   = 0;
	windCoefficient		= 0;
	inheritedVelFactor   = 0;
	constantAcceleration = 0.0;
	lifetimeMS           = 14000;
	lifetimeVarianceMS   = 0;
	textureName          = "base/data/particles/cloud";
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	colors[0]     = "0.0 0.1 0.9 0.05";
	colors[1]     = "0.0 0.05 0.6 0.1";
	colors[2]     = "0.0 0.0 0.4 0.05";
	colors[3]     = "0.0 0.0 0.05 0.0";

	sizes[0]	= 6.0;
	sizes[1]	= 4.3;
	sizes[2]	= 4.5;
	sizes[3]	= 3.5;

	times[0]	= 0.0;
	times[1]	= 0.7;
	times[2]	= 0.9;
	times[3]	= 1.0;

	useInvAlpha = false;
};
datablock ParticleEmitterData(grenade_stimExplosionHazeEmitter)
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
   particles = "grenade_stimExplosionHazeParticle";
};

datablock TriggerData(grenade_stimTriggerData)
{
	tickPeriodMS = 100;
};
datablock ExplosionData(grenade_clusterletExplosion)
{
	lifeTimeMS = 150;

	soundProfile = ""; //grenade_clusterExplosionSound; // played via onExplode

	particleEmitter = grenade_concExplosionDebris2Emitter;
	particleDensity = 100;
	particleRadius = 0.7;

	emitter[0] = grenade_nailExplosionPopEmitter;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeFreq = "1.0 1.0 1.0";
	camShakeAmp = "19.8 19.8 19.8";
	camShakeDuration = 0.8;
	camShakeRadius = 11.0;

	// Dynamic light
	lightStartRadius = 0;
	lightEndRadius = 0;
	lightStartColor = "0.45 0.3 0.1";
	lightEndColor = "0 0 0";

	//impulse
	impulseRadius = 5;
	impulseForce = 400;

	damageRadius = 11;
	radiusDamage = 38;

	uiName = "";
};

datablock ProjectileData(grenade_clusterletProjectile)
{
	projectileShapeName = "./dts/shrapnel.dts";
	directDamage        = 0;
	directDamageType  = $DamageType::clusterDirect;
	radiusDamageType  = $DamageType::clusterDirect;
	impactImpulse	   = 30;
	verticalImpulse	   = 0;
	explosion           = grenade_clusterletExplosion;
	particleEmitter     = grenade_nailTrailEmitter;

	muzzleVelocity      = 8;
	velInheritFactor    = 0;
	explodeOnPlayerImpact = false;
	explodeOnDeath        = true;  

	brickExplosionRadius = 4;
	brickExplosionImpact = false;
	brickExplosionForce  = 30;             
	brickExplosionMaxVolume = 50;
	brickExplosionMaxVolumeFloating = 60;

	armingDelay         = 4900; 
	lifetime            = 5000; //fuse time is set in onExplode
	fadeDelay           = 4999;
	bounceElasticity    = 0.5;
	bounceFriction      = 0.5;
	isBallistic         = true;
	gravityMod = 0.5;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";

	uiName = "";
};

datablock AudioProfile(grenade_clusterExplosionSound)
{
	filename    = "./wav/blast_stinger.wav";
	description = AudioDefault3D;
	preload = true;

	pitchRange = 12;
};

datablock AudioProfile(grenade_clusterletExplosion1Sound)
{
	filename    = "./wav/blast_cluster0.wav";
	description = AudioDefault3D;
	preload = true;

	pitchRange = 12;
};

datablock AudioProfile(grenade_clusterletExplosion2Sound)
{
	filename    = "./wav/blast_cluster1.wav";
	description = AudioDefault3D;
	preload = true;

	pitchRange = 12;
};
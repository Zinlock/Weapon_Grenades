datablock ExplosionData(grenade_remoteChargeExplosion)
{
	explosionShape = "Add-Ons/Weapon_Rocket_Launcher/explosionSphere1.dts";
	lifeTimeMS = 150;

	soundProfile = grenade_clusterletExplosion1Sound;

	emitter[0] = grenade_mollyExplosionCloudEmitter;
	emitter[1] = grenade_concExplosionDebrisEmitter;
	emitter[2] = grenade_concExplosionDebris2Emitter;
	emitter[3] = grenade_concExplosionConeEmitter;

	particleEmitter = grenade_mollyExplosionHazeEmitter;
	particleDensity = 100;
	particleRadius = 0.7;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeFreq = "1.0 1.0 1.0";
	camShakeAmp = "1.8 1.8 1.8";
	camShakeDuration = 1.8;
	camShakeRadius = 8.0;

	// Dynamic light
	lightStartRadius = 0;
	lightEndRadius = 0;
	lightStartColor = "0.45 0.3 0.1";
	lightEndColor = "0 0 0";

	//impulse
	impulseRadius = 13;
	impulseForce = 500;

	damageRadius = 14;
	radiusDamage = 110;

	uiName = "";
};

datablock StaticShapeData(grenade_remoteChargePlantedShape)
{
	shapeFile = "./dts/remote_charge_projectile.dts";
};

datablock AudioProfile(grenade_remoteThrowSound)
{
	filename    = "./wav/throw1.wav";
	description = AudioShort3D;
	preload = true;

	pitchRange = 12;
};

datablock AudioProfile(grenade_remoteClickSound)
{
	filename    = "./wav/charge_click.wav";
	description = AudioDefault3D;
	preload = true;

	pitchRange = 6;
};

datablock AudioProfile(grenade_remoteBeepSound)
{
	filename    = "./wav/charge_on.wav";
	description = AudioShort3D;
	preload = true;
};
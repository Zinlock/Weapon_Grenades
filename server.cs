if(!isFile("Add-Ons/Weapon_Rocket_Launcher/server.cs"))
{
	error("Weapon_Grenades Error: Required Add-On Weapon_Rocket_Launcher not found!");
	return;
}

exec("./support_uselimit.cs");
exec("./support_prjloop.cs");
exec("./support_items.cs");
exec("./effect_stunzap.cs");

function SimObject::IsA(%obj, %type) { return %obj.getClassName() $= %type; }

function Normal2Rotation(%normal)  
{  
	if(getWord(%normal, 2) == 1 || getWord(%normal, 2) == -1)
	{    
			%rotAxis = vectorNormalize(vectorCross(%normal, "0 1 0"));
	}
	else
	{
			%rotAxis = vectorNormalize(vectorCross(%normal, "0 0 1"));
	}

	%initialAngle = mACos(vectorDot(%normal, "0 0 1"));
	%rotation = %rotAxis SPC mRadtoDeg(%initialAngle);
	
	
	return %rotation;  
}

function Player::grenade_UpdateSpeed(%pl)
{
	if(isFunction("Player", "aeUpdateSpeed")) // aebase compatibilty
		%pl.aeUpdateSpeed();
	else if(isFunction("Player", "RWep_UpdateSpeed")) // rallypack compatibility
		%pl.RWep_updateSpeed();
	else // neither aebase nor rallypack is enabled, do the funny manually
	{
		if(%pl.rex_stun $= "")
			%pl.rex_stun = 1;

		%data = %pl.getDatablock();

		%pl.setMaxForwardSpeed(%data.maxForwardSpeed * %pl.rex_stun);
		%pl.setMaxBackwardSpeed(%data.maxBackwardSpeed * %pl.rex_stun);
		%pl.setMaxSideSpeed(%data.maxSideSpeed * %pl.rex_stun);
		%pl.setMaxCrouchForwardSpeed(%data.maxForwardCrouchSpeed * %pl.rex_stun);
		%pl.setMaxCrouchBackwardSpeed(%data.maxBackwardCrouchSpeed * %pl.rex_stun);
		%pl.setMaxCrouchSideSpeed(%data.maxSideCrouchSpeed * %pl.rex_stun);
	}
}

function ProjectileFire(%db, %pos, %vec, %spd, %amt, %srcSlot, %srcObj, %srcCli)
{
	%projectile = %db;
	%spread = %spd / 1000;
	%shellcount = %amt;

	%shells = -1;

	for(%shell=0; %shell<%shellcount; %shell++)
	{
		%velocity = VectorScale(%vec, %projectile.muzzleVelocity); // fuck velocity inheritance :DD
		%x = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;
		%y = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;
		%z = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;
		%mat = MatrixCreateFromEuler(%x @ " " @ %y @ " " @ %z);
		%velocity = MatrixMulVector(%mat, %velocity);

		%p = new Projectile()
		{
			dataBlock = %projectile;
			initialVelocity = %velocity;
			initialPosition = %pos;
			sourceObject = %srcObj;
			sourceSlot = %srcSlot;
			sourceInv = %srcObj.currTool;
			client = %srcCli;
		};
		MissionCleanup.add(%p);

		%shells = %shells TAB %p;
	}

	return removeField(%shells, 0);
}

function Projectile::FuseExplode(%proj) //this function fixes fuse time at the cost of discarding any non default fields
{
	%db = %proj.getDatablock();
	%vel = %proj.getVelocity();
	%pos = %proj.getPosition();
	%sObj = %proj.sourceObject;
	%sSlot = %proj.sourceSlot;
	%cli = %proj.client;

	%proj.delete();

	if(vectorLen(%vel) == 0)
		%vel = "0 0 0.1";

	%p = new Projectile()
	{
		dataBlock = %db;
		initialVelocity = %vel;
		initialPosition = %pos;
		sourceObject = %sObj;
		sourceSlot = %sSlot;
		client = %cli;
	};
	
	MissionCleanup.add(%p);

	%p.explode();
}

function Player::cookPrint(%pl, %img)
{
	if(!isObject(%pl) || !isObject(%cl = %pl.client))
		return;
	
	cancel(%pl.cookSched);

	if(%pl.chargeStartTime[%img] $= "" || !isObject(%pl.getMountedImage(0)) || %pl.getMountedImage(0).getID() != %img.getID())
		return;
	
	%cl.centerPrint("<color:7F4FA8>" @ mFloatLength((((%img.Projectile.lifeTime * 32) - (getSimTime() - %pl.chargeStartTime[%img])) - %img.lifeTimeOffset) / 1000, 1) @ "s left!", 1);

	%pl.cookSched = %pl.schedule(100, cookPrint, %img);
}

package WeaponAltFire
{
	function WeaponImage::onAltFire(%this, %obj, %slot) { }
	function WeaponImage::onAltRelease(%this, %obj, %slot) { }
	function Armor::onTrigger(%db, %pl, %trig, %val)
	{
		if(isObject(%img = %pl.getMountedImage(0)))
		{
			if(%trig == 4)
			{
				if(%val)
					%img.onAltFire(%pl, 0);
				else
					%img.onAltRelease(%pl, 0);
			}
		}

		Parent::onTrigger(%db, %pl, %trig, %val);
	}
};
activatePackage(WeaponAltFire);

package AudioRandomPitch
{
	function GameConnection::Play3D(%cl, %sound, %pos)
	{
		%pitchDev = %sound.pitchRange;
		if(%pitchDev $= "") return Parent::Play3D(%cl, %sound, %pos);
		
		%maxPitch = 100 + %pitchDev;
		%minPitch = 100 - %pitchDev;
		
		%pitch = (getRandom(%minPitch, %maxPitch)) / 100;

		%oldTimescale = getTimescale();
		setTimescale(%pitch);
		
		Parent::Play3D(%cl, %sound, %pos);
		
		setTimescale(%oldTimescale);
	}

	function GameConnection::Play2D(%cl, %sound)
	{
		%pitchDev = %sound.pitchRange;
		if(%pitchDev $= "") return Parent::Play2D(%cl, %sound);
		
		%maxPitch = 100 + %pitchDev;
		%minPitch = 100 - %pitchDev;
		
		%pitch = (getRandom(%minPitch, %maxPitch)) / 100;

		%oldTimescale = getTimescale();
		setTimescale(%pitch);
		
		Parent::Play2D(%cl, %sound);
		
		setTimescale(%oldTimescale);
	}
};
activatePackage(AudioRandomPitch);

function GameConnection::Play2DSpeed(%cl, %sound, %speed)
{
	%pitch = %speed;// / 100;

	%oldTimescale = getTimescale();
	setTimescale(%pitch);
	
	%cl.Play2D(%sound);
	
	setTimescale(%oldTimescale);
}

datablock AudioDescription(AudioShortLooping3D : AudioClosestLooping3D)
{
	maxDistance = 35;
	referenceDistance = 10;
	volume = 0.7;
};

datablock AudioDescription(AudioShort3D : AudioDefault3D)
{
	maxDistance = 35;
	referenceDistance = 10;
	volume = 0.7;
};

datablock AudioProfile(grenade_bounceSound)
{
	filename    = "./wav/bounce1.wav";
	description = AudioShort3D;
	preload = true;

	pitchRange = 6;
};

datablock AudioProfile(grenade_throwSound)
{
	filename    = "./wav/throw0.wav";
	description = AudioShort3D;
	preload = true;

	pitchRange = 12;
};

datablock AudioProfile(grenade_pinLightSound)
{
	filename    = "./wav/pin_flash.wav";
	description = AudioShort3D;
	preload = true;

	pitchRange = 7;
};

datablock AudioProfile(grenade_pinHeavySound)
{
	filename    = "./wav/pin_frag.wav";
	description = AudioShort3D;
	preload = true;

	pitchRange = 7;
};

exec("./weapon_flash.cs");
exec("./weapon_conc.cs");
exec("./weapon_molly.cs");
exec("./weapon_electro.cs");
exec("./weapon_frag.cs");
exec("./weapon_nail.cs");
exec("./weapon_stick.cs");
exec("./weapon_cluster.cs");
exec("./weapon_remote.cs");
exec("./weapon_smoke.cs");
exec("./weapon_riot.cs");
exec("./weapon_dynamite.cs");
exec("./weapon_decoy.cs");
exec("./weapon_holy.cs");

if(!isFile("Add-Ons/Weapon_Rocket_Launcher/server.cs"))
{
	error("Weapon_Grenades Error: Required Add-On Weapon_Rocket_Launcher not found!");
	return;
}

if ($RTB::Hooks::ServerControl)
{
	RTB_registerPref("Hide Ammo", "Grenades", "$Pref::XNades::hideAmmo", "bool", "Weapon_Grenades", 0, false, false, "");
	RTB_registerPref("Events Bypass Ammo Limit", "Grenades", "$Pref::XNades::eventsBypassAmmo", "bool", "Weapon_Grenades", 0, false, false, "");
}
else
{
	if ($Pref::XNades::hideAmmo $= "") $Pref::XNades::hideAmmo = 0;
	if ($Pref::XNades::eventsBypassAmmo $= "") $Pref::XNades::eventsBypassAmmo = 0;
}

exec("./support_dataprefs.cs");
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
	if($Version !$= 20)
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
}

// triggers dont detect players until they move. this sucks, but it works
function Player::triggerFix(%pl)
{
	%vel = %pl.getVelocity();
	%pl.setVelocity(vectorAdd(%pl.getVelocity(), "0 0 0.01"));
}

function triggerFix(%pos, %rad)
{
	initContainerRadiusSearch(%pos, %rad, $TypeMasks::PlayerObjectType);
	while(isObject(%col = containerSearchNext()))
		%col.triggerFix();
}

function ProjectileFire(%db, %pos, %vec, %spd, %amt, %srcSlot, %srcObj, %srcCli, %vel)
{	
	%projectile = %db;
	%spread = %spd / 1000;
	%shellcount = %amt;

	if(%vel $= "")
		%vel = %projectile.muzzleVelocity;

	%shells = -1;

	for(%shell=0; %shell<%shellcount; %shell++)
	{
		%velocity = VectorScale(%vec, %vel); // fuck velocity inheritance :DD
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
	
	%val = (getSimTime() - %pl.chargeStartTime[%img]) / (%img.Projectile.lifeTime * 32);

	%str = "<font:impact:16><color:44FF44>";
	%bars = 60;

	for(%i = %bars; %i >= 0; %i--)
	{
		if(%i == mCeil(%val * %bars))
			%str = %str @ "<color:444444>";

		%str = %str @ ".";
	}

	%cl.centerPrint(%str @ "<br><color:7F4FA8><font:arial bold:16>" @ mFloatLength((((%img.Projectile.lifeTime * 32) - (getSimTime() - %pl.chargeStartTime[%img])) - %img.lifeTimeOffset) / 1000, 1) @ "s", 1);

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

exec("./Support_AudioPitch.cs");

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
exec("./weapon_ninebang.cs");
exec("./weapon_stim.cs");

registerDataPref("Default Reserve Cluster Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 1, false, false, grenade_clusterImage, weaponUseCount);
registerDataPref("Max Reserve Cluster Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 2, false, false, grenade_clusterImage, weaponReserveMax);

registerDataPref("Default Reserve Concussion Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 1, false, false, grenade_concussionImage, weaponUseCount);
registerDataPref("Max Reserve Concussion Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 2, false, false, grenade_concussionImage, weaponReserveMax);

registerDataPref("Default Reserve Decoy Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 1, false, false, grenade_decoyImage, weaponUseCount);
registerDataPref("Max Reserve Decoy Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 2, false, false, grenade_decoyImage, weaponReserveMax);

registerDataPref("Default Reserve Electric Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 1, false, false, grenade_electroImage, weaponUseCount);
registerDataPref("Max Reserve Electric Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 2, false, false, grenade_electroImage, weaponReserveMax);

registerDataPref("Default Reserve Flash Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 1, false, false, grenade_flashbangImage, weaponUseCount);
registerDataPref("Max Reserve Flash Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 2, false, false, grenade_flashbangImage, weaponReserveMax);

registerDataPref("Default Reserve Frag Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 1, false, false, grenade_fragmentImage, weaponUseCount);
registerDataPref("Max Reserve Frag Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 2, false, false, grenade_fragmentImage, weaponReserveMax);

registerDataPref("Default Reserve Health Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 1, false, false, grenade_stimImage, weaponUseCount);
registerDataPref("Max Reserve Health Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 2, false, false, grenade_stimImage, weaponReserveMax);

registerDataPref("Default Reserve Holy Hand Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 1, false, false, grenade_holyhandImage, weaponUseCount);
registerDataPref("Max Reserve Holy Hand Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 2, false, false, grenade_holyhandImage, weaponReserveMax);

registerDataPref("Default Reserve Molotovs", "Ammo", "Weapon_Grenades", "int 0 1000", 1, false, false, grenade_mollyImage, weaponUseCount);
registerDataPref("Max Reserve Molotovs", "Ammo", "Weapon_Grenades", "int 0 1000", 2, false, false, grenade_mollyImage, weaponReserveMax);

registerDataPref("Default Reserve Nail Bombs", "Ammo", "Weapon_Grenades", "int 0 1000", 1, false, false, grenade_nailbombImage, weaponUseCount);
registerDataPref("Max Reserve Nail Bombs", "Ammo", "Weapon_Grenades", "int 0 1000", 2, false, false, grenade_nailbombImage, weaponReserveMax);

registerDataPref("Default Reserve Nine-Bang Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 1, false, false, grenade_ninebangImage, weaponUseCount);
registerDataPref("Max Reserve Nine-Bang Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 2, false, false, grenade_ninebangImage, weaponReserveMax);

registerDataPref("Default Reserve Remote Charges", "Ammo", "Weapon_Grenades", "int 0 1000", 1, false, false, grenade_remoteImage, weaponUseCount);
registerDataPref("Max Reserve Remote Charges", "Ammo", "Weapon_Grenades", "int 0 1000", 8, false, false, grenade_remoteImage, weaponReserveMax);

registerDataPref("Default Reserve Smoke Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 1, false, false, grenade_smokeImage, weaponUseCount);
registerDataPref("Max Reserve Smoke Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 2, false, false, grenade_smokeImage, weaponReserveMax);

registerDataPref("Default Reserve Stick Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 1, false, false, grenade_stickImage, weaponUseCount);
registerDataPref("Max Reserve Stick Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 2, false, false, grenade_stickImage, weaponReserveMax);

registerDataPref("Default Reserve Tear Gas Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 1, false, false, grenade_riotImage, weaponUseCount);
registerDataPref("Max Reserve Tear Gas Grenades", "Ammo", "Weapon_Grenades", "int 0 1000", 2, false, false, grenade_riotImage, weaponReserveMax);

registerDataPref("Default Reserve Thermite Sticks", "Ammo", "Weapon_Grenades", "int 0 1000", 1, false, false, grenade_dynamiteImage, weaponUseCount);
registerDataPref("Max Reserve Thermite Sticks", "Ammo", "Weapon_Grenades", "int 0 1000", 2, false, false, grenade_dynamiteImage, weaponReserveMax);

registerDataPref("Explosion Clusterlets (30)", "Cluster Grenade", "Weapon_Grenades", "int 0 200", 30, false, false, grenade_clusterImage, clusterlets);

registerDataPref("Zap Damage (0.9)", "Electric Grenade", "Weapon_Grenades", "int 0 1000", 0.9, false, false, grenade_electroImage, zapDamage);
registerDataPref("Zap Time (5s)", "Electric Grenade", "Weapon_Grenades", "int 0 1000", 5, false, false, grenade_electroImage, zapTime);
registerDataPref("Zap Radius (10u)", "Electric Grenade", "Weapon_Grenades", "int 0 1000", 10, false, false, grenade_electroImage, zapRadius);

registerDataPref("Flash Time (4s)", "Flash Grenade", "Weapon_Grenades", "int 0 100", 4, false, false, grenade_flashbangImage, flashTime);

registerDataPref("Explosion Shrapnel (75)", "Frag Grenade", "Weapon_Grenades", "int 0 200", 75, false, false, grenade_fragmentImage, clusterlets);

registerDataPref("Burn Damage (3.5)", "Molotov", "Weapon_Grenades", "int 0 1000", 3.5, false, false, grenade_mollyImage, burnDamage);
registerDataPref("Afterburn Damage (2.5)", "Molotov", "Weapon_Grenades", "int 0 1000", 2.5, false, false, grenade_mollyImage, afterBurnDamage);
registerDataPref("Afterburn Time (4s)", "Molotov", "Weapon_Grenades", "int 0 1000", 4, false, false, grenade_mollyImage, afterBurnTime);

registerDataPref("Explosion Nails (30)", "Nail Bomb", "Weapon_Grenades", "int 0 200", 30, false, false, grenade_nailbombImage, clusterlets);

registerDataPref("Flash Time (3s)", "Nine-Bang Grenade", "Weapon_Grenades", "int 0 100", 3, false, false, grenade_ninebangImage, flashTime);

registerDataPref("Gas Heal (2.5)", "Stim Grenade", "Weapon_Grenades", "int 0 1000", 2.5, false, false, grenade_stimImage, tickHeal);

registerDataPref("Max Active Charges (4)", "Remote Charge", "Weapon_Grenades", "int 0 100", 4, false, false, grenade_remoteImage, maxActive);

registerDataPref("Dizzy Time (5s)", "Tear Gas Grenade", "Weapon_Grenades", "int 0 30", 5, false, false, grenade_riotImage, slowTime);
registerDataPref("Dizzy Speed Multiplier (0.6x)", "Tear Gas Grenade", "Weapon_Grenades", "int 0 10", 0.6, false, false, grenade_riotImage, slowSpeed);
registerDataPref("Dizzy Damage Multiplier (2x)", "Tear Gas Grenade", "Weapon_Grenades", "int 0 50", 2, false, false, grenade_riotImage, damageMult);

registerDataPref("Burn Damage (4.2)", "Thermite Stick", "Weapon_Grenades", "int 0 1000", 4.2, false, false, grenade_dynamiteImage, burnDamage);
registerDataPref("Afterburn Damage (3.2)", "Thermite Stick", "Weapon_Grenades", "int 0 1000", 3.2, false, false, grenade_dynamiteImage, afterBurnDamage);
registerDataPref("Afterburn Time (4s)", "Thermite Stick", "Weapon_Grenades", "int 0 1000", 4, false, false, grenade_dynamiteImage, afterBurnTime);

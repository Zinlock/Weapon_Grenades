// Support_DataPrefs by Oxy (260031)
// A simple support script that lets mods register prefs applied to datablock fields instead of only global variables
// Also includes a custom safeTransmitDatablocks function that avoids transmitting to players still in loading
// Usage:
//   registerDataPref("Preference Title", "Category", "Weapon_YourModName", "type", defaultValue, hostOnly, restartRequired, datablockName, datablockFieldName, transmitDatablocks);

function registerDataPref(%title, %category, %addon, %type, %default, %host, %restart, %data, %field, %td)
{
	if(isObject(%data))
	{
		if($Pref::DataPref[%data, %field] !$= "")
			%data.setField(%field, $Pref::DataPref[%data, %field]);
		else
		{
			$Pref::DataPref[%data, %field] = %default;
			%data.setField(%field, %default);
		}
	}
	else
	{
		error("registerDataPref() - Object '" @ %data @ "' does not exist");
		return -1;
	}

	if(strlen($BLPrefs::Version))
	{
		%pref = new ScriptObject(Preference) {
			className      = "DataPref";

			addon          = %addon;
			category       = %category;
			title          = %title;

			type           = getWord(%type, 0);
			params				 = getWords(%type, 1);

			variable       = "$Pref::DataPref" @ %data @ "_" @ %field;
			defaultValue   = %default;

			hostOnly       = %host;
			secret         = false;

			loadNow        = false;
			noSave         = false;
			requireRestart = %restart;

			data = %data;
			dataField = %field;
			safeTD = %td;
		};

		return %pref;
	}
	// else warn("registerDataPref() - Missing or unsupported preferences mod");
}

function DataPref::onLoad(%pref, %val)
{
	if(isObject(%pref.data))
	{
		if(%val $= "")
			%val = $Pref::DataPref[%pref.data, %pref.dataField];
		
		//%pref.data.setField(%pref.dataField, %val);
		eval(%pref.data @ "." @ %pref.dataField @ " = " @ %val @ ";"); // terrible. horrible, even. but this'll have to do for now
	}
}

function DataPref::onUpdate(%pref, %val)
{
	if(isObject(%pref.data))
	{
		//%pref.data.setField(%pref.dataField, %val);
		eval(%pref.data @ "." @ %pref.dataField @ " = " @ %val @ ";");

		if(%pref.safeTD)
			safeTransmitDatablocks(1);
	}
}

function safeTransmitDatablocks(%silent)
{
	if(!%silent)
		messageAll('', "Transmitting Datablocks... (safe)");

	%cts = ClientGroup.getCount();
	for(%i = 0; %i < %cts; %i++)
	{
		%cl = ClientGroup.getObject(%i);

		if(%cl.hasSpawnedOnce && %cl.currentPhase == 3)
			%cl.transmitDataBlocks($missionSequence);
		else
			%cl.dataQueued = true;
	}
}

package DataPrefsPkg
{
	function GameConnection::onClientEnterGame(%cl)
	{
		Parent::onClientEnterGame(%cl);

		if(%cl.dataQueued)
		{
			%cl.transmitDataBlocks($missionSequence);
			%cl.dataQueued = false;
		}
	}
};
activatePackage(DataPrefsPkg);
// Support_DataPrefs by Oxy (260031)
// A simple support script that lets mods register prefs applied to datablock fields instead of only global variables
// Also includes a custom safeTransmitDatablocks function that avoids transmitting to players still in loading
// Usage:
//   registerDataPref("Preference Title", "Category", "Weapon_YourModName", "type", defaultValue, hostOnly, restartRequired, datablockName, datablockFieldName, transmitDatablocks);

function registerDataPref(%title, %category, %addon, %type, %default, %host, %restart, %dataName, %fieldName, %td)
{
	%data = getSafeVariableName(%dataName);
	// %data = %dataName;
	%field = getSafeVariableName(%fieldName);
	// %field = %fieldName;

	if(%data !$= %dataName)
	{
		error("registerDataPref() - Invalid datablock name '" @ %dataName @ "'");
		return -1;
	}

	if(%field !$= %fieldName)
	{
		error("registerDataPref() - Invalid field name '" @ %dataName @ "'");
		return -1;
	}

	if(isObject(%data))
	{
		if($Pref::DataPref[%data, %field] !$= "")
			eval(%data @ "." @ %field @ " = $Pref::DataPref[" @ %data @ ", " @ %field @ "];");
		else
		{
			$Pref::DataPref[%data, %field] = expandEscape(%default);
			eval(%data @ "." @ %field @ " = \"" @ expandEscape(%default) @ "\";");
		}
	}
	else
	{
		error("registerDataPref() - Object '" @ %dataName @ "' does not exist");
		return -1;
	}

	%global = "$Pref::DataPref" @ %data @ "_" @ %field;

	if(strlen($BLPrefs::Version))
	{
		%pref = new ScriptObject(Preference) {
			className      = "DataPref";

			addon          = %addon;
			category       = %category;
			title          = %title;

			type           = getWord(%type, 0);
			params				 = getWords(%type, 1);

			variable       = %global;
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
	else if($RTB::Hooks::ServerControl)
	{
		$doSafeTD[%data, %field] = true;
		%callback = "DataPrefCallback__" @ %data @ "_" @ %field;
		eval("function " @ %callback @ "(){" @ %data @ "." @ %field @ " = " @ %global @ "; if($doSafeTD[" @ %data @ ", " @ %field @ "]) safeTransmitDatablocks(1);}");

		RTB_registerPref(%title, %category, %global, %type, %addon, %default, %restart, %host, %callback);
		return 0;
	}
}

function DataPref::onLoad(%pref, %val)
{
	if(isObject(%pref.data))
	{
		if(%val $= "")
			%val = $Pref::DataPref[%pref.data, %pref.dataField];
		
		eval(%pref.data @ "." @ %pref.dataField @ " = \"" @ expandEscape(%val) @ "\";");
	}
}

function DataPref::onUpdate(%pref, %val)
{
	if(isObject(%pref.data))
	{
		eval(%pref.data @ "." @ %pref.dataField @ " = \"" @ expandEscape(%val) @ "\";");

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
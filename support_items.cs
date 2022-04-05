// taken from my randomizer dm

function Player::addItem(%pl, %item, %slot)
{
   if(%slot $= "")
   {
      for(%i = 0; %i < %pl.getDatablock().maxTools; %i++)
      {
         %tool = %pl.tool[%i];
         if(%tool == 0)
         {
            %pl.tool[%i] = %item;
            %pl.weaponCount++;
            messageClient(%pl.Client, 'MsgItemPickup', '', %i, %item);
            break;
         }
      }
   }
   else
   {
      if(%slot >= %pl.getDatablock().maxTools)
      {
         %slot = %pl.getDatablock().maxTools - 1;

         %tool = %pl.tool[%slot];
         if(%tool == 0)
	    {
            %pl.weaponCount++;
		  %pl.tool[%slot] = %item;
		  messageClient(%pl.Client, 'MsgItemPickup', '', %slot, %item);
	    }
      }
   }
}

function Player::setItem(%pl, %item, %slot)
{
	if(%slot $= "")
		%slot = 0;

	if(%slot >= %pl.getDatablock().maxTools)
		%slot = %pl.getDatablock().maxTools - 1;

	%tool = %pl.tool[%slot];
	if(%pl.tool[%i] == 0)
			%pl.weaponCount++;
	%pl.tool[%slot] = %item;
	messageClient(%pl.Client, 'MsgItemPickup', '', %slot, %item);
}

function Player::itemLookup(%pl, %id)
{
	for(%i = 0; %i < %pl.getDatablock().maxTools; %i++)
	{
		if(%pl.tool[%i] == %id)
			return %i;
	}
	return -1;
}

function Player::removeItem(%pl, %slot)
{
	if(isObject(%item = %pl.tool[%slot]))
	{
		%pl.weaponCount--;
          %pl.tool[%slot] = 0;
          messageClient (%pl.Client, 'MsgItemPickup', '', %slot, 0);

		if ( %pl.getMountedImage(%item.image.mountPoint)  >  0 )
		{
			if ( %pl.getMountedImage(%item.image.mountPoint).getId() == %item.image.getId() )
			{
				%pl.unmountImage (%item.image.mountPoint);
			}
		}

		return %item;
	}
}

function Player::removeItemLookup(%pl, %id)
{
	for(%i = 0; %i < %pl.getDatablock().maxTools; %i++)
	{
		if(%pl.tool[%i] == %id)
		{
			%slot = %i;
			%pl.weaponCount--;
			%pl.tool[%slot] = 0;
			messageClient (%pl.Client, 'MsgItemPickup', '', %slot, 0);

			if ( %pl.getMountedImage(%item.image.mountPoint)  >  0 )
			{
				if ( %pl.getMountedImage(%item.image.mountPoint).getId() == %item.image.getId() )
				{
					%pl.unmountImage (%item.image.mountPoint);
				}
			}

			return %i;
		}
	}
}
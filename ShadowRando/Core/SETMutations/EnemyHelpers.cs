using ShadowSET;
using System;
using System.Reflection;

namespace ShadowRando.Core.SETMutations;

internal static class EnemyHelpers
{
	public static bool IsFlyingEnemy(SetObjectShadow enemy)
	{
		switch (enemy.Type)
		{
			case 0x65: // GUNBeetle
			case 0x8E: // BkWingLarge
			case 0x8F: // BkWingSmall
			case 0x92: // BkChaos
				return true;
			case 0x66: // GUNBigfoot
				if (((Object0066_GUNBigfoot)enemy).AppearType == Object0066_GUNBigfoot.EAppear.ZUTTO_HOVERING)
				{
					return true;
				}

				break;
			case 0x90: // BkWorm
				if (enemy.UnkBytes[2] == 0x40 && enemy.UnkBytes[6] == 0x40) // BkWorms that spawn on killplanes
				{
					return true;
				}

				break;
			case 0x93: // BkNinja
				if (((Object0093_BkNinja)enemy).AppearType == Object0093_BkNinja.EAppear.ON_AIR_SAUCER_WARP)
				{
					return true;
				}

				break;
			default:
				return false;
		}

		return false;
	}

	public static bool IsRequiredPathTypeFlyingEnemy(SetObjectShadow enemy)
	{
		switch (enemy)
		{
			case Object0065_GUNBeetle beetle when enemy is Object0065_GUNBeetle:
				if (beetle.AppearType == Object0065_GUNBeetle.EAppear.MOVE_ON_PATH)
					return true;
				break;
			case Object008E_BkWingLarge bkWingLarge when enemy is Object008E_BkWingLarge:
				if (bkWingLarge.AppearType == Object008E_BkWingLarge.EAppear.MOVE_ON_PATH)
					return true;
				break;
			case Object008F_BkWingSmall bkWingSmall when enemy is Object008F_BkWingSmall:
				if (bkWingSmall.AppearType == Object008F_BkWingSmall.EAppear.MOVE_ON_PATH)
					return true;
				break;
			default:
				return false;
		}

		return false;
	}

	public static bool IsPathTypeFlyingEnemy(Type enemyType)
	{
		if (enemyType == typeof(Object0065_GUNBeetle)
			|| enemyType == typeof(Object008E_BkWingLarge)
			|| enemyType == typeof(Object008F_BkWingSmall))
		{
			return true;
		}
		return false;
	}
}
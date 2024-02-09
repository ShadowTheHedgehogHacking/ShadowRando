using System;
using System.Collections.Generic;
using ShadowSET;

namespace ShadowRando.Core.SETMutations;

public static class SETMutations
{
	/**
	 *  Pass the index of the object to modify, and an instance of the target end object to inherit some values from
	 *  Will map based on a blend of the original object's data and the instance of the target
	 */
	public static void MutateObjectAtIndex(int index, SetObjectShadow conversionObject,
		ref List<SetObjectShadow> setData, bool isShadow, Random r)
	{
		switch (conversionObject)
		{
			case Object0064_GUNSoldier enemyTarget when conversionObject is Object0064_GUNSoldier:
				EnemySETMutations.ToGUNSoldier(index, enemyTarget, ref setData, r);
				break;
			case Object0065_GUNBeetle enemyTarget when conversionObject is Object0065_GUNBeetle:
				EnemySETMutations.ToGUNBeetle(index, enemyTarget, ref setData, r);
				break;
			case Object0066_GUNBigfoot enemyTarget when conversionObject is Object0066_GUNBigfoot:
				EnemySETMutations.ToGUNBigfoot(index, enemyTarget, ref setData, r);
				break;
			case Object0068_GUNRobot enemyTarget when conversionObject is Object0068_GUNRobot:
				EnemySETMutations.ToGUNRobot(index, enemyTarget, ref setData, r);
				break;
			case Object0078_EggPierrot enemyTarget when conversionObject is Object0078_EggPierrot:
				EnemySETMutations.ToEggPierrot(index, enemyTarget, ref setData, r);
				break;
			case Object0079_EggPawn enemyTarget when conversionObject is Object0079_EggPawn:
				EnemySETMutations.ToEggPawn(index, enemyTarget, ref setData, r);
				break;
			case Object007A_EggShadowAndroid enemyTarget when conversionObject is Object007A_EggShadowAndroid:
				EnemySETMutations.ToEggShadowAndroid(index, enemyTarget, ref setData, r);
				break;
			case Object008C_BkGiant enemyTarget when conversionObject is Object008C_BkGiant:
				EnemySETMutations.ToBkGiant(index, enemyTarget, ref setData, r);
				break;
			case Object008D_BkSoldier enemyTarget when conversionObject is Object008D_BkSoldier:
				EnemySETMutations.ToBkSoldier(index, enemyTarget, ref setData, r);
				break;
			case Object008E_BkWingLarge enemyTarget when conversionObject is Object008E_BkWingLarge:
				EnemySETMutations.ToBkWingLarge(index, enemyTarget, ref setData, r);
				break;
			case Object008F_BkWingSmall enemyTarget when conversionObject is Object008F_BkWingSmall:
				EnemySETMutations.ToBkWingSmall(index, enemyTarget, ref setData, r);
				break;
			case Object0090_BkWorm enemyTarget when conversionObject is Object0090_BkWorm:
				EnemySETMutations.ToBkWorm(index, enemyTarget, ref setData, r);
				break;
			case Object0091_BkLarva enemyTarget when conversionObject is Object0091_BkLarva:
				EnemySETMutations.ToBkLarva(index, enemyTarget, ref setData, r);
				break;
			case Object0092_BkChaos enemyTarget when conversionObject is Object0092_BkChaos:
				EnemySETMutations.ToBkChaos(index, enemyTarget, ref setData, r);
				break;
			case Object0093_BkNinja enemyTarget when conversionObject is Object0093_BkNinja:
				EnemySETMutations.ToBkNinja(index, enemyTarget, ref setData, r);
				break;
			default:
				break; // do nothing
		}
	}

	/**
	 *  Pass the index of the object to modify, and the desired output type
	 *  Will attempt to map based on the original object's data
	 */
	public static void MutateObjectAtIndex(int index, Type setObjectType, ref List<SetObjectShadow> setData,
		bool isShadow, Random r)
	{
		if (setObjectType == typeof(Object003A_SpecialWeaponBox))
		{
			WeaponContainers.ToSpecialWeaponBox(index, ref setData);
		}

		if (setObjectType == typeof(Object000C_WeaponBox))
		{
			WeaponContainers.ToWeaponBox(index, ref setData);
		}

		if (setObjectType == typeof(Object0064_GUNSoldier))
		{
			EnemySETMutations.ToGUNSoldier(index, ref setData, r);
		}
		else if (setObjectType == typeof(Object0065_GUNBeetle))
		{
			EnemySETMutations.ToGUNBeetle(index, ref setData, r);
		}
		else if (setObjectType == typeof(Object0066_GUNBigfoot))
		{
			EnemySETMutations.ToGUNBigfoot(index, ref setData, r);
		}
		else if (setObjectType == typeof(Object0068_GUNRobot))
		{
			EnemySETMutations.ToGUNRobot(index, ref setData, r);
		}
		else if (setObjectType == typeof(Object0078_EggPierrot))
		{
			EnemySETMutations.ToEggPierrot(index, ref setData, r);
		}
		else if (setObjectType == typeof(Object0079_EggPawn))
		{
			EnemySETMutations.ToEggPawn(index, ref setData, r);
		}
		else if (setObjectType == typeof(Object007A_EggShadowAndroid))
		{
			EnemySETMutations.ToEggShadowAndroid(index, ref setData, r);
		}
		else if (setObjectType == typeof(Object008C_BkGiant))
		{
			EnemySETMutations.ToBkGiant(index, ref setData, r);
		}
		else if (setObjectType == typeof(Object008D_BkSoldier))
		{
			EnemySETMutations.ToBkSoldier(index, ref setData, r);
		}
		else if (setObjectType == typeof(Object008E_BkWingLarge))
		{
			EnemySETMutations.ToBkWingLarge(index, ref setData, r);
		}
		else if (setObjectType == typeof(Object008F_BkWingSmall))
		{
			EnemySETMutations.ToBkWingSmall(index, ref setData, r);
		}
		else if (setObjectType == typeof(Object0090_BkWorm))
		{
			EnemySETMutations.ToBkWorm(index, ref setData, r);
		}
		else if (setObjectType == typeof(Object0091_BkLarva))
		{
			EnemySETMutations.ToBkLarva(index, ref setData, r);
		}
		else if (setObjectType == typeof(Object0092_BkChaos))
		{
			EnemySETMutations.ToBkChaos(index, ref setData, r);
		}
		else if (setObjectType == typeof(Object0093_BkNinja))
		{
			EnemySETMutations.ToBkNinja(index, ref setData, r);
		}
	}
}
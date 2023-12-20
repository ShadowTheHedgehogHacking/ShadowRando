using ShadowSET;
using System.Collections.Generic;
using System;

namespace ShadowRando
{
	internal class EnemySETMutations
	{

		/**
		 *  Pass the index of the object to modify, and an instance of the target end object to inherit some values from
		 *  Will map based on a blend of the original object's data and the instance of the target
		 *  Only supports Enemy <-> Enemy at this time
		 */
		public static void MutateObjectAtIndex(int index, SetObjectShadow conversionObject, ref List<SetObjectShadow> setData, bool isShadow, Random r)
		{
			var targetEntry = setData[index];

			switch (conversionObject)
			{
				case Object0064_GUNSoldier enemyTarget when conversionObject is Object0064_GUNSoldier:
					ToGUNSoldier(index, enemyTarget, ref setData, r);
					break;
				case Object0065_GUNBeetle enemyTarget when conversionObject is Object0065_GUNBeetle:
					ToGUNBeetle(index, enemyTarget, ref setData, r);
					break;
				case Object0066_GUNBigfoot enemyTarget when conversionObject is Object0066_GUNBigfoot:
					ToGUNBigfoot(index, enemyTarget, ref setData, r);
					break;
				case Object0068_GUNRobot enemyTarget when conversionObject is Object0068_GUNRobot:
					ToGUNRobot(index, enemyTarget, ref setData, r);
					break;
				case Object0078_EggPierrot enemyTarget when conversionObject is Object0078_EggPierrot:
					ToEggPierrot(index, enemyTarget, ref setData, r);
					break;
				case Object0079_EggPawn enemyTarget when conversionObject is Object0079_EggPawn:
					ToEggPawn(index, enemyTarget, ref setData, r);
					break;
				case Object007A_EggShadowAndroid enemyTarget when conversionObject is Object007A_EggShadowAndroid:
					ToEggShadowAndroid(index, enemyTarget, ref setData, r);
					break;
				case Object008C_BkGiant enemyTarget when conversionObject is Object008C_BkGiant:
					ToBkGiant(index, enemyTarget, ref setData, r);
					break;
				case Object008D_BkSoldier enemyTarget when conversionObject is Object008D_BkSoldier:
					ToBkSoldier(index, enemyTarget, ref setData, r);
					break;
				case Object008E_BkWingLarge enemyTarget when conversionObject is Object008E_BkWingLarge:
					ToBkWingLarge(index, enemyTarget, ref setData, r);
					break;
				case Object008F_BkWingSmall enemyTarget when conversionObject is Object008F_BkWingSmall:
					ToBkWingSmall(index, enemyTarget, ref setData, r);
					break;
				case Object0090_BkWorm enemyTarget when conversionObject is Object0090_BkWorm:
					ToBkWorm(index, enemyTarget, ref setData, r);
					break;
				case Object0091_BkLarva enemyTarget when conversionObject is Object0091_BkLarva:
					ToBkLarva(index, enemyTarget, ref setData, r);
					break;
				case Object0092_BkChaos enemyTarget when conversionObject is Object0092_BkChaos:
					ToBkChaos(index, enemyTarget, ref setData, r);
					break;
				case Object0093_BkNinja enemyTarget when conversionObject is Object0093_BkNinja:
					ToBkNinja(index, enemyTarget, ref setData, r);
					break;
				default:
					break; // do nothing
			}
		}

		/**
		 *  Pass the index of the object to modify, and the desired output type
		 *  Will attempt to map based on the original object's data
		 *  Only supports Enemy <-> Enemy at this time
		 */
		public static void MutateObjectAtIndex(int index, Type setObjectType, ref List<SetObjectShadow> setData, bool isShadow, Random r)
		{
			if (setObjectType == typeof(Object0064_GUNSoldier))
			{
				ToGUNSoldier(index, ref setData, r);
			} else
			{
				ToGUNSoldier(index, ref setData, r);
			}
			/*
			else if (setObjectType == typeof(Object0065_GUNBeetle))
			{
				ToGUNBeetle(index, ref setData, r);
			}
			else if (setObjectType == typeof(Object0066_GUNBigfoot))
			{
				ToGUNBigfoot(index, ref setData, r);
			}
			else if (setObjectType == typeof(Object0068_GUNRobot))
			{
				ToGUNRobot(index, ref setData, r);
			}
			else if (setObjectType == typeof(Object0078_EggPierrot))
			{
				ToEggPierrot(index, ref setData, r);
			}
			else if (setObjectType == typeof(Object0079_EggPawn))
			{
				ToEggPawn(index, ref setData, r);
			}
			else if (setObjectType == typeof(Object007A_EggShadowAndroid))
			{
				ToEggShadowAndroid(index, ref setData, r);
			}
			else if (setObjectType == typeof(Object008C_BkGiant))
			{
				ToBkGiant(index, ref setData, r);
			}
			else if (setObjectType == typeof(Object008D_BkSoldier))
			{
				ToBkSoldier(index, ref setData, r);
			}
			else if (setObjectType == typeof(Object008E_BkWingLarge))
			{
				ToBkWingLarge(index, ref setData, r);
			}
			else if (setObjectType == typeof(Object008F_BkWingSmall))
			{
				ToBkWingSmall(index, ref setData, r);
			}
			else if (setObjectType == typeof(Object0090_BkWorm))
			{
				ToBkWorm(index, ref setData, r);
			}
			else if (setObjectType == typeof(Object0091_BkLarva))
			{
				ToBkLarva(index, ref setData, r);
			}
			else if (setObjectType == typeof(Object0092_BkChaos))
			{
				ToBkChaos(index, ref setData, r);
			}
			else if (setObjectType == typeof(Object0093_BkNinja))
			{
				ToBkNinja(index, ref setData, r);
			} */
		}

		private static void ToGUNSoldier(int index, Object0064_GUNSoldier donor, ref List<SetObjectShadow> setData, Random r, bool ignoreDonorProperties = false)
		{
			var targetEntry = setData[index];
			switch (targetEntry)
			{
				case Object0064_GUNSoldier originalEnemy when targetEntry is Object0064_GUNSoldier:
					break; // for now do nothing, but we may want to extend this to copy the attributions of the donor, depending on the options picked
				case Object0065_GUNBeetle originalEnemy when targetEntry is Object0065_GUNBeetle:
					{
						if (originalEnemy.PathType == Object0065_GUNBeetle.EPathType.FLY_FORWARD_UPDOWN
							|| originalEnemy.PathType == Object0065_GUNBeetle.EPathType.FLY_FORWARD_SWOOP
							|| originalEnemy.PathType == Object0065_GUNBeetle.EPathType.FLY_FORWARD
							|| originalEnemy.PathType == Object0065_GUNBeetle.EPathType.FLY_LEFT)
							break; // We cannot safely convert FLY_* PathType to GroundEnemy, since these are usually way off screen | Probably drop this, should be the responsibility of the caller?
						var newEntry = (Object0064_GUNSoldier)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						// EnemyBase
						newEntry.MoveRange = originalEnemy.MoveRange;
						newEntry.SearchRange = originalEnemy.SearchRange;
						newEntry.SearchAngle = originalEnemy.SearchAngle;
						newEntry.SearchWidth = originalEnemy.SearchWidth;
						newEntry.SearchHeight = originalEnemy.SearchHeight;
						newEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset; // may want to not set this for FlyingEnemy -> GroundEnemy
						newEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase
						newEntry.AppearType = Object0064_GUNSoldier.EAppear.RANDOM_MOVE; // there is no good translation for pos0,1,2 for FlyingEnemy -> GroundEnemy; always set to random move type instead
						newEntry.WeaponType = donor.WeaponType;
						newEntry.HaveShield = donor.HaveShield;
						setData[index] = newEntry;
						break;
					}
				case Object0066_GUNBigfoot originalEnemy when targetEntry is Object0066_GUNBigfoot:
					{
						var newEntry = (Object0064_GUNSoldier)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						// EnemyBase
						newEntry.MoveRange = originalEnemy.MoveRange;
						newEntry.SearchRange = originalEnemy.SearchRange;
						newEntry.SearchAngle = originalEnemy.SearchAngle;
						newEntry.SearchWidth = originalEnemy.SearchWidth;
						newEntry.SearchHeight = originalEnemy.SearchHeight;
						newEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						newEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase
						newEntry.AppearType = Object0064_GUNSoldier.EAppear.RANDOM_MOVE;
						newEntry.WeaponType = donor.WeaponType;
						newEntry.HaveShield = donor.HaveShield;
						setData[index] = newEntry;
						break;
					}
				case Object0068_GUNRobot originalEnemy when targetEntry is Object0068_GUNRobot:
					{
						var newEntry = (Object0064_GUNSoldier)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						// EnemyBase
						newEntry.MoveRange = originalEnemy.MoveRange;
						newEntry.SearchRange = originalEnemy.SearchRange;
						newEntry.SearchAngle = originalEnemy.SearchAngle;
						newEntry.SearchWidth = originalEnemy.SearchWidth;
						newEntry.SearchHeight = originalEnemy.SearchHeight;
						newEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						newEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase

						switch (originalEnemy.AppearType)
						{
							case Object0068_GUNRobot.EAppear.WAIT_ACT:
								newEntry.AppearType = Object0064_GUNSoldier.EAppear.TRIANGLE_MOVE;
								break;
							case Object0068_GUNRobot.EAppear.OFFSET:
							case Object0068_GUNRobot.EAppear.WARP:
								newEntry.AppearType = Object0064_GUNSoldier.EAppear.OFFSETPOS;
								newEntry.Pos0_TranslationXFromOrigin = originalEnemy.OffsetPos_X;
								newEntry.SearchHeightOffset = originalEnemy.OffsetPos_Y; // GUNSoldiers share their SearchHeightOffset property for Offset mode
								newEntry.Pos0_TranslationZFromOrigin = originalEnemy.OffsetPos_Z;
								break;
							case Object0068_GUNRobot.EAppear.XXX:
								newEntry.AppearType = Object0064_GUNSoldier.EAppear.LINEAR_MOVE;
								newEntry.Pos0_TranslationXFromOrigin = originalEnemy.OffsetPos_X;
								newEntry.SearchHeightOffset = originalEnemy.OffsetPos_Y; // GUNSoldiers share their SearchHeightOffset property for Offset mode (XXX is basically offset mode)
								newEntry.Pos0_TranslationZFromOrigin = originalEnemy.OffsetPos_Z;
								break;
						}

						Object0064_GUNSoldier.EWeapon originalWeaponTypeMapping = 0;
						switch (originalEnemy.WeaponType)
						{
							case Object0068_GUNRobot.EWeapon.AUTORIFLE:
								originalWeaponTypeMapping = Object0064_GUNSoldier.EWeapon.MACHINEGUN;
								break;
							case Object0068_GUNRobot.EWeapon.AIRCRAFTRIFLE:
							case Object0068_GUNRobot.EWeapon.LASERRIFLE:
								originalWeaponTypeMapping = Object0064_GUNSoldier.EWeapon.RIFLE;
								break;
							case Object0068_GUNRobot.EWeapon.BAZOOKA:
								originalWeaponTypeMapping = Object0064_GUNSoldier.EWeapon.GRENADE;
								break;
							case Object0068_GUNRobot.EWeapon.ROCKET4:
							case Object0068_GUNRobot.EWeapon.ROCKET8:
								originalWeaponTypeMapping = Object0064_GUNSoldier.EWeapon.MISSILE;
								break;
						}

						newEntry.WeaponType = ignoreDonorProperties ? originalWeaponTypeMapping : donor.WeaponType;
						newEntry.HaveShield = ignoreDonorProperties ? (ENoYes)originalEnemy.BodyType : donor.HaveShield;

						newEntry.Pos0_WaitType = (Object0064_GUNSoldier.EWaitType)originalEnemy.Pos0_ActionType;
						newEntry.Pos0_WaitSec = originalEnemy.Pos0_ActionTime;
						newEntry.Pos0_MoveSpeedRatio = originalEnemy.Pos0_MoveSpeedRatio;
						newEntry.Pos0_TranslationXFromOrigin = originalEnemy.Pos0_TranslationXFromOrigin;
						newEntry.Pos0_TranslationZFromOrigin = originalEnemy.Pos0_TranslationZFromOrigin;

						newEntry.Pos1_WaitType = (Object0064_GUNSoldier.EWaitType)originalEnemy.Pos1_ActionType;
						newEntry.Pos1_WaitSec = originalEnemy.Pos1_ActionTime;
						newEntry.Pos1_MoveSpeedRatio = originalEnemy.Pos1_MoveSpeedRatio;
						newEntry.Pos1_TranslationXFromOrigin = originalEnemy.Pos1_TranslationXFromOrigin;
						newEntry.Pos1_TranslationZFromOrigin = originalEnemy.Pos1_TranslationZFromOrigin;

						newEntry.Pos2_WaitType = (Object0064_GUNSoldier.EWaitType)originalEnemy.Pos2_ActionType;
						newEntry.Pos2_WaitSec = originalEnemy.Pos2_ActionTime;
						newEntry.Pos2_MoveSpeedRatio = originalEnemy.Pos2_MoveSpeedRatio;
						setData[index] = newEntry;
						break;
					}
				case Object0078_EggPierrot originalEnemy when targetEntry is Object0078_EggPierrot:
					{
						var newEntry = (Object0064_GUNSoldier)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						// EnemyBase
						newEntry.MoveRange = originalEnemy.MoveRange;
						newEntry.SearchRange = originalEnemy.SearchRange;
						newEntry.SearchAngle = originalEnemy.SearchAngle;
						newEntry.SearchWidth = originalEnemy.SearchWidth;
						newEntry.SearchHeight = originalEnemy.SearchHeight;
						newEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						newEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase
						newEntry.AppearType = Object0064_GUNSoldier.EAppear.RANDOM_MOVE;
						newEntry.WeaponType = donor.WeaponType;
						newEntry.HaveShield = donor.HaveShield;
						setData[index] = newEntry;
						break;
					}
				case Object0079_EggPawn originalEnemy when targetEntry is Object0079_EggPawn:
					{
						var newEntry = (Object0064_GUNSoldier)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						// EnemyBase
						newEntry.MoveRange = originalEnemy.MoveRange;
						newEntry.SearchRange = originalEnemy.SearchRange;
						newEntry.SearchAngle = originalEnemy.SearchAngle;
						newEntry.SearchWidth = originalEnemy.SearchWidth;
						newEntry.SearchHeight = originalEnemy.SearchHeight;
						newEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						newEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase

						switch (originalEnemy.AppearType)
						{
							case Object0079_EggPawn.EAppear.WAIT_ACT:
								newEntry.AppearType = Object0064_GUNSoldier.EAppear.TRIANGLE_MOVE;
								break;
							case Object0079_EggPawn.EAppear.OFFSET:
							case Object0079_EggPawn.EAppear.WARP:
								newEntry.AppearType = Object0064_GUNSoldier.EAppear.OFFSETPOS;
								newEntry.Pos0_TranslationXFromOrigin = originalEnemy.OffsetPos_X;
								newEntry.SearchHeightOffset = originalEnemy.OffsetPos_Y; // GUNSoldiers share their SearchHeightOffset property for Offset mode
								newEntry.Pos0_TranslationZFromOrigin = originalEnemy.OffsetPos_Z;
								break;
							case Object0079_EggPawn.EAppear.DASH:
								newEntry.AppearType = Object0064_GUNSoldier.EAppear.LINEAR_MOVE;
								newEntry.Pos0_TranslationXFromOrigin = originalEnemy.OffsetPos_X;
								newEntry.SearchHeightOffset = originalEnemy.OffsetPos_Y; // GUNSoldiers share their SearchHeightOffset property for Offset mode (DASH is basically offset mode)
								newEntry.Pos0_TranslationZFromOrigin = originalEnemy.OffsetPos_Z;
								break;
						}

						Object0064_GUNSoldier.EWeapon originalWeaponTypeMapping = 0;
						switch (originalEnemy.WeaponType)
						{
							case Object0079_EggPawn.EWeapon.NONE:
								originalWeaponTypeMapping = Object0064_GUNSoldier.EWeapon.NONE;
								break;
							case Object0079_EggPawn.EWeapon.EGG_PISTOL:
								originalWeaponTypeMapping = Object0064_GUNSoldier.EWeapon.GUN;
								break;
							case Object0079_EggPawn.EWeapon.BAZOOKA:
								originalWeaponTypeMapping = Object0064_GUNSoldier.EWeapon.GRENADE;
								break;
							case Object0079_EggPawn.EWeapon.LANCE:
								originalWeaponTypeMapping = Object0064_GUNSoldier.EWeapon.KNIFE;
								break;
						}

						newEntry.WeaponType = ignoreDonorProperties ? originalWeaponTypeMapping : donor.WeaponType;
						newEntry.HaveShield = ignoreDonorProperties ? originalEnemy.HaveShield : donor.HaveShield;

						newEntry.Pos0_WaitType = (Object0064_GUNSoldier.EWaitType)originalEnemy.Pos0_ActionType;
						newEntry.Pos0_WaitSec = originalEnemy.Pos0_ActionTime;
						newEntry.Pos0_MoveSpeedRatio = originalEnemy.Pos0_MoveSpeedRatio;
						newEntry.Pos0_TranslationXFromOrigin = originalEnemy.Pos0_TranslationXFromOrigin;
						newEntry.Pos0_TranslationZFromOrigin = originalEnemy.Pos0_TranslationZFromOrigin;

						newEntry.Pos1_WaitType = (Object0064_GUNSoldier.EWaitType)originalEnemy.Pos1_ActionType;
						newEntry.Pos1_WaitSec = originalEnemy.Pos1_ActionTime;
						newEntry.Pos1_MoveSpeedRatio = originalEnemy.Pos1_MoveSpeedRatio;
						newEntry.Pos1_TranslationXFromOrigin = originalEnemy.Pos1_TranslationXFromOrigin;
						newEntry.Pos1_TranslationZFromOrigin = originalEnemy.Pos1_TranslationZFromOrigin;

						newEntry.Pos2_WaitType = (Object0064_GUNSoldier.EWaitType)originalEnemy.Pos2_ActionType;
						newEntry.Pos2_WaitSec = originalEnemy.Pos2_ActionTime;
						newEntry.Pos2_MoveSpeedRatio = originalEnemy.Pos2_MoveSpeedRatio;
						setData[index] = newEntry;
						break;
					}
				case Object007A_EggShadowAndroid originalEnemy when targetEntry is Object007A_EggShadowAndroid:
					{
						var newEntry = (Object0064_GUNSoldier)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						// EnemyBase
						newEntry.MoveRange = originalEnemy.MoveRange;
						newEntry.SearchRange = originalEnemy.SearchRange;
						newEntry.SearchAngle = originalEnemy.SearchAngle;
						newEntry.SearchWidth = originalEnemy.SearchWidth;
						newEntry.SearchHeight = originalEnemy.SearchHeight;
						newEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						newEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase
						newEntry.AppearType = originalEnemy.AppearType == Object007A_EggShadowAndroid.EAppear.OFFSET ? Object0064_GUNSoldier.EAppear.OFFSETPOS : Object0064_GUNSoldier.EAppear.RANDOM_MOVE;
						if (originalEnemy.AppearType == Object007A_EggShadowAndroid.EAppear.OFFSET)
						{
							newEntry.Pos0_TranslationXFromOrigin = originalEnemy.OffsetPos_X;
							newEntry.SearchHeightOffset = originalEnemy.OffsetPos_Y; // GUNSoldiers share their SearchHeightOffset property for Offset mode
							newEntry.Pos0_TranslationZFromOrigin = originalEnemy.OffsetPos_Z;
						}
						newEntry.WeaponType = donor.WeaponType;
						newEntry.HaveShield = donor.HaveShield;
						setData[index] = newEntry;
						break;
					}
				case Object008C_BkGiant originalEnemy when targetEntry is Object008C_BkGiant:
					{
						var newEntry = (Object0064_GUNSoldier)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						// EnemyBase
						newEntry.MoveRange = originalEnemy.MoveRange;
						newEntry.SearchRange = originalEnemy.SearchRange;
						newEntry.SearchAngle = originalEnemy.SearchAngle;
						newEntry.SearchWidth = originalEnemy.SearchWidth;
						newEntry.SearchHeight = originalEnemy.SearchHeight;
						newEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						newEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase

						switch (originalEnemy.AppearType)
						{
							case Object008C_BkGiant.EAppear.WAIT:
								newEntry.AppearType = Object0064_GUNSoldier.EAppear.STAND;
								break;
							case Object008C_BkGiant.EAppear.DROP:
								newEntry.AppearType = Object0064_GUNSoldier.EAppear.OFFSETPOS;
								// GUNSoldiers share their SearchHeightOffset property for Offset mode
								newEntry.SearchHeightOffset = originalEnemy.OffsetPos_Y;
								break;
						}

						Object0064_GUNSoldier.EWeapon originalWeaponTypeMapping = 0;
						switch (originalEnemy.WeaponType)
						{
							case Object008C_BkGiant.EWeapon.BLACK_SWORD:
								originalWeaponTypeMapping = Object0064_GUNSoldier.EWeapon.NONE;
								break;
							case Object008C_BkGiant.EWeapon.DARK_HAMMER:
								originalWeaponTypeMapping = Object0064_GUNSoldier.EWeapon.KNIFE;
								break;
							case Object008C_BkGiant.EWeapon.BIG_BARREL:
								originalWeaponTypeMapping = Object0064_GUNSoldier.EWeapon.MISSILE;
								break;
						}

						newEntry.WeaponType = ignoreDonorProperties ? originalWeaponTypeMapping : donor.WeaponType;
						newEntry.HaveShield = ignoreDonorProperties ? originalEnemy.CanBlockShots : donor.HaveShield;
						setData[index] = newEntry;
						break;
					}
				case Object008D_BkSoldier originalEnemy when targetEntry is Object008D_BkSoldier:
					{
						var newEntry = (Object0064_GUNSoldier)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						// EnemyBase
						newEntry.MoveRange = originalEnemy.MoveRange;
						newEntry.SearchRange = originalEnemy.SearchRange;
						newEntry.SearchAngle = originalEnemy.SearchAngle;
						newEntry.SearchWidth = originalEnemy.SearchWidth;
						newEntry.SearchHeight = originalEnemy.SearchHeight;
						newEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						newEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase

						Object0064_GUNSoldier.EWeapon originalWeaponTypeMapping = 0;
						switch (originalEnemy.WeaponType)
						{
							case Object008D_BkSoldier.EWeapon.NONE:
								originalWeaponTypeMapping = Object0064_GUNSoldier.EWeapon.NONE;
								break;
							case Object008D_BkSoldier.EWeapon.BLACK_SWORD:
								originalWeaponTypeMapping = Object0064_GUNSoldier.EWeapon.KNIFE;
								break;
							case Object008D_BkSoldier.EWeapon.LIGHT_SHOT:
								originalWeaponTypeMapping = Object0064_GUNSoldier.EWeapon.GUN;
								break;
							case Object008D_BkSoldier.EWeapon.FLASH_SHOT:
								originalWeaponTypeMapping = Object0064_GUNSoldier.EWeapon.MACHINEGUN;
								break;
							case Object008D_BkSoldier.EWeapon.BLACK_BARREL:
								originalWeaponTypeMapping = Object0064_GUNSoldier.EWeapon.GRENADE;
								break;
							case Object008D_BkSoldier.EWeapon.SPLITTER:
							case Object008D_BkSoldier.EWeapon.VACUUM_POD:
								originalWeaponTypeMapping = Object0064_GUNSoldier.EWeapon.MISSILE;
								break;
							case Object008D_BkSoldier.EWeapon.HEAVY_SHOT:
							case Object008D_BkSoldier.EWeapon.RING_SHOT:
								originalWeaponTypeMapping = Object0064_GUNSoldier.EWeapon.RIFLE;
								break;
						}

						newEntry.AppearType = (Object0064_GUNSoldier.EAppear)originalEnemy.AppearType; // TODO: Future - if gun soldiers being dead are breaking things, then this is the cause; exclude WARP appearType in that case
						newEntry.WeaponType = ignoreDonorProperties ? originalWeaponTypeMapping : donor.WeaponType;
						newEntry.HaveShield = ignoreDonorProperties ? originalEnemy.HaveShield : donor.HaveShield;

						newEntry.Pos0_WaitType = (Object0064_GUNSoldier.EWaitType)originalEnemy.Pos0_WaitType + 1; // +1 because GUNSoldiers have an extra RADIO state as the first element
						newEntry.Pos0_WaitSec = originalEnemy.Pos0_WaitSec;
						newEntry.Pos0_MoveSpeedRatio = originalEnemy.Pos0_MoveSpeedRatio;
						newEntry.Pos0_TranslationXFromOrigin = originalEnemy.Pos0_TranslationXFromOrigin;
						newEntry.Pos0_TranslationZFromOrigin = originalEnemy.Pos0_TranslationZFromOrigin;

						newEntry.Pos1_WaitType = (Object0064_GUNSoldier.EWaitType)originalEnemy.Pos1_WaitType + 1; // +1 because GUNSoldiers have an extra RADIO state as the first element
						newEntry.Pos1_WaitSec = originalEnemy.Pos1_WaitSec;
						newEntry.Pos1_MoveSpeedRatio = originalEnemy.Pos1_MoveSpeedRatio;
						newEntry.Pos1_TranslationXFromOrigin = originalEnemy.Pos1_TranslationXFromOrigin;
						newEntry.Pos1_TranslationZFromOrigin = originalEnemy.Pos1_TranslationZFromOrigin;

						newEntry.Pos2_WaitType = (Object0064_GUNSoldier.EWaitType)originalEnemy.Pos2_WaitType + 1; // +1 because GUNSoldiers have an extra RADIO state as the first element
						newEntry.Pos2_WaitSec = originalEnemy.Pos2_WaitSec;
						newEntry.Pos2_MoveSpeedRatio = originalEnemy.Pos2_MoveSpeedRatio;
						setData[index] = newEntry;
						break;
					}
				case Object008E_BkWingLarge originalEnemy when targetEntry is Object008E_BkWingLarge:
					{
						if (originalEnemy.PathType == Object008E_BkWingLarge.EPathType.FLY_FORWARD_UPDOWN
							|| originalEnemy.PathType == Object008E_BkWingLarge.EPathType.FLY_FORWARD_SWOOP
							|| originalEnemy.PathType == Object008E_BkWingLarge.EPathType.FLY_FORWARD
							|| originalEnemy.PathType == Object008E_BkWingLarge.EPathType.FLY_LEFT)
							break; // We cannot safely convert FLY_* PathType to GroundEnemy, since these are usually way off screen | Probably drop this, should be the responsibility of the caller?
						var newEntry = (Object0064_GUNSoldier)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						// EnemyBase
						newEntry.MoveRange = originalEnemy.MoveRange;
						newEntry.SearchRange = originalEnemy.SearchRange;
						newEntry.SearchAngle = originalEnemy.SearchAngle;
						newEntry.SearchWidth = originalEnemy.SearchWidth;
						newEntry.SearchHeight = originalEnemy.SearchHeight;
						newEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset; // may want to not set this for FlyingEnemy -> GroundEnemy
						newEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase
						newEntry.AppearType = Object0064_GUNSoldier.EAppear.RANDOM_MOVE; // there is no good translation for pos0,1,2 for FlyingEnemy -> GroundEnemy; always set to random move type instead
						newEntry.WeaponType = donor.WeaponType;
						newEntry.HaveShield = donor.HaveShield;
						setData[index] = newEntry;
						break;
					}
				case Object008F_BkWingSmall originalEnemy when targetEntry is Object008F_BkWingSmall:
					{
						if (originalEnemy.PathType == Object008F_BkWingSmall.EPathType.FLY_FORWARD_UPDOWN
							|| originalEnemy.PathType == Object008F_BkWingSmall.EPathType.FLY_FORWARD_SWOOP
							|| originalEnemy.PathType == Object008F_BkWingSmall.EPathType.FLY_FORWARD
							|| originalEnemy.PathType == Object008F_BkWingSmall.EPathType.FLY_LEFT)
							break; // We cannot safely convert FLY_* PathType to GroundEnemy, since these are usually way off screen | Probably drop this, should be the responsibility of the caller?
						var newEntry = (Object0064_GUNSoldier)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						// EnemyBase
						newEntry.MoveRange = originalEnemy.MoveRange;
						newEntry.SearchRange = originalEnemy.SearchRange;
						newEntry.SearchAngle = originalEnemy.SearchAngle;
						newEntry.SearchWidth = originalEnemy.SearchWidth;
						newEntry.SearchHeight = originalEnemy.SearchHeight;
						newEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset; // may want to not set this for FlyingEnemy -> GroundEnemy
						newEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase
						newEntry.AppearType = Object0064_GUNSoldier.EAppear.RANDOM_MOVE; // there is no good translation for pos0,1,2 for FlyingEnemy -> GroundEnemy; always set to random move type instead
						newEntry.WeaponType = donor.WeaponType;
						newEntry.HaveShield = donor.HaveShield;
						setData[index] = newEntry;
						break;
					}
				case Object0090_BkWorm originalEnemy when targetEntry is Object0090_BkWorm:
					{
						var newEntry = (Object0064_GUNSoldier)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						// EnemyBase
						newEntry.MoveRange = originalEnemy.MoveRange;
						newEntry.SearchRange = originalEnemy.SearchRange;
						newEntry.SearchAngle = originalEnemy.SearchAngle;
						newEntry.SearchWidth = originalEnemy.SearchWidth;
						newEntry.SearchHeight = originalEnemy.SearchHeight;
						newEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						newEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase
						newEntry.AppearType = Object0064_GUNSoldier.EAppear.RANDOM_MOVE;
						newEntry.WeaponType = donor.WeaponType;
						newEntry.HaveShield = donor.HaveShield;
						setData[index] = newEntry;
						break;
					}
				case Object0091_BkLarva originalEnemy when targetEntry is Object0091_BkLarva:
					{
						var newEntry = (Object0064_GUNSoldier)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						// EnemyBase
						newEntry.MoveRange = originalEnemy.MoveRange;
						newEntry.SearchRange = originalEnemy.SearchRange;
						newEntry.SearchAngle = originalEnemy.SearchAngle;
						newEntry.SearchWidth = originalEnemy.SearchWidth;
						newEntry.SearchHeight = originalEnemy.SearchHeight;
						newEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						newEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase
						newEntry.AppearType = Object0064_GUNSoldier.EAppear.RANDOM_MOVE;
						newEntry.WeaponType = donor.WeaponType;
						newEntry.HaveShield = donor.HaveShield;
						setData[index] = newEntry;
						break;
					}
				case Object0092_BkChaos originalEnemy when targetEntry is Object0092_BkChaos:
					{
						var newEntry = (Object0064_GUNSoldier)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						// EnemyBase
						newEntry.MoveRange = originalEnemy.MoveRange;
						newEntry.SearchRange = originalEnemy.SearchRange;
						newEntry.SearchAngle = originalEnemy.SearchAngle;
						newEntry.SearchWidth = originalEnemy.SearchWidth;
						newEntry.SearchHeight = originalEnemy.SearchHeight;
						newEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						newEntry.MoveSpeedRatio = 1f; // BkChaos does not have moveSpeed, instead has HP value
													  // end EnemyBase
						newEntry.AppearType = Object0064_GUNSoldier.EAppear.RANDOM_MOVE;
						newEntry.WeaponType = donor.WeaponType;
						newEntry.HaveShield = donor.HaveShield;
						setData[index] = newEntry;
						break;
					}
				case Object0093_BkNinja originalEnemy when targetEntry is Object0093_BkNinja:
					{
						var newEntry = (Object0064_GUNSoldier)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						// EnemyBase
						newEntry.MoveRange = originalEnemy.MoveRange;
						newEntry.SearchRange = originalEnemy.SearchRange;
						newEntry.SearchAngle = originalEnemy.SearchAngle;
						newEntry.SearchWidth = originalEnemy.SearchWidth;
						newEntry.SearchHeight = originalEnemy.SearchHeight;
						newEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						newEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase
						newEntry.AppearType = Object0064_GUNSoldier.EAppear.RANDOM_MOVE;
						newEntry.WeaponType = donor.WeaponType;
						newEntry.HaveShield = donor.HaveShield;
						setData[index] = newEntry;
						break;
					}
				default:
					{
						var newEntry = (Object0064_GUNSoldier)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
								targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
						// EnemyBase
						newEntry.MoveRange = 1000;
						newEntry.SearchRange = 1000;
						newEntry.SearchAngle = 0;
						newEntry.SearchWidth = 400;
						newEntry.SearchHeight = 400;
						newEntry.SearchHeightOffset = 0;
						newEntry.MoveSpeedRatio = 1;
						// end EnemyBase
						newEntry.AppearType = Object0064_GUNSoldier.EAppear.RANDOM_MOVE;
						newEntry.WeaponType = donor.WeaponType;
						newEntry.HaveShield = donor.HaveShield;
						setData[index] = newEntry;
						break;
					}
			}
		}

		private static void ToGUNBeetle(int index, Object0065_GUNBeetle donor, ref List<SetObjectShadow> setData, Random r, bool ignoreDonorProperties = false)
		{
			var targetEntry = setData[index];

			switch (targetEntry)
			{
				case Object0064_GUNSoldier originalEnemy when targetEntry is Object0064_GUNSoldier:
				default:
					var newEntry = (Object0065_GUNBeetle)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
					targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
					// EnemyBase
					newEntry.MoveRange = 1000;
					newEntry.SearchRange = 1000;
					newEntry.SearchAngle = 0;
					newEntry.SearchWidth = 400;
					newEntry.SearchHeight = 400;
					newEntry.SearchHeightOffset = 0;
					newEntry.MoveSpeedRatio = 1;
					// end EnemyBase
					newEntry.AppearType = Object0065_GUNBeetle.EAppear.WAIT_FLOATING;
					newEntry.WeaponType = donor.WeaponType;
					newEntry.PathType = Object0065_GUNBeetle.EPathType.UPDOWN;
					setData[index] = newEntry;
					break;
			}
		}

		private static void ToGUNBigfoot(int index, Object0066_GUNBigfoot donor, ref List<SetObjectShadow> setData, Random r, bool ignoreDonorProperties = false)
		{
			var targetEntry = setData[index];

			switch (targetEntry)
			{
				case Object0064_GUNSoldier originalEnemy when targetEntry is Object0064_GUNSoldier:
				default:
					var newEntry = (Object0066_GUNBigfoot)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
					targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
					// EnemyBase
					newEntry.MoveRange = 1000;
					newEntry.SearchRange = 1000;
					newEntry.SearchAngle = 0;
					newEntry.SearchWidth = 400;
					newEntry.SearchHeight = 400;
					newEntry.SearchHeightOffset = 0;
					newEntry.MoveSpeedRatio = 1;
					// end EnemyBase
					newEntry.AppearType = Object0066_GUNBigfoot.EAppear.HOVERING;
					newEntry.WeaponType = donor.WeaponType;
					setData[index] = newEntry;
					break;
			}
		}

		private static void ToGUNRobot(int index, Object0068_GUNRobot donor, ref List<SetObjectShadow> setData, Random r, bool ignoreDonorProperties = false)
		{
			var targetEntry = setData[index];

			switch (targetEntry)
			{
				case Object0064_GUNSoldier originalEnemy when targetEntry is Object0064_GUNSoldier:
				default:
					var newEntry = (Object0068_GUNRobot)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
					targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
					// EnemyBase
					newEntry.MoveRange = 1000;
					newEntry.SearchRange = 1000;
					newEntry.SearchAngle = 0;
					newEntry.SearchWidth = 400;
					newEntry.SearchHeight = 400;
					newEntry.SearchHeightOffset = 0;
					newEntry.MoveSpeedRatio = 1;
					// end EnemyBase
					newEntry.AppearType = Object0068_GUNRobot.EAppear.WAIT_ACT;
					newEntry.WeaponType = donor.WeaponType;
					setData[index] = newEntry;
					break;
			}
		}

		private static void ToEggPierrot(int index, Object0078_EggPierrot donor, ref List<SetObjectShadow> setData, Random r, bool ignoreDonorProperties = false)
		{
			var targetEntry = setData[index];

			switch (targetEntry)
			{
				case Object0064_GUNSoldier originalEnemy when targetEntry is Object0064_GUNSoldier:
				default:
					var newEntry = (Object0078_EggPierrot)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
					targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
					// EnemyBase
					newEntry.MoveRange = 1000;
					newEntry.SearchRange = 1000;
					newEntry.SearchAngle = 0;
					newEntry.SearchWidth = 400;
					newEntry.SearchHeight = 400;
					newEntry.SearchHeightOffset = 0;
					newEntry.MoveSpeedRatio = 1;
					// end EnemyBase
					newEntry.AppearType = Object0078_EggPierrot.EAppear.WANDER;
					setData[index] = newEntry;
					break;
			}
		}

		private static void ToEggPawn(int index, Object0079_EggPawn donor, ref List<SetObjectShadow> setData, Random r, bool ignoreDonorProperties = false)
		{
			var targetEntry = setData[index];

			switch (targetEntry)
			{
				case Object0064_GUNSoldier originalEnemy when targetEntry is Object0064_GUNSoldier:
				default:
					var newEntry = (Object0079_EggPawn)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
					targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
					// EnemyBase
					newEntry.MoveRange = 1000;
					newEntry.SearchRange = 1000;
					newEntry.SearchAngle = 0;
					newEntry.SearchWidth = 400;
					newEntry.SearchHeight = 400;
					newEntry.SearchHeightOffset = 0;
					newEntry.MoveSpeedRatio = 1;
					// end EnemyBase
					newEntry.AppearType = Object0079_EggPawn.EAppear.WAIT_ACT;
					newEntry.WeaponType = donor.WeaponType;
					setData[index] = newEntry;
					break;
			}
		}

		private static void ToEggShadowAndroid(int index, Object007A_EggShadowAndroid donor, ref List<SetObjectShadow> setData, Random r, bool ignoreDonorProperties = false)
		{
			var targetEntry = setData[index];

			switch (targetEntry)
			{
				case Object0064_GUNSoldier originalEnemy when targetEntry is Object0064_GUNSoldier:
				default:
					var newEntry = (Object007A_EggShadowAndroid)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
					targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
					// EnemyBase
					newEntry.MoveRange = 1000;
					newEntry.SearchRange = 1000;
					newEntry.SearchAngle = 0;
					newEntry.SearchWidth = 400;
					newEntry.SearchHeight = 400;
					newEntry.SearchHeightOffset = 0;
					newEntry.MoveSpeedRatio = 1;
					// end EnemyBase
					newEntry.AppearType = Object007A_EggShadowAndroid.EAppear.STAND;
					setData[index] = newEntry;
					break;
			}
		}

		private static void ToBkGiant(int index, Object008C_BkGiant donor, ref List<SetObjectShadow> setData, Random r, bool ignoreDonorProperties = false)
		{
			var targetEntry = setData[index];

			switch (targetEntry)
			{
				case Object0064_GUNSoldier originalEnemy when targetEntry is Object0064_GUNSoldier:
				default:
					var newEntry = (Object008C_BkGiant)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
					targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
					// EnemyBase
					newEntry.MoveRange = 1000;
					newEntry.SearchRange = 1000;
					newEntry.SearchAngle = 0;
					newEntry.SearchWidth = 400;
					newEntry.SearchHeight = 400;
					newEntry.SearchHeightOffset = 0;
					newEntry.MoveSpeedRatio = 1;
					// end EnemyBase
					newEntry.AppearType = Object008C_BkGiant.EAppear.WAIT;
					newEntry.WeaponType = donor.WeaponType;
					setData[index] = newEntry;
					break;
			}
		}

		private static void ToBkSoldier(int index, Object008D_BkSoldier donor, ref List<SetObjectShadow> setData, Random r, bool ignoreDonorProperties = false)
		{
			var targetEntry = setData[index];

			switch (targetEntry)
			{
				case Object0064_GUNSoldier originalEnemy when targetEntry is Object0064_GUNSoldier:
				default:
					var newEntry = (Object008D_BkSoldier)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
					targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
					// EnemyBase
					newEntry.MoveRange = 1000;
					newEntry.SearchRange = 1000;
					newEntry.SearchAngle = 0;
					newEntry.SearchWidth = 400;
					newEntry.SearchHeight = 400;
					newEntry.SearchHeightOffset = 0;
					newEntry.MoveSpeedRatio = 1;
					// end EnemyBase
					newEntry.AppearType = Object008D_BkSoldier.EAppear.RANDOM_MOVE;
					newEntry.WeaponType = donor.WeaponType;
					setData[index] = newEntry;
					break;
			}
		}

		private static void ToBkWingLarge(int index, Object008E_BkWingLarge donor, ref List<SetObjectShadow> setData, Random r, bool ignoreDonorProperties = false)
		{
			var targetEntry = setData[index];

			switch (targetEntry)
			{
				case Object0064_GUNSoldier originalEnemy when targetEntry is Object0064_GUNSoldier:
				default:
					var newEntry = (Object008E_BkWingLarge)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
					targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
					// EnemyBase
					newEntry.MoveRange = 1000;
					newEntry.SearchRange = 1000;
					newEntry.SearchAngle = 0;
					newEntry.SearchWidth = 400;
					newEntry.SearchHeight = 400;
					newEntry.SearchHeightOffset = 0;
					newEntry.MoveSpeedRatio = 1;
					// end EnemyBase
					newEntry.AppearType = Object008E_BkWingLarge.EAppear.WAIT_FLOATING;
					newEntry.PathType = Object008E_BkWingLarge.EPathType.UPDOWN;
					setData[index] = newEntry;
					break;
			}
		}

		private static void ToBkWingSmall(int index, Object008F_BkWingSmall donor, ref List<SetObjectShadow> setData, Random r, bool ignoreDonorProperties = false)
		{
			var targetEntry = setData[index];

			switch (targetEntry)
			{
				case Object0064_GUNSoldier originalEnemy when targetEntry is Object0064_GUNSoldier:
				default:
					var newEntry = (Object008F_BkWingSmall)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
					targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
					// EnemyBase
					newEntry.MoveRange = 1000;
					newEntry.SearchRange = 1000;
					newEntry.SearchAngle = 0;
					newEntry.SearchWidth = 400;
					newEntry.SearchHeight = 400;
					newEntry.SearchHeightOffset = 0;
					newEntry.MoveSpeedRatio = 1;
					// end EnemyBase
					newEntry.AppearType = Object008F_BkWingSmall.EAppear.WAIT_FLOATING;
					newEntry.PathType = Object008F_BkWingSmall.EPathType.UPDOWN;
					setData[index] = newEntry;
					break;
			}
		}

		private static void ToBkWorm(int index, Object0090_BkWorm donor, ref List<SetObjectShadow> setData, Random r, bool ignoreDonorProperties = false)
		{
			var targetEntry = setData[index];

			switch (targetEntry)
			{
				case Object0064_GUNSoldier originalEnemy when targetEntry is Object0064_GUNSoldier:
				default:
					var newEntry = (Object0090_BkWorm)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
					targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
					// EnemyBase
					newEntry.MoveRange = 1000;
					newEntry.SearchRange = 1000;
					newEntry.SearchAngle = 0;
					newEntry.SearchWidth = 400;
					newEntry.SearchHeight = 400;
					newEntry.SearchHeightOffset = 0;
					newEntry.MoveSpeedRatio = 1;
					// end EnemyBase
					newEntry.WormType = donor.WormType;
					setData[index] = newEntry;
					break;
			}
		}

		private static void ToBkLarva(int index, Object0091_BkLarva donor, ref List<SetObjectShadow> setData, Random r, bool ignoreDonorProperties = false)
		{
			var targetEntry = setData[index];

			switch (targetEntry)
			{
				case Object0064_GUNSoldier originalEnemy when targetEntry is Object0064_GUNSoldier:
				default:
					var newEntry = (Object0091_BkLarva)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
					targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
					// EnemyBase
					newEntry.MoveRange = 1000;
					newEntry.SearchRange = 1000;
					newEntry.SearchAngle = 0;
					newEntry.SearchWidth = 400;
					newEntry.SearchHeight = 400;
					newEntry.SearchHeightOffset = 0;
					newEntry.MoveSpeedRatio = 1;
					// end EnemyBase
					newEntry.AppearType = Object0091_BkLarva.EAppear.NORMAL;
					newEntry.NumberOfLarva = donor.NumberOfLarva;
					setData[index] = newEntry;
					break;
			}
		}

		private static void ToBkChaos(int index, Object0092_BkChaos donor, ref List<SetObjectShadow> setData, Random r, bool ignoreDonorProperties = false)
		{
			var targetEntry = setData[index];

			switch (targetEntry)
			{
				case Object0064_GUNSoldier originalEnemy when targetEntry is Object0064_GUNSoldier:
				default:
					var newEntry = (Object0092_BkChaos)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
					targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
					// EnemyBase
					newEntry.MoveRange = 1000;
					newEntry.SearchRange = 1000;
					newEntry.SearchAngle = 0;
					newEntry.SearchWidth = 400;
					newEntry.SearchHeight = 400;
					newEntry.SearchHeightOffset = 0;
					newEntry.Health = 15;
					// end EnemyBase
					newEntry.StartingState = Object0092_BkChaos.EState.Complete;
					newEntry.NumberOfChibi = 5;
					newEntry.BrokenPieceFlyDistance = 1;
					newEntry.CombineTime = 1;
					newEntry.CombineStartTime = 1;
					setData[index] = newEntry;
					break;
			}
		}

		private static void ToBkNinja(int index, Object0093_BkNinja donor, ref List<SetObjectShadow> setData, Random r, bool ignoreDonorProperties = false)
		{
			var targetEntry = setData[index];

			switch (targetEntry)
			{
				case Object0064_GUNSoldier originalEnemy when targetEntry is Object0064_GUNSoldier:
				default:
					var newEntry = (Object0093_BkNinja)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
					targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
					// EnemyBase
					newEntry.MoveRange = 1000;
					newEntry.SearchRange = 1000;
					newEntry.SearchAngle = 0;
					newEntry.SearchWidth = 400;
					newEntry.SearchHeight = 400;
					newEntry.SearchHeightOffset = 0;
					newEntry.MoveSpeedRatio = 1;
					// end EnemyBase
					newEntry.AppearType = Object0093_BkNinja.EAppear.ON_AIR_SAUCER_WARP;
					newEntry.ShootCount = donor.ShootCount;
					newEntry.AttackInterval = donor.AttackInterval;
					newEntry.WaitInterval = donor.WaitInterval;
					setData[index] = newEntry;
					break;
			}
		}

		private static void ToGUNSoldier(int index, ref List<SetObjectShadow> setData, Random r)
		{
			var donor = new Object0064_GUNSoldier
			{
				List = 0x00,
				Type = 0x64,
				WeaponType = (Object0064_GUNSoldier.EWeapon)r.Next(0, 7),
				HaveShield = r.Next(10) == 1 ? (ENoYes)1 : (ENoYes)0 // 10% chance of shield
			};
			ToGUNSoldier(index, donor, ref setData, r, true);
		}
	}
}

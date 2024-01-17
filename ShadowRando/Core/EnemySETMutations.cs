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
			}
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
			}
		}

		private static void ToGUNSoldier(int index, Object0064_GUNSoldier donor, ref List<SetObjectShadow> setData, Random r, bool ignoreDonorProperties = false)
		{
			var targetEntry = setData[index];
			switch (targetEntry)
			{
				case Object0064_GUNSoldier originalEnemy when targetEntry is Object0064_GUNSoldier:
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
						newEntry.Pos0_WaitSec = donor.Pos0_WaitSec;
						newEntry.Pos0_WaitType = donor.Pos0_WaitType;
						newEntry.Pos0_MoveSpeedRatio = donor.Pos0_MoveSpeedRatio;
						newEntry.Pos0_TranslationXFromOrigin = donor.Pos0_TranslationXFromOrigin;
						newEntry.Pos0_TranslationZFromOrigin = donor.Pos0_TranslationZFromOrigin;
						newEntry.Pos1_WaitType = donor.Pos1_WaitType;
						newEntry.Pos1_WaitSec = donor.Pos1_WaitSec;
						newEntry.Pos1_MoveSpeedRatio = donor.Pos1_MoveSpeedRatio;
						newEntry.Pos1_TranslationXFromOrigin = donor.Pos1_TranslationXFromOrigin;
						newEntry.Pos1_TranslationZFromOrigin = donor.Pos1_TranslationZFromOrigin;
						newEntry.Pos2_WaitType = donor.Pos2_WaitType;
						newEntry.Pos2_WaitSec = donor.Pos2_WaitSec;
						newEntry.Pos2_MoveSpeedRatio = donor.Pos2_MoveSpeedRatio;
						setData[index] = newEntry;
						break;
					}
				case Object0065_GUNBeetle originalEnemy when targetEntry is Object0065_GUNBeetle:
					{
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
							case Object0068_GUNRobot.EAppear.XXX:
								newEntry.AppearType = Object0064_GUNSoldier.EAppear.OFFSETPOS;
								newEntry.Pos0_TranslationXFromOrigin = originalEnemy.OffsetPos_X;
								newEntry.SearchHeightOffset = originalEnemy.OffsetPos_Y; // GUNSoldiers share their SearchHeightOffset property for Offset mode
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
						newEntry.MoveRange = donor.MoveRange;
						newEntry.SearchRange = donor.SearchRange;
						newEntry.SearchAngle = donor.SearchAngle;
						newEntry.SearchWidth = donor.SearchWidth;
						newEntry.SearchHeight = donor.SearchHeight;
						newEntry.SearchHeightOffset = donor.SearchHeightOffset;
						newEntry.MoveSpeedRatio = donor.MoveSpeedRatio;
						// end EnemyBase
						newEntry.AppearType = donor.AppearType;
						newEntry.WeaponType = donor.WeaponType;
						newEntry.HaveShield = donor.HaveShield;
						newEntry.Pos0_WaitSec = donor.Pos0_WaitSec;
						newEntry.Pos0_WaitType = donor.Pos0_WaitType;
						newEntry.Pos0_MoveSpeedRatio = donor.Pos0_MoveSpeedRatio;
						newEntry.Pos0_TranslationXFromOrigin = donor.Pos0_TranslationXFromOrigin;
						newEntry.Pos0_TranslationZFromOrigin = donor.Pos0_TranslationZFromOrigin;
						newEntry.Pos1_WaitSec = donor.Pos1_WaitSec;
						newEntry.Pos1_WaitType = donor.Pos1_WaitType;
						newEntry.Pos1_MoveSpeedRatio = donor.Pos1_MoveSpeedRatio;
						newEntry.Pos1_TranslationXFromOrigin = donor.Pos1_TranslationXFromOrigin;
						newEntry.Pos1_TranslationZFromOrigin = donor.Pos1_TranslationZFromOrigin;
						newEntry.Pos2_WaitSec = donor.Pos2_WaitSec;
						newEntry.Pos2_WaitType = donor.Pos2_WaitType;
						newEntry.Pos2_MoveSpeedRatio = donor.Pos2_MoveSpeedRatio;
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
				case Object0065_GUNBeetle originalEnemy when targetEntry is Object0065_GUNBeetle:
					{
						if (originalEnemy.AppearType == Object0065_GUNBeetle.EAppear.MOVE_ON_PATH)
							break;
						goto default; // fall to default case if non-path 
					}
				case Object008E_BkWingLarge originalEnemy when targetEntry is Object008E_BkWingLarge:
					{
						var actionType = Object0065_GUNBeetle.EAction.NONE;
						if (originalEnemy.ActionType == Object008E_BkWingLarge.EAction.AIR_CUTTER)
							actionType = r.Next(2) == 1 ? Object0065_GUNBeetle.EAction.USE_WEAPON : Object0065_GUNBeetle.EAction.SHOCK;

						var newEntry = (Object0065_GUNBeetle)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
							targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
						newEntry.MoveRange = originalEnemy.MoveRange;
						newEntry.SearchRange = originalEnemy.SearchRange;
						newEntry.SearchAngle = originalEnemy.SearchAngle;
						newEntry.SearchWidth = originalEnemy.SearchWidth;
						newEntry.SearchHeight = originalEnemy.SearchHeight;
						newEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						newEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						newEntry.AppearType = (Object0065_GUNBeetle.EAppear)originalEnemy.AppearType;
						newEntry.ActionType = actionType;
						newEntry.WeaponType = (Object0065_GUNBeetle.EWeapon)r.Next(6);
						newEntry.PathType = (Object0065_GUNBeetle.EPathType)originalEnemy.PathType;
						newEntry.PathVariable = originalEnemy.PathVariable;
						newEntry.AttackStart = originalEnemy.AttackStart;
						newEntry.AttackEnd = originalEnemy.AttackEnd;
						newEntry.SparkWait = originalEnemy.AttackStart;
						newEntry.SparkDischarge = originalEnemy.AttackEnd;
						newEntry.PatrolReversed = originalEnemy.PatrolReversed;
						newEntry.IsGolden = r.Next(20) == 1 ? (ENoYes)1 : (ENoYes)0; // 5% chance of golden
						setData[index] = newEntry;
						break;
					}
				case Object008F_BkWingSmall originalEnemy when targetEntry is Object008F_BkWingSmall:
					{
						var actionType = Object0065_GUNBeetle.EAction.NONE;
						if (originalEnemy.ActionType == Object008F_BkWingSmall.EAction.ATTACK)
							actionType = r.Next(2) == 1 ? Object0065_GUNBeetle.EAction.USE_WEAPON : Object0065_GUNBeetle.EAction.SHOCK;

						var newEntry = (Object0065_GUNBeetle)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
							targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
						newEntry.MoveRange = originalEnemy.MoveRange;
						newEntry.SearchRange = originalEnemy.SearchRange;
						newEntry.SearchAngle = originalEnemy.SearchAngle;
						newEntry.SearchWidth = originalEnemy.SearchWidth;
						newEntry.SearchHeight = originalEnemy.SearchHeight;
						newEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						newEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						newEntry.AppearType = (Object0065_GUNBeetle.EAppear)originalEnemy.AppearType;
						newEntry.ActionType = actionType;
						newEntry.WeaponType = (Object0065_GUNBeetle.EWeapon)r.Next(6);
						newEntry.PathType = (Object0065_GUNBeetle.EPathType)originalEnemy.PathType;
						newEntry.PathVariable = originalEnemy.PathVariable;
						newEntry.AttackStart = originalEnemy.AttackStart;
						newEntry.AttackEnd = originalEnemy.AttackEnd;
						newEntry.SparkWait = originalEnemy.AttackStart;
						newEntry.SparkDischarge = originalEnemy.AttackEnd;
						newEntry.PatrolReversed = originalEnemy.PatrolReversed;
						newEntry.IsGolden = r.Next(20) == 1 ? (ENoYes)1 : (ENoYes)0; // 5% chance of golden
						setData[index] = newEntry;
						break;
					}
				case Object0064_GUNSoldier originalEnemy when targetEntry is Object0064_GUNSoldier:
				default:
					{
						var newEntry = (Object0065_GUNBeetle)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
						targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
						// EnemyBase
						newEntry.MoveRange = donor.MoveRange;
						newEntry.SearchRange = donor.SearchRange;
						newEntry.SearchAngle = donor.SearchAngle;
						newEntry.SearchWidth = donor.SearchWidth;
						newEntry.SearchHeight = donor.SearchHeight;
						newEntry.SearchHeightOffset = donor.SearchHeightOffset;
						newEntry.MoveSpeedRatio = donor.MoveSpeedRatio;
						// end EnemyBase
						newEntry.AppearType = donor.AppearType;
						newEntry.ActionType = donor.ActionType;
						newEntry.PathType = donor.PathType;
						newEntry.PathVariable = donor.PathVariable;
						newEntry.AttackStart = donor.AttackStart;
						newEntry.AttackEnd = donor.AttackEnd;
						newEntry.PatrolReversed = donor.PatrolReversed;
						newEntry.IsGolden = donor.IsGolden;
						newEntry.WeaponType = donor.WeaponType;
						newEntry.SparkDischarge = donor.SparkDischarge;
						newEntry.SparkWait = donor.SparkWait;
						setData[index] = newEntry;
						break;
					}
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
					newEntry.MoveRange = donor.MoveRange;
					newEntry.SearchRange = donor.SearchRange;
					newEntry.SearchAngle = donor.SearchAngle;
					newEntry.SearchWidth = donor.SearchWidth;
					newEntry.SearchHeight = donor.SearchHeight;
					newEntry.SearchHeightOffset = donor.SearchHeightOffset;
					newEntry.MoveSpeedRatio = donor.MoveSpeedRatio;
					// end EnemyBase
					newEntry.AppearType = donor.AppearType;
					newEntry.WeaponType = donor.WeaponType;
					donor.OffsetPos_Y = donor.OffsetPos_Y;
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
					newEntry.MoveRange = donor.MoveRange;
					newEntry.SearchRange = donor.SearchRange;
					newEntry.SearchAngle = donor.SearchAngle;
					newEntry.SearchWidth = donor.SearchWidth;
					newEntry.SearchHeight = donor.SearchHeight;
					newEntry.SearchHeightOffset = donor.SearchHeightOffset;
					newEntry.MoveSpeedRatio = donor.MoveSpeedRatio;
					// end EnemyBase
					newEntry.AppearType = donor.AppearType;
					newEntry.WeaponType = donor.WeaponType;
					newEntry.BodyType = donor.BodyType;
					newEntry.OffsetPos_X = donor.OffsetPos_X;
					newEntry.OffsetPos_Y = donor.OffsetPos_Y;
					newEntry.OffsetPos_Z = donor.OffsetPos_Z;
					newEntry.WaitActMoveType = donor.WaitActMoveType;
					newEntry.Pos0_ActionTime = donor.Pos0_ActionTime;
					newEntry.Pos0_ActionType = donor.Pos0_ActionType;
					newEntry.Pos0_MoveSpeedRatio = donor.Pos0_MoveSpeedRatio;
					newEntry.Pos0_TranslationXFromOrigin = donor.Pos0_TranslationXFromOrigin;
					newEntry.Pos0_TranslationZFromOrigin = donor.Pos0_TranslationZFromOrigin;
					newEntry.Pos1_ActionTime = donor.Pos1_ActionTime;
					newEntry.Pos1_ActionType = donor.Pos1_ActionType;
					newEntry.Pos1_MoveSpeedRatio = donor.Pos1_MoveSpeedRatio;
					newEntry.Pos1_TranslationXFromOrigin = donor.Pos1_TranslationXFromOrigin;
					newEntry.Pos1_TranslationZFromOrigin = donor.Pos1_TranslationZFromOrigin;
					newEntry.Pos2_ActionTime = donor.Pos2_ActionTime;
					newEntry.Pos2_ActionType = donor.Pos2_ActionType;
					newEntry.Pos2_MoveSpeedRatio = donor.Pos2_MoveSpeedRatio;
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
					newEntry.MoveRange = donor.MoveRange;
					newEntry.SearchRange = donor.SearchRange;
					newEntry.SearchAngle = donor.SearchAngle;
					newEntry.SearchWidth = donor.SearchWidth;
					newEntry.SearchHeight = donor.SearchHeight;
					newEntry.SearchHeightOffset = donor.SearchHeightOffset;
					newEntry.MoveSpeedRatio = donor.MoveSpeedRatio;
					// end EnemyBase
					newEntry.AppearType = donor.AppearType;
					newEntry.UnknownInt = donor.UnknownInt;
					newEntry.BombTime = donor.BombTime;
					newEntry.OffsetPos_X = donor.OffsetPos_X;
					newEntry.OffsetPos_Y = donor.OffsetPos_Y;
					newEntry.OffsetPos_Z = donor.OffsetPos_Z;
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
					newEntry.MoveRange = donor.MoveRange;
					newEntry.SearchRange = donor.SearchRange;
					newEntry.SearchAngle = donor.SearchAngle;
					newEntry.SearchWidth = donor.SearchWidth;
					newEntry.SearchHeight = donor.SearchHeight;
					newEntry.SearchHeightOffset = donor.SearchHeightOffset;
					newEntry.MoveSpeedRatio = donor.MoveSpeedRatio;
					// end EnemyBase
					newEntry.AppearType = donor.AppearType;
					newEntry.WeaponType = donor.WeaponType;
					newEntry.OffsetPos_X = donor.OffsetPos_X;
					newEntry.OffsetPos_Y = donor.OffsetPos_Y;
					newEntry.OffsetPos_Z = donor.OffsetPos_Z;
					newEntry.WaitActMoveType = donor.WaitActMoveType;
					newEntry.Pos0_ActionTime = donor.Pos0_ActionTime;
					newEntry.Pos0_ActionType = donor.Pos0_ActionType;
					newEntry.Pos0_MoveSpeedRatio = donor.Pos0_MoveSpeedRatio;
					newEntry.Pos0_TranslationXFromOrigin = donor.Pos0_TranslationXFromOrigin;
					newEntry.Pos0_TranslationZFromOrigin = donor.Pos0_TranslationZFromOrigin;
					newEntry.Pos1_ActionTime = donor.Pos1_ActionTime;
					newEntry.Pos1_ActionType = donor.Pos1_ActionType;
					newEntry.Pos1_MoveSpeedRatio = donor.Pos1_MoveSpeedRatio;
					newEntry.Pos1_TranslationXFromOrigin = donor.Pos1_TranslationXFromOrigin;
					newEntry.Pos1_TranslationZFromOrigin = donor.Pos1_TranslationZFromOrigin;
					newEntry.Pos2_ActionTime = donor.Pos2_ActionTime;
					newEntry.Pos2_ActionType = donor.Pos2_ActionType;
					newEntry.Pos2_MoveSpeedRatio = donor.Pos2_MoveSpeedRatio;
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
					newEntry.MoveRange = donor.MoveRange;
					newEntry.SearchRange = donor.SearchRange;
					newEntry.SearchAngle = donor.SearchAngle;
					newEntry.SearchWidth = donor.SearchWidth;
					newEntry.SearchHeight = donor.SearchHeight;
					newEntry.SearchHeightOffset = donor.SearchHeightOffset;
					newEntry.MoveSpeedRatio = donor.MoveSpeedRatio;
					// end EnemyBase
					newEntry.AppearType = donor.AppearType;
					newEntry.OffsetPos_X = donor.OffsetPos_X;
					newEntry.OffsetPos_Y = donor.OffsetPos_Y;
					newEntry.OffsetPos_Z = donor.OffsetPos_Z;
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
					newEntry.MoveRange = donor.MoveRange;
					newEntry.SearchRange = donor.SearchRange;
					newEntry.SearchAngle = donor.SearchAngle;
					newEntry.SearchWidth = donor.SearchWidth;
					newEntry.SearchHeight = donor.SearchHeight;
					newEntry.SearchHeightOffset = donor.SearchHeightOffset;
					newEntry.MoveSpeedRatio = donor.MoveSpeedRatio;
					// end EnemyBase
					newEntry.AppearType = donor.AppearType;
					newEntry.WeaponType = donor.WeaponType;
					newEntry.OffsetPos_Y = donor.OffsetPos_Y;
					newEntry.CanBlockShots = donor.CanBlockShots;
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
					newEntry.MoveRange = donor.MoveRange;
					newEntry.SearchRange = donor.SearchRange;
					newEntry.SearchAngle = donor.SearchAngle;
					newEntry.SearchWidth = donor.SearchWidth;
					newEntry.SearchHeight = donor.SearchHeight;
					newEntry.SearchHeightOffset = donor.SearchHeightOffset;
					newEntry.MoveSpeedRatio = donor.MoveSpeedRatio;
					// end EnemyBase
					newEntry.AppearType = donor.AppearType;
					newEntry.WeaponType = donor.WeaponType;
					newEntry.HaveShield = donor.HaveShield;
					newEntry.Pos0_WaitSec = donor.Pos0_WaitSec;
					newEntry.Pos0_WaitType = donor.Pos0_WaitType;
					newEntry.Pos0_MoveSpeedRatio = donor.Pos0_MoveSpeedRatio;
					newEntry.Pos0_TranslationXFromOrigin = donor.Pos0_TranslationXFromOrigin;
					newEntry.Pos0_TranslationZFromOrigin = donor.Pos0_TranslationZFromOrigin;
					newEntry.Pos1_WaitSec = donor.Pos1_WaitSec;
					newEntry.Pos1_WaitType = donor.Pos1_WaitType;
					newEntry.Pos1_MoveSpeedRatio = donor.Pos1_MoveSpeedRatio;
					newEntry.Pos1_TranslationXFromOrigin = donor.Pos1_TranslationXFromOrigin;
					newEntry.Pos1_TranslationZFromOrigin = donor.Pos1_TranslationZFromOrigin;
					newEntry.Pos2_WaitSec = donor.Pos2_WaitSec;
					newEntry.Pos2_WaitType = donor.Pos2_WaitType;
					newEntry.Pos2_MoveSpeedRatio = donor.Pos2_MoveSpeedRatio;
					newEntry.IsOnAirSaucer = donor.IsOnAirSaucer;
					setData[index] = newEntry;
					break;
			}
		}

		private static void ToBkWingLarge(int index, Object008E_BkWingLarge donor, ref List<SetObjectShadow> setData, Random r, bool ignoreDonorProperties = false)
		{
			var targetEntry = setData[index];

			switch (targetEntry)
			{
				case Object0065_GUNBeetle originalEnemy when targetEntry is Object0065_GUNBeetle:
					{
						// account for BlackVolt non standard enum
						var bodyType = r.Next(4);
						if (bodyType == 2)
							bodyType = 16;
						if (bodyType == 3)
							bodyType = 17;

						var newEntry = (Object008E_BkWingLarge)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
							targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
						newEntry.MoveRange = originalEnemy.MoveRange;
						newEntry.SearchRange = originalEnemy.SearchRange;
						newEntry.SearchAngle = originalEnemy.SearchAngle;
						newEntry.SearchWidth = originalEnemy.SearchWidth;
						newEntry.SearchHeight = originalEnemy.SearchHeight;
						newEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						newEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						newEntry.AppearType = (Object008E_BkWingLarge.EAppear)originalEnemy.AppearType;
						newEntry.ActionType = (originalEnemy.ActionType == Object0065_GUNBeetle.EAction.SHOCK || originalEnemy.ActionType == Object0065_GUNBeetle.EAction.USE_WEAPON) ? Object008E_BkWingLarge.EAction.AIR_CUTTER : Object008E_BkWingLarge.EAction.NONE;
						newEntry.PathType = (Object008E_BkWingLarge.EPathType)originalEnemy.PathType;
						newEntry.PathVariable = originalEnemy.PathVariable;
						newEntry.AttackStart = originalEnemy.AttackStart;
						newEntry.AttackEnd = originalEnemy.AttackEnd;
						newEntry.PatrolReversed = originalEnemy.PatrolReversed;
						newEntry.BodyAndDeathType = (Object008E_BkWingLarge.EBodyAndDeathType)bodyType;
						setData[index] = newEntry;
						break;
					}
				case Object008E_BkWingLarge originalEnemy when targetEntry is Object008E_BkWingLarge:
					{
						if (originalEnemy.AppearType == Object008E_BkWingLarge.EAppear.MOVE_ON_PATH)
							break;
						goto default; // fall to default case if non-path
					}
				case Object008F_BkWingSmall originalEnemy when targetEntry is Object008F_BkWingSmall:
					{
						// account for BlackVolt non standard enum
						var bodyType = r.Next(4);
						if (bodyType == 2)
							bodyType = 16;
						if (bodyType == 3)
							bodyType = 17;

						var newEntry = (Object008E_BkWingLarge)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
							targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
						newEntry.MoveRange = originalEnemy.MoveRange;
						newEntry.SearchRange = originalEnemy.SearchRange;
						newEntry.SearchAngle = originalEnemy.SearchAngle;
						newEntry.SearchWidth = originalEnemy.SearchWidth;
						newEntry.SearchHeight = originalEnemy.SearchHeight;
						newEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						newEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						newEntry.AppearType = (Object008E_BkWingLarge.EAppear)originalEnemy.AppearType;
						newEntry.ActionType = (Object008E_BkWingLarge.EAction)originalEnemy.ActionType;
						newEntry.PathType = (Object008E_BkWingLarge.EPathType)originalEnemy.PathType;
						newEntry.PathVariable = originalEnemy.PathVariable;
						newEntry.AttackStart = originalEnemy.AttackStart;
						newEntry.AttackEnd = originalEnemy.AttackEnd;
						newEntry.PatrolReversed = originalEnemy.PatrolReversed;
						newEntry.BodyAndDeathType = (Object008E_BkWingLarge.EBodyAndDeathType)bodyType;
						setData[index] = newEntry;
						break;
					}
				case Object0064_GUNSoldier originalEnemy when targetEntry is Object0064_GUNSoldier:
				default:
					{
						var newEntry = (Object008E_BkWingLarge)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
						targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
						// EnemyBase
						newEntry.MoveRange = donor.MoveRange;
						newEntry.SearchRange = donor.SearchRange;
						newEntry.SearchAngle = donor.SearchAngle;
						newEntry.SearchWidth = donor.SearchWidth;
						newEntry.SearchHeight = donor.SearchHeight;
						newEntry.SearchHeightOffset = donor.SearchHeightOffset;
						newEntry.MoveSpeedRatio = donor.MoveSpeedRatio;
						// end EnemyBase
						newEntry.AppearType = donor.AppearType;
						newEntry.ActionType = donor.ActionType;
						newEntry.PathType = donor.PathType;
						newEntry.PathVariable = donor.PathVariable;
						newEntry.AttackStart = donor.AttackStart;
						newEntry.AttackEnd = donor.AttackEnd;
						newEntry.PatrolReversed = donor.PatrolReversed;
						newEntry.BodyAndDeathType = donor.BodyAndDeathType;
						setData[index] = newEntry;
						break;
					}
			}
		}

		private static void ToBkWingSmall(int index, Object008F_BkWingSmall donor, ref List<SetObjectShadow> setData, Random r, bool ignoreDonorProperties = false)
		{
			var targetEntry = setData[index];

			switch (targetEntry)
			{
				case Object0065_GUNBeetle originalEnemy when targetEntry is Object0065_GUNBeetle:
					{
						var newEntry = (Object008F_BkWingSmall)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
							targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
						newEntry.MoveRange = originalEnemy.MoveRange;
						newEntry.SearchRange = originalEnemy.SearchRange;
						newEntry.SearchAngle = originalEnemy.SearchAngle;
						newEntry.SearchWidth = originalEnemy.SearchWidth;
						newEntry.SearchHeight = originalEnemy.SearchHeight;
						newEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						newEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						newEntry.AppearType = (Object008F_BkWingSmall.EAppear)originalEnemy.AppearType;
						newEntry.ActionType = (originalEnemy.ActionType == Object0065_GUNBeetle.EAction.SHOCK || originalEnemy.ActionType == Object0065_GUNBeetle.EAction.USE_WEAPON) ? Object008F_BkWingSmall.EAction.ATTACK : Object008F_BkWingSmall.EAction.NONE;
						newEntry.PathType = (Object008F_BkWingSmall.EPathType)originalEnemy.PathType;
						newEntry.PathVariable = originalEnemy.PathVariable;
						newEntry.AttackStart = originalEnemy.AttackStart;
						newEntry.AttackEnd = originalEnemy.AttackEnd;
						newEntry.PatrolReversed = originalEnemy.PatrolReversed;
						setData[index] = newEntry;
						break;
					}
				case Object008E_BkWingLarge originalEnemy when targetEntry is Object008E_BkWingLarge:
					{
						var newEntry = (Object008F_BkWingSmall)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
							targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
						newEntry.MoveRange = originalEnemy.MoveRange;
						newEntry.SearchRange = originalEnemy.SearchRange;
						newEntry.SearchAngle = originalEnemy.SearchAngle;
						newEntry.SearchWidth = originalEnemy.SearchWidth;
						newEntry.SearchHeight = originalEnemy.SearchHeight;
						newEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						newEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						newEntry.AppearType = (Object008F_BkWingSmall.EAppear)originalEnemy.AppearType;
						newEntry.ActionType = (Object008F_BkWingSmall.EAction)originalEnemy.ActionType;
						newEntry.PathType = (Object008F_BkWingSmall.EPathType)originalEnemy.PathType;
						newEntry.PathVariable = originalEnemy.PathVariable;
						newEntry.AttackStart = originalEnemy.AttackStart;
						newEntry.AttackEnd = originalEnemy.AttackEnd;
						newEntry.PatrolReversed = originalEnemy.PatrolReversed;
						setData[index] = newEntry;
						break;
					}
				case Object008F_BkWingSmall originalEnemy when targetEntry is Object008F_BkWingSmall:
					{
						if (originalEnemy.AppearType == Object008F_BkWingSmall.EAppear.MOVE_ON_PATH)
							break;
						goto default; // fall to default case if non-path
					}
				case Object0064_GUNSoldier originalEnemy when targetEntry is Object0064_GUNSoldier:
				default:
					{
						var newEntry = (Object008F_BkWingSmall)LayoutEditorFunctions.CreateShadowObject(donor.List, donor.Type, targetEntry.PosX, targetEntry.PosY,
						targetEntry.PosZ, targetEntry.RotX, targetEntry.RotY, targetEntry.RotZ, targetEntry.Link, targetEntry.Rend, targetEntry.UnkBytes);
						newEntry.MoveRange = donor.MoveRange;
						newEntry.SearchRange = donor.SearchRange;
						newEntry.SearchAngle = donor.SearchAngle;
						newEntry.SearchWidth = donor.SearchWidth;
						newEntry.SearchHeight = donor.SearchHeight;
						newEntry.SearchHeightOffset = donor.SearchHeightOffset;
						newEntry.MoveSpeedRatio = donor.MoveSpeedRatio;
						// end EnemyBase
						newEntry.AppearType = donor.AppearType;
						newEntry.ActionType = donor.ActionType;
						newEntry.PathType = donor.PathType;
						newEntry.PathVariable = donor.PathVariable;
						newEntry.AttackStart = donor.AttackStart;
						newEntry.AttackEnd = donor.AttackEnd;
						newEntry.PatrolReversed = donor.PatrolReversed;
						setData[index] = newEntry;
						break;
					}
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
					newEntry.MoveRange = donor.MoveRange;
					newEntry.SearchRange = donor.SearchRange;
					newEntry.SearchAngle = donor.SearchAngle;
					newEntry.SearchWidth = donor.SearchWidth;
					newEntry.SearchHeight = donor.SearchHeight;
					newEntry.SearchHeightOffset = donor.SearchHeightOffset;
					newEntry.MoveSpeedRatio = donor.MoveSpeedRatio;
					// end EnemyBase
					newEntry.WormType = donor.WormType;
					newEntry.AttackCount = donor.AttackCount;
					newEntry.AppearDelay = donor.AppearDelay;
					newEntry.Unknown1 = donor.Unknown1;
					newEntry.Unknown2 = donor.Unknown2;
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
					newEntry.MoveRange = donor.MoveRange;
					newEntry.SearchRange = donor.SearchRange;
					newEntry.SearchAngle = donor.SearchAngle;
					newEntry.SearchWidth = donor.SearchWidth;
					newEntry.SearchHeight = donor.SearchHeight;
					newEntry.SearchHeightOffset = donor.SearchHeightOffset;
					newEntry.MoveSpeedRatio = donor.MoveSpeedRatio;
					// end EnemyBase
					newEntry.NumberOfLarva = donor.NumberOfLarva;
					newEntry.AppearRange = donor.AppearRange;
					newEntry.AppearType = donor.AppearType;
					newEntry.Offset_Y = donor.Offset_Y;
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
					newEntry.MoveRange = donor.MoveRange;
					newEntry.SearchRange = donor.SearchRange;
					newEntry.SearchAngle = donor.SearchAngle;
					newEntry.SearchWidth = donor.SearchWidth;
					newEntry.SearchHeight = donor.SearchHeight;
					newEntry.SearchHeightOffset = donor.SearchHeightOffset;
					newEntry.Health = donor.Health;
					// end EnemyBase
					newEntry.StartingState = donor.StartingState;
					newEntry.NumberOfChibi = donor.NumberOfChibi;
					newEntry.BrokenPieceFlyDistance = donor.BrokenPieceFlyDistance;
					newEntry.BrokenPieceFlyOffset = donor.BrokenPieceFlyOffset;
					newEntry.CombineStartTime = donor.CombineStartTime;
					newEntry.CombineTime = donor.CombineTime;
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
					newEntry.MoveRange = donor.MoveRange;
					newEntry.SearchRange = donor.SearchRange;
					newEntry.SearchAngle = donor.SearchAngle;
					newEntry.SearchWidth = donor.SearchWidth;
					newEntry.SearchHeight = donor.SearchHeight;
					newEntry.SearchHeightOffset = donor.SearchHeightOffset;
					newEntry.MoveSpeedRatio = donor.MoveSpeedRatio;
					// end EnemyBase
					newEntry.AppearType = donor.AppearType;
					newEntry.ShootCount = donor.ShootCount;
					newEntry.AttackInterval = donor.AttackInterval;
					newEntry.WaitInterval = donor.WaitInterval;
					newEntry.Pos0_X = donor.Pos0_X;
					newEntry.Pos0_Y = donor.Pos0_Y;
					newEntry.Pos0_Z = donor.Pos0_Z;
					// add unused later maybe, if adding customs
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
				MoveRange = 200,
				SearchRange = 60,
				SearchWidth = 100,
				SearchHeight = 100,
				SearchHeightOffset = 0,
				MoveSpeedRatio = 2,
				HaveShield = r.Next(10) == 1 ? (ENoYes)1 : (ENoYes)0, // 10% chance of shield
				WeaponType = (Object0064_GUNSoldier.EWeapon)r.Next(7),
				AppearType = Object0064_GUNSoldier.EAppear.RANDOM_MOVE,
				Pos0_WaitType = Object0064_GUNSoldier.EWaitType.ATTACK,
				Pos0_WaitSec = 2,
				Pos0_MoveSpeedRatio = 2,
				Pos0_TranslationXFromOrigin = 40,
				Pos0_TranslationZFromOrigin = 60,
				Pos1_WaitType = Object0064_GUNSoldier.EWaitType.ATTACK,
				Pos1_WaitSec = 33,
				Pos1_MoveSpeedRatio = 2,
				Pos1_TranslationXFromOrigin = 0,
				Pos1_TranslationZFromOrigin = 0,
				Pos2_WaitType = Object0064_GUNSoldier.EWaitType.RADIO_CONTACT,
				Pos2_WaitSec = 1,
				Pos2_MoveSpeedRatio = 1
			};
			ToGUNSoldier(index, donor, ref setData, r, true);
		}
		private static void ToGUNBeetle(int index, ref List<SetObjectShadow> setData, Random r)
		{
			var weapon = (Object0065_GUNBeetle.EWeapon)r.Next(6);

			var donor = new Object0065_GUNBeetle
			{
				List = 0x00,
				Type = 0x65,
				MoveRange = 80,
				SearchRange = 65,
				SearchAngle = 0,
				SearchWidth = 200,
				SearchHeight = 115,
				SearchHeightOffset = 0,
				MoveSpeedRatio = 1,
				AppearType = Object0065_GUNBeetle.EAppear.WAIT_FLOATING,
				WeaponType = weapon,
				ActionType = weapon == Object0065_GUNBeetle.EWeapon.NONE ? Object0065_GUNBeetle.EAction.SHOCK : Object0065_GUNBeetle.EAction.USE_WEAPON,
				PathType = (Object0065_GUNBeetle.EPathType)r.Next(4),
				PathVariable = r.Next(10, 41),
				AttackStart = 0.4f,
				AttackEnd = 0.6f,
				PatrolReversed = (ENoYes)r.Next(2),
				IsGolden = r.Next(20) == 1 ? (ENoYes)1 : (ENoYes)0, // 5% chance of golden
				SparkDischarge = 2f,
				SparkWait = 2f,
			};
			ToGUNBeetle(index, donor, ref setData, r, true);
		}
		private static void ToGUNBigfoot(int index, ref List<SetObjectShadow> setData, Random r)
		{
			var donor = new Object0066_GUNBigfoot
			{
				List = 0x00,
				Type = 0x66,
				MoveRange = 200, // EnemyBase
				SearchRange = 200,
				SearchAngle = 0,
				SearchWidth = 400,
				SearchHeight = 400,
				SearchHeightOffset = 0,
				MoveSpeedRatio = 1, // end EnemyBase
				AppearType = (Object0066_GUNBigfoot.EAppear)r.Next(5),
				WeaponType = (Object0066_GUNBigfoot.EWeapon)r.Next(2),
				OffsetPos_Y = 50
			};
			ToGUNBigfoot(index, donor, ref setData, r, true);
		}
		private static void ToGUNRobot(int index, ref List<SetObjectShadow> setData, Random r)
		{
			var donor = new Object0068_GUNRobot
			{
				List = 0x00,
				Type = 0x68,
				MoveRange = 200, // EnemyBase
				SearchRange = 200,
				SearchAngle = 0,
				SearchWidth = 400,
				SearchHeight = 400,
				SearchHeightOffset = 0,
				MoveSpeedRatio = 1, // end EnemyBase
				AppearType = (Object0068_GUNRobot.EAppear)r.Next(4),
				WeaponType = (Object0068_GUNRobot.EWeapon)r.Next(6),
				BodyType = (Object0068_GUNRobot.EBody)r.Next(2),
				OffsetPos_X = 0,
				OffsetPos_Y = 50,
				OffsetPos_Z = 0,
				WaitActMoveType = EWaitActMove.Random,
				Pos0_ActionTime = 3,
				Pos0_ActionType = EAction.Attack,
				Pos0_MoveSpeedRatio = 1,
				Pos0_TranslationXFromOrigin = 0,
				Pos0_TranslationZFromOrigin = 0,
				Pos1_ActionTime = 40,
				Pos1_ActionType = EAction.None,
				Pos1_MoveSpeedRatio = 1,
				Pos1_TranslationXFromOrigin = 0,
				Pos1_TranslationZFromOrigin = 0,
				Pos2_ActionTime = 3,
				Pos2_ActionType = EAction.None,
				Pos2_MoveSpeedRatio = 1
			};
			ToGUNRobot(index, donor, ref setData, r, true);
		}
		private static void ToEggPierrot(int index, ref List<SetObjectShadow> setData, Random r)
		{
			var donor = new Object0078_EggPierrot
			{
				List = 0x00,
				Type = 0x78,
				MoveRange = 200, // EnemyBase
				SearchRange = 0,
				SearchAngle = 0,
				SearchWidth = 200,
				SearchHeight = 200,
				SearchHeightOffset = 0,
				MoveSpeedRatio = 1, // end EnemyBase
				AppearType = (Object0078_EggPierrot.EAppear)r.Next(2),
				UnknownInt = 0,
				BombTime = r.Next(4, 9),
				OffsetPos_X = 0,
				OffsetPos_Y = 50,
				OffsetPos_Z = 0
			};
			ToEggPierrot(index, donor, ref setData, r, true);
		}
		private static void ToEggPawn(int index, ref List<SetObjectShadow> setData, Random r)
		{
			var donor = new Object0079_EggPawn
			{
				List = 0x00,
				Type = 0x79,
				MoveRange = 200, // EnemyBase
				SearchRange = 200,
				SearchAngle = 0,
				SearchWidth = 400,
				SearchHeight = 400,
				SearchHeightOffset = 0,
				MoveSpeedRatio = 1, // end EnemyBase
				HaveShield = r.Next(10) == 1 ? (ENoYes)1 : (ENoYes)0, // 10% chance of shield
				AppearType = (Object0079_EggPawn.EAppear)r.Next(4),
				WeaponType = (Object0079_EggPawn.EWeapon)r.Next(4),
				OffsetPos_X = 0,
				OffsetPos_Y = 50,
				OffsetPos_Z = 0,
				WaitActMoveType = EWaitActMove.Random,
				Pos0_ActionTime = 3,
				Pos0_ActionType = EAction.Attack,
				Pos0_MoveSpeedRatio = 1,
				Pos0_TranslationXFromOrigin = 0,
				Pos0_TranslationZFromOrigin = 0,
				Pos1_ActionTime = 40,
				Pos1_ActionType = EAction.None,
				Pos1_MoveSpeedRatio = 1,
				Pos1_TranslationXFromOrigin = 0,
				Pos1_TranslationZFromOrigin = 0,
				Pos2_ActionTime = 3,
				Pos2_ActionType = EAction.None,
				Pos2_MoveSpeedRatio = 1
			};
			ToEggPawn(index, donor, ref setData, r, true);
		}
		private static void ToEggShadowAndroid(int index, ref List<SetObjectShadow> setData, Random r)
		{
			var donor = new Object007A_EggShadowAndroid
			{
				List = 0x00,
				Type = 0x7A,
				MoveRange = 200, // EnemyBase
				SearchRange = 0,
				SearchAngle = 0,
				SearchWidth = 200,
				SearchHeight = 200,
				SearchHeightOffset = 0,
				MoveSpeedRatio = 5, // end EnemyBase
				AppearType = (Object007A_EggShadowAndroid.EAppear)r.Next(2),
				OffsetPos_X = 0,
				OffsetPos_Y = 50,
				OffsetPos_Z = 0
			};
			ToEggShadowAndroid(index, donor, ref setData, r, true);
		}
		private static void ToBkGiant(int index, ref List<SetObjectShadow> setData, Random r)
		{
			var donor = new Object008C_BkGiant
			{
				List = 0x00,
				Type = 0x8C,
				MoveRange = 200, // EnemyBase
				SearchRange = 0,
				SearchAngle = 0,
				SearchWidth = 200,
				SearchHeight = 115,
				SearchHeightOffset = -40,
				MoveSpeedRatio = 1, // end EnemyBase
				AppearType = (Object008C_BkGiant.EAppear)r.Next(2),
				WeaponType = (Object008C_BkGiant.EWeapon)r.Next(3),
				OffsetPos_Y = 30,
				CanBlockShots = r.Next(10) == 1 ? (ENoYes)1 : (ENoYes)0, // 10% chance of shield
			};
			ToBkGiant(index, donor, ref setData, r, true);
		}
		private static void ToBkSoldier(int index, ref List<SetObjectShadow> setData, Random r)
		{
			var donor = new Object008D_BkSoldier
			{
				List = 0x00,
				Type = 0x8D,
				MoveRange = 200,
				SearchRange = 60,
				SearchWidth = 100,
				SearchHeight = 100,
				SearchHeightOffset = 0,
				MoveSpeedRatio = 2,
				HaveShield = r.Next(10) == 1 ? (ENoYes)1 : (ENoYes)0, // 10% chance of shield
				WeaponType = (Object008D_BkSoldier.EWeapon)r.Next(9),
				AppearType = Object008D_BkSoldier.EAppear.RANDOM_MOVE,
				Pos0_WaitType = Object008D_BkSoldier.EWaitType.ATTACK,
				Pos0_WaitSec = 2,
				Pos0_MoveSpeedRatio = 2,
				Pos0_TranslationXFromOrigin = 40,
				Pos0_TranslationZFromOrigin = 60,
				Pos1_WaitType = Object008D_BkSoldier.EWaitType.ATTACK,
				Pos1_WaitSec = 33,
				Pos1_MoveSpeedRatio = 2,
				Pos1_TranslationXFromOrigin = 0,
				Pos1_TranslationZFromOrigin = 0,
				Pos2_WaitType = Object008D_BkSoldier.EWaitType.KAMAE,
				Pos2_WaitSec = 1,
				Pos2_MoveSpeedRatio = 1,
				IsOnAirSaucer = r.Next(10) == 1 ? (Object008D_BkSoldier.EBkSoldierNoYes)1 : (Object008D_BkSoldier.EBkSoldierNoYes)0, // 10% chance of saucer
			};
			ToBkSoldier(index, donor, ref setData, r, true);
		}
		private static void ToBkWingLarge(int index, ref List<SetObjectShadow> setData, Random r)
		{
			// account for BlackVolt non standard enum
			var bodyType = r.Next(4);
			if (bodyType == 2) bodyType = 16;
			if (bodyType == 3) bodyType = 17;

			var donor = new Object008E_BkWingLarge
			{
				List = 0x00,
				Type = 0x8E,
				MoveRange = 80,
				SearchRange = 65,
				SearchAngle = 0,
				SearchWidth = 200,
				SearchHeight = 115,
				SearchHeightOffset = 0,
				MoveSpeedRatio = 0.001f,
				AppearType = Object008E_BkWingLarge.EAppear.WAIT_FLOATING,
				ActionType = Object008E_BkWingLarge.EAction.AIR_CUTTER,
				PathType = (Object008E_BkWingLarge.EPathType)r.Next(4),
				PathVariable = r.Next(10, 41),
				AttackStart = 0.4f,
				AttackEnd = 0.6f,
				PatrolReversed = (ENoYes)r.Next(2),
				BodyAndDeathType = (Object008E_BkWingLarge.EBodyAndDeathType)bodyType
			};
			ToBkWingLarge(index, donor, ref setData, r, true);
		}
		private static void ToBkWingSmall(int index, ref List<SetObjectShadow> setData, Random r)
		{
			var donor = new Object008F_BkWingSmall
			{
				List = 0x00,
				Type = 0x8F,
				MoveRange = 200,
				SearchRange = 65,
				SearchAngle = 0,
				SearchWidth = 200,
				SearchHeight = 115,
				SearchHeightOffset = 0,
				MoveSpeedRatio = 1,
				AppearType = Object008F_BkWingSmall.EAppear.WAIT_FLOATING,
				ActionType = Object008F_BkWingSmall.EAction.ATTACK,
				PathType = (Object008F_BkWingSmall.EPathType)r.Next(4),
				PathVariable = r.Next(10, 41),
				AttackStart = 0.4f,
				AttackEnd = 0.6f,
				PatrolReversed = (ENoYes)r.Next(2),
			};
			ToBkWingSmall(index, donor, ref setData, r, true);
		}
		private static void ToBkWorm(int index, ref List<SetObjectShadow> setData, Random r)
		{
			var donor = new Object0090_BkWorm
			{
				List = 0x00,
				Type = 0x90,
				MoveRange = 80,
				SearchRange = 0,
				SearchAngle = 0,
				SearchWidth = 200,
				SearchHeight = 200,
				SearchHeightOffset = 0,
				MoveSpeedRatio = 1,
				WormType = (Object0090_BkWorm.EWormType)r.Next(3),
				AttackCount = r.Next(1, 5),
				AppearDelay = r.Next(3),
				Unknown1 = -1,
				Unknown2 = -1
			};
			ToBkWorm(index, donor, ref setData, r, true);
		}
		private static void ToBkLarva(int index, ref List<SetObjectShadow> setData, Random r)
		{
			var appearType = (Object0091_BkLarva.EAppear)r.Next(2);
			var donor = new Object0091_BkLarva
			{
				List = 0x00,
				Type = 0x91,
				MoveRange = 80,
				SearchRange = 0,
				SearchAngle = 0,
				SearchWidth = 150,
				SearchHeight = 50,
				SearchHeightOffset = 0,
				MoveSpeedRatio = 0.6f,
				NumberOfLarva = r.Next(1, 20),
				AppearRange = r.Next(10, 60),
				AppearType = appearType,
				Offset_Y = appearType == Object0091_BkLarva.EAppear.NORMAL ? 0 : 50
			};
			ToBkLarva(index, donor, ref setData, r, true);
		}
		private static void ToBkChaos(int index, ref List<SetObjectShadow> setData, Random r)
		{
			var donor = new Object0092_BkChaos
			{
				List = 0x00,
				Type = 0x92,
				MoveRange = 70,
				SearchRange = 90,
				SearchAngle = 0,
				SearchWidth = 150,
				SearchHeight = 150,
				SearchHeightOffset = 0,
				Health = r.Next(101),
				StartingState = (Object0092_BkChaos.EState)r.Next(2),
				NumberOfChibi = r.Next(4, 30),
				BrokenPieceFlyDistance = r.Next(10, 51),
				BrokenPieceFlyOffset = 0,
				CombineStartTime = 1, //r.Next(2),  //  TODO: Inspect why it explodes the game? Probably if float = 0
				CombineTime = 1 //r.Next(2)  //  TODO: Inspect why it explodes the game; Probably if float = 0
			};
			ToBkChaos(index, donor, ref setData, r, true);
		}
		private static void ToBkNinja(int index, ref List<SetObjectShadow> setData, Random r)
		{
			var appearType = (Object0093_BkNinja.EAppear)r.Next(4);
			var donor = new Object0093_BkNinja
			{
				List = 0x00,
				Type = 0x93,
				MoveRange = 120,
				SearchRange = 0,
				SearchAngle = 0,
				SearchWidth = 200,
				SearchHeight = 100,
				SearchHeightOffset = 0,
				MoveSpeedRatio = 1,
				AppearType = appearType,
				ShootCount = r.Next(1, 5),
				AttackInterval = 1,
				WaitInterval = 1,
				Pos0_X = 0,
				Pos0_Y = appearType == Object0093_BkNinja.EAppear.ON_AIR_SAUCER_WARP ? 0 : 50,
				Pos0_Z = 0,
				UNUSED_Pos0_IntWaitType = 0,
				UNUSED_Pos0_DisappearTime = 0,
				UNUSED_Pos1_X = 0,
				UNUSED_Pos1_Y = 0,
				UNUSED_Pos1_Z = 0,
				UNUSED_Pos1_WaitType = 0,
				UNUSED_Pos1_DisappearTime = 0,
				UNUSED_Float21 = 0,
				UNUSED_Float22 = 0
			};
			ToBkNinja(index, donor, ref setData, r, true);
		}
	}
}

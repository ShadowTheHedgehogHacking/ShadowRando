using ShadowSET;
using System.Collections.Generic;
using System;
using System.Data.Odbc;

namespace ShadowRando
{
	internal class EnemySETMutations
	{
		public void MutateObjectAtIndex(int index, SetObjectShadow conversionObject, ref List<SetObjectShadow> setData, bool isShadow, Random r)
		{
			// do this for non instance method version as well
			/*public void ProcessObjectType(Type objectType)
			{
				if (objectType == typeof(SubClass1))
*/
			// isShadow ?
			/*			if (conversionObject is Object0064_GUNSoldier)
						{
							// Handle SubClass1
							Object0064_GUNSoldier subObj = (Object0064_GUNSoldier)conversionObject; // If needed, cast to SubClass1
						}*/

			var targetEntry = setData[index];


			switch (conversionObject)
			{
				case Object0064_GUNSoldier enemyTarget when conversionObject is Object0064_GUNSoldier:
					ToGunSoldier(index, enemyTarget, ref setData, r);
					break;
				case Object0065_GUNBeetle enemyTarget when conversionObject is Object0065_GUNBeetle:
					break;
				case Object0066_GUNBigfoot enemyTarget when conversionObject is Object0066_GUNBigfoot:
					break;
				case Object0068_GUNRobot enemyTarget when conversionObject is Object0068_GUNRobot:
					break;
				case Object0078_EggPierrot enemyTarget when conversionObject is Object0078_EggPierrot:
					break;
				case Object0079_EggPawn enemyTarget when conversionObject is Object0079_EggPawn:
					break;
				case Object007A_EggShadowAndroid enemyTarget when conversionObject is Object007A_EggShadowAndroid:
					break;
				case Object008C_BkGiant enemyTarget when conversionObject is Object008C_BkGiant:
					break;
				case Object008D_BkSoldier enemyTarget when conversionObject is Object008D_BkSoldier:
					break;
				case Object008E_BkWingLarge enemyTarget when conversionObject is Object008E_BkWingLarge:
					break;
				case Object008F_BkWingSmall enemyTarget when conversionObject is Object008F_BkWingSmall:
					break;
				case Object0090_BkWorm enemyTarget when conversionObject is Object0090_BkWorm:
					break;
				case Object0091_BkLarva enemyTarget when conversionObject is Object0091_BkLarva:
					break;
				case Object0092_BkChaos enemyTarget when conversionObject is Object0092_BkChaos:
					break;
				case Object0093_BkNinja enemyTarget when conversionObject is Object0093_BkNinja:
					break;
			}
		}

		private void ToGunSoldier(int index, Object0064_GUNSoldier gunSoldierDonor, ref List<SetObjectShadow> setData, Random r)
		{
			var targetEntry = setData[index];

			// if "Prevent Bad Link ID" is on...
			if (targetEntry.Link != 0 || targetEntry.Link != 50)
				return; // skip this object, unsafe to convert to GUN Soldier (TODO: should this be this method's responsibility? or the caller?)

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
							break; // We cannot safely convert FLY_* PathType to GroundEnemy, since these are usually way off screen
						var newEntry = LayoutEditorFunctions.CreateShadowObject(gunSoldierDonor.List, gunSoldierDonor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						var outputEntry = (Object0064_GUNSoldier)newEntry;
						// EnemyBase
						outputEntry.MoveRange = originalEnemy.MoveRange;
						outputEntry.SearchRange = originalEnemy.SearchRange;
						outputEntry.SearchAngle = originalEnemy.SearchAngle;
						outputEntry.SearchWidth = originalEnemy.SearchWidth;
						outputEntry.SearchHeight = originalEnemy.SearchHeight;
						outputEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset; // may want to not set this for FlyingEnemy -> GroundEnemy
						outputEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase
						outputEntry.AppearType = Object0064_GUNSoldier.EAppear.RANDOM_MOVE; // there is no good translation for pos0,1,2 for FlyingEnemy -> GroundEnemy; always set to random move type instead
						outputEntry.WeaponType = gunSoldierDonor.WeaponType;
						outputEntry.HaveShield = gunSoldierDonor.HaveShield;
						setData[index] = outputEntry;
						break;
					}
				case Object0066_GUNBigfoot originalEnemy when targetEntry is Object0066_GUNBigfoot:
					{
						var newEntry = LayoutEditorFunctions.CreateShadowObject(gunSoldierDonor.List, gunSoldierDonor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						var outputEntry = (Object0064_GUNSoldier)newEntry;
						// EnemyBase
						outputEntry.MoveRange = originalEnemy.MoveRange;
						outputEntry.SearchRange = originalEnemy.SearchRange;
						outputEntry.SearchAngle = originalEnemy.SearchAngle;
						outputEntry.SearchWidth = originalEnemy.SearchWidth;
						outputEntry.SearchHeight = originalEnemy.SearchHeight;
						outputEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						outputEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase
						outputEntry.AppearType = Object0064_GUNSoldier.EAppear.RANDOM_MOVE;
						outputEntry.WeaponType = gunSoldierDonor.WeaponType;
						outputEntry.HaveShield = gunSoldierDonor.HaveShield;
						setData[index] = outputEntry;
						break;
					}
				case Object0068_GUNRobot originalEnemy when targetEntry is Object0068_GUNRobot:
					break;
				case Object0078_EggPierrot originalEnemy when targetEntry is Object0078_EggPierrot:
					{
						var newEntry = LayoutEditorFunctions.CreateShadowObject(gunSoldierDonor.List, gunSoldierDonor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						var outputEntry = (Object0064_GUNSoldier)newEntry;
						// EnemyBase
						outputEntry.MoveRange = originalEnemy.MoveRange;
						outputEntry.SearchRange = originalEnemy.SearchRange;
						outputEntry.SearchAngle = originalEnemy.SearchAngle;
						outputEntry.SearchWidth = originalEnemy.SearchWidth;
						outputEntry.SearchHeight = originalEnemy.SearchHeight;
						outputEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						outputEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase
						outputEntry.AppearType = Object0064_GUNSoldier.EAppear.RANDOM_MOVE;
						outputEntry.WeaponType = gunSoldierDonor.WeaponType;
						outputEntry.HaveShield = gunSoldierDonor.HaveShield;
						setData[index] = outputEntry;
						break;
					}
				case Object0079_EggPawn originalEnemy when targetEntry is Object0079_EggPawn:
					{
						break; // TODO: finish Eggpawn
						var newEntry = LayoutEditorFunctions.CreateShadowObject(gunSoldierDonor.List, gunSoldierDonor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						var outputEntry = (Object0064_GUNSoldier)newEntry;
						// EnemyBase
						outputEntry.MoveRange = originalEnemy.MoveRange;
						outputEntry.SearchRange = originalEnemy.SearchRange;
						outputEntry.SearchAngle = originalEnemy.SearchAngle;
						outputEntry.SearchWidth = originalEnemy.SearchWidth;
						outputEntry.SearchHeight = originalEnemy.SearchHeight;
						outputEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						outputEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase
						switch (originalEnemy.AppearType)
						{
							case Object0079_EggPawn.EAppear.WAIT_ACT:

								break;
							case Object0079_EggPawn.EAppear.OFFSET:
								break;
							case Object0079_EggPawn.EAppear.WARP:
								break;
							case Object0079_EggPawn.EAppear.DASH:
								break;
						}
/*						outputEntry.AppearType = (Object0064_GUNSoldier.EAppear)originalEnemy.AppearType; // TODO: Future - if gun soldiers being dead are breaking things, then this is the cause; exclude WARP appearType in that case
						outputEntry.WeaponType = gunSoldierDonor.WeaponType;
						outputEntry.HaveShield = gunSoldierDonor.HaveShield; // for no instance pass, use originalEnemy.HaveShield

						outputEntry.Pos0_WaitType = (Object0064_GUNSoldier.EWaitType)originalEnemy.Pos0_WaitType + 1; // +1 because GUNSoldiers have an extra RADIO state as the first element
						outputEntry.Pos0_WaitSec = originalEnemy.Pos0_WaitSec;
						outputEntry.Pos0_MoveSpeedRatio = originalEnemy.Pos0_MoveSpeedRatio;
						outputEntry.Pos0_TranslationXFromOrigin = originalEnemy.Pos0_TranslationXFromOrigin;
						outputEntry.Pos0_TranslationZFromOrigin = originalEnemy.Pos0_TranslationZFromOrigin;

						outputEntry.Pos1_WaitType = (Object0064_GUNSoldier.EWaitType)originalEnemy.Pos0_WaitType + 1; // +1 because GUNSoldiers have an extra RADIO state as the first element
						outputEntry.Pos1_WaitSec = originalEnemy.Pos1_WaitSec;
						outputEntry.Pos1_MoveSpeedRatio = originalEnemy.Pos1_MoveSpeedRatio;
						outputEntry.Pos1_TranslationXFromOrigin = originalEnemy.Pos1_TranslationXFromOrigin;
						outputEntry.Pos1_TranslationZFromOrigin = originalEnemy.Pos1_TranslationZFromOrigin;

						outputEntry.Pos2_WaitType = (Object0064_GUNSoldier.EWaitType)originalEnemy.Pos2_WaitType + 1; // +1 because GUNSoldiers have an extra RADIO state as the first element
						outputEntry.Pos2_WaitSec = originalEnemy.Pos2_WaitSec;
						outputEntry.Pos2_MoveSpeedRatio = originalEnemy.Pos2_MoveSpeedRatio;
						setData[index] = outputEntry;*/
						break;
					}
				case Object007A_EggShadowAndroid originalEnemy when targetEntry is Object007A_EggShadowAndroid:
					{
						var newEntry = LayoutEditorFunctions.CreateShadowObject(gunSoldierDonor.List, gunSoldierDonor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						var outputEntry = (Object0064_GUNSoldier)newEntry;
						// EnemyBase
						outputEntry.MoveRange = originalEnemy.MoveRange;
						outputEntry.SearchRange = originalEnemy.SearchRange;
						outputEntry.SearchAngle = originalEnemy.SearchAngle;
						outputEntry.SearchWidth = originalEnemy.SearchWidth;
						outputEntry.SearchHeight = originalEnemy.SearchHeight;
						outputEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						outputEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase
						outputEntry.AppearType = Object0064_GUNSoldier.EAppear.RANDOM_MOVE;
						outputEntry.WeaponType = gunSoldierDonor.WeaponType;
						outputEntry.HaveShield = gunSoldierDonor.HaveShield;
						setData[index] = outputEntry;
						break;
					}
				case Object008C_BkGiant originalEnemy when targetEntry is Object008C_BkGiant:
					{
						var newEntry = LayoutEditorFunctions.CreateShadowObject(gunSoldierDonor.List, gunSoldierDonor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						var outputEntry = (Object0064_GUNSoldier)newEntry;
						// EnemyBase
						outputEntry.MoveRange = originalEnemy.MoveRange;
						outputEntry.SearchRange = originalEnemy.SearchRange;
						outputEntry.SearchAngle = originalEnemy.SearchAngle;
						outputEntry.SearchWidth = originalEnemy.SearchWidth;
						outputEntry.SearchHeight = originalEnemy.SearchHeight;
						outputEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						outputEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase
						outputEntry.AppearType = Object0064_GUNSoldier.EAppear.RANDOM_MOVE;
						outputEntry.WeaponType = gunSoldierDonor.WeaponType;
						outputEntry.HaveShield = gunSoldierDonor.HaveShield; // for no instance pass, use originalEnemy.CanBlockShots
						setData[index] = outputEntry;
						break;
					}
				case Object008D_BkSoldier originalEnemy when targetEntry is Object008D_BkSoldier:
					{
						var newEntry = LayoutEditorFunctions.CreateShadowObject(gunSoldierDonor.List, gunSoldierDonor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						var outputEntry = (Object0064_GUNSoldier)newEntry;
						// EnemyBase
						outputEntry.MoveRange = originalEnemy.MoveRange;
						outputEntry.SearchRange = originalEnemy.SearchRange;
						outputEntry.SearchAngle = originalEnemy.SearchAngle;
						outputEntry.SearchWidth = originalEnemy.SearchWidth;
						outputEntry.SearchHeight = originalEnemy.SearchHeight;
						outputEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						outputEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase
						outputEntry.AppearType = (Object0064_GUNSoldier.EAppear)originalEnemy.AppearType; // TODO: Future - if gun soldiers being dead are breaking things, then this is the cause; exclude WARP appearType in that case
						outputEntry.WeaponType = gunSoldierDonor.WeaponType;
						outputEntry.HaveShield = gunSoldierDonor.HaveShield; // for no instance pass, use originalEnemy.HaveShield
						
						outputEntry.Pos0_WaitType = (Object0064_GUNSoldier.EWaitType)originalEnemy.Pos0_WaitType+1; // +1 because GUNSoldiers have an extra RADIO state as the first element
						outputEntry.Pos0_WaitSec = originalEnemy.Pos0_WaitSec;
						outputEntry.Pos0_MoveSpeedRatio = originalEnemy.Pos0_MoveSpeedRatio;
						outputEntry.Pos0_TranslationXFromOrigin = originalEnemy.Pos0_TranslationXFromOrigin;
						outputEntry.Pos0_TranslationZFromOrigin = originalEnemy.Pos0_TranslationZFromOrigin;

						outputEntry.Pos1_WaitType = (Object0064_GUNSoldier.EWaitType)originalEnemy.Pos0_WaitType + 1; // +1 because GUNSoldiers have an extra RADIO state as the first element
						outputEntry.Pos1_WaitSec = originalEnemy.Pos1_WaitSec;
						outputEntry.Pos1_MoveSpeedRatio = originalEnemy.Pos1_MoveSpeedRatio;
						outputEntry.Pos1_TranslationXFromOrigin = originalEnemy.Pos1_TranslationXFromOrigin;
						outputEntry.Pos1_TranslationZFromOrigin = originalEnemy.Pos1_TranslationZFromOrigin;
						
						outputEntry.Pos2_WaitType = (Object0064_GUNSoldier.EWaitType)originalEnemy.Pos2_WaitType + 1; // +1 because GUNSoldiers have an extra RADIO state as the first element
						outputEntry.Pos2_WaitSec = originalEnemy.Pos2_WaitSec;
						outputEntry.Pos2_MoveSpeedRatio = originalEnemy.Pos2_MoveSpeedRatio;
						setData[index] = outputEntry;
						break;
					}
				case Object008E_BkWingLarge originalEnemy when targetEntry is Object008E_BkWingLarge:
					{
						if (originalEnemy.PathType == Object008E_BkWingLarge.EPathType.FLY_FORWARD_UPDOWN
							|| originalEnemy.PathType == Object008E_BkWingLarge.EPathType.FLY_FORWARD_SWOOP
							|| originalEnemy.PathType == Object008E_BkWingLarge.EPathType.FLY_FORWARD
							|| originalEnemy.PathType == Object008E_BkWingLarge.EPathType.FLY_LEFT)
							break; // We cannot safely convert FLY_* PathType to GroundEnemy, since these are usually way off screen
						var newEntry = LayoutEditorFunctions.CreateShadowObject(gunSoldierDonor.List, gunSoldierDonor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						var outputEntry = (Object0064_GUNSoldier)newEntry;
						// EnemyBase
						outputEntry.MoveRange = originalEnemy.MoveRange;
						outputEntry.SearchRange = originalEnemy.SearchRange;
						outputEntry.SearchAngle = originalEnemy.SearchAngle;
						outputEntry.SearchWidth = originalEnemy.SearchWidth;
						outputEntry.SearchHeight = originalEnemy.SearchHeight;
						outputEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset; // may want to not set this for FlyingEnemy -> GroundEnemy
						outputEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase
						outputEntry.AppearType = Object0064_GUNSoldier.EAppear.RANDOM_MOVE; // there is no good translation for pos0,1,2 for FlyingEnemy -> GroundEnemy; always set to random move type instead
						outputEntry.WeaponType = gunSoldierDonor.WeaponType;
						outputEntry.HaveShield = gunSoldierDonor.HaveShield;
						setData[index] = outputEntry;
						break;
					}
				case Object008F_BkWingSmall originalEnemy when targetEntry is Object008F_BkWingSmall:
					{
						if (originalEnemy.PathType == Object008F_BkWingSmall.EPathType.FLY_FORWARD_UPDOWN
							|| originalEnemy.PathType == Object008F_BkWingSmall.EPathType.FLY_FORWARD_SWOOP
							|| originalEnemy.PathType == Object008F_BkWingSmall.EPathType.FLY_FORWARD
							|| originalEnemy.PathType == Object008F_BkWingSmall.EPathType.FLY_LEFT)
							break; // We cannot safely convert FLY_* PathType to GroundEnemy, since these are usually way off screen
						var newEntry = LayoutEditorFunctions.CreateShadowObject(gunSoldierDonor.List, gunSoldierDonor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						var outputEntry = (Object0064_GUNSoldier)newEntry;
						// EnemyBase
						outputEntry.MoveRange = originalEnemy.MoveRange;
						outputEntry.SearchRange = originalEnemy.SearchRange;
						outputEntry.SearchAngle = originalEnemy.SearchAngle;
						outputEntry.SearchWidth = originalEnemy.SearchWidth;
						outputEntry.SearchHeight = originalEnemy.SearchHeight;
						outputEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset; // may want to not set this for FlyingEnemy -> GroundEnemy
						outputEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase
						outputEntry.AppearType = Object0064_GUNSoldier.EAppear.RANDOM_MOVE; // there is no good translation for pos0,1,2 for FlyingEnemy -> GroundEnemy; always set to random move type instead
						outputEntry.WeaponType = gunSoldierDonor.WeaponType;
						outputEntry.HaveShield = gunSoldierDonor.HaveShield;
						setData[index] = outputEntry;
						break;
					}
				case Object0090_BkWorm originalEnemy when targetEntry is Object0090_BkWorm:
					{
						var newEntry = LayoutEditorFunctions.CreateShadowObject(gunSoldierDonor.List, gunSoldierDonor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						var outputEntry = (Object0064_GUNSoldier)newEntry;
						// EnemyBase
						outputEntry.MoveRange = originalEnemy.MoveRange;
						outputEntry.SearchRange = originalEnemy.SearchRange;
						outputEntry.SearchAngle = originalEnemy.SearchAngle;
						outputEntry.SearchWidth = originalEnemy.SearchWidth;
						outputEntry.SearchHeight = originalEnemy.SearchHeight;
						outputEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						outputEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase
						outputEntry.AppearType = Object0064_GUNSoldier.EAppear.RANDOM_MOVE;
						outputEntry.WeaponType = gunSoldierDonor.WeaponType;
						outputEntry.HaveShield = gunSoldierDonor.HaveShield;
						setData[index] = outputEntry;
						break;
					}
				case Object0091_BkLarva originalEnemy when targetEntry is Object0091_BkLarva:
					{
						var newEntry = LayoutEditorFunctions.CreateShadowObject(gunSoldierDonor.List, gunSoldierDonor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						var outputEntry = (Object0064_GUNSoldier)newEntry;
						// EnemyBase
						outputEntry.MoveRange = originalEnemy.MoveRange;
						outputEntry.SearchRange = originalEnemy.SearchRange;
						outputEntry.SearchAngle = originalEnemy.SearchAngle;
						outputEntry.SearchWidth = originalEnemy.SearchWidth;
						outputEntry.SearchHeight = originalEnemy.SearchHeight;
						outputEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						outputEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase
						outputEntry.AppearType = Object0064_GUNSoldier.EAppear.RANDOM_MOVE;
						outputEntry.WeaponType = gunSoldierDonor.WeaponType;
						outputEntry.HaveShield = gunSoldierDonor.HaveShield;
						setData[index] = outputEntry;
						break;
					}
				case Object0092_BkChaos originalEnemy when targetEntry is Object0092_BkChaos:
					{
						var newEntry = LayoutEditorFunctions.CreateShadowObject(gunSoldierDonor.List, gunSoldierDonor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						var outputEntry = (Object0064_GUNSoldier)newEntry;
						// EnemyBase
						outputEntry.MoveRange = originalEnemy.MoveRange;
						outputEntry.SearchRange = originalEnemy.SearchRange;
						outputEntry.SearchAngle = originalEnemy.SearchAngle;
						outputEntry.SearchWidth = originalEnemy.SearchWidth;
						outputEntry.SearchHeight = originalEnemy.SearchHeight;
						outputEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						outputEntry.MoveSpeedRatio = 1f; // BkChaos does not have moveSpeed, instead has HP value
														 // end EnemyBase
						outputEntry.AppearType = Object0064_GUNSoldier.EAppear.RANDOM_MOVE;
						outputEntry.WeaponType = gunSoldierDonor.WeaponType;
						outputEntry.HaveShield = gunSoldierDonor.HaveShield;
						setData[index] = outputEntry;
						break;
					}
				case Object0093_BkNinja originalEnemy when targetEntry is Object0093_BkNinja:
					{
						var newEntry = LayoutEditorFunctions.CreateShadowObject(gunSoldierDonor.List, gunSoldierDonor.Type, originalEnemy.PosX, originalEnemy.PosY,
								originalEnemy.PosZ, originalEnemy.RotX, originalEnemy.RotY, originalEnemy.RotZ, originalEnemy.Link, originalEnemy.Rend, originalEnemy.UnkBytes);
						var outputEntry = (Object0064_GUNSoldier)newEntry;
						// EnemyBase
						outputEntry.MoveRange = originalEnemy.MoveRange;
						outputEntry.SearchRange = originalEnemy.SearchRange;
						outputEntry.SearchAngle = originalEnemy.SearchAngle;
						outputEntry.SearchWidth = originalEnemy.SearchWidth;
						outputEntry.SearchHeight = originalEnemy.SearchHeight;
						outputEntry.SearchHeightOffset = originalEnemy.SearchHeightOffset;
						outputEntry.MoveSpeedRatio = originalEnemy.MoveSpeedRatio;
						// end EnemyBase
						outputEntry.AppearType = Object0064_GUNSoldier.EAppear.RANDOM_MOVE;
						outputEntry.WeaponType = gunSoldierDonor.WeaponType;
						outputEntry.HaveShield = gunSoldierDonor.HaveShield;
						setData[index] = outputEntry;
						break;
					}
			}
		}
	}
}

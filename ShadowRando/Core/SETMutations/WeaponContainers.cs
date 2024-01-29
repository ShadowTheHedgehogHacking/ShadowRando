using System.Collections.Generic;
using ShadowSET;

namespace ShadowRando.Core.SETMutations;

internal static class WeaponContainers
{
	internal static void ToSpecialWeaponBox(int index, ref List<SetObjectShadow> setData)
	{
		var newEntry = (Object003A_SpecialWeaponBox)LayoutEditorFunctions.CreateShadowObject(0x00, 0x3A,
			setData[index].PosX, setData[index].PosY,
			setData[index].PosZ, setData[index].RotX, setData[index].RotY, setData[index].RotZ, setData[index].Link,
			setData[index].Rend, setData[index].UnkBytes);

		if (setData[index].List == 0x00)
		{
			if (setData[index].Type == 0x09) // Wood Box
			{
				var woodbox = (Object0009_WoodBox)setData[index];
				newEntry.Weapon = woodbox.ModifierWeapon;
				setData[index] = newEntry;
			}
			else if (setData[index].Type == 0x0A) // Metal Box
			{
				var metalbox = (Object000A_MetalBox)setData[index];
				newEntry.Weapon = metalbox.ModifierWeapon;
				setData[index] = newEntry;
			}
			else if (setData[index].Type == 0x0C) // Weapon Box
			{
				var weaponbox = (Object000C_WeaponBox)setData[index];
				newEntry.Weapon = weaponbox.Weapon;
				setData[index] = newEntry;
			}
		}
	}

	public static EBoxType? GetWeaponAffiliationBoxType(EWeapon weapon)
	{
		switch (weapon)
		{
			case EWeapon.Pistol:
			case EWeapon.SubmachineGun:
			case EWeapon.MachineGun:
			case EWeapon.HeavyMachineGun:
			case EWeapon.GatlingGun:
			case EWeapon.GrenadeLauncher:
			case EWeapon.GUNBazooka:
			case EWeapon.TankCannon:
			case EWeapon.RPG:
			case EWeapon.FourShot:
			case EWeapon.EightShot:
			case EWeapon.LaserRifle:
			case EWeapon.Knife:
				return EBoxType.GUN;
			case EWeapon.LightShot:
			case EWeapon.FlashShot:
			case EWeapon.RingShot:
			case EWeapon.HeavyShot:
			case EWeapon.BlackBarrel:
			case EWeapon.BigBarrel:
			case EWeapon.WormShooterBlack:
			case EWeapon.WideWormShooterRed:
			case EWeapon.BigWormShooterGold:
			case EWeapon.VacuumPod:
			case EWeapon.Splitter:
			case EWeapon.Refractor:
			case EWeapon.BlackSword:
			case EWeapon.DarkHammer:
				return EBoxType.BlackArms;
			case EWeapon.EggGun:
			case EWeapon.EggBazooka:
			case EWeapon.EggLance:
				return EBoxType.Eggman;
			default:
				return null;
		}
	}

	internal static void ToWeaponBox(int index, ref List<SetObjectShadow> setData)
	{
		var newEntry = (Object000C_WeaponBox)LayoutEditorFunctions.CreateShadowObject(0x00, 0x0C, setData[index].PosX,
			setData[index].PosY,
			setData[index].PosZ, setData[index].RotX, setData[index].RotY, setData[index].RotZ, setData[index].Link,
			setData[index].Rend, setData[index].UnkBytes);

		if (setData[index].List != 0x00 || setData[index].Type != 0x3A) return;
		var specialWeaponBox = (Object003A_SpecialWeaponBox)setData[index];
		newEntry.Weapon = specialWeaponBox.Weapon;
		var boxType = GetWeaponAffiliationBoxType(specialWeaponBox.Weapon);
		if (boxType.HasValue)
			newEntry.BoxType = boxType.Value;
		setData[index] = newEntry;
	}
}
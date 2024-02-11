using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using HeroesONE_R.Structures.Substructures;
using ShadowSET.Utilities;

// Hacky file for lazy spline editing; Uses the inaccurate save version from HPP; Only good for editing core attributes!

namespace ShadowRando.Core
{
	public class ShadowSplineSec5Bytes
	{
		public byte slot1 { get; set; }
		public byte slot2 { get; set; }
		public bool noSlot2 { get; set; }
	}
	public class ShadowSplineVertex
	{
		public Vector3 Position;
		public float PositionX
		{
			get => Position.X;
			set => Position.X = value;
		}
		public float PositionY
		{
			get => Position.Y;
			set => Position.Y = value;
		}
		public float PositionZ
		{
			get => Position.Z;
			set => Position.Z = value;
		}
		public Vector3 Rotation;
		public float RotationX
		{
			get => RadiansToDegrees(Rotation.X);
			set => Rotation.X = DegreesToRadians(value);
		}
		public float RotationY
		{
			get => RadiansToDegrees(Rotation.Y);
			set => Rotation.Y = DegreesToRadians(value);
		}
		public float RotationZ
		{
			get => RadiansToDegrees(Rotation.Z);
			set => Rotation.Z = DegreesToRadians(value);
		}
		public int AngularAttachmentToleranceInt { get; set; }

		public override string ToString()
		{
			return $"X:{PositionX} Y:{PositionY} Z:{PositionZ} AAT:{AngularAttachmentToleranceInt}";
		}

		public static float DegreesToRadians(float degree)
		{
			return degree * ((float)Math.PI / 180f);
		}

		public static float RadiansToDegrees(float radian)
		{
			return radian * (180f / (float)Math.PI);
		}
	}
	public class ShadowSpline
	{
		public byte Setting1 { get; set; }
		public byte Setting2 { get; set; }
		public byte SplineType { get; set; }
		public byte Setting4 { get; set; }
		public int SettingInt { get; set; }
		public string Name { get; set; }
		public ShadowSplineSec5Bytes[] UnknownSec5Bytes { get; set; }
		public ShadowSplineVertex[] Vertices { get; set; }
		public ShadowSpline()
		{
			Vertices = new ShadowSplineVertex[0];
			UnknownSec5Bytes = new ShadowSplineSec5Bytes[0];
			Name = "NewSpline";
		}
		public IEnumerable<byte> ToByteArray(int startOffset)
		{
			List<byte> vertexBytes = new List<byte>(0x20 * Vertices.Length);

			float totalLength = 0;
			Vector3 Max = Vertices[0].Position;
			Vector3 Min = Vertices[0].Position;

			for (int i = 0; i < Vertices.Length; i++)
			{
				float distance = i == Vertices.Length - 1 ? 0 : Vector3.Distance(Vertices[i].Position, Vertices[i + 1].Position);
				totalLength += distance;

				if (Vertices[i].PositionX > Max.X)
					Max.X = Vertices[i].Position.X;
				if (Vertices[i].PositionY > Max.Y)
					Max.Y = Vertices[i].PositionY;
				if (Vertices[i].PositionZ > Max.Z)
					Max.Z = Vertices[i].PositionZ;
				if (Vertices[i].PositionX < Min.X)
					Min.X = Vertices[i].PositionX;
				if (Vertices[i].PositionY < Min.Y)
					Min.Y = Vertices[i].PositionY;
				if (Vertices[i].PositionZ < Min.Z)
					Min.Z = Vertices[i].PositionZ;

				vertexBytes.AddRange(BitConverter.GetBytes(Vertices[i].PositionX).Reverse());
				vertexBytes.AddRange(BitConverter.GetBytes(Vertices[i].PositionY).Reverse());
				vertexBytes.AddRange(BitConverter.GetBytes(Vertices[i].PositionZ).Reverse());
				vertexBytes.AddRange(BitConverter.GetBytes(Vertices[i].Rotation.X).Reverse());
				vertexBytes.AddRange(BitConverter.GetBytes(Vertices[i].Rotation.Y).Reverse());
				vertexBytes.AddRange(BitConverter.GetBytes(Vertices[i].Rotation.Z).Reverse());
				vertexBytes.AddRange(BitConverter.GetBytes(distance).Reverse());
				vertexBytes.AddRange(BitConverter.GetBytes(Vertices[i].AngularAttachmentToleranceInt).Reverse());
			}

			List<byte> bytes = new List<byte>(0x30 + 0x20 * Vertices.Length);

			bytes.AddRange(BitConverter.GetBytes(Vertices.Length).Reverse());
			bytes.AddRange(BitConverter.GetBytes(totalLength).Reverse());
			bytes.AddRange(BitConverter.GetBytes(startOffset + 0x30).Reverse());
			bytes.Add(Setting1);
			bytes.Add(Setting2);
			bytes.Add(SplineType);
			bytes.Add(Setting4);
			bytes.AddRange(BitConverter.GetBytes(Max.X).Reverse());
			bytes.AddRange(BitConverter.GetBytes(Max.Y).Reverse());
			bytes.AddRange(BitConverter.GetBytes(Max.Z).Reverse());
			bytes.AddRange(BitConverter.GetBytes(SettingInt).Reverse());
			bytes.AddRange(BitConverter.GetBytes(Min.X).Reverse());
			bytes.AddRange(BitConverter.GetBytes(Min.Y).Reverse());
			bytes.AddRange(BitConverter.GetBytes(Min.Z).Reverse());
			bytes.AddRange(BitConverter.GetBytes(0));

			bytes.AddRange(vertexBytes);

			return bytes;
		}
	}

	public class SplineReader
	{
		private static string ReadString(BinaryReader binaryReader)
		{
			List<char> list = new List<char>();

			while (binaryReader.PeekChar() != '\0')
				list.Add(binaryReader.ReadChar());

			binaryReader.BaseStream.Position += 1;

			return new string(list.ToArray());
		}
		public static List<ShadowSpline> ReadShadowSplineFile(ArchiveFile pathPTP)
		{
			var splineReader = new EndianBinaryReader(new MemoryStream(pathPTP.DecompressThis().ToArray()), Endianness.Big);

			List<ShadowSpline> splineList = new List<ShadowSpline>();

			splineReader.BaseStream.Position = 0x4;
			int sec5offset = splineReader.ReadInt32();
			int sec5length = splineReader.ReadInt32();

			splineReader.BaseStream.Position = 0x20;
			List<int> offsetList = new List<int>();

			int a = splineReader.ReadInt32();

			while (a != 0)
			{
				offsetList.Add(a + 0x20);
				a = splineReader.ReadInt32();
			}

			foreach (int i in offsetList)
			{
				if (i >= splineReader.BaseStream.Length)
					throw new Exception();

				splineReader.BaseStream.Position = i;

				ShadowSpline spline = new ShadowSpline();
				int amountOfPoints = splineReader.ReadInt32();

				splineReader.BaseStream.Position += 8;

				spline.Setting1 = splineReader.ReadByte();
				spline.Setting2 = splineReader.ReadByte();
				spline.SplineType = splineReader.ReadByte();
				spline.Setting4 = splineReader.ReadByte();

				splineReader.BaseStream.Position += 0xC;

				spline.SettingInt = splineReader.ReadInt32();

				splineReader.BaseStream.Position += 0xC;

				int nameOffset = splineReader.ReadInt32();

				spline.Vertices = new ShadowSplineVertex[amountOfPoints];

				for (int j = 0; j < amountOfPoints; j++)
				{
					ShadowSplineVertex vertex = new ShadowSplineVertex
					{
						Position = new Vector3(splineReader.ReadSingle(), splineReader.ReadSingle(), splineReader.ReadSingle()),
						Rotation = new Vector3(splineReader.ReadSingle(), splineReader.ReadSingle(), splineReader.ReadSingle())
					};
					splineReader.BaseStream.Position += 0x4;
					vertex.AngularAttachmentToleranceInt = splineReader.ReadInt32();

					spline.Vertices[j] = vertex;
				}

				splineReader.BaseStream.Position = nameOffset + 0x20;
				spline.Name = ReadString(splineReader);

				splineList.Add(spline);
			}

			splineReader.BaseStream.Position = sec5offset + 0x20 + splineList.Count;

			for (int i = 0; i < splineList.Count; i++)
			{
				byte byte0 = splineReader.ReadByte();

				if (byte0 >= 0x80)
				{
					byte byte1 = splineReader.ReadByte();
					splineList[i].UnknownSec5Bytes = new ShadowSplineSec5Bytes[1] { new ShadowSplineSec5Bytes { slot1 = byte0, slot2 = byte1, noSlot2 = false } };
				}
				else
					splineList[i].UnknownSec5Bytes = new ShadowSplineSec5Bytes[1] { new ShadowSplineSec5Bytes { slot1 = byte0, noSlot2 = true } };

				splineReader.ReadByte();
			}

			splineReader.Close();

			return splineList;
		}

		public static byte[] ShadowSplinesToByteArray(string shadowFolderNamePrefix, List<ShadowSpline> Splines)
		{
			List<byte> bytes = new List<byte>();

			bytes.AddRange(BitConverter.GetBytes(0));
			bytes.AddRange(BitConverter.GetBytes(0));
			bytes.AddRange(BitConverter.GetBytes(0));
			bytes.AddRange(BitConverter.GetBytes(1).Reverse());
			bytes.AddRange(BitConverter.GetBytes(0));
			bytes.AddRange(BitConverter.GetBytes(12610).Reverse());
			bytes.AddRange(BitConverter.GetBytes(0));
			bytes.AddRange(BitConverter.GetBytes(0));

			// add 0x10 offset (breaks without this on stg0412)
			// with this added offset, breaks stg0504, so for now just hardcode for 0412
			if (shadowFolderNamePrefix == "stg0412")
			{
				for (int i = 0; i < 10; i++)
					bytes.Add(0);
			}

			foreach (ShadowSpline s in Splines)
				bytes.AddRange(BitConverter.GetBytes(0));

			while (bytes.Count % 0x10 != 0)
				bytes.Add(0);

			List<int> offsets = new List<int>();

			for (int i = 0; i < Splines.Count; i++)
			{
				offsets.Add(bytes.Count - 0x20);
				bytes.AddRange(Splines[i].ToByteArray(bytes.Count - 0x20));
			}

			for (int i = 0; i < Splines.Count; i++)
			{
				byte[] offsetBytes = BitConverter.GetBytes(offsets[i]);

				bytes[0x20 + 4 * i + 0] = offsetBytes[3];
				bytes[0x20 + 4 * i + 1] = offsetBytes[2];
				bytes[0x20 + 4 * i + 2] = offsetBytes[1];
				bytes[0x20 + 4 * i + 3] = offsetBytes[0];

				byte[] nameOffset = BitConverter.GetBytes(bytes.Count - 0x20);

				bytes[offsets[i] + 0x20 + 0x2C] = nameOffset[3];
				bytes[offsets[i] + 0x20 + 0x2D] = nameOffset[2];
				bytes[offsets[i] + 0x20 + 0x2E] = nameOffset[1];
				bytes[offsets[i] + 0x20 + 0x2F] = nameOffset[0];

				foreach (char c in Splines[i].Name)
					bytes.Add((byte)c);

				bytes.Add(0);
			}

			while (bytes.Count % 0x4 != 0)
				bytes.Add(0);

			int section5startOffset = bytes.Count - 0x20;

			bytes.Add(0x40);

			for (int i = 1; i < Splines.Count; i++)
				bytes.Add(0x41);

			for (int i = 0; i < Splines.Count; i++)
			{
				bytes.Add(Splines[i].UnknownSec5Bytes[0].slot1);
				if (!Splines[i].UnknownSec5Bytes[0].noSlot2)
					bytes.Add(Splines[i].UnknownSec5Bytes[0].slot2);
				bytes.Add(0x49);
			}

			while (bytes.Count % 0x4 != 0)
				bytes.Add(0);

			int section5length = bytes.Count - section5startOffset - 0x20;

			for (int i = 0; i < 8; i++)
				bytes.Add(0);

			foreach (char c in ("o:\\PJS\\PJSart\\exportdata\\stage\\" + shadowFolderNamePrefix + "\\path"))
				bytes.Add((byte)c);
			bytes.Add(0);

			// Inspect byte % 0x10

			while (bytes.Count % 0x4 != 0)
				bytes.Add(0);

			byte[] aux = BitConverter.GetBytes(bytes.Count);

			bytes[0] = aux[3];
			bytes[1] = aux[2];
			bytes[2] = aux[1];
			bytes[3] = aux[0];

			aux = BitConverter.GetBytes(section5startOffset);

			bytes[4] = aux[3];
			bytes[5] = aux[2];
			bytes[6] = aux[1];
			bytes[7] = aux[0];

			aux = BitConverter.GetBytes(section5length);

			bytes[8] = aux[3];
			bytes[9] = aux[2];
			bytes[10] = aux[1];
			bytes[11] = aux[0];

			return bytes.ToArray();
		}
	}
}

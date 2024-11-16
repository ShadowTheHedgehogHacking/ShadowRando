using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using HeroesONE_R.Structures.Substructures;
using ShadowSET.Utilities;

// Synced with HeroesPowerPlant commit https://github.com/igorseabra4/HeroesPowerPlant/commit/f6afdc8fd67cc5798f4eed3d615a6588608a475b
namespace ShadowRando.Core
{
	public class ShadowSplinePOF0
	{
		public byte slot1 { get; set; }
		public byte slot2 { get; set; }
		public bool noSlot2 { get; set; }

		public override string ToString()
		{
			return $"S1: {slot1} | S2: {slot2} | noS2: {noSlot2}";
		}
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
		public ShadowSplinePOF0 pof0 { get; set; }
		public ShadowSplineVertex[] Vertices { get; set; }
		public ShadowSpline()
		{
			Vertices = new ShadowSplineVertex[0];
			pof0 = new ShadowSplinePOF0();
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
					splineList[i].pof0 = new ShadowSplinePOF0 { slot1 = byte0, slot2 = byte1, noSlot2 = false };
				}
				else
				{
					splineList[i].pof0 = new ShadowSplinePOF0 { slot1 = byte0, noSlot2 = true };
				}
				splineReader.ReadByte();
			}

			splineReader.Close();

			return splineList;
		}

		public static byte[] ShadowSplinesToByteArray(string shadowFolderNamePrefix, List<ShadowSpline> Splines)
		{
			List<byte> bytes = new List<byte>();
			List<int> offsetLocations = new List<int>();
			bytes.AddRange(BitConverter.GetBytes(0));
			bytes.AddRange(BitConverter.GetBytes(0));
			bytes.AddRange(BitConverter.GetBytes(0));
			bytes.AddRange(BitConverter.GetBytes(1).Reverse());
			bytes.AddRange(BitConverter.GetBytes(0));
			bytes.AddRange(BitConverter.GetBytes(12610).Reverse());
			bytes.AddRange(BitConverter.GetBytes(0));
			bytes.AddRange(BitConverter.GetBytes(0));

			// add forced offset if its already == 0 (-_-)
			if (bytes.Count % 0x10 == 0)
			{
				for (int i = 0; i < 10; i++)
					bytes.Add(0);
			}

			while (bytes.Count % 0x10 != 0)
				bytes.Add(0);

			foreach (ShadowSpline s in Splines)
				bytes.AddRange(BitConverter.GetBytes(0));

			List<int> offsets = new List<int>();

			for (int i = 0; i < Splines.Count; i++)
			{
				offsetLocations.Add(bytes.Count - 0x20 + 0x8);
				offsets.Add(bytes.Count - 0x20);
				bytes.AddRange(Splines[i].ToByteArray(bytes.Count - 0x20));
			}

			for (int i = 0; i < Splines.Count; i++)
			{
				offsetLocations.Add(4 * i);
				byte[] offsetBytes = BitConverter.GetBytes(offsets[i]);

				bytes[0x20 + 4 * i + 0] = offsetBytes[3];
				bytes[0x20 + 4 * i + 1] = offsetBytes[2];
				bytes[0x20 + 4 * i + 2] = offsetBytes[1];
				bytes[0x20 + 4 * i + 3] = offsetBytes[0];

				offsetLocations.Add(offsets[i] + 0x2C);
				offsets.Add(bytes.Count - 0x20);
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

			offsets.Add(bytes.Count - 0x20);
			int pof0startOffset = bytes.Count - 0x20;

			offsetLocations.Sort();
			var pof0 = POF0.GenerateRawPOF0(offsetLocations);
			bytes.AddRange(pof0);

			int pof0Length = pof0.Length;

			for (int i = 0; i < 8; i++)
				bytes.Add(0);

			foreach (char c in ("o:\\PJS\\PJSart\\exportdata\\stage\\" + shadowFolderNamePrefix + "\\path"))
				bytes.Add((byte)c);
			bytes.Add(0);

			while (bytes.Count % 0x4 != 0)
				bytes.Add(0);

			byte[] aux = BitConverter.GetBytes(bytes.Count);

			bytes[0] = aux[3];
			bytes[1] = aux[2];
			bytes[2] = aux[1];
			bytes[3] = aux[0];

			aux = BitConverter.GetBytes(pof0startOffset);

			bytes[4] = aux[3];
			bytes[5] = aux[2];
			bytes[6] = aux[1];
			bytes[7] = aux[0];

			aux = BitConverter.GetBytes(pof0Length);

			bytes[8] = aux[3];
			bytes[9] = aux[2];
			bytes[10] = aux[1];
			bytes[11] = aux[0];

			return bytes.ToArray();
		}
	}
}

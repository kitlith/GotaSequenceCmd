using GotaSoundIO.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevolutionFileLoader {
    //using Table = GotaSoundIO.Table;

    /// <summary>
    /// DSP-ADPCM info.
    /// </summary>
    public class DspAdpcmInfo : IReadable, IWriteable {

        /// <summary>
        /// Get the coeffecients.
        /// </summary>
        /// <returns>The coefficients.</returns>
        public short[] GetCoeffs() {
            List<short> c = new List<short>();
            foreach (var a in coefs) {
                c.AddRange(a);
            }
            return c.ToArray();
        }

        /// <summary>
        /// Load the coefficients.
        /// </summary>
        /// <param name="c">The coefficients.</param>
        public void LoadCoeffs(short[] c) {
            coefs = new short[8][];
            coefs[0] = new short[] { c[0], c[1] };
            coefs[1] = new short[] { c[2], c[3] };
            coefs[2] = new short[] { c[4], c[5] };
            coefs[3] = new short[] { c[6], c[7] };
            coefs[4] = new short[] { c[8], c[9] };
            coefs[5] = new short[] { c[10], c[11] };
            coefs[6] = new short[] { c[12], c[13] };
            coefs[7] = new short[] { c[14], c[15] };
        }

        /// <summary>
        /// Read the info.
        /// </summary>
        /// <param name="r">The reader.</param>
        public void Read(FileReader r) {
            LoadCoeffs(r.ReadInt16s(16));
            gain = r.ReadUInt16();
            pred_scale = r.ReadUInt16();
            yn1 = r.ReadInt16();
            yn2 = r.ReadInt16();
            loop_pred_scale = r.ReadUInt16();
            loop_yn1 = r.ReadInt16();
            loop_yn2 = r.ReadInt16();
        }

        /// <summary>
        /// Write the info.
        /// </summary>
        /// <param name="w">The writer.</param>
        public void Write(FileWriter w) {
            w.Write(GetCoeffs());
            w.Write(gain);
            w.Write(pred_scale);
            w.Write(yn1);
            w.Write(yn2);
            w.Write(loop_pred_scale);
            w.Write(loop_yn1);
            w.Write(loop_yn2);
        }

        /// <summary>
        /// [8][2] array of coefficients.
        /// </summary>
        public Int16[][] coefs;

        /// <summary>
        /// Gain.
        /// </summary>
        public UInt16 gain;

        /// <summary>
        /// Predictor scale.
        /// </summary>
        public UInt16 pred_scale;

        /// <summary>
        /// History 1.
        /// </summary>
        public Int16 yn1;

        /// <summary>
        /// History 2.
        /// </summary>
        public Int16 yn2;

        /// <summary>
        /// Loop predictor scale.
        /// </summary>
        public UInt16 loop_pred_scale;

        /// <summary>
        /// Loop history 1.
        /// </summary>
        public Int16 loop_yn1;

        /// <summary>
        /// Loop history 2.
        /// </summary>
        public Int16 loop_yn2;

    }

    /// <summary>
    /// PCM format.
    /// </summary>
    public enum PcmFormat { 
        SignedPCM8 = 0,
        PCM16 = 1,
        DspAdpcm = 2
    }

    /// <summary>
    /// Id type.
    /// </summary>
    public enum IdType { 
        Invalid = 0,
        Sequence = 1,
        Stream = 2,
        Wave = 3
    }

    /// <summary>
    /// Type of reference.
    /// </summary>
    public enum ReferenceType : byte { 
        Address = 0,
        Offset = 1
    }

    /// <summary>
    /// Data identifiers for references.
    /// </summary>
    public enum DataIdentifier : byte { 
    
    }

    /// <summary>
    /// A version.
    /// </summary>
    public class RVersion : GotaSoundIO.IO.Version {

        /// <summary>
        /// Create a version from a ushort.
        /// </summary>
        /// <param name="s">The ushort.</param>
        public void FromUShort(ushort s) {
            Major = (byte)((s & 0xFF00) >> 8);
            Minor = (byte)(s & 0xFF);
        }

        /// <summary>
        /// Convert the version to a ushort.
        /// </summary>
        /// <returns>The version as a ushort.</returns>
        public ushort ToUShort() {
            return (ushort)((Major << 8) | Minor);
        }

        /// <summary>
        /// Read the version.
        /// </summary>
        /// <param name="r">The reader.</param>
        public override void Read(FileReader r) {
            FromUShort(r.ReadUInt16());
        }

        /// <summary>
        /// Write the version.
        /// </summary>
        /// <param name="w">The writer.</param>
        public override void Write(FileWriter w) {
            w.Write(ToUShort());
        }

    }


    /// <summary>
    /// Offset to data.
    /// </summary>
    public class ROffset<T> : Reference<T> {

        /// <summary>
        /// Don't set current offset on jump.
        /// </summary>
        public override bool SetCurrentOffsetOnJump() => false;

        /// <summary>
        /// Null reference is not 0.
        /// </summary>
        public override bool NullReferenceIs0() => false;

        /// <summary>
        /// Read the reference.
        /// </summary>
        /// <param name="r">The reader.</param>
        public override void ReadRef(FileReader r) {
            Offset = r.ReadInt32();
        }

        /// <summary>
        /// Write the reference.
        /// </summary>
        /// <param name="w">The writer.</param>
        public override void WriteRef(FileWriter w, bool ignoreNullData = false) {
            w.Write((int)Offset);
        }

    }

    /// <summary>
    /// Reference to data.
    /// </summary>
    public class RReference<T> : Reference<T> {

        /// <summary>
        /// Don't set current offset on jump.
        /// </summary>
        public override bool SetCurrentOffsetOnJump() => false;

        /// <summary>
        /// Null reference is not 0.
        /// </summary>
        public override bool NullReferenceIs0() => true;

        /// <summary>
        /// Read the reference.
        /// </summary>
        /// <param name="r">The reader.</param>
        public override void ReadRef(FileReader r) {
            Absolute = !r.ReadBoolean();
            Identifier = r.ReadByte();
            r.ReadUInt16();
            Offset = r.ReadInt32();
        }

        /// <summary>
        /// Write the reference.
        /// </summary>
        /// <param name="w">The writer.</param>
        /// <param name="ignoreNullData">Ignore null data.</param>
        public override void WriteRef(FileWriter w, bool ignoreNullData = false) {
            if (!ignoreNullData && ((NullReferenceIs0() && Offset == 0) || (!NullReferenceIs0() && Offset == -1))) {
                w.Write(new byte[8]);
            } else {
                w.Write(!Absolute);
                w.Write((byte)Identifier);
                w.Write((ushort)0);
                w.Write((int)Offset);
            }
        }

    }

    /// <summary>
    /// File info.
    /// </summary>
    public class FileInfo : IReadable, IWriteable {

        /// <summary>
        /// Group Id.
        /// </summary>
        public Id GroupId;

        /// <summary>
        /// Index in the group.
        /// </summary>
        public uint Index;

        /// <summary>
        /// Read the file info.
        /// </summary>
        /// <param name="r">The reader.</param>
        public void Read(FileReader r) {
            GroupId = r.Read<Id>();
            Index = r.ReadUInt32();
        }

        public void Write(FileWriter w) {
            w.Write(GroupId);
            w.Write(Index);
        }

    }

    /// <summary>
    /// An Id.
    /// </summary>
    public class Id : IReadable, IWriteable {

        /// <summary>
        /// Value.
        /// </summary>
        private uint value;

        /// <summary>
        /// Type.
        /// </summary>
        public IdType Type {

            //Get type.
            get {
                return (IdType)((value & 0xFF000000) >> 24);
            }

            //Set type.
            set {
                this.value = (this.value & 0xFFFFFF) | (((uint)value & 0xFF) << 24);
            }

        }

        /// <summary>
        /// Index of the item.
        /// </summary>
        public int Index {

            //Get index.
            get {
                return (int)(value & 0xFFFFFF);
            }

            //Set index.
            set {
                this.value = (this.value & 0xFF000000) | ((uint)value & 0xFFFFFF);
            }

        }

        /// <summary>
        /// Read the id.
        /// </summary>
        /// <param name="r">The reader.</param>
        public void Read(FileReader r) {
            value = r.ReadUInt32();
        }

        /// <summary>
        /// Write the id.
        /// </summary>
        /// <param name="w">The writer.</param>
        public void Write(FileWriter w) {
            w.Write(value);
        }

    }

    /// <summary>
    /// A revolution file header.
    /// </summary>
    public class RFileHeader : FileHeader {

        /// <summary>
        /// Read the header.
        /// </summary>
        /// <param name="r">The reader.</param>
        public override void Read(FileReader r) {
            Magic = new string(r.ReadChars(4));
            ByteOrder = r.ByteOrder = ByteOrder.BigEndian;
            if (r.ReadUInt16() == 0xFFFE) {
                ByteOrder = ByteOrder.LittleEndian;
            }
            Version = r.Read<RVersion>();
            FileSize = r.ReadUInt32();
            HeaderSize = r.ReadUInt16();
            ushort numBlocks = r.ReadUInt16();
            BlockOffsets = new long[numBlocks];
            BlockSizes = new long[numBlocks];
            for (int i = 0; i < numBlocks; i++) {
                BlockOffsets[i] = r.ReadUInt32();
                BlockSizes[i] = r.ReadUInt32();
            }
        }

        /// <summary>
        /// Write the header.
        /// </summary>
        /// <param name="w">The writer.</param>
        public override void Write(FileWriter w) {
            w.Write(Magic.ToCharArray());
            w.ByteOrder = ByteOrder;
            w.Write((ushort)0xFEFF);
            w.Write(Version);
            w.Write((uint)FileSize);
            HeaderSize = 0x10 + 8 * BlockOffsets.Length;
            while (HeaderSize % 0x20 != 0) {
                HeaderSize++;
            }
            w.Write((ushort)HeaderSize);
            w.Write((ushort)BlockOffsets.Length);
            for (int i = 0; i < BlockOffsets.Length; i++) {
                w.Write((uint)BlockOffsets[i]);
                w.Write((uint)BlockSizes[i]);
            }
            w.Align(0x20);
        }

    }

    /// <summary>
    /// An RSTM file header.
    /// </summary>
    public class RSTMFileHeader : FileHeader {

        /// <summary>
        /// Read the header.
        /// </summary>
        /// <param name="r">The reader.</param>
        public override void Read(FileReader r) {
            Magic = new string(r.ReadChars(4));
            ByteOrder = r.ByteOrder = ByteOrder.BigEndian;
            if (r.ReadUInt16() == 0xFFFE) {
                ByteOrder = ByteOrder.LittleEndian;
            }
            Version = r.Read<RVersion>();
            FileSize = r.ReadUInt32();
            HeaderSize = r.ReadUInt16();
            ushort numBlocks = r.ReadUInt16();
            if (Version.Major >= 1) { numBlocks++; }
            BlockOffsets = new long[numBlocks];
            BlockSizes = new long[numBlocks];
            for (int i = 0; i < numBlocks; i++) {
                BlockOffsets[i] = r.ReadUInt32();
                BlockSizes[i] = r.ReadUInt32();
            }
        }

        /// <summary>
        /// Write the header.
        /// </summary>
        /// <param name="w">The writer.</param>
        public override void Write(FileWriter w) {
            w.Write(Magic.ToCharArray());
            w.ByteOrder = ByteOrder;
            w.Write((ushort)0xFEFF);
            w.Write(Version);
            w.InitOffset("fileSize");
            HeaderSize = 0x10 + 8 * BlockOffsets.Length;
            while (HeaderSize % 0x20 != 0) {
                HeaderSize++;
            }
            w.Write((ushort)HeaderSize);
            w.Write((ushort)2);
            for (int i = 0; i < BlockOffsets.Length; i++) {
                w.InitOffset("block" + i + "Offset");
                w.InitOffset("block" + i + "Size");
            }
            w.Align(0x20);
        }

    }


}

using BrawlLib.Imaging;
using System;
using System.Runtime.InteropServices;

namespace BrawlLib.SSBBTypes
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct STEX
    {
        public const string Tag = "STEX";
        
        public const int Size = 0x40;
        public BinTag _tag;                 // 0x00 - Uneditable; STEX
        public bushort _soundBankID;        // 
        public bushort _effectBankID;       //
        public buint _trackListID;          //
        public const byte FolderNameOffset = 0x1A;
        public bushort _stageNameOffset;    //
        public bushort _moduleNameOffset;   // 
        public bushort _variantNamesOffset; // 
        public RGBAPixel _rgbaOverlay;      //
        public byte _isFlat;                // Just a bool for now. May turn into flags at later date
        public byte _isFixedCamera;         // 
        public VarianceType _varianceType;
        public byte _varianceRange;

        private VoidPtr Address
        {
            get
            {
                fixed (void* ptr = &this)
                {
                    return ptr;
                }
            }
        }
        
        public string FolderName
        {
            get => new string((sbyte*) Address + FolderNameOffset);
            set
            {
                int len = value.Length + 1;

                byte* dPtr = (byte*) Address + FolderNameOffset;
                fixed (char* sPtr = value)
                {
                    for (int i = 0; i < len; i++)
                    {
                        *dPtr++ = (byte) sPtr[i];
                    }
                }

                //Align to 4 bytes
                while ((len++ & 3) != 0)
                {
                    *dPtr++ = 0;
                }
            }
        }
        
        public string StageName
        {
            get => new string((sbyte*) Address + _stageNameOffset);
            set
            {
                int len = value.Length + 1;

                byte* dPtr = (byte*) Address + _stageNameOffset;
                fixed (char* sPtr = value)
                {
                    for (int i = 0; i < len; i++)
                    {
                        *dPtr++ = (byte) sPtr[i];
                    }
                }

                //Align to 4 bytes
                while ((len++ & 3) != 0)
                {
                    *dPtr++ = 0;
                }
            }
        }
        
        public string ModuleName
        {
            get => new string((sbyte*) Address + _moduleNameOffset);
            set
            {
                int len = value.Length + 1;

                byte* dPtr = (byte*) Address + _moduleNameOffset;
                fixed (char* sPtr = value)
                {
                    for (int i = 0; i < len; i++)
                    {
                        *dPtr++ = (byte) sPtr[i];
                    }
                }

                //Align to 4 bytes
                while ((len++ & 3) != 0)
                {
                    *dPtr++ = 0;
                }
            }
        }

        public enum VarianceType : byte
        {
            Normal = 0,
            RandomVariance = 1,
            SequentialTransform = 2,
            RandomTransform = 3,
            TimeBased = 4
        }
    }
}
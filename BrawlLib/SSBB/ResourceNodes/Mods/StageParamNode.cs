using BrawlLib.Imaging;
using BrawlLib.Internal;
using BrawlLib.SSBBTypes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BrawlLib.SSBB.ResourceNodes
{
    public unsafe class StageParamNode : ResourceNode
    {
        internal STEX* Header => (STEX*) WorkingUncompressed.Address;

        private ushort _soundBankID;

        [Category("SFX/GFX")]
        public ushort SoundBankId
        {
            get => _soundBankID;
            set => _soundBankID = value;
        }

        private ushort _effectBankID;

        [Category("SFX/GFX")]
        public ushort EffectBankId
        {
            get => _effectBankID;
            set
            {
                _effectBankID = value;
                SignalPropertyChange();
            }
        }

        private uint _trackListID;

        [Category("SFX/GFX")]
        public uint TrackListId
        {
            get => _trackListID;
            set
            { 
                _trackListID = value;
                SignalPropertyChange();
            }
        }

        private RGBAPixel _rgbaOverlay;

        [Category("Stage Properties")]
        public RGBAPixel RgbaOverlay
        {
            get => _rgbaOverlay;
            set
            {
                _rgbaOverlay = value;
                SignalPropertyChange();
            }
        }

        private bool _isFlat;

        [Category("Stage Properties")]
        public bool IsFlat
        {
            get => _isFlat;
            set
            {
                _isFlat = value;
                SignalPropertyChange();
            }
        }

        private bool _isFixedCamera;

        [Category("Stage Properties")]
        public bool IsFixedCamera
        {
            get => _isFixedCamera;
            set
            {
                _isFixedCamera = value;
                SignalPropertyChange();
            }
        }

        private STEX.VarianceType _varianceType;

        [Category("File Properties")]
        public STEX.VarianceType VarianceType
        {
            get => _varianceType;
            set
            {
                _varianceType = value;
                SignalPropertyChange();
            }
        }

        private string _folderName;

        [Category("File Properties")]
        public string FolderName
        {
            get => _folderName;
            set
            {
                _folderName = value;
                SignalPropertyChange();
            }
        }

        private string _stageName;

        [Category("File Properties")]
        public string StageName
        {
            get => _stageName;
            set
            {
                _stageName = value;
                Name = value;
                SignalPropertyChange();
            }
        }

        private string _moduleName;

        [Category("File Properties")]
        public string ModuleName
        {
            get => _moduleName;
            set
            {
                _moduleName = value;
                SignalPropertyChange();
            }
        }

        private List<string> _variantNames = new List<string>();

        [Category("File Properties")]
        public string[] VariantNames
        {
            get => _variantNames.ToArray();
            set
            {
                _variantNames = value.ToList();
                SignalPropertyChange();
            }
        }

        public override int OnCalculateSize(bool force)
        {
            int size = STEX.FolderNameOffset + (_folderName.Length + 1) + (_stageName.Length + 1) +
                       (_moduleName.Length + (_variantNames.Count > 0 ? 1 : 0));
            if (_variantNames != null && _variantNames.Count > 0)
            {
                foreach (string s in _variantNames)
                {
                    size += s.Length + 1;
                }
            }

            return size.Align(0x10);
        }

        public override void OnRebuild(VoidPtr address, int length, bool force)
        {
            STEX* hdr = (STEX*) address;
            hdr->_tag = STEX.Tag;
            hdr->_soundBankID = _soundBankID;
            hdr->_effectBankID = _effectBankID;
            hdr->_trackListID = _trackListID;
            hdr->_stageNameOffset = (ushort)(STEX.FolderNameOffset + (_folderName.Length + 1));
            hdr->_moduleNameOffset = (ushort)(hdr->_stageNameOffset + (_stageName.Length + 1));
            hdr->_variantNamesOffset = (ushort)(hdr->_moduleNameOffset + (_moduleName.Length + 1));
            hdr->_rgbaOverlay = _rgbaOverlay;
            hdr->_isFlat = (byte)(_isFlat ? 0x01 : 0x00);
            hdr->_isFixedCamera = (byte)(_isFixedCamera ? 0x01 : 0x00);
            hdr->_varianceType = _varianceType;
            hdr->_varianceRange = (byte)(_variantNames?.Count ?? 0);

            hdr->FolderName = _folderName;
            hdr->StageName = _stageName;
            hdr->ModuleName = _moduleName;
            hdr->VariantNames = VariantNames;
        }

        public override bool OnInitialize()
        {
            _soundBankID = Header->_soundBankID;
            _effectBankID = Header->_effectBankID;
            _trackListID = Header->_trackListID;
            _rgbaOverlay = Header->_rgbaOverlay;
            _isFlat = Header->_isFlat != 0;
            _isFixedCamera = Header->_isFixedCamera != 0;
            _varianceType = Header->_varianceType;

            _folderName = Header->FolderName;
            _stageName = Header->StageName;
            _moduleName = Header->ModuleName;

            _name = StageName;

            if (Header->_varianceRange > 0)
            {
                bushort* addr = (bushort*) ((VoidPtr) Header + Header->_variantNamesOffset);
                for (int i = 0; i < Header->_varianceRange; i++)
                {
                    _variantNames.Add(new string((sbyte*) addr));
                    addr += VariantNames.Last().Length + 1;
                }
            }

            return false;
        }

        internal static ResourceNode TryParse(DataSource source)
        {
            return ((STEX*) source.Address)->_tag == STEX.Tag ? new StageParamNode() : null;
        }
    }
}
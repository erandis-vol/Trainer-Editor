using System.Collections.Generic;

namespace Hopeless
{
    public class Trainer
    {
        public int Index { get; }

        public byte Class;
        public byte Gender;
        public byte Music;
        public byte Sprite;
        public string Name = string.Empty;
        public bool HasHeldItems;
        public ushort[] Items = new ushort[4];
        public bool DoubleBattle;
        //public byte[] Padding = new byte[3];
        public uint AI;

        public bool HasCustomAttacks;
        public int PartyOffset;
        public List<Pokemon> Party = new List<Pokemon>();

        public Trainer(int index)
        {
            Index = index;
        }

        /// <summary>
        /// The size of this <see cref="Trainer"/> in bytes. 
        /// </summary>
        public int Size
        {
            get { return 40; }
        }

        /// <summary>
        /// The original size of this <see cref="Trainer"/>'s party in bytes.
        /// </summary>
        public int OriginalPartySize { get; set; } = 0;

        /// <summary>
        /// The size of this <see cref="Trainer"/>'s party in bytes.
        /// </summary>
        public int PartySize
        {
            get
            {
                return Party.Count * (HasCustomAttacks ? 16 : 8);
            }
        }

        /// <summary>
        /// Gets whether this <see cref="Trainer"/>'s party requires repointing to safely save.
        /// </summary>
        public bool RequiresRepoint
        {
            get { return PartySize > OriginalPartySize || (PartyOffset == 0 && Party.Count != 0); }
        }
    }

    public class Pokemon
    {
        public Pokemon(int index)
        {
            Index = index;
        }

        public int Index { get; set; }

        public ushort EVs { get; set; } = 0;
        public ushort Species { get; set; } = 0;
        public ushort Level { get; set; } = 0;
        public ushort HeldItem { get; set; } = 0;
        public ushort[] Attacks { get; } = new ushort[4];
    }
}

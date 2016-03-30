using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTE.GBA
{
    public class Trainer
    {
        // variables
        public byte Class;
        public byte Gender;
        public byte Music;
        public byte Sprite;
        public string Name;
        public bool HasHeldItems;
        public ushort[] HeldItems;
        public bool DoubleBattle;
        public uint AI;

        public bool HasCustomAttacks;
        public uint PartyOffset;
        public List<Pokemon> Party;

        public Trainer()
        {
            Class = 0;
            Gender = 0;
            Music = 0;
            Sprite = 0;
            Name = "TRAINER";
            HasHeldItems = true;
            HeldItems = new ushort[4];
            HeldItems[0] = 0;
            HeldItems[1] = 0;
            HeldItems[2] = 0;
            HeldItems[3] = 0;
            DoubleBattle = false;
            AI = 0;

            HasCustomAttacks = true;
            PartyOffset = 0;
            Party = new List<Pokemon>();
            //Party.Add(new Pokemon());
        }
    }

    public class Pokemon
    {
        public ushort IVs;
        public ushort Species;
        public ushort Level;
        public ushort HeldItem;
        public ushort[] Attacks;

        public Pokemon()
        {
            Species = 0;
            Level = 0;
            HeldItem = 0;
            Attacks = new ushort[4];
            Attacks[0] = 0;
            Attacks[1] = 0;
            Attacks[2] = 0;
            Attacks[3] = 0;
        }
    }
}

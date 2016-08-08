using System.Collections.Generic;

namespace HTE.GBA
{
    public class Trainer
    {
        public byte Class;
        public byte Gender;
        public byte Music;
        public byte Sprite;
        public string Name = string.Empty;
        public bool HasHeldItems;
        public ushort[] HeldItems = new ushort[4];
        public bool DoubleBattle;
        public uint AI;
        public byte InitialPartySize;

        public bool HasCustomAttacks;
        public uint PartyOffset;
        public List<Pokemon> Party = new List<Pokemon>();

        /*public Trainer()
        {
            Class = 0;
            Gender = 0;
            Music = 0;
            Sprite = 0;
            Name = "TRAINER";
            HasHeldItems = true;
            HeldItems = new ushort[4] { 0, 0, 0, 0 };
            DoubleBattle = false;
            AI = 0;

            HasCustomAttacks = true;
            PartyOffset = 0;
            Party = new List<Pokemon>();
        }*/
    }

    public class Pokemon
    {
        public ushort IVs;
        public ushort Species;
        public ushort Level;
        public ushort HeldItem;
        public ushort[] Attacks = new ushort[4];
    }

    public static class ROMExtensions
    {
        public static Trainer ReadTrainer(this ROM rom)
        {
            var trainer = new Trainer();

            byte flags = rom.ReadByte();
            trainer.HasCustomAttacks = ((flags & 1) == 1);
            trainer.HasHeldItems = ((flags & 2) == 2);

            trainer.Class = rom.ReadByte();
            byte genderMusic = rom.ReadByte();
            trainer.Gender = (byte)((genderMusic & 128) >> 7);
            trainer.Music = (byte)(genderMusic & 127);

            trainer.Sprite = rom.ReadByte();
            trainer.Name = TextTable.GetEnglishString(rom.ReadBytes(12));
            for (int i = 0; i < 4; i++) // If HasHeldItems == false, should all be 0
                trainer.HeldItems[i] = rom.ReadUInt16();

            trainer.DoubleBattle = (rom.ReadByte() == 1);
            rom.Skip(3);

            trainer.AI = rom.ReadUInt32();

            int partySize = rom.ReadByte();

            rom.Skip(3);
            trainer.PartyOffset = (uint)rom.ReadPointer();

            // read Trainer's party
            // TODO: remove this from here
            int p = rom.Position;
            if (partySize > 0 && trainer.PartyOffset > 0)
            {
                rom.Seek((int)trainer.PartyOffset);
                for (int i = 0; i < partySize; i++)
                    trainer.Party.Add(rom.ReadPokemon(trainer.HasHeldItems, trainer.HasCustomAttacks));
            }
            rom.Seek(p);

            return trainer;
        }

        public static Pokemon ReadPokemon(this ROM rom, bool hasHeldItems, bool hasCustomAttacks)
        {
            var pkmn = new Pokemon();
            pkmn.IVs = rom.ReadUInt16();
            pkmn.Level = rom.ReadUInt16();
            pkmn.Species = rom.ReadUInt16();

            if (hasHeldItems)
                pkmn.HeldItem = rom.ReadUInt16();

            if (hasCustomAttacks)
                for (int a = 0; a < 4; a++)
                    pkmn.Attacks[a] = rom.ReadUInt16();

            if (!hasHeldItems)
                rom.ReadUInt16();

            return pkmn;
        }
    }
}

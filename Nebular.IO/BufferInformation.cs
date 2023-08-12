using System.Collections.Generic;
using Nebular.IO.Interfaces;


namespace Nebular.IO
{ 
    public class BufferInformation  
    { 
        public const ushort Id = 1328;
        public virtual ushort TypeId => Id;

        public long id;
        public long amount;

        public BufferInformation()
        {
        }
        public BufferInformation(long id,long amount)
        {
            this.id = id;
            this.amount = amount;
        }
        public virtual void Serialize(IDataWriter writer)
        {
            if (id < 0 || id > 9.00719925474099E+15)
            {
                throw new System.Exception("Forbidden value (" + id + ") on element id.");
            }

            writer.WriteVarLong((long)id);
            if (amount < 0 || amount > 9.00719925474099E+15)
            {
                throw new System.Exception("Forbidden value (" + amount + ") on element amount.");
            }

            writer.WriteVarLong((long)amount);
        }
        public virtual void Deserialize(IDataReader reader)
        {
            id = (long)reader.ReadVarUhLong();
            if (id < 0 || id > 9.00719925474099E+15)
            {
                throw new System.Exception("Forbidden value (" + id + ") on element of BufferInformation.id.");
            }

            amount = (long)reader.ReadVarUhLong();
            if (amount < 0 || amount > 9.00719925474099E+15)
            {
                throw new System.Exception("Forbidden value (" + amount + ") on element of BufferInformation.amount.");
            }

        }


    }
}









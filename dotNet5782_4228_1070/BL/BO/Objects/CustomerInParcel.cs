using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;



namespace BO
{
    public class CustomerInParcel //targetId in parcel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return ($"Id: {Id} , Name: {Name} \n");
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.Models
{

    public class State
    {
        public int StateId { get; set; } // Primary Key
        public string Name { get; set; } // Name of the State

        // Navigation property
        public virtual ICollection<City> Cities { get; set; } // Cities in the State

        public State()
        {
            Cities = new HashSet<City>();
        }
    }

    public class City
    {
        public int CityId { get; set; } // Primary Key
        public string Name { get; set; } // Name of the City
        public int StateId { get; set; } // Foreign Key

        // Navigation property
        public virtual State State { get; set; } // State to which the City belongs
    }

}

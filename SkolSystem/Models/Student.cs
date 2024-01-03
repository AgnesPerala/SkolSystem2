using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkolSystem.Models;

public class Student
{
    // kolumnet i table student
    public int StudentID { get; } // primery key 
    public int KlassID { get; set; } // foriegn key 
    public string SFörnamn { get; set; }
    public string SEfternamn { get; set; }

    public Klass Klass { get; set; } // pekar på 
    public List<Betyg> Betyg { get; set; } = new List<Betyg>(); // pekas av 
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkolSystem.Models;

public class Personal
{
    // kolumner för table Personal 
    public int PersonalID { get; } // primery key 
    public string Befattning { get; set; }
    public string PFörnamn { get; set; }
    public string PEfternamn { get; set; }
    public string AnställningsDatum { get; set; }
    public int Lön { get; set; }

    public List<Kurs> Kurser { get; set; } = new List<Kurs>(); // pekas av 

    public List<Betyg> Betyg { get; set; } = new List<Betyg>(); // pekas av
}

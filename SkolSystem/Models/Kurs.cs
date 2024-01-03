using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkolSystem.Models;

public class Kurs
{
    // kolumner i table Kurs
    public int KursID { get; } // primery key 
    public string KursNamn { get; set; }
    public int PersonalID { get; set; } // foriegn key


    public Personal Lärare { get; set; } // pekar på  

    public List<Betyg> Betyg { get; set; } = new List<Betyg>(); // pekas av

}

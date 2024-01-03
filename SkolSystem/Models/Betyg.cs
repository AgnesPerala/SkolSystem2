using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkolSystem.Models;

public class Betyg
{
    // kolumner i tabellen betyg
    public int KursID { get; set; } // composit key (foreign key)
    public DateTime Datum { get; set; }
    public int StudentID { get; set; } // composit key (foreign key)
    public int PersonalID { get; set; } // foreign key
    public int SlutBetyg {  get; set; }

    public Kurs Kurs { get; set; } // pekar på
    public Student Student { get; set; } // pekar på 
    public Personal Lärare { get; set; } // pekar på 
}

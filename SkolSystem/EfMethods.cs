using Microsoft.EntityFrameworkCore;
using SkolSystem.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SkolSystem;

public class EfMethods
{
    public void GetStudents()
    {
        using (var context = new MyDbContext())
        {
            // hämtar lista av table student 
            List<Student> allStudents = context.Student.ToList();

            // hämtar lista av table klass 
            List<Klass> allKlasser = context.Klass.ToList();

            // skriver ut alla studenter med foreach 
            foreach (Student student in allStudents)
            {
                Console.WriteLine($"id: {student.StudentID}, namn: {student.SFörnamn} {student.SEfternamn}, klass: {student.Klass.KlassNamn}");
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            }
        }
    }
    public void GetStudentsFromClass()
    {
        using (var context = new MyDbContext())
        {
            // hämtar lista med alla studenter
            List<Student> allStudents = context.Student.ToList();
            // hämtar lista med alla klasser
            List<Klass> allKlasser = context.Klass.ToList();

            // metod som ber användaren skriva in en klass
            int valdKlassId = GetValdKlassID(allKlasser);

            foreach (Student student in allStudents)
            {
                // kollar om klassID stämmer överens med den användaren valt och skriver då ut de som går i den klassen
                if (student.KlassID == valdKlassId)
                {
                    Console.WriteLine($"id: {student.StudentID}, namn: {student.SFörnamn} {student.SEfternamn}, klass: {student.Klass.KlassNamn}");
                    Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                }
            }
        }
    }

    private int GetValdKlassID(List<Klass> allKlasser)
    {
        Console.WriteLine("de klasser som finns är: ");
        // skriver ut klasserna 
        foreach (Klass klass in allKlasser)
        {
            Console.Write($"{klass.KlassNamn}, ");
        }
        string valdKlass = "";
        bool klassExists = false;
        int valdKlassId = 0;
        while (!klassExists)
        {
            Console.Write("välj en av klasserna: ");
            valdKlass = Console.ReadLine();

            klassExists = allKlasser.Any(k => k.KlassNamn == valdKlass);
            if (klassExists) // if stats om det användaren skriver in stämmer överens med de klasser som finns 
            {

                Klass hittadKlass = allKlasser.FirstOrDefault(k => k.KlassNamn == valdKlass);

                valdKlassId = hittadKlass.KlassID;
            }
        }

        return valdKlassId;
    }

    public void AddNewEmlpoyee()
    {
        // användaren skriver in 
        Console.Write("skriv in förnam på anstälda: ");
        string fornamn = Console.ReadLine();

        Console.Write("skriv in efternamn: ");
        string efternamn = Console.ReadLine();

        Console.Write("vilken roll har den anstälda: ");
        string befattning = Console.ReadLine();

        // skapar objekt av personal 
        Personal personal = new Personal()
        {
            Befattning = befattning,
            PFörnamn = fornamn,
            PEfternamn = efternamn

        };
        try 
        {
            using (var context = new MyDbContext())
            {
                // lägger till den nya anställda 
                context.Personal.Add(personal);
                context.SaveChanges(); // sparar den i data basen 
                Console.WriteLine($"namn: {fornamn} {efternamn}, befattning: {befattning}");
            }
        }
        catch
        {
            Console.WriteLine("misslyckades med att lägga till den anställda");
        }

    }

    internal void GetAllCourse()
    {
        using (var context = new MyDbContext())
        {
            // hämtar lista av table kurs 
            List<Kurs> kurser = context.Kurs.ToList();

            Console.WriteLine("-----kurser som finns på skolan-----\n");

            // skriver ut namn på kurser med foreach 
            foreach (Kurs kurs in kurser)
            {
                Console.WriteLine($"{kurs.KursNamn}");
            }
        }
    }

    internal void GetPersnoalForEachSection()
    {
        using (var context = new MyDbContext())
        {
            // hämtar lista av tables personal
            List<Personal> personals = context.Personal.ToList();

            Console.WriteLine("1. visa alla lärare");
            Console.WriteLine("2. visa alla rektorer");
            Console.WriteLine("3. visa alla administratörer");

            // användaren får välja vilken anställning den vill se
            string vald = Console.ReadLine();

            IEnumerable<Personal> valdpersonal = null;
            int antalPersonal = 0;

            switch (vald)
            {
                case "1":
                    // lamda funktion för att hitta alla lärare i listan
                    valdpersonal = personals.Where(p => p.Befattning == "lärare");
                    // lamda funktion för att räkna antalet antställda 
                    antalPersonal = personals.Count(p => p.Befattning == "lärare");
                    break;
                case "2":
                    // lamda funktion för att hitta alla rekotrer i listan
                    valdpersonal = personals.Where(p => p.Befattning == "rektor");
                    // lamda funktion för att räkna antalet antställda 
                    antalPersonal = personals.Count(p => p.Befattning == "rektor");
                    break;
                case "3":
                    // lamda funktion för att hitta alla administratörer i listan
                    valdpersonal = personals.Where(p => p.Befattning == "administratör");
                    // lamda funktion för att räkna antalet antställda 
                    antalPersonal = personals.Count(p => p.Befattning == "administratör");
                    break;
            }

            // if-sats om den valda katergorin inte är tom
            if (valdpersonal != null)
            {
                Console.WriteLine($"\nantal personal under denna anställning: {antalPersonal}");

                // skriver ut personalen från dne valda karegorin
                foreach (Personal personal in valdpersonal)
                {
                    Console.WriteLine($"ID: {personal.PersonalID}, yrke: {personal.Befattning}, Namn: {personal.PFörnamn} {personal.PEfternamn}");
                    
                }
            }
        }
        
    }
}

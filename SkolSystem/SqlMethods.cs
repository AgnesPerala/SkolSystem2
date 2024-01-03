using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using SkolSystem.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static System.Collections.Specialized.BitVector32;
using System.Linq.Expressions;

namespace SkolSystem;

public class SqlMethods
{
    private string connectionString;
    public SqlMethods()
    {
        // connection string till min lokala databas
        connectionString = "Data Source=DESKTOP-UKFB1CL\\MSSQLSERVER01;Initial Catalog=Skolsystem2;Integrated Security=True;TrustServerCertificate=True;";
    }

    public void GetPersonal()
    {
        string query = "SELECT * FROM Personal"; // query kod som hämtar all personal 

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read()) 
                    {
                        // skriver ut den hämtade informationen
                        string text = $"ID: {reader["PersonalID"]}, yrke: {reader["Befattning"]}, Namn: {reader["PFörnamn"]} {reader["PEfternamn"]}, anställnings datum: {reader["AnställningsDatum"]}, lön: {reader["Lön"]}";
                        Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                        Console.WriteLine(text);
                    }
                }
            }

            connection.Close();
        }
    }
    public void GetLatestGrades()
    {
        // query som hämtar betyg från senaste månaden
        string query = "SELECT Kurs.KursNamn, Betyg.Datum, student.SFörnamn, Student.SEfternamn , Betyg.SlutBetyg, Personal.PFörnamn, Personal.PEfternamn FROM (((Betyg INNER JOIN Personal on Betyg.PersonalID=Personal.PersonalID) INNER JOIN Student on Betyg.StudentID=Student.StudentID) INNER JOIN Kurs on betyg.KursID=Kurs.KursID) WHERE Datum >= DATEADD(MONTH, -1, GETDATE());";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string text = $"{reader["SFörnamn"]} {reader["SEfternamn"]} fick betyget {reader["SlutBetyg"]} i kursen {reader["KursNamn"]} av läraren: {reader["PFörnamn"]} {reader["PEfternamn"]} den {((DateTime)reader["Datum"]).ToString("yy-MM-dd")}";
                        Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                        Console.WriteLine(text);
                    }
                }
            }

            connection.Close();
        }
    }
    public void GetCourseStatistic()
    {
        // query som visar kurs statistic 
        string query = "SELECT Kurs.KursNamn, AVG(Betyg.SlutBetyg) AS Snittbetyg, MAX(Betyg.SlutBetyg) AS HogstaBetyg, MIN(Betyg.SlutBetyg) AS LagstaBetyg FROM Kurs JOIN Betyg ON Kurs.KursID = Betyg.KursID GROUP BY Kurs.KursNamn;";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(query, connection)) // läser av queryn 
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string text = $"kurs: {reader["KursNamn"]}, snittbetyg: {reader["Snittbetyg"]}, högsta betyg: {reader["HogstaBetyg"]}, lägsta betyg: {reader["LagstaBetyg"]}\t";
                        Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                        Console.WriteLine(text);
                    }
                }
            }

            connection.Close();
        }
    }
    public void AddNewStudent()
    {
        // query som hämtar alla table från klassen
        string query = "SELECT * FROM Klass;";
        List<string> allKlassNamn = new List<string>();
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    Console.WriteLine("De klasser som finns är: ");
                    while (reader.Read())
                    {
                        // skriver ut vilka klasser som finns i klass tablet 
                        Console.Write($"{reader["KlassNamn"]}, ");
                        allKlassNamn.Add(reader["KlassNamn"].ToString()); // och sparar det i en lista 
                    }
                }
            }

            connection.Close();
        }

        bool klassExists = false;
        string valdKlass = "";
        while (!klassExists) // loop för att programmet inte ska kracha om användaer skriver in fel
        {
            Console.Write("välj id på klassen: ");
            valdKlass = Console.ReadLine();

            // kollar om användarens svar stämmer överens om de som finns i listan
            if (allKlassNamn.Contains(valdKlass))
            {
                klassExists = true;
            }
        }

        int klassId = GetKlassIdFromKlassNamn(valdKlass);

        // användaren skriver in namn på nya eleven
        Console.Write("skriv in förnam på eleven: ");
        string fornamn = Console.ReadLine();

        Console.Write("skriv in efternamn: ");
        string efternamn = Console.ReadLine();

        // query som lägger till den nya studenten
        query = $"INSERT INTO Student(SFörnamn, SEfternamn, KlassID) VALUES ('{fornamn}','{efternamn}',{klassId});";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0) // kollar om den läggs till annrs skrivs felmedelande ut 
                {
                    Console.WriteLine("Eleven är nu tillagd");
                    Console.WriteLine($" namn: {fornamn} {efternamn}, klass: {valdKlass}");
                }
                else
                {
                    Console.WriteLine("Något gick fel. Eleven kunde inte läggas till.");
                }
            }

            connection.Close();
        }
    }

    private int GetKlassIdFromKlassNamn(string valdKlass)
    {
        // query som hämtar klassnamn med klass id 
        string query = $"SELECT Klass.KlassID FROM Klass WHERE KlassNamn = '{valdKlass}';";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // returnerar klassid 
                        return int.Parse(reader["KlassID"].ToString());
                    }
                }
            }
            connection.Close();
        }
        throw new Exception("klassen fanns inte som du valt");
    }

    public void GetTotalMonthlyPayPerSection()
    {

        List<string> sections = GetDistinctSections(); // sätter metoden till en lista
        // loopar igenom alla löner i varje avdelning
        foreach (string section in sections)
        {
            // query som visar summerar allas lön i olika avdelningar
            string query = $"SELECt SUM(personal.Lön) as TotalSalary from Personal WHere personal.Befattning = '{section}';";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection)) // läser av queryn 
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string text = $" totala lönen för alla {section} är {reader["TotalSalary"]} i månaden.";
                            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                            Console.WriteLine(text);
                        }
                    }
                }

                connection.Close();
            }
        }
        
    }

    private List<string> GetDistinctSections() // metod som delar upp varje avdelning 
    {
        List<string> list = new List<string>();

        string query = "SELECT DISTINCT Personal.Befattning FROM Personal;";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(query, connection)) // läser av queryn 
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(reader["Befattning"].ToString());

                    }
                }
            }

            connection.Close();
        }

        return list; 

    }

    public void GetAvgSalaryPerSection() // metod som räknar ut medellönen på alla avdelningar 
    {
        List<string> sections = GetDistinctSections(); // metoden till en lista 

        // loopar igenom varje lön i en avdelning 
        foreach (string section in sections) 
        {
            // query som räknar ut medelölen 
            string query = $"SELECt avg(personal.Lön) as avgSalary from Personal WHere personal.Befattning = '{section}';";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection)) // läser av queryn 
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string text = $" medellönen för alla {section} är {reader["avgSalary"]} i månaden.";
                            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                            Console.WriteLine(text);
                        }
                    }
                }

                connection.Close();
            }
        }
    }

    

    public void GetStudentById() // metod som hämtar information när ett id skrivs in 
    {
        int val = ChooseStudentId(); // kallar på metoden som väljer ett id 

        // 
        string storedProcedureName = "GetStudentById";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Id", val);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string text = $"ID: {reader["StudentID"]}, KlassId: {reader["KLassID"]}, Namn: {reader["SFörnamn"]} {reader["SEfternamn"]}";
                        Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                        Console.WriteLine(text);
                    }
                }
            }

            connection.Close();
        }
    }

    
    private List<int> GetAllStudentId() // metod som hämtar alla student id 
    {
        List<int> studentIdList = new List<int>(); // skapar en lista med alla id 

        string storedProcedureName = "GetAllStudentId";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        studentIdList.Add((int)reader["StudentID"]); // lägger till alla id i listan 
                    }

                }
            }

            connection.Close();
        }

        studentIdList.Sort(); // sorterar i nummer ordning   
        return studentIdList;

    }

    public void SetGrade() // metod för att sätta betyg på en elev
    {
        int studentID = ChooseStudentId(); // kallar på metoden att välja ett Id 
        int kursID = ChooseKursId(); // kallar på metoden att välja ett kurs Id
        string datum = DateTime.Now.ToString("yyyy-MM-dd"); // väljer aoutomatiskt dagens datum
        int slutBetyg = ChooseGrade(); // metod för att sätta ett betyg 
        int personalID = ChoosePersonalId(); // metod för att välja personal Id


        string transaction = "transactionName";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlTransaction sqlTransaction = connection.BeginTransaction(); // startar transaktion

            try
            {
                using (SqlCommand command = new SqlCommand(transaction, connection, sqlTransaction))
                {
                    // query för att ändra betyd på en elev
                    command.CommandText = $"INSERT INTO Betyg (KursID, Datum, StudentID, SlutBetyg, PersonalID) VALUES ({kursID}, '{datum}', {studentID}, {slutBetyg}, {personalID});";
                    command.ExecuteNonQuery();

                    sqlTransaction.Commit();

                    Console.WriteLine($" ett nytt betyg har lagts till på student ID; {studentID}, betyg {slutBetyg}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                sqlTransaction.Rollback(); // stoppar alla ändringar om något går fel
            }
            finally
            {
                connection.Close();
            }

        }
    }

    private int ChooseGrade() // metod som låter en välja betyg 
    {
        Console.WriteLine("välj ett betyg mellan 0 och 100: ");

        int valtBetyg = -1;
        while (true)
        {
            try
            {
                valtBetyg = int.Parse(Console.ReadLine());
                if(valtBetyg >= 0 && valtBetyg <= 100) // retrunerar om användaren skrivit in rätt 
                {
                    return valtBetyg;
                }
                else
                {
                    Console.WriteLine("du måste skriva ett tal mellan 0 och 100");
                }
            }
            catch
            {
                Console.WriteLine("du måste skriva ett tal mellan 0 och 100");
            }
        }
    }

    private int ChooseStudentId() // metod som låter en välja ett befintligt id
    {

        List<int> validStudentIdList = GetAllStudentId(); // skapar lista på alla student id 

        Console.WriteLine($"Student id som finns på skolan: ");
        foreach (int studentId in validStudentIdList) // loopar alla id
        {
            Console.Write($"{studentId}, ");
        }

        Console.WriteLine("\nvälj ett id: ");
        int val = 0;
        while (true)
        {

            try
            {
                val = int.Parse(Console.ReadLine());
                if (validStudentIdList.Contains(val)) // kollar om användares svar finns i listan 
                {
                    break;
                }
                else
                {
                    Console.Write("skriv in ett ID som finn i skolan: ");
                }
            }
            catch
            {
                Console.Write("skriv in ett ID som finn i skolan: ");
            }

        }

        return val;
    }

    private int ChooseKursId()
    {

        List<int> valiKursIdList = GetAllKursId(); // skapar lista på alla kurs Id

        Console.WriteLine($"Kurs id som finns på skolan: ");
        foreach (int kursId in valiKursIdList) // loopar alla id 
        {
            Console.Write($"{kursId}, ");
        }

        Console.WriteLine("\nvälj ett id: ");
        int val = 0;
        while (true)
        {

            try
            {
                val = int.Parse(Console.ReadLine());
                if (valiKursIdList.Contains(val)) // kollar om val finns i listan 
                {
                    break;
                }
                else
                {
                    Console.Write("skriv in ett ID som finn i skolan: ");
                }
            }
            catch
            {
                Console.Write("skriv in ett ID som finn i skolan: ");
            }

        }

        return val;
    }

    private List<int> GetAllKursId() // metod som hämtar alla kursID
    {
        List<int> kursIdList = new List<int>();

        string query = "select Kurs.KursID from Kurs";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(query, connection))
            {

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        kursIdList.Add((int)reader["KursID"]);
                    }

                }
            }

            connection.Close();
        }

        kursIdList.Sort(); // sorterar i nummer ordning 
        return kursIdList;
    }


    private int ChoosePersonalId() // metod som låter en välja ett personal id
    {

        List<int> valiPersonalIdList = GetAllPersonalId(); // lista med alla id 

        Console.WriteLine($"Personal id som finns på skolan: ");
        foreach (int personalId in valiPersonalIdList) // loopar alla id 
        {
            Console.Write($"{personalId}, ");
        }

        Console.WriteLine("\nvälj ett id: ");
        int val = 0;
        while (true)
        {

            try
            {
                val = int.Parse(Console.ReadLine());
                if (valiPersonalIdList.Contains(val)) // kollar om val finns i listan 
                {
                    break;
                }
                else
                {
                    Console.Write("skriv in ett ID som finn i skolan: ");
                }
            }
            catch
            {
                Console.Write("skriv in ett ID som finn i skolan: ");
            }

        }

        return val;
    }

    private List<int> GetAllPersonalId() // metod som hämtar all personalid 
    {
        List<int> persoanlIdList = new List<int>();

        string query = "select Personal.PersonalID from Personal";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(query, connection))
            {

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        persoanlIdList.Add((int)reader["PersonalID"]);
                    }

                }
            }

            connection.Close();
        }

        persoanlIdList.Sort(); // sorterar i nummer ordning 
        return persoanlIdList;
    }
}

using System;
using System.Collections.Generic;


//using System.Data.SqlClient;


using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;



namespace FirstApp
{
    internal class Program
    {
        //static string connectionString = "Server=DESKTOP-S4OLS6R\\SQLEXPRESS;Persist Security Info=False;initial catalog=StudentMarksDB; Encrypt=False; User Id=; Password=;TrustServerCertificate=True;";
        static string connectionString = "Data Source=DESKTOP-S4OLS6R\\SQLEXPRESS;Initial Catalog=StudentMarksDB;Integrated Security=True;Trust Server Certificate=True";

        static void Main(string[] args)
        {

            decimal[] marks = new decimal[10];

            decimal total = 0;

            Console.Write("Enter your name: ");
            string name = Console.ReadLine();

            for (int i = 0; i < marks.Length; i++)
            {

                Console.Write("Enter your " + (i + 1) + " Mark: ");
                marks[i] = decimal.Parse(Console.ReadLine());

                total = total + marks[i];
            }
            decimal average = total / marks.Length;

            Console.WriteLine();
            Console.WriteLine("Total = " + total);
            Console.WriteLine("Average = " + average);

            Console.WriteLine("--------------------------------");

            


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Insert into StudentMarks table
                    string insertStudentQuery = "INSERT INTO StudentMarks (Name, Total, Average) OUTPUT INSERTED.SID VALUES (@Name, @Total, @Average)";
                    int studentId;

                    using (SqlCommand studentCommand = new SqlCommand(insertStudentQuery, connection, transaction))
                    {
                        studentCommand.Parameters.AddWithValue("@Name", name);
                        studentCommand.Parameters.AddWithValue("@Total", total);
                        studentCommand.Parameters.AddWithValue("@Average", average);

                        studentId = (int)studentCommand.ExecuteScalar(); 
                    }

                    string insertMarksQuery = "INSERT INTO Marks (MSID, Mark) VALUES (@MSID, @Mark)";
                    using (SqlCommand marksCommand = new SqlCommand(insertMarksQuery, connection, transaction))
                    {
                        marksCommand.Parameters.AddWithValue("@MSID", studentId);
                        marksCommand.Parameters.Add("@Mark", System.Data.SqlDbType.Float);

                        foreach (var mark in marks)
                        {
                            marksCommand.Parameters["@Mark"].Value = mark;
                            marksCommand.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit(); 

                    Console.WriteLine("Data inserted successfully.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); 
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }




                Console.WriteLine();
                Console.Write("Enter your sudent ID: ");
                int sid = int.Parse(Console.ReadLine());
                

                Console.WriteLine();


                string queryf = "SELECT SID, Name, Total, Average FROM StudentMarks WHERE SID = @SID";


                using (SqlCommand command = new SqlCommand(queryf, connection))
                {
                    command.Parameters.AddWithValue("@SID", sid);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {

                                Console.WriteLine("SID: " + reader["SID"]);
                                Console.WriteLine("Name: " + reader["Name"]);
                                Console.WriteLine("Total: " + reader["Total"]);
                                Console.WriteLine("Average: " + reader["Average"]);
                                Console.WriteLine();
                            }
                        }

                        else
                        {
                            Console.WriteLine("No records found for SID = " + sid);
                        }
                    }

                }

                string fetchMarksQuery = "SELECT Mark FROM Marks WHERE MSID = @MSID";
                using (SqlCommand fetchMarksCommand = new SqlCommand(fetchMarksQuery, connection))
                {
                    fetchMarksCommand.Parameters.AddWithValue("@MSID", sid);
                    using (SqlDataReader marksReader = fetchMarksCommand.ExecuteReader())
                    {
                        Console.WriteLine("Marks:");
                        while (marksReader.Read())
                        {
                            Console.WriteLine(marksReader["Mark"]);
                        }
                    }
                }




                Console.WriteLine("--------------------------------");

                string queryn = "SELECT Name FROM StudentMarks";

                List<string> allNames = new List<string>();

                using (SqlCommand command = new SqlCommand(queryn, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            allNames.Add(reader["Name"].ToString());


                        }

                    }

                    Console.WriteLine("Names:");
                    Console.WriteLine();

                    foreach (var names in allNames)
                    {
                        Console.WriteLine(names);
                    }

                }

                Console.WriteLine("--------------------------------");

                Student studentManager = new Student(" ", new List<float>(), 0, 0);



                studentManager.AddStudent(connectionString);

                studentManager.Display();

                


                Console.WriteLine("Finished...");
                Console.ReadKey();


                Console.ReadLine();
            }
        }
    }
}


              

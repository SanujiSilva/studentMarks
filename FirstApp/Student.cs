using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace FirstApp
{
    public class Student
    {
        private string name;
        private List<float> marks; 
        private float total;
        private float average;

        public Student(string name, List<float> marks, float total, float average)
        {
            this.name = name;
            this.marks = marks;
            this.total = total;
            this.average = average;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public List<float> Marks
        {
            get { return marks; }
            set { marks = value; }
        }

        public float Total
        {
            get { return total; }
            set { total = value; }
        }

        public float Average
        {
            get { return average; }
            set { average = value; }
        }

        private List<Student> students = new List<Student>();

        public void AddStudent(string connectionString)
        {
            if (students == null)
                students = new List<Student>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Fetch student details
                    string queryStudent = "SELECT SID, Name, Total, Average FROM StudentMarks";
                    using (SqlCommand commandStudent = new SqlCommand(queryStudent, connection))
                    {
                        using (SqlDataReader readerStudent = commandStudent.ExecuteReader())
                        {
                            while (readerStudent.Read())
                            {
                                int sid = Convert.ToInt32(readerStudent["SID"]);
                                string name = readerStudent["Name"].ToString();
                                float total = Convert.ToSingle(readerStudent["Total"]);
                                float average = Convert.ToSingle(readerStudent["Average"]);

                                List<float> marks = GetMarksBySID(connectionString, sid);

                                
                                students.Add(new Student(name, marks, total, average));
                            }
                        }
                    }
                }

                //Console.WriteLine("Students successfully added from the database.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private List<float> GetMarksBySID(string connectionString, int sid)
        {
            List<float> marks = new List<float>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string queryMarks = "SELECT Mark FROM Marks WHERE MSID = @MSID";
                using (SqlCommand commandMarks = new SqlCommand(queryMarks, connection))
                {
                    commandMarks.Parameters.AddWithValue("@MSID", sid);

                    using (SqlDataReader readerMarks = commandMarks.ExecuteReader())
                    {
                        while (readerMarks.Read())
                        {
                            marks.Add(Convert.ToSingle(readerMarks["Mark"]));
                        }
                    }
                }
            }

            return marks;
        }

        public void Display()
        {
            Console.WriteLine("List of Students:");
            Console.WriteLine();
            Console.WriteLine();

            foreach (var student in students)
            {
                
                string marksString = string.Join(", ", student.Marks);

                Console.WriteLine($"Name: {student.Name}, Marks: {marksString}, Total: {student.Total}, Average: {student.Average}");
                Console.WriteLine();
            }
        }

    }
}

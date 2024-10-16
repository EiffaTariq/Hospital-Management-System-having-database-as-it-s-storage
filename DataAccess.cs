using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Collections;
using System.Numerics;
using System.IO;
using System.ComponentModel.Design;

namespace HospitalDAL
{
    public class DataAccess
    {
        private string connString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=Hospital;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        //input validation function for email
        public static bool IsValidEmail(string email)
        {
            // Check if the email is null or empty
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }
            bool hasAtSymbol = false;
            bool hasDot = false;

            for (int i = 0; i < email.Length; i++)
            {
                // Check for the '@' symbol
                if (email[i] == '@')
                {
                    if (hasAtSymbol) // Check for multiple '@' symbols
                    {
                        return false;
                    }
                    hasAtSymbol = true;
                }

                if (email[i] == '.')
                {
                    hasDot = true;
                }
            }

            return hasAtSymbol && hasDot;
        }

        // Insert Functions

        public void InsertPatient(Patient patient)
        {
            Console.Write("Enter Name: ");
            patient.Name = Console.ReadLine();

            Console.Write("Enter Email: ");
            patient.Email = Console.ReadLine();
            if (IsValidEmail(patient.Email))
            {
                Console.Write("Enter Disease: ");
                patient.Disease = Console.ReadLine();
                using (SqlConnection sqlConnection = new SqlConnection(connString))
                {
                    sqlConnection.Open();
                    string query = "INSERT INTO Patient(name, email, disease) VALUES (@Name, @Email, @Disease)";

                    using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                    {
                        cmd.Parameters.AddWithValue("@Name", patient.Name);
                        cmd.Parameters.AddWithValue("@Email", patient.Email);
                        cmd.Parameters.AddWithValue("@Disease", patient.Disease);

                        try
                        {
                            cmd.ExecuteNonQuery(); // Execute the command
                            Console.WriteLine("Patient inserted successfully.");
                        }
                        catch (Exception Ex)
                        {
                            Console.WriteLine("An error occurred while inserting the patient: ");
                        }
                    }
                }

            }
            else
            {
                while (!IsValidEmail(patient.Email))
                {
                    patient.Email = Console.ReadLine();
                }
            }
        }
        public void InsertDoctor(Doctor doctor)
        {
            using (SqlConnection sc = new SqlConnection(connString))
            {
                sc.Open();

                Console.Write("Enter the doctor name: ");
                doctor.DoctorName = Console.ReadLine();

                Console.Write("Enter the doctor specialization: ");
                doctor.DoctorSpecialization = Console.ReadLine();

                string query = "INSERT INTO Doctor(doctorName, doctorSpecialization) VALUES(@doctorName, @doctorSpecialization)";

                using (SqlCommand cmd = new SqlCommand(query, sc))
                {
                    cmd.Parameters.AddWithValue("@doctorName", doctor.DoctorName);
                    cmd.Parameters.AddWithValue("@doctorSpecialization", doctor.DoctorSpecialization);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Doctor added successfully.");
                    }
                    catch (Exception Ex)
                    {
                        Console.WriteLine("Some error occurred: ");
                    }
                } // No need for finally to close connection,it is already handled by using statement
            }
        }
        //Appointment input validation
        public bool IsAppointmentDateInFuture(DateTime appointmentDate)
        {
            return appointmentDate > DateTime.Now;
        }

        public bool IsDuplicateAppointment(int patientId, DateTime appointmentDate)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();
                string query = "SELECT COUNT(*) FROM Appointment WHERE PatientId = @PatientId AND AppointmentDate = @AppointmentDate";
                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@PatientId", patientId);
                    cmd.Parameters.AddWithValue("@AppointmentDate", appointmentDate);

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0; // True if duplicate exists
                }
            }
        }
        public bool IsSlotAvailable(int doctorId, DateTime appointmentDate)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();
                string query = "SELECT COUNT(*) FROM Appointment WHERE DoctorId = @DoctorId AND AppointmentDate = @AppointmentDate";
                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@DoctorId", doctorId);
                    cmd.Parameters.AddWithValue("@AppointmentDate", appointmentDate);

                    int count = (int)cmd.ExecuteScalar();
                    return count == 0; // True if slot is available
                }
            }
        }

        private bool DoesPatientExist(int patientId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();
                string query = "SELECT COUNT(1) FROM Patient WHERE PatientId = @PatientId";
                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@PatientId", patientId);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0; // True if patient exists
                }
            }
        }


        public bool DoesDoctorExist(int doctorId)
        {
            
            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();
                string query = "SELECT COUNT(1) FROM Doctor WHERE DoctorId = @DoctorId";
                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@DoctorId", doctorId);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0; // True if doctor exists
                }
            }
        }
        public void InsertAppointment(Appointment a)
        {
            Console.Write("Enter Patient ID: ");
            if (!int.TryParse(Console.ReadLine(), out int patientId))
            {
                Console.WriteLine("Error: Invalid Patient ID. Please enter a numeric value.");
                return;
            }
            if (!DoesPatientExist(patientId))
            {
                Console.WriteLine($"Error: No patient found with ID {patientId}.");
                return;
            }
            Console.Write("Enter Doctor ID: ");
            if (!int.TryParse(Console.ReadLine(), out int doctorId))
            {
                Console.WriteLine("Error: Invalid Doctor ID. Please enter a numeric value.");
                return;
            }
            if (!DoesDoctorExist(doctorId))
            {
                Console.WriteLine($"Error: No doctor found with ID {doctorId}.");
                return;
            }
            Console.Write("Enter Appointment Date (yyyy-MM-dd HH:mm): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime appointmentDate))
            {
                Console.WriteLine("Error: Invalid appointment date format.");
                return;
            }
            if (!IsAppointmentDateInFuture(appointmentDate))
            {
                Console.WriteLine("Error: Appointment date must be in the future.");
                return;
            }
            if (IsDuplicateAppointment(patientId, appointmentDate))
            {
                Console.WriteLine("Error: Duplicate appointment for this patient on the same date and time.");
                return;
            }
            if (!IsSlotAvailable(doctorId, appointmentDate))
            {
                Console.WriteLine("Error: This appointment slot is already taken.");
                return;
            }
            string connString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=Hospital;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();
                string query = "INSERT INTO Appointment (PatientId, DoctorId, AppointmentDate) VALUES (@PatientId, @DoctorId, @AppointmentDate)";
                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@PatientId", patientId);
                    cmd.Parameters.AddWithValue("@DoctorId", doctorId);
                    cmd.Parameters.AddWithValue("@AppointmentDate", appointmentDate);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Appointment added successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred while adding the appointment: ");
                    }
                }
            }
        }
        public Patient GetPatientById(int patientId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();
                string query = "SELECT * FROM Patient WHERE PatientId = @PatientId";
                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@PatientId", patientId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Patient
                            {
                                PatientId = (int)reader["PatientId"],
                                Name = (string)reader["Name"],
                                Email = (string)reader["Email"],
                                Disease = (string)reader["Disease"]
                            };
                        }
                    }
                }
            }
            return null; 
        }


        //delete functions
        public void DeletePatientFromDatabase(int patientId)
        {
            string connString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=Hospital;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

            string query = "DELETE FROM Patient WHERE PatientId = @patientId";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PatientId", patientId);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Patient deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Delete failed.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Some error occured: {ex.Message}");
                }
            }
        }
        public void DeleteDoctorFromDatabase(int doctorId)
        {
            string connString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=Hospital;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

            string query = "DELETE FROM Doctor WHERE doctorId = @doctorId";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@DoctorId",doctorId);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Doctor deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Delete failed.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Some error occured: {ex.Message}");
                }
            }
        }

        public Doctor GetDoctorById(int doctorId)
        {
            Doctor doctor = null;

            string query = "SELECT DoctorId, DoctorName, DoctorSpecialization FROM Doctors WHERE DoctorId = @DoctorId";

            using (SqlConnection connection = new SqlConnection(connString))  // Replace with your actual connection string
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DoctorId", doctorId);

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())  // If a record is found
                        {
                            doctor = new Doctor
                            {
                                DoctorId = (int)reader["DoctorId"],
                                DoctorName = reader["DoctorName"].ToString(),
                                DoctorSpecialization = reader["DoctorSpecialization"].ToString()
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while retrieving the doctor: " + ex.Message);
                }
            }

            return doctor;
        }


        public Appointment GetAppointmentById(int appointmentId)
        {
            string connString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=Hospital;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();
                string query = "SELECT AppointmentId, PatientId, DoctorId, AppointmentDate FROM Appointment WHERE AppointmentId = @AppointmentId";
                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Appointment
                            {
                                AppointmentId = (int)reader["AppointmentId"],
                                PatientId = (int)reader["PatientId"],
                                DoctorId = (int)reader["DoctorId"],
                                AppointmentDate = reader.GetDateTime(reader.GetOrdinal("AppointmentDate"))
                            };
                        }
                    }
                }
            }
            return null; // Return null if not found
        }
        public void DeleteAppointmentFromDatabase(int appointmentId)
        {
            string connString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=Hospital;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

            // Retrieve the appointment details to save in history
            Appointment appointmentToDelete = GetAppointmentById(appointmentId); 
            if (appointmentToDelete != null)
            {
                using (SqlConnection sqlConnection = new SqlConnection(connString))
                {
                    sqlConnection.Open();
                    string query = "DELETE FROM Appointment WHERE AppointmentId = @AppointmentId";

                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@AppointmentId", appointmentId);

                        try
                        {
                            int count = sqlCommand.ExecuteNonQuery(); 
                            if (count > 0)
                            {
                                Console.WriteLine("Appointment deleted successfully.");
                            }
                            else
                            {
                                Console.WriteLine("No appointment found with the given ID.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("An error occurred while deleting the appointment: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"No appointment found with ID {appointmentId}. Deletion operation aborted.");
            }
        }

        public void UpdatePatientInDatabase(Patient patient)
        {
            string connString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=Hospital;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();

                string query = "UPDATE patient SET name = @Name, email = @Email, disease = @Disease WHERE patientId = @PatientId";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    Console.Write("Enter patient Id that needs to be updated");
                    string patientId = Console.ReadLine();
                    Console.Write("Enter updated name");
                    string newName = Console.ReadLine();
                    Console.Write("Enter updated email");
                    string newEmail = Console.ReadLine();
                    while (!IsValidEmail(newEmail))
                    {
                        Console.WriteLine("Enter a valid gamil");
                        newName = Console.ReadLine();
                    }
                    Console.Write("Enter updated disease");
                    string newDisease = Console.ReadLine();
                    sqlCommand.Parameters.AddWithValue("@Name", newName);
                    sqlCommand.Parameters.AddWithValue("@Email", newEmail);
                    sqlCommand.Parameters.AddWithValue("@Disease", newDisease);
                    sqlCommand.Parameters.AddWithValue("@PatientId", patientId);
                    try
                    {
                        int rowsAffected = sqlCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Patient updated successfully.");
                        }
                        else
                        {
                            Console.WriteLine("No patient found with the given ID.");
                        }
                    }
                    catch (Exception Ex)
                    {
                        Console.WriteLine("An error occurred while updating the patient: ");
                    }
                }
            }
        }

        public void UpdateDoctorInDatabase(Doctor d)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();

                string query = "UPDATE Doctor SET doctorName = @DoctorName, doctorSpecialization = @DoctorSpecialization WHERE doctorId = @DoctorId";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    Console.Write("Enter doctor ID that needs to be updated: ");
                    string idInput = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(idInput))
                    {
                        Console.WriteLine("Doctor ID cannot be empty.");
                        return; // Exit method if the input is empty
                    }

                    try
                    {
                        d.DoctorId = int.Parse(idInput); 
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid ID format. Please enter a valid number.");
                        return; 
                    }
                    catch (OverflowException)
                    {
                        Console.WriteLine("The number entered is too large. Please enter a valid number.");
                        return; 
                    }

                    Console.Write("Enter updated name: ");
                    d.DoctorName = Console.ReadLine();

                    Console.Write("Enter updated specialization: ");
                    d.DoctorSpecialization = Console.ReadLine();

                    sqlCommand.Parameters.AddWithValue("@DoctorId", d.DoctorId);
                    sqlCommand.Parameters.AddWithValue("@DoctorName", d.DoctorName);
                    sqlCommand.Parameters.AddWithValue("@DoctorSpecialization", d.DoctorSpecialization);

                    try
                    {
                        int rowsAffected = sqlCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Doctor updated successfully.");
                        }
                        else
                        {
                            Console.WriteLine("No doctor found with the given ID.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred while updating the doctor: " + ex.Message);
                    }
                }
            }
        }


        public void UpdateAppointmentInDatabase(Appointment appointment)
        {
            string connString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=Hospital;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();
                Console.Write("Enter appointment ID to update: ");
                appointment.AppointmentId = int.Parse(Console.ReadLine());
                Console.Write("Enter new Patient ID: ");
                appointment.PatientId = int.Parse(Console.ReadLine()); 
                Console.Write("Enter new Doctor ID: ");
                appointment.DoctorId = int.Parse(Console.ReadLine());
                Console.Write("Enter new Appointment Date (yyyy-MM-dd HH:mm): ");
                appointment.AppointmentDate = DateTime.Parse(Console.ReadLine());
                if (IsDuplicateAppointment(appointment.PatientId, appointment.AppointmentDate))
                {
                    Console.WriteLine("A duplicate appointment already exists for this patient at the specified date.");
                    return; 
                }
                if (!DoesDoctorExist(appointment.DoctorId))
                {
                    Console.WriteLine("The specified Doctor ID does not exist. Please enter a valid Doctor ID.");
                    return;
                }

                string query = "UPDATE Appointment SET PatientId = @PatientId, DoctorId = @DoctorId, AppointmentDate = @AppointmentDate WHERE AppointmentId = @AppointmentId";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@AppointmentId", appointment.AppointmentId);
                    sqlCommand.Parameters.AddWithValue("@PatientId", appointment.PatientId);
                    sqlCommand.Parameters.AddWithValue("@DoctorId", appointment.DoctorId);
                    sqlCommand.Parameters.AddWithValue("@AppointmentDate", appointment.AppointmentDate);

                    try
                    {
                        int rowsAffected = sqlCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Appointment updated successfully.");
                        }
                        else
                        {
                            Console.WriteLine("No appointment found with the given ID.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred while updating the appointment: " + ex.Message);
                    }
                }
            }
        }

        //search functions

        public List<Patient> SearchPatientsInDatabase(string name)
        {
            List<Patient> patients = new List<Patient>();
            string connString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=Hospital;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();

                string query = "SELECT patientId, name, email, disease FROM patient WHERE name LIKE @Name";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@Name", "%" + name + "%");
                    try
                    {
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read()) // Read all matching records
                            {
                                Patient patient = new Patient
                                {
                                    PatientId = reader.GetInt32(0),
                                    Name = reader.GetString(1),      
                                    Email = reader.GetString(2),     
                                    Disease = reader.GetString(3)    
                                };
                                patients.Add(patient); // Add to the list
                            }
                        }
                        if (patients.Count > 0)
                        {
                            Console.WriteLine($"{patients.Count} patient(s) found with the name '{name}'.");
                        }
                        else
                        {
                            Console.WriteLine("No patients found with the given name.");
                        }
                    }
                    catch (Exception Ex)
                    {
                        Console.WriteLine("An error occurred while searching for patients: ");
                    }
                }
            }

            return patients;
        }

        public List<Doctor> SearchDoctorsInDatabase(string docName)
        {
            List<Doctor> doctors = new List<Doctor>();
            string connString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=Hospital;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();
                string query = "SELECT doctorId, doctorName, doctorSpecialization FROM Doctor WHERE doctorName LIKE @Name";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@Name", "%" + docName + "%");

                    try
                    {
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Doctor d = new Doctor
                                {
                                    DoctorId = reader.GetInt32(0),               
                                    DoctorName = reader.GetString(1),           
                                    DoctorSpecialization = reader.GetString(2)  
                                };
                                doctors.Add(d); 
                            }
                        }

                        if (doctors.Count > 0)
                        {
                            Console.WriteLine($"{doctors.Count} doctor(s) found with the name '{docName}'.");
                        }
                        else
                        {
                            Console.WriteLine("No doctors found with the given name.");
                        }
                    }
                    catch (Exception Ex)
                    {
                        Console.WriteLine("An error occurred while searching for doctors: ");
                    }
                }
            }

            return doctors;
        }

        public List<Appointment> SearchAppointmentsInDatabase(int patientId, int doctorId)
        {
            List<Appointment> appointments = new List<Appointment>();
            string connString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=Hospital;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();
                string query = "SELECT appointmentId, patientId, doctorId, appointmentDate FROM Appointment WHERE patientId = @PatientId AND doctorId = @DoctorId";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@PatientId", patientId);
                    sqlCommand.Parameters.AddWithValue("@DoctorId", doctorId);

                    try
                    {
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read()) // Read all matching records
                            {
                                Appointment appointment = new Appointment
                                {
                                    AppointmentId = reader.GetInt32(0),      
                                    PatientId = reader.GetInt32(1),          
                                    DoctorId = reader.GetInt32(2),           
                                    AppointmentDate = reader.GetDateTime(3) 
                                };
                                appointments.Add(appointment); 
                            }
                        }

                        if (appointments.Count > 0)
                        {
                            Console.WriteLine($"{appointments.Count} appointment(s) found.");
                        }
                        else
                        {
                            Console.WriteLine("No appointments found with the given patient and doctor IDs.");
                        }
                    }
                    catch (Exception Ex)
                    {
                        Console.WriteLine("An error occurred while searching for appointments: ");
                    }
                }
            }

            return appointments;
        }

        public List<Doctor> GetAllDoctorsFromDatabase()
        {
            List<Doctor> doctors = new List<Doctor>();
            string connString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=Hospital;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();

                string query = "SELECT doctorId, doctorName, doctorSpecialization FROM Doctor";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    try
                    {
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Doctor doctor = new Doctor
                                {
                                    DoctorId = reader.GetInt32(0),                
                                    DoctorName = reader.GetString(1),             
                                    DoctorSpecialization = reader.GetString(2)    
                                };
                                doctors.Add(doctor);
                            }
                        }

                        Console.WriteLine($"{doctors.Count} doctor(s) found.");
                    }
                    catch (SqlException sqlEx)
                    {
                        Console.WriteLine("An error occurred while getting doctors: " + sqlEx.Message);
                    }
                }
            }

            return doctors;
        }
        public List<Appointment> GetAllAppointmentFromDatabase()
        {
            List<Appointment> appointments = new List<Appointment>();
            string connString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=Hospital;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();

                string query = "SELECT appointmentId, patientId, doctorId, appointmentDate FROM Appointment";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    try
                    {
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Appointment appointment = new Appointment
                                {
                                    AppointmentId = reader.GetInt32(0),          
                                    PatientId = reader.GetInt32(1),              
                                    DoctorId = reader.GetInt32(2),               
                                    AppointmentDate = reader.GetDateTime(3)      
                                };
                                appointments.Add(appointment);
                            }
                        }

                        Console.WriteLine($"{appointments.Count} appointment(s) found.");
                    }
                    catch (SqlException sqlEx)
                    {
                        Console.WriteLine("An error occurred while getting appointments: " + sqlEx.Message);
                    }
                }
            }

            return appointments;
        }
        public List<Patient> GetAllPatientsFromDatabase()
        {
            List<Patient> patients = new List<Patient>();

            string conn = @"Data Source=(localdb)\ProjectModels;Initial Catalog=Hospital;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

            using (SqlConnection sqlConnection = new SqlConnection(conn))
            {
                sqlConnection.Open();
                string query = "SELECT patientId, name, email, disease FROM patient";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    try
                    {
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read()) // Loop through the results
                            {
                                Patient patient = new Patient
                                {
                                    PatientId = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Email = reader.GetString(2),
                                    Disease = reader.IsDBNull(3) ? null : reader.GetString(3) // disease with handling for nulls
                                };
                                patients.Add(patient); // Add to the list
                            }
                        }
                    }
                    catch (Exception Ex)
                    {
                        Console.WriteLine("An error occurred while retrieving patients: ");
                    }
                }
            }

            return patients; 
        }

    }
}

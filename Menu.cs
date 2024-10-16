using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace HospitalDAL
{
    public class Menu
    {
        private void ViewAllPatients()
        {
            DataAccess dataAccess = new DataAccess();
            List<Patient> patients = dataAccess.GetAllPatientsFromDatabase();

            if (patients != null && patients.Count > 0)
            {
                Console.WriteLine("\n--- List of Patients ---");
                foreach (var patient in patients)
                {
                    Console.WriteLine($"ID: {patient.PatientId}, Name: {patient.Name}, Email: {patient.Email}, Disease: {patient.Disease}");
                }
            }
            else
            {
                Console.WriteLine("No patients found.");
            }
        }

        private void ViewAllDoctors()
        {
            DataAccess dataAccess = new DataAccess();
            List<Doctor> doctors = dataAccess.GetAllDoctorsFromDatabase();

            if (doctors != null && doctors.Count > 0)
            {
                Console.WriteLine("\n--- List of Doctors ---");
                foreach (var doctor in doctors)
                {
                    Console.WriteLine($"ID: {doctor.DoctorId}, Name: {doctor.DoctorName}, Specialization: {doctor.DoctorSpecialization}");
                }
            }
            else
            {
                Console.WriteLine("No doctors found.");
            }
        }

        private void ViewAllAppointments()
        {
            DataAccess dataAccess = new DataAccess();
            List<Appointment> appointments = dataAccess.GetAllAppointmentFromDatabase();

            if (appointments != null && appointments.Count > 0)
            {
                Console.WriteLine("\n--- List of Appointments ---");
                foreach (var appointment in appointments)
                {
                    Console.WriteLine($"ID: {appointment.AppointmentId}, Patient ID: {appointment.PatientId}, Doctor ID: {appointment.DoctorId}, Date: {appointment.AppointmentDate}");
                }
            }
            else
            {
                Console.WriteLine("No appointments found.");
            }
        }

        private void ViewHistory()
        {
            Console.WriteLine("\n--- View History of Deleted Records ---");
            Console.WriteLine("1. View Deleted Patients");
            Console.WriteLine("2. View Deleted Doctors");
            Console.WriteLine("3. View Deleted Appointments");
            Console.WriteLine("4. Exit");

            int option = int.Parse(Console.ReadLine());

            switch (option)
            {
                case 1:
                    HistoryLogger.ViewHistory<Patient>();
                    break;
                case 2:
                    HistoryLogger.ViewHistory<Doctor>();
                    break;
                case 3:
                    HistoryLogger.ViewHistory<Appointment>();
                    break;
                case 4:
                    return;
            }
        }

        public void UserMenu()
        {
            DataAccess da = new DataAccess();
            Doctor d = new Doctor();
            Patient p = new Patient();
            Appointment a = new Appointment();

            while (true)
            {
                Console.WriteLine("\n--- Hospital Management System ---");
                Console.WriteLine("Enter any of the following options");
                Console.WriteLine("1. Add a new patient");
                Console.WriteLine("2. Update a patient");
                Console.WriteLine("3. Delete a patient (and save deleted record to history)");
                Console.WriteLine("4. Search for patients by name");
                Console.WriteLine("5. View all patients");
                Console.WriteLine("6. Add a new doctor");
                Console.WriteLine("7. Update a doctor");
                Console.WriteLine("8. Delete a doctor (and save deleted record to history)");
                Console.WriteLine("9. Search for doctors by specialization");
                Console.WriteLine("10. View all doctors");
                Console.WriteLine("11. Book an appointment");
                Console.WriteLine("12. Update an appointment");
                Console.WriteLine("13. View all appointments");
                Console.WriteLine("14. Search appointments by doctor or patient");
                Console.WriteLine("15. Cancel an appointment (and save deleted appointment to history)");
                Console.WriteLine("16. View history of deleted records (patients, doctors, or appointments)");
                Console.WriteLine("17. Exit the application");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        da.InsertPatient(p);
                        break;

                    case "2":
                        da.UpdatePatientInDatabase(p);
                        break;

                    case "3":
                        Console.WriteLine("Enter ID to be deleted");
                        int id = int.Parse(Console.ReadLine());
                        da.DeletePatientFromDatabase(id);
                        break;

                    case "4":
                        Console.WriteLine("Enter patient's name you want to search");
                        string name = Console.ReadLine();
                        da.SearchPatientsInDatabase(name);
                        break;

                    case "5":
                        ViewAllPatients();
                        break;

                    case "6":
                        da.InsertDoctor(d);
                        break;

                    case "7":
                        
                        da.UpdateDoctorInDatabase(d);
                        break;

                    case "8":
                        Console.WriteLine("Enter doctors Id you want to delete");
                        int docId = int.Parse(Console.ReadLine());
                        da.DeleteDoctorFromDatabase(docId);
                        break;

                    case "9":
                        Console.WriteLine("Enter doctors name to be searched");
                        string docName = Console.ReadLine();
                        da.SearchDoctorsInDatabase(docName);
                        break;

                    case "10":
                        ViewAllDoctors();
                        break;

                    case "11":
                        da.InsertAppointment(a);
                        break;

                    case "12":
                        da.UpdateAppointmentInDatabase(a);
                        break;

                    case "13":
                        ViewAllAppointments();
                        break;

                    case "14":
                        Console.WriteLine("Enter the patient id whose appointment you want to find out");
                        int pId = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter the doctor id whose appointment you want to find out");
                        int dId = int.Parse(Console.ReadLine());
                        da.SearchAppointmentsInDatabase(pId, dId);
                        break;

                    case "15":
                        Console.WriteLine("Enter aqppointment Id you want to delete");
                        int appId = int.Parse(Console.ReadLine());
                        da.DeleteAppointmentFromDatabase(appId);
                        break;

                    case "16":
                        ViewHistory();
                        break;

                    case "17":
                        Console.WriteLine("Exiting the application. Goodbye!");
                        return;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }
}
    

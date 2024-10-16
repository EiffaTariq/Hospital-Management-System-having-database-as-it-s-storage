using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace HospitalDAL
{
    public class Appointment
    {
        public int appointmentId;
        public int patientId;
        public int doctorId;
        public DateTime appointmentDate;
        public int AppointmentId
        {
            get { return appointmentId; }
            set { appointmentId = value; }
        }
        public int PatientId
        {
            get { return patientId; }
            set { patientId = value; }
        }
        public int DoctorId
        {
            get { return doctorId; }
            set { doctorId = value; }
        }
        public DateTime AppointmentDate
        {
            get { return appointmentDate; }
            set { appointmentDate = value; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalDAL
{
    public class Doctor
    {
        public int doctorId;
        public string doctorName;
        public string doctorSpecialization;
        public int DoctorId
        {
            get { return doctorId; }
            set { doctorId = value; }
        }
        public string DoctorName
        {
            get { return doctorName; }
            set { doctorName = value; }
        }
        public string DoctorSpecialization
        {
            get { return doctorSpecialization; }
            set { doctorSpecialization = value; }
        }
    }
}

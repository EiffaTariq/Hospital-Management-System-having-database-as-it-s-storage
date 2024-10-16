//new project
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace HospitalDAL
{
    public class Patient
    {
        public int patientId;
        public string name;
        public string email;
        public string disease;

        public int PatientId
        {
            get { return patientId; }
            set { patientId = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string Disease
        {
            get { return disease; }
            set { disease = value; }
        }
    }
}

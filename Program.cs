using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.IO;
using HospitalDAL;
using System.Numerics;
using HospitalDAL;
namespace HospitalDAL
{
    public class Program
    {
        static void Main(string[] args)
        {
            Menu menu = new Menu();
            menu.UserMenu();
        }            
    }
}
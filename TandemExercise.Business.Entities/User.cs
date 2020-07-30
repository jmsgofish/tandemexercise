using System;
using System.Collections.Generic;
using System.Text;

namespace TandemExercise.Business.Entities
{
    public class User : EntityBase
    {
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string phoneNumber { get; set; }
        public string emailAddress { get; set; }
    }
}

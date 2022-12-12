using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ModilistPortal.Infrastructure.Shared.Enums;

namespace ModilistPortal.Business.Seed.Data.DataModels
{
    internal class UserData
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public Gender Gender { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string InstagramUserName { get; set; }

        public string JobTitle { get; set; }
    }
}

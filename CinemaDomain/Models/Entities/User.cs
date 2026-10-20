using CinemaDomain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Models.Entities
{
    public class User
    {
        private string _userName;
        private string _email;
        private string _passwordHash;
        private Role _role;
        public int Id { get; set; }
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("UserName cannot be null or empty.");
                }
                _userName = value;
            }
        }
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Email cannot be null or empty.");
                if (!value.Contains('@') || !value.Contains('.'))
                    throw new ArgumentException("Email must contain '@' and '.' characters.");
                
                _email = value;
            }
        }
        public string PasswordHash
        {
            get
            {
                return _passwordHash;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("PasswordHash cannot be null or empty.");
                }
                _passwordHash = value;
            }
        }
        public Role Role
        {
            get
            {
                return _role;
            }
            set
            {
                if (!Enum.IsDefined(typeof(Role), value))
                    throw new ArgumentException("Invalid user role.");
                _role = value;
            }
        }
    }
}

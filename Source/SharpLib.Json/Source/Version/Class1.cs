using System;
using System.Collections.Generic;

namespace SharpLib.Json
{
    internal class Class1
    {
        #region Методы

        internal void Func()
        {
            Account account = new Account
            {
                Email = "james@example.com",
                Active = true,
                CreatedDate = new DateTime(2013, 1, 20, 0, 0, 0, DateTimeKind.Utc),
                Roles = new List<string>
                {
                    "User",
                    "Admin"
                }
            };

            string json = JsonConvert.SerializeObject(account, Formatting.Indented);
        }

        #endregion
    }

    public class Account
    {
        #region Свойства

        public string Email { get; set; }

        public bool Active { get; set; }

        public DateTime CreatedDate { get; set; }

        public IList<string> Roles { get; set; }

        #endregion
    }
}
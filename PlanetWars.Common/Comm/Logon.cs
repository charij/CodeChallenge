namespace PlanetWars.Common.Comm
{
    using System;

    public class Logon
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Logon(string[] args)
        {
            var id = args?[0];
            if (id == null)
            {
                id = Environment.GetEnvironmentVariable("User_Id", EnvironmentVariableTarget.User);
                Environment.SetEnvironmentVariable("User_Id", (Id = Guid.NewGuid()).ToString(), EnvironmentVariableTarget.User);
            }

            Name = args?[1];
            if (Name == null)
            {
                Name = Environment.GetEnvironmentVariable("User_Name", EnvironmentVariableTarget.User) ?? "Anonymous";
            }
            else
            {
                Environment.SetEnvironmentVariable("User_Name", Name, EnvironmentVariableTarget.User);
            }
        }
    }
}
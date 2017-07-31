using AF_Market.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AF_Market.ModelView
{
    public class UserView
    {
        public string UserID { get; set; }

        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        public string EMail { get; set; }

        // IMP! Usado en AddRole 
        // Almaceno temporalmente el rol que el usuario escogio pero que todavia no se ha almacenado
        public RoleView  Role { get; set; }

        // Un usuario puede tener varios roles
        public List<RoleView> Roles { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AF_Market.Models
{
   // AFRAGO: Nombre para la tabla [Table="hh"]
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }

        [Display(Name= "FirstName")]
        [Required(ErrorMessage = "You must enter {0}")]
        [StringLength (30, ErrorMessage = "The field {0} must be between {1} and {2} characters", MinimumLength = 3)]
        [Column("Name")]
        public string FirstName { get; set; }
        [Display(Name = "LastName")]
        [Required(ErrorMessage = "You must enter {0}")]
        [StringLength(30, ErrorMessage = "The field {0} must be between {1} and {2} characters", MinimumLength = 3)]
        public string LastName { get; set; }
        [Required(ErrorMessage = "You must enter {0}")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Salary { get; set; }
        [Display(Name = "Bonus Percent")]
        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = false)]
        public float BonusPercent { get; set; }
        [Display(Name = "Date Of Birth")]
        [Required(ErrorMessage = "You must enter {0}")]
        [DataType (DataType.Date )]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }
        [Display(Name = "Start Time")]
        [Required(ErrorMessage = "You must enter {0}")]
        [DisplayFormat(DataFormatString = "{0:hh:mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time )]
        public DateTime StartTime { get; set; }
        [DataType(DataType.EmailAddress)]
        public string EMail { get; set; }
        [DataType(DataType.Url)]
        public string URL { get; set; }
        [Display(Name ="Document")]
        [Required(ErrorMessage = "You must enter {0}")] 
        // Es recomendable poner siempre el mismo nombre, pero de si no [ForeignKey("")]
        public int DocumentTypeID { get; set; }
        // AFRAGO:  Propiedad calculada que no se genera en la base de datos
        [NotMapped]
        public int Age { get { return DateTime.Now.Year  - DateOfBirth.Year  ; } }


        // AFRAGO: esto permite borrar registros en cascada, lo cual se tiene que solucionar.
        public virtual DocumentType DocumentType { get; set; }


        // *AFRAGO Para evitar la eliminación en cascada se modifica el AF_MarketContext
       // modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
    }
}
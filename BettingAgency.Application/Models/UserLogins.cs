using System.ComponentModel.DataAnnotations;

namespace BettingAgency.Application.Models {
    public class UserLogins {
        [Required]
        public string UserName {
            get;
            set;
        }
        [Required]
        public string Password {
            get;
            set;
        }
        public UserLogins() {}
    }
}
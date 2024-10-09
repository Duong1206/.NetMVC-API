using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanSach.Model.ViewModel
{
    public class ContactListViewModel
    {
        public IEnumerable<Contact> Contacts { get; set; } = Enumerable.Empty<Contact>();

    }
}

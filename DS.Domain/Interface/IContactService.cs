using System.Collections.Generic;

namespace DS.Domain.Interface
{
    public interface IContactService
    {
        IList<Contact> GetAll();
        GenericResult Create(Contact contact);
    }
}
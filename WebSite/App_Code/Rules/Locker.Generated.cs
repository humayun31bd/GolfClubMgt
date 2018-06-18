using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using MyCompany.Data;

namespace MyCompany.Rules
{
	public partial class LockerBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("Locker", RowKind.New)]
        public void BuildNewLocker()
        {
            UpdateFieldValue("IsFree", 1);
        }
    }
}

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
	public partial class CaddieBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("Caddie", RowKind.New)]
        public void BuildNewCaddie()
        {
            UpdateFieldValue("IsActive", 1);
        }
    }
}

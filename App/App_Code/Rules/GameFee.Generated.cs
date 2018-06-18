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
	public partial class GameFeeBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("GameFee", RowKind.New)]
        public void BuildNewGameFee()
        {
            UpdateFieldValue("IsActive", 1);
        }
    }
}

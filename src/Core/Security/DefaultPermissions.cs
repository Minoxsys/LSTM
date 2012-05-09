﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Security
{
    public class DefaultPermissions
    {
        public const string Employee = "Employee";
        public const string Manager = "Manager";
        public const string Lead = "Lead";

        public const string Any = "Lead, Manager, Employee";

    }
}

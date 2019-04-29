
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using STOApi.Models;

namespace STOApi.Models
{
    public class DbInitializer
    {
        public static void Initialize(STOContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
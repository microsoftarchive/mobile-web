/*  
Copyright Microsoft Corporation

Licensed under the Apache License, Version 2.0 (the "License"); you may not
use this file except in compliance with the License. You may obtain a copy of
the License at 

http://www.apache.org/licenses/LICENSE-2.0 

THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED 
ARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, 
MERCHANTABLITY OR NON-INFRINGEMENT. 

See the Apache 2 License for the specific language governing permissions and
limitations under the License. */

using System;

namespace MileageStats.Web.Tests.Controllers
{
    static class YearsMakesAndModelsFor
    {
        public static Tuple<int[], string[], string[]> YearWithoutMakes(int year)
        {
            return new Tuple<int[], string[], string[]>(new int[] { year }, new string[] { }, new string[] { });
        }

        public static Tuple<int[], string[], string[]> YearWithMakes(int year, params string[] makes)
        {
            return new Tuple<int[], string[], string[]>(new int[] { year }, makes, new string[] { });
        }

        public static Tuple<int[], string[], string[]> MakeWithModels(int year, string makeSelected, params string[] models)
        {
            return new Tuple<int[], string[], string[]>(new int[] { year }, new string[] { makeSelected }, models);
        }
    }
}
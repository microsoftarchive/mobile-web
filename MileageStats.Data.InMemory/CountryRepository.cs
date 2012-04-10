/*  
Copyright Microsoft Corporation

Licensed under the Apache License, Version 2.0 (the "License"); you may not
use this file except in compliance with the License. You may obtain a copy of
the License at 

http://www.apache.org/licenses/LICENSE-2.0 

THIS CODE IS PROVIDED ON AN *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED 
WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, 
MERCHANTABLITY OR NON-INFRINGEMENT. 

See the Apache 2 License for the specific language governing permissions and
limitations under the License. */

using System.Collections.Generic;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Models;

namespace MileageStats.Data.InMemory
{
    public class CountryRepository : ICountryRepository
    {
        private static readonly IList<Country> countries = new List<Country>(); 

        static CountryRepository()
        {
            countries.Add(new Country { Name = "Afghanistan" });
            countries.Add(new Country { Name = "Albania" });
            countries.Add(new Country { Name = "Algeria" });
            countries.Add(new Country { Name = "American Samoa" });
            countries.Add(new Country { Name = "Andorra" });
            countries.Add(new Country { Name = "Angola" });
            countries.Add(new Country { Name = "Anguilla" });
            countries.Add(new Country { Name = "Antarctica" });
            countries.Add(new Country { Name = "Antigua and Barbuda" });
            countries.Add(new Country { Name = "Argentina" });
            countries.Add(new Country { Name = "Armenia" });
            countries.Add(new Country { Name = "Aruba" });
            countries.Add(new Country { Name = "Australia" });
            countries.Add(new Country { Name = "Austria" });
            countries.Add(new Country { Name = "Azerbaijan" });
            countries.Add(new Country { Name = "Bahamas, The" });
            countries.Add(new Country { Name = "Bahrain" });
            countries.Add(new Country { Name = "Bangladesh" });
            countries.Add(new Country { Name = "Barbados" });
            countries.Add(new Country { Name = "Belarus" });
            countries.Add(new Country { Name = "Belgium" });
            countries.Add(new Country { Name = "Belize" });
            countries.Add(new Country { Name = "Benin" });
            countries.Add(new Country { Name = "Bermuda" });
            countries.Add(new Country { Name = "Bhutan" });
            countries.Add(new Country { Name = "Bolivia" });
            countries.Add(new Country { Name = "Bosnia and Herzegovina" });
            countries.Add(new Country { Name = "Botswana" });
            countries.Add(new Country { Name = "Bouvet Island" });
            countries.Add(new Country { Name = "Brazil" });
            countries.Add(new Country { Name = "British Indian Ocean Territory" });
            countries.Add(new Country { Name = "Brunei" });
            countries.Add(new Country { Name = "Bulgaria" });
            countries.Add(new Country { Name = "Burkina Faso" });
            countries.Add(new Country { Name = "Burundi" });
            countries.Add(new Country { Name = "Cambodia" });
            countries.Add(new Country { Name = "Cameroon" });
            countries.Add(new Country { Name = "Canada" });
            countries.Add(new Country { Name = "Cape Verde" });
            countries.Add(new Country { Name = "Cayman Islands" });
            countries.Add(new Country { Name = "Central African Republic" });
            countries.Add(new Country { Name = "Chad" });
            countries.Add(new Country { Name = "Chile" });
            countries.Add(new Country { Name = "China" });
            countries.Add(new Country { Name = "Christmas Island" });
            countries.Add(new Country { Name = "Cocos Islands" });
            countries.Add(new Country { Name = "Colombia" });
            countries.Add(new Country { Name = "Comoros" });
            countries.Add(new Country { Name = "Congo" });
            countries.Add(new Country { Name = "Cook Islands" });
            countries.Add(new Country { Name = "Costa Rica" });
            countries.Add(new Country { Name = "Cote d'Ivoire" });
            countries.Add(new Country { Name = "Croatia" });
            countries.Add(new Country { Name = "Cyprus" });
            countries.Add(new Country { Name = "Czech Republic" });
            countries.Add(new Country { Name = "Denmark" });
            countries.Add(new Country { Name = "Djibouti" });
            countries.Add(new Country { Name = "Dominica" });
            countries.Add(new Country { Name = "Dominican Republic" });
            countries.Add(new Country { Name = "Ecuador" });
            countries.Add(new Country { Name = "Egypt" });
            countries.Add(new Country { Name = "El Salvador" });
            countries.Add(new Country { Name = "Equatorial Guinea" });
            countries.Add(new Country { Name = "Eritrea" });
            countries.Add(new Country { Name = "Estonia" });
            countries.Add(new Country { Name = "Ethiopia" });
            countries.Add(new Country { Name = "Falkland Islands" });
            countries.Add(new Country { Name = "Faroe Islands" });
            countries.Add(new Country { Name = "Fiji" });
            countries.Add(new Country { Name = "Finland" });
            countries.Add(new Country { Name = "France" });
            countries.Add(new Country { Name = "French Guiana" });
            countries.Add(new Country { Name = "French Polynesia" });
            countries.Add(new Country { Name = "French Southern and Antarctic Lands" });
            countries.Add(new Country { Name = "Gabon" });
            countries.Add(new Country { Name = "Gambia, The" });
            countries.Add(new Country { Name = "Georgia" });
            countries.Add(new Country { Name = "Germany" });
            countries.Add(new Country { Name = "Ghana" });
            countries.Add(new Country { Name = "Gibraltar" });
            countries.Add(new Country { Name = "Greece" });
            countries.Add(new Country { Name = "Greenland" });
            countries.Add(new Country { Name = "Grenada" });
            countries.Add(new Country { Name = "Guadeloupe" });
            countries.Add(new Country { Name = "Guam" });
            countries.Add(new Country { Name = "Guatemala" });
            countries.Add(new Country { Name = "Guernsey" });
            countries.Add(new Country { Name = "Guinea" });
            countries.Add(new Country { Name = "Guinea-Bissau" });
            countries.Add(new Country { Name = "Guyana" });
            countries.Add(new Country { Name = "Haiti" });
            countries.Add(new Country { Name = "Heard Island and McDonald Islands" });
            countries.Add(new Country { Name = "Honduras" });
            countries.Add(new Country { Name = "Hong Kong SAR" });
            countries.Add(new Country { Name = "Hungary" });
            countries.Add(new Country { Name = "Iceland" });
            countries.Add(new Country { Name = "India" });
            countries.Add(new Country { Name = "Indonesia" });
            countries.Add(new Country { Name = "Iraq" });
            countries.Add(new Country { Name = "Ireland" });
            countries.Add(new Country { Name = "Isle of Man" });
            countries.Add(new Country { Name = "Israel" });
            countries.Add(new Country { Name = "Italy" });
            countries.Add(new Country { Name = "Jamaica" });
            countries.Add(new Country { Name = "Japan" });
            countries.Add(new Country { Name = "Jersey" });
            countries.Add(new Country { Name = "Jordan" });
            countries.Add(new Country { Name = "Kazakhstan" });
            countries.Add(new Country { Name = "Kenya" });
            countries.Add(new Country { Name = "Kiribati" });
            countries.Add(new Country { Name = "Korea" });
            countries.Add(new Country { Name = "Kuwait" });
            countries.Add(new Country { Name = "Kyrgyzstan" });
            countries.Add(new Country { Name = "Laos" });
            countries.Add(new Country { Name = "Latvia" });
            countries.Add(new Country { Name = "Lebanon" });
            countries.Add(new Country { Name = "Lesotho" });
            countries.Add(new Country { Name = "Liberia" });
            countries.Add(new Country { Name = "Libya" });
            countries.Add(new Country { Name = "Liechtenstein" });
            countries.Add(new Country { Name = "Lithuania" });
            countries.Add(new Country { Name = "Luxembourg" });
            countries.Add(new Country { Name = "Macao SAR" });
            countries.Add(new Country { Name = "Macedonia, Former Yugoslav Republic of" });
            countries.Add(new Country { Name = "Madagascar" });
            countries.Add(new Country { Name = "Malawi" });
            countries.Add(new Country { Name = "Malaysia" });
            countries.Add(new Country { Name = "Maldives" });
            countries.Add(new Country { Name = "Mali" });
            countries.Add(new Country { Name = "Malta" });
            countries.Add(new Country { Name = "Marshall Islands" });
            countries.Add(new Country { Name = "Martinique" });
            countries.Add(new Country { Name = "Mauritania" });
            countries.Add(new Country { Name = "Mauritius" });
            countries.Add(new Country { Name = "Mayotte" });
            countries.Add(new Country { Name = "Mexico" });
            countries.Add(new Country { Name = "Micronesia" });
            countries.Add(new Country { Name = "Moldova" });
            countries.Add(new Country { Name = "Monaco" });
            countries.Add(new Country { Name = "Mongolia" });
            countries.Add(new Country { Name = "Montenegro" });
            countries.Add(new Country { Name = "Montserrat" });
            countries.Add(new Country { Name = "Morocco" });
            countries.Add(new Country { Name = "Mozambique" });
            countries.Add(new Country { Name = "Myanmar" });
            countries.Add(new Country { Name = "Namibia" });
            countries.Add(new Country { Name = "Nauru" });
            countries.Add(new Country { Name = "Nepal" });
            countries.Add(new Country { Name = "Netherlands" });
            countries.Add(new Country { Name = "Netherlands Antilles" });
            countries.Add(new Country { Name = "New Caledonia" });
            countries.Add(new Country { Name = "New Zealand" });
            countries.Add(new Country { Name = "Nicaragua" });
            countries.Add(new Country { Name = "Niger" });
            countries.Add(new Country { Name = "Nigeria" });
            countries.Add(new Country { Name = "Niue" });
            countries.Add(new Country { Name = "Norfolk Island" });
            countries.Add(new Country { Name = "Northern Mariana Islands" });
            countries.Add(new Country { Name = "Norway" });
            countries.Add(new Country { Name = "Oman" });
            countries.Add(new Country { Name = "Pakistan" });
            countries.Add(new Country { Name = "Palau" });
            countries.Add(new Country { Name = "Palestinian Authority" });
            countries.Add(new Country { Name = "Panama" });
            countries.Add(new Country { Name = "Papua New Guinea" });
            countries.Add(new Country { Name = "Paraguay" });
            countries.Add(new Country { Name = "Peru" });
            countries.Add(new Country { Name = "Philippines" });
            countries.Add(new Country { Name = "Pitcairn Islands" });
            countries.Add(new Country { Name = "Poland" });
            countries.Add(new Country { Name = "Portugal" });
            countries.Add(new Country { Name = "Puerto Rico" });
            countries.Add(new Country { Name = "Qatar" });
            countries.Add(new Country { Name = "Reunion" });
            countries.Add(new Country { Name = "Romania" });
            countries.Add(new Country { Name = "Russia" });
            countries.Add(new Country { Name = "Rwanda" });
            countries.Add(new Country { Name = "Saint Helena" });
            countries.Add(new Country { Name = "Saint Kitts and Nevis" });
            countries.Add(new Country { Name = "Saint Lucia" });
            countries.Add(new Country { Name = "Saint Pierre and Miquelon" });
            countries.Add(new Country { Name = "Saint Vincent and the Grenadines" });
            countries.Add(new Country { Name = "Samoa" });
            countries.Add(new Country { Name = "San Marino" });
            countries.Add(new Country { Name = "Sao Tome and Principe" });
            countries.Add(new Country { Name = "Saudi Arabia" });
            countries.Add(new Country { Name = "Senegal" });
            countries.Add(new Country { Name = "Serbia" });
            countries.Add(new Country { Name = "Seychelles" });
            countries.Add(new Country { Name = "Sierra Leone" });
            countries.Add(new Country { Name = "Singapore" });
            countries.Add(new Country { Name = "Slovakia" });
            countries.Add(new Country { Name = "Slovenia" });
            countries.Add(new Country { Name = "Solomon Islands" });
            countries.Add(new Country { Name = "Somalia" });
            countries.Add(new Country { Name = "South Africa" });
            countries.Add(new Country { Name = "South Georgia and the South Sandwich Islands" });
            countries.Add(new Country { Name = "Spain" });
            countries.Add(new Country { Name = "Sri Lanka" });
            countries.Add(new Country { Name = "Suriname" });
            countries.Add(new Country { Name = "Svalbard" });
            countries.Add(new Country { Name = "Swaziland" });
            countries.Add(new Country { Name = "Sweden" });
            countries.Add(new Country { Name = "Switzerland" });
            countries.Add(new Country { Name = "Taiwan" });
            countries.Add(new Country { Name = "Tajikistan" });
            countries.Add(new Country { Name = "Tanzania" });
            countries.Add(new Country { Name = "Thailand" });
            countries.Add(new Country { Name = "Timor-Leste" });
            countries.Add(new Country { Name = "Togo" });
            countries.Add(new Country { Name = "Tokelau" });
            countries.Add(new Country { Name = "Tonga" });
            countries.Add(new Country { Name = "Trinidad and Tobago" });
            countries.Add(new Country { Name = "Tunisia" });
            countries.Add(new Country { Name = "Turkey" });
            countries.Add(new Country { Name = "Turkmenistan" });
            countries.Add(new Country { Name = "Turks and Caicos Islands" });
            countries.Add(new Country { Name = "Tuvalu" });
            countries.Add(new Country { Name = "U.S. Minor Outlying Islands" });
            countries.Add(new Country { Name = "Uganda" });
            countries.Add(new Country { Name = "Ukraine" });
            countries.Add(new Country { Name = "United Arab Emirates" });
            countries.Add(new Country { Name = "United Kingdom" });
            countries.Add(new Country { Name = "United States" });
            countries.Add(new Country { Name = "Uruguay" });
            countries.Add(new Country { Name = "Uzbekistan" });
            countries.Add(new Country { Name = "Vanuatu" });
            countries.Add(new Country { Name = "Holy See" });
            countries.Add(new Country { Name = "Venezuela" });
            countries.Add(new Country { Name = "Vietnam" });
            countries.Add(new Country { Name = "Virgin Islands, British" });
            countries.Add(new Country { Name = "Virgin Islands" });
            countries.Add(new Country { Name = "Wallis and Futuna" });
            countries.Add(new Country { Name = "Yemen" });
            countries.Add(new Country { Name = "Zambia" });
            countries.Add(new Country { Name = "Zimbabwe" });
            countries.Add(new Country { Name = "Saint Barthelemy" });
            countries.Add(new Country { Name = "Saint Martin" });
        }

        public IEnumerable<Country> GetAll()
        {
            return countries;
        }
    }
}
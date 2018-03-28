using System;
using System.Collections.Generic;
using DM.Models.System.Security;
using Tools.Utilities;

namespace DM.Models.Setup
{
    public class Entity
    {
        public long Pin { get; set; }
        public string Title { get; set; }
        public string Gender { get; set; }
        public string FirstName { get; set; }
        public string FatherName { get; set; }
        public string FamilyName { get; set; }
        public string MotherName { get; set; }
        public string MaidenName { get; set; }
        public string ArFirstName { get; set; }
        public string ArFatherName { get; set; }
        public string ArFamilyName { get; set; }
        public string ArMotherName { get; set; }
        public string ArMaidenName { get; set; }
        public string FullEnLongName
        {
            get
            {
                return Gender == "F" || Gender == "M"
                    ? FirstName + (CheckEmpty.String(FatherName) == "" ? "" : " ") + FatherName + (CheckEmpty.String(FamilyName) == "" ? "" : " ") + FamilyName
                    : FirstName;
            }
        }
        public string FullEnShortName
        {
            get
            {
                return Gender == "F" || Gender == "M" ?
                  FirstName + (CheckEmpty.String(FamilyName) == "" ? "" : " ") + FamilyName :
                  FirstName;
            }
        }
        public string FullArLongName
        {
            get
            {
                return Gender == "F" || Gender == "M" ?
                  ArFirstName + (CheckEmpty.String(ArFatherName) == "" ? "" : " ") + ArFatherName + (CheckEmpty.String(ArFamilyName) == "" ? "" : " ") + ArFamilyName
                  : ArFirstName;
            }
        }
        public string FullArShortName
        {
            get
            {
                return Gender == "F" || Gender == "M" ?
                  (ArFirstName + (CheckEmpty.String(ArFamilyName) == "" ? "" : " ") + ArFamilyName) :
                  ArFirstName;
            }
        }

        public DateTime? Dob { get; set; }
        public string Pob { get; set; }
        public string Marital { get; set; }
        public string Language { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string WebAddress { get; set; }
        public string Education { get; set; }
        public string IdType { get; set; }
        public string IdNum { get; set; }
        public int Nationality { get; set; }
        public string NameIndex { get; set; }
        public string RegNum { get; set; }
        public string Notes { get; set; }
        public string ExtraData { get; set; }
        public DateTime EntryDate { get; set; }
        //public User User { get; set; }
        public long UserId { get; set; }
        public string Status { get; set; }
        public long BranchId { get; set; }
        public IEnumerable<Address> Addresses { get; set; }
        public byte[] Version { get; set; }
    }
}